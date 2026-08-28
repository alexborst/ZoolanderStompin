using ZoolanderStompin;
using ZoolanderStompin.Game;

var builder = Host.CreateApplicationBuilder(args);

var gameOptions = builder.Configuration.GetSection(GameOptions.SectionName).Get<GameOptions>()
    ?? throw new GameConfigurationException($"Configuration section '{GameOptions.SectionName}' is missing.");
gameOptions.EnsureValid();
builder.Services.AddSingleton(gameOptions);
builder.Services.AddSingleton<IGameIo, KeyboardGameIo>();
builder.Services.AddSingleton<IGameClock, SystemGameClock>();
builder.Services.AddSingleton<IPadPicker, RandomPadPicker>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
