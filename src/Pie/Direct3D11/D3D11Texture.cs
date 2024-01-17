using System;
using static Pie.Direct3D11.DxUtils;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11Texture : Texture
{
    private ComPtr<ID3D11DeviceContext> _context;

    public ComPtr<ID3D11Resource> Texture;
    public ComPtr<ID3D11ShaderResourceView> View;

    public override bool IsDisposed { get; protected set; }
    
    public override TextureDescription Description { get; set; }

    public D3D11Texture(ComPtr<ID3D11Device> device, ComPtr<ID3D11DeviceContext> context, in TextureDescription description, void* data)
    {
        _context = context;
        
        Description = description;
        
        int pitch = PieUtils.CalculatePitch(description.Format, description.Width, out int bpp);

        Silk.NET.DXGI.Format fmt =
            description.Format.ToDxgiFormat((description.Usage & TextureUsage.ShaderResource) ==
                                            TextureUsage.ShaderResource);
        
        BindFlag flags = BindFlag.None;
        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
            flags |= BindFlag.ShaderResource;

        if (description.Format is Format.D24_UNorm_S8_UInt or Format.D32_Float or Format.D16_UNorm)
            flags |= BindFlag.DepthStencil;
        else if ((description.Usage & TextureUsage.Framebuffer) == TextureUsage.Framebuffer || description.MipLevels == 0)
            flags |= BindFlag.RenderTarget;

        Silk.NET.DXGI.Format svFmt = description.Format.ToDxgiFormat(false);

        svFmt = svFmt switch
        {
            Silk.NET.DXGI.Format.FormatD32Float => Silk.NET.DXGI.Format.FormatR32Float,
            Silk.NET.DXGI.Format.FormatD16Unorm => Silk.NET.DXGI.Format.FormatR16Unorm,
            Silk.NET.DXGI.Format.FormatD24UnormS8Uint => Silk.NET.DXGI.Format.FormatR24UnormX8Typeless,
            _ => svFmt
        };
        
        ShaderResourceViewDesc svDesc = new ShaderResourceViewDesc()
        {
            Format = svFmt,
        };
        
        int mipLevels = description.MipLevels == 0
            ? PieUtils.CalculateMipLevels(description.Width, description.Height, description.Depth)
            : description.MipLevels;

        switch (description.TextureType)
        {
            case TextureType.Texture1D:
                Texture1DDesc desc1d = new Texture1DDesc()
                {
                    Width = (uint) description.Width,
                    Format = fmt,
                    MipLevels = (uint) mipLevels,
                    ArraySize = (uint) description.ArraySize,
                    Usage = Usage.Default,
                    BindFlags = (uint) flags,
                    CPUAccessFlags = (uint) CpuAccessFlag.None,
                    MiscFlags = (uint) (description.MipLevels == 0 ? ResourceMiscFlag.GenerateMips : ResourceMiscFlag.None)
                };

                ComPtr<ID3D11Texture1D> tex1d = null;
                if (!Succeeded(device.CreateTexture1D(&desc1d, null, ref tex1d)))
                    throw new PieException("Failed to create 1D texture.");
                Texture = ComPtr.Downcast<ID3D11Texture1D, ID3D11Resource>(tex1d);

                if (description.ArraySize == 1)
                {
                    svDesc.ViewDimension = D3DSrvDimension.D3DSrvDimensionTexture1D;
                    svDesc.Texture1D = new Tex1DSrv()
                    {
                        // MipLevels = -1
                        MipLevels = uint.MaxValue,
                        MostDetailedMip = 0
                    };
                }
                else
                {
                    svDesc.ViewDimension = D3DSrvDimension.D3DSrvDimensionTexture1Darray;
                    svDesc.Texture1DArray = new Tex1DArraySrv()
                    {
                        ArraySize = (uint) description.ArraySize,
                        FirstArraySlice = 0,
                        // MipLevels = -1
                        MipLevels = uint.MaxValue,
                        MostDetailedMip = 0
                    };
                }

                break;
            case TextureType.Texture2D:
                Texture2DDesc desc2d = new Texture2DDesc()
                {
                    Width = (uint) description.Width,
                    Height = (uint) description.Height,
                    Format = fmt,
                    MipLevels = (uint) mipLevels,
                    ArraySize = (uint) description.ArraySize,
                    SampleDesc = new SampleDesc(1, 0),
                    //Usage = description.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
                    Usage = Usage.Default,
                    BindFlags = (uint) flags,
                    CPUAccessFlags = (uint) CpuAccessFlag.None,
                    MiscFlags = (uint) (description.MipLevels == 0 ? ResourceMiscFlag.GenerateMips : ResourceMiscFlag.None)
                };

                ComPtr<ID3D11Texture2D> tex2d = null;
                if (!Succeeded(device.CreateTexture2D(&desc2d, null, ref tex2d)))
                    throw new PieException("Failed to create 2D texture.");
                Texture = ComPtr.Downcast<ID3D11Texture2D, ID3D11Resource>(tex2d);

                if (description.ArraySize == 1)
                {
                    svDesc.ViewDimension = D3DSrvDimension.D3DSrvDimensionTexture2D;
                    svDesc.Texture2D = new Tex2DSrv()
                    {
                        // MipLevels = -1
                        MipLevels = uint.MaxValue,
                        MostDetailedMip = 0
                    };
                }
                else
                {
                    svDesc.ViewDimension = D3DSrvDimension.D3DSrvDimensionTexture2Darray;
                    svDesc.Texture2DArray = new Tex2DArraySrv()
                    {
                        // MipLevels = -1
                        MipLevels = uint.MaxValue,
                        MostDetailedMip = 0,
                        ArraySize = (uint) description.ArraySize,
                        FirstArraySlice = 0
                    };
                }
                
                break;
            case TextureType.Texture3D:
                Texture3DDesc desc3d = new Texture3DDesc()
                {
                    Width = (uint) description.Width,
                    Height = (uint) description.Height,
                    Depth = (uint) description.Depth,
                    Format = fmt,
                    MipLevels = (uint) mipLevels,
                    Usage = Usage.Default,
                    BindFlags = (uint) flags,
                    CPUAccessFlags = (uint) CpuAccessFlag.None,
                    MiscFlags = (uint) (description.MipLevels == 0 ? ResourceMiscFlag.GenerateMips : ResourceMiscFlag.None)
                };

                ComPtr<ID3D11Texture3D> tex3d = null;
                if (!Succeeded(device.CreateTexture3D(&desc3d, null, ref tex3d)))
                    throw new PieException("Failed to create 3D texture.");
                Texture = ComPtr.Downcast<ID3D11Texture3D, ID3D11Resource>(tex3d);
                
                if (description.ArraySize == 1)
                {
                    svDesc.ViewDimension = D3DSrvDimension.D3DSrvDimensionTexture3D;
                    svDesc.Texture3D = new Tex3DSrv()
                    {
                        // MipLevels = -1
                        MipLevels = uint.MaxValue,
                        MostDetailedMip = 0
                    };
                }
                else
                    throw new NotSupportedException("3D texture arrays are not supported.");
                break;
            case TextureType.Cubemap:
                Texture2DDesc desc2dcube = new Texture2DDesc()
                {
                    Width = (uint) description.Width,
                    Height = (uint) description.Height,
                    Format = fmt,
                    MipLevels = (uint) mipLevels,
                    ArraySize = (uint) description.ArraySize * 6, // Multiply by 6 for cubemap
                    SampleDesc = new SampleDesc(1, 0),
                    //Usage = description.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
                    Usage = Usage.Default,
                    BindFlags = (uint) flags,
                    CPUAccessFlags = (uint) CpuAccessFlag.None,
                    MiscFlags = (uint) (ResourceMiscFlag.Texturecube | (description.MipLevels == 0 ? ResourceMiscFlag.GenerateMips : ResourceMiscFlag.None))
                };

                ComPtr<ID3D11Texture2D> texCube = null;
                if (!Succeeded(device.CreateTexture2D(&desc2dcube, null, ref texCube)))
                    throw new PieException("Failed to create Cubemap texture.");
                Texture = ComPtr.Downcast<ID3D11Texture2D, ID3D11Resource>(texCube);

                if (description.ArraySize == 1)
                {
                    svDesc.ViewDimension = D3DSrvDimension.D3DSrvDimensionTexturecube;
                    svDesc.TextureCube = new TexcubeSrv()
                    {
                        // MipLevels = -1
                        MipLevels = uint.MaxValue,
                        MostDetailedMip = 0
                    };
                }
                else
                    throw new NotImplementedException("Cubemap arrays are not currently supported.");

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        // Now upload texture data assuming it is not null.
        if (data != null)
        {
            // The current offset in bytes to look at the data.
            uint currentOffset = 0;

            for (int a = 0; a < description.ArraySize * (description.TextureType == TextureType.Cubemap ? 6 : 1); a++)
            {
                int width = description.Width;
                // While width must always have a width >= 1, height and depth may not always. If the height or depth
                // are 0, this will cause the size calculation to fail, so we must set it to 1 here.
                int height = PieUtils.Max(description.Height, 1);
                int depth = PieUtils.Max(description.Depth, 1);

                // The loop must run at least once, even if the mip levels are 0.
                for (int i = 0; i < PieUtils.Max(1, description.MipLevels); i++)
                {
                    uint currSize = (uint) (width * height * depth * (bpp / 8f));

                    int rowPitch = PieUtils.CalculatePitch(description.Format, width, out _);
                    int depthPitch = PieUtils.CalculatePitch(description.Format, depth, out _);

                    context.UpdateSubresource(Texture, CalcSubresource((uint) i, (uint) a, (uint) mipLevels), null,
                        (byte*) data + currentOffset, (uint) rowPitch, (uint) depthPitch);

                    currentOffset += currSize;

                    // Divide the width and height by 2 for each mip level.
                    width /= 2;
                    height /= 2;
                    depth /= 2;

                    if (width < 1)
                        width = 1;
                    if (height < 1)
                        height = 1;
                    if (depth < 1)
                        depth = 1;
                }
            }
        }

        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
        {
            if (!Succeeded(device.CreateShaderResourceView(Texture, &svDesc, ref View)))
                throw new PieException("Failed to create shader resource view.");
        }
    }

    public unsafe void Update(int x, int y, int z, int width, int height, int depth, int mipLevel, int arrayIndex, void* data)
    {
        TextureDescription description = Description;

        uint subresource = CalcSubresource((uint) mipLevel, (uint) arrayIndex,
            (uint) (description.MipLevels == 0
                ? PieUtils.CalculateMipLevels(description.Width, description.Height, description.Depth)
                : description.MipLevels));

        int rowPitch = PieUtils.CalculatePitch(description.Format, width, out _);
        int depthPitch = PieUtils.CalculatePitch(description.Format, depth, out _);

        Box box = new Box((uint) x, (uint) y, (uint) z, (uint) (x + width), (uint) (y + height),
            (uint) (z + depth + 1));
        
        _context.UpdateSubresource(Texture, subresource, box, data, (uint) rowPitch, (uint) depthPitch);
    }

    public override void Dispose()
    {
        Texture.Dispose();
        View.Dispose();
    }

    internal override MappedSubresource Map(MapMode mode)
    {
        throw new NotImplementedException();
    }

    internal override void Unmap()
    {
        throw new NotImplementedException();
    }
}