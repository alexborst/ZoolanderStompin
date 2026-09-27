namespace ZoolanderStompin.Game;

public static class RoundRating
{
    /// <summary>
    /// Rates one round: perfect when every presentation was hit, otherwise by the configured hit thresholds.
    /// </summary>
    public static RoundPerformance Rate(int roundHits, GameOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (roundHits >= options.PresentationsPerRound)
        {
            return RoundPerformance.Perfect;
        }

        if (roundHits >= options.RoundEndGoodMinimumHits)
        {
            return RoundPerformance.Good;
        }

        if (roundHits >= options.RoundEndMediumMinimumHits)
        {
            return RoundPerformance.Medium;
        }

        return RoundPerformance.Bad;
    }

    public static GameSound Sound(RoundPerformance performance) => performance switch
    {
        RoundPerformance.Perfect => GameSound.RoundEndPerfect,
        RoundPerformance.Good => GameSound.RoundEndGood,
        RoundPerformance.Medium => GameSound.RoundEndMedium,
        _ => GameSound.RoundEndBad,
    };
}
