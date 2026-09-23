using ZoolanderStompin;
using ZoolanderStompin.Game;

var builder = Host.CreateApplicationBuilder(args);

var gameOptions = builder.Configuration.GetSection(GameOptions.SectionName).Get<GameOptions>()
    ?? throw new GameConfigurationException($"Configuration section '{GameOptions.SectionName}' is missing.");
gameOptions.EnsureValid();
builder.Services.AddSingleton(gameOptions);
builder.Services.AddSingleton<KeyboardGameIo>();

var linux = OperatingSystem.IsLinux();
if (gameOptions.Io.UseGpio(linux))
{
    if (!linux)
    {
        throw new GameConfigurationException("GPIO I/O requires Linux (Raspberry Pi). Set Game:io:adapter to Keyboard or Auto on Windows.");
    }

    builder.Services.AddSingleton<IGpioBank, LinuxGpioBank>();
    builder.Services.AddSingleton(services =>
        new GpioGameIo(
            gameOptions.Io.Gpio,
            services.GetRequiredService<IGpioBank>(),
            readPadInputs: !gameOptions.Io.UseJoystick(linux)));
}

if (gameOptions.Io.UseJoystick(linux))
{
    builder.Services.AddSingleton<IJoystickDevice>(_ => new LinuxJoystickDevice(gameOptions.Io.Joystick.Device));
    builder.Services.AddSingleton(services =>
        new JoystickGameIo(gameOptions.Io.Joystick, services.GetRequiredService<IJoystickDevice>()));
}

builder.Services.AddSingleton(services => GameIoFactory.Create(services, gameOptions));
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
