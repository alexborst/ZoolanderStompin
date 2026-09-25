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

    [TestMethod]
    public void A_miss_queues_a_miss_cue()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();
        driver.Session.DrainCues();
        driver.MissCurrent();

        CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.Miss);
    }

    [TestMethod]
    public void Starting_a_game_queues_a_start_cue()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.Session.DrainCues();
        driver.PulseDifficulty(Difficulty.Easy);

        CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.GameStart);
    }

    [TestMethod]
    public void Results_always_queue_a_game_end_cue()
    {
        var win = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        win.Tick();
        win.PulseCredit();
        win.PulseDifficulty(Difficulty.Easy);
        win.PlayUntilResults(hitEveryPresentation: true);

        CollectionAssert.Contains(win.Session.DrainCues().ToList(), GameSound.GameEnd);

        var lose = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        lose.Tick();
        lose.PulseCredit();
        lose.PulseDifficulty(Difficulty.Easy);
        lose.PlayUntilResults(hitEveryPresentation: false);

        CollectionAssert.Contains(lose.Session.DrainCues().ToList(), GameSound.GameEnd);
    }
}
