using System;
using Silk.NET.OpenGLES;
using static Pie.OpenGLES20.OpenGLES20GraphicsDevice;

namespace Pie.OpenGLES20;

internal sealed class OpenGLES20InputLayout : InputLayout
{
    private readonly InputLayoutDescription[] _descriptions;

    public OpenGLES20InputLayout(InputLayoutDescription[] descriptions)
    {
        _descriptions = descriptions;
    }

    public unsafe void Set(uint slot, uint stride)
    {
        uint location = 0;
        foreach (InputLayoutDescription description in _descriptions)
        {
            if (description.Slot != slot)
            {
                location++;
                continue;
            }

            //uint location = (uint) Gl.GetAttribLocation(handle, description.Name);
            //if (location == uint.MaxValue)
            //    continue;
            Gl.EnableVertexAttribArray(location);
            
            switch (description.Type)
            {
                case AttributeType.Int:
                    throw new NotSupportedException("Integer types are not (currently) supported in OpenGL ES 2.0.");
                    //Gl.VertexAttribIPointer(location, 1, VertexAttribIType.Int, stride, (void*) description.Offset);
                    break;
                case AttributeType.Int2:
                    //Gl.VertexAttribIPointer(location, 2, VertexAttribIType.Int, stride, (void*) description.Offset);
                    throw new NotSupportedException("Integer types are not (currently) supported in OpenGL ES 2.0.");
                    break;
                case AttributeType.Int3:
                    //Gl.VertexAttribIPointer(location, 3, VertexAttribIType.Int, stride, (void*) description.Offset);
                    throw new NotSupportedException("Integer types are not (currently) supported in OpenGL ES 2.0.");
                    break;
                case AttributeType.Int4:
                    //Gl.VertexAttribIPointer(location, 4, VertexAttribIType.Int, stride, (void*) description.Offset);
                    throw new NotSupportedException("Integer types are not (currently) supported in OpenGL ES 2.0.");
                    break;
                case AttributeType.Float:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.Float, false, stride, (void*) description.Offset);
                    break;
                case AttributeType.Float2:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, stride, (void*) description.Offset);
                    break;
                case AttributeType.Float3:
                    Gl.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, stride, (void*) description.Offset);
                    break;
                case AttributeType.Float4:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.Float, false, stride, (void*) description.Offset);
                    break;
                case AttributeType.Byte:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.UnsignedByte, false, stride, (void*) description.Offset);
                    break;
                case AttributeType.Byte2:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.UnsignedByte, false, stride, (void*) description.Offset);
                    break;
                case AttributeType.Byte4:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.UnsignedByte, false, stride, (void*) description.Offset);
                    break;
                case AttributeType.NByte:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.UnsignedByte, true, stride, (void*) description.Offset);
                    break;
                case AttributeType.NByte2:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.UnsignedByte, true, stride, (void*) description.Offset);
                    break;
                case AttributeType.NByte4:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.UnsignedByte, true, stride, (void*) description.Offset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Gl.VertexAttribDivisor(location, (uint) description.InputType);
            
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