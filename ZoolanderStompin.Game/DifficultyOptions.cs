namespace ZoolanderStompin.Game;

public sealed class DifficultyOptions
{
    public int[] PadsInPlay { get; set; } = [];

    public int HitWindowMilliseconds { get; set; }

    public IReadOnlyList<FloorPad> Pads => PadsInPlay.Select(number => new FloorPad(number)).ToArray();
}
