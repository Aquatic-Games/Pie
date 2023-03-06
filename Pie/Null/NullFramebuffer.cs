using System;
using System.Drawing;

namespace Pie.Null;

internal class NullFramebuffer : Framebuffer
{
    public override bool IsDisposed { get; protected set; }
    public override Size Size { get; set; }

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
