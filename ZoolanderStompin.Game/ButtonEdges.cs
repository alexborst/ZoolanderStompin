namespace ZoolanderStompin.Game;

public sealed class ButtonEdges
{
    private bool _easy;
    private bool _medium;
    private bool _hard;
    private bool _credit;
    private bool _serviceCredit;

    public bool Easy { get; private set; }

    public bool Medium { get; private set; }

    public bool Hard { get; private set; }

    public bool Credit { get; private set; }

    public bool ServiceCredit { get; private set; }

    public Difficulty? DifficultyPress =>
        Easy ? Difficulty.Easy : Medium ? Difficulty.Medium : Hard ? Difficulty.Hard : null;

    public void Observe(GameIoInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Easy = input.EasyHeld && !_easy;
        Medium = input.MediumHeld && !_medium;
        Hard = input.HardHeld && !_hard;
        Credit = input.CreditHeld && !_credit;
        ServiceCredit = input.ServiceCreditHeld && !_serviceCredit;

        _easy = input.EasyHeld;
        _medium = input.MediumHeld;
        _hard = input.HardHeld;
        _credit = input.CreditHeld;
        _serviceCredit = input.ServiceCreditHeld;
    }
}
