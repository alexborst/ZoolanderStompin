namespace ZoolanderStompin.Game;

public sealed class JoystickOptions
{
    public bool Enabled { get; set; }

    public string Device { get; set; } = "/dev/input/js0";

    public int Pad1Button { get; set; }
}
