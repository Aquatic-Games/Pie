using System;

namespace Pie.Null;

internal class NullBlendState : BlendState
{
    public override bool IsDisposed { get; protected set; }
    public override BlendStateDescription Description { get; }

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
