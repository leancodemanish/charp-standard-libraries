using Serilog;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LeanCode.Standard.Logging
{
    public static class LoggerFactoryFacade
    {
        private const string DEFAULT_NAME = "GLOBAL";
        private static readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();
        private static Serilog.ILogger _globalLogger;
        private static object _syncLock = new object();

        public static ILogger GetLogger<T>()
        {
            var name = typeof(T).AssemblyQualifiedName;
            var globalLogger = GetOrCreateGlobalLogger(DEFAULT_NAME);
            Serilog.ILogger logger = globalLogger.ForContext("SourceContext",typeof(T).Name);
            //Serilog.ILogger logger = globalLogger.ForContext<T>();


            return _loggers.GetOrAdd(name, key => new SerilogLogger(logger, name));
        }

        public static ILogger GetLogger(Type type)
        {
            var name = type.AssemblyQualifiedName;
            var globalLogger = GetOrCreateGlobalLogger(DEFAULT_NAME);            
            Serilog.ILogger logger = globalLogger.ForContext("SourceContext", type.Name);
            return _loggers.GetOrAdd(name, key => new SerilogLogger(logger, name));
        }


        private static Serilog.ILogger GetOrCreateGlobalLogger(string name)
        {
            if (name == null)
            {
                return null;
            }
            lock (_syncLock)
            {
                if (_globalLogger == null)
                {
                    _globalLogger = new LoggerConfiguration()
                    .ReadFrom.AppSettings()
                    .Enrich.FromLogContext()
                    .CreateLogger();
                }                
            }
            return _globalLogger;
        }
    }
}
