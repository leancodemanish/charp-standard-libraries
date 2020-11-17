using System;
using System.Runtime.CompilerServices;

namespace LeanCode.Standard.Logging
{
    public interface ILogger
    {
        bool IsDebugEnabled { get; }
        void Debug(object message);
        void Debug(string messageTemplate, params object[] args);

        void Info(object message);
        void Info(string messageTemplate, params object[] args);

        void Warn(object message);
        void Warn(string messageTemplate, params object[] args);

        [Obsolete]
        void Error(object message);
        void Error(Exception exception, string messageTemplate, params object[] args);        
        void Error(object message, Exception exception);

        void Fatal(object message);
        void Fatal(object message, Exception exception);

        ILogger WithContext([CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0);

    }
}
