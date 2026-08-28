using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionRoundTests
{
    [TestMethod]
    public void Round_two_starts_after_intermission_and_hits_accumulate()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();

        driver.Stomp(1);
        driver.FinishGap();
        driver.MissCurrent();

        Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);
        Assert.AreEqual(1, driver.Session.Score.Hits);
        Assert.AreEqual(1, driver.Session.Score.Misses);
        Assert.AreEqual(1, driver.Session.CurrentRound);

        driver.AdvanceAndTick(driver.Intermission);
        driver.Tick();

        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        Assert.AreEqual(2, driver.Session.CurrentRound);
        Assert.AreEqual(3, driver.Session.LitPad?.Number);

        driver.Stomp(3);
        driver.FinishGap();
        driver.Stomp(4);

        Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
        Assert.AreEqual(3, driver.Session.Score.Hits);
        Assert.AreEqual(1, driver.Session.Score.Misses);
        Assert.AreEqual(GameSound.GameEnd, driver.Output.Sound);
    }
}
