using System;
using System.Collections.Generic;

namespace Pie.DebugLayer;

/// <summary>
/// Utility class for performing memory leak checks.
/// </summary>
public static class DebugMemory
{
    private static bool _isMemoryLeakCheck;
    private static List<IDisposable> _memory;

    static DebugMemory()
    {
        _memory = new List<IDisposable>();
    }

    public static void BeginMemoryLeakCheck(MemoryCheckOptions options)
    {
        _memory.Clear();
        _isMemoryLeakCheck = true;
    }

    public static MemoryCheckResult EndMemoryLeakCheck()
    {
        _isMemoryLeakCheck = false;
        return new MemoryCheckResult(_memory.Count);
    }

    internal static void AddMemoryItem(IDisposable disposable)
    {
        _memory.Add(disposable);
    }

    internal static void RemoveMemoryItem(IDisposable disposable)
    {
        _memory.Remove(disposable);
    }
}