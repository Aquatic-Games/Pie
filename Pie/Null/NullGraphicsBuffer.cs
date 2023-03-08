using System;

namespace Pie.Null;

internal sealed class NullGraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        GC.SuppressFinalize(this);
    }
}
