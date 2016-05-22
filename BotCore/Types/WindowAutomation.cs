using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace BotCore
{
    public static class WindowAutomator
    {
        public static readonly byte VK_SHIFT = 0x10;
        public static readonly byte VK_ESCAPE = 0x1B;
        public static readonly byte VK_SPACE = 0x20;
        public static readonly byte VK_TILDE = 0xC0;

        public static readonly uint MK_LBUTTON = 0x1;
        public static readonly uint MK_RBUTTON = 0x2;
        public static readonly uint MK_MBUTTON = 0x10;
        public static readonly uint MK_XBUTTON1 = 0x20;
        public static readonly uint MK_XUBTTON2 = 0x40;

        enum KeyboardCommand : uint
        {
            HotKey = 0x312,
            KeyDown = 0x100,
            KeyUp = 0x101,
            Char = 0x102,
            DeadChar = 0x103,
            SysKeyDown = 0x014,
            SysKeyUp = 0x105,
            SysDeadChar = 0x107
        }

        enum MouseCommand : uint
        {
            MouseMove = 0x200,
            LeftButtonDown = 0x201,
            LeftButtonUp = 0x202,
            LeftButtonDoubleClick = 203,
            RightButtonDown = 0x204,
            RightButtonUp = 0x205,
            RightButtonDoubleClick = 0x206,
            MiddleButtonDown = 0x207,
            MiddleButtonUp = 0x208,
            MiddleButtonDoubleClick = 0x209
        }

        enum ControlCommand : uint
        {
            WM_CLOSE = 0x10
        }

        sealed class KeyParameter
        {
            short repeatCount;
            byte scanCode;
            bool isExtendedKey;
            bool contextCode;
            bool previousState;
            bool transitionState;

            public KeyParameter(uint lParam)
            {
                this.repeatCount = (short)(lParam & 0xFFFF);
                this.scanCode = (byte)((lParam >> 16) & 0xFF);
                this.isExtendedKey = (lParam & (1 << 24)) != 0;
                this.contextCode = (lParam & (1 << 29)) != 0;
                this.previousState = (lParam & (1 << 30)) != 0;
                this.transitionState = (lParam & (1 << 31)) != 0;
            }

            public KeyParameter(short repeatCount, byte scanCode, bool isExtendedKey = false, bool contextCode = false, bool previousState = false, bool transitionState = true)
            {
                this.repeatCount = repeatCount;
                this.scanCode = scanCode;
                this.isExtendedKey = isExtendedKey;
                this.contextCode = contextCode;
                this.previousState = previousState;
                this.transitionState = transitionState;
            }

            public uint ToLParam()
            {
                uint lParam = (uint)repeatCount;

                lParam |= ((uint)scanCode << 16);

                if (isExtendedKey)
                    lParam |= (1u << 24);

                if (contextCode)
                    lParam |= (1u << 29);

                if (previousState)
                    lParam |= (1u << 30);

                if (transitionState)
                    lParam |= (1u << 31);

                return lParam;
            }
        }

        #region Keyboard Actions
        public static void SendKeyDown(IntPtr windowHandle, char key)
        {
            ModifierKeys modifiers;
            var virtualKey = GetVirtualKey(key, out modifiers);

            SendKeyDown(windowHandle, virtualKey);
        }

        public static void SendKeyChar(IntPtr windowHandle, char key)
        {
            ModifierKeys modifiers;
            var virtualKey = GetVirtualKey(key, out modifiers);

            SendKeyChar(windowHandle, virtualKey);
        }

        public static void SendKeyUp(IntPtr windowHandle, char key)
        {
            ModifierKeys modifiers;
            var virtualKey = GetVirtualKey(key, out modifiers);

            SendKeyUp(windowHandle, virtualKey);
        }

        public static void SendKeyDown(IntPtr windowHandle, byte virtualKey)
        {
            var scanCode = GetScanCode(virtualKey);
            var keyParameter = new KeyParameter(1, scanCode, false, false, false, false);

            NativeMethods.PostMessage(windowHandle, (uint)KeyboardCommand.KeyDown, virtualKey, keyParameter.ToLParam());
        }

        public static void SendKeyChar(IntPtr windowHandle, byte virtualKey)
        {

        }

        public static void SendKeyUp(IntPtr windowHandle, byte virtualKey)
        {
            var scanCode = GetScanCode(virtualKey);
            var keyParameter = new KeyParameter(1, scanCode, false, false, true, true);

            NativeMethods.PostMessage(windowHandle, (uint)KeyboardCommand.KeyUp, virtualKey, keyParameter.ToLParam());
        }

        public static void SendShiftKeyDown(IntPtr windowHandle)
        {
            var virtualKey = VK_SHIFT;
            var scanCode = GetScanCode(virtualKey);
            var keyParameter = new KeyParameter(1, scanCode, false, false, false, false);

            NativeMethods.PostMessage(windowHandle, (uint)KeyboardCommand.KeyDown, virtualKey, keyParameter.ToLParam());
        }

        public static void SendShiftKeyUp(IntPtr windowHandle)
        {
            var virtualKey = VK_SHIFT;
            var scanCode = GetScanCode(virtualKey);
            var keyParameter = new KeyParameter(1, scanCode, false, false, true, true);

            NativeMethods.PostMessage(windowHandle, (uint)KeyboardCommand.KeyUp, virtualKey, keyParameter.ToLParam());
        }

        public static void SendKeystroke(IntPtr windowHandle, char key, bool includeCharMessage = false)
        {
            SendKeyDown(windowHandle, key);

            if (includeCharMessage)
                SendKeyChar(windowHandle, key);

            SendKeyUp(windowHandle, key);
        }

        public static void SendKeystroke(IntPtr windowHandle, byte virtualKey, bool includeCharMessage = false)
        {
            SendKeyDown(windowHandle, virtualKey);

            if (includeCharMessage)
                SendKeyChar(windowHandle, virtualKey);

            SendKeyUp(windowHandle, virtualKey);
        }
        #endregion

        #region Mouse Actions
        public static void SendMouseMove(IntPtr windowHandle, int x, int y)
        {
            NativeMethods.PostMessage(windowHandle, (uint)MouseCommand.MouseMove, 0x0, MakeXYParameter(new Point(x, y)));
        }
        #endregion

        #region Control Actions
        public static void SendCloseWindow(IntPtr windowHandle)
        {
            NativeMethods.PostMessage(windowHandle, (uint)ControlCommand.WM_CLOSE, 0, 0);
        }
        #endregion

        #region Helper Methods
        static uint MakeXYParameter(Point pt)
        {
            uint parameter = (uint)pt.X;
            parameter |= ((uint)pt.Y << 16);

            return parameter;
        }

        static byte GetScanCode(char c)
        {
            var vkey = GetVirtualKey(c);
            var scanCode = NativeMethods.MapVirtualKey(vkey, VirtualKeyMapMode.VirtualToScanCode);

            return (byte)scanCode;
        }

        static byte GetScanCode(byte virtualKey)
        {
            var scanCode = NativeMethods.MapVirtualKey(virtualKey, VirtualKeyMapMode.VirtualToScanCode);

            return (byte)scanCode;
        }

        static byte GetVirtualKey(char c)
        {
            ModifierKeys modifiers;
            return GetVirtualKey(c, out modifiers);
        }

        static byte GetVirtualKey(char c, out ModifierKeys modifiers)
        {
            modifiers = ModifierKeys.None;
            var keyScan = NativeMethods.VkKeyScan(c);

            byte vkey = (byte)keyScan;
            byte modifierScan = (byte)(keyScan >> 8);

            modifiers = (ModifierKeys)modifierScan;
            return vkey;
        }
        #endregion
    }

    internal enum VirtualKeyMapMode
    {
        VirtualToScanCode = 0,
        ScanCodeToVirtual = 1,
        VirtualToChar = 2,
        ScanCodeToVirtualEx = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        int left;
        int top;
        int right;
        int bottom;

        public int Left
        {
            get { return left; }
            set { left = value; }
        }

        public int Top
        {
            get { return top; }
            set { top = value; }
        }

        public int Right
        {
            get { return right; }
            set { right = value; }
        }

        public int Bottom
        {
            get { return bottom; }
            set { bottom = value; }
        }

        public int Width
        {
            get { return Math.Abs(right - left); }
        }

        public int Height
        {
            get { return Math.Abs(bottom - top); }
        }
    }

    [return: MarshalAs(UnmanagedType.Bool)]
    internal delegate bool EnumWindowsProc(IntPtr windowHandle, IntPtr lParam);

    internal static class NativeMethods
    {
        [DllImport("user32", EntryPoint = "VkKeyScan")]
        internal static extern ushort VkKeyScan(char character);

        [DllImport("user32", EntryPoint = "MapVirtualKey")]
        internal static extern uint MapVirtualKey(uint keyCode, VirtualKeyMapMode mapMode);


        [DllImport("user32", EntryPoint = "EnumWindows")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumWindows(EnumWindowsProc enumWindowProc, IntPtr lParam);

        [DllImport("user32", EntryPoint = "GetClassName")]
        internal static extern int GetClassName(IntPtr windowHandle, StringBuilder className, int maxLength);

        [DllImport("user32", EntryPoint = "GetWindowTextLength")]
        internal static extern int GetWindowTextLength(IntPtr windowHandle);

        [DllImport("user32", EntryPoint = "GetWindowText")]
        internal static extern int GetWindowText(IntPtr windowHandle, StringBuilder windowText, int maxLength);

        [DllImport("user32", EntryPoint = "GetWindowThreadProcessId")]
        internal static extern int GetWindowThreadProcessId(IntPtr windowHandle, out int processId);

        [DllImport("user32", EntryPoint = "GetWindowRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr windowHandle, out Rect windowRectangle);

        [DllImport("user32", EntryPoint = "PostMessage")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PostMessage(IntPtr windowHandle, uint message, uint wParam, uint lParam);

        [DllImport("user32", EntryPoint = "SendMessage")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SendMessage(IntPtr windowHandle, uint message, uint wParam, uint lParam);


        [DllImport("user32", EntryPoint = "SetForegroundWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr windowHandle);

        [DllImport("user32", EntryPoint = "RegisterHotKey")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RegisterHotKey(IntPtr windowHandle, int hotkeyId, ModifierKeys modifiers, int virtualKey);

        [DllImport("user32", EntryPoint = "UnregisterHotKey")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnregisterHotKey(IntPtr windowHandle, int hotkeyId);

        [DllImport("kernel32", EntryPoint = "GlobalAddAtom")]
        internal static extern ushort GlobalAddAtom(string atomName);

        [DllImport("kernel32", EntryPoint = "GlobalDeleteAtom")]
        internal static extern ushort GlobalDeleteAtom(ushort atom);

        [DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr baseAddress, byte[] buffer, int count, out int numberOfBytesRead);

        [DllImport("kernel32", EntryPoint = "WriteProcessMemory")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteProcessMemory(IntPtr processHandle, IntPtr baseAddress, byte[] buffer, int count, out int numberOfBytesWritten);

        [DllImport("kernel32", EntryPoint = "ResumeThread")]
        internal static extern int ResumeThread(IntPtr threadHandle);

        [DllImport("kernel32", EntryPoint = "CloseHandle")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", EntryPoint = "GetLastError")]
        internal static extern int GetLastError();
    }
}
