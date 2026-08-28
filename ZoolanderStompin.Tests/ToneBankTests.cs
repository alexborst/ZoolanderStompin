using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class ToneBankTests
{
    [TestMethod]
    public void Generated_cues_are_wav_files_and_hit_differs_from_miss()
    {
        var hit = ToneBank.ToWav(GameSound.Hit);
        var miss = ToneBank.ToWav(GameSound.Miss);
        var end = ToneBank.ToWav(GameSound.GameEnd);

        Assert.IsTrue(hit.Length > 44);
        CollectionAssert.AreEqual("RIFF"u8.ToArray(), hit.Take(4).ToArray());
        CollectionAssert.AreNotEqual(hit, miss);
        CollectionAssert.AreNotEqual(miss, end);
    }

    [TestMethod]
    public void Every_game_sound_produces_audio()
    {
        foreach (var sound in Enum.GetValues<GameSound>())
        {
            var wav = ToneBank.ToWav(sound);
            Assert.IsTrue(wav.Length > 44, sound.ToString());
        }
    }
}
