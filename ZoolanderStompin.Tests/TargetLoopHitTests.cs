using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class TargetLoopHitTests
{
    [TestMethod]
    public void Rising_edge_on_the_lit_pad_inside_the_window_is_a_hit()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 2),
            pads: [2, 3]);
        driver.LightFirst();
        driver.Stomp(2);

        Assert.AreEqual(1, driver.Loop.Score.Hits);
        Assert.AreEqual(0, driver.Loop.Score.Misses);
        Assert.AreEqual(TargetLoopPhase.Gap, driver.Loop.Phase);
        Assert.IsNull(driver.Loop.LitPad);
        Assert.AreEqual(0, driver.Output.PadLampsOn.Count);
        Assert.AreEqual(1, driver.Output.ScoreDigits);
        Assert.AreEqual(GameSound.Hit, driver.Output.Sound);
    }

    [TestMethod]
    public void A_press_that_has_not_debounced_does_not_score_yet()
    {
        var driver = TargetLoopDriver.Scripted(pads: [1]);
        driver.LightFirst();
        driver.SetPads(1);
        driver.Tick();

        Assert.AreEqual(0, driver.Loop.Score.Hits);
        Assert.AreEqual(TargetLoopPhase.Presenting, driver.Loop.Phase);
        Assert.IsTrue(driver.Output.IsPadLampOn(new FloorPad(1)));
    }
}
