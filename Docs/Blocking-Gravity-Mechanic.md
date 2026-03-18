# Blocking Tiles, Gravity, and Refill

This document explains the **new blocking-tile gravity behavior**, how it interacts with **empty tiles** and **tiles that can share a cell with dots**, and how we conceptually separate **tiles** from **board mechanic tiles** in the design.

## Conceptual model

At a given grid coordinate \((x, y)\) we can have:

- **Dot**: the matchable piece; at most one per cell; subject to gravity, connections, clears, etc.
- **Tile**: a board element that may or may not allow a dot to **share** the same cell.

Tiles are split into three behavioral families:

1. **Blocking tiles (board mechanic – hard blockers)**
   - Examples: `Block`, `OneSidedBlock` (and similar solid blockers).
   - **Dots cannot pass through**.
   - **Dots cannot occupy the same cell**.
   - They split a column into independent gravity “segments”: nothing above may fall below the blocker, and no dot is ever placed on the blocker cell itself.

2. **Host tiles (tiles – shareable ground)**
   - Examples: slime, ice, trains, and other ground-like tiles that sit _under_ dots.
   - **Dots can pass through them while falling**.
   - **Dots can stop on them and share the cell** (tile + dot at the same grid position).
   - These tiles primarily modify dot behavior (e.g. add hit-points, sliding, or color effects) rather than board topology.

3. **Pass-through-only tiles (board mechanic – empty / holes)**
   - Example: `EmptyTile`.
   - **Dots can pass through them** while falling.
   - **Dots can never stop on that cell** – the position is permanently reserved as a hole in the board.
   - They shape the board layout while keeping columns around them active.

In code terms, we can think of:

- `IsBlockingTile(x, y)` → true for family (1).
- `CanHostDot(x, y)` → true for family (2).
- Family (3) is `!IsBlockingTile && !CanHostDot`.

See also: [Empty Tile (Board Mechanic)](Empty-Tile-Mechanic.md) for details on empty tiles.

## Gravity: collapsing a column

### Previous issue

Originally, `CollapseColumn` stopped scanning a column as soon as it hit a **blocking tile** by using `break`. This caused a problem in scenarios like:

- Column 0 has a blocking tile at `(0, 3)`.
- Below it are dots at `(0, 2)`, `(0, 1)`, `(0, 0)`.
- If the dots at `(0, 0)` and `(0, 1)` are cleared, the dot at `(0, 2)` should fall down to `(0, 0)`.

Because of the early `break`, the logic never revisited the rows **below** the blocker, so the dot at `(0, 2)` appeared “suspended” instead of falling down to fill the new empty spaces.

### New segment-based gravity behavior

The fix is to treat blocking tiles as **segment separators** instead of a hard stop for the entire column:

- We scan the column from top to bottom.
- When we encounter a **blocking tile**, we:
  - Do **not** allow any dot to occupy that cell.
  - Reset the “write” (landing) row to the row **below** the blocker.
  - **Continue** scanning the column from there (no `break`).

For non-blocking rows:

- If the cell contains a dot, we compute where it should land within its current gravity segment:
  - The target row is at or below the current “write” row.
  - We **skip over** any cells that cannot host a dot (e.g. empty tiles / holes).
  - We stop at the first cell that **can host a dot** (e.g. normal ground, slime, ice, trains).

Resulting behavior:

- **Blocking tiles**:
  - Partition the column into independent gravity segments.
  - Dots never pass through or land on them.
  - Dots below a blocker still fall within their own segment.

- **Host tiles (shareable)**:
  - Are valid landing cells; gravity can stop a dot there.
  - Dots can pass through them while there is still empty space further down in the same segment.

- **Pass-through-only tiles (empty / holes)**:
  - Gravity skips them as landing positions.
  - Dots fall **past** them to the next valid, non-blocking, host cell.

In the earlier example, this means the dot at `(0, 2)` correctly falls to `(0, 0)` after the clear, because the lower part of the column remains active and the empty spaces are filled until a host cell or the board edge is reached.

## Refill: spawning new dots

The refill pass must agree with gravity about which cells can hold dots:

- We scan from the **top** of each column, looking for cells that:
  - Are empty of dots, and
  - Are **not** a blocking or pass-through-only board mechanic tile.

- For each such cell, we enqueue a spawn:
  - The dot is spawned above the board and falls along the same rules as gravity:
    - It **cannot** pass through blocking tiles.
    - It **can** pass through empty / hole cells and other pass-through-only mechanics.
    - It may land only on host tiles (or plain “floor” cells).

By using the same classification flags (`IsBlockingTile` / `CanHostDot`) that gravity uses, refill and collapse stay consistent:

- Blocking tiles never get a dot spawned onto them.
- Pass-through-only tiles never end up with a dot sitting inside them.
- Host tiles can both receive falling dots and have new dots spawned over them.

## Design separation: tiles vs board mechanic tiles

From a design perspective, this separation helps keep logic and content organized:

- **Tiles (host tiles)**
  - Represent things that a dot can **stand on** or **share a cell with**.
  - Influence dot behavior (hits, effects, movement) but not the fundamental shape of where dots can exist on the board.
  - Examples: slime, ice, trains, special ground surfaces.

- **Board mechanic tiles**
  - Represent fixed structures that define **where the board exists** and how gravity flows.
  - May:
    - Fully block occupancy and gravity (blocking tiles), or
    - Reserve cells as permanent holes while allowing dots to fall past (empty tiles and similar mechanics).
  - Examples: blocks, one-sided blocks, zappers, empty/hole tiles.

In implementation, this is reflected by:

- Treating **blocking** and **pass-through-only** mechanics as “board mechanic tiles” (never host dots).
- Treating **host tiles** such as slime and ice as regular “tiles” that can share a cell with a dot and participate in gameplay rules without redefining the board topology.
