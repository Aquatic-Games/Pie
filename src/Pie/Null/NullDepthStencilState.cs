using System;

namespace Pie.Null;

internal sealed class NullDepthStencilState : DepthStencilState
{
    public override bool IsDisposed { get; protected set; }
    public override DepthStencilStateDescription Description { get; }

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
