using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace LeanCode.Standard.Logging
{
    public class LogTimer : IDisposable
    {

        private readonly ILogger _log = LoggerFactoryFacade.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string _operation;
        private readonly DateTime _startTime;
        private readonly LogType _logMode;

        public LogTimer(string operation, LogType logMode = LogType.Debug, bool logStartAndFinish = false)
        {
            _logMode = logMode;
            _startTime = DateTime.UtcNow;
            _operation = operation;

            if (logStartAndFinish)
                LogMessage(operation);
        }

        public static LogTimer FromScopeNameAndCallerMemberName(string typeOrScope,
            [CallerMemberName] string callerMemberName = null)
        {
            var operation = string.Format("{0}::{1}", typeOrScope, callerMemberName);
            return new LogTimer(operation, LogType.Information, true);
        }

        public static LogTimer FromScopeNameAndCallerMemberNameWithMessage(string actionMessage, string typeOrScope,
            [CallerMemberName] string callerMemberName = null)
        {
            var operation = string.Format("{0}::{1} {2}", typeOrScope, callerMemberName, actionMessage);
            return new LogTimer(operation, LogType.Information, true);
        }

        public static LogTimer FromScopeNameAndCallerMemberNameAndMode(string typeOrScope,
            LogType logMode = LogType.Information,
            bool logStartAndFinish = false, [CallerMemberName] string callerMemberName = null)
        {
            var operation = string.Format("{0}::{1}", typeOrScope, callerMemberName);
            return new LogTimer(operation, logMode, logStartAndFinish);
        }

        private void LogMessage(string message)
        {
            switch (_logMode)
            {
                case LogType.Information:
                    {
                        //if (_log.IsInfoEnabled)
                        _log.Info(message);
                        break;
                    }
                case LogType.Debug:
                    {
                        //if (_log.IsDebugEnabled)
                        _log.Debug(message);
                        break;
                    }
                case LogType.Warning:
                    {
                        _log.Warn(message);
                        break;
                    }
            }
        }

        public double ExecutionInSeconds { get; private set; }

        public void Dispose()
        {
            double executionTime = DateTime.UtcNow.Subtract(_startTime).TotalSeconds;
            ExecutionInSeconds = executionTime;
            string message = string.Format("Finished [{0}] in [{1}] seconds", _operation, executionTime);

            LogMessage(message);
            _operation = null;
        }
    }
}
