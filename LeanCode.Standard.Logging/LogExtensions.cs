using System.Diagnostics;

namespace Onyx.Standard.Logging
{
    public static class LogExtensions
    {
        private static void Log(this IEventPublisher eventPublisher, LogType logType, string message, params object[] messageParameters)
        {
            var method = new StackFrame(1, false).GetMethod();

            eventPublisher.PublishAsync(new Log(method.DeclaringType, logType, message, messageParameters));
        }

        public static void LogDebug(this IEventPublisher eventPublisher, string message, params object[] messageParameters)
        {
            eventPublisher.Log(LogType.Debug, message, messageParameters);
        }

        public static void LogInformation(this IEventPublisher eventPublisher, string message, params object[] messageParameters)
        {
            eventPublisher.Log(LogType.Information, message, messageParameters);
        }

        public static void LogWarning(this IEventPublisher eventPublisher, string message, params object[] messageParameters)
        {
            eventPublisher.Log(LogType.Warning, message, messageParameters);
        }

        public static void LogError(this IEventPublisher eventPublisher, string message, params object[] messageParameters)
        {
            eventPublisher.Log(LogType.Error, message, messageParameters);
        }

        public static void LogFatal(this IEventPublisher eventPublisher, string message, params object[] messageParameters)
        {
            eventPublisher.Log(LogType.Fatal, message, messageParameters);
        }
    }
}
