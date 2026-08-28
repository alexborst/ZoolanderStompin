namespace ZoolanderStompin.Game;

public interface IGameClock
{
    TimeSpan Now { get; }

    Task Delay(TimeSpan duration, CancellationToken cancellationToken = default);
}
