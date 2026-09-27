using System.Diagnostics;
using System.Media;
using System.Runtime.Versioning;
using ZoolanderStompin.Game;

namespace ZoolanderStompin;

[SupportedOSPlatform("windows")]
public sealed class WindowsGameAudio : IGameAudio
{
    private readonly SoundLibrary _library;
    private readonly object _gate = new();
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private SoundPlayer? _player;
    private TimeSpan _playingUntil;

    public WindowsGameAudio(SoundLibrary library)
    {
        _library = library;
    }

    public void Play(GameSound? sound)
    {
        if (sound is null || !OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            var clip = _library.Resolve(sound.Value);
            if (clip is null)
            {
                return;
            }

            lock (_gate)
            {
                // SoundPlayer has one channel: starting a sound stops the current one.
                // Only cues that are allowed to interrupt may cut a clip that is still playing.
                if (_player is not null && _clock.Elapsed < _playingUntil && !SoundFolders.InterruptsPlayback(sound.Value))
                {
                    return;
                }

                StopPlayer();
                _player = CreatePlayer(clip);
                _player.Play();
                _playingUntil = _clock.Elapsed + Duration(clip);
            }
        }
        catch
        {
        }
    }

    private static TimeSpan Duration(SoundClip clip)
    {
        var known = clip.FilePath is { } path
            ? WavDuration.TryRead(path, out var fromFile) ? fromFile : TimeSpan.Zero
            : WavDuration.TryRead(clip.WavBytes ?? [], out var fromBytes) ? fromBytes : TimeSpan.Zero;
        return known;
    }

    private static SoundPlayer CreatePlayer(SoundClip clip)
    {
        if (clip.FilePath is { } path)
        {
            return new SoundPlayer(path);
        }

        var stream = new MemoryStream(clip.WavBytes ?? [], writable: false);
        return new SoundPlayer(stream);
    }

    private void StopPlayer()
    {
        try
        {
            _player?.Stop();
            _player?.Dispose();
        }
        catch
        {
        }

        _player = null;
        _playingUntil = TimeSpan.Zero;
    }
}
