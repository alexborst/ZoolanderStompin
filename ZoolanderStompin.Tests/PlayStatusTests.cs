using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class PlayStatusTests
{
    [TestMethod]
    public void Attract_shows_phase_credits_and_the_key_legend()
    {
        var driver = GameSessionDriver.Scripted(pads: [1]);
        driver.Tick();
        var text = PlayStatus.Format(driver.Session);

        StringAssert.Contains(text, GameInfo.Name);
        StringAssert.Contains(text, "Attract");
        StringAssert.Contains(text, "credits 0");
        StringAssert.Contains(text, "hits --");
        StringAssert.Contains(text, "tickets --");
        StringAssert.Contains(text, PlayStatus.KeyLegend);
        StringAssert.Contains(text, "Insert a credit");
    }

    [TestMethod]
    public void Playing_shows_hits_misses_and_the_lit_pad()
    {
        var driver = GameSessionDriver.Scripted(pads: [3, 1, 2, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Medium);
        driver.SkipCountdown();

        var text = PlayStatus.Format(driver.Session);
        StringAssert.Contains(text, "Playing");
        StringAssert.Contains(text, "STOMP 3!");
        StringAssert.Contains(text, "hits 00");
        StringAssert.Contains(text, "misses 00");
        StringAssert.Contains(text, "[Medium]");
    }

    [TestMethod]
    public void Results_shows_win_or_lose_percentage_and_tickets()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.PlayUntilResults(hitEveryPresentation: true);
        var text = PlayStatus.Format(driver.Session);

        StringAssert.Contains(text, "Results");
        StringAssert.Contains(text, "WIN");
        StringAssert.Contains(text, "100%");
        StringAssert.Contains(text, "tickets 08");
        StringAssert.Contains(text, "You win");
    }
}
