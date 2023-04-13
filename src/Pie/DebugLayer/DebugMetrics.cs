namespace Pie.DebugLayer;

/// <summary>
/// Some debug metrics, used in conjunction with <see cref="GraphicsDeviceOptions.Debug"/>.
/// </summary>
public static class DebugMetrics
{
    /// <summary>
    /// The number of warnings generated.
    /// </summary>
    public static int Warnings;

    /// <summary>
    /// The number of errors generated.
    /// </summary>
    public static int Errors;

    /// <summary>
    /// Get a report string.
    /// </summary>
    /// <returns>The report string.</returns>
    public static string GetString()
    {
        if (Warnings == 0 && Errors == 0)
            return "Error check: Nothing to report!";
        return $"Error check: Potential issues found. {Warnings} warning(s) and {Errors} error(s) generated.";
    }
}