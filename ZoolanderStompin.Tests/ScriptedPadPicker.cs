using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

public sealed class ScriptedPadPicker : IPadPicker
{
    private readonly Queue<FloorPad> _queue;

    public ScriptedPadPicker(params int[] pads)
    {
        _queue = new Queue<FloorPad>(pads.Select(number => new FloorPad(number)));
    }

    public List<IReadOnlyList<FloorPad>> CandidateSets { get; } = [];

    public int Remaining => _queue.Count;

    public FloorPad Next(IReadOnlyList<FloorPad> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        CandidateSets.Add(candidates.ToArray());
        if (_queue.Count == 0)
        {
            throw new InvalidOperationException("Scripted pad picker has no remaining pads.");
        }

        return _queue.Dequeue();
    }
}
