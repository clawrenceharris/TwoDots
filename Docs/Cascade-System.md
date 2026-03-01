# Cascade System (Fillsteps)

The cascade system runs after a connection is completed (pointer up). It clears the connected dots, runs pre-gravity reactions (e.g. seeds, hedgehogs), applies gravity and refill, then runs post-fill reactions (anchors, lotus, gems). The cycle repeats until the board is stable. Input is disabled for the duration via `CascadeState`.

## Responsibilities

- **Orchestration**: Run fill steps in phase order (pre-gravity → gravity → refill → post-fill) until no new work.
- **Input gating**: Transition to `CascadeState` at cascade start (input off) and back to the previous state at end (input on).
- **Determinism**: Steps are ordered by priority and sequence; board mutations and clears use a stable sort (e.g. row then column).
- **Extensibility**: New mechanics plug in as `IFillStepProducer` implementations; no change to the runner loop.

## Components

### CascadeRunner (`Gameplay/Cascade/CascadeRunner.cs`)

- MonoBehaviour that subscribes to `ConnectionPresenter.OnConnectionCompleted`.
- **EnsureInstance**: `[RuntimeInitializeOnLoadMethod]` creates a `CascadeRunner` in the scene if none exists.
- **StartCascade(payload)**: Validates payload (≥2 dot IDs), builds a `CascadeContext`, switches `LevelStateManager` to `CascadeState`, and starts the `RunCascade` coroutine.
- **RunCascade**: Loop: (1) enqueue steps from pre-gravity producers, (2) process pre-gravity queue until empty (each step can enqueue more), (3) collect and play gravity drops, (4) collect and play refill drops, (5) enqueue steps from post-fill producers, (6) process post-fill queue. Repeat until no work or `_maxCascadeIterations` is hit.
- **ExecuteStep**: Resolves presenters for step dot IDs (stable order), removes dots from board, runs clear animations, updates context recent clears and `ClearedDotIds`, returns `FillStepResult`.
- **Trace**: Optional `_enableTrace` logs each executed step (phase, type, count, source).
- **FinishCascade**: Restores `LevelStateManager` to the previous state and sets `_isRunning = false`.

### FillStep (`Gameplay/Cascade/FillStep.cs`)

- Immutable data for one cascade action: `Type`, `Priority`, `Phase`, `DotIds`, optional `Positions`, `Source` (debug), and `Sequence` (set by queue).
- Producers create steps; the runner executes them (clear those dots, wait for animations, then re-run producers for the same phase so new steps can be enqueued).

### FillStepResult (`Gameplay/Cascade/FillStepResult.cs`)

- Result of executing one step: `ClearedDotIds`, `ClearedPositions`, and `HasClears`.
- Runner uses it to call `context.SetRecentClears(...)` so adjacency-based producers (seed, hedgehog, gem) can react.

### CascadeContext (`Gameplay/Cascade/CascadeContext.cs`)

- Holds `Board`, `ConnectionPayload`, `TurnIndex`, `ChainIndex`, and `ClearedDotIds`.
- **TryConsumeConnectionPayload**: Returns the payload and marks it consumed (one-time; only `ConnectionClearProducer` should consume).
- **RecentClearedDotIds / RecentClearedPositions**: Set after each step execution; cleared when the phase queue is drained. Producers use these to find adjacent seeds, hedgehogs, gems, etc.
- **SetRecentClears / ClearRecentClears**: Called by the runner.

### IFillStepProducer (`Gameplay/Cascade/IFillStepProducer.cs`)

- **Phase**: `PreGravity` or `PostFill`.
- **CollectSteps(context, outSteps)**: Inspect context and append any applicable `FillStep` instances to `outSteps`. Called at the start of each phase (and again after each step execution within that phase, so chains like hedgehog→hedgehog can enqueue more steps before gravity).

### FillStepQueue (`Gameplay/Cascade/FillStepQueue.cs`)

- Priority queue: **Enqueue(step, ref sequence)** assigns a sequence number; **TryDequeue** returns the step with highest priority, then lowest sequence.
- **Clear** resets the queue. Used for both pre-gravity and post-fill phases.

### Enums

- **FillStepPhase**: PreGravity, PostFill.
- **FillStepType**: ConnectionClear, SeedClear, HedgehogCollision, AnchorSink, LotusClear, GemExplode, GravityDrop, RefillSpawn (latter two for tracing only).
- **FillStepPriority**: Low (0), Normal (100), High (200), VeryHigh (300).

## Producers

| Producer | Phase | Description |
| -------- | ----- | ----------- |
| **ConnectionClearProducer** | PreGravity | Consumes the connection payload once and enqueues one step to clear all path dots (VeryHigh priority). |
| **SeedAdjacencyProducer** | PreGravity | Finds seed dots adjacent (cardinal) to `RecentClearedPositions`; enqueues one step to clear them (High). Seeds clear with the connection, before gravity. |
| **HedgehogProducer** | PreGravity | Finds hedgehog dots adjacent to recent clears; enqueues one step (High). Multiple hedgehogs can be in one step so gravity waits for all. |
| **AnchorSinkProducer** | PostFill | Finds anchor dots where `IsAtBottomOfBoard` is true; enqueues one step to clear them (High). |
| **LotusProducer** | PostFill | For each lotus dot, finds cardinal neighbors with same color; if any, enqueues a step to clear lotus + those neighbors (Normal). |
| **GemProducer** | PostFill | Finds gem types (SquareGem, RectangleGem) adjacent to recent clears; enqueues one step to clear each gem and all of its neighbors (explosion) (High). |

## Data Flow

1. **Pointer up** → `ConnectionModel.End()` → `OnConnectionCompleted(payload)` → `CascadeRunner.HandleConnectionCompleted` → `StartCascade(payload)` (if payload has ≥2 dot IDs).
2. **StartCascade** → Create `CascadeContext(board, payload)` → `LevelStateManager.ChangeState(CascadeState)` → Start `RunCascade` coroutine.
3. **RunCascade loop**:
   - Pre-gravity: Enqueue from ConnectionClear, SeedAdjacency, Hedgehog producers. While queue not empty: dequeue step → ExecuteStep (clear dots, play animations) → SetRecentClears → wait for animations → enqueue again from same producers → repeat.
   - Gravity: `Board.CollectGravityDrops()` → apply model moves → play drop animations → wait for all drops.
   - Refill: `Board.CollectRefillDrops()` → spawn dots → play drop animations → wait.
   - Post-fill: Enqueue from AnchorSink, Lotus, Gem producers; process queue same way as pre-gravity.
   - If any work was done in the loop, repeat from pre-gravity; else exit.
4. **FinishCascade** → `LevelStateManager.ChangeState(_previousState)` → input enabled again.

## Integration

- **CascadeState**: On enter, `InputRouter.Gate.SetEnabled(false)`; on exit, `SetEnabled(true)`.
- **BoardPresenter**: `CollectPresenters(dotIds)` (stable sort by position), `CollectGravityDrops()`, `CollectRefillDrops()`; runner uses these for clearing and for gravity/refill animation.
- **ConnectionPresenter**: Only emits `OnConnectionCompleted`; it does not call the runner directly. The runner subscribes in OnEnable and unsubscribes in OnDisable.

## Adding a New Mechanic

1. Implement `IFillStepProducer`: set `Phase` (PreGravity or PostFill) and in `CollectSteps` read `context.Board`, `context.RecentClearedPositions` / `RecentClearedDotIds`, or `context.TryConsumeConnectionPayload` as needed; append `new FillStep(...)` to `outSteps`.
2. Register the producer in `CascadeRunner.BuildProducers()` in the appropriate list (`_preGravityProducers` or `_postFillProducers`).
3. Optionally add a new `FillStepType` and use it in your step for tracing.
