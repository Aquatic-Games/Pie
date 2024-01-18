using System.Collections.Generic;
using System.Drawing;
using Vortice.Direct3D11;

namespace Pie.Direct3D11;

internal sealed class D3D11Framebuffer : Framebuffer
{
    public ID3D11RenderTargetView[] Targets;
    public ID3D11DepthStencilView DepthStencil;

    public D3D11Framebuffer(ID3D11Device device, FramebufferAttachment[] attachments)
    {
        int depthCount = 0;

        List<ID3D11RenderTargetView> targets = new List<ID3D11RenderTargetView>();
        foreach (FramebufferAttachment attachment in attachments)
        {
            Vortice.DXGI.Format fmt = attachment.Texture.Description.Format.ToDxgiFormat(false);
            ID3D11Resource texture = ((D3D11Texture) attachment.Texture).Texture;

            switch (attachment.Texture.Description.Format)
            {
                case Format.D32_Float:
                case Format.D16_UNorm:
                case Format.D24_UNorm_S8_UInt:
                    depthCount++;
                    if (depthCount > 1)
                        throw new PieException("Framebuffer cannot have more than one depth stencil attachment.");

                    DepthStencilViewDescription depthDesc = new DepthStencilViewDescription()
                    {
                        ViewDimension = DepthStencilViewDimension.Texture2D,
                        Format = fmt,
                        Texture2D = new Texture2DDepthStencilView()
                        {
                            MipSlice = 0
                        }
                    };

                    device.CreateDepthStencilView(texture, depthDesc);
                    break;
                default:
                    RenderTargetViewDescription viewDesc = new RenderTargetViewDescription()
                    {
                        ViewDimension = RenderTargetViewDimension.Texture2D,
                        Format = fmt,
                        Texture2D = new Texture2DRenderTargetView()
                        {
                            MipSlice = 0
                        }
                    };

                    ID3D11RenderTargetView view = device.CreateRenderTargetView(texture, viewDesc);
                    targets.Add(view);
                    break;
            }
        }

        Targets = targets.ToArray();
    }

    public override bool IsDisposed { get; protected set; }
    
    public override Size Size { get; set; }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;
        
        foreach (ID3D11RenderTargetView view in Targets)
            view.Dispose();
        
        DepthStencil?.Dispose();
    }
}