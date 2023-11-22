namespace Pie.ShaderCompiler;

/// <summary>
/// The result of a shader compilation.
/// </summary>
public struct CompilerResult
{
    /// <summary>
    /// The result, if any.
    /// </summary>
    /// <remarks>Will be <see langword="null" /> if <see cref="IsSuccess"/> is <see langword="false" />.</remarks>
    public readonly byte[] Result;

    /// <summary>
    /// Whether or not the result was a success.
    /// </summary>
    public readonly bool IsSuccess;

    /// <summary>
    /// An error string, if any.
    /// </summary>
    /// <remarks>Will be <see langword="null" /> or empty if <see cref="IsSuccess"/> is <see langword="true" />.</remarks>
    public readonly string Error;

    /// <summary>
    /// Create a new <see cref="CompilerResult"/>.
    /// </summary>
    /// <param name="result">The result, if any. This should be <see langword="null" /> if <paramref name="isSuccess"/> is <see langword="false" />.</param>
    /// <param name="isSuccess">Whether or not the result was a success.</param>
    /// <param name="error">An error string, if any. This should be <see langword="null" /> or empty if <paramref name="isSuccess"/> is <see langword="true" />.</param>
    public CompilerResult(byte[] result, bool isSuccess, string error)
    {
        Result = result;
        IsSuccess = isSuccess;
        Error = error;
    }
}