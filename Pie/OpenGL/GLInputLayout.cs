using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GLGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GLInputLayout : InputLayout
{
    private readonly InputLayoutDescription[] _descriptions;

    public GLInputLayout(InputLayoutDescription[] descriptions)
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
            
            switch (description.Format)
            {
                case Format.R8_UNorm:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.UnsignedByte, true, stride, (void*) description.Offset);
                    break;
                case Format.R8G8_UNorm:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.UnsignedByte, true, stride, (void*) description.Offset);
                    break;
                case Format.R8G8B8A8_UNorm:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.UnsignedByte, true, stride, (void*) description.Offset);
                    break;
                case Format.B8G8R8A8_UNorm:
                    throw new NotSupportedException("BGRA format not supported, use RGBA instead.");
                case Format.R16G16B16A16_UNorm:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.UnsignedShort, true, stride, (void*) description.Offset);
                    break;
                case Format.R16G16B16A16_SNorm:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.Short, true, stride, (void*) description.Offset);
                    break;
                case Format.R16G16B16A16_SInt:
                    Gl.VertexAttribIPointer(location, 4, VertexAttribIType.Short, stride, (void*) description.Offset);
                    break;
                case Format.R16G16B16A16_UInt:
                    Gl.VertexAttribIPointer(location, 4, VertexAttribIType.UnsignedShort, stride, (void*) description.Offset);
                    break;
                case Format.R16G16B16A16_Float:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.HalfFloat, false, stride, (void*) description.Offset);
                    break;
                case Format.R32G32_SInt:
                    Gl.VertexAttribIPointer(location, 2, VertexAttribIType.Int, stride, (void*) description.Offset);
                    break;
                case Format.R32G32_UInt:
                    Gl.VertexAttribIPointer(location, 2, VertexAttribIType.UnsignedInt, stride, (void*) description.Offset);
                    break;
                case Format.R32G32_Float:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, stride, (void*) description.Offset);
                    break;
                case Format.R32G32B32_SInt:
                    Gl.VertexAttribIPointer(location, 3, VertexAttribIType.Int, stride, (void*) description.Offset);
                    break;
                case Format.R32G32B32_UInt:
                    Gl.VertexAttribIPointer(location, 3, VertexAttribIType.UnsignedInt, stride, (void*) description.Offset);
                    break;
                case Format.R32G32B32_Float:
                    Gl.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, stride, (void*) description.Offset);
                    break;
                case Format.R32G32B32A32_SInt:
                    Gl.VertexAttribIPointer(location, 4, VertexAttribIType.Int, stride, (void*) description.Offset);
                    break;
                case Format.R32G32B32A32_UInt:
                    Gl.VertexAttribIPointer(location, 4, VertexAttribIType.UnsignedInt, stride, (void*) description.Offset);
                    break;
                case Format.R32G32B32A32_Float:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.Float, false, stride, (void*) description.Offset);
                    break;
                case Format.D24_UNorm_S8_UInt:
                    throw new NotSupportedException("Depth-stencil format not supported for input layouts.");
                case Format.R8_SNorm:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.Byte, true, stride, (void*) description.Offset);
                    break;
                case Format.R8_SInt:
                    Gl.VertexAttribIPointer(location, 1, VertexAttribIType.Byte, stride, (void*) description.Offset);
                    break;
                case Format.R8_UInt:
                    Gl.VertexAttribIPointer(location, 1, VertexAttribIType.UnsignedByte, stride, (void*) description.Offset);
                    break;
                case Format.R8G8_SNorm:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.Byte, true, stride, (void*) description.Offset);
                    break;
                case Format.R8G8_SInt:
                    Gl.VertexAttribIPointer(location, 2, VertexAttribIType.Byte, stride, (void*) description.Offset);
                    break;
                case Format.R8G8_UInt:
                    Gl.VertexAttribIPointer(location, 2, VertexAttribIType.UnsignedByte, stride, (void*) description.Offset);
                    break;
                case Format.R8G8B8A8_SNorm:
                    Gl.VertexAttribPointer(location, 4, VertexAttribPointerType.Byte, true, stride, (void*) description.Offset);
                    break;
                case Format.R8G8B8A8_SInt:
                    Gl.VertexAttribIPointer(location, 4, VertexAttribIType.Byte, stride, (void*) description.Offset);
                    break;
                case Format.R8G8B8A8_UInt:
                    Gl.VertexAttribIPointer(location, 4, VertexAttribIType.UnsignedByte, stride, (void*) description.Offset);
                    break;
                case Format.R16_UNorm:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.UnsignedShort, true, stride, (void*) description.Offset);
                    break;
                case Format.R16_SNorm:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.Short, true, stride, (void*) description.Offset);
                    break;
                case Format.R16_SInt:
                    Gl.VertexAttribIPointer(location, 1, VertexAttribIType.Short, stride, (void*) description.Offset);
                    break;
                case Format.R16_UInt:
                    Gl.VertexAttribIPointer(location, 1, VertexAttribIType.UnsignedShort, stride, (void*) description.Offset);
                    break;
                case Format.R16_Float:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.HalfFloat, false, stride, (void*) description.Offset);
                    break;
                case Format.R16G16_UNorm:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.UnsignedShort, true, stride, (void*) description.Offset);
                    break;
                case Format.R16G16_SNorm:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.Short, true, stride, (void*) description.Offset);
                    break;
                case Format.R16G16_SInt:
                    Gl.VertexAttribIPointer(location, 2, VertexAttribIType.Short, stride, (void*) description.Offset);
                    break;
                case Format.R16G16_UInt:
                    Gl.VertexAttribIPointer(location, 2, VertexAttribIType.UnsignedShort, stride, (void*) description.Offset);
                    break;
                case Format.R16G16_Float:
                    Gl.VertexAttribPointer(location, 2, VertexAttribPointerType.HalfFloat, false, stride, (void*) description.Offset);
                    break;
                case Format.R32_SInt:
                    Gl.VertexAttribIPointer(location, 1, VertexAttribIType.Int, stride, (void*) description.Offset);
                    break;
                case Format.R32_UInt:
                    Gl.VertexAttribIPointer(location, 1, VertexAttribIType.UnsignedInt, stride, (void*) description.Offset);
                    break;
                case Format.R32_Float:
                    Gl.VertexAttribPointer(location, 1, VertexAttribPointerType.Float, false, stride, (void*) description.Offset);
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