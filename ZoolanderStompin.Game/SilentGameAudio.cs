namespace ZoolanderStompin.Game;

public sealed class SilentGameAudio : IGameAudio
{
    public void Play(GameSound? sound)
    {
        _ = sound;
    }
}
