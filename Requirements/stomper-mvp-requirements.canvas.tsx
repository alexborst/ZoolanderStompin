import {
  Callout,
  Card,
  CardBody,
  CardHeader,
  Divider,
  Grid,
  H1,
  H2,
  H3,
  Pill,
  Row,
  Stack,
  Stat,
  Table,
  Text,
  computeDAGLayout,
  useCanvasState,
  useHostTheme,
} from "cursor/canvas";

type TabId = "done" | "session" | "software" | "scenarios" | "scope";

const TABS: { id: TabId; label: string }[] = [
  { id: "done", label: "Definition of done" },
  { id: "session", label: "How a session plays" },
  { id: "software", label: "Software behavior" },
  { id: "scenarios", label: "Scenario matrix" },
  { id: "scope", label: "MVP scope and gaps" },
];

const SESSION_LAYOUT = computeDAGLayout({
  nodes: [
    { id: "Attract" },
    { id: "Select" },
    { id: "Countdown" },
    { id: "Round" },
    { id: "Intermission" },
    { id: "Results" },
  ],
  edges: [
    { from: "Attract", to: "Select" },
    { from: "Select", to: "Countdown" },
    { from: "Countdown", to: "Round" },
    { from: "Round", to: "Intermission" },
    { from: "Intermission", to: "Round" },
    { from: "Round", to: "Results" },
    { from: "Results", to: "Attract" },
    { from: "Select", to: "Attract" },
  ],
  direction: "horizontal",
  nodeWidth: 124,
  nodeHeight: 36,
  rankGap: 36,
  nodeGap: 28,
  padding: 12,
});

const TARGET_LAYOUT = computeDAGLayout({
  nodes: [
    { id: "Pick target" },
    { id: "Light + window" },
    { id: "Hit" },
    { id: "Miss" },
    { id: "Ignore" },
    { id: "Next" },
  ],
  edges: [
    { from: "Pick target", to: "Light + window" },
    { from: "Light + window", to: "Hit" },
    { from: "Light + window", to: "Miss" },
    { from: "Light + window", to: "Ignore" },
    { from: "Hit", to: "Next" },
    { from: "Miss", to: "Next" },
    { from: "Ignore", to: "Light + window" },
    { from: "Next", to: "Pick target" },
  ],
  direction: "horizontal",
  nodeWidth: 118,
  nodeHeight: 36,
  rankGap: 28,
  nodeGap: 22,
  padding: 12,
});

function Graph({
  layout,
  labels,
  nodeWidth,
  nodeHeight,
}: {
  layout: ReturnType<typeof computeDAGLayout>;
  labels: Record<string, string>;
  nodeWidth: number;
  nodeHeight: number;
}) {
  const theme = useHostTheme();
  return (
    <svg
      width="100%"
      viewBox={`0 0 ${layout.width} ${layout.height}`}
      role="img"
      style={{ display: "block", maxWidth: layout.width }}
    >
      {layout.edges.map((e, i) => (
        <line
          key={`${e.from}-${e.to}-${i}`}
          x1={e.sourceX}
          y1={e.sourceY}
          x2={e.targetX}
          y2={e.targetY}
          stroke={theme.stroke.secondary}
          strokeWidth={1.5}
          strokeDasharray={e.isBackEdge ? "4 3" : undefined}
        />
      ))}
      {layout.nodes.map((n) => (
        <g key={n.id}>
          <rect
            x={n.x}
            y={n.y}
            width={nodeWidth}
            height={nodeHeight}
            rx={4}
            fill={theme.fill.secondary}
            stroke={theme.stroke.primary}
          />
          <text
            x={n.x + nodeWidth / 2}
            y={n.y + nodeHeight / 2 + 4}
            textAnchor="middle"
            fill={theme.text.primary}
            fontSize={11}
            fontFamily="inherit"
          >
            {labels[n.id] ?? n.id}
          </text>
        </g>
      ))}
    </svg>
  );
}

export default function StomperMvpRequirements() {
  const [tab, setTab] = useCanvasState<TabId>("stomper-tab", "done");

  return (
    <Stack gap={20}>
      <Stack gap={8}>
        <H1>Floor-stomp redemption MVP requirements</H1>
        <Text tone="secondary">
          Software definition of done for a custom-themed game that follows the
          1994 Jaleco / Island Design Spider Stompin' rule set. Mechanics and
          operator behavior only — original name, art, and audio are out of
          scope.
        </Text>
        <Text size="small" tone="tertiary">
          Sources: Island Design 1995 installation manual (via Wikipedia), NJ
          amusement certification 2-306, 1996 Play Meter ad copy, PrimeTime
          Amusements listing, Island Design Spider Stompin' Deluxe 1996
          owner's manual (used only where the 1994 manual is not public).
        </Text>
      </Stack>

      <Row gap={8} wrap>
        {TABS.map((t) => (
          <span key={t.id}>
            <Pill active={tab === t.id} onClick={() => setTab(t.id)}>
              {t.label}
            </Pill>
          </span>
        ))}
      </Row>

      {tab === "done" && <DoneTab />}
      {tab === "session" && <SessionTab />}
      {tab === "software" && <SoftwareTab />}
      {tab === "scenarios" && <ScenariosTab />}
      {tab === "scope" && <ScopeTab />}
    </Stack>
  );
}

function DoneTab() {
  return (
    <Stack gap={20}>
      <Callout tone="info" title="When the software MVP is done">
        A player can take a credit from attract through difficulty select,
        complete every presentation in every round, see hits and misses update
        live, finish with a win/lose result, and receive a ticket count derived
        from hit percentage — and the software still behaves correctly if they
        pick nothing, stomp the wrong pad, stand on a pad, or time out.
      </Callout>

      <Grid columns={4} gap={12}>
        <Stat value="7" label="Floor targets" />
        <Stat value="3" label="Difficulties" />
        <Stat value="1" label="Lit target at a time" />
        <Stat value="Hit / miss" label="Scoring model" />
      </Grid>

      <H2>Must-pass capabilities</H2>
      <Text>
        These are the acceptance criteria. If any row fails, the MVP is not
        done. Physical ticket hardware, coin mechs, and cabinet art are not
        required for this software bar — stubs are enough if the behavior is
        real.
      </Text>
      <Table
        striped
        headers={["ID", "Capability", "Done looks like"]}
        columnAlign={["left", "left", "left"]}
        rows={[
          [
            "M1",
            "Session state machine",
            "Attract, credit, select, countdown, play, intermission, results, then back to attract — no stuck states.",
          ],
          [
            "M2",
            "One-at-a-time targets",
            "Exactly one pad is lit. Lighting another pad ends the previous window.",
          ],
          [
            "M3",
            "Hit-while-lit scoring",
            "A rising-edge press on the lit pad inside the window scores one hit. Force does not matter.",
          ],
          [
            "M4",
            "Misses are first-class",
            "A timeout with no valid hit increments miss count and is shown on the score display.",
          ],
          [
            "M5",
            "Difficulty changes the field",
            "Easy / Medium / Hard change how many pads can light and how long each window lasts.",
          ],
          [
            "M6",
            "Live dual feedback",
            "Numeric hit count and a simple pictorial meter both update on every hit. Ticket count appears at end.",
          ],
          [
            "M7",
            "Percentage payout",
            "Tickets = operator table or formula on hits / (hits + misses). Fixed payout is an alternate config.",
          ],
          [
            "M8",
            "Edge-case input",
            "Wrong pad, held pad, bounce, no difficulty chosen, and extra credits do not corrupt score or state.",
          ],
          [
            "M9",
            "Configurable tunables",
            "Windows, pads-in-play, rounds, presentations, and payout live in config — not hardcoded magic numbers.",
          ],
        ]}
        rowTone={[
          "info",
          "success",
          "success",
          "success",
          "info",
          "info",
          "warning",
          "warning",
          "neutral",
        ]}
      />
      <Text size="small" tone="tertiary">
        Source: NJ cert 2-306 (hit when lit, hits and misses both register,
        payout from percentage); Wikipedia citing the 1995 Island Design
        manual (one-at-a-time lights, three difficulties, force-independent
        score, numeric + visual display).
      </Text>

      <H2>What “same game” means for this build</H2>
      <Grid columns={2} gap={16}>
        <Stack gap={8}>
          <H3>In the contract</H3>
          <Text>
            Rules, timing model, input semantics, scoring, difficulty effect,
            round structure, attract/credit/result loop, and operator payout
            modes.
          </Text>
        </Stack>
        <Stack gap={8}>
          <H3>Not in the contract</H3>
          <Text>
            The Spider Stompin' name, queen/spider art, original voice lines,
            drum/crunch samples, or a pixel-identical cabinet. Theme and
            naming are yours.
          </Text>
        </Stack>
      </Grid>
    </Stack>
  );
}

function SessionTab() {
  return (
    <Stack gap={20}>
      <H2>A full credit, start to finish</H2>
      <Text>
        Confirmed from original marketing, the NJ rule description, and the
        Deluxe voice script (which still names round 2, Get Ready / GO, and a
        win vs spiders-win ending). Dashed arrows are returns.
      </Text>
      <Graph
        layout={SESSION_LAYOUT}
        nodeWidth={124}
        nodeHeight={36}
        labels={{
          Attract: "Attract",
          Select: "Pick difficulty",
          Countdown: "Get ready",
          Round: "Play round",
          Intermission: "Next round",
          Results: "Payout",
        }}
      />

      <Table
        striped
        headers={["Phase", "What the player experiences", "What software must do"]}
        rows={[
          [
            "Attract",
            "Machine is idle. Lights cycle. Decorative motion and occasional call-in audio.",
            "Cycle pad and marquee lights. Ignore floor stomps for scoring. Accept coin/start. Do not award tickets.",
          ],
          [
            "Credit",
            "Coin takes. Difficulty buttons light or flash. Prompt to pick Easy, Medium, or Hard.",
            "Increment credits. Decrement one credit when a difficulty is accepted. Stack extra coins.",
          ],
          [
            "Select",
            "Player chooses a difficulty. If they walk away, the credit is not left hanging forever.",
            "Arm Easy/Medium/Hard inputs. On timeout, either auto-start Easy or return the credit to attract — operator choice.",
          ],
          [
            "Countdown",
            "Short Get Ready, then GO.",
            "Lock difficulty. Zero hit/miss. Play countdown audio. Do not score stomps yet.",
          ],
          [
            "Round play",
            "One pad lights at random from the difficulty's active set. Stomp it before it goes out. Repeat.",
            "Run the target loop until the round's presentation count is done. Update score displays on every resolution.",
          ],
          [
            "Intermission",
            "If more rounds remain, a brief pause and a 'round N' cue, then play continues, typically faster or denser.",
            "Announce next round. Optionally tighten the hit window. Resume the target loop.",
          ],
          [
            "Results",
            "Game-end cue. Win or lose line. Ticket count shown, then tickets (or a stubbed count) pay out.",
            "Compute percentage. Map to tickets. Show numeric tickets. Drive dispenser until notch count matches, or display the stub.",
          ],
        ]}
      />
      <Text size="small" tone="tertiary">
        Source: NJ cert 2-306; PrimeTime / Play Meter 1996 (“seven spiders”,
        three difficulties, numeric + picture score, adjustable tickets);
        Deluxe manual game description and self-test phrase list.
      </Text>

      <H2>The core rule, one sentence</H2>
      <Callout tone="neutral">
        A presentation is a randomly chosen floor target that lights for a
        limited time; stomping that target while it is lit is a hit, letting
        the window expire is a miss, and tickets come from the resulting hit
        percentage — not from how hard the player stomps.
      </Callout>

      <H2>Difficulty</H2>
      <Text>
        Original documentation is explicit that Easy / Medium / Hard change
        two things: how many targets are eligible to light, and how long each
        light stays on. Exact pad maps and millisecond windows were never
        published. Treat the numbers below as MVP starting values, not
        recovered ROM values.
      </Text>
      <Table
        striped
        headers={[
          "Difficulty",
          "Pads in play (proposed)",
          "Hit window (proposed)",
          "Intent",
        ]}
        rows={[
          ["Easy", "4 of 7", "1200 ms", "Toddlers can reach and still score"],
          ["Medium", "6 of 7", "900 ms", "Default family play"],
          ["Hard", "All 7", "650 ms", "Requires moving around the octagon"],
        ]}
      />
      <Text size="small" tone="tertiary">
        Proposed defaults for the software MVP. Replace after measuring an
        original machine or playtesting. Wikipedia (1995 manual): difficulties
        “adjust the number of spiders in play and the amount of time players
        have to press each button.”
      </Text>
    </Stack>
  );
}

function SoftwareTab() {
  return (
    <Stack gap={20}>
      <H2>Target loop (while a round is live)</H2>
      <Text>
        This is the inner machine the rest of the product exists to serve.
        Ignore-paths (wrong pad, bounce, already-held pad) must not consume
        the window.
      </Text>
      <Graph
        layout={TARGET_LAYOUT}
        nodeWidth={118}
        nodeHeight={36}
        labels={{
          "Pick target": "Pick target",
          "Light + window": "Lit window",
          Hit: "Hit",
          Miss: "Miss",
          Ignore: "Ignore input",
          Next: "Resolve",
        }}
      />

      <H3>Resolution rules</H3>
      <Table
        striped
        headers={["Event during lit window", "Software response"]}
        rows={[
          [
            "Rising edge on the lit pad",
            "Hit. Light off immediately. Increment hits. Play success cue. Advance.",
          ],
          [
            "Window expires with no valid hit",
            "Miss. Light off. Increment misses. Play miss cue. Advance.",
          ],
          [
            "Rising edge on any other pad",
            "Ignore. Do not score. Window keeps running so a later correct stomp can still hit.",
          ],
          [
            "Pad already held down when it lights",
            "Ignore until release, then a new press. Standing on a pad must not auto-score.",
          ],
          [
            "Switch bounce / double-fire",
            "Debounce (target ~20–40 ms). One stomp is one event.",
          ],
          [
            "Two pads claimed at once",
            "Accept only the lit pad. Others ignored.",
          ],
        ]}
        rowTone={["success", "danger", "neutral", "warning", "info", "neutral"]}
      />
      <Text size="small" tone="tertiary">
        Hit-while-lit and force-independence are original. Wrong-pad ignore
        and rising-edge-after-light are recommended floor-game defaults; they
        are not printed in the surviving 1994 docs.
      </Text>

      <H2>Displays the software must drive</H2>
      <Grid columns={2} gap={16}>
        <Card>
          <CardHeader trailing={<Pill size="sm" active>Original</Pill>}>
            Player-facing
          </CardHeader>
          <CardBody>
            <Stack gap={8}>
              <Text>
                Two-digit numeric hits. Two-digit tickets at end. Pictorial
                score of four cartoon lamps so young children can see progress
                without reading numbers. Difficulty button lamps. One lamp per
                floor pad.
              </Text>
              <Text size="small" tone="tertiary">
                PrimeTime listing; Deluxe wiring (7 foot lights, 4 cartoon
                lights, Easy/Medium/Hard lights).
              </Text>
            </Stack>
          </CardBody>
        </Card>
        <Card>
          <CardHeader trailing={<Pill size="sm">Stub OK</Pill>}>
            Operator-facing
          </CardHeader>
          <CardBody>
            <Stack gap={8}>
              <Text>
                Coin counter, ticket counter, credit add without bumping the
                coin meter, self-test entry. Ticket-notch feedback so payout
                stops when the physical count matches. Low-ticket warning if
                a dispenser is attached.
              </Text>
              <Text size="small" tone="tertiary">
                Deluxe service panel and dispenser door. Software MVP can log
                these instead of driving hardware.
              </Text>
            </Stack>
          </CardBody>
        </Card>
      </Grid>

      <H2>I/O contract (for later device choice)</H2>
      <Text>
        The device only has to satisfy this map. GPIO-capable boards, arcade
        I/O boards, or a keyboard simulator all work if the game loop sees
        the same events.
      </Text>
      <Table
        striped
        headers={["Direction", "Count", "Signal"]}
        rows={[
          ["In", "7", "Floor pad switches"],
          ["In", "3", "Easy / Medium / Hard"],
          ["In", "1+", "Coin or start (plus service credit, test)"],
          ["In", "1", "Ticket notch (optional until hardware)"],
          ["Out", "7", "Floor pad lamps"],
          ["Out", "3", "Difficulty button lamps"],
          ["Out", "4", "Pictorial score lamps"],
          ["Out", "2 groups", "Score digits and ticket digits (or one screen)"],
          ["Out", "1", "Audio"],
          ["Out", "1", "Ticket enable (optional until hardware)"],
        ]}
      />
    </Stack>
  );
}

function ScenariosTab() {
  return (
    <Stack gap={20}>
      <H2>Scenarios the software must survive</H2>
      <Text>
        Happy path plus the ways a kid actually uses a floor game. Each row is
        a test the MVP should pass before calling gameplay done.
      </Text>
      <Table
        striped
        stickyHeader
        headers={["Scenario", "Expected software behavior"]}
        rows={[
          [
            "Idle attract, no credit",
            "Lights and audio only. Floor stomps do not score or start a game.",
          ],
          [
            "One coin, one credit, pick Medium",
            "Countdown, then Medium pad-set and window. One credit consumed.",
          ],
          [
            "Two coins stacked during attract",
            "After game 1 results, return to difficulty select with one credit remaining — not attract.",
          ],
          [
            "Coin during an active game",
            "Bank the extra credit. Do not restart or add time to the current session.",
          ],
          [
            "Credit taken, nobody picks a difficulty",
            "After the configured wait (Deluxe: about 22–64 s), auto-start Easy or dump back to attract.",
          ],
          [
            "Free play",
            "Start is always available. Still require a difficulty press so the session has a defined field.",
          ],
          [
            "Correct stomp, light still on",
            "Immediate hit. Light off. Next target after a short gap.",
          ],
          [
            "Correct stomp a moment after the light dies",
            "Miss already recorded. This press is ignored. No retroactive hit.",
          ],
          [
            "Stomp a dark pad, then the lit one in time",
            "Wrong pad ignored. Correct pad still counts as a hit.",
          ],
          [
            "Stand on a pad that later lights",
            "No hit until the player lifts and presses again during the window.",
          ],
          [
            "Two people cooperating on Hard",
            "Legal. Original is single-player intended but does not lock out extra bodies. Score whoever closes the switch.",
          ],
          [
            "Every presentation missed",
            "Hit rate 0%. Lose result. Tickets follow the payout table (often a consolation 0 or 1 — operator set).",
          ],
          [
            "Every presentation hit",
            "Hit rate 100%. Win result. Max tickets for that table.",
          ],
          [
            "Round 1 ends with more rounds configured",
            "Intermission cue, then round 2 with the same difficulty. Hits/misses accumulate for payout.",
          ],
          [
            "Last round ends",
            "Stop lighting pads. Show percentage, win/lose, ticket digits, then pay (or stub).",
          ],
          [
            "Ticket dispenser jammed / notch never arrives",
            "Stop enabling the motor after a timeout. Show an error. Do not loop forever. Do not void the earned count.",
          ],
          [
            "Power loss mid-game",
            "On reboot, attract. Do not restore a half-played credit. Coin meter is hardware-side.",
          ],
          [
            "Service credit",
            "Adds a playable credit without incrementing the coin counter.",
          ],
        ]}
        rowTone={[
          "neutral",
          "success",
          "info",
          "info",
          "warning",
          "info",
          "success",
          "danger",
          "success",
          "warning",
          "neutral",
          "danger",
          "success",
          "info",
          "success",
          "warning",
          "neutral",
          "neutral",
        ]}
      />
      <Text size="small" tone="tertiary">
        Timeout-if-no-select and coins-per-credit values: Deluxe DIP bank 2.
        Cooperation exploit: Wikipedia. Ticket-notch stop condition: Deluxe
        self-test and dispenser wiring.
      </Text>

      <H2>Payout model</H2>
      <Callout tone="warning" title="Two legal modes, one MVP formula">
        Operators could pay a fixed ticket count or scale tickets by hit
        percentage. NJ certification for the original describes percentage of
        stomps. MVP should implement percentage with a lookup table, plus a
        config switch for a flat ticket count.
      </Callout>
      <Text>
        Percentage = hits / (hits + misses). Presentations that were never
        shown do not enter the denominator. A proposed table until an original
        DIP chart is recovered:
      </Text>
      <Table
        headers={["Hit rate", "Tickets (proposed starting table)"]}
        columnAlign={["left", "right"]}
        rows={[
          ["0–19%", "0"],
          ["20–39%", "1"],
          ["40–59%", "2"],
          ["60–79%", "4"],
          ["80–99%", "6"],
          ["100%", "8"],
        ]}
      />
      <Text size="small" tone="tertiary">
        Invented for MVP so payout is testable. Replace with measured original
        DIP bank 1 values. Deluxe also had a bonus-mode DIP that the 1994
        game did not advertise — leave bonus out of MVP.
      </Text>
    </Stack>
  );
}

function ScopeTab() {
  return (
    <Stack gap={20}>
      <H2>In MVP vs later</H2>
      <Grid columns={2} gap={16}>
        <Card>
          <CardHeader>Software MVP includes</CardHeader>
          <CardBody>
            <Stack gap={6}>
              <Text>Full session and target state machines.</Text>
              <Text>Three difficulties with pads-in-play and windows.</Text>
              <Text>Hit / miss / ignore / rising-edge rules.</Text>
              <Text>Multi-round play with accumulated percentage.</Text>
              <Text>Numeric + simple pictorial score.</Text>
              <Text>Win/lose + ticket result (hardware stub allowed).</Text>
              <Text>Attract, credit stacking, select timeout, free play.</Text>
              <Text>Config file for every unpublished number.</Text>
              <Text>Simulated pads (keys or on-screen) so it is playable before the cabinet exists.</Text>
            </Stack>
          </CardBody>
        </Card>
        <Card>
          <CardHeader>Not required to call MVP done</CardHeader>
          <CardBody>
            <Stack gap={6}>
              <Text>Physical platform, coin door, or ticket hopper.</Text>
              <Text>Deluxe plasma display, queen voice, or bonus mode.</Text>
              <Text>Adaptive mid-game difficulty (“programmed intelligence” is a Deluxe claim).</Text>
              <Text>Copied audio, art, or character script.</Text>
              <Text>Self-test lamp walk (useful later, not gameplay-done).</Text>
              <Text>Marquee / spinner motor control.</Text>
              <Text>Dual ticket dispensers.</Text>
              <Text>Final device selection — only the I/O contract.</Text>
            </Stack>
          </CardBody>
        </Card>
      </Grid>

      <H2>Unpublished numbers (block exact ROM parity, not an MVP)</H2>
      <Text>
        The original 1995 installation manual is cited but not publicly
        transcribed. These values must be config until someone times a
        working machine or we playtest them into place.
      </Text>
      <Table
        striped
        headers={["Gap", "What we know", "MVP default"]}
        rows={[
          [
            "Hit windows",
            "Harder = less time. No milliseconds published.",
            "1200 / 900 / 650 ms",
          ],
          [
            "Pads per difficulty",
            "Harder = more spiders in play. Mapping unknown.",
            "4 / 6 / 7 of 7",
          ],
          [
            "Presentations per round",
            "Score display is two digits, so under 100 hits.",
            "20 per round",
          ],
          [
            "Round count",
            "Deluxe DIP selects rounds; voice names round 2.",
            "2 rounds",
          ],
          [
            "No-repeat target",
            "Not documented.",
            "Never light the same pad twice in a row",
          ],
          [
            "Inter-target gap",
            "Not documented.",
            "250 ms dark between presentations",
          ],
          [
            "Win vs lose line",
            "Deluxe has both phrases. Threshold unknown.",
            "Win at ≥ 60% hits",
          ],
          [
            "Select timeout",
            "Deluxe: ~22–64 seconds, auto-Easy or attract.",
            "30 s, then auto-Easy",
          ],
        ]}
      />

      <H2>Source split</H2>
      <Table
        striped
        headers={["Claim", "Authority"]}
        rows={[
          [
            "Choose difficulty, randomly lit button, score on hit-when-lit, register hits and misses, pay on percentage",
            "NJ cert 2-306 (original game)",
          ],
          [
            "Seven floor spiders, three difficulties, numeric + picture score, adjustable tickets, two-digit displays",
            "1996 Play Meter / PrimeTime (original cabinet)",
          ],
          [
            "One light at a time, force does not change score, difficulties change pad count and time",
            "Wikipedia citing 1995 Island Design manual",
          ],
          [
            "Get Ready / GO, round 2, win vs spiders-win, ticket notch, 7 foot switches, 4 cartoon lamps, DIP payout / rounds / coins",
            "Deluxe 1996 manual — same designer, treat as analog not gospel",
          ],
          [
            "European 6-spider listings",
            "Likely clones. Do not use. Original I/O is 7 pads.",
          ],
        ]}
        rowTone={["success", "success", "success", "info", "danger"]}
      />

      <Callout tone="info" title="Suggested next step after this brief">
        Lock the unpublished numbers as a config schema, then pick a device
        that can debounce 7+3 digital inputs, drive 7+ pad lamps, play audio,
        and later pulse a ticket hopper. Programming can start against a
        keyboard simulator of that same I/O map.
      </Callout>
    </Stack>
  );
}
