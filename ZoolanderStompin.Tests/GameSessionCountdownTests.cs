using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionCountdownTests
{
    [TestMethod]
    public void Stomps_during_the_start_hold_and_countdown_do_not_count()
    {
        var wav = ToneBank.ToWav(GameSound.GameStart);
        Assert.IsTrue(WavDuration.TryRead(wav, out var hold));
        var root = CreateBank((SoundFolders.GameStart, wav));
        try
        {
            var sounds = new SoundLibrary(root);
            var driver = new GameSessionDriver(new ScriptedPadPicker(1, 2, 1, 2), GameSessionDriver.CreateShortSession(), sounds);
            driver.Tick();
            driver.PulseCredit();
            driver.PulseDifficulty(Difficulty.Easy);

            Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
            driver.Stomp(1);
            Assert.AreEqual(0, driver.Session.Score.Hits);

            driver.AdvanceAndTick(hold);
            Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);
            driver.Stomp(1);
            Assert.AreEqual(0, driver.Session.Score.Hits);
            Assert.AreEqual(0, driver.Session.Score.Misses);

            driver.AdvanceAndTick(GameSessionDriver.CountdownHold(sounds));
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

    [TestMethod]
    public void Countdown_clip_from_its_folder_is_cued_after_game_start_and_held_for_its_length()
    {
        var start = ToneBank.ToWav(GameSound.GameStart);
        var countdown = ToneBank.ToWav(GameSound.RoundEndPerfect);
        Assert.IsTrue(WavDuration.TryRead(start, out var startHold));
        Assert.IsTrue(WavDuration.TryRead(countdown, out var countdownHold));
        var root = CreateBank((SoundFolders.GameStart, start), (SoundFolders.Countdown, countdown));
        try
        {
            var driver = new GameSessionDriver(new ScriptedPadPicker(1, 2), GameSessionDriver.CreateShortSession(), new SoundLibrary(root));
            driver.Tick();
            driver.PulseCredit();
            driver.PulseDifficulty(Difficulty.Easy);
            driver.Session.DrainCues();

            driver.AdvanceAndTick(startHold);
            CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.Countdown);
            Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);

            driver.AdvanceAndTick(countdownHold - TimeSpan.FromMilliseconds(20));
            Assert.AreEqual(SessionPhase.Countdown, driver.Session.Phase);

            driver.AdvanceAndTick(TimeSpan.FromMilliseconds(40));
            Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Countdown_follows_the_first_round_end_but_not_the_last()
    {
        var countdown = ToneBank.ToWav(GameSound.Countdown);
        Assert.IsTrue(WavDuration.TryRead(countdown, out var countdownHold));
        var root = CreateBank((SoundFolders.Countdown, countdown));
        try
        {
            var sounds = new SoundLibrary(root);
            var driver = new GameSessionDriver(new ScriptedPadPicker(1, 2, 3, 4), GameSessionDriver.CreateShortSession(), sounds);
            driver.Tick();
            driver.PulseCredit();
            driver.PulseDifficulty(Difficulty.Easy);
            driver.SkipCountdown();

            driver.HitCurrent();
            driver.FinishGap();
            driver.HitCurrent();
            Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);
            driver.Session.DrainCues();

            var roundEndHold = sounds.HoldDuration(GameSound.RoundEndPerfect, TimeSpan.Zero);
            driver.AdvanceAndTick(roundEndHold > driver.Intermission ? roundEndHold : driver.Intermission);
            Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);
            CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.Countdown);

            driver.AdvanceAndTick(countdownHold);
            Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
            Assert.AreEqual(2, driver.Session.CurrentRound);
            driver.Tick();
            driver.Session.DrainCues();

            driver.HitCurrent();
            driver.FinishGap();
            driver.HitCurrent();
            Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);

            var lastRoundEndHold = sounds.HoldDuration(GameSound.RoundEndPerfect, TimeSpan.Zero);
            driver.AdvanceAndTick(lastRoundEndHold + driver.GameEndDelay);
            var cues = driver.Session.DrainCues().ToList();
            CollectionAssert.Contains(cues, GameSound.GameEnd);
            CollectionAssert.DoesNotContain(cues, GameSound.Countdown);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateBank(params (string Folder, byte[] Wav)[] clips)
    {
        var root = Path.Combine(Path.GetTempPath(), $"zoolander-countdown-{Guid.NewGuid():N}");
        foreach (var (folder, wav) in clips)
        {
            var directory = Path.Combine(root, folder);
            Directory.CreateDirectory(directory);
            File.WriteAllBytes(Path.Combine(directory, "clip.wav"), wav);
        }

        return root;
    }
}
