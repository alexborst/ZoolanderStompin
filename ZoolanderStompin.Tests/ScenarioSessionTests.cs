using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class ScenarioSessionTests
{
    [TestMethod]
    public void Idle_attract_stomps_do_not_score_or_start_a_game()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2]);
        driver.Tick();
        driver.Stomp(1);

        Assert.AreEqual(SessionPhase.Attract, driver.Session.Phase);
        Assert.AreEqual(0, driver.Session.Credits);
        Assert.AreEqual(0, driver.Session.Score.Hits);
        Assert.AreEqual(0, driver.Session.Score.Misses);
    }

    [TestMethod]
    public void One_coin_and_medium_consumes_the_credit_and_uses_the_medium_set()
    {
        var driver = GameSessionDriver.Scripted(pads: [5, 6, 5, 6]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Medium);
        driver.SkipCountdown();

        Assert.AreEqual(0, driver.Session.Credits);
        Assert.AreEqual(Difficulty.Medium, driver.Session.SelectedDifficulty);
        Assert.AreEqual(5, driver.Session.LitPad?.Number);
        Assert.IsTrue(driver.Output.IsPadLampOn(new FloorPad(5)));
    }

    [TestMethod]
    public void Two_coins_stacked_during_attract_return_to_select_after_the_first_game()
    {
        var options = GameSessionDriver.CreateShortSession();
        options.RoundCount = 1;
        var driver = GameSessionDriver.Scripted(options, 1, 2);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseCredit();

        Assert.AreEqual(SessionPhase.Select, driver.Session.Phase);
        Assert.AreEqual(2, driver.Session.Credits);

        driver.PulseDifficulty(Difficulty.Easy);
        driver.PlayUntilResults(hitEveryPresentation: false);
        driver.AdvanceAndTick(driver.ResultsHold);

        Assert.AreEqual(SessionPhase.Select, driver.Session.Phase);
        Assert.AreEqual(1, driver.Session.Credits);
    }

    [TestMethod]
    public void A_coin_during_an_active_game_banks_credit_and_does_not_restart()
    {
        var options = GameSessionDriver.CreateShortSession();
        options.RoundCount = 1;
        var driver = GameSessionDriver.Scripted(options, 1, 2);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();
        var lit = driver.Session.LitPad;

        driver.PulseCredit();

        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        Assert.AreEqual(lit, driver.Session.LitPad);
        Assert.AreEqual(1, driver.Session.Credits);
        Assert.AreEqual(0, driver.Session.Score.Hits);
    }

    [TestMethod]
    public void Select_timeout_auto_starts_easy_when_nobody_picks()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.AdvanceAndTick(driver.SelectTimeout);

        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(Difficulty.Easy, driver.Session.SelectedDifficulty);
    }

    [TestMethod]
    public void Free_play_still_requires_a_difficulty_press()
    {
        var options = GameSessionDriver.CreateShortSession();
        options.FreePlay = true;
        var driver = GameSessionDriver.Scripted(options, 1, 2, 1, 2);
        driver.Tick();
        driver.Stomp(1);

        Assert.AreEqual(SessionPhase.Attract, driver.Session.Phase);

        driver.PulseDifficulty(Difficulty.Medium);
        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(Difficulty.Medium, driver.Session.SelectedDifficulty);
    }

    [TestMethod]
    public void Service_credit_is_playable_without_incrementing_the_coin_meter()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2]);
        driver.Tick();
        driver.PulseServiceCredit();
        driver.PulseDifficulty(Difficulty.Easy);

        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(0, driver.Session.CoinMeter);
        Assert.AreEqual(0, driver.Session.Credits);
    }

    [TestMethod]
    public void A_new_session_after_power_loss_is_attract_with_no_restored_credit()
    {
        var options = GameSessionDriver.CreateShortSession();
        var clock = new FakeGameClock();
        var session = new GameSession(options, clock, new ScriptedPadPicker(1, 2));

        Assert.AreEqual(SessionPhase.Attract, session.Phase);
        Assert.AreEqual(0, session.Credits);
        Assert.AreEqual(0, session.CoinMeter);
        Assert.AreEqual(0, session.Score.Hits);
    }
}
