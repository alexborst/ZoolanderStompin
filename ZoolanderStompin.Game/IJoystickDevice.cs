namespace ZoolanderStompin.Game;

public interface IJoystickDevice
{
    void Pump();

    bool IsButtonHeld(int button);
}
