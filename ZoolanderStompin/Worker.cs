using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public class Worker : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);

    private readonly ILogger<Worker> _logger;
    private readonly IGameIo _gameIo;
    private readonly IGameClock _clock;
    private readonly GameSession _session;

    public Worker(ILogger<Worker> logger, IGameIo gameIo, IGameClock clock, GameSession session)
    {
        _logger = logger;
        _gameIo = gameIo;
        _clock = clock;
        _session = session;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "{Product} session ready. Keys: C credit, F service credit, E/M/H difficulty, 1-7 toggle pads (press once to stomp, again to lift).",
            GameInfo.Name);

        _gameIo.Apply(_session.ToOutput());

        var lastPhase = _session.Phase;
        var lastCredits = _session.Credits;
        var lastScore = _session.Score;

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var input = _gameIo.Read();
                _session.Tick(input);

                if (_session.Phase != lastPhase)
                {
                    _logger.LogInformation(
                        "Phase {Phase}. Round {Round}. Credits {Credits}.",
                        _session.Phase,
                        _session.CurrentRound,
                        _session.Credits);
                    lastPhase = _session.Phase;
                }

                if (_session.Credits != lastCredits)
                {
                    _logger.LogInformation(
                        "Credits {Credits}. Coin meter {CoinMeter}.",
                        _session.Credits,
                        _session.CoinMeter);
                    lastCredits = _session.Credits;
                }

                LogScoreChange(lastScore);
                lastScore = _session.Score;

                _gameIo.Apply(_session.ToOutput());
                await _clock.Delay(PollInterval, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private void LogScoreChange(Score scoreBefore)
    {
        if (_session.Score.ResolvedPresentations <= scoreBefore.ResolvedPresentations)
        {
            return;
        }

        if (_session.Score.Hits > scoreBefore.Hits)
        {
            _logger.LogInformation("Hit. {Hits} hits, {Misses} misses.", _session.Score.Hits, _session.Score.Misses);
        }
        else
        {
            _logger.LogInformation("Miss. {Hits} hits, {Misses} misses.", _session.Score.Hits, _session.Score.Misses);
        }
    }
}
