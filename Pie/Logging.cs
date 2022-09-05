using System;
using System.Diagnostics;

namespace Pie;

public static class Logging
{
    public static event OnLog DebugLog;
    
    internal static void Log(LogType type, string message)
    {
        DebugLog?.Invoke(type, message);
    }

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