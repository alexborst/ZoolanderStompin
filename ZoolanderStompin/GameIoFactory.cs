using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public static class GameIoFactory
{
    public static IGameIo Create(IServiceProvider services, GameOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        var linux = OperatingSystem.IsLinux();
        var parts = new List<IGameIo> { services.GetRequiredService<KeyboardGameIo>() };
        if (options.Io.UseGpio(linux))
        {
            parts.Add(services.GetRequiredService<GpioGameIo>());
        }

        if (options.Io.UseJoystick(linux))
        {
            parts.Add(services.GetRequiredService<JoystickGameIo>());
        }

        return parts.Count == 1 ? parts[0] : new CompositeGameIo([.. parts]);
    }
}
