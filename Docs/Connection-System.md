# Connection System

The connection system handles drag-to-connect gameplay: the player draws a path through dots, and the system validates and displays it.

## Responsibilities

- **Session lifecycle**: Begin (first dot), append/backtrack/cycle-close, End (pointer up), Cancel (cleanup).
- **Path state**: Ordered list of dots, current color, and whether the path is closed (square).
- **Validation**: Pluggable rules decide if a dot can be added (adjacency, color, etc.).
- **Visuals**: A live drag line from the last dot to the pointer, plus locked segments between consecutive dots.

## Components

### ConnectionModel (`Gameplay/Connection/Model/ConnectionModel.cs`)

- Implements `IConnectionModel`.
- Holds: `Path` (read-only list of `IDotPresenter`), `IsSessionActive`, `CurrentColor`, `IsSquare`.
- Methods: `Begin(dot)`, `TryAppend(dot)`, `TryBacktrack(dot)`, `End()`, `Cancel()`, `UpdateColor()`.
- Events: `OnPathChanged`, `OnConnectionCompleted`, `OnColorChanged`, `OnDotAddedToPath`, `OnDotRemovedFromPath`.
- Uses an `IDotConnectionRule` and `IBoardPresenter` for validation and board queries.
- **UpdateColor** derives the connection color from the path (e.g. first dot’s color or blank).

### ConnectionPresenter (`Gameplay/Connection/Presenter/ConnectionPresenter.cs`)

- MonoBehaviour that wires input to the model and view.
- **Initialize(rule, board)**: Builds a `ConnectionModel` with the given rule and board; subscribes to model events.
- Subscribes to `InputRouter`: `OnDotSelected`, `OnDotConnected`, `OnDotSelectionEnded`, `OnPointerDragged`.
- On path change: updates segment visuals and drag line; notifies connectable dot presenters.
- Static events for UI/feedback: `OnDotSelected`, `OnDotDeselected`, `OnDotConnected`, `OnConnectionCompleted`, `OnPathChanged`, `OnColorChanged`.
- Uses `ConnectorLineView` instances from a pool for the drag line and for each locked segment.

### IDotConnectionRule (`Gameplay/Connection/Model/IDotConnectionRule.cs`)

- Single method: `CanConnect(fromDot, toDot, currentConnection, board)`.
- Implementations:
  - **BaseConnectionRule**: Composes default rules (e.g. adjacency + color).
  - **AdjacencyRule**: Only allows cardinal neighbours (no diagonals).
  - **ColorRule**: Ensures dots match the connection color and each other (uses `ColorableModel` / comparable color).

### ConnectionCompletedPayload (`Gameplay/Connection/ConnectionCompletedPayload.cs`)

- Immutable result of a connection session when the pointer is released.
- Properties: `DotIds` (ordered), `IsCycle` (path closed), `SegmentCount`.
- Consumed by future clear/score/objective logic.

### ConnectorLineView (`Views/Connection/ConnectorLineView.cs`)

- MonoBehaviour with a `LineRenderer`; used for both the drag line and locked segments.
- `SetPositions(fromWorld, toWorld)` to draw a segment.
- Subscribes to `ConnectionPresenter.OnColorChanged` to update line color from the current connection color.
- Width and world-space usage configured on the component/prefab.

## Data Flow

1. **Pointer down on dot** → `InputRouter.OnDotSelected` → `ConnectionPresenter` calls `_model.Begin(dot)` and notifies connectable presenter; static `OnDotSelected` for feedback.
2. **Pointer move over another dot** → `OnDotConnected` → `TryAppend(dot)`; if true, add segment view and notify connectable presenter.
3. **Pointer move (every frame)** → `OnPointerDragged(worldPos)` → update drag line from last dot to `worldPos`.
4. **Pointer up** → `OnDotSelectionEnded` → `_model.End()` → `OnConnectionCompleted(payload)`; cleanup drag line and segments.
5. **Path change** → `_model.OnPathChanged` → presenter updates segment list and calls `_model.UpdateColor()`; `ConnectorLineView` instances react to `OnColorChanged`.

## Level Setup

`LevelManager.StartLevel` calls:

```csharp
_connectionPresenter.Initialize(new BaseConnectionRule(), _board);
_board.Initialize(level);
```

So the connection rule and board are set before the board is filled. The connection presenter must be present in the scene and will subscribe to `InputRouter` in `Start`.
