using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionSelectTests
{
    [TestMethod]
    public void Picking_medium_consumes_one_credit_and_locks_the_medium_set()
    {
        var driver = GameSessionDriver.Scripted(pads: [5, 6, 5, 6]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Medium);

        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(0, driver.Session.Credits);
        Assert.AreEqual(Difficulty.Medium, driver.Session.SelectedDifficulty);
        Assert.IsTrue(driver.Output.MediumLampOn);
        Assert.IsFalse(driver.Output.EasyLampOn);

        driver.SkipCountdown();

        Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        Assert.AreEqual(5, driver.Session.LitPad?.Number);
        Assert.IsTrue(driver.Output.IsPadLampOn(new FloorPad(5)));
        Assert.IsTrue(driver.Output.MediumLampOn);
    }

    [TestMethod]
    public void Select_timeout_auto_starts_easy()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.AdvanceAndTick(driver.SelectTimeout);

        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(Difficulty.Easy, driver.Session.SelectedDifficulty);
        Assert.AreEqual(0, driver.Session.Credits);
    }

    [TestMethod]
    public void Select_timeout_can_return_to_attract_and_drop_the_idle_credit()
    {
        var options = GameSessionDriver.CreateShortSession();
        options.SelectTimeoutAction = SelectTimeoutAction.ReturnToAttract;
        var driver = GameSessionDriver.Scripted(options, 1);
        driver.Tick();
        driver.PulseCredit();
        driver.AdvanceAndTick(driver.SelectTimeout);

        Assert.AreEqual(SessionPhase.Attract, driver.Session.Phase);
        Assert.AreEqual(0, driver.Session.Credits);
    }
}
