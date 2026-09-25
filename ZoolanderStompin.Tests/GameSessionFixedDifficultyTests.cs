using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionFixedDifficultyTests
{
    [TestMethod]
    public void Frozen_defaults_start_hard_on_credit_and_ignore_difficulty_buttons()
    {
        var driver = new GameSessionDriver(new ScriptedPadPicker(7, 1, 2, 3), GameOptions.CreateDefault());
        driver.Tick();
        driver.PulseCredit();

        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(Difficulty.Hard, driver.Session.SelectedDifficulty);
        Assert.AreEqual(0, driver.Session.Credits);
        Assert.AreNotEqual(SessionPhase.Select, driver.Session.Phase);

        driver.PulseDifficulty(Difficulty.Easy);
        Assert.AreEqual(Difficulty.Hard, driver.Session.SelectedDifficulty);

        driver.SkipCountdown();
        Assert.AreEqual(7, driver.Session.LitPad?.Number);
    }

    [TestMethod]
    public void Extra_credit_after_results_starts_hard_again_without_select()
    {
        var options = GameSessionDriver.CreateShortSession();
        options.FixedDifficulty = Difficulty.Hard;
        options.RoundCount = 1;
        var driver = GameSessionDriver.Scripted(options, 1, 2, 3, 4, 5, 6, 7);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseCredit();
        driver.PlayUntilResults(hitEveryPresentation: false);
        driver.AdvanceAndTick(driver.ResultsHold);

        Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
        Assert.AreEqual(Difficulty.Hard, driver.Session.SelectedDifficulty);
        Assert.AreEqual(0, driver.Session.Credits);
        Assert.AreNotEqual(SessionPhase.Select, driver.Session.Phase);
    }

    [TestMethod]
    public void Scoreboard_omits_difficulty_choice_when_hard_is_locked()
    {
        var driver = new GameSessionDriver(new ScriptedPadPicker(1), GameOptions.CreateDefault());
        driver.Tick();
        var attract = PlaySnapshot.Capture(driver.Session);

        Assert.AreEqual("", attract.Difficulty);
        Assert.AreEqual(PlayStatus.CreditOnlyKeyLegend, attract.KeyLegend);
        Assert.AreEqual("Insert a credit (C) to start.", attract.Prompt);
        Assert.IsFalse(attract.Prompt.Contains("Easy", StringComparison.Ordinal));
        var attractText = PlayStatus.Format(driver.Session);
        Assert.IsFalse(attractText.Contains("Easy", StringComparison.Ordinal));
        Assert.IsFalse(attractText.Contains("Medium", StringComparison.Ordinal));
        Assert.IsFalse(attractText.Contains("Hard", StringComparison.Ordinal));

        driver.PulseCredit();
        var countdown = PlaySnapshot.Capture(driver.Session);
        Assert.AreEqual("", countdown.Difficulty);
        Assert.IsFalse(countdown.Prompt.Contains("Easy", StringComparison.Ordinal));
        Assert.IsFalse(countdown.Prompt.Contains("pick", StringComparison.Ordinal));
    }
}
