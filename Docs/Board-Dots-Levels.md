# Board, Dots, and Levels

## Board

### BoardModel (`Gameplay/Board/Models/BoardModel.cs`)

- Implements `IBoardModel`.
- Grid state: `DotGrid` (2D array of `Dot`), `TileGrid` (2D array of `TileModel`); `Width`, `Height`.
- Lookups: `GetDotAt(x,y)`, `GetDot(id)`, `GetTileAt`, `GetTile`; `GetAllDots()`, `GetAllTiles()`.
- Mutations: `SpawnDot(dot)`, `RemoveDot(id)`, `MoveDot(id, toPosition)`; `SpawnTile`, `RemoveTile`; `ClearBoard()`.
- Initialization: `InitDots(level)`, `InitTiles(level)` return lists for the level loader/presenter to place.
- Events: `OnDotCleared`, `OnDotSpawned`, `OnTileRemoved`, `OnTileSpawned`.
- Dots are stored by ID in a dictionary and in the grid by position.

### BoardPresenter (`Gameplay/Board/BoardPresenter.cs`)

- Implements `IBoardPresenter`.
- Holds `BoardModel` and `BoardView`; creates tile and dot presenters when spawning.
- **Initialize(level)**: Builds board model, initializes view, creates tiles and dots from level data (and may fill empty cells).
- Exposes board queries (dots at position/row, neighbors, edge checks) and dot/tile lifecycle (Spawn, Remove, Move).
- Dot presenters are registered by dot ID; tile presenters by tile ID. Clears and recreates on re-initialization.

### BoardView (`Views/Board/Views/BoardView.cs`)

- Creates and holds `TileView` and `DotView` instances; uses `BoardView.TileSize` for dot placement.
- **CreateTileView(tile)**, **CreateDotView(dot)**: Instantiate or obtain from pools and position from grid.
- **ReleaseDotView(dotId)**: Removes a dot view (e.g. for pooling when a dot is cleared).
- Connection visuals (segments, drag line) may be managed here or by the connection presenter’s line view stack; see Connection System doc.

---

## Dots

### Dot entity (`Gameplay/Dots/Enitities/Base/Dot.cs`)

- Implements `IBoardEntity` (`ID`, `GridPosition`).
- **Component pattern**: `Dictionary<Type, IModel>` for attached models.
  - **AddComponent&lt;T&gt;(component)**: Registers by `typeof(T)`.
  - **GetComponent&lt;T&gt;()**: Exact type first; if missing, searches for a component assignable to `T` (e.g. interface).
  - **TryGetComponent&lt;T&gt;(out T)**, **RemoveComponent&lt;T&gt;**.
- Created via `DotFactory.CreateDot(dotsObject)` from level/spawn data. Dot type (Normal, Beetle, Blank, Bomb, etc.) determines which components are added (e.g. `ColorableModel`, `BlankColorableModel`, `DirectionalModel`).

### Dot presenters and views

- **IDotPresenter**: `Dot`, `View`, Clear/Drop/Spawn animations, and presenter registry (`GetPresenter&lt;T&gt;`, `AddPresenter&lt;T&gt;`, `TryGetPresenter&lt;T&gt;`).
- **DotPresenter** (and **NormalDotPresenter**, **LotusDotPresenter**, etc.): Bind a dot to a view; handle clear/drop and events.
- **ConnectableDotPresenter** / **IConnectableDotPresenter**: Used when a dot can be part of a connection; notified by `ConnectionPresenter` (e.g. `Connect(connectionModel)`).
- **DotView** (and **NormalDotView**, **BlankDotView**): Visual representation; may be pooled via `DotPool`.

### Dot models (components)

- **ColorableModel** / **IColorableModel**: Holds `DotColor`; used by connection color rule. **BlankColorableModel** for blank dots; **BeetleColorableModel** for beetle type.
- **DirectionalModel** / **IDirectionalModel**: Direction (e.g. for beetles); `SetDirection`, `FindBestDirection`, `ToRotation`.

#### Bomb dots

- **Type**: `DotType.Bomb` (see `GameTypeExtensions` and `LevelDataKeys`).
- **Creation**:
  - Bomb dots are usually **created at runtime**, not hand-authored in level JSON.
  - When a **big square** connection is committed, the `Square` helper in the connection system finds dots fully enclosed by the loop and asks the board to replace each interior dot with a pooled bomb presenter.
- **Pooling**:
  - `BombPool` pre-allocates bomb presenters by constructing `DotsObject` instances with `DotType.Bomb` and calling `BoardPresenter.CreateDotPresenter`.
  - Each pooled entry is wrapped in a `BombPoolObject` exposing an `IDotPresenter` and is returned to the pool when no longer in use.
- **Visuals and preview**:
  - While the player is drawing a big square, interior dots are hidden and temporary bomb views are shown as a **preview**.
  - If the square is cancelled or backtracked, the preview bombs are removed and original dot views are restored; if the connection is completed, the preview bombs are committed as real bomb dots on the board.

---

## Tiles

- **TileModel**: `TileType`, `GridPosition`, `ID`; created from level tile data.
- **TilePresenter** / **ITilePresenter**: Wraps model and **TileView**; created when the board spawns a tile.
- **TileView**: Rendered tile; created by `BoardView.CreateTileView(tile)` from prefabs keyed by `TileType`.

---

## Levels and Worlds

### LevelData (`Level/LevelData.cs`)

- ScriptableObject (or serialized) level definition: `levelNum`, `width`, `height`, `moves`, `colors`.
- Arrays: `dotsOnBoard`, `dotsToSpawn`, `initDotsToSpawn`, `tilesOnBoard` (e.g. `DotsObject[]`, `tilesOnBoard`).
- Optional: `isTutorial`, `tutorialSteps`.
- **DotsObject**: Col, Row, Type (string), HitCount, and a property bag for Colors, Directions, etc., used by `DotFactory` and level loader.

### World (`Level/World.cs`)

- ScriptableObject: `levels` (e.g. `TextAsset[]`) and `colorScheme`.
- Used by **Game** to enumerate level count and by level progression to pick the next level’s asset.

### Loading

- **LevelLoader**: Loads `LevelData` from a `TextAsset` (e.g. JSON); may set a static `Level` for factories that need level colors.
- **LevelDataConverter** / **JSONLevelLoader**: Convert between JSON and `LevelData` / `DotsObject`.
- **LevelManager.StartLevel(level)**: Initializes connection (with rule and board), then board; applies color scheme. **StartNextLevel** / **Restart** use level index or current level asset.

### Game bootstrap (`Core/GameManager.cs`)

- **Game** (class in `GameManager.cs`): Singleton; in `Awake` loads starting level from a serialized `TextAsset` and calls `LevelManager.StartLevel`. Holds **Worlds** and **WorldIndex** for level count and progression.
