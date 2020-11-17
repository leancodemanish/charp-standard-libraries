using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanCode.Standard.Logging
{
    public class Log
    {
        public Log(
            Type callingType,
            LogType logType,
            string message,
            params object[] messageParameters)
        {
            CallingType = callingType;
            LogType = logType;
            Message = message;
            MessageParameters = messageParameters.ToList();
        }

        public Type CallingType { get; private set; }
        public LogType LogType { get; private set; }
        public string Message { get; private set; }
        public IReadOnlyList<object> MessageParameters { get; private set; }
    }
}
