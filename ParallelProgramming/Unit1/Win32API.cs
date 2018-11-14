using System;
using System.Runtime.InteropServices;

namespace SharedData
{
	// this class is queried from winnt.h
    public class Win32API
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr OpenEvent(UInt32 dwDesiredAccess,
                bool bInheritHandle, String lpName);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(
            out long lpFrequency);
    }
}