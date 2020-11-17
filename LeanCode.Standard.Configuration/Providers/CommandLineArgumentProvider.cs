using LeanCode.Standard.Collections.Extensions;
using System;
using System.Collections.Generic;

namespace LeanCode.Standard.Configuration
{
    public class CommandLineArgumentProvider : ArgumentProviderBase
    {
        private static readonly Dictionary<string, object> arguments = new Dictionary<string, object>();

        public CommandLineArgumentProvider()
        {
        }

        public CommandLineArgumentProvider(string key, int priority)
            : base(priority)
        {
            HasValue = arguments.ContainsKeyIgnoreCase(ref key);

            if (HasValue)
            {
                Value = arguments[key];
            }
        }

        static CommandLineArgumentProvider()
        {
            var args = Environment.GetCommandLineArgs();

            foreach (var arg in args)
            {
                if (!arg.StartsWith("-") && !arg.StartsWith("/") && !arg.StartsWith("\\"))
                    continue;

                var argLocal = arg.TrimStart('-').TrimStart().TrimStart('/').TrimStart('\\').TrimStart('=');

                string key;
                string value;

                var index = argLocal.IndexOf('=');

                if (index > 0)
                {
                    key = argLocal.Substring(0, index);
                    value = argLocal.Substring(index + 1);
                }
                else
                {
                    key = argLocal;
                    value = "true";

                    LooseKeys.Add(key);
                }

                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                arguments[key] = value;
            }
        }

        public static List<string> LooseKeys { get; } = new List<string>();

        public override IDictionary<string, object> GetArguments() => arguments;
    }
}
