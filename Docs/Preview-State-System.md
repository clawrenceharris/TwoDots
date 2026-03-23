# Preview State System

This document describes the preview pipeline used to drive reversible, connection-time animations and sound transitions before a move is committed.

## Purpose

Preview state is for anticipation only:

- It reacts to active connection path changes while the player drags.
- It does not mutate gameplay models (`Hit`, `Clear`, `Replace`) directly.
- It is reset when the connection ends or is cancelled.

## Core components

| Component | Location | Responsibility |
| --- | --- | --- |
| `PreviewStateManager` | `Assets/Scripts/Gameplay/Preview/PreviewStateManager.cs` | Recomputes preview state from current connection path and applies transitions to previewable presenters. |
| `PreviewContext` | `Assets/Scripts/Gameplay/Preview/PreviewContext.cs` | Read-only snapshot used by rules (board, active path, helper checks like one-hit-left). |
| `IPreviewRule` | `Assets/Scripts/Gameplay/Preview/IPreviewRule.cs` | Mechanic-specific rule contract that maps context to `PreviewSignal` flags. |
| `PreviewDiffEngine` | `Assets/Scripts/Gameplay/Preview/PreviewDiffEngine.cs` | Produces enter/exit transitions by diffing previous vs current signals per entity. |
| `IPreviewablePresenter` | `Assets/Scripts/Gameplay/Preview/IPreviewablePresenter.cs` | Presenter contract for applying/removing preview animation state. |
| `PreviewAudioBridge` | `Assets/Scripts/Gameplay/Preview/PreviewAudioBridge.cs` | Transition-only event bridge for audio hooks (enter/exit signals). |
| `PreviewConfig` | `Assets/Scripts/Gameplay/Preview/PreviewConfig.cs` | Optional ScriptableObject for global/per-mechanic toggles and animation tuning values. |

## Lifecycle integration

`ConnectionPresenter` owns runtime integration:

- Creates `PreviewStateManager`.
- Registers mechanic rules (`NestingPreviewRule`, `BeetlePreviewRule`).
- Calls `Recompute()` on connection start and path changes.
- Calls `ResetAllPreview()` on cancel and completion.

This keeps preview event-driven and avoids per-frame board scans.

## Implemented mechanic behavior

- **Nesting**
  - Emits preview when one hit remains (`HitMax - HitCount == 1`).
  - Presenter response: shake + pulse while signal is active.
- **Beetle**
  - Emits `ConnectedActive` and `WingFlap` while the beetle is in the active connection path.
  - If beetle has a hittable model and one hit remains, it also emits shake.
  - Presenter response: shake and wing-flap loop, both transition-safe.

## Advanced mechanics integration guide (no implementation in this phase)

Use the same rule/presenter pipeline to add complex mechanics without changing manager internals.

### 1) Define signals

Add mechanic-specific flags to `PreviewSignal` (for example `GhostTargeting`, `ValidTrigger`).

### 2) Implement rule(s)

Create `IPreviewRule` implementations that:

- Gate by entity/mechanic type in `CanEvaluate`.
- Read only from `PreviewContext` and presenter/entity models.
- Return combined flags representing current anticipated outcomes.

### 3) Implement previewable presenter

Add an `IPreviewablePresenter` for that mechanic and map active signals to visual states:

- Start loops on enter.
- Stop and restore defaults on exit.
- Keep animation state idempotent (safe to reapply each recompute).

### 4) Optional pooled preview visuals

For ghost/target projections (example: magnet ghost):

- Use pooled view objects owned by the preview presenter.
- Update target only on signal transitions/path changes.
- Return pooled visuals in `ResetPreview`.

### 5) Transition-driven sound

Subscribe audio systems to `PreviewAudioBridge.OnPreviewTransition` and play sounds only on transitions:

- Enter signal -> play anticipation cue.
- Exit signal -> optional tail/stop cue.

This avoids repeated sound spam during drag updates.

## Pseudo-flow examples

### Magnet ghost prediction

1. Rule computes projected landing from active path + board occupancy.
2. Rule emits `GhostTargeting` when projection is valid.
3. Presenter spawns/updates pooled ghost at projected cell.
4. On signal exit, presenter despawns ghost.

### Hedgehog valid trigger

1. Rule checks whether active path currently satisfies hedgehog trigger placement.
2. Rule emits `ValidTrigger` while condition is true.
3. Presenter plays a dedicated anticipation animation while active.
4. Exit transition restores normal state.

## Testing checklist for advanced mechanics

- Signals enter/exit correctly across append, backtrack, square activate/deactivate.
- No gameplay model changes occur during preview-only updates.
- Reset occurs on cancel/end with no leftover tweens/pooled objects.
- Audio events fire once per transition (not every recompute).
- Multiple previewable entities can coexist without cross-state leaks.
