using System;

namespace Pie;

/// <summary>
/// Describes how a <see cref="DepthStencilState"/> should behave.
/// </summary>
public struct DepthStencilStateDescription : IEquatable<DepthStencilStateDescription>
{
    /// <summary>
    /// Disable depth testing.
    /// </summary>
    public static readonly DepthStencilStateDescription Disabled =
        new DepthStencilStateDescription(false, true, ComparisonFunc.Never);

    /// <summary>
    /// The depth test passes when the incoming depth value is less than or equal to the stored depth value.
    /// </summary>
    public static readonly DepthStencilStateDescription LessEqual =
        new DepthStencilStateDescription(true, true, ComparisonFunc.LessEqual);
    
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
    public ComparisonFunc DepthComparison;

    /// <summary>
    /// Enable/disable stencil testing.
    /// </summary>
    public bool StencilEnabled;

    /// <summary>
    /// The stencil read mask.
    /// </summary>
    public byte StencilReadMask;

    /// <summary>
    /// The stencil write mask.
    /// </summary>
    public byte StencilWriteMask;

    /// <summary>
    /// The stencil operations to perform for a front-facing pixel.
    /// </summary>
    public StencilFace StencilFrontFace;

    /// <summary>
    /// The stencil operations to perform for a back-facing pixel.
    /// </summary>
    public StencilFace StencilBackFace;

    /// <summary>
    /// Create a new <see cref="DepthStencilStateDescription"/>.
    /// </summary>
    /// <param name="depthEnabled">Enable/disable depth testing.</param>
    /// <param name="depthMask">Enable/disable writing to the depth buffer.</param>
    /// <param name="depthComparison">The depth comparison to use for this depth state.</param>
    /// <param name="stencilEnabled">Enable/disable stencil testing.</param>
    /// <param name="stencilReadMask">The stencil read mask.</param>
    /// <param name="stencilWriteMask">The stencil write mask.</param>
    /// <param name="stencilFrontFace">The stencil operations to perform for a front-facing pixel.</param>
    /// <param name="stencilBackFace">The stencil operations to perform for a back-facing pixel.</param>
    public DepthStencilStateDescription(bool depthEnabled, bool depthMask, ComparisonFunc depthComparison,
        bool stencilEnabled, byte stencilReadMask, byte stencilWriteMask, StencilFace stencilFrontFace,
        StencilFace stencilBackFace)
    {
        DepthEnabled = depthEnabled;
        DepthMask = depthMask;
        DepthComparison = depthComparison;
        StencilEnabled = stencilEnabled;
        StencilReadMask = stencilReadMask;
        StencilWriteMask = stencilWriteMask;
        StencilFrontFace = stencilFrontFace;
        StencilBackFace = stencilBackFace;
    }

    /// <summary>
    /// Create a new <see cref="DepthStencilStateDescription"/>, with depth-only parameters.
    /// </summary>
    /// <param name="depthEnabled">Enable/disable depth testing.</param>
    /// <param name="depthMask">Enable/disable writing to the depth buffer.</param>
    /// <param name="depthComparison">The depth comparison to use for this depth state.</param>
    public DepthStencilStateDescription(bool depthEnabled, bool depthMask, ComparisonFunc depthComparison)
    {
        DepthEnabled = depthEnabled;
        DepthMask = depthMask;
        DepthComparison = depthComparison;
        StencilEnabled = false;
        StencilReadMask = 0xFF;
        StencilWriteMask = 0xFF;
        StencilFrontFace = new StencilFace(StencilOp.Keep, StencilOp.Increment, StencilOp.Keep, ComparisonFunc.Always);
        StencilBackFace = new StencilFace(StencilOp.Keep, StencilOp.Decrement, StencilOp.Keep, ComparisonFunc.Always);
    }

    /// <summary>
    /// Create a new <see cref="DepthStencilStateDescription"/>, with stencil-only parameters.
    /// </summary>
    /// <param name="stencilEnabled">Enable/disable stencil testing.</param>
    /// <param name="stencilReadMask">The stencil read mask.</param>
    /// <param name="stencilWriteMask">The stencil write mask.</param>
    /// <param name="stencilFrontFace">The stencil operations to perform for a front-facing pixel.</param>
    /// <param name="stencilBackFace">The stencil operations to perform for a back-facing pixel.</param>
    public DepthStencilStateDescription(bool stencilEnabled, byte stencilReadMask, byte stencilWriteMask,
        StencilFace stencilFrontFace, StencilFace stencilBackFace)
    {
        DepthEnabled = false;
        DepthMask = true;
        DepthComparison = ComparisonFunc.LessEqual;
        StencilEnabled = stencilEnabled;
        StencilReadMask = stencilReadMask;
        StencilWriteMask = stencilWriteMask;
        StencilFrontFace = stencilFrontFace;
        StencilBackFace = stencilBackFace;
    }

    public bool Equals(DepthStencilStateDescription other)
    {
        return DepthEnabled == other.DepthEnabled && DepthMask == other.DepthMask && DepthComparison == other.DepthComparison;
    }

    public override bool Equals(object obj)
    {
        return obj is DepthStencilStateDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DepthEnabled, DepthMask, (int) DepthComparison);
    }

    public static bool operator ==(DepthStencilStateDescription left, DepthStencilStateDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DepthStencilStateDescription left, DepthStencilStateDescription right)
    {
        return !left.Equals(right);
    }
}