# Two Dots Clone

A Unity puzzle game inspired by [Two Dots](https://www.twodots-game.com): connect same-colored dots by drawing paths on a grid to clear them and complete objectives.

## Overview

- **Platform:** Unity (C#)
- **Architecture:** MVP (Model–View–Presenter)
- **Input:** Pointer/touch (Unity Input System)

## Features

- **Grid board** with configurable levels (width, height, tiles, spawn rules)
- **Dot types** Normal, Blank, with opportunity for pooling for performance
- **Connection mechanic:** drag from dot to dot; path is shown as a line that follows the cursor and locks segments between connected dots
- **Line pooling** for connection segments and drag line
- **Pluggable connection rules** (`IDotConnectionRule`) so adjacency/color/special-dot logic can be added without changing the connection flow
- **Level data** (ScriptableObject / JSON) for dots to spawn, tiles, and objectives

## Project Structure (high level)

- **Models** – Board, dots, tiles, connection payload; dot component pattern for optional behaviors (e.g. colorable, directional)
- **Views** – Board, tiles, dots; line segments and drag line rendered via `LinePool` / `LineRenderer`
- **Presenters** – Board, dots, tiles, connection (session state, path, backtracking, cycle-close/squares)
- **Gameplay** – Connection rules, completion payload; level manager wires input → connection → visuals
- **Input** – Pointer down/move/up; dot hit detection; `OnDotSelected`, `OnDotConnected`, `OnDotSelectionEnded`, `OnPointerDragged`

## Requirements

- Unity 6.3
- Unity Input System
- DOTween (used for dot animations)
- Json Newtonsoft

## Getting Started

1. Open the project in Unity.
2. Ensure a scene has `BoardPresenter`, `BoardView`, `InputRouter`, `LevelManager`, and `LinePool`, `PoolService`, `DotSpawner` and `ConnectionPresenter`. Also add the `PrefabLibrary` and add prefab references.
3. Assign or create level data and enter Play mode to start a level and connect dots.

## License

Portfolio project
