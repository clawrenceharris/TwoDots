# Empty Tile (Board Mechanic)

Empty tiles are board mechanic tiles that reserve a cell so **no dot may ever occupy it**. They are used to create permanent holes or gaps in the board (e.g. at the top row or edges) without blocking gravity in the rest of the column.

## Summary

- **Tile type**: `TileType.EmptyTile`
- **Level key**: `"e"` (see [LevelDataKeys](Assets/Scripts/Level/LevelDataKeys.cs))
- **Classification**: Treated as a board mechanic via `GameTypeExtensions.IsBoardMechanicTile()` (with `Block` and `OneSidedBlock`), so gravity and refill logic treat the cell as non-fillable.

## Level data

In level JSON, empty tiles are listed in `tilesOnBoard` with type `"e"` and a position `"p": [col, row]`:

```json
"tilesOnBoard": [
  { "t": "e", "p": [5, 4] },
  { "t": "e", "p": [4, 4] }
]
```

The level loader maps `"e"` to `TileType.EmptyTile` and the board creates a tile (and `EmptyTileView`) at that grid position.

## Behavior

- **Gravity (CollapseColumn)**  
  When a cell is empty, the board only pulls a dot down from above if the cell is **not** a board mechanic tile (`!tile.Tile.TileType.IsBoardMechanicTile()`). So columns with an empty tile never get a dot moved into that cell.

- **Refill (CollectRefillDrops)**  
  When scanning for empty cells to spawn new dots, the board only spawns in a cell if it is empty **and** (`tile == null || !tile.Tile.TileType.IsBoardMechanicTile()`). So empty tiles are never refilled.

Result: an empty tile is a permanent hole; dots above it can fall past it (into cells below), but the empty-tile cell itself never holds a dot.

## Components

| Item | Location | Role |
|------|----------|------|
| `TileType.EmptyTile` | [TileType.cs](Assets/Scripts/Types/TileType.cs) | Enum value for empty tiles. |
| `IsBoardMechanicTile()` | [GameTypeExtensions.cs](Assets/Scripts/Types/GameTypeExtensions.cs) | Returns true for `Block`, `OneSidedBlock`, and `EmptyTile` so all are treated as non-fillable. |
| `EmptyTileView` | [EmptyTileView.cs](Assets/Scripts/Gameplay/Tiles/Views/EmptyTile/EmptyTileView.cs) | View for empty tiles (extends `TileView`). |
| Prefab | `PrefabLibrary.FromTileType(TileType.EmptyTile)` → EmptyTile prefab | Used when the board spawns tiles from level data. |
| Gravity / refill | [BoardPresenter](Assets/Scripts/Gameplay/Board/Presenters/BoardPresenter.cs) | `CollapseColumn` and `CollectRefillDrops` both skip filling cells where the tile is a board mechanic tile. |

## Visuals

Empty tiles use the same color scheme as other tiles; `ColorSchemeService` maps `TileType.EmptyTile` to `CurrentColorScheme.emptyTile`. The visual is optional—the important part is that the grid position is reserved and no dot is ever placed there.
