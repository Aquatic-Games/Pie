using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Pie.Utils.NativeHelpers;

public unsafe class PinnedString : IDisposable
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
    
    public void Dispose()
    {
        _handle.Free();
    }
}