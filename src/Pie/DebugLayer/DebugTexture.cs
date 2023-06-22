using System;
using static Pie.DebugLayer.DebugGraphicsDevice;

namespace Pie.DebugLayer;

internal sealed unsafe class DebugTexture : Texture
{
    public Texture Texture;
    
    public override bool IsDisposed { get; protected set; }
    
    public override TextureDescription Description { get; set; }

    public DebugTexture(TextureDescription description, void* data)
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

        Texture = Device.CreateTexture(description, data);
        
        Description = Texture.Description;
    }

    public void Update(int mipLevel, int arrayIndex, int x, int y, int z, int width, int height, int depth, void* data)
    {
        if (IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to update a disposed texture!");
        
        // TODO: Calculate mip levels when description mip levels is 0.
        if (mipLevel >= Description.MipLevels && Description.MipLevels != 0)
            PieLog.Log(LogType.Critical, "Mip level was out of range.");
        
        if (arrayIndex >= Description.ArraySize)
            PieLog.Log(LogType.Critical, "Array index was out of range.");
        
        Device.UpdateTexture(Texture, mipLevel, arrayIndex, x, y, z, width, height, depth, data);
    }
    
    public override void Dispose()
    {
        Texture.Dispose();
        IsDisposed = Texture.IsDisposed;
        
        PieLog.Log(LogType.Debug, "Texture disposed.");
    }

    internal override MappedSubresource Map(MapMode mode)
    {
        throw new NotImplementedException();
    }

    internal override void Unmap()
    {
        throw new NotImplementedException();
    }
}