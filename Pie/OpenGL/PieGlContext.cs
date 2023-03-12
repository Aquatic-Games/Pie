using System;
using System.Runtime.CompilerServices;

namespace Pie.OpenGL;

/// <summary>
/// As OpenGL does not provide a graphics device, it is down to the windowing backend to handle proc addresses and
/// presenting. As pie does not work like this, you must provide a Pie context so that it can work as expected.
/// </summary>
public sealed class PieGlContext
{
    /// <summary>
    /// The GetProcAddress function pointer.
    /// </summary>
    public Func<string, nint> GetProcFunc;
    
    /// <summary>
    /// The presentation function pointer.
    /// </summary>
    public Action<int> PresentFunc;

    /// <summary>
    /// Create a new <see cref="PieGlContext"/>.
    /// </summary>
    /// <param name="getProcAddress">The GetProcAddress function pointer.</param>
    /// <param name="present">The presentation function pointer.</param>
    public PieGlContext(Func<string, nint> getProcAddress, Action<int> present)
    {
        GetProcFunc = getProcAddress;
        PresentFunc = present;
    }

    /// <summary>
    /// Get the proc address with the given name.
    /// </summary>
    /// <param name="name">The name to get.</param>
    /// <returns>The found proc address.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nint GetProcAddress(string name)
    {
        return GetProcFunc.Invoke(name);
    }

    /// <summary>
    /// Present to the screen.
    /// </summary>
    /// <param name="swapInterval">The swap interval to use.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Present(int swapInterval)
    {
        PresentFunc.Invoke(swapInterval);
    }
}