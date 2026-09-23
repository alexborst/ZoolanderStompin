namespace ZoolanderStompin.Game;

public sealed record PlaySnapshot
{
    public static PlaySnapshot Empty { get; } = new()
    {
        Title = GameInfo.Name,
        Phase = "",
        Outcome = "",
        Credits = "0",
        Round = "round -",
        Hits = "--",
        Misses = "--",
        Percent = "--%",
        Tickets = "--",
        Prompt = "",
        Pads = "",
        Difficulty = "",
        Pictorial = "",
    };

    public required string Title { get; init; }

    public required string Phase { get; init; }

    public required string Outcome { get; init; }

    public required string Credits { get; init; }

    public required string Round { get; init; }

    public required string Hits { get; init; }

    public required string Misses { get; init; }

    public required string Percent { get; init; }

    public required string Tickets { get; init; }

    public required string Prompt { get; init; }

    public required string Pads { get; init; }

    public required string Difficulty { get; init; }

    public required string Pictorial { get; init; }

    public static PlaySnapshot Capture(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        var output = session.ToOutput();
        var score = session.Score;
        var showScore = score.ResolvedPresentations > 0
            || session.Phase is SessionPhase.Countdown or SessionPhase.Playing
            or SessionPhase.Intermission or SessionPhase.Results;

        var outcome = session.Phase is SessionPhase.Results && session.Result is { } result
            ? (result.Won ? "WIN" : "LOSE")
            : "";

        var pads = string.Join(
            " ",
            Enumerable.Range(1, FloorPad.Count).Select(n =>
            {
                var pad = new FloorPad(n);
                return output.IsPadLampOn(pad) ? $"[{n}]" : $" {n} ";
            }));

        return new PlaySnapshot
        {
            Title = GameInfo.Name,
            Phase = session.Phase.ToString(),
            Outcome = outcome,
            Credits = session.Credits.ToString(),
            Round = session.CurrentRound > 0 ? $"round {session.CurrentRound}" : "round -",
            Hits = showScore ? score.Hits.ToString("00") : "--",
            Misses = showScore ? score.Misses.ToString("00") : "--",
            Percent = score.HitPercent is { } p ? $"{p:0}%" : "--%",
            Tickets = output.TicketDigits?.ToString("00") ?? "--",
            Prompt = FormatPrompt(session),
            Pads = pads,
            Difficulty =
                $"{Lamp("Easy", output.EasyLampOn)} {Lamp("Medium", output.MediumLampOn)} {Lamp("Hard", output.HardLampOn)}",
            Pictorial = string.Join(" ", output.PictorialLampsOn.Select(on => on ? "*" : ".")),
        };
    }

    internal static string FormatPrompt(GameSession session) => session.Phase switch
    {
        SessionPhase.Attract => "Insert a credit (C), then pick Easy, Medium, or Hard.",
        SessionPhase.Select => "Pick Easy, Medium, or Hard.",
        SessionPhase.Countdown => "Get ready — stomps do not score yet.",
        SessionPhase.Playing when session.LitPad is { } pad => $"STOMP {pad.Number}!",
        SessionPhase.Playing => "GO",
        SessionPhase.Intermission => "Round break — same difficulty next.",
        SessionPhase.Results when session.Result is { } result =>
            result.Won
                ? $"You win! Stub payout {result.Tickets} tickets."
                : $"You lose. Stub payout {result.Tickets} tickets.",
        _ => "",
    };

    private static string Lamp(string name, bool on) => on ? $"[{name}]" : name;
}
