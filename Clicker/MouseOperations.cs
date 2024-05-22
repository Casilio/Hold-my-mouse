using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

public class MouseOperations
{
    [Flags]
    public enum MouseEventFlags
    {
        LeftDown = 0x00000002,
        LeftUp = 0x00000004,
        MiddleDown = 0x00000020,
        MiddleUp = 0x00000040,
        Move = 0x00000001,
        Absolute = 0x00008000,
        RightDown = 0x00000008,
        RightUp = 0x00000010
    }

    [Flags]
    public enum MouseButtons
    {
        VK_LBUTTON = 0x01,
        VK_RBUTTON = 0x02
    }

    [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out MousePoint lpMousePoint);

    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    public static void ToggleButton()
    {
        var currentState = GetAsyncKeyState((int)MouseButtons.VK_LBUTTON);
        if (currentState != 0)
        {
            MouseEvent(MouseEventFlags.LeftUp);
        }
        else
        {
            MouseEvent(MouseEventFlags.LeftDown);
        }
    }

    private static MousePoint GetCursorPosition()
    {
        MousePoint currentMousePoint;
        var gotPoint = GetCursorPos(out currentMousePoint);
        if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
        return currentMousePoint;
    }

    private static void MouseEvent(MouseEventFlags value)
    {
        MousePoint position = GetCursorPosition();
        mouse_event((int)value, position.X, position.Y, 0, 0);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
        public int X;
        public int Y;

        public MousePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
