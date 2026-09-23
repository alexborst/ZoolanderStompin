using System.Collections.Concurrent;

namespace ZoolanderStompin.Game;

public sealed class KeyPressQueue
{
    private readonly ConcurrentQueue<ConsoleKey> _keys = new();

    public void Enqueue(ConsoleKey key) => _keys.Enqueue(key);

    public bool TryDequeue(out ConsoleKey key) => _keys.TryDequeue(out key);
}
