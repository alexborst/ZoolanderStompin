namespace ZoolanderStompin.Game;

public static class SoundFolders
{
    public const string RootName = "Sounds";

    public const string Hits = "hits";

    public const string Misses = "misses";

    public const string GameStart = "game-start";

    public const string GameEnd = "game-end";

    public const string Attract = "attract";

    public const string RoundEnd = "round-end";

    public const string RoundEndBad = RoundEnd + "/bad";

    public const string RoundEndMedium = RoundEnd + "/medium";

    public const string RoundEndGood = RoundEnd + "/good";

    public const string RoundEndPerfect = RoundEnd + "/perfect";

    public static readonly string[] Banks =
    [
        Hits,
        Misses,
        GameStart,
        GameEnd,
        Attract,
        RoundEndBad,
        RoundEndMedium,
        RoundEndGood,
        RoundEndPerfect,
    ];

    public static string? For(GameSound sound) => sound switch
    {
        GameSound.Hit => Hits,
        GameSound.Miss => Misses,
        GameSound.GameStart => GameStart,
        GameSound.GameEnd => GameEnd,
        GameSound.Attract => Attract,
        GameSound.RoundEndBad => RoundEndBad,
        GameSound.RoundEndMedium => RoundEndMedium,
        GameSound.RoundEndGood => RoundEndGood,
        GameSound.RoundEndPerfect => RoundEndPerfect,
        _ => null,
    };

    public static bool InterruptsPlayback(GameSound sound) =>
        sound is GameSound.Hit
            or GameSound.Miss
            or GameSound.Attract
            or GameSound.GameStart
            or GameSound.GameEnd
            or GameSound.RoundEndBad
            or GameSound.RoundEndMedium
            or GameSound.RoundEndGood
            or GameSound.RoundEndPerfect;
}
