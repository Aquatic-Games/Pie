namespace Pie.ShaderCompiler;

public struct SpecializationConstant
{
    public uint ID;
    public ConstantType Type;
    public ulong Value;

    public SpecializationConstant(uint id, uint value)
    {
        ID = id;
        Type = ConstantType.U32;
        Value = value;
    }

    public SpecializationConstant(uint id, int value)
    {
        ID = id;
        Type = ConstantType.I32;
        Value = (ulong) value;
    }

    public unsafe SpecializationConstant(uint id, float value)
    {
        ID = id;
        Type = ConstantType.F32;
        Value = *(uint*) &value;
    }
    
    public SpecializationConstant(uint id, ulong value)
    {
        ID = id;
        Type = ConstantType.U64;
        Value = value;
    }

    public SpecializationConstant(uint id, long value)
    {
        ID = id;
        Type = ConstantType.I64;
        Value = (ulong) value;
    }

    public unsafe SpecializationConstant(uint id, double value)
    {
        ID = id;
        Type = ConstantType.F64;
        Value = *(ulong*) &value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Type, Value);
    }
}