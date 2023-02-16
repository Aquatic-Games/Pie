using System;
using Silk.NET.OpenGL;
using static Pie.OpenGL.GLGraphicsDevice;

namespace Pie.OpenGL;

internal sealed class GLSamplerState : SamplerState
{
    public readonly uint Handle;

    public unsafe GLSamplerState(SamplerStateDescription description)
    {
        Description = description;
        
        Handle = Gl.GenSampler();

        TextureMinFilter minFilter;
        TextureMagFilter magFilter;

        // this gave me a brain aneurysm
        switch (description.Filter)
        {
            case TextureFilter.Anisotropic:
                minFilter = TextureMinFilter.LinearMipmapLinear;
                magFilter = TextureMagFilter.Linear;
                break;
            case TextureFilter.MinMagMipPoint:
                minFilter = TextureMinFilter.NearestMipmapNearest;
                magFilter = TextureMagFilter.Nearest;
                break;
            case TextureFilter.MinMagPointMipLinear:
                minFilter = TextureMinFilter.NearestMipmapLinear;
                magFilter = TextureMagFilter.Nearest;
                break;
            case TextureFilter.MinPointMagLinearMipPoint:
                minFilter = TextureMinFilter.NearestMipmapNearest;
                magFilter = TextureMagFilter.Linear;
                break;
            case TextureFilter.MinPointMagMipLinear:
                minFilter = TextureMinFilter.NearestMipmapLinear;
                magFilter = TextureMagFilter.Linear;
                break;
            case TextureFilter.MinLinearMagMipPoint:
                minFilter = TextureMinFilter.LinearMipmapNearest;
                magFilter = TextureMagFilter.Nearest;
                break;
            case TextureFilter.MinLinearMagPointMipLinear:
                minFilter = TextureMinFilter.LinearMipmapLinear;
                magFilter = TextureMagFilter.Nearest;
                break;
            case TextureFilter.MinMagLinearMipPoint:
                minFilter = TextureMinFilter.LinearMipmapNearest;
                magFilter = TextureMagFilter.Linear;
                break;
            case TextureFilter.MinMagMipLinear:
                minFilter = TextureMinFilter.LinearMipmapLinear;
                magFilter = TextureMagFilter.Linear;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Gl.SamplerParameter(Handle, SamplerParameterI.MinFilter, (int) minFilter);
        Gl.SamplerParameter(Handle, SamplerParameterI.MagFilter, (int) magFilter);
        Gl.SamplerParameter(Handle, SamplerParameterI.WrapS, (int) GetWrapModeFromTextureAddress(description.AddressU));
        Gl.SamplerParameter(Handle, SamplerParameterI.WrapT, (int) GetWrapModeFromTextureAddress(description.AddressV));
        Gl.SamplerParameter(Handle, SamplerParameterI.WrapR, (int) GetWrapModeFromTextureAddress(description.AddressW));
        Gl.SamplerParameter(Handle, SamplerParameterF.LodBias, 0);
        // OpenGL doesn't have a specific anisotropic filter mode, so to make it behave like DirectX we just ignore the
        // given anisotropy if we're not using anisotropic filtering.
        if (description.Filter == TextureFilter.Anisotropic)
            Gl.SamplerParameter(Handle, SamplerParameterF.MaxAnisotropy, description.MaxAnisotropy);
        Gl.SamplerParameter(Handle, SamplerParameterI.CompareFunc, (int) DepthFunction.Lequal);
        float[] bColor =
        {
            description.BorderColor.R / 255f, description.BorderColor.G / 255f, description.BorderColor.B / 255f,
            description.BorderColor.A / 255f
        };
        fixed (float* border = bColor)
            Gl.SamplerParameter(Handle, SamplerParameterF.BorderColor, border);
        Gl.SamplerParameter(Handle, SamplerParameterF.MinLod, description.MinLOD);
        Gl.SamplerParameter(Handle, SamplerParameterF.MaxLod, description.MaxLOD);
    }
    
    public override bool IsDisposed { get; protected set; }
    public override SamplerStateDescription Description { get; }
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        Gl.DeleteSampler(Handle);
    }

    private static TextureWrapMode GetWrapModeFromTextureAddress(TextureAddress address)
    {
        return address switch
        {
            TextureAddress.Repeat => TextureWrapMode.Repeat,
            TextureAddress.Mirror => TextureWrapMode.MirroredRepeat,
            TextureAddress.ClampToEdge => TextureWrapMode.ClampToEdge,
            TextureAddress.ClampToBorder => TextureWrapMode.ClampToBorder,
            _ => throw new ArgumentOutOfRangeException(nameof(address), address, null)
        };
    }
}