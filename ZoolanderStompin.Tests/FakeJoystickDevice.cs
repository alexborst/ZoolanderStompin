using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

public sealed class FakeJoystickDevice : IJoystickDevice
{
    private readonly HashSet<int> _held = [];

    public int PumpCount { get; private set; }

    public void SetHeld(int button, bool held)
    {
        if (held)
        {
            _held.Add(button);
        }
        else
        {
            _held.Remove(button);
        }
    }

    public void Pump() => PumpCount++;

    public bool IsButtonHeld(int button) => _held.Contains(button);
}
