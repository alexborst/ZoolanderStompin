namespace ZoolanderStompin.Game;

public static class PlayStatus
{
    public const string KeyLegend =
        "C/Enter credit   E Easy  M Medium  H Hard   1-7 stomp (hold to stand)   Esc quit";

    public const string CreditOnlyKeyLegend =
        "C/Enter credit   1-7 stomp (hold to stand)   Esc quit";

    public static string Format(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        var snap = PlaySnapshot.Capture(session);
        var headerParts = new List<string> { snap.Title, snap.Phase };
        if (snap.Outcome.Length > 0)
        {
            headerParts.Add(snap.Outcome);
        }

        headerParts.Add($"credits {snap.Credits}");
        headerParts.Add(snap.Round);

        var padLine = snap.Difficulty.Length > 0
            ? $"pads {snap.Pads}  |  {snap.Difficulty}  |  pics {snap.Pictorial}"
            : $"pads {snap.Pads}  |  pics {snap.Pictorial}";

        return string.Join(
            Environment.NewLine,
            [
                string.Join("  ", headerParts),
                padLine,
                $"hits {snap.Hits}  misses {snap.Misses}  {snap.Percent}  tickets {snap.Tickets}",
                snap.Prompt,
                snap.KeyLegend,
            ]);
    }
}
