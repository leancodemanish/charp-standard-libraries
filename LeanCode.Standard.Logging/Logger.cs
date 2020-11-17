using log4net;
using log4net.Repository;
using System;
using System.Runtime.CompilerServices;

namespace LeanCode.Standard.Logging
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog logger;

        internal Log4NetLogger(ILog logger, string name)
        {
            this.logger = logger;
            Name = name;
        }

        public bool IsDebugEnabled
        {
            get { return logger.IsDebugEnabled; }
        }

        public string Name { get; }

        public ILoggerRepository Repository => throw new NotImplementedException();

        public void Debug(object message)
        {
            logger.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            logger.Debug(message, exception);
        }

        public void Info(object message)
        {
            logger.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            logger.Info(message, exception);
        }

        public void Warn(object message)
        {
            logger.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            logger.Warn(message, exception);
        }

        public void Error(object message)
        {
            logger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            logger.Error(message, exception);
        }

        public void Fatal(object message)
        {
            logger.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            logger.Fatal(message, exception);
        }

        public void Info(string messageTemplate, params object[] args)
        {
            throw new NotImplementedException();
        }

        public ILogger Here([CallerMemberName] string memberName = "")
        {
            throw new NotImplementedException();
        }

        public void Debug(string messageTemplate, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(string messageTemplate, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception exception, string messageTemplate, params object[] args)
        {
            throw new NotImplementedException();
        }

        public ILogger WithContext([CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            throw new NotImplementedException();
        }
    }
}
