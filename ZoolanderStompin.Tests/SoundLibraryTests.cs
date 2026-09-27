using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class SoundLibraryTests
{
    [TestMethod]
    public void Picks_a_wav_from_the_event_folder()
    {
        var root = CreateBank(
            SoundFolders.GameEnd,
            ("a.wav", GameSound.GameEnd),
            ("b.wav", GameSound.Round));
        try
        {
            var library = new SoundLibrary(root, pick: _ => 1);
            var clip = library.Resolve(GameSound.GameEnd);

            Assert.IsNotNull(clip);
            Assert.IsTrue(
                clip!.FilePath!.EndsWith("b.wav", StringComparison.OrdinalIgnoreCase),
                clip.FilePath);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Falls_back_to_a_generated_tone_when_the_folder_is_empty()
    {
        var root = CreateBank(SoundFolders.GameEnd);
        try
        {
            var library = new SoundLibrary(root);
            var clip = library.Resolve(GameSound.GameEnd);

            Assert.IsNotNull(clip);
            Assert.IsNull(clip!.FilePath);
            CollectionAssert.AreEqual(ToneBank.ToWav(GameSound.GameEnd), clip.WavBytes);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Hits_and_misses_come_from_their_folders()
    {
        var root = CreateBank(SoundFolders.Hits, ("hit-tone.wav", GameSound.Hit));
        Directory.CreateDirectory(Path.Combine(root, SoundFolders.Misses));
        File.WriteAllBytes(Path.Combine(root, SoundFolders.Misses, "miss-tone.wav"), ToneBank.ToWav(GameSound.Miss));
        try
        {
            var library = new SoundLibrary(root);

            var hit = library.Resolve(GameSound.Hit);
            Assert.IsTrue(hit!.FilePath!.EndsWith("hit-tone.wav", StringComparison.OrdinalIgnoreCase), hit.FilePath);

            var miss = library.Resolve(GameSound.Miss);
            Assert.IsTrue(miss!.FilePath!.EndsWith("miss-tone.wav", StringComparison.OrdinalIgnoreCase), miss.FilePath);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Shipped_hit_and_miss_tones_match_the_tone_bank()
    {
        var root = Path.Combine(FindRepoRoot(), "ZoolanderStompin", SoundFolders.RootName);

        CollectionAssert.AreEqual(
            ToneBank.ToWav(GameSound.Hit),
            File.ReadAllBytes(Path.Combine(root, SoundFolders.Hits, "hit-tone.wav")));
        CollectionAssert.AreEqual(
            ToneBank.ToWav(GameSound.Miss),
            File.ReadAllBytes(Path.Combine(root, SoundFolders.Misses, "miss-tone.wav")));
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "ZoolanderStompin.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("Repo root not found.");
    }

    [TestMethod]
    public void Hold_duration_is_the_longest_readable_wav_in_the_folder()
    {
        var root = CreateBank(
            SoundFolders.GameStart,
            ("short.wav", GameSound.Hit),
            ("long.wav", GameSound.GameStart));
        try
        {
            var library = new SoundLibrary(root);
            Assert.IsTrue(WavDuration.TryRead(ToneBank.ToWav(GameSound.GameStart), out var expected));
            Assert.AreEqual(expected, library.HoldDuration(GameSound.GameStart, TimeSpan.FromSeconds(40)));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Hold_duration_uses_the_fallback_when_wavs_cannot_be_parsed()
    {
        var root = Path.Combine(Path.GetTempPath(), $"zoolander-sounds-{Guid.NewGuid():N}");
        var directory = Path.Combine(root, SoundFolders.GameStart);
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "broken.wav"), "not a wav");
        try
        {
            var library = new SoundLibrary(root);
            Assert.AreEqual(
                TimeSpan.FromSeconds(40),
                library.HoldDuration(GameSound.GameStart, TimeSpan.FromSeconds(40)));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Attract_stays_silent_when_its_folder_has_no_wavs()
    {
        var root = CreateBank(SoundFolders.Attract);
        try
        {
            var library = new SoundLibrary(root);
            Assert.IsNull(library.Resolve(GameSound.Attract));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateBank(string folder, params (string Name, GameSound Tone)[] files)
    {
        var root = Path.Combine(Path.GetTempPath(), $"zoolander-sounds-{Guid.NewGuid():N}");
        var directory = Path.Combine(root, folder);
        Directory.CreateDirectory(directory);
        foreach (var (name, tone) in files)
        {
            File.WriteAllBytes(Path.Combine(directory, name), ToneBank.ToWav(tone));
        }

        return root;
    }
}
