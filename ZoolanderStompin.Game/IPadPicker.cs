namespace ZoolanderStompin.Game;

public interface IPadPicker
{
    FloorPad Next(IReadOnlyList<FloorPad> candidates);
}
