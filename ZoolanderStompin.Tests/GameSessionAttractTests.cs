using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionAttractTests
{
    [TestMethod]
    public void Starts_in_attract_and_cycles_pad_lamps()
    {
        var driver = GameSessionDriver.Scripted(pads: [1]);
        driver.Tick();

        Assert.AreEqual(SessionPhase.Attract, driver.Session.Phase);
        Assert.IsTrue(driver.Output.IsPadLampOn(new FloorPad(1)));
        Assert.AreEqual(1, driver.Output.PadLampsOn.Count);

        driver.AdvanceAndTick(driver.AttractCycle);
        Assert.IsTrue(driver.Output.IsPadLampOn(new FloorPad(2)));
        Assert.IsFalse(driver.Output.IsPadLampOn(new FloorPad(1)));
    }

    [TestMethod]
    public void Stomps_in_attract_do_not_score()
    {
        var driver = GameSessionDriver.Scripted(pads: [1]);
        driver.Tick();
        driver.Stomp(1);

        Assert.AreEqual(SessionPhase.Attract, driver.Session.Phase);
        Assert.AreEqual(0, driver.Session.Score.Hits);
        Assert.AreEqual(0, driver.Session.Score.Misses);
        Assert.IsNull(driver.Output.ScoreDigits);
    }

    [TestMethod]
    public void A_credit_leaves_attract_for_select()
    {
        var driver = GameSessionDriver.Scripted(pads: [1]);
        driver.Tick();
        driver.PulseCredit();

        Assert.AreEqual(SessionPhase.Select, driver.Session.Phase);
        Assert.AreEqual(1, driver.Session.Credits);
        Assert.AreEqual(1, driver.Session.CoinMeter);
        Assert.IsTrue(driver.Output.EasyLampOn);
        Assert.IsTrue(driver.Output.MediumLampOn);
        Assert.IsTrue(driver.Output.HardLampOn);
        Assert.AreEqual(GameSound.Coin, driver.Output.Sound);
    }
}
