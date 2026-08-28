using Microsoft.Extensions.Hosting;
using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public sealed class KeyboardGameIo : IGameIo
{
    private readonly KeyboardPadLatch _pads;
    private readonly IHostApplicationLifetime _lifetime;
    private bool _easyHeld;
    private bool _mediumHeld;
    private bool _hardHeld;
    private bool _creditPulse;
    private bool _serviceCreditPulse;
    private bool _ticketNotchPulse;

    public KeyboardGameIo(IGameClock clock, GameOptions gameOptions, IHostApplicationLifetime lifetime)
    {
        var holdMs = Math.Max(gameOptions.DebounceMilliseconds + 50, 80);
        _pads = new KeyboardPadLatch(clock, TimeSpan.FromMilliseconds(holdMs));
        _lifetime = lifetime;
    }

    public GameIoInput Read()
    {
        _easyHeld = false;
        _mediumHeld = false;
        _hardHeld = false;
        _creditPulse = false;
        _serviceCreditPulse = false;
        _ticketNotchPulse = false;

        while (Console.KeyAvailable)
        {
            var key = Console.ReadKey(intercept: true).Key;
            ApplyKey(key);
        }

        return new GameIoInput(
            padsHeld: _pads.Held,
            easyHeld: _easyHeld,
            mediumHeld: _mediumHeld,
            hardHeld: _hardHeld,
            creditHeld: _creditPulse,
            serviceCreditHeld: _serviceCreditPulse,
            ticketNotchHeld: _ticketNotchPulse);
    }

    public void Apply(GameIoOutput output)
    {
        _ = output;
    }

    private void ApplyKey(ConsoleKey key)
    {
        if (KeyboardBindings.IsQuit(key))
        {
            _lifetime.StopApplication();
            return;
        }

        var pad = KeyboardBindings.TryGetFloorPad(key);
        if (pad is { } floorPad)
        {
            _pads.Press(floorPad);
            return;
        }

        var difficulty = KeyboardBindings.TryGetDifficulty(key);
        if (difficulty is { } selected)
        {
            PulseDifficulty(selected);
            return;
        }

        if (KeyboardBindings.IsCredit(key))
        {
            _creditPulse = true;
            return;
        }

        if (KeyboardBindings.IsServiceCredit(key))
        {
            _serviceCreditPulse = true;
            return;
        }

        if (KeyboardBindings.IsTicketNotch(key))
        {
            _ticketNotchPulse = true;
        }
    }

    private void PulseDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                _easyHeld = true;
                break;
            case Difficulty.Medium:
                _mediumHeld = true;
                break;
            case Difficulty.Hard:
                _hardHeld = true;
                break;
        }
    }
}
