using System.Collections.Concurrent;
using System.Media;
using System.Runtime.Versioning;
using Microsoft.Extensions.Hosting;
using ZoolanderStompin.Game;

namespace ZoolanderStompin;

[SupportedOSPlatform("windows")]
public sealed class WindowsGameAudio : IGameAudio
{
    private readonly string _soundsDirectory;
    private readonly ConcurrentQueue<byte[]> _pending = new();
    private int _playing;

    public WindowsGameAudio(IHostEnvironment environment)
    {
        _soundsDirectory = Path.Combine(environment.ContentRootPath, "Sounds");
    }

    public void Play(GameSound? sound)
    {
        if (sound is null || !OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            _pending.Enqueue(LoadWav(sound.Value));
            if (Interlocked.CompareExchange(ref _playing, 1, 0) == 0)
            {
                ThreadPool.QueueUserWorkItem(_ => Drain());
            }
        }
        catch
        {
        }
    }

    private void Drain()
    {
        try
        {
            while (_pending.TryDequeue(out var wav))
            {
                using var stream = new MemoryStream(wav, writable: false);
                using var player = new SoundPlayer(stream);
                player.PlaySync();
            }
        }
        catch
        {
        }
        finally
        {
            Interlocked.Exchange(ref _playing, 0);
            if (!_pending.IsEmpty && Interlocked.CompareExchange(ref _playing, 1, 0) == 0)
            {
                ThreadPool.QueueUserWorkItem(_ => Drain());
            }
        }
    }

    private byte[] LoadWav(GameSound sound)
    {
        var path = Path.Combine(_soundsDirectory, $"{sound}.wav");
        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }

        return ToneBank.ToWav(sound);
    }
}
