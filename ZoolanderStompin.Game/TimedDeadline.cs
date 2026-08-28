namespace ZoolanderStompin.Game;

public sealed class TimedDeadline
{
    private readonly IGameClock _clock;
    private readonly TimeSpan _deadline;

    public TimedDeadline(IGameClock clock, TimeSpan duration)
    {
        ArgumentNullException.ThrowIfNull(clock);
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration cannot be negative.");
        }

        _clock = clock;
        _deadline = clock.Now + duration;
    }

    public bool IsExpired => _clock.Now >= _deadline;
}
