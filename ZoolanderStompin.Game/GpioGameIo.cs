namespace ZoolanderStompin.Game;

public sealed class GpioGameIo : IGameIo
{
    private readonly GpioOptions _pins;
    private readonly IGpioBank _bank;
    private readonly bool _readPadInputs;

    public GpioGameIo(GpioOptions pins, IGpioBank bank, bool readPadInputs = true)
    {
        ArgumentNullException.ThrowIfNull(pins);
        ArgumentNullException.ThrowIfNull(bank);

        _pins = pins;
        _bank = bank;
        _readPadInputs = readPadInputs;
        if (_readPadInputs)
        {
            for (var number = 1; number <= FloorPad.Count; number++)
            {
                _bank.OpenInputPullUp(_pins.InputBcmForPad(number));
            }

            _bank.OpenInputPullUp(_pins.EasyInputBcm);
            _bank.OpenInputPullUp(_pins.MediumInputBcm);
            _bank.OpenInputPullUp(_pins.HardInputBcm);
            _bank.OpenInputPullUp(_pins.CreditInputBcm);
        }

        _bank.OpenOutput(_pins.Pad1LampBcm, initialHigh: false);
    }

    public GameIoInput Read()
    {
        if (!_readPadInputs)
        {
            return GameIoInput.None;
        }

        var held = new List<FloorPad>();
        for (var number = 1; number <= FloorPad.Count; number++)
        {
            if (IsPressed(_pins.InputBcmForPad(number)))
            {
                held.Add(new FloorPad(number));
            }
        }

        return new GameIoInput(
            padsHeld: held,
            easyHeld: IsPressed(_pins.EasyInputBcm),
            mediumHeld: IsPressed(_pins.MediumInputBcm),
            hardHeld: IsPressed(_pins.HardInputBcm),
            creditHeld: IsPressed(_pins.CreditInputBcm),
            serviceCreditHeld: false,
            ticketNotchHeld: false);
    }

    public void Apply(GameIoOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _bank.WriteHigh(_pins.Pad1LampBcm, output.IsPadLampOn(new FloorPad(1)));
    }

    private bool IsPressed(int bcm) => !_bank.ReadHigh(bcm);
}
