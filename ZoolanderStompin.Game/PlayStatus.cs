namespace ZoolanderStompin.Game;

public static class PlayStatus
{
    public const string KeyLegend =
        "C/Enter credit   E Easy  M Medium  H Hard   1-7 stomp (hold to stand)   Esc quit";

    public static string Format(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        var output = session.ToOutput();
        var score = session.Score;
        var percent = score.HitPercent is { } p ? $"{p:0}%" : "--%";
        var hits = score.ResolvedPresentations > 0 || session.Phase is SessionPhase.Countdown or SessionPhase.Playing
            or SessionPhase.Intermission or SessionPhase.Results
            ? score.Hits.ToString("00")
            : "--";
        var misses = score.ResolvedPresentations > 0 || session.Phase is SessionPhase.Countdown or SessionPhase.Playing
            or SessionPhase.Intermission or SessionPhase.Results
            ? score.Misses.ToString("00")
            : "--";
        var tickets = output.TicketDigits?.ToString("00") ?? "--";
        var round = session.CurrentRound > 0
            ? $"round {session.CurrentRound}"
            : "round -";
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

        var difficulty =
            $"{Lamp("Easy", output.EasyLampOn)} {Lamp("Medium", output.MediumLampOn)} {Lamp("Hard", output.HardLampOn)}";
        var pictorial = string.Join(" ", output.PictorialLampsOn.Select(on => on ? "*" : "."));
        var prompt = Prompt(session);

        var headerParts = new List<string> { GameInfo.Name, session.Phase.ToString() };
        if (outcome.Length > 0)
        {
            headerParts.Add(outcome);
        }

        headerParts.Add($"credits {session.Credits}");
        headerParts.Add(round);

        return string.Join(
            Environment.NewLine,
            [
                string.Join("  ", headerParts),
                $"pads {pads}  |  {difficulty}  |  pics {pictorial}",
                $"hits {hits}  misses {misses}  {percent}  tickets {tickets}",
                prompt,
                KeyLegend,
            ]);
    }

    private static string Prompt(GameSession session)
    {
        return session.Phase switch
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
    }

    private static string Lamp(string name, bool on) => on ? $"[{name}]" : name;
}
