using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public sealed class ConsolePlayHud
{
    private string _last = "";
    private bool _cleared;

    public void Render(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        var text = PlayStatus.Format(session);
        if (text == _last)
        {
            return;
        }

        _last = text;
        try
        {
            if (!_cleared)
            {
                Console.Clear();
                _cleared = true;
            }

            DrawInPlace(text);
        }
        catch (IOException)
        {
            Console.WriteLine(text);
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine(text);
        }
    }

    private static void DrawInPlace(string text)
    {
        Console.CursorVisible = false;
        Console.SetCursorPosition(0, 0);
        var width = Math.Max(1, Console.WindowWidth - 1);
        foreach (var line in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            var padded = line.Length >= width ? line[..width] : line.PadRight(width);
            Console.WriteLine(padded);
        }
    }
}
