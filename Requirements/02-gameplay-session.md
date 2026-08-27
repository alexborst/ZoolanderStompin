# How a session plays

Confirmed from original marketing, the New Jersey rule description, and the Deluxe voice script (which still names round 2, Get Ready / GO, and a win vs spiders-win ending).

## Session flow

```
Attract → Pick difficulty → Get ready → Play round ⇄ Next round → Payout → Attract
                ↓
         (timeout: auto-Easy or back to attract)
```

| Phase | What the player experiences | What software must do |
| --- | --- | --- |
| Attract | Machine is idle. Lights cycle. Decorative motion and occasional call-in audio. | Cycle pad and marquee lights. Ignore floor stomps for scoring. Accept coin/start. Do not award tickets. |
| Credit | Coin takes. Difficulty buttons light or flash. Prompt to pick Easy, Medium, or Hard. | Increment credits. Decrement one credit when a difficulty is accepted. Stack extra coins. |
| Select | Player chooses a difficulty. If they walk away, the credit is not left hanging forever. | Arm Easy/Medium/Hard inputs. On timeout, either auto-start Easy or return to attract — operator choice. |
| Countdown | Short Get Ready, then GO. | Lock difficulty. Zero hit/miss. Play countdown audio. Do not score stomps yet. |
| Round play | One pad lights at random from the difficulty's active set. Stomp it before it goes out. Repeat. | Run the target loop until the round's presentation count is done. Update score displays on every resolution. |
| Intermission | If more rounds remain, a brief pause and a “round N” cue, then play continues, typically faster or denser. | Announce next round. Optionally tighten the hit window. Resume the target loop. |
| Results | Game-end cue. Win or lose line. Ticket count shown, then tickets (or a stubbed count) pay out. | Compute percentage. Map to tickets. Show numeric tickets. Drive dispenser until notch count matches, or display the stub. |

**Sources:** NJ cert 2-306; PrimeTime / Play Meter 1996 (“seven spiders”, three difficulties, numeric + picture score, adjustable tickets); Deluxe manual game description and self-test phrase list.

## Core rule

A presentation is a randomly chosen floor target that lights for a limited time; stomping that target while it is lit is a hit, letting the window expire is a miss, and tickets come from the resulting hit percentage — not from how hard the player stomps.

## Difficulty

Original documentation is explicit that Easy / Medium / Hard change two things:

1. How many targets are eligible to light
2. How long each light stays on

Exact pad maps and millisecond windows were never published. The numbers below are **MVP starting values**, not recovered ROM values. They must live in config.

| Difficulty | Pads in play (proposed) | Hit window (proposed) | Intent |
| --- | --- | --- | --- |
| Easy | 4 of 7 | 1200 ms | Toddlers can reach and still score |
| Medium | 6 of 7 | 900 ms | Default family play |
| Hard | All 7 | 650 ms | Requires moving around the octagon |

**Source for the two-axis model:** Wikipedia citing the 1995 Island Design manual — difficulties “adjust the number of spiders in play and the amount of time players have to press each button.”
