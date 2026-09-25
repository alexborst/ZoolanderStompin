using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class JoystickGameIoTests
{
    [TestMethod]
    public void Mapped_buttons_are_the_seven_floor_pads()
    {
        var stick = new FakeJoystickDevice();
        var io = new JoystickGameIo(
            new JoystickOptions
            {
                Pad1Button = 1,
                Pad2Button = 2,
                Pad3Button = 3,
                Pad4Button = 4,
                Pad5Button = 5,
                Pad6Button = 6,
                Pad7Button = 7,
            },
            stick);
        stick.SetHeld(3, true);
        stick.SetHeld(7, true);
        stick.SetHeld(0, true);

        var input = io.Read();

        Assert.AreEqual(1, stick.PumpCount);
        Assert.IsTrue(input.IsPadHeld(new FloorPad(3)));
        Assert.IsTrue(input.IsPadHeld(new FloorPad(7)));
        Assert.AreEqual(2, input.PadsHeld.Count);
    }

    [TestMethod]
    public void Mapped_button_up_is_not_a_stomp()
    {
        var stick = new FakeJoystickDevice();
        var io = new JoystickGameIo(new JoystickOptions { Pad1Button = 0 }, stick);

        Assert.IsFalse(io.Read().IsPadHeld(new FloorPad(1)));
    }
}
