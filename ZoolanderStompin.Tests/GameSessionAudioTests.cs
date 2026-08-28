using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionAudioTests
{
    [TestMethod]
    public void A_credit_queues_a_coin_cue_and_a_hit_queues_a_distinct_hit_cue()
    {
        var driver = GameSessionDriver.Scripted(pads: [2, 1, 3, 4]);
        driver.Tick();
        driver.Session.DrainCues();
        driver.PulseCredit();

        CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.Coin);

        driver.PulseDifficulty(Difficulty.Easy);
        driver.Session.DrainCues();
        driver.SkipCountdown();
        var afterLight = driver.Session.DrainCues();
        CollectionAssert.Contains(afterLight.ToList(), GameSound.NewLight);

        driver.Stomp(2);
        CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.Hit);
    }
}
