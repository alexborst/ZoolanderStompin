namespace ZoolanderStompin.Game;

public sealed class PayoutBand
{
    public int MinPercentInclusive { get; set; }

    public int MaxPercentInclusive { get; set; }

    public int Tickets { get; set; }
}
