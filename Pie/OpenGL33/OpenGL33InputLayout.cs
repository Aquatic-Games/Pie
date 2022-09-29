using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL33.OpenGL33GraphicsDevice;

namespace Pie.OpenGL33;

internal sealed class OpenGL33InputLayout : InputLayout
{
    private readonly InputLayoutDescription[] _descriptions;

    public override uint Stride { get; }

    public OpenGL33InputLayout(uint stride, InputLayoutDescription[] descriptions)
    {
        _descriptions = descriptions;
        Stride = stride;
    }

    public unsafe void Set(uint handle)
    {
        int offset = 0;
        uint location = 0;
        foreach (InputLayoutDescription description in _descriptions)
        {
            //uint location = (uint) Gl.GetAttribLocation(handle, description.Name);
            //if (location == uint.MaxValue)
            //    continue;
            Gl.EnableVertexAttribArray(location);
            
            switch (description.Type)
            {
                case AttributeType.Int:
                    Gl.VertexAttribIPointer(location, 1, VertexAttribIType.Int, Stride, (void*) offset);
                    offset += 4;
                    break;
                case AttributeType.Int2:
                    Gl.VertexAttribIPointer(location, 2, VertexAttribIType.Int, Stride, (void*) offset);
                    offset += 8;
                    break;
                case AttributeType.Int3:
                    Gl.VertexAttribIPointer(location, 3, VertexAttribIType.Int, Stride, (void*) offset);
                    offset += 12;
                    break;
                case AttributeType.Int4:
                    Gl.VertexAttribIPointer(location, 4, VertexAttribIType.Int, Stride, (void*) offset);
                    offset += 16;
                    break;
                case AttributeType.Float:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.Float, false, Stride, (void*) offset);
                    offset += 4;
                    break;
                case AttributeType.Float2:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, Stride, (void*) offset);
                    offset += 8;
                    break;
                case AttributeType.Float3:
                    Gl.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, Stride, (void*) offset);
                    offset += 12;
                    break;
                case AttributeType.Float4:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.Float, false, Stride, (void*) offset);
                    offset += 16;
                    break;
                case AttributeType.Byte:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.UnsignedByte, false, Stride, (void*) offset);
                    offset += 1;
                    break;
                case AttributeType.Byte2:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.UnsignedByte, false, Stride, (void*) offset);
                    offset += 2;
                    break;
                case AttributeType.Byte4:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.UnsignedByte, false, Stride, (void*) offset);
                    offset += 4;
                    break;
                case AttributeType.NByte:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.UnsignedByte, true, Stride, (void*) offset);
                    offset += 1;
                    break;
                case AttributeType.NByte2:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.UnsignedByte, true, Stride, (void*) offset);
                    offset += 2;
                    break;
                case AttributeType.NByte4:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.UnsignedByte, true, Stride, (void*) offset);
                    offset += 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            location++;
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