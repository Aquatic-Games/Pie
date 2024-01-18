using System;
using OpenTK.Graphics.OpenGL4;

namespace Pie.OpenGL;

internal sealed class GlInputLayout : InputLayout
{
    private readonly InputLayoutDescription[] _descriptions;

    public readonly int VertexArray;

    public GlInputLayout(InputLayoutDescription[] descriptions)
    {
        _descriptions = descriptions;

        VertexArray = GL.GenVertexArray();
        
        uint lastSlot = uint.MaxValue;
        foreach (InputLayoutDescription desc in descriptions)
        {
            // This isn't great, if someone defines slots in weird orders this will call Set multiple times
            // But you really shouldn't do that, so I'm not overly bothered...
            if (desc.Slot == lastSlot)
                continue;
            lastSlot = desc.Slot;
            
            Set(desc.Slot);
        }
    }

    public unsafe void Set(uint slot)
    {
        GL.BindVertexArray(VertexArray);
        
        uint location = 0;
        foreach (InputLayoutDescription description in _descriptions)
        {
            if (description.Slot != slot)
            {
                location++;
                continue;
            }
            
            GL.EnableVertexAttribArray(location);
            
            switch (description.Format)
            {
                case Format.R8_UNorm:
                    GL.VertexAttribFormat(location, 1, VertexAttribType.UnsignedByte, true, description.Offset);
                    break;
                case Format.R8G8_UNorm:
                    GL.VertexAttribFormat(location, 2, VertexAttribType.UnsignedByte, true, description.Offset);
                    break;
                case Format.R8G8B8A8_UNorm:
                    GL.VertexAttribFormat(location, 4, VertexAttribType.UnsignedByte, true, description.Offset);
                    break;
                case Format.B8G8R8A8_UNorm:
                    // According to the docs, the size parameter can be GL_BGRA. I'm not convinced I've implemented this
                    // correctly, but I don't really have anything to check.
                    GL.VertexAttribFormat(location, (int) All.Bgra, VertexAttribType.UnsignedByte, true, description.Offset);
                    break;
                case Format.R16G16B16A16_UNorm:
                    GL.VertexAttribFormat(location, 4, VertexAttribType.UnsignedShort, true, description.Offset);
                    break;
                case Format.R16G16B16A16_SNorm:
                    GL.VertexAttribFormat(location, 4, VertexAttribType.Short, true, description.Offset);
                    break;
                case Format.R16G16B16A16_SInt:
                    GL.VertexAttribIFormat(location, 4, VertexAttribIntegerType.Short, description.Offset);
                    break;
                case Format.R16G16B16A16_UInt:
                    GL.VertexAttribIFormat(location, 4, VertexAttribIntegerType.UnsignedShort, description.Offset);
                    break;
                case Format.R16G16B16A16_Float:
                    GL.VertexAttribFormat(location, 4, VertexAttribType.HalfFloat, false, description.Offset);
                    break;
                case Format.R32G32_SInt:
                    GL.VertexAttribIFormat(location, 2, VertexAttribIntegerType.Int, description.Offset);
                    break;
                case Format.R32G32_UInt:
                    GL.VertexAttribIFormat(location, 2, VertexAttribIntegerType.UnsignedInt, description.Offset);
                    break;
                case Format.R32G32_Float:
                    GL.VertexAttribFormat(location, 2, VertexAttribType.Float, false, description.Offset);
                    break;
                case Format.R32G32B32_SInt:
                    GL.VertexAttribIFormat(location, 3, VertexAttribIntegerType.Int, description.Offset);
                    break;
                case Format.R32G32B32_UInt:
                    GL.VertexAttribIFormat(location, 3, VertexAttribIntegerType.UnsignedInt, description.Offset);
                    break;
                case Format.R32G32B32_Float:
                    GL.VertexAttribFormat(location, 3, VertexAttribType.Float, false, description.Offset);
                    break;
                case Format.R32G32B32A32_SInt:
                    GL.VertexAttribIFormat(location, 4, VertexAttribIntegerType.Int, description.Offset);
                    break;
                case Format.R32G32B32A32_UInt:
                    GL.VertexAttribIFormat(location, 4, VertexAttribIntegerType.UnsignedInt, description.Offset);
                    break;
                case Format.R32G32B32A32_Float:
                    GL.VertexAttribFormat(location, 4, VertexAttribType.Float, false, description.Offset);
                    break;
                case Format.D24_UNorm_S8_UInt:
                    throw new NotSupportedException("Depth-stencil format not supported for input layouts.");
                case Format.R8_SNorm:
                    GL.VertexAttribFormat(location, 1, VertexAttribType.Byte, true, description.Offset);
                    break;
                case Format.R8_SInt:
                    GL.VertexAttribIFormat(location, 1, VertexAttribIntegerType.Byte, description.Offset);
                    break;
                case Format.R8_UInt:
                    GL.VertexAttribIFormat(location, 1, VertexAttribIntegerType.UnsignedByte, description.Offset);
                    break;
                case Format.R8G8_SNorm:
                    GL.VertexAttribFormat(location, 2, VertexAttribType.Byte, true, description.Offset);
                    break;
                case Format.R8G8_SInt:
                    GL.VertexAttribIFormat(location, 2, VertexAttribIntegerType.Byte, description.Offset);
                    break;
                case Format.R8G8_UInt:
                    GL.VertexAttribIFormat(location, 2, VertexAttribIntegerType.UnsignedByte, description.Offset);
                    break;
                case Format.R8G8B8A8_SNorm:
                    GL.VertexAttribFormat(location, 4, VertexAttribType.Byte, true, description.Offset);
                    break;
                case Format.R8G8B8A8_SInt:
                    GL.VertexAttribIFormat(location, 4, VertexAttribIntegerType.Byte, description.Offset);
                    break;
                case Format.R8G8B8A8_UInt:
                    GL.VertexAttribIFormat(location, 4, VertexAttribIntegerType.UnsignedByte, description.Offset);
                    break;
                case Format.R16_UNorm:
                    GL.VertexAttribFormat(location, 1, VertexAttribType.UnsignedShort, true, description.Offset);
                    break;
                case Format.R16_SNorm:
                    GL.VertexAttribFormat(location, 1, VertexAttribType.Short, true, description.Offset);
                    break;
                case Format.R16_SInt:
                    GL.VertexAttribIFormat(location, 1, VertexAttribIntegerType.Short, description.Offset);
                    break;
                case Format.R16_UInt:
                    GL.VertexAttribIFormat(location, 1, VertexAttribIntegerType.UnsignedShort, description.Offset);
                    break;
                case Format.R16_Float:
                    GL.VertexAttribFormat(location, 1, VertexAttribType.HalfFloat, false, description.Offset);
                    break;
                case Format.R16G16_UNorm:
                    GL.VertexAttribFormat(location, 2, VertexAttribType.UnsignedShort, true, description.Offset);
                    break;
                case Format.R16G16_SNorm:
                    GL.VertexAttribFormat(location, 2, VertexAttribType.Short, true, description.Offset);
                    break;
                case Format.R16G16_SInt:
                    GL.VertexAttribIFormat(location, 2, VertexAttribIntegerType.Short, description.Offset);
                    break;
                case Format.R16G16_UInt:
                    GL.VertexAttribIFormat(location, 2, VertexAttribIntegerType.UnsignedShort, description.Offset);
                    break;
                case Format.R16G16_Float:
                    GL.VertexAttribFormat(location, 2, VertexAttribType.HalfFloat, false, description.Offset);
                    break;
                case Format.R32_SInt:
                    GL.VertexAttribIFormat(location, 1, VertexAttribIntegerType.Int, description.Offset);
                    break;
                case Format.R32_UInt:
                    GL.VertexAttribIFormat(location, 1, VertexAttribIntegerType.UnsignedInt, description.Offset);
                    break;
                case Format.R32_Float:
                    GL.VertexAttribFormat(location, 1, VertexAttribType.Float, false, description.Offset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            GL.VertexBindingDivisor(slot, (uint) description.InputType);
            GL.VertexAttribBinding(location, slot);
            
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
        
        GL.DeleteVertexArray(VertexArray);
    }
}