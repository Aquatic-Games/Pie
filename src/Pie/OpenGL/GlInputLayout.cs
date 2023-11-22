using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GlGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GlInputLayout : InputLayout
{
    private readonly InputLayoutDescription[] _descriptions;

    public readonly uint Vao;

    public GlInputLayout(InputLayoutDescription[] descriptions, uint vao)
    {
        _descriptions = descriptions;

        Vao = vao;
    }

    public unsafe void Set(uint slot, uint stride)
    {
        Gl.BindVertexArray(Vao);
        
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
                    Gl.VertexAttribFormat(location, 1, VertexAttribType.UnsignedByte, true, description.Offset);
                    break;
                case Format.R8G8_UNorm:
                    Gl.VertexAttribFormat(location, 2, VertexAttribType.UnsignedByte, true, description.Offset);
                    break;
                case Format.R8G8B8A8_UNorm:
                    Gl.VertexAttribFormat(location, 4, VertexAttribType.UnsignedByte, true, description.Offset);
                    break;
                case Format.B8G8R8A8_UNorm:
                    // According to the docs, the size parameter can be GL_BGRA. I'm not convinced I've implemented this
                    // correctly, but I don't really have anything to check.
                    Gl.VertexAttribFormat(location, (int) GLEnum.Bgra, GLEnum.UnsignedByte, true, description.Offset);
                    break;
                case Format.R16G16B16A16_UNorm:
                    Gl.VertexAttribFormat(location, 4, VertexAttribType.UnsignedShort, true, description.Offset);
                    break;
                case Format.R16G16B16A16_SNorm:
                    Gl.VertexAttribFormat(location, 4, VertexAttribType.Short, true, description.Offset);
                    break;
                case Format.R16G16B16A16_SInt:
                    Gl.VertexAttribIFormat(location, 4, VertexAttribIType.Short, description.Offset);
                    break;
                case Format.R16G16B16A16_UInt:
                    Gl.VertexAttribIFormat(location, 4, VertexAttribIType.UnsignedShort, description.Offset);
                    break;
                case Format.R16G16B16A16_Float:
                    Gl.VertexAttribFormat(location, 4, VertexAttribType.HalfFloat, false, description.Offset);
                    break;
                case Format.R32G32_SInt:
                    Gl.VertexAttribIFormat(location, 2, VertexAttribIType.Int, description.Offset);
                    break;
                case Format.R32G32_UInt:
                    Gl.VertexAttribIFormat(location, 2, VertexAttribIType.UnsignedInt, description.Offset);
                    break;
                case Format.R32G32_Float:
                    Gl.VertexAttribFormat(location, 2, VertexAttribType.Float, false, description.Offset);
                    break;
                case Format.R32G32B32_SInt:
                    Gl.VertexAttribIFormat(location, 3, VertexAttribIType.Int, description.Offset);
                    break;
                case Format.R32G32B32_UInt:
                    Gl.VertexAttribIFormat(location, 3, VertexAttribIType.UnsignedInt, description.Offset);
                    break;
                case Format.R32G32B32_Float:
                    Gl.VertexAttribFormat(location, 3, VertexAttribType.Float, false, description.Offset);
                    break;
                case Format.R32G32B32A32_SInt:
                    Gl.VertexAttribIFormat(location, 4, VertexAttribIType.Int, description.Offset);
                    break;
                case Format.R32G32B32A32_UInt:
                    Gl.VertexAttribIFormat(location, 4, VertexAttribIType.UnsignedInt, description.Offset);
                    break;
                case Format.R32G32B32A32_Float:
                    Gl.VertexAttribFormat(location, 4, VertexAttribType.Float, false, description.Offset);
                    break;
                case Format.D24_UNorm_S8_UInt:
                    throw new NotSupportedException("Depth-stencil format not supported for input layouts.");
                case Format.R8_SNorm:
                    Gl.VertexAttribFormat(location, 1, VertexAttribType.Byte, true, description.Offset);
                    break;
                case Format.R8_SInt:
                    Gl.VertexAttribIFormat(location, 1, VertexAttribIType.Byte, description.Offset);
                    break;
                case Format.R8_UInt:
                    Gl.VertexAttribIFormat(location, 1, VertexAttribIType.UnsignedByte, description.Offset);
                    break;
                case Format.R8G8_SNorm:
                    Gl.VertexAttribFormat(location, 2, VertexAttribType.Byte, true, description.Offset);
                    break;
                case Format.R8G8_SInt:
                    Gl.VertexAttribIFormat(location, 2, VertexAttribIType.Byte, description.Offset);
                    break;
                case Format.R8G8_UInt:
                    Gl.VertexAttribIFormat(location, 2, VertexAttribIType.UnsignedByte, description.Offset);
                    break;
                case Format.R8G8B8A8_SNorm:
                    Gl.VertexAttribFormat(location, 4, VertexAttribType.Byte, true, description.Offset);
                    break;
                case Format.R8G8B8A8_SInt:
                    Gl.VertexAttribIFormat(location, 4, VertexAttribIType.Byte, description.Offset);
                    break;
                case Format.R8G8B8A8_UInt:
                    Gl.VertexAttribIFormat(location, 4, VertexAttribIType.UnsignedByte, description.Offset);
                    break;
                case Format.R16_UNorm:
                    Gl.VertexAttribFormat(location, 1, VertexAttribType.UnsignedShort, true, description.Offset);
                    break;
                case Format.R16_SNorm:
                    Gl.VertexAttribFormat(location, 1, VertexAttribType.Short, true, description.Offset);
                    break;
                case Format.R16_SInt:
                    Gl.VertexAttribIFormat(location, 1, VertexAttribIType.Short, description.Offset);
                    break;
                case Format.R16_UInt:
                    Gl.VertexAttribIFormat(location, 1, VertexAttribIType.UnsignedShort, description.Offset);
                    break;
                case Format.R16_Float:
                    Gl.VertexAttribFormat(location, 1, VertexAttribType.HalfFloat, false, description.Offset);
                    break;
                case Format.R16G16_UNorm:
                    Gl.VertexAttribFormat(location, 2, VertexAttribType.UnsignedShort, true, description.Offset);
                    break;
                case Format.R16G16_SNorm:
                    Gl.VertexAttribFormat(location, 2, VertexAttribType.Short, true, description.Offset);
                    break;
                case Format.R16G16_SInt:
                    Gl.VertexAttribIFormat(location, 2, VertexAttribIType.Short, description.Offset);
                    break;
                case Format.R16G16_UInt:
                    Gl.VertexAttribIFormat(location, 2, VertexAttribIType.UnsignedShort, description.Offset);
                    break;
                case Format.R16G16_Float:
                    Gl.VertexAttribFormat(location, 2, VertexAttribType.HalfFloat, false, description.Offset);
                    break;
                case Format.R32_SInt:
                    Gl.VertexAttribIFormat(location, 1, VertexAttribIType.Int, description.Offset);
                    break;
                case Format.R32_UInt:
                    Gl.VertexAttribIFormat(location, 1, VertexAttribIType.UnsignedInt, description.Offset);
                    break;
                case Format.R32_Float:
                    Gl.VertexAttribFormat(location, 1, VertexAttribType.Float, false, description.Offset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Gl.VertexBindingDivisor(location, (uint) description.InputType);
            Gl.VertexAttribBinding(location, slot);
            
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