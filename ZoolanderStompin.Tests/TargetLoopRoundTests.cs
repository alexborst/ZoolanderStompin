using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class TargetLoopRoundTests
{
    [TestMethod]
    public void Scripted_round_matches_the_stomp_scenario_rows()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 5),
            pads: [1, 2, 3, 4, 2]);

        driver.LightFirst();
        driver.Stomp(1);
        Assert.AreEqual(1, driver.Loop.Score.Hits);

        driver.FinishGap();
        driver.MissCurrent();
        driver.Stomp(2);
        Assert.AreEqual(1, driver.Loop.Score.Hits);
        Assert.AreEqual(1, driver.Loop.Score.Misses);

        driver.FinishGap();
        driver.Stomp(1);
        Assert.AreEqual(TargetLoopPhase.Presenting, driver.Loop.Phase);
        driver.Stomp(1, 3);
        Assert.AreEqual(2, driver.Loop.Score.Hits);
        Assert.AreEqual(1, driver.Loop.Score.Misses);

        driver.SetPads(4);
        driver.StabilizeHeldPads();
        driver.FinishGap();
        driver.StabilizeHeldPads();
        driver.MissCurrent();
        Assert.AreEqual(2, driver.Loop.Score.Hits);
        Assert.AreEqual(2, driver.Loop.Score.Misses);

        driver.Release();
        driver.FinishGap();
        driver.Stomp(1, 2);

        Assert.AreEqual(3, driver.Loop.Score.Hits);
        Assert.AreEqual(2, driver.Loop.Score.Misses);
        Assert.AreEqual(TargetLoopPhase.Complete, driver.Loop.Phase);
        Assert.AreEqual(60.0, driver.Loop.Score.HitPercent);
    }
}
