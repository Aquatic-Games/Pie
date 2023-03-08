using System;

namespace Pie.Null;

internal sealed class NullShader : Shader
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
