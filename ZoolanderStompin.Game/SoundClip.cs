namespace ZoolanderStompin.Game;

public sealed class SoundClip
{
    private SoundClip(string? filePath, byte[]? wavBytes)
    {
        FilePath = filePath;
        WavBytes = wavBytes;
    }

    public string? FilePath { get; }

    public byte[]? WavBytes { get; }

    public static SoundClip FromFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        return new SoundClip(filePath, null);
    }

    public static SoundClip FromWav(byte[] wavBytes)
    {
        ArgumentNullException.ThrowIfNull(wavBytes);
        return new SoundClip(null, wavBytes);
    }
}
