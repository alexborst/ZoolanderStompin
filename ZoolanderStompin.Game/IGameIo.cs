namespace ZoolanderStompin.Game;

public interface IGameIo
{
    GameIoInput Read();

    void Apply(GameIoOutput output);
}
