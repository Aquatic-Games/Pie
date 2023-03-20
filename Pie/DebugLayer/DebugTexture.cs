using System;

namespace Pie.DebugLayer;

internal sealed unsafe class DebugTexture : Texture
{
    private Texture _texture;
    
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

        _texture = device.CreateTexture(description, data);
        
        Description = _texture.Description;
    }
    
    public override void Dispose()
    {
        _texture.Dispose();
        IsDisposed = _texture.IsDisposed;
        
        PieLog.Log(LogType.Debug, "Texture disposed.");
    }
}