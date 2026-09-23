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

    [TestMethod]
    public void Pad_difficulty_and_credit_inputs_are_held_when_low()
    {
        var pins = new GpioOptions();
        var bank = new FakeGpioBank();
        var io = new GpioGameIo(pins, bank);
        bank.SetHigh(pins.Pad3InputBcm, false);
        bank.SetHigh(pins.Pad7InputBcm, false);
        bank.SetHigh(pins.EasyInputBcm, false);
        bank.SetHigh(pins.CreditInputBcm, false);

        var input = io.Read();

        Assert.IsFalse(input.IsPadHeld(new FloorPad(1)));
        Assert.IsTrue(input.IsPadHeld(new FloorPad(3)));
        Assert.IsTrue(input.IsPadHeld(new FloorPad(7)));
        Assert.IsTrue(input.EasyHeld);
        Assert.IsFalse(input.MediumHeld);
        Assert.IsFalse(input.HardHeld);
        Assert.IsTrue(input.CreditHeld);
        Assert.IsFalse(input.ServiceCreditHeld);
    }

    [TestMethod]
    public void Skips_switch_inputs_when_the_joystick_owns_stomps()
    {
        var bank = new FakeGpioBank();
        var io = new GpioGameIo(new GpioOptions(), bank, readPadInputs: false);
        bank.SetHigh(GpioOptions.DefaultPad1InputBcm, false);

        Assert.IsFalse(io.Read().IsPadHeld(new FloorPad(1)));

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
    }
}
