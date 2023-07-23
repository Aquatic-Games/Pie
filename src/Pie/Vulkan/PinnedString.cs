using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Pie.Vulkan;

internal class PinnedString : IDisposable
{
    private GCHandle _handle;

    public nint Handle => _handle.AddrOfPinnedObject();

    public PinnedString(string @string)
    {
        _handle = GCHandle.Alloc(Encoding.UTF8.GetBytes(@string + '\0'), GCHandleType.Pinned);
    }

    public void Dispose()
    {
        _handle.Free();
    }
}