using ZoolanderStompin.Game;

namespace ZoolanderStompin.Tests;

public sealed class TargetLoopDriver
{
    public TargetLoopDriver(IPadPicker picker, GameOptions? options = null, Difficulty difficulty = Difficulty.Easy)
    {
        Options = options ?? CreateShortRound();
        Clock = new FakeGameClock();
        Picker = picker;
        Difficulty = difficulty;
        Loop = new TargetLoop(Options, difficulty, Clock, picker);
        Input = GameIoInput.None;
    }

    public static TargetLoopDriver Scripted(
        GameOptions? options = null,
        Difficulty difficulty = Difficulty.Easy,
        params int[] pads) =>
        new(new ScriptedPadPicker(pads), options, difficulty);

    public GameOptions Options { get; }

    public FakeGameClock Clock { get; }

    public IPadPicker Picker { get; }

    public Difficulty Difficulty { get; }

    public TargetLoop Loop { get; }

    public GameIoInput Input { get; set; }

    public ScriptedPadPicker Script =>
        Picker as ScriptedPadPicker
        ?? throw new InvalidOperationException("This driver is not using a scripted pad picker.");

    public TimeSpan Debounce => TimeSpan.FromMilliseconds(Options.DebounceMilliseconds);

    public TimeSpan Window => TimeSpan.FromMilliseconds(Options.For(Difficulty).HitWindowMilliseconds);

    public TimeSpan Gap => TimeSpan.FromMilliseconds(Options.InterTargetGapMilliseconds);

    public GameIoOutput Output => Loop.ToOutput();

    public void Tick() => Loop.Tick(Input);

    public void LightFirst() => Tick();

    public void SetPads(params int[] numbers)
    {
        Input = new GameIoInput(
            padsHeld: numbers.Select(number => new FloorPad(number)),
            easyHeld: false,
            mediumHeld: false,
            hardHeld: false,
            creditHeld: false,
            serviceCreditHeld: false,
            ticketNotchHeld: false);
    }

    public void AdvanceAndTick(TimeSpan duration)
    {
        Clock.Advance(duration);
        Tick();
    }

    public void StabilizeHeldPads()
    {
        Tick();
        AdvanceAndTick(Debounce);
    }

    public void Stomp(params int[] pads)
    {
        SetPads(pads);
        StabilizeHeldPads();
    }

    public void Release()
    {
        SetPads();
        StabilizeHeldPads();
    }

    public void MissCurrent()
    {
        if (Loop.Phase != TargetLoopPhase.Presenting)
        {
            throw new InvalidOperationException($"Expected Presenting, was {Loop.Phase}.");
        }

        AdvanceAndTick(Window);
    }

    public void FinishGap()
    {
        if (Loop.Phase != TargetLoopPhase.Gap)
        {
            throw new InvalidOperationException($"Expected Gap, was {Loop.Phase}.");
        }

        AdvanceAndTick(Gap);
    }

    public static GameOptions CreateShortRound(int presentations = 5)
    {
        var options = GameOptions.CreateDefault();
        options.PresentationsPerRound = presentations;
        options.DebounceMilliseconds = 30;
        options.InterTargetGapMilliseconds = 50;
        options.Easy.HitWindowMilliseconds = 100;
        options.Medium.HitWindowMilliseconds = 100;
        options.Hard.HitWindowMilliseconds = 100;
        return options;
    }
}
