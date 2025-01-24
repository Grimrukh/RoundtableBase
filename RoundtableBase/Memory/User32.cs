using System.Runtime.InteropServices;

namespace RoundtableBase.Memory;

internal static class User32
{
    [DllImport("user32.dll")]
    static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    public static uint GetForegroundProcessID()
    {
        IntPtr hWnd = GetForegroundWindow();
        GetWindowThreadProcessId(hWnd, out uint pid);
        return pid;
    }
}