namespace ZoolanderStompin.Game;

public readonly record struct Score
{
    public int Hits { get; }
    public int Misses { get; }

    public Score(int hits, int misses)
    {
        if (hits < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hits), hits, "Hits cannot be negative.");
        }

        if (misses < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(misses), misses, "Misses cannot be negative.");
        }

        Hits = hits;
        Misses = misses;
    }

    public int ResolvedPresentations => Hits + Misses;

    /// <summary>
    /// Hits as a percent of resolved presentations. Null when nothing has been resolved yet
    /// (unshown presentations do not belong in the denominator).
    /// </summary>
    public double? HitPercent =>
        ResolvedPresentations == 0 ? null : 100.0 * Hits / ResolvedPresentations;

    public Score WithHit() => new(Hits + 1, Misses);

    public Score WithMiss() => new(Hits, Misses + 1);
}
