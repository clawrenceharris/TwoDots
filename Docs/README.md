# Two Dots Clone – Documentation

Documentation for the current project state. All paths are relative to the repository root.

## Contents

| Document                                          | Description                                                                                                 |
| ------------------------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| [Architecture](Architecture.md)                   | MVP layers, entry point, data flow, and folder layout.                                                      |
| [Connection System](Connection-System.md)         | Connection model, presenter, rules, payload, and line visuals.                                              |
| [Cascade System](Cascade-System.md)              | Fillstep cascade runner, phases, producers (connection, seed, hedgehog, anchor, lotus, gem).               |
| [Board, Dots, and Levels](Board-Dots-Levels.md)   | Board model/presenter/view, dot entity and components, tiles, level data, worlds, and loading.             |
| [Empty Tile Mechanic](Empty-Tile-Mechanic.md)     | Behavior and classification of empty tiles as board mechanic tiles that reserve unfillable board positions. |
| [Blocking Gravity Mechanic](Blocking-Gravity-Mechanic.md) | Segment-based gravity and refill behavior with blocking tiles, host tiles, and pass-through-only tiles.    |

## Quick reference

- **Game bootstrap**: `Assets/Scripts/Core/GameManager.cs` (class `Game`).
- **Level lifecycle**: `LevelManager` – `StartLevel`, `Restart`, `StartNextLevel`, `LeaveLevel`.
- **Input**: `InputRouter` – static events `OnDotSelected`, `OnDotConnected`, `OnDotSelectionEnded`, `OnPointerDragged`.
- **Connection**: `ConnectionPresenter` (MonoBehaviour) + `ConnectionModel` (pure C#); rules in `Gameplay/Connection/Rules/`.
- **Board**: `BoardPresenter` + `BoardModel` + `BoardView`; dots are `Dot` entities with a component bag (`GetComponent<T>`).
- **Cascade**: `CascadeRunner` (MonoBehaviour) runs fillsteps on connection complete; pre-gravity (connection/seed/hedgehog) then gravity/refill then post-fill (anchor/lotus/gem); input gated by `CascadeState`.
- **Level data**: `LevelData`, `World`, `DotsObject`; loading via `LevelLoader` / JSON.
