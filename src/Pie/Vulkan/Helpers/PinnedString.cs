using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Pie.Vulkan.Helpers;

internal unsafe class PinnedString : IDisposable
{
    private byte[] _bytes;
    private GCHandle _handle;

    public nint Address => _handle.AddrOfPinnedObject();

    public byte* AsPtr => (byte*) _handle.AddrOfPinnedObject();
    
    public PinnedString(string @string, Encoding encoding)
    {
        _bytes = encoding.GetBytes(@string);
        _handle = GCHandle.Alloc(_bytes, GCHandleType.Pinned);
    }

    public static implicit operator PinnedString(string text)
    {
        return new PinnedString(text, Encoding.UTF8);
    }

    public void Dispose()
    {
        _handle.Free();
    }
}