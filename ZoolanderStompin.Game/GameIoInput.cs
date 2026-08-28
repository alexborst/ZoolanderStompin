namespace ZoolanderStompin.Game;

public sealed class GameIoInput
{
    public static GameIoInput None { get; } = new(
        padsHeld: [],
        easyHeld: false,
        mediumHeld: false,
        hardHeld: false,
        creditHeld: false,
        serviceCreditHeld: false,
        ticketNotchHeld: false);

    public GameIoInput(
        IEnumerable<FloorPad> padsHeld,
        bool easyHeld,
        bool mediumHeld,
        bool hardHeld,
        bool creditHeld,
        bool serviceCreditHeld,
        bool ticketNotchHeld)
    {
        PadsHeld = padsHeld.ToHashSet();
        EasyHeld = easyHeld;
        MediumHeld = mediumHeld;
        HardHeld = hardHeld;
        CreditHeld = creditHeld;
        ServiceCreditHeld = serviceCreditHeld;
        TicketNotchHeld = ticketNotchHeld;
    }

    public IReadOnlySet<FloorPad> PadsHeld { get; }

    public bool EasyHeld { get; }

    public bool MediumHeld { get; }

    public bool HardHeld { get; }

    public bool CreditHeld { get; }

    public bool ServiceCreditHeld { get; }

    public bool TicketNotchHeld { get; }

    public bool IsPadHeld(FloorPad pad) => PadsHeld.Contains(pad);
}
