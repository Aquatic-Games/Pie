namespace Pie.DebugLayer;

internal sealed unsafe class DebugGraphicsBuffer : GraphicsBuffer
{
    public GraphicsBuffer Buffer;

    public override bool IsDisposed { get; protected set; }

    public bool IsDynamic;

    public BufferType BufferType;

    public DebugGraphicsBuffer(GraphicsDevice device, BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        IsDynamic = dynamic;

        PieLog.Log(LogType.Debug, $@"Buffer info:
    Type: {type},
    Dynamic: {dynamic},
    VideoMemory: {sizeInBytes}B
    HasInitialData: {data != null}");

        BufferType = type;
        Buffer = device.CreateBuffer(type, sizeInBytes, data, dynamic);
    }
    
    public override void Dispose()
    {
        Buffer.Dispose();
        IsDisposed = Buffer.IsDisposed;
        
        PieLog.Log(LogType.Debug, "Buffer disposed.");
    }
}