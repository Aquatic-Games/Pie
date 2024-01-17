using System.Collections.Generic;
using System.Drawing;
using static Pie.Direct3D11.DxUtils;

namespace Pie.Direct3D11;

internal sealed unsafe class D3D11Framebuffer : Framebuffer
{
    public ComPtr<ID3D11RenderTargetView>[] Targets;
    public ComPtr<ID3D11DepthStencilView> DepthStencil;

    public D3D11Framebuffer(ComPtr<ID3D11Device> device, FramebufferAttachment[] attachments)
    {
        int depthCount = 0;

        List<ComPtr<ID3D11RenderTargetView>> targets = new List<ComPtr<ID3D11RenderTargetView>>();
        foreach (FramebufferAttachment attachment in attachments)
        {
            Silk.NET.DXGI.Format fmt = attachment.Texture.Description.Format.ToDxgiFormat(false);

            switch (attachment.Texture.Description.Format)
            {
                case Format.D32_Float:
                case Format.D16_UNorm:
                case Format.D24_UNorm_S8_UInt:
                    depthCount++;
                    if (depthCount > 1)
                        throw new PieException("Framebuffer cannot have more than one depth stencil attachment.");

                    DepthStencilViewDesc depthDesc = new DepthStencilViewDesc()
                    {
                        ViewDimension = DsvDimension.Texture2D,
                        Format = fmt,
                        Texture2D = new Tex2DDsv()
                        {
                            MipSlice = 0
                        }
                    };

                    if (!Succeeded(device.CreateDepthStencilView(((D3D11Texture) attachment.Texture).Texture,
                            &depthDesc, ref DepthStencil)))
                        throw new PieException("Failed to create depth stencil view.");
                    break;
                default:

                    RenderTargetViewDesc viewDesc = new RenderTargetViewDesc()
                    {
                        ViewDimension = RtvDimension.Texture2D,
                        Format = fmt,
                        Texture2D = new Tex2DRtv()
                        {
                            MipSlice = 0
                        }
                    };

                    ComPtr<ID3D11RenderTargetView> targetView = null;
                    device.CreateRenderTargetView(((D3D11Texture) attachment.Texture).Texture, &viewDesc,
                        ref targetView);

                    targets.Add(targetView);
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
        
        foreach (ComPtr<ID3D11RenderTargetView> view in Targets)
            view.Dispose();
        
        if (DepthStencil.Handle != null)
            DepthStencil.Dispose();
    }
}