using System.Collections.Generic;

namespace LeanCode.Standard.Configuration
{
    public interface IArgumentProvider
    {
        int Priority { get; set; }
        bool HasValue { get; }
        object Value { get; }
        IDictionary<string, object> GetArguments();
    }
}
