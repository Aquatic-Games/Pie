using System;
using static Pie.Direct3D11.DxUtils;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11SamplerState : SamplerState
{
    public ComPtr<ID3D11SamplerState> State;
    
    public D3D11SamplerState(ComPtr<ID3D11Device> device, SamplerStateDescription description)
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

        SamplerDesc desc = new SamplerDesc()
        {
            Filter = filter,
            AddressU = GetAddressModeFromTextureAddress(description.AddressU),
            AddressV = GetAddressModeFromTextureAddress(description.AddressV),
            AddressW = GetAddressModeFromTextureAddress(description.AddressW),
            MipLODBias = 0,
            MaxAnisotropy = (uint) description.MaxAnisotropy,
            ComparisonFunc = Silk.NET.Direct3D11.ComparisonFunc.LessEqual,
            MinLOD = description.MinLOD,
            MaxLOD = description.MaxLOD
        };

        desc.BorderColor[0] = description.BorderColor.R / 255f;
        desc.BorderColor[1] = description.BorderColor.G / 255f;
        desc.BorderColor[2] = description.BorderColor.B / 255f;
        desc.BorderColor[3] = description.BorderColor.A / 255f;

        if (!Succeeded(device.CreateSamplerState(&desc, ref State)))
            throw new PieException("Failed to create sampler state.");
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