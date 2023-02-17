using System;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using static Pie.Direct3D11.D3D11GraphicsDevice;
using Size = System.Drawing.Size;

namespace Pie.Direct3D11;

internal sealed class D3D11Texture : Texture
{
    public ID3D11Resource Texture;
    public ID3D11ShaderResourceView View;

    public override bool IsDisposed { get; protected set; }
    
    public override Size Size { get; set; }
    public override TextureDescription Description { get; set; }

    public D3D11Texture(ID3D11Resource texture, ID3D11ShaderResourceView view, Size size, TextureDescription description)
    {
        Texture = texture;
        View = view;
        Size = size;
        Description = description;
    }

    public static unsafe Texture CreateTexture(TextureDescription description, void* data)
    {
        PieUtils.CheckIfValid(description);
        int pitch = PieUtils.CalculatePitch(description.Format, description.Width, out int bpp);

        Vortice.DXGI.Format fmt = PieUtils.ToDxgiFormat(description.Format,
            (description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource);

        BindFlags flags = BindFlags.None;
        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
            flags |= BindFlags.ShaderResource;
        if (((description.Usage & TextureUsage.Framebuffer) == TextureUsage.Framebuffer && description.Format != Format.D24_UNorm_S8_UInt) || description.MipLevels != 1)
            flags |= BindFlags.RenderTarget;

        if (description.Format == Format.D24_UNorm_S8_UInt)
            flags |= BindFlags.DepthStencil;
        ID3D11Resource texture;
        ShaderResourceViewDescription svDesc = new ShaderResourceViewDescription()
        {
            Format = fmt,
        };
        
        switch (description.TextureType)
        {
            case TextureType.Texture1D:
                Texture1DDescription desc1d = new Texture1DDescription()
                {
                    Width = description.Width,
                    Format = fmt,
                    MipLevels = description.MipLevels,
                    ArraySize = description.ArraySize,
                    //Usage = description.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
                    Usage = ResourceUsage.Default,
                    BindFlags = flags,
                    CPUAccessFlags = CpuAccessFlags.None,
                    MiscFlags = description.MipLevels != 1 ? ResourceOptionFlags.GenerateMips : ResourceOptionFlags.None
                };

                texture = Device.CreateTexture1D(desc1d);
                if (data != null)
                {
                    Context.UpdateSubresource(texture, 0, null, new IntPtr(data), pitch,
                        description.ArraySize > 1 ? description.ArraySize * bpp / 8 : 0);
                }

                if (description.ArraySize == 1)
                {
                    svDesc.ViewDimension = ShaderResourceViewDimension.Texture1D;
                    svDesc.Texture1D = new Texture1DShaderResourceView()
                    {
                        MipLevels = -1,
                        MostDetailedMip = 0
                    };
                }
                else
                {
                    svDesc.ViewDimension = ShaderResourceViewDimension.Texture2DArray;
                    svDesc.Texture1DArray = new Texture1DArrayShaderResourceView()
                    {
                        MipLevels = -1,
                        MostDetailedMip = 0,
                        ArraySize = description.ArraySize,
                        FirstArraySlice = 0
                    };
                }
                break;
            case TextureType.Texture2D:
                Texture2DDescription desc2d = new Texture2DDescription()
                {
                    Width = description.Width,
                    Height = description.Height,
                    Format = fmt,
                    MipLevels = description.MipLevels,
                    ArraySize = description.ArraySize,
                    SampleDescription = new SampleDescription(1, 0),
                    //Usage = description.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
                    Usage = ResourceUsage.Default,
                    BindFlags = flags,
                    CPUAccessFlags = CpuAccessFlags.None,
                    MiscFlags = description.MipLevels != 1 ? ResourceOptionFlags.GenerateMips : ResourceOptionFlags.None
                };

                texture = Device.CreateTexture2D(desc2d);
                if (data != null)
                {
                    Context.UpdateSubresource(texture, 0, null, new IntPtr(data), pitch,
                        description.ArraySize > 1 ? description.ArraySize * bpp / 8 : 0);
                }

                if (description.ArraySize == 1)
                {
                    svDesc.ViewDimension = ShaderResourceViewDimension.Texture2D;
                    svDesc.Texture2D = new Texture2DShaderResourceView()
                    {
                        MipLevels = -1,
                        MostDetailedMip = 0
                    };
                }
                else
                {
                    svDesc.ViewDimension = ShaderResourceViewDimension.Texture2DArray;
                    svDesc.Texture2DArray = new Texture2DArrayShaderResourceView()
                    {
                        MipLevels = -1,
                        MostDetailedMip = 0,
                        ArraySize = description.ArraySize,
                        FirstArraySlice = 0
                    };
                }
                
                break;
            case TextureType.Texture3D:
                Texture3DDescription desc3d = new Texture3DDescription()
                {
                    Width = description.Width,
                    Height = description.Height,
                    Depth = description.Depth,
                    Format = fmt,
                    MipLevels = description.MipLevels,
                    //Usage = description.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
                    Usage = ResourceUsage.Default,
                    BindFlags = flags,
                    CPUAccessFlags = CpuAccessFlags.None,
                    MiscFlags = description.MipLevels != 1 ? ResourceOptionFlags.GenerateMips : ResourceOptionFlags.None
                };

                texture = Device.CreateTexture3D(desc3d);
                if (data != null)
                {
                    Context.UpdateSubresource(texture, 0, null, new IntPtr(data), pitch,
                        description.Depth * bpp / 8);
                }

                if (description.ArraySize > 1)
                    throw new NotSupportedException("Texture3D arrays are not supported.");
                
                svDesc.ViewDimension = ShaderResourceViewDimension.Texture3D;
                svDesc.Texture3D = new Texture3DShaderResourceView()
                {
                    MipLevels = -1,
                    MostDetailedMip = 0
                };
                break;
            case TextureType.Cubemap:
                Texture2DDescription cDesc = new Texture2DDescription()
                {
                    Width = description.Width,
                    Height = description.Height,
                    Format = fmt,
                    MipLevels = description.MipLevels,
                    ArraySize = description.ArraySize * 6,
                    SampleDescription = new SampleDescription(1, 0),
                    //Usage = description.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
                    Usage = ResourceUsage.Default,
                    BindFlags = flags,
                    CPUAccessFlags = CpuAccessFlags.None,
                    MiscFlags = ResourceOptionFlags.TextureCube | (description.MipLevels != 1 ? ResourceOptionFlags.GenerateMips : ResourceOptionFlags.None)
                };

                SubresourceData[] subresourceDatas = new SubresourceData[cDesc.ArraySize];
                int size = description.Width * description.Height * bpp / 8;
                for (int i = 0; i < cDesc.ArraySize; i++)
                {
                    void* ptr = (byte*) data + i * size;
                    subresourceDatas[i] = new SubresourceData(ptr, pitch);
                }

                texture = Device.CreateTexture2D(cDesc, subresourceDatas);
                
                svDesc.ViewDimension = ShaderResourceViewDimension.TextureCube;
                svDesc.Texture2D = new Texture2DShaderResourceView()
                {
                    MipLevels = -1,
                    MostDetailedMip = 0
                };
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ID3D11ShaderResourceView view = null;

        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
          view = Device.CreateShaderResourceView(texture, svDesc);

        // TODO: Clean up D3D texture bits
        
        return new D3D11Texture(texture, view, new Size(description.Width, description.Height), description);
    }

    public unsafe void Update(int x, int y, uint width, uint height, void* data)
    {
        // TODO: Implement texture mapping for fast transfers, i think
        Context.UpdateSubresource(Texture, 0, new Box(x, y, 0, (int) (x + width), (int) (y + height), 1),
            new IntPtr(data), PieUtils.CalculatePitch(Description.Format, (int) width, out _), 0);
    }

    public override void Dispose()
    {
        Texture.Dispose();
        View.Dispose();
    }
}