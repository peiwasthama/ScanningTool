using System;
using System.IO;
using System.Text;

namespace scanningTool.Helpers
{
    /// <summary>
    /// Helper class for logging application events and errors.
    /// </summary>
    public static class LoggingHelper
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static readonly string LogFileName = "scanningTool.log";
        private static readonly object LogLock = new object();

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogInfo(string message)
        {
            Log("INFO", message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(string message)
        {
            Log("ERROR", message);
        }

        /// <summary>
        /// Logs an exception with an optional message.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">An optional message to include with the exception.</param>
        public static void LogException(Exception ex, string message = null)
        {
            StringBuilder sb = new StringBuilder();
            
            if (!string.IsNullOrEmpty(message))
            {
                sb.AppendLine(message);
            }
            
            sb.AppendLine($"Exception: {ex.GetType().Name}");
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"StackTrace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                sb.AppendLine("Inner Exception:");
                sb.AppendLine($"Type: {ex.InnerException.GetType().Name}");
                sb.AppendLine($"Message: {ex.InnerException.Message}");
                sb.AppendLine($"StackTrace: {ex.InnerException.StackTrace}");
            }
            
            Log("EXCEPTION", sb.ToString());
        }

        /// <summary>
        /// Logs a message with the specified level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to log.</param>
        private static void Log(string level, string message)
        {
            try
            {
                // Ensure log directory exists
                if (!Directory.Exists(LogFilePath))
                {
                    Directory.CreateDirectory(LogFilePath);
                }
                
                string fullPath = Path.Combine(LogFilePath, LogFileName);
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string logEntry = $"[{timestamp}] [{level}] {message}";
                
                // Use a lock to prevent multiple threads from writing to the file simultaneously
                lock (LogLock)
                {
                    File.AppendAllText(fullPath, logEntry + Environment.NewLine);
                }
            }
            catch
            {
                // Silently fail if logging fails
                // We don't want logging failures to crash the application
            }
        }
    }
}
