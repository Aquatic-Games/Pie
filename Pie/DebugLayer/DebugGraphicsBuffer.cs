namespace Pie.DebugLayer;

internal sealed unsafe class DebugGraphicsBuffer : GraphicsBuffer
{
    private GraphicsBuffer _buffer;
    
    public override bool IsDisposed { get; protected set; }

    public bool IsDynamic;

    public DebugGraphicsBuffer(GraphicsDevice device, BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        IsDynamic = dynamic;

        PieLog.Log(LogType.Debug, $@"Buffer info:
    Type: {type},
    Dynamic: {dynamic},
    VideoMemory: {sizeInBytes}B
    HasInitialData: {data != null}");
        
        _buffer = device.CreateBuffer(type, sizeInBytes, data, dynamic);
    }
    
    public override void Dispose()
    {
        PieLog.Log(LogType.Debug, "Buffer disposed.");
        _buffer.Dispose();
        IsDisposed = _buffer.IsDisposed;
    }
}