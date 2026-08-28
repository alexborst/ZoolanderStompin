using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class TargetLoopPickTests
{
    [TestMethod]
    public void Lights_exactly_one_pad_from_the_difficulty_set()
    {
        var driver = TargetLoopDriver.Scripted(pads: [3]);
        driver.LightFirst();

        Assert.AreEqual(TargetLoopPhase.Presenting, driver.Loop.Phase);
        Assert.AreEqual(3, driver.Loop.LitPad?.Number);
        Assert.AreEqual(1, driver.Output.PadLampsOn.Count);
        Assert.IsTrue(driver.Output.IsPadLampOn(new FloorPad(3)));
        Assert.IsFalse(driver.Output.IsPadLampOn(new FloorPad(1)));
        Assert.AreEqual(GameSound.NewLight, driver.Output.Sound);
        Assert.IsTrue(driver.Output.EasyLampOn);
        CollectionAssert.Contains(
            driver.Script.CandidateSets[0].Select(pad => pad.Number).ToArray(),
            3);
    }

    [TestMethod]
    public void Next_pick_omits_the_previous_pad_when_configured()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 2),
            pads: [1, 2]);
        driver.LightFirst();
        driver.MissCurrent();
        driver.FinishGap();

        var secondCandidates = driver.Script.CandidateSets[1].Select(pad => pad.Number).ToArray();
        CollectionAssert.DoesNotContain(secondCandidates, 1);
        CollectionAssert.Contains(secondCandidates, 2);
        Assert.AreEqual(2, driver.Loop.LitPad?.Number);
        Assert.AreEqual(1, driver.Output.PadLampsOn.Count);
    }

    [TestMethod]
    public void Dark_gap_turns_all_pad_lamps_off()
    {
        var driver = TargetLoopDriver.Scripted(
            TargetLoopDriver.CreateShortRound(presentations: 2),
            pads: [1, 2]);
        driver.LightFirst();
        driver.Stomp(1);

        Assert.AreEqual(TargetLoopPhase.Gap, driver.Loop.Phase);
        Assert.IsNull(driver.Loop.LitPad);
        Assert.AreEqual(0, driver.Output.PadLampsOn.Count);
    }

    [TestMethod]
    public void Allows_a_repeat_when_only_one_pad_is_in_play()
    {
        var options = TargetLoopDriver.CreateShortRound(presentations: 2);
        options.Easy.PadsInPlay = [4];
        var driver = TargetLoopDriver.Scripted(options, pads: [4, 4]);
        driver.LightFirst();
        driver.MissCurrent();
        driver.FinishGap();

        Assert.AreEqual(4, driver.Loop.LitPad?.Number);
        CollectionAssert.AreEqual(
            new[] { 4 },
            driver.Script.CandidateSets[1].Select(pad => pad.Number).ToArray());
    }

    [TestMethod]
    public void Random_picks_do_not_repeat_consecutively()
    {
        var options = TargetLoopDriver.CreateShortRound(presentations: 40);
        var driver = new TargetLoopDriver(new RandomPadPicker(new Random(42)), options);
        driver.LightFirst();

        var sequence = new List<int>();
        for (var i = 0; i < 40; i++)
        {
            Assert.IsNotNull(driver.Loop.LitPad);
            sequence.Add(driver.Loop.LitPad.Value.Number);
            Assert.AreEqual(1, driver.Output.PadLampsOn.Count);
            driver.MissCurrent();
            if (i < 39)
            {
                driver.FinishGap();
            }
        }

        for (var i = 1; i < sequence.Count; i++)
        {
            Assert.AreNotEqual(sequence[i - 1], sequence[i], $"Consecutive repeat at presentation {i + 1}.");
        }
    }
}
