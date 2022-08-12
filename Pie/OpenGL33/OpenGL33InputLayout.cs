using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal class OpenGL33InputLayout : InputLayout
{
    private readonly InputLayoutDescription[] _descriptions;

    private readonly uint _stride;

    public OpenGL33InputLayout(InputLayoutDescription[] descriptions)
    {
        _descriptions = descriptions;
        foreach (InputLayoutDescription description in descriptions)
            _stride += (uint) description.Type * 4;
    }

    public unsafe void Set(uint handle)
    {
        int offset = 0;
        foreach (InputLayoutDescription description in _descriptions)
        {
            uint location = (uint) Gl.GetAttribLocation(handle, description.Name);
            Gl.EnableVertexAttribArray(location);
            Gl.VertexAttribPointer(location, (int) description.Type, VertexAttribPointerType.Float, false, _stride,
                (void*) offset);
            offset += (int) description.Type * 4;
        }
    }

    public override bool IsDisposed { get; protected set; }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        // There is nothing to actually dispose.
    }
}