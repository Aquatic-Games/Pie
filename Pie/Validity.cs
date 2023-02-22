namespace Pie;

/// <summary>
/// Represents the result of a validity check.
/// </summary>
public struct Validity
{
    /// <summary>
    /// Is <see langword="true" /> when the validity check passes.
    /// </summary>
    public readonly bool IsValid;

    /// <summary>
    /// An accompanying message, if any. This is only set if <see cref="IsValid"/> is <see langword="false" />.
    /// </summary>
    public readonly string Message;

    /// <summary>
    /// Create a new <see cref="Validity"/> result.
    /// </summary>
    /// <param name="isValid">Is <see langword="true" /> when the validity check passes.</param>
    /// <param name="message">An accompanying message, if any.</param>
    public Validity(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }
}