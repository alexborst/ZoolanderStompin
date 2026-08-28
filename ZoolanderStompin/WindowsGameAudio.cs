using System.Media;
using System.Runtime.Versioning;
using Microsoft.Extensions.Hosting;
using ZoolanderStompin.Game;

namespace ZoolanderStompin;

[SupportedOSPlatform("windows")]
public sealed class WindowsGameAudio : IGameAudio
{
    private readonly string _soundsDirectory;
    private readonly List<object> _keepAlive = [];

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
            var wav = LoadWav(sound.Value);
            var stream = new MemoryStream(wav, writable: false);
            var player = new SoundPlayer(stream);
            player.Play();
            _keepAlive.Add(stream);
            _keepAlive.Add(player);
            if (_keepAlive.Count > 24)
            {
                _keepAlive.RemoveRange(0, 8);
            }
        }
        catch
        {
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
