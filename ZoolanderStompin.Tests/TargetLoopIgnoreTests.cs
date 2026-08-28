using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class TargetLoopIgnoreTests
{
    [TestMethod]
    public void Wrong_pad_then_the_lit_pad_still_counts_as_a_hit()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 1),
            pads: [2]);
        driver.LightFirst();
        driver.Stomp(1);

        Assert.AreEqual(0, driver.Loop.Score.Hits);
        Assert.AreEqual(TargetLoopPhase.Presenting, driver.Loop.Phase);
        Assert.IsTrue(driver.Output.IsPadLampOn(new FloorPad(2)));

        driver.Stomp(1, 2);

        Assert.AreEqual(1, driver.Loop.Score.Hits);
        Assert.AreEqual(0, driver.Loop.Score.Misses);
        Assert.AreEqual(TargetLoopPhase.Complete, driver.Loop.Phase);
    }

    [TestMethod]
    public void A_pad_already_held_when_it_lights_does_not_auto_score()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 1),
            pads: [1]);
        driver.SetPads(1);
        driver.LightFirst();
        driver.StabilizeHeldPads();

        Assert.AreEqual(0, driver.Loop.Score.Hits);
        Assert.AreEqual(TargetLoopPhase.Presenting, driver.Loop.Phase);

        driver.MissCurrent();

        Assert.AreEqual(0, driver.Loop.Score.Hits);
        Assert.AreEqual(1, driver.Loop.Score.Misses);
    }

    [TestMethod]
    public void Releasing_and_pressing_again_after_a_held_light_is_a_hit()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 1),
            pads: [1]);
        driver.SetPads(1);
        driver.LightFirst();
        driver.StabilizeHeldPads();
        driver.Release();
        driver.Stomp(1);

        Assert.AreEqual(1, driver.Loop.Score.Hits);
        Assert.AreEqual(0, driver.Loop.Score.Misses);
    }

    [TestMethod]
    public void Bounce_does_not_score_and_does_not_kill_the_window()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 1),
            pads: [1]);
        driver.LightFirst();

        driver.SetPads(1);
        driver.Tick();
        driver.Clock.Advance(TimeSpan.FromMilliseconds(10));
        driver.SetPads();
        driver.Tick();
        driver.SetPads(1);
        driver.Tick();
        driver.Clock.Advance(TimeSpan.FromMilliseconds(10));
        driver.Tick();

        Assert.AreEqual(0, driver.Loop.Score.Hits);
        Assert.AreEqual(TargetLoopPhase.Presenting, driver.Loop.Phase);

        driver.SetPads();
        driver.Tick();
        driver.MissCurrent();
        Assert.AreEqual(1, driver.Loop.Score.Misses);
    }

    [TestMethod]
    public void Two_pads_at_once_accept_only_the_lit_pad()
    {
        var options = TargetLoopDriver.CreateShortRound(presentations: 1);
        var driver = TargetLoopDriver.Scripted(options, Difficulty.Hard, 7);
        driver.LightFirst();
        driver.Stomp(1, 7);

        Assert.AreEqual(1, driver.Loop.Score.Hits);
        Assert.AreEqual(0, driver.Loop.Score.Misses);
        Assert.AreEqual(TargetLoopPhase.Complete, driver.Loop.Phase);
    }
}
