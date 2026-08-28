namespace ZoolanderStompin.Game;

public static class ToneBank
{
    public const int SampleRate = 22050;

    public static byte[] ToWav(GameSound sound)
    {
        var pcm = sound switch
        {
            GameSound.NewLight => Concat(Sine(880, 70)),
            GameSound.Hit => Concat(Sine(523, 80), Sine(784, 100)),
            GameSound.Miss => Concat(Sine(185, 180)),
            GameSound.Countdown => Concat(Sine(440, 140)),
            GameSound.Round => Concat(Sine(349, 220)),
            GameSound.GameEnd => Concat(Sine(330, 140), Sine(262, 140), Sine(196, 220)),
            GameSound.Coin => Concat(Sine(1319, 70), Sine(1568, 90)),
            GameSound.Ticket => Concat(Sine(2000, 40), Silence(30), Sine(2000, 40), Silence(30), Sine(2000, 40)),
            _ => Concat(Sine(440, 100)),
        };

        return WrapWav(pcm);
    }

    private static short[] Sine(double frequencyHz, int milliseconds)
    {
        var count = Math.Max(1, SampleRate * milliseconds / 1000);
        var samples = new short[count];
        var fade = Math.Min(count / 8, SampleRate / 100);
        for (var i = 0; i < count; i++)
        {
            var envelope = 1.0;
            if (i < fade)
            {
                envelope = i / (double)fade;
            }
            else if (i > count - fade)
            {
                envelope = (count - i) / (double)fade;
            }

            samples[i] = (short)(Math.Sin(2 * Math.PI * frequencyHz * i / SampleRate) * envelope * 0.28 * short.MaxValue);
        }

        return samples;
    }

    private static short[] Silence(int milliseconds)
    {
        var count = Math.Max(1, SampleRate * milliseconds / 1000);
        return new short[count];
    }

    private static short[] Concat(params short[][] parts)
    {
        var length = parts.Sum(part => part.Length);
        var combined = new short[length];
        var offset = 0;
        foreach (var part in parts)
        {
            Buffer.BlockCopy(part, 0, combined, offset * sizeof(short), part.Length * sizeof(short));
            offset += part.Length;
        }

        return combined;
    }

    private static byte[] WrapWav(short[] pcm)
    {
        var dataSize = pcm.Length * sizeof(short);
        var fileSize = 36 + dataSize;
        using var stream = new MemoryStream(44 + dataSize);
        using var writer = new BinaryWriter(stream);
        writer.Write("RIFF"u8);
        writer.Write(fileSize);
        writer.Write("WAVE"u8);
        writer.Write("fmt "u8);
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(SampleRate);
        writer.Write(SampleRate * 2);
        writer.Write((short)2);
        writer.Write((short)16);
        writer.Write("data"u8);
        writer.Write(dataSize);
        foreach (var sample in pcm)
        {
            writer.Write(sample);
        }

        return stream.ToArray();
    }
}
