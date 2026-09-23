namespace ZoolanderStompin.Game;

public sealed class JoystickGameIo : IGameIo
{
    private static readonly FloorPad Pad1 = new(1);

    private readonly JoystickOptions _options;
    private readonly IJoystickDevice _device;

    public JoystickGameIo(JoystickOptions options, IJoystickDevice device)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(device);

        _options = options;
        _device = device;
    }

    public GameIoInput Read()
    {
        _device.Pump();
        var pressed = _device.IsButtonHeld(_options.Pad1Button);
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
    }
}
