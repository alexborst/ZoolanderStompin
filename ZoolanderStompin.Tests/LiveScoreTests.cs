using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class LiveScoreTests
{
    [TestMethod]
    public void A_hit_updates_numeric_score_and_pictorial_together()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();
        driver.Stomp(1);

        Assert.AreEqual(1, driver.Output.ScoreDigits);
        Assert.IsNull(driver.Output.TicketDigits);
        CollectionAssert.AreEqual(
            new[] { true, false, false, false },
            driver.Output.PictorialLampsOn.ToArray());
    }

    [TestMethod]
    public void A_miss_does_not_advance_the_pictorial()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();
        driver.Stomp(1);
        var pictorialAfterHit = driver.Output.PictorialLampsOn.ToArray();

        driver.FinishGap();
        driver.MissCurrent();

        Assert.AreEqual(1, driver.Output.ScoreDigits);
        Assert.AreEqual(1, driver.Session.Score.Misses);
        CollectionAssert.AreEqual(pictorialAfterHit, driver.Output.PictorialLampsOn.ToArray());
        Assert.IsNull(driver.Output.TicketDigits);
    }
}
