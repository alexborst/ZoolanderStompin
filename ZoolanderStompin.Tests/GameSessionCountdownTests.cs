using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionCountdownTests
{
    [TestMethod]
    public void Stomps_during_the_start_hold_do_not_count()
    {
        var wav = ToneBank.ToWav(GameSound.GameStart);
        Assert.IsTrue(WavDuration.TryRead(wav, out var hold));
        var root = Path.Combine(Path.GetTempPath(), $"zoolander-countdown-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(root, SoundFolders.GameStart));
        File.WriteAllBytes(Path.Combine(root, SoundFolders.GameStart, "start.wav"), wav);
        try
        {
            var driver = new GameSessionDriver(
                new ScriptedPadPicker(1, 2, 1, 2),
                GameSessionDriver.CreateShortSession(),
                new SoundLibrary(root));
            driver.Tick();
            driver.PulseCredit();
            driver.PulseDifficulty(Difficulty.Easy);

            Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
            driver.Stomp(1);

            Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
            Assert.AreEqual(0, driver.Session.Score.Hits);
            Assert.AreEqual(0, driver.Session.Score.Misses);

            driver.AdvanceAndTick(hold);
            driver.Tick();

            Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
            Assert.AreEqual(1, driver.Session.LitPad?.Number);
            Assert.AreEqual(0, driver.Session.Score.Hits);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
