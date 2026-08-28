using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public class Worker : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);
    private static readonly TimeSpan RoundRestartDelay = TimeSpan.FromSeconds(2);

    private readonly ILogger<Worker> _logger;
    private readonly GameOptions _gameOptions;
    private readonly IGameIo _gameIo;
    private readonly IGameClock _clock;
    private readonly IPadPicker _picker;
    private TargetLoop _loop;
    private TimedDeadline? _restartAt;

    public Worker(
        ILogger<Worker> logger,
        GameOptions gameOptions,
        IGameIo gameIo,
        IGameClock clock,
        IPadPicker picker)
    {
        _logger = logger;
        _gameOptions = gameOptions;
        _gameIo = gameIo;
        _clock = clock;
        _picker = picker;
        _loop = CreateLoop(Difficulty.Easy);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "{Product} target loop ready. Keys: 1-7 toggle pads (press once to stomp, again to lift). E/M/H starts a new round. Easy uses pads 1-4.",
            GameInfo.Name);
        StartRound(Difficulty.Easy);

        _gameIo.Apply(_loop.ToOutput());

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var input = _gameIo.Read();
                ApplyDifficultyOrRestart(input);

                var scoreBefore = _loop.Score;
                var phaseBefore = _loop.Phase;
                _loop.Tick(input);
                LogScoreChange(scoreBefore);
                LogRoundComplete(phaseBefore);

                _gameIo.Apply(_loop.ToOutput());
                await _clock.Delay(PollInterval, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private void ApplyDifficultyOrRestart(GameIoInput input)
    {
        if (input.EasyHeld)
        {
            StartRound(Difficulty.Easy);
            return;
        }

        if (input.MediumHeld)
        {
            StartRound(Difficulty.Medium);
            return;
        }

        if (input.HardHeld)
        {
            StartRound(Difficulty.Hard);
            return;
        }

        if (_loop.Phase == TargetLoopPhase.Complete && _restartAt is { IsExpired: true })
        {
            StartRound(_loop.Difficulty);
        }
    }

    private void StartRound(Difficulty difficulty)
    {
        _loop = CreateLoop(difficulty);
        _restartAt = null;
        _logger.LogInformation(
            "Starting {Difficulty} round. {PadCount} pads in play, {WindowMs} ms hit window, {Presentations} presentations.",
            difficulty,
            _gameOptions.For(difficulty).Pads.Count,
            _gameOptions.For(difficulty).HitWindowMilliseconds,
            _gameOptions.PresentationsPerRound);
    }

    private TargetLoop CreateLoop(Difficulty difficulty) =>
        new(_gameOptions, difficulty, _clock, _picker);

    private void LogScoreChange(Score scoreBefore)
    {
        if (_loop.Score.ResolvedPresentations <= scoreBefore.ResolvedPresentations)
        {
            return;
        }

        if (_loop.Score.Hits > scoreBefore.Hits)
        {
            _logger.LogInformation("Hit. {Hits} hits, {Misses} misses.", _loop.Score.Hits, _loop.Score.Misses);
        }
        else
        {
            _logger.LogInformation("Miss. {Hits} hits, {Misses} misses.", _loop.Score.Hits, _loop.Score.Misses);
        }
    }

    private void LogRoundComplete(TargetLoopPhase phaseBefore)
    {
        if (_loop.Phase != TargetLoopPhase.Complete || phaseBefore == TargetLoopPhase.Complete)
        {
            return;
        }

        _restartAt = new TimedDeadline(_clock, RoundRestartDelay);
        _logger.LogInformation(
            "Round finished. {Hits} hits, {Misses} misses ({Percent:0}%). Next round in {Seconds}s, or press E/M/H.",
            _loop.Score.Hits,
            _loop.Score.Misses,
            _loop.Score.HitPercent ?? 0,
            RoundRestartDelay.TotalSeconds);
    }
}
