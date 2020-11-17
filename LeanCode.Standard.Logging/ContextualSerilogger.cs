using log4net.Repository;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace LeanCode.Standard.Logging
{
    public class ContextualSerilogger : SerilogLogger
    {
        internal ContextualSerilogger(Serilog.ILogger logger, string name): base(logger, name)
        {            
        }

        public override ILogger WithContext([CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            throw new NotImplementedException("Logger context is already configured");
        }
    }
}
