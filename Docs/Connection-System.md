# Connection System

The connection system handles drag-to-connect gameplay: the player draws a path through dots, and the system validates and displays it.

## Responsibilities

- **Session lifecycle**: Begin (first dot), append/backtrack/cycle-close, End (pointer up), Cancel (cleanup).
- **Path state**: Ordered list of dots, current color, and whether the path is closed (square).
- **Validation**: Pluggable rules decide if a dot can be added (adjacency, color, etc.).
- **Visuals**: A live drag line from the last dot to the pointer, plus locked segments between consecutive dots.

## Components

### ConnectionModel (`Gameplay/Connection/Models/ConnectionModel.cs`)

- Implements `IConnectionModel`.
- Holds: `Path` (read-only list of `IDotPresenter`), `IsSessionActive`, `CurrentColor`, `IsSquare`.
- Methods: `Begin(dot)`, `TryAppend(dot)`, `TryBacktrack(dot)`, `End()`, `Cancel()`, `UpdateColor()`.
- Events: `OnPathChanged`, `OnConnectionCompleted`, `OnColorChanged`, `OnDotAddedToPath`, `OnDotRemovedFromPath`, `OnSquareActivated`, `OnSquareDeactivated`.
- Uses an `IDotConnectionRule` and `IBoardPresenter` for validation and board queries.
- **UpdateColor** derives the connection color from the path (e.g. first dot’s color or blank).

### Square (closed-cycle) connection logic

A **square** connection is activated when the player **revisits a dot that is already in the path** (cycle-close). When this happens, `ConnectionModel` flips `IsSquare = true` and computes an expanded “hit/clear” set:

- **Dots in the path**: the connected dots (existing `_pathIds`) are always included.
- **Dots outside the path**: the model scans the full board and includes **any colorable dot** whose comparable color matches the connection’s `CurrentColor`.
  - If the connection color is **blank**, the square can include **all** eligible dots (per the model’s rules).
  - Blank-colored dots are excluded when the connection color is not blank.

This expanded set is exposed via:

- **`DotsToHitFromSquare`**: the additional dot IDs included by the square logic (outside the normal connection path).
- **`OnSquareActivated(dotsToActivate)`**: raised when the cycle is closed; used for square selection feedback.
- **`OnSquareDeactivated(dotsToDeactivate)`**: raised when the player backtracks out of the closed cycle; used to remove square feedback from dots that are no longer “hit” by the square.

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

### ConnectionResult (`Gameplay/Connection/ConnectionResult.cs`)

- Result of a connection session when the pointer is released.
- Properties:
  - `DotIds`: ordered dot IDs in the path.
  - `IsSquare`: whether the path was closed (cycle).
  - `ConnectionColor`: the final resolved connection color.
  - `DotsToHitFromSquare`: dot IDs selected by square logic (outside the path).
  - `AllDotsToHit`: union of `DotIds` and `DotsToHitFromSquare` (deduped). This is the typical set you’d pass into clear/cascade logic for a square.

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
