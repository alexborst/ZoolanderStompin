using System.Device.Gpio;
using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public sealed class LinuxGpioBank : IGpioBank, IDisposable
{
    private readonly GpioController _controller;

    public LinuxGpioBank()
    {
        try
        {
            _controller = new GpioController(PinNumberingScheme.Logical);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Could not open GPIO. On Raspberry Pi OS, add this user to the gpio group (`sudo usermod -aG gpio $USER`) and log in again.",
                ex);
        }
    }

    public void OpenInputPullUp(int bcm)
    {
        Open(bcm, PinMode.InputPullUp);
    }

    public void OpenOutput(int bcm, bool initialHigh)
    {
        Open(bcm, PinMode.Output);
        WriteHigh(bcm, initialHigh);
    }

    public bool ReadHigh(int bcm) => _controller.Read(bcm) == PinValue.High;

    public void WriteHigh(int bcm, bool high) =>
        _controller.Write(bcm, high ? PinValue.High : PinValue.Low);

    public void Dispose() => _controller.Dispose();

    private void Open(int bcm, PinMode mode)
    {
        if (_controller.IsPinOpen(bcm))
        {
            _controller.SetPinMode(bcm, mode);
            return;
        }

        _controller.OpenPin(bcm, mode);
    }
}
