using ZoolanderStompin;
using ZoolanderStompin.Game;

var builder = Host.CreateApplicationBuilder(args);

var gameOptions = builder.Configuration.GetSection(GameOptions.SectionName).Get<GameOptions>()
    ?? throw new GameConfigurationException($"Configuration section '{GameOptions.SectionName}' is missing.");
gameOptions.EnsureValid();
builder.Services.AddSingleton(gameOptions);
builder.Services.AddSingleton<KeyboardGameIo>();
if (gameOptions.Io.UseGpio(OperatingSystem.IsLinux()))
{
    if (!OperatingSystem.IsLinux())
    {
        throw new GameConfigurationException("GPIO I/O requires Linux (Raspberry Pi). Set Game:io:adapter to Keyboard or Auto on Windows.");
    }

    builder.Services.AddSingleton<IGpioBank, LinuxGpioBank>();
    builder.Services.AddSingleton<GpioGameIo>(services =>
        new GpioGameIo(gameOptions.Io.Gpio, services.GetRequiredService<IGpioBank>()));
    builder.Services.AddSingleton<IGameIo>(services =>
        new CompositeGameIo(
            services.GetRequiredService<KeyboardGameIo>(),
            services.GetRequiredService<GpioGameIo>()));
}
else
{
    builder.Services.AddSingleton<IGameIo>(services => services.GetRequiredService<KeyboardGameIo>());
}
builder.Services.AddSingleton<IGameClock, SystemGameClock>();
builder.Services.AddSingleton<IPadPicker, RandomPadPicker>();
builder.Services.AddSingleton<GameSession>();
builder.Services.AddSingleton<ConsolePlayHud>();
#if LINUX_HOST
builder.Services.AddSingleton<IGameAudio, SilentGameAudio>();
#else
builder.Services.AddSingleton<IGameAudio>(services =>
    OperatingSystem.IsWindows()
        ? new WindowsGameAudio(services.GetRequiredService<IHostEnvironment>())
        : new SilentGameAudio());
#endif
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
