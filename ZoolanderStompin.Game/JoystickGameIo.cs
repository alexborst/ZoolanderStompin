namespace ZoolanderStompin.Game;

public sealed class JoystickGameIo : IGameIo
{
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
        var held = new List<FloorPad>();
        for (var number = 1; number <= FloorPad.Count; number++)
        {
            if (_device.IsButtonHeld(_options.ButtonForPad(number)))
            {
                held.Add(new FloorPad(number));
            }
        }

        return new GameIoInput(
            padsHeld: held,
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
