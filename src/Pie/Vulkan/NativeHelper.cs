using System.Runtime.InteropServices;
using System.Text;

namespace Pie.Vulkan;

public static unsafe class NativeHelper
{
    public static string[] PtrToStringArray(nint ptr, uint numStrings)
    {
        byte** bytePtr = (byte**) ptr;
        string[] strings = new string[numStrings];

        for (int i = 0; i < numStrings; i++)
            strings[i] = Marshal.PtrToStringAnsi((nint) bytePtr[i]);

        return strings;
    }
}