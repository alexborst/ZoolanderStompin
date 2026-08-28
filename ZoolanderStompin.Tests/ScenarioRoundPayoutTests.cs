using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class ScenarioRoundPayoutTests
{
    [TestMethod]
    public void Every_presentation_missed_is_zero_percent_a_loss_and_table_tickets()
    {
        var driver = StartAndFinish(hitEveryPresentation: false);

        Assert.AreEqual(0, driver.Session.Score.HitPercent);
        Assert.IsFalse(driver.Session.Result?.Won);
        Assert.AreEqual(0, driver.Session.Result?.Tickets);
        Assert.AreEqual(0, driver.Output.TicketDigits);
    }

    [TestMethod]
    public void Every_presentation_hit_is_one_hundred_percent_a_win_and_max_tickets()
    {
        var driver = StartAndFinish(hitEveryPresentation: true);

        Assert.AreEqual(100, driver.Session.Score.HitPercent);
        Assert.IsTrue(driver.Session.Result?.Won);
        Assert.AreEqual(8, driver.Session.Result?.Tickets);
        Assert.AreEqual(8, driver.Output.TicketDigits);
    }

    [TestMethod]
    public void Round_one_intermission_then_round_two_with_accumulated_score()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();
        driver.HitCurrent();
        driver.FinishGap();
        driver.MissCurrent();

        Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);
        Assert.AreEqual(1, driver.Session.CurrentRound);
        Assert.AreEqual(1, driver.Session.Score.Hits);
        Assert.AreEqual(1, driver.Session.Score.Misses);

        driver.AdvanceAndTick(driver.Intermission);
        driver.Tick();

        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        Assert.AreEqual(2, driver.Session.CurrentRound);
        Assert.AreEqual(Difficulty.Easy, driver.Session.SelectedDifficulty);
        Assert.AreEqual(1, driver.Session.Score.Hits);
    }

    [TestMethod]
    public void Last_round_stops_lighting_pads_and_shows_percentage_outcome_and_tickets()
    {
        var driver = StartAndFinish(hitEveryPresentation: true);

        Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
        Assert.IsNull(driver.Session.LitPad);
        Assert.AreEqual(0, driver.Output.PadLampsOn.Count);
        StringAssert.Contains(PlayStatus.Format(driver.Session), "WIN");
        StringAssert.Contains(PlayStatus.Format(driver.Session), "100%");
        Assert.AreEqual(8, driver.Output.TicketDigits);
        Assert.IsTrue(driver.Output.TicketEnable);
    }

    [TestMethod]
    public void Ticket_notches_never_arriving_does_not_block_or_void_the_stub_payout()
    {
        var driver = StartAndFinish(hitEveryPresentation: true);
        var earned = driver.Session.Result?.Tickets;

        Assert.AreEqual(8, earned);
        Assert.IsTrue(driver.Output.TicketEnable);

        driver.AdvanceAndTick(driver.ResultsHold);

        Assert.AreNotEqual(SessionPhase.Results, driver.Session.Phase);
        Assert.AreEqual(8, earned);
        Assert.IsFalse(driver.Output.TicketEnable);
    }

    [TestMethod]
    public void Unshown_presentations_are_not_in_the_payout_denominator()
    {
        var result = GameResult.Evaluate(new Score(3, 1), GameOptions.CreateDefault());

        Assert.AreEqual(75, result.HitPercent);
        Assert.AreEqual(4, result.Tickets);
        Assert.IsTrue(result.Won);
    }

    private static GameSessionDriver StartAndFinish(bool hitEveryPresentation)
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.PlayUntilResults(hitEveryPresentation);
        return driver;
    }
}
