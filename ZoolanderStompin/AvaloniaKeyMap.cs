using Avalonia.Input;

namespace ZoolanderStompin;

public static class AvaloniaKeyMap
{
    public static bool TryMap(Key key, out ConsoleKey consoleKey)
    {
        consoleKey = key switch
        {
            Key.D1 => ConsoleKey.D1,
            Key.D2 => ConsoleKey.D2,
            Key.D3 => ConsoleKey.D3,
            Key.D4 => ConsoleKey.D4,
            Key.D5 => ConsoleKey.D5,
            Key.D6 => ConsoleKey.D6,
            Key.D7 => ConsoleKey.D7,
            Key.NumPad1 => ConsoleKey.NumPad1,
            Key.NumPad2 => ConsoleKey.NumPad2,
            Key.NumPad3 => ConsoleKey.NumPad3,
            Key.NumPad4 => ConsoleKey.NumPad4,
            Key.NumPad5 => ConsoleKey.NumPad5,
            Key.NumPad6 => ConsoleKey.NumPad6,
            Key.NumPad7 => ConsoleKey.NumPad7,
            Key.C => ConsoleKey.C,
            Key.E => ConsoleKey.E,
            Key.M => ConsoleKey.M,
            Key.H => ConsoleKey.H,
            Key.F => ConsoleKey.F,
            Key.N => ConsoleKey.N,
            Key.Enter => ConsoleKey.Enter,
            Key.Escape => ConsoleKey.Escape,
            _ => ConsoleKey.None,
        };

        return consoleKey is not ConsoleKey.None;
    }
}
