using System;
using System.Runtime.InteropServices;

namespace Pie.Null;

internal sealed class NullGraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public readonly IntPtr Data;

    public NullGraphicsBuffer(IntPtr data)
    {
        Data = data;
    }

    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        Marshal.FreeHGlobal(Data);
        GC.SuppressFinalize(this);
    }
}
