using System;
using UnityEngine;

public class CLog
{

    /// <summary>
    /// 統一推送一般訊息
    /// </summary>
    /// <param name="value"></param>
    public static void PrintLog(string value, string title = "CLog", string color = "black", LogLevel logLevel = LogLevel.Normal)
    {
        Debug.Log(value);
    }

    /// <summary>
    /// 統一推送錯誤訊息
    /// </summary>
    /// <param name="value"></param>
    /// <param name="args"></param>
    public static void PrintLogError(string value, string title = "CLog", string color = "black", LogLevel logLevel = LogLevel.Normal)
    {
        Debug.LogError(value);
    }
    /// <summary>
    /// 統一推送警告訊息
    /// </summary>
    /// <param name="value"></param>
    public static void PrintLogWarning(string value, string title = "CLog", string color = "black", LogLevel logLevel = LogLevel.Normal)
    {
        Debug.LogWarning(value);
    }
}
