using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public sealed class AvaloniaPlayHud : IPlayHud
{
    private readonly ScoreboardViewModel _viewModel;

    public AvaloniaPlayHud(ScoreboardViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public void Render(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _viewModel.Apply(PlaySnapshot.Capture(session));
    }
}
