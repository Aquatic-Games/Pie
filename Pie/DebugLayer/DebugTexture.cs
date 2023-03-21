using System;

namespace Pie.DebugLayer;

internal sealed unsafe class DebugTexture : Texture
{
    public Texture Texture;
    
    public override bool IsDisposed { get; protected set; }
    
    public override TextureDescription Description { get; set; }

    public DebugTexture(GraphicsDevice device, TextureDescription description, void* data)
    {
        Validity validity = description.Validity;
        if (!validity.IsValid)
            PieLog.Log(LogType.Critical, validity.Message);
        
        int bpp = description.Format.BitsPerPixel();

        int totalSize = bpp * description.ArraySize;

        if (description.Width > 0)
            totalSize *= description.Width;
        if (description.Height > 0)
            totalSize *= description.Height;
        if (description.Depth > 0)
            totalSize *= description.Depth;

        PieLog.Log(LogType.Debug, $@"Texture info:
    Type: {description.TextureType}
    Width: {description.Width}
    Height: {description.Height}
    Depth: {description.Depth}
    MipLevels: {description.MipLevels}
    ArraySize: {description.ArraySize}
    Usage: {description.Usage}
    Format: {description.Format}
    BitsPerPixel: {bpp}
    VideoMemory: {totalSize}B
    HasInitialData: {data != null}
");

        Texture = device.CreateTexture(description, data);
        
        Description = Texture.Description;
    }
    
    public override void Dispose()
    {
        Texture.Dispose();
        IsDisposed = Texture.IsDisposed;
        
        PieLog.Log(LogType.Debug, "Texture disposed.");
    }
}