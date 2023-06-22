using System;
using static Pie.DebugLayer.DebugGraphicsDevice;

namespace Pie.DebugLayer;

internal sealed unsafe class DebugGraphicsBuffer : GraphicsBuffer
{
    private uint _sizeInBytes;
    
    public GraphicsBuffer Buffer;

    public override bool IsDisposed { get; protected set; }

    public bool IsDynamic;

    public bool IsMapped;

    public BufferType BufferType;

    public DebugGraphicsBuffer(BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        _sizeInBytes = sizeInBytes;
        
        IsDynamic = dynamic;
        IsMapped = false;
        BufferType = type;

        PieLog.Log(LogType.Debug, $@"Buffer info:
    Type: {type},
    Dynamic: {dynamic},
    VideoMemory: {sizeInBytes}B
    HasInitialData: {data != null}");
        
        Buffer = Device.CreateBuffer(type, sizeInBytes, data, dynamic);
    }

    public void Update(uint offsetInBytes, uint sizeInBytes, void* data)
    {
        if (IsDisposed)
            PieLog.Log(LogType.Critical, "Attempting to update a disposed buffer!");
        
        if (!IsDynamic)
            PieLog.Log(LogType.Critical, "Cannot update a non-dynamic buffer.");
        
        if (offsetInBytes > _sizeInBytes)
            PieLog.Log(LogType.Critical, "Offset is greater than the size of the buffer.");
        if (sizeInBytes > _sizeInBytes)
            PieLog.Log(LogType.Critical, "Data size is greater than the size of the buffer.");
        if (offsetInBytes + sizeInBytes > _sizeInBytes)
            PieLog.Log(LogType.Critical, "The data size cannot fit into the buffer with the given offset, its end point is larger than the size of the buffer.");
        
        Device.UpdateBuffer(Buffer, offsetInBytes, sizeInBytes, data);
    }
    
    public override void Dispose()
    {
        Buffer.Dispose();
        IsDisposed = Buffer.IsDisposed;
        
        PieLog.Log(LogType.Debug, "Buffer disposed.");
    }

    internal override MappedSubresource Map(MapMode mode)
    {
        if (IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to map a disposed buffer!");
        
        if (!IsDynamic)
            PieLog.Log(LogType.Critical, "Cannot map a non-dynamic buffer.");

        if (IsMapped)
            PieLog.Log(LogType.Critical, "Cannot map a buffer that has already been mapped.");

        IsMapped = true;

        return Device.MapResource(Buffer, mode);
    }

    internal override void Unmap()
    {
        if (IsDisposed)
            PieLog.Log(LogType.Critical, "Attempted to unmap a disposed buffer!");
        
        if (!IsMapped)
            PieLog.Log(LogType.Critical, "Cannot unmap a buffer that has not been mapped.");
        
        // This should never happen but it doesn't hurt to have the check!
        if (!IsDynamic)
            PieLog.Log(LogType.Critical, "Attempted to unmap a non-dynamic buffer. If you see this message, Pie's checks have gone wrong. Either that or something MAJORLY bad has happened. You should now panic.");

        IsMapped = false;
        
        Device.UnmapResource(Buffer);
    }
}