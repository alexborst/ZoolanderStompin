namespace ZoolanderStompin.Game;

public sealed class PayoutOptions
{
    public PayoutMode Mode { get; set; } = PayoutMode.PercentageTable;

    public int FixedTickets { get; set; }

    public List<PayoutBand> Table { get; set; } = [];

    public int TicketsForHitPercent(double hitPercent)
    {
        if (hitPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(hitPercent), hitPercent, "Hit percent must be between 0 and 100.");
        }

        if (Mode == PayoutMode.Fixed)
        {
            return FixedTickets;
        }

        var rate = (int)Math.Floor(hitPercent);

        var band = Table.FirstOrDefault(b => rate >= b.MinPercentInclusive && rate <= b.MaxPercentInclusive)
            ?? throw new GameConfigurationException($"No payout band covers {rate}%.");

        return band.Tickets;
    }
}
