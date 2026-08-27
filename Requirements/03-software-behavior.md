# Software behavior

## Target loop (while a round is live)

This is the inner machine the rest of the product exists to serve. Ignore-paths (wrong pad, bounce, already-held pad) must **not** consume the window.

```
Pick target → Light + start window
                    ↓
         ┌──────────┼──────────┐
         ↓          ↓          ↓
        Hit       Miss      Ignore ──→ window still running
         ↓          ↓
         └──── Resolve ────→ Pick next target
```

### Resolution rules

| Event during lit window | Software response |
| --- | --- |
| Rising edge on the lit pad | **Hit.** Light off immediately. Increment hits. Play success cue. Advance. |
| Window expires with no valid hit | **Miss.** Light off. Increment misses. Play miss cue. Advance. |
| Rising edge on any other pad | **Ignore.** Do not score. Window keeps running so a later correct stomp can still hit. |
| Pad already held down when it lights | **Ignore** until release, then a new press. Standing on a pad must not auto-score. |
| Switch bounce / double-fire | Debounce (target ~20–40 ms). One stomp is one event. |
| Two pads claimed at once | Accept only the lit pad. Others ignored. |

Hit-while-lit and force-independence are original. Wrong-pad ignore and rising-edge-after-light are recommended floor-game defaults; they are not printed in the surviving 1994 docs. They are still **required for this MVP** so standing on a pad cannot cheat a hit.

Score **presses**, not “switch is currently closed.”

## Displays the software must drive

### Player-facing (original behavior)

- Two-digit numeric hits
- Two-digit tickets at end
- Pictorial score of four “cartoon” lamps so young children can see progress without reading numbers
- Difficulty button lamps
- One lamp per floor pad

**Sources:** PrimeTime listing; Deluxe wiring (7 foot lights, 4 cartoon lights, Easy/Medium/Hard lights).

### Operator-facing (stub OK for software MVP)

- Coin counter
- Ticket counter
- Credit add without bumping the coin meter
- Self-test entry
- Ticket-notch feedback so payout stops when the physical count matches
- Low-ticket warning if a dispenser is attached

Software MVP may log these instead of driving hardware.

## I/O contract (for later device choice)

The device only has to satisfy this map. GPIO-capable boards, arcade I/O boards, or a keyboard simulator all work if the game loop sees the same events.

| Direction | Count | Signal |
| --- | --- | --- |
| In | 7 | Floor pad switches |
| In | 3 | Easy / Medium / Hard |
| In | 1+ | Coin or start (plus service credit, test) |
| In | 1 | Ticket notch (optional until hardware) |
| Out | 7 | Floor pad lamps |
| Out | 3 | Difficulty button lamps |
| Out | 4 | Pictorial score lamps |
| Out | 2 groups | Score digits and ticket digits (or one screen) |
| Out | 1 | Audio |
| Out | 1 | Ticket enable (optional until hardware) |
