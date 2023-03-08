using System;
using System.Runtime.InteropServices;

namespace Pie.Null;

internal sealed class NullGraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public readonly IntPtr Data;

    private readonly bool owned;

    public NullGraphicsBuffer(IntPtr data, bool owned)
    {
        Data = data;
        this.owned = owned;
    }

    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        if (owned)
        {
            Marshal.FreeHGlobal(Data);
        }

        GC.SuppressFinalize(this);
    }
}
