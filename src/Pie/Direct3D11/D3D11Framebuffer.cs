using System;
using System.Collections.Generic;
using System.Drawing;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Vortice.Direct3D11;
using Vortice.DXGI;
using static Pie.Direct3D11.D3D11GraphicsDevice;

namespace Pie.Direct3D11;

internal sealed class D3D11Framebuffer : Framebuffer
{
    public ComPtr<ID3D11RenderTargetView>[] Targets;
    public ComPtr<ID3D11DepthStencilView> DepthStencil;

    public D3D11Framebuffer(FramebufferAttachment[] attachments)
    {
        int depthCount = 0;

        List<ID3D11RenderTargetView> targets = new List<ID3D11RenderTargetView>();
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
                    DepthStencil = Device.CreateDepthStencilView(((D3D11Texture) attachment.Texture).Texture,
                        new DepthStencilViewDescription(DepthStencilViewDimension.Texture2D, fmt, 0, 0, 1));
                    break;
                default:
                    targets.Add(Device.CreateRenderTargetView(((D3D11Texture) attachment.Texture).Texture,
                        new RenderTargetViewDescription(RenderTargetViewDimension.Texture2D, fmt, 0, 0, 1)));
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