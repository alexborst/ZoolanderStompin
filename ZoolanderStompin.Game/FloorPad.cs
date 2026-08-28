namespace ZoolanderStompin.Game;

/// <summary>
/// One of the seven floor stomping targets (switch and lamp as a single playable location).
/// </summary>
public readonly record struct FloorPad
{
    public const int Count = 7;

    public int Number { get; }

    public FloorPad(int number)
    {
        if (number is < 1 or > Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                number,
                "Floor pad number must be between 1 and 7.");
        }

        Number = number;
    }

    public override string ToString() => Number.ToString();
}
