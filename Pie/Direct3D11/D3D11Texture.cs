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

    public static unsafe Texture CreateTexture<T>(TextureDescription description, T[] data) where T : unmanaged
    {
        PieUtils.CheckIfValid(description);
        if (data != null)
            PieUtils.CheckIfValid(description.Width * description.Height * 4, data.Length);
        
        Format fmt = PieUtils.ToDxgiFormat(description.Format,
            (description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource);

        BindFlags flags = BindFlags.None;
        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
            flags |= BindFlags.ShaderResource;
        if ((description.Usage & TextureUsage.Framebuffer) == TextureUsage.Framebuffer || description.Mipmap)
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
                    MipLevels = description.Mipmap ? 0 : 1,
                    ArraySize = description.ArraySize,
                    SampleDescription = new SampleDescription(1, 0),
                    //Usage = description.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
                    Usage = ResourceUsage.Default,
                    BindFlags = flags,
                    CPUAccessFlags = CpuAccessFlags.None,
                    MiscFlags = description.Mipmap ? ResourceOptionFlags.GenerateMips : ResourceOptionFlags.None
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
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (data != null)
            Context.UpdateSubresource(data, texture, 0, description.Width * 4 * sizeof(byte));

        ID3D11ShaderResourceView view = null;

        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
          view = Device.CreateShaderResourceView(texture, svDesc);
        if (description.Mipmap)
            Context.GenerateMips(view);

        // TODO: Clean up D3D texture bits
        
        return new D3D11Texture(texture, view, new Size(description.Width, description.Height), description);
    }
    
    public void Update<T>(int x, int y, uint width, uint height, T[] data) where T : unmanaged
    {
        // TODO: Implement texture mapping for fast transfers, i think.
        Context.UpdateSubresource(data, Texture, 0, (int) width * 4 * sizeof(byte),
            region: new Box(x, 0, 0, (int) (x + width), (int) (y + height), 0));
    }

    public override void Dispose()
    {
        Texture.Dispose();
        View.Dispose();
    }
}