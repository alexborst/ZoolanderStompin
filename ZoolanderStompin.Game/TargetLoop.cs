namespace ZoolanderStompin.Game;

public sealed class TargetLoop
{
    private readonly GameOptions _options;
    private readonly DifficultyOptions _difficultyOptions;
    private readonly IGameClock _clock;
    private readonly IPadPicker _picker;
    private readonly PadDebouncer _debouncer;
    private readonly HashSet<FloorPad> _blockedUntilRelease = [];
    private TimedDeadline? _window;
    private TimedDeadline? _gap;
    private FloorPad? _previousPad;
    private bool _started;
    private int _resolvedThisRound;

    public TargetLoop(
        GameOptions options,
        Difficulty difficulty,
        IGameClock clock,
        IPadPicker picker,
        Score startingScore = default,
        FloorPad? previousPad = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(picker);
        if (!Enum.IsDefined(difficulty))
        {
            throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, "Difficulty is not a recognized value.");
        }

        _options = options;
        Difficulty = difficulty;
        _difficultyOptions = options.For(difficulty);
        _clock = clock;
        _picker = picker;
        _debouncer = new PadDebouncer(clock, TimeSpan.FromMilliseconds(options.DebounceMilliseconds));
        Score = startingScore;
        _previousPad = previousPad;
        LastPresentedPad = previousPad;
        Phase = TargetLoopPhase.Gap;
    }

    public Difficulty Difficulty { get; }

    public TargetLoopPhase Phase { get; private set; }

    public Score Score { get; private set; }

    public FloorPad? LitPad { get; private set; }

    public FloorPad? LastPresentedPad { get; private set; }

    public GameSound? Sound { get; private set; }

    public void Tick(GameIoInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        _debouncer.Observe(input.PadsHeld);
        ReleaseBlockedPadsThatAreUp();

        if (!_started)
        {
            _started = true;
            StartPresentation();
            return;
        }

        switch (Phase)
        {
            case TargetLoopPhase.Presenting:
                ProcessPresenting();
                break;
            case TargetLoopPhase.Gap:
                if (_gap is { IsExpired: true })
                {
                    StartPresentation();
                }

                break;
        }
    }

    public GameIoOutput ToOutput()
    {
        IEnumerable<FloorPad> lamps = LitPad is { } pad ? [pad] : [];
        var hits = Math.Min(99, Score.Hits);
        return new GameIoOutput(
            padLampsOn: lamps,
            easyLampOn: Difficulty is Difficulty.Easy,
            mediumLampOn: Difficulty is Difficulty.Medium,
            hardLampOn: Difficulty is Difficulty.Hard,
            pictorialLampsOn: PictorialMeter.Lamps(Score.Hits, _options.SessionPresentations),
            scoreDigits: hits,
            ticketDigits: null,
            sound: Sound,
            ticketEnable: false);
    }

    private void ProcessPresenting()
    {
        if (LitPad is not { } lit)
        {
            return;
        }

        if (_debouncer.RisingEdges.Contains(lit) && !_blockedUntilRelease.Contains(lit))
        {
            ResolveHit();
            return;
        }

        if (_window is { IsExpired: true })
        {
            ResolveMiss();
        }
    }

    private void StartPresentation()
    {
        if (_resolvedThisRound >= _options.PresentationsPerRound)
        {
            CompleteRound();
            return;
        }

        var pick = _picker.Next(BuildCandidates());
        if (!_difficultyOptions.Pads.Contains(pick))
        {
            throw new InvalidOperationException(
                $"Pad picker returned pad {pick.Number}, which is not in the {Difficulty} set.");
        }

        LitPad = pick;
        LastPresentedPad = pick;
        _previousPad = pick;
        _window = new TimedDeadline(_clock, TimeSpan.FromMilliseconds(_difficultyOptions.HitWindowMilliseconds));
        _gap = null;
        Phase = TargetLoopPhase.Presenting;
        Sound = GameSound.NewLight;
        BlockPadsCurrentlyDown();
    }

    private IReadOnlyList<FloorPad> BuildCandidates()
    {
        var pads = _difficultyOptions.Pads;
        if (!_options.PreventConsecutiveRepeat || _previousPad is not { } previous || pads.Count < 2)
        {
            return pads;
        }

        return pads.Where(pad => pad != previous).ToArray();
    }

    private void BlockPadsCurrentlyDown()
    {
        _blockedUntilRelease.Clear();
        foreach (var pad in _debouncer.RawHeld)
        {
            _blockedUntilRelease.Add(pad);
        }

        foreach (var pad in _debouncer.StableHeld)
        {
            _blockedUntilRelease.Add(pad);
        }
    }

    private void ReleaseBlockedPadsThatAreUp()
    {
        _blockedUntilRelease.RemoveWhere(pad =>
            !_debouncer.RawHeld.Contains(pad) && !_debouncer.StableHeld.Contains(pad));
    }

    private void ResolveHit()
    {
        Score = Score.WithHit();
        _resolvedThisRound++;
        Sound = GameSound.Hit;
        EndPresentation();
    }

    private void ResolveMiss()
    {
        Score = Score.WithMiss();
        _resolvedThisRound++;
        Sound = GameSound.Miss;
        EndPresentation();
    }

    private void EndPresentation()
    {
        LitPad = null;
        _window = null;
        if (_resolvedThisRound >= _options.PresentationsPerRound)
        {
            CompleteRound();
            return;
        }

        Phase = TargetLoopPhase.Gap;
        _gap = new TimedDeadline(_clock, TimeSpan.FromMilliseconds(_options.InterTargetGapMilliseconds));
    }

    private void CompleteRound()
    {
        LitPad = null;
        _window = null;
        _gap = null;
        Phase = TargetLoopPhase.Complete;
    }
}
