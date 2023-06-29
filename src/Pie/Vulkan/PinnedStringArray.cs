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

    public uint Length => _numStrings;

    public PinnedStringArray(string[] strings)
    {
        _numStrings = (uint) strings.Length;
        
        _ptr = (byte**) NativeMemory.Alloc((nuint) (_numStrings * sizeof(byte*)));

        for (int i = 0; i < _numStrings; i++)
        {
            // We need to append a null character at the end as one is not added for us.
            // C strings are always null terminated.
            byte[] bytes = Encoding.UTF8.GetBytes(strings[i] + '\0');
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