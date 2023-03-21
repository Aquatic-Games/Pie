namespace Pie.DebugLayer;

internal sealed unsafe class DebugGraphicsBuffer : GraphicsBuffer
{
    private uint _sizeInBytes;
    
    public GraphicsBuffer Buffer;

    public override bool IsDisposed { get; protected set; }

    public bool IsDynamic;

    public bool IsMapped;

    public BufferType BufferType;

    public DebugGraphicsBuffer(GraphicsDevice device, BufferType type, uint sizeInBytes, void* data, bool dynamic)
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
        
        Buffer = device.CreateBuffer(type, sizeInBytes, data, dynamic);
    }

    public void Update(GraphicsDevice device, uint offsetInBytes, uint sizeInBytes, void* data)
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
        
        device.UpdateBuffer(Buffer, offsetInBytes, sizeInBytes, data);
    }
    
    public override void Dispose()
    {
        Buffer.Dispose();
        IsDisposed = Buffer.IsDisposed;
        
        PieLog.Log(LogType.Debug, "Buffer disposed.");
    }
}