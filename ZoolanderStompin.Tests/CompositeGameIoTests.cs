using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class CompositeGameIoTests
{
    [TestMethod]
    public void Combines_keyboard_and_gpio_stomps_and_applies_both()
    {
        var keyboard = new FakeGameIo
        {
            NextInput = new GameIoInput(
                padsHeld: [new FloorPad(2)],
                easyHeld: true,
                mediumHeld: false,
                hardHeld: false,
                creditHeld: true,
                serviceCreditHeld: false,
                ticketNotchHeld: false),
        };
        var gpio = new FakeGameIo
        {
            NextInput = new GameIoInput(
                padsHeld: [new FloorPad(1)],
                easyHeld: false,
                mediumHeld: false,
                hardHeld: false,
                creditHeld: false,
                serviceCreditHeld: false,
                ticketNotchHeld: false),
        };
        var io = new CompositeGameIo(keyboard, gpio);

        var input = io.Read();
        Assert.IsTrue(input.IsPadHeld(new FloorPad(1)));
        Assert.IsTrue(input.IsPadHeld(new FloorPad(2)));
        Assert.IsTrue(input.EasyHeld);
        Assert.IsTrue(input.CreditHeld);

        var output = GameIoOutput.Off;
        io.Apply(output);
        Assert.AreSame(output, keyboard.LastOutput);
        Assert.AreSame(output, gpio.LastOutput);
    }
}
