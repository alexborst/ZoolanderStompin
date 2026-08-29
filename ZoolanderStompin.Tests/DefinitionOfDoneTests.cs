using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class DefinitionOfDoneTests
{
    [TestMethod]
    public void M1_session_returns_to_attract_and_is_not_stuck()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        var phases = new List<SessionPhase>();
        Record(driver, phases);

        driver.Tick();
        Record(driver, phases);
        Assert.AreEqual(SessionPhase.Attract, driver.Session.Phase);

        driver.PulseCredit();
        Record(driver, phases);
        Assert.AreEqual(SessionPhase.Select, driver.Session.Phase);

        driver.PulseDifficulty(Difficulty.Easy);
        Record(driver, phases);
        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);

        driver.SkipCountdown();
        Record(driver, phases);
        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);

        driver.MissCurrent();
        driver.FinishGap();
        driver.MissCurrent();
        Record(driver, phases);
        Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);

        driver.AdvanceAndTick(driver.Intermission);
        driver.Tick();
        Record(driver, phases);
        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);

        driver.MissCurrent();
        driver.FinishGap();
        driver.MissCurrent();
        Record(driver, phases);
        Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);

        driver.AdvanceAndTick(driver.ResultsHold);
        Record(driver, phases);

        Assert.AreEqual(SessionPhase.Attract, driver.Session.Phase);
        Assert.AreEqual(0, driver.Session.Credits);
        Assert.AreEqual(0, driver.Session.CurrentRound);
        var attract = PlayStatus.Format(driver.Session);
        StringAssert.Contains(attract, "Attract");
        StringAssert.Contains(attract, "hits --");
        StringAssert.Contains(attract, "round -");

        driver.PulseCredit();
        Assert.AreEqual(SessionPhase.Select, driver.Session.Phase);
        StringAssert.Contains(PlayStatus.Format(driver.Session), "hits --");

        CollectionAssert.AreEqual(
            new[]
            {
                SessionPhase.Attract,
                SessionPhase.Select,
                SessionPhase.Countdown,
                SessionPhase.Playing,
                SessionPhase.Intermission,
                SessionPhase.Playing,
                SessionPhase.Results,
                SessionPhase.Attract,
            },
            phases);
    }

    [TestMethod]
    public void M2_exactly_one_pad_is_lit_and_the_next_light_ends_the_window()
    {
        var driver = StartEasy(1, 3, 2, 4);
        driver.SkipCountdown();

        Assert.AreEqual(1, driver.Output.PadLampsOn.Count);
        Assert.AreEqual(1, driver.Session.LitPad?.Number);

        driver.Stomp(1);
        Assert.AreEqual(0, driver.Output.PadLampsOn.Count);
        Assert.IsNull(driver.Session.LitPad);

        driver.FinishGap();
        Assert.AreEqual(1, driver.Output.PadLampsOn.Count);
        Assert.AreEqual(3, driver.Session.LitPad?.Number);
        Assert.IsFalse(driver.Output.IsPadLampOn(new FloorPad(1)));
    }

    [TestMethod]
    public void M3_rising_edge_on_the_lit_pad_is_a_hit()
    {
        var driver = StartEasy(2, 1, 3, 4);
        driver.SkipCountdown();
        driver.Stomp(2);

        Assert.AreEqual(1, driver.Session.Score.Hits);
        Assert.AreEqual(0, driver.Session.Score.Misses);
        Assert.AreEqual(1, driver.Output.ScoreDigits);
        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
    }

    [TestMethod]
    public void M4_timeout_is_a_miss_and_shows_on_the_score_display()
    {
        var driver = StartEasy(1, 2, 3, 4);
        driver.SkipCountdown();
        driver.MissCurrent();

        Assert.AreEqual(0, driver.Session.Score.Hits);
        Assert.AreEqual(1, driver.Session.Score.Misses);
        Assert.AreEqual(0, driver.Output.ScoreDigits);
        var text = PlayStatus.Format(driver.Session);
        StringAssert.Contains(text, "hits 00");
        StringAssert.Contains(text, "misses 01");
        Assert.IsNull(driver.Output.TicketDigits);
    }

    [TestMethod]
    public void M5_difficulty_changes_pads_in_play_and_hit_windows()
    {
        var easyPicks = CollectLitPads(Difficulty.Easy, samples: 80);
        CollectionAssert.IsSubsetOf(easyPicks, new[] { 1, 2, 3, 4 });
        CollectionAssert.DoesNotContain(easyPicks, 5);
        CollectionAssert.DoesNotContain(easyPicks, 7);

        var hard = GameSessionDriver.Scripted(pads: [7, 1, 2, 3]);
        hard.Tick();
        hard.PulseCredit();
        hard.PulseDifficulty(Difficulty.Hard);
        hard.SkipCountdown();
        Assert.AreEqual(7, hard.Session.LitPad?.Number);

        AssertWindow(Difficulty.Easy, GameOptions.CreateDefault().Easy.HitWindowMilliseconds);
        AssertWindow(Difficulty.Hard, GameOptions.CreateDefault().Hard.HitWindowMilliseconds);
        AssertWindow(Difficulty.Medium, GameOptions.CreateDefault().Medium.HitWindowMilliseconds);
    }

    [TestMethod]
    public void M6_numeric_and_pictorial_update_on_hit_and_tickets_appear_at_end()
    {
        var driver = StartEasy(1, 2, 3, 4);
        driver.SkipCountdown();
        driver.Stomp(1);

        Assert.AreEqual(1, driver.Output.ScoreDigits);
        CollectionAssert.AreEqual(new[] { true, false, false, false }, driver.Output.PictorialLampsOn.ToArray());
        Assert.IsNull(driver.Output.TicketDigits);
        StringAssert.Contains(PlayStatus.Format(driver.Session), "tickets --");

        driver.FinishGap();
        driver.PlayUntilResults(hitEveryPresentation: true);
        Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
        Assert.AreEqual(8, driver.Output.TicketDigits);
        StringAssert.Contains(PlayStatus.Format(driver.Session), "tickets 08");
        StringAssert.Contains(PlayStatus.Format(driver.Session), "WIN");
    }

    [TestMethod]
    public void M7_tickets_come_from_hit_percent_or_fixed_config()
    {
        var table = GameResult.Evaluate(new Score(2, 2), GameOptions.CreateDefault());
        Assert.AreEqual(50, table.HitPercent);
        Assert.AreEqual(2, table.Tickets);
        Assert.IsFalse(table.Won);

        var sweep = GameResult.Evaluate(new Score(40, 0), GameOptions.CreateDefault());
        Assert.AreEqual(100, sweep.HitPercent);
        Assert.AreEqual(8, sweep.Tickets);
        Assert.IsTrue(sweep.Won);

        var options = GameOptions.CreateDefault();
        options.Payout.Mode = PayoutMode.Fixed;
        options.Payout.FixedTickets = 3;
        var fixedPayout = GameResult.Evaluate(new Score(0, 8), options);
        Assert.AreEqual(3, fixedPayout.Tickets);
        Assert.IsFalse(fixedPayout.Won);
    }

    [TestMethod]
    public void M8_wrong_pad_held_pad_bounce_no_pick_and_extra_credits_do_not_corrupt()
    {
        var wrong = StartEasy(2, 1, 3, 4);
        wrong.SkipCountdown();
        wrong.Stomp(1);
        Assert.AreEqual(0, wrong.Session.Score.Hits);
        Assert.IsTrue(wrong.Output.IsPadLampOn(new FloorPad(2)));
        wrong.Stomp(1, 2);
        Assert.AreEqual(1, wrong.Session.Score.Hits);
        Assert.AreEqual(0, wrong.Session.Score.Misses);

        var held = StartEasy(1, 2, 3, 4);
        held.SkipToPlayingUnlit();
        held.SetPads(1);
        held.Tick();
        held.StabilizeHeldPads();
        Assert.AreEqual(0, held.Session.Score.Hits);
        Assert.AreEqual(1, held.Session.LitPad?.Number);
        held.MissCurrent();
        Assert.AreEqual(1, held.Session.Score.Misses);

        var bounce = StartEasy(1, 2, 3, 4);
        bounce.SkipCountdown();
        bounce.SetPads(1);
        bounce.Tick();
        bounce.Clock.Advance(TimeSpan.FromMilliseconds(10));
        bounce.SetPads();
        bounce.Tick();
        bounce.SetPads(1);
        bounce.Tick();
        bounce.Clock.Advance(TimeSpan.FromMilliseconds(10));
        bounce.Tick();
        Assert.AreEqual(0, bounce.Session.Score.Hits);
        Assert.AreEqual(SessionPhase.Playing, bounce.Session.Phase);
        Assert.AreEqual(1, bounce.Session.LitPad?.Number);

        var timeout = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        timeout.Tick();
        timeout.PulseCredit();
        timeout.AdvanceAndTick(timeout.SelectTimeout);
        Assert.AreEqual(SessionPhase.Countdown, timeout.Session.Phase);
        Assert.AreEqual(Difficulty.Easy, timeout.Session.SelectedDifficulty);

        var options = GameSessionDriver.CreateShortSession();
        options.RoundCount = 1;
        var extra = GameSessionDriver.Scripted(options, 1, 2);
        extra.Tick();
        extra.PulseCredit();
        extra.PulseDifficulty(Difficulty.Easy);
        extra.SkipCountdown();
        extra.PulseCredit();
        Assert.AreEqual(SessionPhase.Playing, extra.Session.Phase);
        Assert.AreEqual(1, extra.Session.Credits);
        extra.PlayUntilResults(hitEveryPresentation: false);
        extra.AdvanceAndTick(extra.ResultsHold);
        Assert.AreEqual(SessionPhase.Select, extra.Session.Phase);
        Assert.AreEqual(1, extra.Session.Credits);
        Assert.AreEqual(0, extra.Session.Score.Hits);
        Assert.AreEqual(0, extra.Session.CurrentRound);
    }

    [TestMethod]
    public void M9_tunables_live_in_frozen_host_config_not_magic_numbers()
    {
        var json = File.ReadAllText("host-appsettings.json");
        using var document = System.Text.Json.JsonDocument.Parse(
            json,
            new System.Text.Json.JsonDocumentOptions
            {
                CommentHandling = System.Text.Json.JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            });
        var fromFile = GameOptions.FromJson(document.RootElement.GetProperty("Game").GetRawText());
        GameOptionsTests.AssertFrozenMatch(GameOptions.CreateDefault(), fromFile);

        var tuned = GameOptions.FromJson(
            """
            {
              "debounceMilliseconds": 30,
              "presentationsPerRound": 10,
              "roundCount": 3,
              "preventConsecutiveRepeat": true,
              "interTargetGapMilliseconds": 250,
              "winPercentThreshold": 60,
              "selectTimeoutSeconds": 30,
              "selectTimeoutAction": "AutoStartEasy",
              "countdownGetReadyMilliseconds": 1500,
              "countdownGoMilliseconds": 700,
              "intermissionMilliseconds": 2000,
              "resultsMilliseconds": 3000,
              "attractLampCycleMilliseconds": 400,
              "freePlay": false,
              "coinsPerCredit": 1,
              "easy": { "padsInPlay": [1, 2], "hitWindowMilliseconds": 1500 },
              "medium": { "padsInPlay": [1, 2, 3, 4, 5, 6], "hitWindowMilliseconds": 900 },
              "hard": { "padsInPlay": [1, 2, 3, 4, 5, 6, 7], "hitWindowMilliseconds": 650 },
              "payout": {
                "mode": "Fixed",
                "fixedTickets": 5,
                "table": [
                  { "minPercentInclusive": 0, "maxPercentInclusive": 100, "tickets": 1 }
                ]
              }
            }
            """);

        Assert.AreEqual(10, tuned.PresentationsPerRound);
        Assert.AreEqual(3, tuned.RoundCount);
        CollectionAssert.AreEqual(new[] { 1, 2 }, tuned.Easy.PadsInPlay);
        Assert.AreEqual(1500, tuned.Easy.HitWindowMilliseconds);
        Assert.AreEqual(PayoutMode.Fixed, tuned.Payout.Mode);
        Assert.AreEqual(5, tuned.Payout.TicketsForHitPercent(0));
    }

    private static GameSessionDriver StartEasy(params int[] pads)
    {
        var driver = GameSessionDriver.Scripted(pads: pads);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        return driver;
    }

    private static void Record(GameSessionDriver driver, List<SessionPhase> phases)
    {
        if (phases.Count == 0 || phases[^1] != driver.Session.Phase)
        {
            phases.Add(driver.Session.Phase);
        }
    }

    private static int[] CollectLitPads(Difficulty difficulty, int samples)
    {
        var options = GameOptions.CreateDefault();
        options.PresentationsPerRound = samples;
        options.RoundCount = 1;
        options.InterTargetGapMilliseconds = 50;
        options.Easy.HitWindowMilliseconds = 80;
        options.Medium.HitWindowMilliseconds = 80;
        options.Hard.HitWindowMilliseconds = 80;
        var driver = new GameSessionDriver(new RandomPadPicker(new Random(42)), options);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(difficulty);
        driver.SkipCountdown();

        var seen = new HashSet<int>();
        while (driver.Session.Phase == SessionPhase.Playing && driver.Session.LitPad is { } pad)
        {
            seen.Add(pad.Number);
            driver.MissCurrent();
            if (driver.Session.Phase == SessionPhase.Playing)
            {
                driver.FinishGap();
            }
        }

        return seen.ToArray();
    }

    private static void AssertWindow(Difficulty difficulty, int milliseconds)
    {
        var options = GameOptions.CreateDefault();
        options.PresentationsPerRound = 2;
        var pad = options.For(difficulty).PadsInPlay[0];
        var driver = TargetLoopDriver.Scripted(options, difficulty, pad, pad == 1 ? 2 : 1);
        driver.LightFirst();

        driver.Clock.Advance(TimeSpan.FromMilliseconds(milliseconds - 1));
        driver.Tick();
        Assert.AreEqual(TargetLoopPhase.Presenting, driver.Loop.Phase);
        Assert.AreEqual(0, driver.Loop.Score.Misses);

        driver.AdvanceAndTick(TimeSpan.FromMilliseconds(1));
        Assert.AreEqual(1, driver.Loop.Score.Misses);
        Assert.IsNull(driver.Loop.LitPad);
    }
}
