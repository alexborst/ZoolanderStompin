using System.Runtime.InteropServices;
using ZoolanderStompin.Game;

namespace ZoolanderStompin;

public sealed class LinuxJoystickDevice : IJoystickDevice, IDisposable
{
    private const int OpenReadOnly = 0;
    private const int OpenNonblock = 2048;
    private const int JsEventButton = 0x01;
    private const int JsEventInit = 0x80;
    private const int EventBytes = 8;
    private const int Eagain = 11;
    private const int Eintr = 4;

    private int _fd;
    private readonly bool[] _held = new bool[32];

    public LinuxJoystickDevice(string device)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(device);

        _fd = Open(device, OpenReadOnly | OpenNonblock);
        if (_fd < 0)
        {
            throw new InvalidOperationException(
                $"Could not open joystick '{device}'. Plug in the CY-822B, check `ls /dev/input/js*`, and add this user to the input group (`sudo usermod -aG input $USER`) then log in again.");
        }

        try
        {
            Pump();
        }
        catch
        {
            Close(_fd);
            _fd = -1;
            throw;
        }
    }

    public void Pump()
    {
        var buffer = new byte[EventBytes];
        while (true)
        {
            var n = Read(_fd, buffer, EventBytes);
            if (n < 0)
            {
                var errno = Marshal.GetLastPInvokeError();
                if (errno is Eagain or Eintr)
                {
                    return;
                }

                throw new InvalidOperationException($"Joystick read failed (errno {errno}).");
            }

            if (n == 0 || n != EventBytes)
            {
                return;
            }

            var value = BitConverter.ToInt16(buffer, 4);
            var type = buffer[6];
            var number = buffer[7];
            if ((type & ~JsEventInit) != JsEventButton || number >= _held.Length)
            {
                continue;
            }

            _held[number] = value != 0;
        }
    }

    public bool IsButtonHeld(int button) =>
        button >= 0 && button < _held.Length && _held[button];

    public void Dispose()
    {
        if (_fd < 0)
        {
            return;
        }

        Close(_fd);
        _fd = -1;
    }

    [DllImport("libc", EntryPoint = "open", SetLastError = true)]
    private static extern int Open(string pathname, int flags);

    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    private static extern int Close(int fd);

    [DllImport("libc", EntryPoint = "read", SetLastError = true)]
    private static extern int Read(int fd, byte[] buffer, int count);
}
