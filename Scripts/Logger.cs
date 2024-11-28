using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class Logger
{
    [Conditional("LOG_ENABLED")]
    public static void Log(string message)
    {
        Debug.Log(message);
    }

    [Conditional("LOG_ENABLED")]
    public static void Log(string message, Object sender)
    {
        Debug.Log(message, sender);
    }

    [Conditional("LOG_ENABLED")]
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    [Conditional("LOG_ENABLED")]
    public static void LogWarning(string message, Object sender)
    {
        Debug.LogWarning(message, sender);
    }

    public static void LogError(string message)
    {
        Debug.LogError(message);
    }

    public static void LogError(string message, Object sender)
    {
        Debug.LogError(message, sender);
    }
}
