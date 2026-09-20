namespace ZoolanderStompin.Game;

public sealed class GpioOptions
{
    public const int MinBcm = 2;

    public const int MaxBcm = 27;

    /// <summary>BCM 17 = header pin 11.</summary>
    public const int DefaultPad1InputBcm = 17;

    /// <summary>BCM 27 = header pin 13.</summary>
    public const int DefaultPad1LampBcm = 27;

    public int Pad1InputBcm { get; set; } = DefaultPad1InputBcm;

    public int Pad1LampBcm { get; set; } = DefaultPad1LampBcm;
}
