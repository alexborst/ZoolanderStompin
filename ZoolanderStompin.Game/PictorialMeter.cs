namespace ZoolanderStompin.Game;

public static class PictorialMeter
{
    public static IReadOnlyList<bool> Lamps(int hits, int sessionPresentations)
    {
        var lamps = new bool[GameIoOutput.PictorialLampCount];
        if (hits <= 0 || sessionPresentations <= 0)
        {
            return lamps;
        }

        var lampsOn = Math.Min(
            GameIoOutput.PictorialLampCount,
            (hits * GameIoOutput.PictorialLampCount + sessionPresentations - 1) / sessionPresentations);

        for (var i = 0; i < lampsOn; i++)
        {
            lamps[i] = true;
        }

        return lamps;
    }
}
