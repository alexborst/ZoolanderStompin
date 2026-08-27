# Zoolander Stompin' — software MVP requirements

These files are the durable, git-tracked requirements for the software MVP. They capture the same material as the Cursor canvas *Stomper MVP Requirements*, which is a live in-editor view only and is **not** part of this repository.

**Product:** a custom-themed floor-stomp redemption game for a Zoolander party.  
**Rule set:** same gameplay, scoring, and operator behavior as Jaleco / Island Design *Spider Stompin'* (1994). Original name, art, and audio are out of scope.

## Documents

| File | What it answers |
| --- | --- |
| [01-definition-of-done.md](01-definition-of-done.md) | When is the software MVP complete? |
| [02-gameplay-session.md](02-gameplay-session.md) | How does a credit play from start to finish? |
| [03-software-behavior.md](03-software-behavior.md) | How must the software behave during that process? |
| [04-scenario-matrix.md](04-scenario-matrix.md) | What happens in each situation that can unfold? |
| [05-scope-and-sources.md](05-scope-and-sources.md) | What is in/out of MVP, unpublished numbers, and sources? |
| [06-implementation-plan.md](06-implementation-plan.md) | Step-by-step build order, effort, story vs epic |

## One-sentence definition of done

A player can take a credit from attract through difficulty select, complete every presentation in every round, see hits and misses update live, finish with a win/lose result, and receive a ticket count derived from hit percentage — and the software still behaves correctly if they pick nothing, stomp the wrong pad, stand on a pad, or time out.

## Core rule

A presentation is a randomly chosen floor target that lights for a limited time. Stomping **that** target while it is lit is a **hit**. Letting the window expire is a **miss**. Tickets come from the resulting hit percentage, not from how hard the player stomps.

## Confirmed constants

- **7** floor targets (one switch + one lamp each)
- **3** difficulties: Easy, Medium, Hard
- **1** lit target at a time
- Scoring is **hit / miss**, force-independent
- Payout is **percentage-based**, with a flat ticket count as an operator alternate

Physical cabinet, coin hardware, and ticket hopper are **not** required to call the software MVP done. Stubs are enough if the behavior is real.
