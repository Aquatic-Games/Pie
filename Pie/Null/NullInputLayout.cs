using System;

namespace Pie.Null;

internal class NullInputLayout : InputLayout
{
    public override bool IsDisposed { get; protected set; }
    public override InputLayoutDescription[] Descriptions { get; } = Array.Empty<InputLayoutDescription>();

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
