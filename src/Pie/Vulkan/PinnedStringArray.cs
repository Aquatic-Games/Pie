using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Pie.Vulkan;

public unsafe class PinnedStringArray : IDisposable
{
    private byte** _ptr;
    private uint _numStrings;

    public nint Handle => (nint) _ptr;

    public PinnedStringArray(string[] strings)
    {
        _numStrings = (uint) strings.Length;
        
        _ptr = (byte**) NativeMemory.Alloc((nuint) (_numStrings * sizeof(byte*)));

        for (int i = 0; i < _numStrings; i++)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(strings[i]);
            _ptr[i] = (byte*) NativeMemory.Alloc((uint) bytes.Length);
            fixed (byte* ptr = bytes)
                Unsafe.CopyBlock(_ptr[i], ptr, (uint) bytes.Length);
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < _numStrings; i++)
            NativeMemory.Free(_ptr[i]);
        
        NativeMemory.Free(_ptr);
    }
}