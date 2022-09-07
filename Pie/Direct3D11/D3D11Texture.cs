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
    private ID3D11Texture2D _texture;
    public ID3D11ShaderResourceView View;

    public override bool IsDisposed { get; protected set; }
    
    public override Size Size { get; set; }

    public D3D11Texture(ID3D11Texture2D texture, ID3D11ShaderResourceView view, Size size)
    {
        _texture = texture;
        View = view;
        Size = size;
    }

    public static unsafe Texture CreateTexture<T>(int width, int height, PixelFormat format, T[] data, bool mipmap) where T : unmanaged
    {
        Format fmt = format switch
        {
            PixelFormat.R8G8B8A8_UNorm => Format.R8G8B8A8_UNorm,
            PixelFormat.B8G8R8A8_UNorm => Format.B8G8R8A8_UNorm,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        Texture2DDescription description = new Texture2DDescription()
        {
            Width = width,
            Height = height,
            Format = fmt,
            MipLevels = mipmap ? 0 : 1,
            ArraySize = 1,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
            CPUAccessFlags = CpuAccessFlags.None,
            MiscFlags = mipmap ? ResourceOptionFlags.GenerateMips : ResourceOptionFlags.None
        };

        ID3D11Texture2D texture = Device.CreateTexture2D(description);
        Context.UpdateSubresource(data, texture, 0, width * 4 * sizeof(byte));

        ShaderResourceViewDescription svDesc = new ShaderResourceViewDescription()
        {
            Format = description.Format,
            ViewDimension = ShaderResourceViewDimension.Texture2D,
            Texture2D = new Texture2DShaderResourceView()
            {
                MipLevels = -1,
                MostDetailedMip = 0
            }
        };
        ID3D11ShaderResourceView view = Device.CreateShaderResourceView(texture, svDesc);
        if (mipmap)
            Context.GenerateMips(view);

        return new D3D11Texture(texture, view, new Size(width, height));
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