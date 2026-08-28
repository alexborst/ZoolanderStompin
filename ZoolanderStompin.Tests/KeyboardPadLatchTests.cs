using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class KeyboardPadLatchTests
{
    [TestMethod]
    public void A_press_stays_held_through_debounce_then_lifts()
    {
        var clock = new FakeGameClock();
        var debounce = TimeSpan.FromMilliseconds(30);
        var hold = TimeSpan.FromMilliseconds(80);
        var latch = new KeyboardPadLatch(clock, hold);
        var pad = new FloorPad(3);

        latch.Press(pad);
        Assert.IsTrue(latch.IsHeld(pad));

        clock.Advance(debounce);
        Assert.IsTrue(latch.IsHeld(pad));

        clock.Advance(hold - debounce);
        Assert.IsFalse(latch.IsHeld(pad));
    }

    [TestMethod]
    public void Another_press_extends_the_hold()
    {
        var clock = new FakeGameClock();
        var latch = new KeyboardPadLatch(clock, TimeSpan.FromMilliseconds(80));
        var pad = new FloorPad(1);

        latch.Press(pad);
        clock.Advance(TimeSpan.FromMilliseconds(70));
        latch.Press(pad);
        clock.Advance(TimeSpan.FromMilliseconds(70));

        Assert.IsTrue(latch.IsHeld(pad));

        clock.Advance(TimeSpan.FromMilliseconds(10));
        Assert.IsFalse(latch.IsHeld(pad));
    }
}
