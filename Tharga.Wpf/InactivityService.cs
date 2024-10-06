using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Tharga.Wpf;

internal class InactivityService : IDisposable
{
    private readonly GlobalHook _globalHook;
    private readonly DispatcherTimer _inactivityTimer;
    private readonly Action<TimeSpan> _action;

    public InactivityService(TimeSpan timeout, Action<TimeSpan> action)
    {
        _action = action;

        // Initiera GlobalHook
        _globalHook = new GlobalHook();
        _globalHook.OnUserActivity += OnUserActivity;

        // Initiera timern
        _inactivityTimer = new DispatcherTimer();
        _inactivityTimer.Interval = timeout;
        _inactivityTimer.Tick += OnInactivityTimeout;

        // Hooka händelser
        _globalHook.Hook();
        _inactivityTimer.Start();
    }

    private void OnUserActivity()
    {
        _inactivityTimer.Stop();
        _inactivityTimer.Start();
    }

    private void OnInactivityTimeout(object sender, EventArgs e)
    {
        _action.Invoke(_inactivityTimer.Interval);
    }

    public void Dispose()
    {
        _globalHook.Unhook(); // Avregistrera hook när fönstret stängs
    }

    private class GlobalHook
    {
        public event Action OnUserActivity;

        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;

        private LowLevelMouseProc _mouseProc;
        private LowLevelMouseProc _keyboardProc;
        private IntPtr _mouseHookID = IntPtr.Zero;
        private IntPtr _keyboardHookID = IntPtr.Zero;

        public GlobalHook()
        {
            _mouseProc = HookCallbackMouse;
            _keyboardProc = HookCallbackKeyboard;
        }

        public void Hook()
        {
            _mouseHookID = SetHook(_mouseProc);
            _keyboardHookID = SetHook(_keyboardProc);
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(_mouseHookID);
            UnhookWindowsHookEx(_keyboardHookID);
        }

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallbackMouse(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                OnUserActivity?.Invoke();
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        private IntPtr HookCallbackKeyboard(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                OnUserActivity?.Invoke();
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        #region PInvoke

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}