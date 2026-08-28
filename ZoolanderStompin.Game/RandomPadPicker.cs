namespace ZoolanderStompin.Game;

public sealed class RandomPadPicker : IPadPicker
{
    private readonly Random _random;

    public RandomPadPicker()
        : this(Random.Shared)
    {
    }

    public RandomPadPicker(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);
        _random = random;
    }

    public FloorPad Next(IReadOnlyList<FloorPad> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        if (candidates.Count == 0)
        {
            throw new ArgumentException("At least one pad candidate is required.", nameof(candidates));
        }

        return candidates[_random.Next(candidates.Count)];
    }
}
