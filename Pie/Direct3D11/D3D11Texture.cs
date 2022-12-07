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

    public static unsafe Texture CreateTexture(TextureDescription description, TextureData* data)
    {
        PieUtils.CheckIfValid(description);
        int sizeMultiplier = PieUtils.GetSizeMultiplier(description.Format);

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
                //Context.UpdateSubresource(new ReadOnlySpan<byte>(data[0].DataPtr, (int) data[0].DataLength), texture, 0, description.Width * sizeMultiplier);
                Context.UpdateSubresource(texture, 0, null, new IntPtr(data[0].DataPtr), description.Width * sizeMultiplier, 0);
                
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
                    TextureData tData = data[i];
                    subresourceDatas[i] = new SubresourceData(tData.DataPtr, description.Width * sizeMultiplier);
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

    public unsafe void Update(int x, int y, uint width, uint height, TextureData data)
    {
        // TODO: Implement texture mapping for fast transfers, i think.
        Context.UpdateSubresource(Texture, 0, new Box(x, y, 0, (int) (x + width), (int) (y + height), 1),
            new IntPtr(data.DataPtr), (int) (width * PieUtils.GetSizeMultiplier(Description.Format)), 0);
    }

    public override void Dispose()
    {
        Texture.Dispose();
        View.Dispose();
    }
}