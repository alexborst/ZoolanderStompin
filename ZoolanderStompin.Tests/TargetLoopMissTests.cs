using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class TargetLoopMissTests
{
    [TestMethod]
    public void Window_expiry_with_no_valid_hit_is_a_miss()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 2),
            pads: [4, 1]);
        driver.LightFirst();
        driver.MissCurrent();

        Assert.AreEqual(0, driver.Loop.Score.Hits);
        Assert.AreEqual(1, driver.Loop.Score.Misses);
        Assert.AreEqual(TargetLoopPhase.Gap, driver.Loop.Phase);
        Assert.IsNull(driver.Loop.LitPad);
        Assert.AreEqual(GameSound.Miss, driver.Output.Sound);
        Assert.AreEqual(0, driver.Output.ScoreDigits);
    }

    [TestMethod]
    public void A_stomp_after_the_light_dies_does_not_become_a_hit()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 2),
            pads: [1, 2]);
        driver.LightFirst();
        driver.MissCurrent();
        driver.Stomp(1);

        Assert.AreEqual(0, driver.Loop.Score.Hits);
        Assert.AreEqual(1, driver.Loop.Score.Misses);
        Assert.AreEqual(TargetLoopPhase.Gap, driver.Loop.Phase);
    }
}
