using System;
using System.Diagnostics;

namespace Pie;

/// <summary>
/// Pie will log various events here. Can be especially useful during debugging.
/// </summary>
public static class PieLog
{
    /// <summary>
    /// Called whenever Pie logs an event.
    /// </summary>
    public static event OnLog DebugLog;
    
    internal static void Log(LogType type, string message)
    {
        DebugLog?.Invoke(type, message);
    }

    /// <summary>
    /// Called whenever Pie logs an event.
    /// </summary>
    public delegate void OnLog(LogType logType, string message);
}

public enum LogType
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}