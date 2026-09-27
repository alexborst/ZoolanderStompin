using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class WavDurationTests
{
    [TestMethod]
    public void Reads_pcm_duration_from_a_generated_wav()
    {
        var wav = ToneBank.ToWav(GameSound.GameStart);

        Assert.IsTrue(WavDuration.TryRead(wav, out var duration));
        Assert.IsTrue(duration > TimeSpan.FromMilliseconds(400), duration.ToString());
        Assert.IsTrue(duration < TimeSpan.FromMilliseconds(500), duration.ToString());
    }

    [TestMethod]
    public void Rejects_bytes_that_are_not_a_wav()
    {
        Assert.IsFalse(WavDuration.TryRead("not a wav"u8.ToArray(), out var duration));
        Assert.AreEqual(TimeSpan.Zero, duration);
    }
}
