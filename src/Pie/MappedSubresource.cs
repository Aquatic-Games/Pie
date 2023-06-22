using System;

namespace Pie;

public struct MappedSubresource
{
    public IntPtr DataPtr;

    public MappedSubresource(IntPtr dataPtr)
    {
        DataPtr = dataPtr;
    }
}