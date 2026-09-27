using System.Diagnostics;
using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public sealed class LinuxAlsaAudio : IGameAudio
{
    private readonly SoundLibrary _library;
    private readonly string? _device;
    private readonly object _gate = new();
    private Process? _aplay;
    private string? _tempWav;
    private int _announced;

    public LinuxAlsaAudio(SoundLibrary library, string? device = null)
    {
        _library = library;
        _device = string.IsNullOrWhiteSpace(device) ? null : device.Trim();
    }

    public void Play(GameSound? sound)
    {
        if (sound is null || !OperatingSystem.IsLinux())
        {
            return;
        }

        try
        {
            AnnounceOnce();
            var clip = _library.Resolve(sound.Value);
            if (clip is null)
            {
                return;
            }

            lock (_gate)
            {
                if (_aplay is { HasExited: false })
                {
                    if (!SoundFolders.InterruptsPlayback(sound.Value))
                    {
                        return;
                    }

                    StopPlayback();
                }

                var path = clip.FilePath ?? WriteTemp(clip.WavBytes);
                if (path is null)
                {
                    return;
                }

                StartAplay(path);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Audio play failed: {ex.Message}");
        }
    }

    private void StartAplay(string path)
    {
        var info = new ProcessStartInfo
        {
            FileName = AplayPath(),
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = AppContext.BaseDirectory,
        };
        if (_device is not null)
        {
            info.ArgumentList.Add("-D");
            info.ArgumentList.Add(_device);
        }

        info.ArgumentList.Add("-q");
        info.ArgumentList.Add(path);

        var process = new Process
        {
            StartInfo = info,
            EnableRaisingEvents = true,
        };
        process.Exited += OnAplayExited;
        if (!process.Start())
        {
            Console.Error.WriteLine($"aplay did not start for {path}");
            process.Dispose();
            return;
        }

        _aplay = process;
    }

    private void OnAplayExited(object? sender, EventArgs e)
    {
        if (sender is not Process process)
        {
            return;
        }

        lock (_gate)
        {
            if (!ReferenceEquals(process, _aplay))
            {
                return;
            }

            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine($"aplay exited {process.ExitCode}");
            }

            try
            {
                process.Dispose();
            }
            catch
            {
            }

            _aplay = null;
            DeleteTemp();
        }
    }

    private void AnnounceOnce()
    {
        if (Interlocked.Exchange(ref _announced, 1) == 1)
        {
            return;
        }

        var banks = string.Join(
            ", ",
            SoundFolders.Banks.Select(bank => $"{bank}={_library.WavCount(bank)}"));
        var device = _device ?? "default";
        Console.WriteLine($"Audio folder {_library.Root} device={device} ({banks})");
    }

    private static string AplayPath() =>
        File.Exists("/usr/bin/aplay") ? "/usr/bin/aplay" : "aplay";

    private string? WriteTemp(byte[]? wav)
    {
        if (wav is null || wav.Length == 0)
        {
            return null;
        }

        var path = Path.Combine(Path.GetTempPath(), $"zoolander-{Guid.NewGuid():N}.wav");
        File.WriteAllBytes(path, wav);
        _tempWav = path;
        return path;
    }

    private void StopPlayback()
    {
        if (_aplay is { } process)
        {
            process.Exited -= OnAplayExited;
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch
            {
            }

            try
            {
                process.Dispose();
            }
            catch
            {
            }

            _aplay = null;
        }

        DeleteTemp();
    }

    private void DeleteTemp()
    {
        if (_tempWav is not { } leftover)
        {
            return;
        }

        try
        {
            File.Delete(leftover);
        }
        catch
        {
        }

        _tempWav = null;
    }
}
