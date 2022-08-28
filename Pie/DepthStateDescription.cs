using System;

namespace Pie;

public struct DepthStateDescription : IEquatable<DepthStateDescription>
{
    public static readonly DepthStateDescription Disabled =
        new DepthStateDescription(false, true, DepthComparison.Never);

    public static readonly DepthStateDescription LessEqual =
        new DepthStateDescription(true, true, DepthComparison.LessEqual);
    
    public bool DepthEnabled;

    public bool DepthMask;

    public DepthComparison DepthComparison;

    public DepthStateDescription(bool depthEnabled, bool depthMask, DepthComparison depthComparison)
    {
        DepthEnabled = depthEnabled;
        DepthMask = depthMask;
        DepthComparison = depthComparison;
    }

    public bool Equals(DepthStateDescription other)
    {
        return DepthEnabled == other.DepthEnabled && DepthMask == other.DepthMask && DepthComparison == other.DepthComparison;
    }

    public override bool Equals(object obj)
    {
        return obj is DepthStateDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DepthEnabled, DepthMask, (int) DepthComparison);
    }

    public static bool operator ==(DepthStateDescription left, DepthStateDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DepthStateDescription left, DepthStateDescription right)
    {
        return !left.Equals(right);
    }
}