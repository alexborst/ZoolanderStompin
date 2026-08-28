namespace ZoolanderStompin.Game;

public sealed class GameIoOutput
{
    public const int PictorialLampCount = 4;

    public static GameIoOutput Off { get; } = new(
        padLampsOn: [],
        easyLampOn: false,
        mediumLampOn: false,
        hardLampOn: false,
        pictorialLampsOn: [false, false, false, false],
        scoreDigits: null,
        ticketDigits: null,
        sound: null,
        ticketEnable: false);

    public GameIoOutput(
        IEnumerable<FloorPad> padLampsOn,
        bool easyLampOn,
        bool mediumLampOn,
        bool hardLampOn,
        IReadOnlyList<bool> pictorialLampsOn,
        int? scoreDigits,
        int? ticketDigits,
        GameSound? sound,
        bool ticketEnable)
    {
        if (pictorialLampsOn.Count != PictorialLampCount)
        {
            throw new ArgumentException(
                $"Pictorial lamps must have exactly {PictorialLampCount} entries.",
                nameof(pictorialLampsOn));
        }

        if (scoreDigits is < 0 or > 99)
        {
            throw new ArgumentOutOfRangeException(nameof(scoreDigits), scoreDigits, "Score digits must be 0-99 or null.");
        }

        if (ticketDigits is < 0 or > 99)
        {
            throw new ArgumentOutOfRangeException(nameof(ticketDigits), ticketDigits, "Ticket digits must be 0-99 or null.");
        }

        PadLampsOn = padLampsOn.ToHashSet();
        EasyLampOn = easyLampOn;
        MediumLampOn = mediumLampOn;
        HardLampOn = hardLampOn;
        PictorialLampsOn = pictorialLampsOn.ToArray();
        ScoreDigits = scoreDigits;
        TicketDigits = ticketDigits;
        Sound = sound;
        TicketEnable = ticketEnable;
    }

    public IReadOnlySet<FloorPad> PadLampsOn { get; }

    public bool EasyLampOn { get; }

    public bool MediumLampOn { get; }

    public bool HardLampOn { get; }

    public IReadOnlyList<bool> PictorialLampsOn { get; }

    public int? ScoreDigits { get; }

    public int? TicketDigits { get; }

    public GameSound? Sound { get; }

    public bool TicketEnable { get; }

    public bool IsPadLampOn(FloorPad pad) => PadLampsOn.Contains(pad);
}
