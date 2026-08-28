using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly GameOptions _gameOptions;
    private readonly IGameIo _gameIo;
    private readonly IGameClock _clock;

    public Worker(ILogger<Worker> logger, GameOptions gameOptions, IGameIo gameIo, IGameClock clock)
    {
        _logger = logger;
        _gameOptions = gameOptions;
        _gameIo = gameIo;
        _clock = clock;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "{Product} host started. Easy hit window {HitWindowMs} ms. Keys: 1-7 pads, E/M/H difficulty, C credit, F service, N ticket notch.",
            GameInfo.Name,
            _gameOptions.Easy.HitWindowMilliseconds);

        _gameIo.Apply(GameIoOutput.Off);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var input = _gameIo.Read();
                _gameIo.Apply(ToProbeOutput(input));
                await _clock.Delay(TimeSpan.FromMilliseconds(50), stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private static GameIoOutput ToProbeOutput(GameIoInput input)
    {
        return new GameIoOutput(
            padLampsOn: input.PadsHeld,
            easyLampOn: input.EasyHeld,
            mediumLampOn: input.MediumHeld,
            hardLampOn: input.HardHeld,
            pictorialLampsOn: GameIoOutput.Off.PictorialLampsOn,
            scoreDigits: null,
            ticketDigits: null,
            sound: input.CreditHeld ? GameSound.Coin : null,
            ticketEnable: false);
    }
}
