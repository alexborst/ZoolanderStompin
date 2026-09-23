using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public sealed class CompositePlayHud : IPlayHud
{
    private readonly IPlayHud[] _huds;

    public CompositePlayHud(params IPlayHud[] huds)
    {
        ArgumentNullException.ThrowIfNull(huds);
        if (huds.Length == 0)
        {
            throw new ArgumentException("At least one HUD is required.", nameof(huds));
        }

        foreach (var hud in huds)
        {
            ArgumentNullException.ThrowIfNull(hud);
        }

        _huds = huds;
    }

    public void Render(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        foreach (var hud in _huds)
        {
            hud.Render(session);
        }
    }
}
