using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class TargetLoopCarryoverTests
{
    [TestMethod]
    public void A_new_round_runs_its_own_presentations_on_top_of_the_carried_score()
    {
        var options = TargetLoopDriver.CreateShortRound(presentations: 2);
        var clock = new FakeGameClock();
        var loop = new TargetLoop(
            options,
            Difficulty.Easy,
            clock,
            new ScriptedPadPicker(3, 4),
            startingScore: new Score(3, 1),
            previousPad: new FloorPad(1));

        loop.Tick(GameIoInput.None);
        Assert.AreEqual(3, loop.LitPad?.Number);

        clock.Advance(TimeSpan.FromMilliseconds(options.Easy.HitWindowMilliseconds));
        loop.Tick(GameIoInput.None);
        clock.Advance(TimeSpan.FromMilliseconds(options.InterTargetGapMilliseconds));
        loop.Tick(GameIoInput.None);
        clock.Advance(TimeSpan.FromMilliseconds(options.Easy.HitWindowMilliseconds));
        loop.Tick(GameIoInput.None);

        Assert.AreEqual(TargetLoopPhase.Complete, loop.Phase);
        Assert.AreEqual(3, loop.Score.Hits);
        Assert.AreEqual(3, loop.Score.Misses);
    }
}
