using System.Collections.Generic;

namespace LeanCode.Standard.Configuration
{
    public abstract class ArgumentProviderBase : IArgumentProvider
    {
        protected ArgumentProviderBase() { }
        protected ArgumentProviderBase(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; set; }
        public bool HasValue { get; protected set; }
        public object Value { get; protected set; }
        public abstract IDictionary<string, object> GetArguments();
    }
}
