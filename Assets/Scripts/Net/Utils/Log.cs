using System;

/// <summary>
/// Logger.
/// </summary>
public static class Log 
{
    // Right now we are just passing through to Unity logs.
    // Future improvements - 
    // log to file
    // Config that specifies which classes to log
    // Conditional pre-process to strip logs out of the asssembly.
    // Cool console for standalone builds/VR.
    public static void Debug(object sender, string message, params object[] args)
    {
        UnityEngine.Debug.Log(string.Format(message, args), sender as UnityEngine.Object);
    }

    public static void Warning(object sender, string message, params object[] args)
    {
        UnityEngine.Debug.LogWarning(string.Format(message, args), sender as UnityEngine.Object);
    }

    public static void Error(object sender, string message, params object[] args)
    {
        UnityEngine.Debug.LogError(string.Format(message, args), sender as UnityEngine.Object);
    }

    public static void Exception(object sender, Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    public static void Assert(object sender, bool condition)
    {
        UnityEngine.Debug.Assert(condition);
    }

    public static void Assert(object sender, bool condition, string message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }
}
