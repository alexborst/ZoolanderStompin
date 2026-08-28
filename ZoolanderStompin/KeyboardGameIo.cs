using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public sealed class KeyboardGameIo : IGameIo
{
    private readonly ILogger<KeyboardGameIo> _logger;
    private readonly HashSet<FloorPad> _padsHeld = [];
    private bool _easyHeld;
    private bool _mediumHeld;
    private bool _hardHeld;
    private bool _creditPulse;
    private bool _serviceCreditPulse;
    private bool _ticketNotchPulse;
    private string _lastRendered = "";

    public KeyboardGameIo(ILogger<KeyboardGameIo> logger)
    {
        _logger = logger;
    }

    public GameIoInput Read()
    {
        _creditPulse = false;
        _serviceCreditPulse = false;
        _ticketNotchPulse = false;

        while (Console.KeyAvailable)
        {
            var key = Console.ReadKey(intercept: true).Key;
            ApplyKey(key);
        }

        return new GameIoInput(
            padsHeld: _padsHeld,
            easyHeld: _easyHeld,
            mediumHeld: _mediumHeld,
            hardHeld: _hardHeld,
            creditHeld: _creditPulse,
            serviceCreditHeld: _serviceCreditPulse,
            ticketNotchHeld: _ticketNotchPulse);
    }

    public void Apply(GameIoOutput output)
    {
        var line = FormatStatus(output);
        if (line == _lastRendered)
        {
            return;
        }

        _lastRendered = line;
        Console.WriteLine(line);
    }

    private void ApplyKey(ConsoleKey key)
    {
        var pad = KeyboardBindings.TryGetFloorPad(key);
        if (pad is { } floorPad)
        {
            if (!_padsHeld.Add(floorPad))
            {
                _padsHeld.Remove(floorPad);
            }

            _logger.LogInformation("Pad {Pad} {State}.", floorPad.Number, _padsHeld.Contains(floorPad) ? "held" : "released");
            return;
        }

        var difficulty = KeyboardBindings.TryGetDifficulty(key);
        if (difficulty is { } selected)
        {
            ToggleDifficulty(selected);
            return;
        }

        if (KeyboardBindings.IsCredit(key))
        {
            _creditPulse = true;
            _logger.LogInformation("Credit.");
            return;
        }

        if (KeyboardBindings.IsServiceCredit(key))
        {
            _serviceCreditPulse = true;
            _logger.LogInformation("Service credit.");
            return;
        }

        if (KeyboardBindings.IsTicketNotch(key))
        {
            _ticketNotchPulse = true;
            _logger.LogInformation("Ticket notch.");
        }
    }

    private void ToggleDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                _easyHeld = !_easyHeld;
                _logger.LogInformation("Easy {State}.", _easyHeld ? "held" : "released");
                break;
            case Difficulty.Medium:
                _mediumHeld = !_mediumHeld;
                _logger.LogInformation("Medium {State}.", _mediumHeld ? "held" : "released");
                break;
            case Difficulty.Hard:
                _hardHeld = !_hardHeld;
                _logger.LogInformation("Hard {State}.", _hardHeld ? "held" : "released");
                break;
        }
    }

    private string FormatStatus(GameIoOutput output)
    {
        var pads = string.Join(
            " ",
            Enumerable.Range(1, FloorPad.Count).Select(n =>
            {
                var pad = new FloorPad(n);
                return output.IsPadLampOn(pad) ? $"[{n}]" : $" {n} ";
            }));

        var difficulty =
            $"{Lamp("Easy", output.EasyLampOn)} {Lamp("Medium", output.MediumLampOn)} {Lamp("Hard", output.HardLampOn)}";
        var pictorial = string.Join(" ", output.PictorialLampsOn.Select(on => on ? "*" : "."));
        var score = output.ScoreDigits?.ToString("00") ?? "--";
        var tickets = output.TicketDigits?.ToString("00") ?? "--";
        var sound = output.Sound?.ToString() ?? "-";

        return $"pads {pads} | {difficulty} | pics {pictorial} | score {score} tickets {tickets} | sound {sound}";
    }

    private static string Lamp(string name, bool on) => on ? $"[{name}]" : name;
}
