namespace ZoolanderStompin.Game;

public sealed class PadDebouncer
{
    private readonly IGameClock _clock;
    private readonly TimeSpan _duration;
    private readonly bool[] _raw = new bool[FloorPad.Count + 1];
    private readonly bool[] _stable = new bool[FloorPad.Count + 1];
    private readonly TimeSpan[] _changedAt = new TimeSpan[FloorPad.Count + 1];
    private readonly HashSet<FloorPad> _rawHeld = [];
    private readonly HashSet<FloorPad> _stableHeld = [];
    private readonly HashSet<FloorPad> _risingEdges = [];
    private readonly HashSet<FloorPad> _fallingEdges = [];

    public PadDebouncer(IGameClock clock, TimeSpan duration)
    {
        ArgumentNullException.ThrowIfNull(clock);
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration cannot be negative.");
        }

        _clock = clock;
        _duration = duration;
    }

    public IReadOnlySet<FloorPad> RawHeld => _rawHeld;

    public IReadOnlySet<FloorPad> StableHeld => _stableHeld;

    public IReadOnlySet<FloorPad> RisingEdges => _risingEdges;

    public IReadOnlySet<FloorPad> FallingEdges => _fallingEdges;

    public void Observe(IEnumerable<FloorPad> rawHeld)
    {
        ArgumentNullException.ThrowIfNull(rawHeld);
        var heldNow = rawHeld as IReadOnlySet<FloorPad> ?? rawHeld.ToHashSet();

        _rawHeld.Clear();
        _risingEdges.Clear();
        _fallingEdges.Clear();

        for (var number = 1; number <= FloorPad.Count; number++)
        {
            var pad = new FloorPad(number);
            var held = heldNow.Contains(pad);
            if (held)
            {
                _rawHeld.Add(pad);
            }

            if (held != _raw[number])
            {
                _raw[number] = held;
                _changedAt[number] = _clock.Now;
            }

            if (_raw[number] == _stable[number])
            {
                continue;
            }

            if (_clock.Now - _changedAt[number] < _duration)
            {
                continue;
            }

            _stable[number] = _raw[number];
            if (_stable[number])
            {
                _stableHeld.Add(pad);
                _risingEdges.Add(pad);
            }
            else
            {
                _stableHeld.Remove(pad);
                _fallingEdges.Add(pad);
            }
        }
    }
}
