using System;
using System.Runtime.InteropServices;

namespace A500Launcher.Services;

public static class GamepadDetector
{
    [StructLayout(LayoutKind.Sequential)]
    private struct XInputState
    {
        public uint PacketNumber;
        public XInputGamepad Gamepad;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XInputGamepad
    {
        public ushort Buttons;
        public byte LeftTrigger;
        public byte RightTrigger;
        public short ThumbLX;
        public short ThumbLY;
        public short ThumbRX;
        public short ThumbRY;
    }

    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
    private static extern uint XInputGetState14(uint index, out XInputState state);

    [DllImport("xinput1_3.dll", EntryPoint = "XInputGetState")]
    private static extern uint XInputGetState13(uint index, out XInputState state);

    private const uint ErrorSuccess = 0;

    public static bool IsGamepadConnected()
    {
        for (uint index = 0; index < 4; index++)
        {
            if (TryGetState(index, out _))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetState(uint index, out XInputState state)
    {
        try
        {
            return XInputGetState14(index, out state) == ErrorSuccess;
        }
        catch (DllNotFoundException)
        {
        }

        try
        {
            return XInputGetState13(index, out state) == ErrorSuccess;
        }
        catch (DllNotFoundException)
        {
        }

        state = default;
        return false;
    }
}
