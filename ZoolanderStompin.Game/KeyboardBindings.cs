namespace ZoolanderStompin.Game;

public static class KeyboardBindings
{
    public static FloorPad? TryGetFloorPad(ConsoleKey key)
    {
        var number = key switch
        {
            ConsoleKey.D1 or ConsoleKey.NumPad1 => 1,
            ConsoleKey.D2 or ConsoleKey.NumPad2 => 2,
            ConsoleKey.D3 or ConsoleKey.NumPad3 => 3,
            ConsoleKey.D4 or ConsoleKey.NumPad4 => 4,
            ConsoleKey.D5 or ConsoleKey.NumPad5 => 5,
            ConsoleKey.D6 or ConsoleKey.NumPad6 => 6,
            ConsoleKey.D7 or ConsoleKey.NumPad7 => 7,
            _ => 0,
        };

        return number == 0 ? null : new FloorPad(number);
    }

    public static Difficulty? TryGetDifficulty(ConsoleKey key) => key switch
    {
        ConsoleKey.E => Difficulty.Easy,
        ConsoleKey.M => Difficulty.Medium,
        ConsoleKey.H => Difficulty.Hard,
        _ => null,
    };

    public static bool IsCredit(ConsoleKey key) => key is ConsoleKey.C or ConsoleKey.Enter;

    public static bool IsServiceCredit(ConsoleKey key) => key is ConsoleKey.F;

    public static bool IsTicketNotch(ConsoleKey key) => key is ConsoleKey.N;
}
