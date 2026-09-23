using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ZoolanderStompin;

public partial class App : Application
{
    public IHost Host { get; set; } = default!;

    public IServiceProvider Services { get; set; } = default!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var lifetime = Services.GetRequiredService<IHostApplicationLifetime>();
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            desktop.MainWindow = Services.GetRequiredService<ScoreboardWindow>();
            desktop.Startup += (_, _) => _ = Host.StartAsync();
            desktop.Exit += (_, _) => lifetime.StopApplication();
            lifetime.ApplicationStopping.Register(() =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime running)
                    {
                        running.Shutdown();
                    }
                });
            });
        }

        base.OnFrameworkInitializationCompleted();
    }
}
