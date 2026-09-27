using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

public sealed class GameSessionDriver
{
    public GameSessionDriver(IPadPicker picker, GameOptions? options = null, SoundLibrary? sounds = null)
    {
        Options = options ?? CreateShortSession();
        Clock = new FakeGameClock();
        Picker = picker;
        Session = new GameSession(Options, Clock, picker, sounds);
        Input = GameIoInput.None;
    }

    public static GameSessionDriver Scripted(GameOptions? options = null, params int[] pads) =>
        new(new ScriptedPadPicker(pads), options);

    public GameOptions Options { get; }

    public FakeGameClock Clock { get; }

    public IPadPicker Picker { get; }

    public GameSession Session { get; }

    public GameIoInput Input { get; set; }

    public GameIoOutput Output => Session.ToOutput();

    public TimeSpan Debounce => TimeSpan.FromMilliseconds(Options.DebounceMilliseconds);

    public TimeSpan Window => TimeSpan.FromMilliseconds(Options.Easy.HitWindowMilliseconds);

    public TimeSpan Gap => TimeSpan.FromMilliseconds(Options.InterTargetGapMilliseconds);

    public TimeSpan GetReady => TimeSpan.FromMilliseconds(Options.CountdownGetReadyMilliseconds);

    public TimeSpan Go => TimeSpan.FromMilliseconds(Options.CountdownGoMilliseconds);

    public TimeSpan Intermission => TimeSpan.FromMilliseconds(Options.IntermissionMilliseconds);

    public TimeSpan ResultsHold => TimeSpan.FromMilliseconds(Options.ResultsMilliseconds);

    public TimeSpan GameEndDelay => TimeSpan.FromMilliseconds(Options.GameEndDelayMilliseconds);

    public TimeSpan AttractCycle => TimeSpan.FromMilliseconds(Options.AttractLampCycleMilliseconds);

    public TimeSpan SelectTimeout => TimeSpan.FromSeconds(Options.SelectTimeoutSeconds);

    public void Tick() => Session.Tick(Input);

    public void AdvanceAndTick(TimeSpan duration)
    {
        Clock.Advance(duration);
        Tick();
    }

    public void PulseCredit() => Pulse(credit: true);

    public void PulseServiceCredit() => Pulse(serviceCredit: true);

    public void PulseDifficulty(Difficulty difficulty) =>
        Pulse(
            easy: difficulty is Difficulty.Easy,
            medium: difficulty is Difficulty.Medium,
            hard: difficulty is Difficulty.Hard);

    public void SetPads(params int[] numbers)
    {
        Input = BuildInput(pads: numbers);
    }

    public void Stomp(params int[] pads)
    {
        SetPads(pads);
        StabilizeHeldPads();
    }

    public void StabilizeHeldPads()
    {
        Tick();
        AdvanceAndTick(Debounce);
    }

    public void Release()
    {
        SetPads();
        Tick();
        AdvanceAndTick(Debounce);
    }

    public void SkipCountdown()
    {
        SkipToPlayingUnlit();
        Tick();
    }

    public void SkipToPlayingUnlit()
    {
        if (Session.Phase == SessionPhase.Playing)
        {
            return;
        }

        if (Session.Phase != SessionPhase.Countdown)
        {
            throw new InvalidOperationException($"Expected Countdown, was {Session.Phase}.");
        }

        var steps = 0;
        while (Session.Phase == SessionPhase.Countdown && steps++ < 8)
        {
            AdvanceAndTick(TimeSpan.FromMinutes(1));
        }

        if (Session.Phase == SessionPhase.Countdown)
        {
            throw new InvalidOperationException("Countdown did not finish.");
        }
    }

    /// <summary>
    /// Sessions without a sound library hold results only for the game-end delay before cueing GameEnd.
    /// </summary>
    public void AwaitGameEnd()
    {
        if (Session.Phase != SessionPhase.Results)
        {
            throw new InvalidOperationException($"Expected Results, was {Session.Phase}.");
        }

        AdvanceAndTick(GameEndDelay);
    }

    public void FinishResults()
    {
        AwaitGameEnd();
        AdvanceAndTick(ResultsHold);
    }

    public void MissCurrent()
    {
        if (Session.Phase != SessionPhase.Playing)
        {
            throw new InvalidOperationException($"Expected Playing, was {Session.Phase}.");
        }

        AdvanceAndTick(Window);
    }

    public void FinishGap()
    {
        if (Session.Phase != SessionPhase.Playing)
        {
            throw new InvalidOperationException($"Expected Playing, was {Session.Phase}.");
        }

        AdvanceAndTick(Gap);
    }

    public void HitCurrent()
    {
        if (Session.LitPad is not { } pad)
        {
            throw new InvalidOperationException("No lit pad to stomp.");
        }

        Stomp(pad.Number);
    }

    public void PlayUntilResults(bool hitEveryPresentation)
    {
        while (Session.Phase is not SessionPhase.Results)
        {
            switch (Session.Phase)
            {
                case SessionPhase.Countdown:
                    SkipCountdown();
                    break;
                case SessionPhase.Intermission:
                    AdvanceAndTick(Intermission);
                    Tick();
                    break;
                case SessionPhase.Playing:
                    if (hitEveryPresentation)
                    {
                        HitCurrent();
                    }
                    else
                    {
                        MissCurrent();
                    }

                    if (Session.Phase == SessionPhase.Playing)
                    {
                        FinishGap();
                    }

                    break;
                default:
                    Tick();
                    break;
            }
        }
    }

    public static GameOptions CreateShortSession()
    {
        var options = GameOptions.CreateDefault();
        options.PresentationsPerRound = 2;
        options.RoundEndMediumMinimumHits = 1;
        options.RoundEndGoodMinimumHits = 2;
        options.RoundCount = 2;
        options.DebounceMilliseconds = 30;
        options.InterTargetGapMilliseconds = 50;
        options.SelectTimeoutSeconds = 2;
        options.CountdownGetReadyMilliseconds = 80;
        options.CountdownGoMilliseconds = 40;
        options.IntermissionMilliseconds = 60;
        options.ResultsMilliseconds = 70;
        options.GameEndDelayMilliseconds = 60;
        options.AttractLampCycleMilliseconds = 40;
        options.AttractSoundMilliseconds = 80;
        options.Easy.HitWindowMilliseconds = 100;
        options.Medium.HitWindowMilliseconds = 100;
        options.Hard.HitWindowMilliseconds = 100;
        options.FixedDifficulty = null;
        return options;
    }

    private void Pulse(bool easy = false, bool medium = false, bool hard = false, bool credit = false, bool serviceCredit = false)
    {
        Input = BuildInput(easy: easy, medium: medium, hard: hard, credit: credit, serviceCredit: serviceCredit);
        Tick();
        Input = BuildInput();
        Tick();
    }

    private GameIoInput BuildInput(
        int[]? pads = null,
        bool easy = false,
        bool medium = false,
        bool hard = false,
        bool credit = false,
        bool serviceCredit = false) =>
        new(
            padsHeld: (pads ?? []).Select(number => new FloorPad(number)),
            easyHeld: easy,
            mediumHeld: medium,
            hardHeld: hard,
            creditHeld: credit,
            serviceCreditHeld: serviceCredit,
            ticketNotchHeld: false);
}
