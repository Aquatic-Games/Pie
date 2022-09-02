using System;
using Vortice.Direct3D11;
using Vortice.Mathematics;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11SamplerState : SamplerState
{
    public readonly ID3D11SamplerState State;
    
    public D3D11SamplerState(SamplerStateDescription description)
    {
        Description = description;
        
        Filter filter = description.Filter switch
        {
            TextureFilter.Anisotropic => Filter.Anisotropic,
            TextureFilter.MinMagMipPoint => Filter.MinMagMipPoint,
            TextureFilter.MinMagPointMipLinear => Filter.MinMagPointMipLinear,
            TextureFilter.MinPointMagLinearMipPoint => Filter.MinPointMagLinearMipPoint,
            TextureFilter.MinPointMagMipLinear => Filter.MinPointMagMipLinear,
            TextureFilter.MinLinearMagMipPoint => Filter.MinLinearMagMipPoint,
            TextureFilter.MinLinearMagPointMipLinear => Filter.MinLinearMagPointMipLinear,
            TextureFilter.MinMagLinearMipPoint => Filter.MinMagLinearMipPoint,
            TextureFilter.MinMagMipLinear => Filter.MinMagMipLinear,
            _ => throw new ArgumentOutOfRangeException()
        };

        SamplerDescription desc = new SamplerDescription()
        {
            Filter = filter,
            AddressU = GetAddressModeFromTextureAddress(description.AddressU),
            AddressV = GetAddressModeFromTextureAddress(description.AddressV),
            AddressW = GetAddressModeFromTextureAddress(description.AddressW),
            MipLODBias = 0,
            MaxAnisotropy = description.MaxAnisotropy,
            ComparisonFunction = ComparisonFunction.LessEqual,
            BorderColor = new Color4(description.BorderColor.R / 255f, description.BorderColor.G / 255f, description.BorderColor.B / 255f, description.BorderColor.A / 255f),
            MinLOD = description.MinLOD,
            MaxLOD = description.MaxLOD
        };

        State = Device.CreateSamplerState(desc);
    }
    
    public override bool IsDisposed { get; protected set; }
    
    public override SamplerStateDescription Description { get; }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        State.Dispose();
    }

    private static TextureAddressMode GetAddressModeFromTextureAddress(TextureAddress address)
    {
        return address switch
        {
            TextureAddress.Repeat => TextureAddressMode.Wrap,
            TextureAddress.Mirror => TextureAddressMode.Mirror,
            TextureAddress.ClampToEdge => TextureAddressMode.Clamp,
            TextureAddress.ClampToBorder => TextureAddressMode.Border,
            _ => throw new ArgumentOutOfRangeException(nameof(address), address, null)
        };
    }
}