namespace ZoolanderStompin.Game;

public sealed class CompositeGameIo : IGameIo
{
    private readonly IGameIo[] _adapters;

    public CompositeGameIo(params IGameIo[] adapters)
    {
        ArgumentNullException.ThrowIfNull(adapters);
        if (adapters.Length < 2)
        {
            throw new ArgumentException("At least two I/O adapters are required.", nameof(adapters));
        }

        foreach (var adapter in adapters)
        {
            ArgumentNullException.ThrowIfNull(adapter);
        }

        _adapters = adapters;
    }

    public GameIoInput Read()
    {
        var combined = _adapters[0].Read();
        for (var i = 1; i < _adapters.Length; i++)
        {
            combined = combined.Combine(_adapters[i].Read());
        }

        return combined;
    }

    public void Apply(GameIoOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);
        foreach (var adapter in _adapters)
        {
            adapter.Apply(output);
        }
    }
}
