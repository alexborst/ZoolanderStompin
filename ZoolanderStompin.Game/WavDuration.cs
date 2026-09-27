namespace ZoolanderStompin.Game;

public static class WavDuration
{
    public static bool TryRead(string path, out TimeSpan duration)
    {
        duration = TimeSpan.Zero;
        try
        {
            return TryRead(File.ReadAllBytes(path), out duration);
        }
        catch
        {
            return false;
        }
    }

    public static bool TryRead(byte[] wav, out TimeSpan duration)
    {
        duration = TimeSpan.Zero;
        if (wav.Length < 44 || !Matches(wav, 0, "RIFF") || !Matches(wav, 8, "WAVE"))
        {
            return false;
        }

        var offset = 12;
        int? byteRate = null;
        int? dataSize = null;
        while (offset + 8 <= wav.Length)
        {
            var size = BitConverter.ToInt32(wav, offset + 4);
            if (size < 0)
            {
                return false;
            }

            if (Matches(wav, offset, "fmt ") && size >= 16 && offset + 24 <= wav.Length)
            {
                byteRate = BitConverter.ToInt32(wav, offset + 16);
            }
            else if (Matches(wav, offset, "data"))
            {
                dataSize = size;
                break;
            }

            offset += 8 + size;
            if ((size & 1) == 1)
            {
                offset++;
            }
        }

        if (byteRate is not > 0 || dataSize is not >= 0)
        {
            return false;
        }

        duration = TimeSpan.FromSeconds(dataSize.Value / (double)byteRate.Value);
        return duration > TimeSpan.Zero;
    }

    private static bool Matches(byte[] wav, int offset, string ascii)
    {
        if (offset + ascii.Length > wav.Length)
        {
            return false;
        }

        for (var i = 0; i < ascii.Length; i++)
        {
            if (wav[offset + i] != (byte)ascii[i])
            {
                return false;
            }
        }

        return true;
    }
}
