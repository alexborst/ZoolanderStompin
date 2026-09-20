using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

[TestClass]
public class GpioGameIoTests
{
    [TestMethod]
    public void A_low_pad1_input_is_a_held_stomp()
    {
        var bank = new FakeGpioBank();
        var io = new GpioGameIo(new GpioOptions(), bank);
        bank.SetHigh(GpioOptions.DefaultPad1InputBcm, false);

        var input = io.Read();

        Assert.IsTrue(input.IsPadHeld(new FloorPad(1)));
        Assert.AreEqual(0, input.PadsHeld.Count(pad => pad.Number != 1));
    }

    [TestMethod]
    public void A_high_pad1_input_is_released()
    {
        var bank = new FakeGpioBank();
        var io = new GpioGameIo(new GpioOptions(), bank);

        Assert.IsFalse(io.Read().IsPadHeld(new FloorPad(1)));
    }

    [TestMethod]
    public void Pad1_lamp_follows_game_output()
    {
        var bank = new FakeGpioBank();
        var io = new GpioGameIo(new GpioOptions(), bank);

        io.Apply(new GameIoOutput(
            padLampsOn: [new FloorPad(1)],
            easyLampOn: false,
            mediumLampOn: false,
            hardLampOn: false,
            pictorialLampsOn: GameIoOutput.Off.PictorialLampsOn,
            scoreDigits: null,
            ticketDigits: null,
            sound: null,
            ticketEnable: false));

        Assert.IsTrue(bank.ReadHigh(GpioOptions.DefaultPad1LampBcm));

        io.Apply(GameIoOutput.Off);
        Assert.IsFalse(bank.ReadHigh(GpioOptions.DefaultPad1LampBcm));
    }
}
