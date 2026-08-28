using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

public sealed class FakeGameIo : IGameIo
{
    public GameIoInput NextInput { get; set; } = GameIoInput.None;

    public GameIoOutput? LastOutput { get; private set; }

    public int ReadCount { get; private set; }

    public GameIoInput Read()
    {
        ReadCount++;
        return NextInput;
    }

    public void Apply(GameIoOutput output)
    {
        LastOutput = output;
    }
}
