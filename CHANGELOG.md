# Changelog

All notable changes to the project are documented in this file.

---

## [Unreleased]

### Added

- **BoardMask** – A child GameObject with a `SpriteMask` component is used to clip dot rendering to the board bounds. Dots that are outside the board (e.g. during drop animations starting above the top row) are invisible until they enter the masked area. `BoardView` finds the mask in `Awake` via `GetComponentInChildren<SpriteMask>()` and in `Init(IBoardModel)` sizes and positions it to match the board (with a margin). Dot prefabs use `SpriteRenderer.maskInteraction = VisibleInsideMask` so they are only visible inside the mask.

- **Selection throttle (InputRouter)** – A throttle was added at the input layer to prevent rapid connection input. In `InputRouter`, after a dot is selected or connected, subsequent selection/connection events for any dot are ignored for a short window (`_selectionThrottleTime`, default 0.08s). This is implemented in `SelectDot` and `ConnectDot` using `_lastSelectionTime` and `Time.unscaledTime`. The throttle lives in the input router rather than in the dot selection/connection view for separation of concerns: input is gated at a lower level so the connection system receives a steadier stream of events and rapid taps do not produce multiple selections.

- **Blocking gravity + board mechanics** – Column gravity and refill were updated to treat blocking tiles as segment separators instead of aborting the entire column, and to distinguish three tile families: (1) blocking board mechanic tiles that dots can neither pass through nor occupy; (2) host tiles that can share a cell with a dot and can be valid landing positions; and (3) pass-through-only board mechanic tiles (such as empty tiles) that dots can fall through but never stop on. Gravity and refill now both rely on the same classification so dots never land on non-host mechanic tiles while still falling past holes and respecting blockers. See `Docs/Blocking-Gravity-Mechanic.md` for full details.

- **Preview state pipeline** – Added an event-driven preview system that computes transient signals from active connection state and applies reversible transitions through previewable presenters. Initial implementation includes Nesting and Beetle preview rules, a transition diff engine, optional preview config toggles, and integration documentation for scaling to advanced mechanics. See `Docs/Preview-State-System.md`.
