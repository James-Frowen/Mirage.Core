using System.Diagnostics;
using Mirage.Logging;

namespace Mirage.SocketLayer
{
    internal static class LoggerExtensions
    {
        internal static void Assert(this ILogger logger, bool condition)
        {
            if (!condition) logger.Log(LogType.Assert, "Failed Assertion");
        }
        internal static void Assert<T>(this ILogger logger, bool condition, T msg)
        {
            if (!condition) logger.Log(LogType.Assert, $"Failed Assertion: {msg}");
        }

        /// <summary>
        /// Assert only when DEBUG
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="condition"></param>
        [Conditional("DEBUG")]
        internal static void DebugAssert(this ILogger logger, bool condition)
        {
            if (!condition) logger.Log(LogType.Assert, "Failed Assertion");
        }
        /// <summary>
        /// Assert only when DEBUG
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="condition"></param>
        [Conditional("DEBUG")]
        internal static void DebugAssert<T>(this ILogger logger, bool condition, T msg)
        {
            if (!condition) logger.Log(LogType.Assert, $"Failed Assertion: {msg}");
        }

        internal static void Error<T>(this ILogger logger, T msg = default)
        {
            logger.Log(LogType.Error, msg);
        }
        internal static void Warn<T>(this ILogger logger, T msg = default)
        {
            logger.Log(LogType.Warning, msg);
        }

        internal static bool Enabled(this ILogger logger, LogType logType)
        {
            return logger != null && logger.IsLogTypeAllowed(logType);
        }
    }
}
