namespace ZoolanderStompin.Game;

public sealed class JoystickOptions
{
    public bool Enabled { get; set; }

    public string Device { get; set; } = "/dev/input/js0";

    public int Pad1Button { get; set; } = 1;

    public int Pad2Button { get; set; } = 2;

    public int Pad3Button { get; set; } = 3;

    public int Pad4Button { get; set; } = 4;

    public int Pad5Button { get; set; } = 5;

    public int Pad6Button { get; set; } = 6;

    public int Pad7Button { get; set; } = 7;

    public int ButtonForPad(int padNumber) => padNumber switch
    {
        1 => Pad1Button,
        2 => Pad2Button,
        3 => Pad3Button,
        4 => Pad4Button,
        5 => Pad5Button,
        6 => Pad6Button,
        7 => Pad7Button,
        _ => throw new ArgumentOutOfRangeException(
            nameof(padNumber),
            padNumber,
            "Floor pad number must be between 1 and 7."),
    };
}
