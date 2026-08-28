using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly GameOptions _gameOptions;

    public Worker(ILogger<Worker> logger, GameOptions gameOptions)
    {
        _logger = logger;
        _gameOptions = gameOptions;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "{Product} host started. Easy hit window {HitWindowMs} ms.",
            GameInfo.Name,
            _gameOptions.Easy.HitWindowMilliseconds);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }
}
