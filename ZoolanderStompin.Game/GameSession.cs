namespace ZoolanderStompin.Game;

public sealed class GameSession
{
    private readonly GameOptions _options;
    private readonly IGameClock _clock;
    private readonly IPadPicker _picker;
    private readonly SoundLibrary? _sounds;
    private readonly ButtonEdges _buttons = new();
    private readonly List<GameSound> _cues = [];
    private TargetLoop? _loop;
    private TimedDeadline? _phaseDeadline;
    private TimedDeadline _attractSoundDeadline;
    private Score _score;
    private FloorPad? _previousPad;
    private FloorPad _attractPad = new(1);
    private int _coinsTowardCredit;
    private int _roundStartHits;
    private bool _resultsAwaitingGameEnd;

    public GameSession(GameOptions options, IGameClock clock, IPadPicker picker, SoundLibrary? sounds = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(picker);

        _options = options;
        _clock = clock;
        _picker = picker;
        _sounds = sounds;
        Phase = SessionPhase.Attract;
        _phaseDeadline = new TimedDeadline(clock, AttractCycle);
        _attractSoundDeadline = new TimedDeadline(clock, TimeSpan.Zero);
    }

    public SessionPhase Phase { get; private set; }

    public int Credits { get; private set; }

    public int CoinMeter { get; private set; }

    public Score Score => _loop?.Score ?? _score;

    public Difficulty? SelectedDifficulty { get; private set; }

    public Difficulty? FixedDifficulty => _options.FixedDifficulty;

    public int CurrentRound { get; private set; }

    public GameSound? Sound { get; private set; }

    public FloorPad? LitPad => Phase is SessionPhase.Playing ? _loop?.LitPad : Phase is SessionPhase.Attract ? _attractPad : null;

    public FloorPad? LastPresentedPad => _loop?.LastPresentedPad ?? _previousPad;

    public GameResult? Result { get; private set; }

    public IReadOnlyList<GameSound> DrainCues()
    {
        var cues = _cues.ToArray();
        _cues.Clear();
        return cues;
    }

    public void Tick(GameIoInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        _buttons.Observe(input);
        BankIncomingCredits();

        switch (Phase)
        {
            case SessionPhase.Attract:
                HandleAttract();
                break;
            case SessionPhase.Select:
                HandleSelect();
                break;
            case SessionPhase.Countdown:
                HandleCountdown(input);
                break;
            case SessionPhase.Playing:
                HandlePlaying(input);
                break;
            case SessionPhase.Intermission:
                HandleIntermission();
                break;
            case SessionPhase.Results:
                HandleResults();
                break;
        }
    }

    public GameIoOutput ToOutput()
    {
        return Phase switch
        {
            SessionPhase.Attract => BuildOutput(
                padLampsOn: [_attractPad],
                easyLampOn: _options.FreePlay,
                mediumLampOn: _options.FreePlay,
                hardLampOn: _options.FreePlay,
                scoreDigits: null,
                ticketDigits: null),
            SessionPhase.Select => BuildOutput(
                padLampsOn: [],
                easyLampOn: true,
                mediumLampOn: true,
                hardLampOn: true,
                scoreDigits: null,
                ticketDigits: null),
            SessionPhase.Countdown => BuildOutput(
                padLampsOn: [],
                easyLampOn: SelectedDifficulty is Difficulty.Easy,
                mediumLampOn: SelectedDifficulty is Difficulty.Medium,
                hardLampOn: SelectedDifficulty is Difficulty.Hard,
                scoreDigits: 0,
                ticketDigits: null),
            SessionPhase.Playing => _loop?.ToOutput() ?? GameIoOutput.Off,
            SessionPhase.Intermission => BuildOutput(
                padLampsOn: [],
                easyLampOn: SelectedDifficulty is Difficulty.Easy,
                mediumLampOn: SelectedDifficulty is Difficulty.Medium,
                hardLampOn: SelectedDifficulty is Difficulty.Hard,
                scoreDigits: Math.Min(99, Score.Hits),
                ticketDigits: null),
            SessionPhase.Results => BuildOutput(
                padLampsOn: [],
                easyLampOn: SelectedDifficulty is Difficulty.Easy,
                mediumLampOn: SelectedDifficulty is Difficulty.Medium,
                hardLampOn: SelectedDifficulty is Difficulty.Hard,
                scoreDigits: Math.Min(99, Score.Hits),
                ticketDigits: Math.Min(99, Result?.Tickets ?? 0),
                ticketEnable: (Result?.Tickets ?? 0) > 0),
            _ => GameIoOutput.Off,
        };
    }

    private TimeSpan AttractCycle => TimeSpan.FromMilliseconds(_options.AttractLampCycleMilliseconds);

    private TimeSpan AttractSound => TimeSpan.FromMilliseconds(_options.AttractSoundMilliseconds);

    private void CueAttractIfDue()
    {
        if (!_attractSoundDeadline.IsExpired)
        {
            return;
        }

        Cue(GameSound.Attract);
        _attractSoundDeadline = new TimedDeadline(_clock, AttractSound);
    }

    private TimeSpan SelectTimeout => TimeSpan.FromSeconds(_options.SelectTimeoutSeconds);

    private TimeSpan GameStartHold
    {
        get
        {
            var hold = ClipHold(GameSound.GameStart);
            return hold > TimeSpan.Zero ? hold : TimeSpan.FromTicks(1);
        }
    }

    private TimeSpan ClipHold(GameSound sound) =>
        _sounds?.HoldDuration(sound, TimeSpan.FromMilliseconds(_options.SoundHoldFallbackMilliseconds))
        ?? TimeSpan.Zero;

    private TimeSpan Intermission => TimeSpan.FromMilliseconds(_options.IntermissionMilliseconds);

    private TimeSpan ResultsHold => TimeSpan.FromMilliseconds(_options.ResultsMilliseconds);

    private TimeSpan GameEndDelay => TimeSpan.FromMilliseconds(_options.GameEndDelayMilliseconds);

    private void BankIncomingCredits()
    {
        if (_buttons.Credit)
        {
            AddCoinCredit();
        }

        if (_buttons.ServiceCredit)
        {
            AddServiceCredit();
        }
    }

    private void HandleAttract()
    {
        AdvanceAttractChaser();

        if (Credits > 0)
        {
            BeginPaidGame();
            return;
        }

        if (_options.FixedDifficulty is null && _options.FreePlay && _buttons.DifficultyPress is { } difficulty)
        {
            EnterCountdown(difficulty, consumeCredit: false);
            return;
        }

        CueAttractIfDue();
    }

    private void HandleSelect()
    {
        if (_buttons.Credit || _buttons.ServiceCredit)
        {
            ResetSelectTimeout();
        }

        if (_buttons.DifficultyPress is { } difficulty)
        {
            EnterCountdown(difficulty, consumeCredit: true);
            return;
        }

        if (_phaseDeadline is not { IsExpired: true })
        {
            return;
        }

        if (_options.SelectTimeoutAction is SelectTimeoutAction.AutoStartEasy)
        {
            EnterCountdown(Difficulty.Easy, consumeCredit: true);
            return;
        }

        Credits = Math.Max(0, Credits - 1);
        if (Credits > 0)
        {
            ResetSelectTimeout();
            return;
        }

        EnterAttract();
    }

    private void HandleCountdown(GameIoInput input)
    {
        _ = input;
        if (_phaseDeadline is { IsExpired: true })
        {
            EnterPlaying();
        }
    }

    private void HandlePlaying(GameIoInput input)
    {
        if (_loop is null)
        {
            EnterPlaying();
        }

        var generation = _loop!.SoundGeneration;
        _loop.Tick(input);
        if (_loop.SoundGeneration != generation && _loop.Sound is { } loopSound)
        {
            _cues.Add(loopSound);
            Sound = loopSound;
        }

        if (_loop.Phase != TargetLoopPhase.Complete)
        {
            return;
        }

        _score = _loop.Score;
        _previousPad = _loop.LastPresentedPad;
        var roundEnd = RoundRating.Sound(RoundRating.Rate(_score.Hits - _roundStartHits, _options));
        if (CurrentRound < _options.RoundCount)
        {
            EnterIntermission(roundEnd);
            return;
        }

        EnterResults(roundEnd);
    }

    private void HandleIntermission()
    {
        if (_phaseDeadline is { IsExpired: true })
        {
            CurrentRound++;
            EnterPlaying();
        }
    }

    private void HandleResults()
    {
        if (_phaseDeadline is not { IsExpired: true })
        {
            return;
        }

        if (_resultsAwaitingGameEnd)
        {
            _resultsAwaitingGameEnd = false;
            CueGameEnd();
            _phaseDeadline = new TimedDeadline(_clock, ResultsHold);
            return;
        }

        if (Credits > 0)
        {
            BeginPaidGame();
            return;
        }

        EnterAttract();
    }

    private void AdvanceAttractChaser()
    {
        if (_phaseDeadline is not { IsExpired: true })
        {
            return;
        }

        var next = _attractPad.Number == FloorPad.Count ? 1 : _attractPad.Number + 1;
        _attractPad = new FloorPad(next);
        _phaseDeadline = new TimedDeadline(_clock, AttractCycle);
    }

    private void AddCoinCredit()
    {
        CoinMeter++;
        _coinsTowardCredit++;
        Cue(GameSound.Coin);
        if (_coinsTowardCredit < _options.CoinsPerCredit)
        {
            return;
        }

        Credits++;
        _coinsTowardCredit = 0;
    }

    private void AddServiceCredit()
    {
        Credits++;
        Cue(GameSound.Coin);
    }

    private void BeginPaidGame()
    {
        if (_options.FixedDifficulty is { } locked)
        {
            EnterCountdown(locked, consumeCredit: true);
            return;
        }

        EnterSelect();
    }

    private void EnterSelect()
    {
        Phase = SessionPhase.Select;
        SelectedDifficulty = null;
        CurrentRound = 0;
        _score = default;
        Result = null;
        _loop = null;
        ResetSelectTimeout();
        if (Sound is not GameSound.Coin)
        {
            Cue(GameSound.Coin);
        }
    }

    private void ResetSelectTimeout() =>
        _phaseDeadline = new TimedDeadline(_clock, SelectTimeout);

    private void EnterCountdown(Difficulty difficulty, bool consumeCredit)
    {
        if (consumeCredit)
        {
            if (Credits < 1)
            {
                return;
            }

            Credits--;
        }

        Phase = SessionPhase.Countdown;
        SelectedDifficulty = difficulty;
        CurrentRound = 1;
        _score = new Score(0, 0);
        _previousPad = null;
        Result = null;
        _loop = null;
        _phaseDeadline = new TimedDeadline(_clock, GameStartHold);
        Cue(GameSound.GameStart);
    }

    private void EnterPlaying()
    {
        if (SelectedDifficulty is not { } difficulty)
        {
            throw new InvalidOperationException("Cannot start play without a locked difficulty.");
        }

        Phase = SessionPhase.Playing;
        _roundStartHits = _score.Hits;
        _loop = new TargetLoop(_options, difficulty, _clock, _picker, _score, _previousPad);
        _phaseDeadline = null;
        Cue(GameSound.Round);
    }

    private void EnterIntermission(GameSound roundEnd)
    {
        Phase = SessionPhase.Intermission;
        _loop = null;
        var hold = ClipHold(roundEnd);
        _phaseDeadline = new TimedDeadline(_clock, hold > Intermission ? hold : Intermission);
        Cue(roundEnd);
    }

    private void EnterResults(GameSound roundEnd)
    {
        Phase = SessionPhase.Results;
        Result = GameResult.Evaluate(_score, _options);
        _loop = null;
        Cue(roundEnd);
        var hold = ClipHold(roundEnd) + GameEndDelay;
        if (hold > TimeSpan.Zero)
        {
            _resultsAwaitingGameEnd = true;
            _phaseDeadline = new TimedDeadline(_clock, hold);
            return;
        }

        _resultsAwaitingGameEnd = false;
        CueGameEnd();
        _phaseDeadline = new TimedDeadline(_clock, ResultsHold);
    }

    private void CueGameEnd()
    {
        Cue(GameSound.GameEnd);
        if (Result is { Tickets: > 0 })
        {
            Cue(GameSound.Ticket);
        }
    }

    private void EnterAttract()
    {
        Phase = SessionPhase.Attract;
        SelectedDifficulty = null;
        CurrentRound = 0;
        _score = default;
        Result = null;
        _loop = null;
        _attractPad = new FloorPad(1);
        _phaseDeadline = new TimedDeadline(_clock, AttractCycle);
        _attractSoundDeadline = new TimedDeadline(_clock, TimeSpan.Zero);
        Sound = null;
    }

    private GameIoOutput BuildOutput(
        IEnumerable<FloorPad> padLampsOn,
        bool easyLampOn,
        bool mediumLampOn,
        bool hardLampOn,
        int? scoreDigits,
        int? ticketDigits,
        bool ticketEnable = false) =>
        new(
            padLampsOn: padLampsOn,
            easyLampOn: easyLampOn,
            mediumLampOn: mediumLampOn,
            hardLampOn: hardLampOn,
            pictorialLampsOn: scoreDigits is null
                ? GameIoOutput.Off.PictorialLampsOn
                : PictorialMeter.Lamps(Score.Hits, _options.SessionPresentations),
            scoreDigits: scoreDigits,
            ticketDigits: ticketDigits,
            sound: Sound,
            ticketEnable: ticketEnable);

    private void Cue(GameSound sound)
    {
        Sound = sound;
        _cues.Add(sound);
    }
}
