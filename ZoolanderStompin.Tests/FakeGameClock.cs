using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

public sealed class FakeGameClock : IGameClock
{
    public TimeSpan Now { get; private set; }

    public void Advance(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration cannot be negative.");
        }

        Now += duration;
    }

    public Task Delay(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Advance(duration);
        return Task.CompletedTask;
    }
}
