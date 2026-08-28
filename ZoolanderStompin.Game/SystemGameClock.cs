using System.Diagnostics;

namespace ZoolanderStompin.Game;

public sealed class SystemGameClock : IGameClock
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public TimeSpan Now => _stopwatch.Elapsed;

    public Task Delay(TimeSpan duration, CancellationToken cancellationToken = default) =>
        Task.Delay(duration, cancellationToken);
}
