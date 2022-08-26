using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal sealed class OpenGL33InputLayout : InputLayout
{
    private readonly InputLayoutDescription[] _descriptions;

    public override uint Stride { get; }

    public OpenGL33InputLayout(InputLayoutDescription[] descriptions)
    {
        _descriptions = descriptions;
        foreach (InputLayoutDescription description in descriptions)
            Stride += (uint) description.Type * 4;
    }

    public OpenGL33InputLayout(uint stride, InputLayoutDescription[] descriptions)
    {
        _descriptions = descriptions;
        Stride = stride;
    }

    public unsafe void Set(uint handle)
    {
        int offset = 0;
        foreach (InputLayoutDescription description in _descriptions)
        {
            uint location = (uint) Gl.GetAttribLocation(handle, description.Name);
            Gl.EnableVertexAttribArray(location);
            Gl.VertexAttribPointer(location, (int) description.Type, VertexAttribPointerType.Float, false, Stride,
                (void*) offset);
            offset += (int) description.Type * 4;
        }
    }

    public override bool IsDisposed { get; protected set; }

    public override InputLayoutDescription[] Descriptions => _descriptions;

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        // There is nothing to actually dispose.
    }
}