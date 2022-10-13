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

    public static Texture CreateTexture<T>(TextureDescription description, T[] data) where T : unmanaged
    {
        PieUtils.CheckIfValid(description);
        
        if (data != null && description.TextureType != TextureType.Cubemap)
            PieUtils.CheckIfValid(description.Width * description.Height * 4, data.Length);

        Format fmt = PieUtils.ToDxgiFormat(description.Format,
            (description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource);

        BindFlags flags = BindFlags.None;
        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
            flags |= BindFlags.ShaderResource;
        if (((description.Usage & TextureUsage.Framebuffer) == TextureUsage.Framebuffer && description.Format != PixelFormat.D24_UNorm_S8_UInt) || description.MipLevels != 1)
            flags |= BindFlags.RenderTarget;

        if (description.Format == PixelFormat.D24_UNorm_S8_UInt)
            flags |= BindFlags.DepthStencil;

        ID3D11Resource texture;
        ShaderResourceViewDescription svDesc = new ShaderResourceViewDescription()
        {
            Format = fmt,
        };
        
        switch (description.TextureType)
        {
            case TextureType.Texture2D:
                Texture2DDescription desc = new Texture2DDescription()
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

                texture = Device.CreateTexture2D(desc);

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
                    /*svDesc.ViewDimension = ShaderResourceViewDimension.Texture2DArray;
                    svDesc.Texture2DArray = new Texture2DArrayShaderResourceView()
                    {
                        
                    }*/
                    throw new NotImplementedException("Currently texture arrays have not been implemented.");
                }
                
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
                for (int i = 0; i < cDesc.ArraySize; i++)
                {
                    unsafe
                    {
                        CubemapData cData = (CubemapData) (object) data[i];
                        subresourceDatas[i] = new SubresourceData(cData.Data, description.Width * 4 * sizeof(byte));
                    }
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
        
        if (data != null && description.TextureType != TextureType.Cubemap)
            Context.UpdateSubresource(data, texture, 0, description.Width * 4 * sizeof(byte));

        ID3D11ShaderResourceView view = null;

        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
          view = Device.CreateShaderResourceView(texture, svDesc);

        // TODO: Clean up D3D texture bits
        
        return new D3D11Texture(texture, view, new Size(description.Width, description.Height), description);
    }

    public static unsafe Texture CreateTexture(TextureDescription description, IntPtr data)
    {
        // TODO remove *4 and replace with an amount that changes based on the format.
        ReadOnlySpan<byte> bData = new ReadOnlySpan<byte>(data.ToPointer(), description.Width * description.Height * 4);
        return CreateTexture(description, bData.ToArray());
    }
    
    public void Update<T>(int x, int y, uint width, uint height, T[] data) where T : unmanaged
    {
        // TODO: Implement texture mapping for fast transfers, i think.
        Context.UpdateSubresource(data, Texture, 0, (int) width * 4 * sizeof(byte),
            region: new Box(x, y, 0, (int) (x + width), (int) (y + height), 1));
    }

    public override void Dispose()
    {
        Texture.Dispose();
        View.Dispose();
    }
}