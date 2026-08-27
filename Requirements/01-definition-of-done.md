# Definition of done

The software MVP is **done** when a player can complete a full credit under the 1994 rule set, including the messy cases a floor game actually produces.

Physical ticket hardware, coin mechs, and cabinet art are not required for this bar. Stubs are enough if the behavior is real.

## Must-pass capabilities

If any row fails, the MVP is not done.

| ID | Capability | Done looks like |
| --- | --- | --- |
| M1 | Session state machine | Attract → credit → select → countdown → play → intermission → results → attract. No stuck states. |
| M2 | One-at-a-time targets | Exactly one pad is lit. Lighting another pad ends the previous window. |
| M3 | Hit-while-lit scoring | A rising-edge press on the lit pad inside the window scores one hit. Force does not matter. |
| M4 | Misses are first-class | A timeout with no valid hit increments miss count and is shown on the score display. |
| M5 | Difficulty changes the field | Easy / Medium / Hard change how many pads can light and how long each window lasts. |
| M6 | Live dual feedback | Numeric hit count and a simple pictorial meter both update on every hit. Ticket count appears at end. |
| M7 | Percentage payout | Tickets = operator table or formula on `hits / (hits + misses)`. Fixed payout is an alternate config. |
| M8 | Edge-case input | Wrong pad, held pad, bounce, no difficulty chosen, and extra credits do not corrupt score or state. |
| M9 | Configurable tunables | Windows, pads-in-play, rounds, presentations, and payout live in config — not hardcoded magic numbers. |

**Sources:** NJ amusement certification 2-306 (hit when lit, hits and misses both register, payout from percentage); Wikipedia citing the 1995 Island Design manual (one-at-a-time lights, three difficulties, force-independent score, numeric + visual display).

## What “same game” means

**In the contract**

- Rules and timing model
- Input semantics (what counts as a stomp)
- Scoring
- Difficulty effect
- Round structure
- Attract / credit / result loop
- Operator payout modes

**Not in the contract**

- The *Spider Stompin'* name
- Queen / spider art
- Original voice lines
- Original drum / crunch samples
- A pixel-identical cabinet

Theme and naming for this build are original to Zoolander Stompin'.
