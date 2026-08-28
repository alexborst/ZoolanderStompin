using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionCreditTests
{
    [TestMethod]
    public void A_coin_during_play_banks_a_credit_and_returns_to_select_after_results()
    {
        var options = GameSessionDriver.CreateShortSession();
        options.RoundCount = 1;
        var driver = GameSessionDriver.Scripted(options, 1, 2);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();

        driver.PulseCredit();
        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        Assert.AreEqual(1, driver.Session.Credits);
        Assert.AreEqual(2, driver.Session.CoinMeter);

        driver.MissCurrent();
        driver.FinishGap();
        driver.MissCurrent();

        Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
        driver.AdvanceAndTick(driver.ResultsHold);

        Assert.AreEqual(SessionPhase.Select, driver.Session.Phase);
        Assert.AreEqual(1, driver.Session.Credits);
    }

    [TestMethod]
    public void Free_play_starts_from_attract_only_after_a_difficulty_press()
    {
        var options = GameSessionDriver.CreateShortSession();
        options.FreePlay = true;
        var driver = GameSessionDriver.Scripted(options, 1, 2, 1, 2);
        driver.Tick();

        Assert.AreEqual(SessionPhase.Attract, driver.Session.Phase);
        Assert.IsTrue(driver.Output.EasyLampOn);
        Assert.AreEqual(0, driver.Session.Credits);

        driver.PulseDifficulty(Difficulty.Hard);
        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(Difficulty.Hard, driver.Session.SelectedDifficulty);
        Assert.AreEqual(0, driver.Session.Credits);
        Assert.AreEqual(0, driver.Session.CoinMeter);
    }

    [TestMethod]
    public void Service_credit_is_playable_and_does_not_increment_the_coin_meter()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 1, 2]);
        driver.Tick();
        driver.PulseServiceCredit();

        Assert.AreEqual(SessionPhase.Select, driver.Session.Phase);
        Assert.AreEqual(1, driver.Session.Credits);
        Assert.AreEqual(0, driver.Session.CoinMeter);

        driver.PulseDifficulty(Difficulty.Easy);
        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(0, driver.Session.Credits);
        Assert.AreEqual(0, driver.Session.CoinMeter);
    }
}
