using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class KeyPressQueueTests
{
    [TestMethod]
    public void Dequeues_keys_in_the_order_they_were_enqueued()
    {
        var keys = new KeyPressQueue();
        keys.Enqueue(ConsoleKey.C);
        keys.Enqueue(ConsoleKey.E);

        Assert.IsTrue(keys.TryDequeue(out var first));
        Assert.AreEqual(ConsoleKey.C, first);
        Assert.IsTrue(keys.TryDequeue(out var second));
        Assert.AreEqual(ConsoleKey.E, second);
        Assert.IsFalse(keys.TryDequeue(out _));
    }
}
