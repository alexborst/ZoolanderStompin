using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GameSessionRoundEndTests
{
    [TestMethod]
    public void A_perfect_first_round_cues_the_perfect_round_end_sound()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();
        driver.Session.DrainCues();

        driver.HitCurrent();
        driver.FinishGap();
        driver.HitCurrent();

        Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);
        CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.RoundEndPerfect);
    }

    [TestMethod]
    public void A_round_with_no_hits_cues_the_bad_round_end_sound()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();
        driver.Session.DrainCues();

        driver.MissCurrent();
        driver.FinishGap();
        driver.MissCurrent();

        Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);
        CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.RoundEndBad);
    }

    [TestMethod]
    public void Second_round_is_rated_on_its_own_hits()
    {
        var driver = GameSessionDriver.Scripted(pads: [1, 2, 3, 4]);
        driver.Tick();
        driver.PulseCredit();
        driver.PulseDifficulty(Difficulty.Easy);
        driver.SkipCountdown();

        driver.HitCurrent();
        driver.FinishGap();
        driver.HitCurrent();
        driver.AdvanceAndTick(driver.Intermission);
        driver.Tick();
        driver.Session.DrainCues();

        driver.MissCurrent();
        driver.FinishGap();
        driver.MissCurrent();

        Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
        CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.RoundEndBad);

        driver.AwaitGameEnd();
        CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.GameEnd);
    }

    [TestMethod]
    public void Intermission_waits_for_the_round_end_clip_before_round_two()
    {
        var wav = ToneBank.ToWav(GameSound.RoundEndPerfect);
        Assert.IsTrue(WavDuration.TryRead(wav, out var hold));
        var root = CreateRoundEndBank(SoundFolders.RoundEndPerfect, wav);
        try
        {
            var options = GameSessionDriver.CreateShortSession();
            Assert.IsTrue(hold > TimeSpan.FromMilliseconds(options.IntermissionMilliseconds));
            var sounds = new SoundLibrary(root);
            var driver = new GameSessionDriver(new ScriptedPadPicker(1, 2, 3, 4), options, sounds);
            driver.Tick();
            driver.PulseCredit();
            driver.PulseDifficulty(Difficulty.Easy);
            driver.SkipCountdown();

            driver.HitCurrent();
            driver.FinishGap();
            driver.HitCurrent();
            Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);

            driver.AdvanceAndTick(driver.Intermission);
            driver.Tick();
            Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);

            driver.AdvanceAndTick(hold - driver.Intermission);
            Assert.AreEqual(SessionPhase.Intermission, driver.Session.Phase);
            CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.Countdown);

            driver.AdvanceAndTick(GameSessionDriver.CountdownHold(sounds));
            driver.Tick();
            Assert.AreEqual(SessionPhase.Playing, driver.Session.Phase);
            Assert.AreEqual(2, driver.Session.CurrentRound);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Results_play_the_round_end_clip_then_pause_before_the_game_end_cue()
    {
        var wav = ToneBank.ToWav(GameSound.RoundEndBad);
        Assert.IsTrue(WavDuration.TryRead(wav, out var hold));
        var root = CreateRoundEndBank(SoundFolders.RoundEndBad, wav);
        try
        {
            var options = GameSessionDriver.CreateShortSession();
            options.RoundCount = 1;
            var driver = new GameSessionDriver(new ScriptedPadPicker(1, 2), options, new SoundLibrary(root));
            driver.Tick();
            driver.PulseCredit();
            driver.PulseDifficulty(Difficulty.Easy);
            driver.SkipCountdown();
            driver.Session.DrainCues();

            driver.MissCurrent();
            driver.FinishGap();
            driver.MissCurrent();

            Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
            var cues = driver.Session.DrainCues().ToList();
            CollectionAssert.Contains(cues, GameSound.RoundEndBad);
            CollectionAssert.DoesNotContain(cues, GameSound.GameEnd);

            driver.AdvanceAndTick(hold);
            Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
            CollectionAssert.DoesNotContain(driver.Session.DrainCues().ToList(), GameSound.GameEnd);

            driver.AdvanceAndTick(driver.GameEndDelay);
            Assert.AreEqual(SessionPhase.Results, driver.Session.Phase);
            CollectionAssert.Contains(driver.Session.DrainCues().ToList(), GameSound.GameEnd);

            driver.AdvanceAndTick(driver.ResultsHold);
            Assert.AreEqual(SessionPhase.Attract, driver.Session.Phase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateRoundEndBank(string folder, byte[] wav)
    {
        var root = Path.Combine(Path.GetTempPath(), $"zoolander-round-end-{Guid.NewGuid():N}");
        var directory = Path.Combine(root, folder);
        Directory.CreateDirectory(directory);
        File.WriteAllBytes(Path.Combine(directory, "clip.wav"), wav);
        return root;
    }
}
