using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BotCore.Interop
{
    [SuppressUnmanagedCodeSecurityAttribute]
    internal static class SafeNativeMethods
    {
        [Flags]
        internal enum PageAccessFlags : uint
        {
            PAGE_READWRITE = 0x00000004,
            MEM_COMMIT = 0x00001000,
            MEM_PRIVATE = 0x00020000
        }
        [Flags]
        internal enum ProcessAccessFlags : uint
        {
            PROCESS_VM_READ = 0x00000010,
            PROCESS_QUERY_INFORMATION = 0x00000400
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr baseAddress;
            public IntPtr allocationBase;
            public uint allocationProtect;
            public UIntPtr regionSize;
            public PageAccessFlags state;
            public PageAccessFlags protect;
            public PageAccessFlags type;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, UIntPtr nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        internal static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, UIntPtr dwLength);
    }
    public sealed class MemoryPatternSearcher
    {
        GameClient Client { get; set; }

        public MemoryPatternSearcher(GameClient Client)
        {
            this.Client = Client;
        }

        public byte[] ReadProcessMemory(IntPtr baseAddress, UIntPtr size)
        {
            byte[] ReturnBuffer = new byte[(ulong)size];
            UInt32 ReadBytes = 0;
            return SafeNativeMethods.ReadProcessMemory(Client.Memory.Handle.DangerousGetHandle(), baseAddress, ReturnBuffer, size, ref ReadBytes) ? ReturnBuffer : null;
        }

        public int? FindMemoryRegion(int FunctionPointer)
        {
            if (Client == null)
                return null;

            if (!Client.Memory.IsRunning && !Client.IsInGame())
                return 0;

            byte[] srchlpBuffer;
            uint lpMem = 0x00010000;
            UIntPtr lLenMPI = (UIntPtr)Marshal.SizeOf(typeof(SafeNativeMethods.MEMORY_BASIC_INFORMATION));
            SafeNativeMethods.MEMORY_BASIC_INFORMATION mbi;

            while (lpMem < 0x7ffeffff)
            {
                SafeNativeMethods.VirtualQueryEx(Client.Memory.Handle.DangerousGetHandle(), (IntPtr)lpMem, out mbi, lLenMPI);

                if (mbi.type == SafeNativeMethods.PageAccessFlags.MEM_PRIVATE && mbi.state == SafeNativeMethods.PageAccessFlags.MEM_COMMIT && mbi.protect == SafeNativeMethods.PageAccessFlags.PAGE_READWRITE)
                {
                    if ((srchlpBuffer = ReadProcessMemory(mbi.baseAddress, mbi.regionSize)) == null)
                        return null;

                    for (uint i = 0; i < (uint)mbi.regionSize; i = i + 4)
                    {
                        if ((srchlpBuffer[i] + 256 * srchlpBuffer[i + 1] + 256 * 256 * srchlpBuffer[i + 2]) == FunctionPointer)
                            return (int)((int)mbi.baseAddress + i);
                    }
                }
                lpMem = (uint)mbi.baseAddress + (uint)mbi.regionSize;
            }
            return 0; 
        }
    }
}