using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class PadDebouncerTests
{
    [TestMethod]
    public void Rising_edge_fires_only_after_the_debounce_window()
    {
        var clock = new FakeGameClock();
        var debounce = TimeSpan.FromMilliseconds(30);
        var debouncer = new PadDebouncer(clock, debounce);
        var pad = new FloorPad(3);

        debouncer.Observe(new HashSet<FloorPad> { pad });
        Assert.AreEqual(0, debouncer.RisingEdges.Count);
        Assert.IsFalse(debouncer.StableHeld.Contains(pad));

        clock.Advance(debounce - TimeSpan.FromMilliseconds(1));
        debouncer.Observe(new HashSet<FloorPad> { pad });
        Assert.AreEqual(0, debouncer.RisingEdges.Count);

        clock.Advance(TimeSpan.FromMilliseconds(1));
        debouncer.Observe(new HashSet<FloorPad> { pad });
        Assert.IsTrue(debouncer.RisingEdges.Contains(pad));
        Assert.IsTrue(debouncer.StableHeld.Contains(pad));
    }

    [TestMethod]
    public void Bounce_shorter_than_debounce_is_not_a_press()
    {
        var clock = new FakeGameClock();
        var debounce = TimeSpan.FromMilliseconds(30);
        var debouncer = new PadDebouncer(clock, debounce);
        var pad = new FloorPad(1);

        debouncer.Observe(new HashSet<FloorPad> { pad });
        clock.Advance(TimeSpan.FromMilliseconds(10));
        debouncer.Observe(new HashSet<FloorPad>());
        clock.Advance(TimeSpan.FromMilliseconds(10));
        debouncer.Observe(new HashSet<FloorPad> { pad });
        clock.Advance(TimeSpan.FromMilliseconds(10));
        debouncer.Observe(new HashSet<FloorPad> { pad });

        Assert.AreEqual(0, debouncer.RisingEdges.Count);
        Assert.IsFalse(debouncer.StableHeld.Contains(pad));
    }

    [TestMethod]
    public void Falling_edge_fires_after_a_stable_release()
    {
        var clock = new FakeGameClock();
        var debounce = TimeSpan.FromMilliseconds(30);
        var debouncer = new PadDebouncer(clock, debounce);
        var pad = new FloorPad(2);

        debouncer.Observe(new HashSet<FloorPad> { pad });
        clock.Advance(debounce);
        debouncer.Observe(new HashSet<FloorPad> { pad });
        Assert.IsTrue(debouncer.StableHeld.Contains(pad));

        debouncer.Observe(new HashSet<FloorPad>());
        Assert.AreEqual(0, debouncer.FallingEdges.Count);

        clock.Advance(debounce);
        debouncer.Observe(new HashSet<FloorPad>());
        Assert.IsTrue(debouncer.FallingEdges.Contains(pad));
        Assert.IsFalse(debouncer.StableHeld.Contains(pad));
    }
}
