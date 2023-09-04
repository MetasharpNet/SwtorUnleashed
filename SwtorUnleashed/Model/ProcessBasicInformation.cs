using System;
using System.Runtime.InteropServices;

namespace SwtorUnleashed.Model
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessBasicInformation
    {
        // These members must match PROCESS_BASIC_INFORMATION
        internal IntPtr Reserved1;
        internal IntPtr PebBaseAddress;
        internal IntPtr Reserved2_0;
        internal IntPtr Reserved2_1;
        internal IntPtr UniqueProcessId;
        internal IntPtr InheritedFromUniqueProcessId;
    }
}
