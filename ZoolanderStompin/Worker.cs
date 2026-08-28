using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public class Worker : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);

    private readonly IGameIo _gameIo;
    private readonly IGameClock _clock;
    private readonly GameSession _session;
    private readonly ConsolePlayHud _hud;

    public Worker(IGameIo gameIo, IGameClock clock, GameSession session, ConsolePlayHud hud)
    {
        _gameIo = gameIo;
        _clock = clock;
        _session = session;
        _hud = hud;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.Title = GameInfo.Name;
        }
        catch (IOException)
        {
        }

        _hud.Render(_session);
        _gameIo.Apply(_session.ToOutput());

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var input = _gameIo.Read();
                _session.Tick(input);
                _hud.Render(_session);
                _gameIo.Apply(_session.ToOutput());
                await _clock.Delay(PollInterval, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }
}
