using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionStartHoldTests
{
    [TestMethod]
    public void Waits_for_the_game_start_wav_then_starts_play()
    {
        var wav = ToneBank.ToWav(GameSound.GameStart);
        Assert.IsTrue(WavDuration.TryRead(wav, out var hold));
        var root = CreateStartBank(wav);
        try
        {
            var driver = new GameSessionDriver(
                new ScriptedPadPicker(1, 2),
                GameSessionDriver.CreateShortSession(),
                new SoundLibrary(root));
            driver.Tick();
            driver.PulseCredit();
            driver.PulseDifficulty(Difficulty.Easy);

            Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
            driver.AdvanceAndTick(hold - TimeSpan.FromMilliseconds(20));
            Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
            Assert.AreEqual(0, driver.Session.Score.Hits);

            driver.AdvanceAndTick(TimeSpan.FromMilliseconds(40));
            Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Uses_the_fallback_hold_when_the_start_wav_cannot_be_parsed()
    {
        var root = CreateStartBank("not a wav"u8.ToArray());
        try
        {
            var options = GameSessionDriver.CreateShortSession();
            options.SoundHoldFallbackMilliseconds = 250;
            var driver = new GameSessionDriver(new ScriptedPadPicker(1), options, new SoundLibrary(root));
            driver.Tick();
            driver.PulseCredit();
            driver.PulseDifficulty(Difficulty.Easy);

            driver.AdvanceAndTick(TimeSpan.FromMilliseconds(200));
            Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);

            driver.AdvanceAndTick(TimeSpan.FromMilliseconds(80));
            Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateStartBank(byte[] wav)
    {
        var root = Path.Combine(Path.GetTempPath(), $"zoolander-start-{Guid.NewGuid():N}");
        var directory = Path.Combine(root, SoundFolders.GameStart);
        Directory.CreateDirectory(directory);
        File.WriteAllBytes(Path.Combine(directory, "start.wav"), wav);
        return root;
    }
}
