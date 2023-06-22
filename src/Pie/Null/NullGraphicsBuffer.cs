using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Pie.Null;

internal sealed unsafe class NullGraphicsBuffer : GraphicsBuffer
{
    public override bool IsDisposed { get; protected set; }

    public void* Data;
    private BufferType _type;

    public NullGraphicsBuffer(BufferType type, uint sizeInBytes, void* data, bool dynamic)
    {
        Data = NativeMemory.Alloc(sizeInBytes);
        Unsafe.CopyBlock(Data, data, sizeInBytes);

        _type = type;
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        NativeMemory.Free(Data);
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
