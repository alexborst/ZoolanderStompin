using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class ScenarioStompTests
{
    [TestMethod]
    public void Correct_stomp_while_lit_is_an_immediate_hit_and_the_lamp_goes_off()
    {
        var driver = StartEasy(pads: [2, 1, 3, 4]);
        driver.SkipCountdown();
        driver.Stomp(2);

        Assert.AreEqual(1, driver.Session.Score.Hits);
        Assert.AreEqual(0, driver.Session.Score.Misses);
        Assert.IsNull(driver.Session.LitPad);
        Assert.AreEqual(0, driver.Output.PadLampsOn.Count);
        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
    }

    [TestMethod]
    public void A_stomp_after_the_light_dies_does_not_become_a_hit()
    {
        var driver = StartEasy(pads: [1, 2, 3, 4]);
        driver.SkipCountdown();
        driver.MissCurrent();
        driver.Stomp(1);

        Assert.AreEqual(0, driver.Session.Score.Hits);
        Assert.AreEqual(1, driver.Session.Score.Misses);
    }

    [TestMethod]
    public void A_wrong_pad_then_the_lit_pad_still_counts_as_a_hit()
    {
        var driver = StartEasy(pads: [2, 1, 3, 4]);
        driver.SkipCountdown();
        driver.Stomp(1);

        Assert.AreEqual(0, driver.Session.Score.Hits);
        Assert.IsTrue(driver.Output.IsPadLampOn(new FloorPad(2)));

        driver.Stomp(1, 2);

        Assert.AreEqual(1, driver.Session.Score.Hits);
        Assert.AreEqual(0, driver.Session.Score.Misses);
    }

    [TestMethod]
    public void Standing_on_a_pad_that_later_lights_does_not_auto_score()
    {
        var driver = StartEasy(pads: [1, 2, 3, 4]);
        driver.SkipToPlayingUnlit();
        driver.SetPads(1);
        driver.Tick();
        driver.StabilizeHeldPads();

        Assert.AreEqual(0, driver.Session.Score.Hits);
        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        Assert.AreEqual(1, driver.Session.LitPad?.Number);

        driver.MissCurrent();
        Assert.AreEqual(0, driver.Session.Score.Hits);
        Assert.AreEqual(1, driver.Session.Score.Misses);
    }

    [TestMethod]
    public void Two_people_on_hard_score_if_either_closes_the_lit_pad()
    {
        var options = GameSessionDriver.CreateShortSession();
        var driver = GameSessionDriver.Scripted(options, 7, 1, 2, 3);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Hard);
        driver.SkipCountdown();
        driver.Stomp(1, 7);

        Assert.AreEqual(1, driver.Session.Score.Hits);
        Assert.AreEqual(0, driver.Session.Score.Misses);
    }

    private static GameSessionDriver StartEasy(params int[] pads)
    {
        var driver = GameSessionDriver.Scripted(pads: pads);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        return driver;
    }
}
