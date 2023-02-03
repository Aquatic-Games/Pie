using System;

namespace Pie;

/// <summary>
/// Describes how a <see cref="DepthState"/> should behave.
/// </summary>
public struct DepthStateDescription : IEquatable<DepthStateDescription>
{
    /// <summary>
    /// Disable depth testing.
    /// </summary>
    public static readonly DepthStateDescription Disabled =
        new DepthStateDescription(false, true, DepthComparison.Never);

    /// <summary>
    /// The depth test passes when the incoming depth value is less than or equal to the stored depth value.
    /// </summary>
    public static readonly DepthStateDescription LessEqual =
        new DepthStateDescription(true, true, DepthComparison.LessEqual);
    
    /// <summary>
    /// Enable/disable depth testing.
    /// </summary>
    public bool DepthEnabled;

    /// <summary>
    /// Enable/disable writing to the depth buffer.
    /// </summary>
    public bool DepthMask;

    /// <summary>
    /// The depth comparison to use for this depth state.
    /// </summary>
    public DepthComparison DepthComparison;

    /// <summary>
    /// Create a new depth state description.
    /// </summary>
    /// <param name="depthEnabled">Enable/disable depth testing.</param>
    /// <param name="depthMask">Enable/disable writing to the depth buffer.</param>
    /// <param name="depthComparison">The depth comparison to use for this depth state.</param>
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