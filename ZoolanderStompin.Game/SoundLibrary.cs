namespace ZoolanderStompin.Game;

public sealed class SoundLibrary
{
    private readonly string _root;
    private readonly Func<int, int> _pick;
    private readonly Dictionary<GameSound, int> _lastIndex = [];

    public SoundLibrary(string rootDirectory, Func<int, int>? pick = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);
        _root = rootDirectory;
        _pick = pick ?? (count => Random.Shared.Next(count));
    }

    public string Root => _root;

    public SoundClip? Resolve(GameSound sound)
    {
        var folder = SoundFolders.For(sound);
        if (folder is not null)
        {
            var files = ListWavs(Path.Combine(_root, folder));
            if (files.Length > 0)
            {
                return SoundClip.FromFile(Pick(files, sound));
            }

            if (sound is GameSound.Attract)
            {
                return null;
            }
        }

        return SoundClip.FromWav(ToneBank.ToWav(sound));
    }

    public int WavCount(string folder) => ListWavs(Path.Combine(_root, folder)).Length;

    public TimeSpan HoldDuration(GameSound sound, TimeSpan unreadFallback)
    {
        var folder = SoundFolders.For(sound);
        if (folder is not null)
        {
            var files = ListWavs(Path.Combine(_root, folder));
            if (files.Length > 0)
            {
                var max = TimeSpan.Zero;
                var parsed = false;
                foreach (var file in files)
                {
                    if (!WavDuration.TryRead(file, out var duration))
                    {
                        continue;
                    }

                    parsed = true;
                    if (duration > max)
                    {
                        max = duration;
                    }
                }

                return parsed ? max : unreadFallback;
            }

            if (sound is GameSound.Attract)
            {
                return TimeSpan.Zero;
            }
        }

        return WavDuration.TryRead(ToneBank.ToWav(sound), out var tone) ? tone : unreadFallback;
    }


    private string Pick(string[] files, GameSound sound)
    {
        var index = Math.Clamp(_pick(files.Length), 0, files.Length - 1);
        if (files.Length > 1 && _lastIndex.TryGetValue(sound, out var last) && index == last)
        {
            index = (index + 1) % files.Length;
        }

        _lastIndex[sound] = index;
        return files[index];
    }

    private static string[] ListWavs(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return [];
        }

        return Directory.GetFiles(directory)
            .Where(path => path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
