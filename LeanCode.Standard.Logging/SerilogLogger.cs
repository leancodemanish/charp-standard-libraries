using log4net;
using log4net.Repository;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LeanCode.Standard.Logging
{
    public class SerilogLogger : ILogger
    {
        private readonly Serilog.ILogger logger;
        private static readonly ConcurrentDictionary<string, Serilog.ILogger> _contextualLoggers = new ConcurrentDictionary<string, Serilog.ILogger>();

        internal SerilogLogger(Serilog.ILogger logger, string name)
        {
            this.logger = logger;
            Name = name;
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public string Name { get; }

        public ILoggerRepository Repository => throw new NotImplementedException();

        public void Debug(object message)
        {
            logger.Debug("{Message}", message);
        }

        public void Debug(string messageTemplate, params object[] args)
        {
            logger.Debug(messageTemplate, args);
        }


        public void Debug(object message, Exception exception)
        {
            logger.Debug(exception, "{Message}", message);
        }

        public void Info(object message)
        {
            logger.Information("{Message}", message);
        }

        public void Info(string messageTemplate, params object[] args)
        {
            logger.Information(messageTemplate, args);
        }

        public void Warn(object message)
        {
            logger.Warning("{Message}", message);
        }

        public void Warn(string messageTemplate, params object[] args)
        {
            logger.Warning(messageTemplate, args);
        }

        public void Error(object message)
        {
            logger.Error("{Message}", message);
        }

        public void Error(object message, Exception exception)
        {
            logger.Error(exception, "{Message}", message);
        }

        public void Error(Exception exception, string messageTemplate, params object[] args)
        {
            logger.Error(exception, messageTemplate, args);
        }

        public void Fatal(object message)
        {
            logger.Fatal("{Message}", message);
        }

        public void Fatal(object message, Exception exception)
        {
            logger.Fatal(exception, "{Message}", message);
        }

        public virtual ILogger WithContext([CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber=0)
        {
            if (!string.IsNullOrEmpty(memberName))
            {
                var nameWithLine = (memberName + lineNumber);
                var contextualLogger = GetContextualLogger(nameWithLine);                
                return new ContextualSerilogger(contextualLogger, Name);
            }
            return this;
        }

        private Serilog.ILogger GetContextualLogger([CallerMemberName] string memberName = "")
        {
            if (!string.IsNullOrEmpty(memberName))
            {
                var memberContext = logger.ForContext("MemberName", memberName);
                return _contextualLoggers.GetOrAdd(memberName, (key) => memberContext);
            }
            return this.logger;
        }

        private void GetCallerInfo(out string memberName, out int lineNumber)
        {
            StackFrame callStack = new StackFrame(2, true);
            memberName = callStack.GetMethod().Name;
            lineNumber = callStack.GetFileLineNumber();            
        }
       
    }
}
