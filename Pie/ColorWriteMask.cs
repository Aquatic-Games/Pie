using System;

namespace Pie;

[Flags]
public enum ColorWriteMask
{
    None = 1 << 0,
    
    Red = 1 << 1,
    
    Green = 1 << 2,
    
    Blue = 1 << 3,
    
    Alpha = 1 << 4,
    
    All = Red | Green | Blue | Alpha
}