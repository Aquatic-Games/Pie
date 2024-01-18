using System;
using OpenTK.Graphics.OpenGL4;

namespace Pie.OpenGL;

internal sealed class GlSamplerState : SamplerState
{
    public readonly int Handle;

    public unsafe GlSamplerState(SamplerStateDescription description)
    {
        Description = description;
        
        Handle = GL.GenSampler();

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
        
        GL.SamplerParameter(Handle, SamplerParameterName.TextureMinFilter, (int) minFilter);
        GL.SamplerParameter(Handle, SamplerParameterName.TextureMagFilter, (int) magFilter);
        GL.SamplerParameter(Handle, SamplerParameterName.TextureWrapS, (int) GetWrapModeFromTextureAddress(description.AddressU));
        GL.SamplerParameter(Handle, SamplerParameterName.TextureWrapT, (int) GetWrapModeFromTextureAddress(description.AddressV));
        GL.SamplerParameter(Handle, SamplerParameterName.TextureWrapR, (int) GetWrapModeFromTextureAddress(description.AddressW));
        GL.SamplerParameter(Handle, SamplerParameterName.TextureLodBias, 0);
        // OpenGL doesn't have a specific anisotropic filter mode, so to make it behave like DirectX we just ignore the
        // given anisotropy if we're not using anisotropic filtering.
        if (description.Filter == TextureFilter.Anisotropic)
            GL.SamplerParameter(Handle, SamplerParameterName.TextureMaxAnisotropyExt, description.MaxAnisotropy);
        GL.SamplerParameter(Handle, SamplerParameterName.TextureCompareFunc, (int) DepthFunction.Lequal);
        float[] bColor =
        {
            description.BorderColor.R / 255f, description.BorderColor.G / 255f, description.BorderColor.B / 255f,
            description.BorderColor.A / 255f
        };
        fixed (float* border = bColor)
            GL.SamplerParameter(Handle, SamplerParameterName.TextureBorderColor, border);
        GL.SamplerParameter(Handle,SamplerParameterName.TextureMinLod, description.MinLOD);
        GL.SamplerParameter(Handle, SamplerParameterName.TextureMaxLod, description.MaxLOD);
    }
    
    public override bool IsDisposed { get; protected set; }
    public override SamplerStateDescription Description { get; }
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        GL.DeleteSampler(Handle);
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