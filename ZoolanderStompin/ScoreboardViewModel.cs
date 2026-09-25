using System.ComponentModel;
using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public sealed class ScoreboardViewModel : INotifyPropertyChanged
{
    private readonly object _gate = new();
    private PlaySnapshot _latest = PlaySnapshot.Empty;
    private PlaySnapshot _shown = PlaySnapshot.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => _shown.Title;

    public string Phase => _shown.Phase;

    public string Outcome => _shown.Outcome;

    public bool IsWin => _shown.Outcome == "WIN";

    public bool IsLose => _shown.Outcome == "LOSE";

    public string Credits => _shown.Credits;

    public string Round => _shown.Round;

    public string Hits => _shown.Hits;

    public string Misses => _shown.Misses;

    public string Percent => _shown.Percent;

    public string Tickets => _shown.Tickets;

    public string Prompt => _shown.Prompt;

    public string Pads => _shown.Pads;

    public string Difficulty => _shown.Difficulty;

    public bool ShowDifficulty => _shown.Difficulty.Length > 0;

    public string Pictorial => _shown.Pictorial;

    public string KeyLegend => _shown.KeyLegend;

    public void Apply(PlaySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        lock (_gate)
        {
            _latest = snapshot;
        }
    }

    public void Flush()
    {
        PlaySnapshot latest;
        lock (_gate)
        {
            latest = _latest;
        }

        if (latest == _shown)
        {
            return;
        }

        _shown = latest;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
