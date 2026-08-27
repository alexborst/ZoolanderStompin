# Scenario matrix

Happy path plus the ways a kid actually uses a floor game. Each row is a test the MVP should pass before calling gameplay done.

## Session and credit

| Scenario | Expected software behavior |
| --- | --- |
| Idle attract, no credit | Lights and audio only. Floor stomps do not score or start a game. |
| One coin, one credit, pick Medium | Countdown, then Medium pad-set and window. One credit consumed. |
| Two coins stacked during attract | After game 1 results, return to difficulty select with one credit remaining — not attract. |
| Coin during an active game | Bank the extra credit. Do not restart or add time to the current session. |
| Credit taken, nobody picks a difficulty | After the configured wait (Deluxe analog: about 22–64 s), auto-start Easy or dump back to attract. |
| Free play | Start is always available. Still require a difficulty press so the session has a defined field. |
| Service credit | Adds a playable credit without incrementing the coin counter. |
| Power loss mid-game | On reboot, attract. Do not restore a half-played credit. Coin meter is hardware-side. |

## Stomps during a lit window

| Scenario | Expected software behavior |
| --- | --- |
| Correct stomp, light still on | Immediate hit. Light off. Next target after a short gap. |
| Correct stomp a moment after the light dies | Miss already recorded. This press is ignored. No retroactive hit. |
| Stomp a dark pad, then the lit one in time | Wrong pad ignored. Correct pad still counts as a hit. |
| Stand on a pad that later lights | No hit until the player lifts and presses again during the window. |
| Two people cooperating on Hard | Legal. Original is single-player intended but does not lock out extra bodies. Score whoever closes the switch. |

## Round and payout outcomes

| Scenario | Expected software behavior |
| --- | --- |
| Every presentation missed | Hit rate 0%. Lose result. Tickets follow the payout table (often a consolation 0 or 1 — operator set). |
| Every presentation hit | Hit rate 100%. Win result. Max tickets for that table. |
| Round 1 ends with more rounds configured | Intermission cue, then round 2 with the same difficulty. Hits/misses accumulate for payout. |
| Last round ends | Stop lighting pads. Show percentage, win/lose, ticket digits, then pay (or stub). |
| Ticket dispenser jammed / notch never arrives | Stop enabling the motor after a timeout. Show an error. Do not loop forever. Do not void the earned count. |

**Sources:** Timeout-if-no-select and coins-per-credit values from Deluxe DIP bank 2. Cooperation note from Wikipedia. Ticket-notch stop condition from Deluxe self-test and dispenser wiring.

## Payout model

Operators could pay a **fixed** ticket count or scale tickets by **hit percentage**. NJ certification for the original describes percentage of stomps.

MVP must implement:

1. Percentage with a lookup table (default)
2. A config switch for a flat ticket count

**Formula:** `percentage = hits / (hits + misses)`

Presentations that were never shown do not enter the denominator.

### Proposed starting table

Invented so payout is testable. Replace if an original DIP chart is recovered. Deluxe bonus-mode DIP is **out of MVP**.

| Hit rate | Tickets |
| --- | ---: |
| 0–19% | 0 |
| 20–39% | 1 |
| 40–59% | 2 |
| 60–79% | 4 |
| 80–99% | 6 |
| 100% | 8 |
