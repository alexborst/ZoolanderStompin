namespace ZoolanderStompin.Game;

public sealed class GpioGameIo : IGameIo
{
    private static readonly FloorPad Pad1 = new(1);

    private readonly GpioOptions _pins;
    private readonly IGpioBank _bank;

    public GpioGameIo(GpioOptions pins, IGpioBank bank)
    {
        ArgumentNullException.ThrowIfNull(pins);
        ArgumentNullException.ThrowIfNull(bank);

        _pins = pins;
        _bank = bank;
        _bank.OpenInputPullUp(_pins.Pad1InputBcm);
        _bank.OpenOutput(_pins.Pad1LampBcm, initialHigh: false);
    }

    public GameIoInput Read()
    {
        var pressed = !_bank.ReadHigh(_pins.Pad1InputBcm);
        return new GameIoInput(
            padsHeld: pressed ? [Pad1] : [],
            easyHeld: false,
            mediumHeld: false,
            hardHeld: false,
            creditHeld: false,
            serviceCreditHeld: false,
            ticketNotchHeld: false);
    }

    public void Apply(GameIoOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _bank.WriteHigh(_pins.Pad1LampBcm, output.IsPadLampOn(Pad1));
    }
}
