using System;

namespace Mirage.Logging
{
    public class ConsoleLogger : ILogHandler
    {
        private ConsoleColor[] logTypeToColor = new ConsoleColor[] {
            ConsoleColor.Red,
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.White,
            ConsoleColor.Red,
        };

        public void LogFormat(LogType logType, string format, params object[] args)
        {
            Console.ForegroundColor = logTypeToColor[(int)logType];

            // only use format if there are args
            var msg = (args != null && args.Length > 0)
                ? string.Format(format, args)
                : format;

            Console.WriteLine(msg);
            Console.ResetColor();
        }

        void ILogHandler.LogException(Exception exception)
        {
            Console.ForegroundColor = logTypeToColor[(int)LogType.Exception];
            Console.WriteLine(exception);
            Console.ResetColor();
        }
    }
    public class StandaloneLogger : ILogger, ILogHandler
    {
        public ILogHandler logHandler { get; set; }
        public bool logEnabled { get; set; }
        public LogType filterLogType { get; set; }

        public StandaloneLogger(ILogHandler logHandler = null)
        {
            this.logHandler = logHandler ?? new ConsoleLogger();
            filterLogType = LogType.Log;
            logEnabled = true;
        }

        public bool IsLogTypeAllowed(LogType logType)
        {
            if (!logEnabled) { return false; }
            if (logType == LogType.Exception) { return true; }
            if (filterLogType == LogType.Exception) { return false; }

            // if check type is less than logger type
            // eg check error <= warning === true
            return logType <= filterLogType;
        }

        public void Log(LogType type, object message)
        {
            logHandler.LogFormat(type, message.ToString());
        }

        public void Log(object message)
        {
            Log(LogType.Log, message.ToString());
        }

        public void LogWarning(object message)
        {
            Log(LogType.Warning, message.ToString());
        }

        public void LogError(object message)
        {
            Log(LogType.Error, message.ToString());
        }

        public void LogException(Exception ex)
        {
            logHandler.LogException(ex);
        }

        public void Log(LogType logType, string msg)
        {
            if (IsLogTypeAllowed(logType))
                logHandler.LogFormat(logType, msg);
        }

        public void LogFormat(LogType logType, string format, params object[] args)
        {
            if (IsLogTypeAllowed(logType))
                logHandler.LogFormat(logType, format, args);
        }
    }

    public static class Debug
    {
        public static ILogger unityLogger { get; set; } = new StandaloneLogger();

        public static void Assert(bool condition)
        {
            if (!condition)
                unityLogger.LogError("Assertion failed");
        }
        public static void Assert(bool condition, string message)
        {
            if (!condition)
                unityLogger.LogError(message);
        }
        public static void Log(string message) => unityLogger.Log(message);
        public static void LogWarning(string message) => unityLogger.LogWarning(message);
        public static void LogError(string message) => unityLogger.LogError(message);
        public static void LogException(Exception e) => unityLogger.LogException(e);
    }
    // enum to match unity
    public enum LogType
    {
        Error = 0,
        Assert = 1,
        Warning = 2,
        Log = 3,
        Exception = 4,
    }

    // Interface to match unity
    public interface ILogHandler
    {
        void LogFormat(LogType logType, string format, params object[] args);

        void LogException(Exception exception);
    }

    // Interface to match unity
    public interface ILogger : ILogHandler
    {
        LogType filterLogType { get; set; }
        ILogHandler logHandler { get; set; }
        bool logEnabled { get; set; }

        bool IsLogTypeAllowed(LogType logType);
        void Log(LogType type, object message);

        void Log(object message);
        void LogWarning(object message);
        void LogError(object message);
    }
}

