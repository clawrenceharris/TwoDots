# Two Dots Clone – Documentation

Documentation for the current project state. All paths are relative to the repository root.

## Contents

| Document                                        | Description                                                                                    |
| ----------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| [Architecture](Architecture.md)                 | MVP layers, entry point, data flow, and folder layout.                                         |
| [Connection System](Connection-System.md)       | Connection model, presenter, rules, payload, and line visuals.                                 |
| [Board, Dots, and Levels](Board-Dots-Levels.md) | Board model/presenter/view, dot entity and components, tiles, level data, worlds, and loading. |

## Quick reference

- **Game bootstrap**: `Assets/Scripts/Core/GameManager.cs` (class `Game`).
- **Level lifecycle**: `LevelManager` – `StartLevel`, `Restart`, `StartNextLevel`, `LeaveLevel`.
- **Input**: `InputRouter` – static events `OnDotSelected`, `OnDotConnected`, `OnDotSelectionEnded`, `OnPointerDragged`.
- **Connection**: `ConnectionPresenter` (MonoBehaviour) + `ConnectionModel` (pure C#); rules in `Gameplay/Connection/Rules/`.
- **Board**: `BoardPresenter` + `BoardModel` + `BoardView`; dots are `Dot` entities with a component bag (`GetComponent<T>`).
- **Level data**: `LevelData`, `World`, `DotsObject`; loading via `LevelLoader` / JSON.
