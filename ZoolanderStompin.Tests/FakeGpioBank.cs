using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

public sealed class FakeGpioBank : IGpioBank
{
    private readonly Dictionary<int, bool> _high = [];
    private readonly HashSet<int> _inputs = [];
    private readonly HashSet<int> _outputs = [];

    public IReadOnlyCollection<int> Inputs => _inputs;

    public IReadOnlyCollection<int> Outputs => _outputs;

    public void OpenInputPullUp(int bcm)
    {
        _inputs.Add(bcm);
        _high[bcm] = true;
    }

    public void OpenOutput(int bcm, bool initialHigh)
    {
        _outputs.Add(bcm);
        _high[bcm] = initialHigh;
    }

    public bool ReadHigh(int bcm) => _high[bcm];

    public void WriteHigh(int bcm, bool high) => _high[bcm] = high;

    public void SetHigh(int bcm, bool high) => _high[bcm] = high;
}
