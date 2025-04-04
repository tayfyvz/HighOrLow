using System;
using UnityEngine;

namespace Utils
{
    public static class Logger
    {
        private static bool _enableLogging = true;

        public static void Log(string message, LogType logType = LogType.Log)
        {
            if (!_enableLogging) return;

            switch (logType)
            {
                case LogType.Log:
                    Debug.Log($"[INFO] {message}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"[WARNING] {message}");
                    break;
                case LogType.Error:
                    Debug.LogError($"[ERROR] {message}");
                    break;
                default:
                    Debug.Log($"[UNKNOWN] {message}");
                    break;
            }
        }

        public static void LogException(Exception exception)
        {
            if (!_enableLogging) return;

            Debug.LogError($"[EXCEPTION] {exception.Message}\nStackTrace: {exception.StackTrace}");
        }

        public static void EnableLogging(bool isEnabled)
        {
            _enableLogging = isEnabled;
        }
        
        public static bool IsLoggingOn()
        {
            return _enableLogging;
        }
    }
}