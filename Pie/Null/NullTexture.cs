using System;

namespace Pie.Null;

internal class NullTexture : Texture
{
    public override bool IsDisposed { get; protected set; }
    public override TextureDescription Description { get; set; }

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
