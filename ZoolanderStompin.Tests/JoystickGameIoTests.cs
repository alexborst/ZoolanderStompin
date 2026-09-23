using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class JoystickGameIoTests
{
    [TestMethod]
    public void Mapped_button_held_is_pad1()
    {
        var stick = new FakeJoystickDevice();
        var io = new JoystickGameIo(new JoystickOptions { Pad1Button = 3 }, stick);
        stick.SetHeld(3, true);
        stick.SetHeld(0, true);

        var input = io.Read();

        Assert.AreEqual(1, stick.PumpCount);
        Assert.IsTrue(input.IsPadHeld(new FloorPad(1)));
        Assert.AreEqual(1, input.PadsHeld.Count);
    }

    [TestMethod]
    public void Mapped_button_up_is_not_a_stomp()
    {
        var stick = new FakeJoystickDevice();
        var io = new JoystickGameIo(new JoystickOptions { Pad1Button = 0 }, stick);

        Assert.IsFalse(io.Read().IsPadHeld(new FloorPad(1)));
    }
}
