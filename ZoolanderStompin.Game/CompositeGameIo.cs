namespace ZoolanderStompin.Game;

public sealed class CompositeGameIo : IGameIo
{
    private readonly IGameIo _primary;
    private readonly IGameIo _overlay;

    public CompositeGameIo(IGameIo primary, IGameIo overlay)
    {
        ArgumentNullException.ThrowIfNull(primary);
        ArgumentNullException.ThrowIfNull(overlay);

        _primary = primary;
        _overlay = overlay;
    }

    public GameIoInput Read() => _primary.Read().Combine(_overlay.Read());

    public void Apply(GameIoOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _primary.Apply(output);
        _overlay.Apply(output);
    }
}
