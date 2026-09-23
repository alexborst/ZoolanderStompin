namespace ZoolanderStompin.Game;

public sealed class IoOptions
{
    public IoAdapter Adapter { get; set; } = IoAdapter.Auto;

    public GpioOptions Gpio { get; set; } = new();

    public JoystickOptions Joystick { get; set; } = new();

    public bool UseGpio(bool isLinux) => Adapter switch
    {
        IoAdapter.Gpio => true,
        IoAdapter.Keyboard => false,
        IoAdapter.Auto => isLinux,
        _ => false,
    };

    public bool UseJoystick(bool isLinux) => Joystick.Enabled && isLinux;
}
