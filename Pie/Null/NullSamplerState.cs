using System;

namespace Pie.Null;

internal class NullSamplerState : SamplerState
{
    public override bool IsDisposed { get; protected set; }
    public override SamplerStateDescription Description { get; }

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
