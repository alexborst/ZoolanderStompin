namespace ZoolanderStompin.Game;

public sealed class GpioOptions
{
    public const int MinBcm = 2;

    public const int MaxBcm = 27;

    /// <summary>BCM 17 = header pin 11.</summary>
    public const int DefaultPad1InputBcm = 17;

    /// <summary>BCM 22 = header pin 15.</summary>
    public const int DefaultPad2InputBcm = 22;

    /// <summary>BCM 23 = header pin 16.</summary>
    public const int DefaultPad3InputBcm = 23;

    /// <summary>BCM 24 = header pin 18.</summary>
    public const int DefaultPad4InputBcm = 24;

    /// <summary>BCM 5 = header pin 29.</summary>
    public const int DefaultPad5InputBcm = 5;

    /// <summary>BCM 6 = header pin 31.</summary>
    public const int DefaultPad6InputBcm = 6;

    /// <summary>BCM 12 = header pin 32.</summary>
    public const int DefaultPad7InputBcm = 12;

    /// <summary>BCM 16 = header pin 36.</summary>
    public const int DefaultEasyInputBcm = 16;

    /// <summary>BCM 20 = header pin 38.</summary>
    public const int DefaultMediumInputBcm = 20;

    /// <summary>BCM 21 = header pin 40.</summary>
    public const int DefaultHardInputBcm = 21;

    /// <summary>BCM 26 = header pin 37.</summary>
    public const int DefaultCreditInputBcm = 26;

    /// <summary>BCM 27 = header pin 13.</summary>
    public const int DefaultPad1LampBcm = 27;

    /// <summary>BCM 4 = header pin 7.</summary>
    public const int DefaultPad2LampBcm = 4;

    /// <summary>BCM 18 = header pin 12.</summary>
    public const int DefaultPad3LampBcm = 18;

    /// <summary>BCM 13 = header pin 33.</summary>
    public const int DefaultPad4LampBcm = 13;

    /// <summary>BCM 19 = header pin 35.</summary>
    public const int DefaultPad5LampBcm = 19;

    /// <summary>BCM 25 = header pin 22.</summary>
    public const int DefaultPad6LampBcm = 25;

    /// <summary>BCM 8 = header pin 24.</summary>
    public const int DefaultPad7LampBcm = 8;

    public int Pad1InputBcm { get; set; } = DefaultPad1InputBcm;

    public int Pad2InputBcm { get; set; } = DefaultPad2InputBcm;

    public int Pad3InputBcm { get; set; } = DefaultPad3InputBcm;

    public int Pad4InputBcm { get; set; } = DefaultPad4InputBcm;

    public int Pad5InputBcm { get; set; } = DefaultPad5InputBcm;

    public int Pad6InputBcm { get; set; } = DefaultPad6InputBcm;

    public int Pad7InputBcm { get; set; } = DefaultPad7InputBcm;

    public int EasyInputBcm { get; set; } = DefaultEasyInputBcm;

    public int MediumInputBcm { get; set; } = DefaultMediumInputBcm;

    public int HardInputBcm { get; set; } = DefaultHardInputBcm;

    public int CreditInputBcm { get; set; } = DefaultCreditInputBcm;

    public int Pad1LampBcm { get; set; } = DefaultPad1LampBcm;

    public int Pad2LampBcm { get; set; } = DefaultPad2LampBcm;

    public int Pad3LampBcm { get; set; } = DefaultPad3LampBcm;

    public int Pad4LampBcm { get; set; } = DefaultPad4LampBcm;

    public int Pad5LampBcm { get; set; } = DefaultPad5LampBcm;

    public int Pad6LampBcm { get; set; } = DefaultPad6LampBcm;

    public int Pad7LampBcm { get; set; } = DefaultPad7LampBcm;

    public int InputBcmForPad(int padNumber) => padNumber switch
    {
        1 => Pad1InputBcm,
        2 => Pad2InputBcm,
        3 => Pad3InputBcm,
        4 => Pad4InputBcm,
        5 => Pad5InputBcm,
        6 => Pad6InputBcm,
        7 => Pad7InputBcm,
        _ => throw new ArgumentOutOfRangeException(
            nameof(padNumber),
            padNumber,
            "Floor pad number must be between 1 and 7."),
    };

    public int LampBcmForPad(int padNumber) => padNumber switch
    {
        1 => Pad1LampBcm,
        2 => Pad2LampBcm,
        3 => Pad3LampBcm,
        4 => Pad4LampBcm,
        5 => Pad5LampBcm,
        6 => Pad6LampBcm,
        7 => Pad7LampBcm,
        _ => throw new ArgumentOutOfRangeException(
            nameof(padNumber),
            padNumber,
            "Floor pad number must be between 1 and 7."),
    };

    public IReadOnlyList<(string Name, int Bcm)> MappedPins() =>
    [
        ("Pad1InputBcm", Pad1InputBcm),
        ("Pad2InputBcm", Pad2InputBcm),
        ("Pad3InputBcm", Pad3InputBcm),
        ("Pad4InputBcm", Pad4InputBcm),
        ("Pad5InputBcm", Pad5InputBcm),
        ("Pad6InputBcm", Pad6InputBcm),
        ("Pad7InputBcm", Pad7InputBcm),
        ("EasyInputBcm", EasyInputBcm),
        ("MediumInputBcm", MediumInputBcm),
        ("HardInputBcm", HardInputBcm),
        ("CreditInputBcm", CreditInputBcm),
        ("Pad1LampBcm", Pad1LampBcm),
        ("Pad2LampBcm", Pad2LampBcm),
        ("Pad3LampBcm", Pad3LampBcm),
        ("Pad4LampBcm", Pad4LampBcm),
        ("Pad5LampBcm", Pad5LampBcm),
        ("Pad6LampBcm", Pad6LampBcm),
        ("Pad7LampBcm", Pad7LampBcm),
    ];
}
