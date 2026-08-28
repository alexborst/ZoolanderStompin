namespace ZoolanderStompin.Game;

public readonly record struct GameResult
{
    public Score Score { get; }
    public double HitPercent { get; }
    public bool Won { get; }
    public int Tickets { get; }

    public GameResult(Score score, double hitPercent, bool won, int tickets)
    {
        if (hitPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(hitPercent), hitPercent, "Hit percent must be between 0 and 100.");
        }

        if (tickets < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tickets), tickets, "Tickets cannot be negative.");
        }

        Score = score;
        HitPercent = hitPercent;
        Won = won;
        Tickets = Math.Min(99, tickets);
    }

    public static GameResult Evaluate(Score score, GameOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var percent = score.HitPercent ?? 0;
        var won = percent >= options.WinPercentThreshold;
        var tickets = options.Payout.TicketsForHitPercent(percent);
        return new GameResult(score, percent, won, tickets);
    }
}
