using System;
using System.Diagnostics;
using Pie.DebugLayer;

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
        switch (type)
        {
            case LogType.Warning:
                DebugMetrics.Warnings++;
                break;
            case LogType.Error:
                DebugMetrics.Errors++;
                break;
            case LogType.Critical:
                DebugMetrics.Errors++;
                break;
            case LogType.Debug:
            case LogType.Info:
            case LogType.Verbose:
            default:
                break;
        }
        
        DebugLog?.Invoke(type, message);
    }

    /// <summary>
    /// Called whenever Pie logs an event.
    /// </summary>
    public delegate void OnLog(LogType logType, string message);
}

public enum LogType
{
    Verbose,
    Debug,
    Info,
    Warning,
    Error,
    Critical
}