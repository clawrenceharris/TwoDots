# Changelog

All notable changes to the project are documented in this file.

---

## [Unreleased]

### Added

- **BoardMask** – A child GameObject with a `SpriteMask` component is used to clip dot rendering to the board bounds. Dots that are outside the board (e.g. during drop animations starting above the top row) are invisible until they enter the masked area. `BoardView` finds the mask in `Awake` via `GetComponentInChildren<SpriteMask>()` and in `Init(IBoardModel)` sizes and positions it to match the board (with a margin). Dot prefabs use `SpriteRenderer.maskInteraction = VisibleInsideMask` so they are only visible inside the mask.

- **Selection throttle (InputRouter)** – A throttle was added at the input layer to prevent rapid connection input. In `InputRouter`, after a dot is selected or connected, subsequent selection/connection events for any dot are ignored for a short window (`_selectionThrottleTime`, default 0.08s). This is implemented in `SelectDot` and `ConnectDot` using `_lastSelectionTime` and `Time.unscaledTime`. The throttle lives in the input router rather than in the dot selection/connection view for separation of concerns: input is gated at a lower level so the connection system receives a steadier stream of events and rapid taps do not produce multiple selections.
