using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionCountdownTests
{
    [TestMethod]
    public void Stomps_during_countdown_do_not_count()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 1, 2]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);

        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        driver.Stomp(1);
        driver.AdvanceAndTick(driver.GetReady);
        driver.Stomp(1);

        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(0, driver.Session.Score.Hits);
        Assert.AreEqual(0, driver.Session.Score.Misses);

        driver.AdvanceAndTick(driver.Go);
        driver.Tick();

        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        Assert.AreEqual(1, driver.Session.LitPad?.Number);
        Assert.AreEqual(0, driver.Session.Score.Hits);
    }
}
