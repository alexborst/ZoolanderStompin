namespace ZoolanderStompin.Game;

public interface IGpioBank
{
    void OpenInputPullUp(int bcm);

    void OpenOutput(int bcm, bool initialHigh);

    bool ReadHigh(int bcm);

    void WriteHigh(int bcm, bool high);
}
