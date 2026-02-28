# Architecture

This document describes the high-level architecture of the Two Dots clone.

## Overview

The game uses **MVP (Model–View–Presenter)** to separate data, presentation, and input/coordination. Core gameplay is driven by **events** (static and instance) rather than deep coupling.

## Layers

| Layer             | Role                                                          | Examples                                                                 |
| ----------------- | ------------------------------------------------------------- | ------------------------------------------------------------------------ |
| **Model**         | Pure data and rules; no Unity types in core models.           | `BoardModel`, `ConnectionModel`, `Dot`, `TileModel`, `LevelData`         |
| **View**          | Unity visuals only; no game logic.                            | `BoardView`, `DotView`, `TileView`, `ConnectorLineView`                  |
| **Presenter**     | Coordinates model and view; reacts to input and model events. | `BoardPresenter`, `ConnectionPresenter`, `DotPresenter`, `TilePresenter` |
| **Services/Core** | App lifecycle, input, loading.                                | `Game` (bootstrap), `LevelManager`, `InputRouter`                        |

## Entry and Flow

1. **Game** (`Core/GameManager.cs`)
   - Singleton; loads starting level via `LevelLoader` and calls `LevelManager.StartLevel`.

2. **LevelManager**
   - Owns level lifecycle: `StartLevel`, `LeaveLevel`, `Restart`, `StartNextLevel`.
   - Initializes `ConnectionPresenter` with a connection rule and `IBoardPresenter`, then initializes the board.

3. **Input → Connection → Board**
   - `InputRouter` (pointer/touch) raises: `OnDotSelected`, `OnDotConnected`, `OnDotSelectionEnded`, `OnPointerDragged`.
   - `ConnectionPresenter` subscribes and drives `ConnectionModel` (path, backtrack, cycle-close).
   - Connection rules (`IDotConnectionRule`) decide if a dot can be added; the board is queried for spatial/state.

4. **Board**
   - `BoardPresenter` holds `BoardModel` (grid, dots, tiles) and `BoardView` (creation/placement of tile and dot views).
   - Dots are entity objects (`Dot`) with a **component pattern** (`GetComponent<T>`, `AddComponent<T>` for `IModel` types like `ColorableModel`, `DirectionalModel`).

## Key Conventions

- **Presenters** are MonoBehaviours and find or receive View and Model; they subscribe to input and model events.
- **Models** are plain C#; board/connection state lives here.
- **Views** are MonoBehaviours; they receive data to display and do not query presenters or models for logic.
- **Connection** is split: `ConnectionModel` holds path/color/square state; `ConnectionPresenter` handles input and updates a `ConnectorLineView` (drag line + segments).
- **Pools** (`DotPool`, `LinePool` / connector lines) are used for dots and connection line segments to avoid per-move allocations.

## Folder Layout (Scripts)

- **Core** – Game bootstrap, LevelManager, DotSpawner, CoroutineHandler.
- **Gameplay** – Board (model + presenter), Connection (model, presenter, rules, payload).
- **Input** – InputRouter, InputGate.
- **Level** – LevelData, World, LevelLoader, LevelDataConverter.
- **Presenters** – Dot (and variants), Tile, Connection.
- **Views** – Board, Dot (and variants), Tile, Connection (ConnectorLineView).
- **Models** – Tiles, Colorable, Directional;
- **Entities** – Dot (entity + component bag), IBoardEntity.
- **Factories** – Dot, Tile creation from level/spawn data.
- **Pooling** – DotPool, LinePool, Pool, PoolService.
- **ColorScheme / Skinning** – Theming and dot appearance.
- **Types** – DotType, TileType, DotColor, etc.
