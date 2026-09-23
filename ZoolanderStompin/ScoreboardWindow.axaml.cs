using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public partial class ScoreboardWindow : Window
{
    private readonly ScoreboardViewModel _viewModel;
    private readonly KeyPressQueue _keys;
    private readonly DispatcherTimer _timer;

    public ScoreboardWindow()
        : this(new ScoreboardViewModel(), new KeyPressQueue())
    {
    }

    public ScoreboardWindow(ScoreboardViewModel viewModel, KeyPressQueue keys)
    {
        _viewModel = viewModel;
        _keys = keys;
        DataContext = viewModel;
        InitializeComponent();

        WindowState = OperatingSystem.IsWindows() ? WindowState.Maximized : WindowState.FullScreen;
        SystemDecorations = OperatingSystem.IsWindows() ? SystemDecorations.Full : SystemDecorations.None;
        Topmost = !OperatingSystem.IsWindows();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _timer.Tick += (_, _) => _viewModel.Flush();

        AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
        Opened += OnOpened;
        Closed += OnClosed;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        _viewModel.Flush();
        _timer.Start();
        Focus();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _timer.Stop();
        Opened -= OnOpened;
        Closed -= OnClosed;
    }

    private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
    {
        if (!AvaloniaKeyMap.TryMap(e.Key, out var consoleKey))
        {
            return;
        }

        _keys.Enqueue(consoleKey);
        e.Handled = true;
    }
}
