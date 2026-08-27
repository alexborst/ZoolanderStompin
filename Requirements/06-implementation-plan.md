# Implementation plan — software MVP

C# on Raspberry Pi 3B+ / 4. Keyboard-simulated I/O until GPIO is wired.

This plan is the step-by-step path to the definition of done in [01-definition-of-done.md](01-definition-of-done.md). Work top to bottom. Later steps assume earlier ones exist.

## How to read this

| Label | Meaning |
| --- | --- |
| **User story** | One slice of behavior you can demo or test on its own. Implement as a single unit of work. |
| **Epic** | A larger outcome that must be split into the child stories listed under it before coding. Do not treat an epic as one commit or one sitting. |

**Level of effort** is wall-clock for one C#-comfortable developer, not including cabinet carpentry or waiting on parts. Ranges assume the requirements stay stable.

- **S** — 2–4 hours
- **M** — 1–2 days (about 6–12 hours)
- **L** — 3–5 days

Software MVP is **done after step 12** (keyboard playable, M1–M9 and the scenario matrix). Steps 13–15 are the Pi/hardware increment. They are not required to call the software MVP done.

## Sequence

### 1. Solution skeleton
**Type:** User story  
**Effort:** S (2–4 h)  
**Maps to:** none yet (enables everything)

Create the .NET solution: game library, console (or Avalonia) host, test project. No gameplay yet. Confirm `dotnet test` and `dotnet run` work on Windows.

**Done when:** empty app starts and tests run.

---

### 2. Domain model and config schema
**Type:** User story  
**Effort:** M (6–8 h)  
**Maps to:** M5, M9

Types for pads (1–7), difficulty, hits/misses, session phase. JSON (or similar) for every unpublished number in [05-scope-and-sources.md](05-scope-and-sources.md): hit windows, pads-in-play, presentations per round, round count, no-repeat, inter-target gap, win threshold, select timeout, payout table, debounce. Load at startup; fail fast on bad config.

**Done when:** config file changes Easy’s window without a recompile; invalid config is rejected.

---

### 3. I/O port and keyboard simulator
**Type:** User story  
**Effort:** M (6–10 h)  
**Maps to:** M8 (enables), simulated pads in scope

One interface the game talks to: pad pressed/released, difficulty buttons, credit/start, service credit; lamp outputs for pads, difficulty, pictorial score; score/ticket digits; audio trigger; ticket enable/notch. Implement a keyboard (and/or on-screen) adapter so a PC can play. GPIO adapter comes in step 13.

**Done when:** pressing mapped keys is visible as pad/credit events; lamp “on” is visible in the host (console or window).

---

### 4. Deterministic test clock
**Type:** User story  
**Effort:** S (2–4 h)  
**Maps to:** tests for M2–M4, M8

Game loop takes an injectable clock/time source. Tests advance time instead of `Thread.Sleep`. Required before the target loop or tests will be slow and flaky.

**Done when:** a test can jump 1200 ms in-process and the game treats it as a timeout.

---

### 5. Target loop
**Type:** Epic  
**Effort:** L (3–4 days)  
**Maps to:** M2, M3, M4, M8 (stomp cases)

The inner machine: one lit pad, hit / miss / ignore, then next presentation. Child stories, in order:

| Child | Type | Effort | Done when |
| --- | --- | --- | --- |
| 5a. Pick and light one pad from the difficulty set; no-repeat; dark gap | Story | S–M | Exactly one lamp on; next pick respects config |
| 5b. Rising-edge hit on the lit pad inside the window | Story | S | Hit count +1; lamp off immediately |
| 5c. Timeout miss | Story | S | Miss count +1 when window expires with no valid hit |
| 5d. Ignore wrong pad, held pad, bounce, extra pads | Story | M | Those inputs do not score and do not kill the window |

**Epic done when:** a scripted round of N presentations produces the expected hit/miss totals for the [04-scenario-matrix.md](04-scenario-matrix.md) stomp rows.

---

### 6. Session flow
**Type:** Epic  
**Effort:** L (3–5 days)  
**Maps to:** M1, M5, M8 (credit/select cases)

Outer machine. Child stories:

| Child | Type | Effort | Done when |
| --- | --- | --- | --- |
| 6a. Attract: cycle lamps, ignore stomps for scoring, accept credit | Story | S–M | Stomps in attract do not score |
| 6b. Credit and difficulty select (Easy/Medium/Hard lamps) | Story | M | One credit consumed on select; difficulty locks pad-set and window |
| 6c. Select timeout (auto-Easy or return to attract) | Story | S | Configured wait expires correctly |
| 6d. Countdown (Get Ready / GO); no scoring yet | Story | S | Stomps during countdown do not count |
| 6e. Run configured rounds with intermission | Story | M | Round 2 starts after round 1; hits/misses accumulate |
| 6f. Credit stacking and free play | Story | M | Extra credit during a game banks the next play; free play still requires a difficulty press |
| 6g. Service credit (playable, coin meter not incremented) | Story | S | Distinct from coin credit if a meter stub exists |

**Epic done when:** Attract → select → countdown → rounds → back to attract with leftover credit, with no stuck state (M1).

---

### 7. Live score and pictorial meter
**Type:** User story  
**Effort:** M (6–10 h)  
**Maps to:** M6

Numeric hit display (two-digit) updates on every hit. Four pictorial lamps (or on-screen stand-ins) show progress for non-readers. Ticket digits stay blank or “--” until results.

**Done when:** a hit updates number and pictorial together; a miss does not advance the pictorial as a hit.

---

### 8. Results, win/lose, and payout
**Type:** User story  
**Effort:** M (4–8 h)  
**Maps to:** M7

`percentage = hits / (hits + misses)`. Lookup table default; config switch for a flat ticket count. Win line at configured threshold (default ≥ 60%). Show ticket count. Stub dispenser: log/display N tickets; do not block the session if hardware is absent.

**Done when:** 0% and 100% games produce table tickets and lose/win; switching to fixed payout ignores percentage.

---

### 9. Playable host on Windows
**Type:** User story  
**Effort:** M (1–2 days)  
**Maps to:** simulated pads; makes 5–8 demoable

A host you can actually play: console key legend **or** a simple window (Avalonia is optional, not required for DoD). Show phase, hits, misses, percentage, tickets. Map keys to 7 pads, 3 difficulties, credit.

**Done when:** a person can play a full Medium game on a PC without a Pi.

---

### 10. Scenario matrix as automated tests
**Type:** Epic  
**Effort:** L (2–3 days)  
**Maps to:** M1–M9, [04-scenario-matrix.md](04-scenario-matrix.md)

Each matrix row becomes a test (or a small cluster). Prefer driving the game through the I/O port and fake clock, not the UI.

| Child | Covers | Effort |
| --- | --- | --- |
| 10a. Session and credit rows | Attract, stacking, timeout, free play, service credit, power-loss reboot = attract | M |
| 10b. Stomp rows | Hit, late stomp, wrong then right, held pad, two players | M |
| 10c. Round and payout rows | All miss, all hit, intermission, last round, ticket-jam timeout (stub) | M |

**Epic done when:** every row in the scenario matrix has an automated test, and they pass.

---

### 11. Audio cues (optional for rules, in for a playable MVP)
**Type:** User story  
**Effort:** M (6–10 h)  
**Maps to:** playable session, not a numbered M#

Original samples are out of scope. Original **cues** are in: new light, hit, miss, countdown, round, game end, coin, ticket. Windows can use a simple player; Pi Linux audio can wait or use a small abstraction (`IAudio`) with `aplay` later.

**Done when:** a full keyboard game has distinct hit vs miss vs end sounds. Silence on failure to open the device, not a crash.

Skip this step if you want rules-DoD only; M1–M9 do not require audio. Recommended before calling the **playable** MVP done.

---

### 12. Keyboard definition-of-done playtest
**Type:** User story  
**Effort:** S–M (4–8 h)  
**Maps to:** all of M1–M9

Manual pass of the one-sentence DoD plus the messy cases: no difficulty chosen, wrong pad, stand on a pad, timeout. Fix gaps. Freeze config defaults.

**Done when:** you can honestly check every M# row in [01-definition-of-done.md](01-definition-of-done.md) on a PC.

**← Software MVP is done here.**

---

### 13. Raspberry Pi GPIO adapter
**Type:** Epic  
**Effort:** L (3–5 days) including wiring bring-up  
**Maps to:** I/O contract in [03-software-behavior.md](03-software-behavior.md)

Same `IGameIo`, real pins via `System.Device.Gpio` on Pi 3+/4. MOSFET/relay HAT for lamps (not direct 12 V from Pi pins). Child stories:

| Child | Type | Effort | Done when |
| --- | --- | --- | --- |
| 13a. Pin map in config; smoke test read one input, write one output | Story | M | One button lights one lamp on the Pi |
| 13b. All 7 pads + 3 difficulty + credit inputs, debounced in software | Story | M | Keyboard and GPIO produce the same events |
| 13c. All lamp outputs (pads, difficulty, pictorial) | Story | M | Host and GPIO lamps stay in sync |
| 13d. Run the same game binary on the Pi with GPIO selected | Story | S | Full session on real switches (or bench buttons) |

---

### 14. Ticket pulse stub on GPIO (hardware optional)
**Type:** User story  
**Effort:** S–M (4–8 h)  
**Maps to:** M7 hardware path; jam scenario

Pulse ticket-enable; count notches; stop on count or timeout; do not void earned tickets on jam. Without a hopper, simulate notch in software.

**Done when:** jam test stops pulsing and surfaces an error; earned count remains.

---

### 15. Pi kiosk run
**Type:** User story  
**Effort:** M (4–8 h)  
**Maps to:** long-running appliance

systemd (or equivalent) auto-start, restart on crash, log to file. Document power-loss: reboot to attract, no restored credit.

**Done when:** power cycle returns to attract and a credit can start a new game.

---

## Out of this plan (explicit)

Per [05-scope-and-sources.md](05-scope-and-sources.md): cabinet build, coin mech, real hopper, Deluxe bonus/voice/plasma, self-test lamp walk, marquee motor, copied audio/art.

## Effort roll-up

| Block | Steps | Approx. effort |
| --- | --- | --- |
| Foundation | 1–4 | 2–3 days |
| Gameplay + session | 5–6 | 6–9 days |
| Score, payout, host | 7–9 | 3–5 days |
| Tests + playtest | 10–12 | 3–5 days |
| **Software MVP subtotal** | **1–12** | **about 3–4 weeks** (one developer, focused) |
| Pi I/O + kiosk | 13–15 | 1–2 weeks including first-time wiring |

Part-time (evenings) stretches the software MVP to roughly 6–8 calendar weeks.

## Suggested next action

Start **step 1** (solution skeleton), then **step 2** (config + domain). Do not begin the target loop until the I/O port and fake clock exist, or tests and GPIO swaps will fight the design.
