namespace ZoolanderStompin.Game;

/// <summary>
/// Keyboard stomps have no key-up. Each press latches the pad down long enough for debounce,
/// then it lifts. Holding a key (auto-repeat) keeps the pad down.
/// </summary>
public sealed class KeyboardPadLatch
{
    private readonly IGameClock _clock;
    private readonly TimeSpan _holdDuration;
    private readonly TimeSpan?[] _heldUntil = new TimeSpan?[FloorPad.Count + 1];

    public KeyboardPadLatch(IGameClock clock, TimeSpan holdDuration)
    {
        ArgumentNullException.ThrowIfNull(clock);
        if (holdDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(holdDuration), holdDuration, "Hold duration must be positive.");
        }

        _clock = clock;
        _holdDuration = holdDuration;
    }

    public void Press(FloorPad pad)
    {
        _heldUntil[pad.Number] = _clock.Now + _holdDuration;
    }

    public bool IsHeld(FloorPad pad) =>
        _heldUntil[pad.Number] is { } until && _clock.Now < until;

    public IReadOnlySet<FloorPad> Held
    {
        get
        {
            var held = new HashSet<FloorPad>();
            for (var number = 1; number <= FloorPad.Count; number++)
            {
                var pad = new FloorPad(number);
                if (IsHeld(pad))
                {
                    held.Add(pad);
                }
            }

            return held;
        }
    }
}
