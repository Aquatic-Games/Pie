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
    private ID3D11Resource _texture;
    public ID3D11ShaderResourceView View;

    public override bool IsDisposed { get; protected set; }
    
    public override Size Size { get; set; }
    public override TextureDescription Description { get; set; }

    public D3D11Texture(ID3D11Resource texture, ID3D11ShaderResourceView view, Size size, TextureDescription description)
    {
        _texture = texture;
        View = view;
        Size = size;
        Description = description;
    }

    public static unsafe Texture CreateTexture<T>(TextureDescription description, T[] data) where T : unmanaged
    {
        if (description.ArraySize < 1)
            throw new PieException("Array size must be at least 1.");

        int bytesExpected = description.Width * description.Height * 4;
        if (data != null && data.Length != bytesExpected)
            throw new PieException($"{bytesExpected} bytes expected, {data.Length} bytes received.");
        
        Format fmt = description.Format switch
        {
            PixelFormat.R8G8B8A8_UNorm => Format.R8G8B8A8_UNorm,
            PixelFormat.B8G8R8A8_UNorm => Format.B8G8R8A8_UNorm,
            _ => throw new ArgumentOutOfRangeException()
        };

        BindFlags bFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;

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
                    BindFlags = bFlags,
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
        Context.UpdateSubresource(data, texture, 0, description.Width * 4 * sizeof(byte));


        ID3D11ShaderResourceView view = Device.CreateShaderResourceView(texture, svDesc);
        if (description.Mipmap)
            Context.GenerateMips(view);

        return new D3D11Texture(texture, view, new Size(description.Width, description.Height), description);
    }
    
    public void Update<T>(int x, int y, uint width, uint height, T[] data) where T : unmanaged
    {
        // TODO: Implement texture mapping for fast transfers, i think.
        Context.UpdateSubresource(data, _texture, 0, (int) width * 4 * sizeof(byte),
            region: new Box(x, 0, 0, (int) (x + width), (int) (y + height), 0));
    }

    public override void Dispose()
    {
        _texture.Dispose();
        View.Dispose();
    }
}