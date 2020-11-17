using System;
using log4net;
using System.Collections.Concurrent;

namespace LeanCode.Standard.Logging
{
    public static class Log4NetLoggerFactoryFacade
    {
        private const string DEFAULT_NAME = "Log4Net";
        private static readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        public static ILogger GetLogger(Type type)
        {
            var name = (type != null) ? type.AssemblyQualifiedName : DEFAULT_NAME;

            if (name == null)
            {
                return null;
            }
            return _loggers.GetOrAdd(name, key => new Log4NetLogger(LogManager.GetLogger(type), name));
        }
    }
}
