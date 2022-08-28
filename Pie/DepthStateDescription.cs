namespace Pie;

public struct DepthStateDescription
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
}