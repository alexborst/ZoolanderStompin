# Scope, unpublished numbers, and sources

## In software MVP

- Full session and target state machines
- Three difficulties with pads-in-play and windows
- Hit / miss / ignore / rising-edge rules
- Multi-round play with accumulated percentage
- Numeric + simple pictorial score
- Win/lose + ticket result (hardware stub allowed)
- Attract, credit stacking, select timeout, free play
- Config file for every unpublished number
- Simulated pads (keys or on-screen) so it is playable before the cabinet exists

## Not required to call MVP done

- Physical platform, coin door, or ticket hopper
- Deluxe plasma display, queen voice, or bonus mode
- Adaptive mid-game difficulty (“programmed intelligence” is a Deluxe claim)
- Copied audio, art, or character script
- Self-test lamp walk (useful later, not gameplay-done)
- Marquee / spinner motor control
- Dual ticket dispensers
- Final device selection — only the I/O contract in [03-software-behavior.md](03-software-behavior.md)

## Unpublished numbers

The original 1995 installation manual is cited in secondary sources but is not publicly transcribed. These values **must be config** until someone times a working machine or we playtest them into place. They do **not** block calling MVP done if they are tunable.

| Gap | What we know | MVP default |
| --- | --- | --- |
| Hit windows | Harder = less time. No milliseconds published. | 1200 / 900 / 650 ms |
| Pads per difficulty | Harder = more targets in play. Mapping unknown. | 4 / 6 / 7 of 7 |
| Presentations per round | Score display is two digits, so under 100 hits. | 20 per round |
| Round count | Deluxe DIP selects rounds; voice names round 2. | 2 rounds |
| No-repeat target | Not documented. | Never light the same pad twice in a row |
| Inter-target gap | Not documented. | 250 ms dark between presentations |
| Win vs lose line | Deluxe has both phrases. Threshold unknown. | Win at ≥ 60% hits |
| Select timeout | Deluxe: ~22–64 seconds, auto-Easy or attract. | 30 s, then auto-Easy |

## Source split

| Claim | Authority |
| --- | --- |
| Choose difficulty, randomly lit button, score on hit-when-lit, register hits and misses, pay on percentage | NJ cert 2-306 (original game) |
| Seven floor spiders, three difficulties, numeric + picture score, adjustable tickets, two-digit displays | 1996 Play Meter / PrimeTime (original cabinet) |
| One light at a time, force does not change score, difficulties change pad count and time | Wikipedia citing 1995 Island Design manual |
| Get Ready / GO, round 2, win vs spiders-win, ticket notch, 7 foot switches, 4 cartoon lamps, DIP payout / rounds / coins | Deluxe 1996 manual — same designer; treat as analog, not gospel |
| European 6-spider listings | Likely clones. Do not use. Original I/O is 7 pads. |

## Suggested next step

Lock the unpublished numbers as a config schema, then pick a device that can debounce 7+3 digital inputs, drive 7+ pad lamps, play audio, and later pulse a ticket hopper. Programming can start against a keyboard simulator of that same I/O map.
