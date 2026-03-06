# Bomb System

Bombs are special dots (`DotType.Bomb`) that are usually **created at runtime** from big squares and resolved by the cascade system.  
This document covers how bombs are **pooled**, how the **big‑square preview/commit** flow works, and how bombs **explode and integrate into cascades**.

---

## Bomb Pooling

### Components

- **`BombPool` (`Assets/Scripts/Pooling/BombPool.cs`)**
  - Inherits from `Pool`.
  - Holds a `Queue<BombPoolObject>` backing the pool.
  - On `Fill(size)`:
    - Builds a `DotsObject` with `DotType.Bomb` for each entry.
    - Uses `BoardPresenter.CreateDotPresenter` to create a bomb `IDotPresenter` and view.
    - Wraps the presenter in a `BombPoolObject` GameObject and parents the bomb view under it.
    - Enqueues the `BombPoolObject` and deactivates it.
- **`BombPoolObject` (`Assets/Scripts/Pooling/BombPoolObject.cs`)**
  - MonoBehaviour wrapper that exposes a pooled `IDotPresenter` via `Presenter`.
- **`PoolService` (`Assets/Scripts/Pooling/PoolService.cs`)**
  - Singleton that registers `BombPool` (and other pools) on `Awake`.
  - APIs:
    - `FillPool<BombPool>(size)` – pre‑allocates bomb entries.
    - `GetFromPool<BombPool, BombPoolObject>()` – returns an activated `BombPoolObject`.
    - `ReturnToPool<BombPool>(item)` – deactivates and re‑queues a bomb object.
- **`BoardView.ReleaseDotView` (`Assets/Scripts/Gameplay/Board/Views/BoardView.cs`)**
  - When releasing a dot view:
    - If it is a `BombView`, finds its `BombPoolObject` parent and returns it to `BombPool` instead of destroying it.
    - Non‑bomb dot views are destroyed normally.

### Pool lifecycle

- **Initialization**
  - `PoolService` locates the scene’s `BombPool` and registers it.
  - A bootstrapper (e.g. game/level setup) calls `PoolService.FillPool<BombPool>(size)` to prepare enough bombs for previews and runtime usage.
- **Get**
  - `BombPool.Get<BombPoolObject>` dequeues a pooled entry, activates it, and returns it for immediate use.
- **Return**
  - `BombPool.Return` enqueues the `BombPoolObject` and deactivates it.
  - `BoardView.ReleaseDotView` integrates with this so bomb views are recycled instead of destroyed.

---

## Big Squares and Bomb Lifecycle

A **big square** is a closed connection with **8+ dots**. When detected, the `Square` helper:

- Finds the **border path** that forms the loop.
- Flood‑fills from the board edges to classify dots as **inside** vs **outside** the loop.
- Builds `DotIdsInSquare` containing all **interior dots** (not on the border, not on the board edge, and not part of the connection path).

### Preview phase (during drag)

Handled by `Square.Activate()` / `ActivateBombsInsideSquare()`:

- **Determine interior dots**
  - `FindDotIdsInsideSquare` returns all interior dot IDs; these are stored in `DotIdsInSquare`.
- **Visual replacement**
  - For each `dotId` in `DotIdsInSquare`:
    - Fetch the original `IDotPresenter` from the board.
    - **Hide** the original dot’s view (`dot.View.gameObject.SetActive(false)`).
    - **Get a bomb from the pool** via `PoolService.Instance.GetFromPool<BombPool, BombPoolObject>()`.
    - Position the bomb’s view at the original dot’s world position.
    - Call `bombPoolObject.Presenter.Spawn()` to play a spawn animation (preview only).
    - Store the mapping `PreviewBombs[dotId] = bombPoolObject`.
- **Important**
  - During preview, the **board model still contains the original dots**; only the visuals are replaced.

### Cancel / backtrack (preview rollback)

If the player backtracks out of the closed loop or cancels the connection, `Square.Deactivate()` calls `DeactivateBombsInsideSquare()`:

- For each `dotId` in `DotIdsInSquare`:
  - Restore the original dot’s view (`dot.View.gameObject.SetActive(true)`).
  - If a preview bomb exists in `PreviewBombs`:
    - Hide its view.
    - Return the `BombPoolObject` to `BombPool` via `PoolService.ReturnToPool<BombPool>(bombPoolObject)`.
- Clear `DotIdsInSquare` and `PreviewBombs`.
- Result: the board is visually and logically restored to its pre‑square state.

### Commit phase (turn previews into real bombs)

When the player releases the pointer and the connection ends, `ConnectionModel.End()` commits the square.  
`Square.Commit()` promotes preview bombs into **real bomb dots** on the board:

- For each `dotId` in `DotIdsInSquare`:
  - Fetch the original dot presenter from the board.
  - Get the corresponding bomb presenter from `PreviewBombs[dotId].Presenter`.
  - Call `_board.ReplaceDot(oldDot, bomb)`:
    - Updates the model to use the bomb dot instead of the original.
    - Releases the old dot view (`BoardView.ReleaseDotView`), destroying or pooling it as appropriate.
    - Moves the bomb’s view to the original position and sets the correct `GridPosition`.
    - Re‑registers the presenter under the bomb’s dot ID.
- After commit:
  - All interior dots are now **true `DotType.Bomb` dots** in the board model.
  - These bombs will be seen by cascade producers (notably `BombProducer`) in subsequent cascade cycles.

---

## Bomb Explosion and Animation

### Bomb presenters and visuals

- **`BombView` (`Assets/Scripts/Gameplay/Dots/Views/Bomb/BombView.cs`)**
  - Inherits from `DotView`.
  - Requires a `BombVisuals` component, which provides the `BombLine` prefab used for explosion lines.
  - `DoLineAnimation(IHittableDotPresenter dot, Action callback = null)`
    - Creates a `LineRenderer` from the bomb to the target dot.
    - Animates the line in two phases using DOTween:
      - **Phase 1 (extend)**: line head grows from just behind the bomb toward the target.
      - **Phase 2 (collapse)**: tail moves toward the target, narrowing the line.
    - Temporarily adjusts the target dot’s color via `DotRenderer.SetColor` for impact feedback.
    - Returns a `Sequence` that is joined into the bomb’s overall explosion sequence.
- **`BombVisuals` (`Assets/Scripts/Gameplay/Dots/Visuals/Bomb/BombVisuals.cs`)**
  - Simple container exposing `GameObject BombLine` for the line renderer prefab.

### Logical explosion (BombDotPresenter)

- **`BombDotPresenter` (`Assets/Scripts/Gameplay/Dots/Presenters/Bomb/BombDotPresenter.cs`)**
  - Implements `IExplodableDotPresenter`.
  - Coordinates which dots each bomb should visually and logically hit.
  - **Assignment of targets**
    - `PrepareForExplode(List<string> targetHittables, List<string> bombIds)`:
      - Calls `AssignHittablesToBombs`, which:
        - Filters out invalid targets (null or other bombs).
        - Builds a position lookup for each bomb via `BoardPresenter.GetDot(id)`.
        - For each valid hittable:
          - Finds its **nearest bomb** by distance in world space.
          - Adds the hittable’s ID to that bomb’s list in the static `bombToDotsMap`.
      - Sets `_explosionReady = true` so assignment only happens once.
  - **Explosion animation**
    - `Explode()`:
      - If `View` is a `BombView` and `bombToDotsMap` contains this bomb’s ID:
        - Builds a DOTween `Sequence`.
        - For each assigned target ID:
          - Attempts to get `IHittableDotPresenter` from the board.
          - Joins `bombView.DoLineAnimation(hittable)` into the sequence.
        - Returns the sequence for the cascade runner to await.

### Hit / clear integration

- The **cascade runner** drives bomb explosions via `FillStep`:
  - `FillStep.ToExplode` contains bomb IDs whose `Explode()` methods should be called for visuals.
  - `FillStep.ToHit` / `ToClear` contain the ids of dots that should be hit/cleared once the explosion finishes.
- In the **hit phase**, for a bomb step:
  - The runner:
    - Calls `Explode()` on each `IExplodableDotPresenter` in `ToExplode` and waits for the combined tween sequence to complete.
    - Calls `IHittableDotPresenter.Hit()` on all ids in `ToHit` (neighbors and the bomb itself) to increment hit counts / trigger state.
- In the **clear phase**:
  - The runner evaluates which of the hit dots should be removed (based on their `IHittableDot` / `IClearableDot` models).
  - Calls `BoardPresenter.TryClearDot` or `ClearDot` as appropriate.
  - Cleared bombs have their views released:
    - Non‑pooled views are destroyed.
    - Bomb views go back to `BombPool` via `BoardView.ReleaseDotView`.

---

## Bombs in the Cascade System

Bombs are integrated into the cascade via the **post‑fill** producer pipeline.

- **`BombProducer` (`Gameplay/Cascade/Producers/BombProducer.cs`)** (see Cascade System doc)
  - Phase: **PostFill**.
  - For each bomb dot currently on the board (including those created by a prior big square commit):
    - Finds its neighboring dots (board‑level neighbors).
    - Builds one or more `FillStep` instances that:
      - Mark bombs in `ToExplode` for line animations.
      - Mark neighboring dots (and the bomb itself) in `ToHit` / `ToClear` so they participate in the hit/clear pipeline.
- **Execution order**
  - After a connection:
    - `ConnectionClearProducer` and other **pre‑gravity** producers run.
    - Gravity and refill steps populate the board with new dots.
    - In **post‑fill**, `BombProducer` inspects the stabilized board and enqueues bomb steps.
  - Any bombs created by a big square will therefore:
    - Appear as normal dots in the board model after `Square.Commit()`.
    - Be consumed by `BombProducer` in a later cascade turn when their trigger conditions are met.

### Summary of bomb lifecycle

1. **Pool setup** – `BombPool` pre‑allocates bomb presenters and views.
2. **Big square preview** – `Square.Activate` hides interior dots and shows pooled bomb previews (`PreviewBombs`).
3. **Cancel** – `Square.Deactivate` restores original dots and returns bombs to the pool.
4. **Commit** – `Square.Commit` replaces interior dots with bomb dots via `BoardPresenter.ReplaceDot`.
5. **Cascade** – `BombProducer` finds bombs on the board and enqueues steps.
6. **Explosion** – `BombDotPresenter.Explode` and `BombView.DoLineAnimation` play line animations to assigned targets.
7. **Clear / recycle** – Cascade clears resolved bombs; `BoardView.ReleaseDotView` returns bomb views to `BombPool` for reuse.

