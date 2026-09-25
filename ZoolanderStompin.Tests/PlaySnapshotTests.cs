using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class PlaySnapshotTests
{
    [TestMethod]
    public void Attract_hides_score_until_a_game_starts()
    {
        var driver = GameSessionDriver.Scripted(pads: [1]);
        driver.Tick();
        var snap = PlaySnapshot.Capture(driver.Session);

        Assert.AreEqual(GameInfo.Name, snap.Title);
        Assert.AreEqual(nameof(SessionPhase.Attract), snap.Phase);
        Assert.AreEqual("", snap.Outcome);
        Assert.AreEqual("0", snap.Credits);
        Assert.AreEqual("round -", snap.Round);
        Assert.AreEqual("--", snap.Hits);
        Assert.AreEqual("--", snap.Misses);
        Assert.AreEqual("--%", snap.Percent);
        Assert.AreEqual("--", snap.Tickets);
        StringAssert.Contains(snap.Prompt, "Insert a credit");
        StringAssert.Contains(snap.Prompt, "pick Easy, Medium, or Hard");
        StringAssert.Contains(snap.Difficulty, "Easy");
        Assert.AreEqual(PlayStatus.KeyLegend, snap.KeyLegend);
    }

    [TestMethod]
    public void Playing_shows_hits_misses_and_the_lit_pad()
    {
        var driver = GameSessionDriver.Scripted(pads: [3, 1, 2, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Medium);
        driver.SkipCountdown();
        var snap = PlaySnapshot.Capture(driver.Session);

        Assert.AreEqual(nameof(SessionPhase.Playing), snap.Phase);
        Assert.AreEqual("00", snap.Hits);
        Assert.AreEqual("00", snap.Misses);
        Assert.AreEqual("STOMP 3!", snap.Prompt);
        StringAssert.Contains(snap.Difficulty, "[Medium]");
        StringAssert.Contains(snap.Pads, "[3]");
    }

    [TestMethod]
    public void Results_shows_win_percent_and_tickets()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.PlayUntilResults(hitEveryPresentation: true);
        var snap = PlaySnapshot.Capture(driver.Session);

        Assert.AreEqual(nameof(SessionPhase.Results), snap.Phase);
        Assert.AreEqual("WIN", snap.Outcome);
        Assert.AreEqual("100%", snap.Percent);
        Assert.AreEqual("08", snap.Tickets);
        StringAssert.Contains(snap.Prompt, "You win");
    }
}
