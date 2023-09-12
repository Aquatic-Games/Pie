using System;

namespace Pie.Audio;

public enum DataType
{
    I8,
    U8,
    I16,
    U16,
    I32,
    F32,
    F64
}

public static class DataTypeExtensions
{
    public static int Bits(this DataType @this)
    {
        return @this switch
        {
            DataType.I8 => 8,
            DataType.U8 => 8,
            DataType.I16 => 16,
            DataType.U16 => 16,
            DataType.I32 => 32,
            DataType.F32 => 32,
            DataType.F64 => 64,
            _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, null)
        };
    }

    public static int Bytes(this DataType @this)
    {
        return Bits(@this) / 8;
    }
}