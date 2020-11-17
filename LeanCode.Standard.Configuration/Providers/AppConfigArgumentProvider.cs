using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace LeanCode.Standard.Configuration.Providers
{
    public class AppConfigArgumentProvider : ArgumentProviderBase
    {
        private static readonly Dictionary<string, object> _arguments;

        static AppConfigArgumentProvider()
        {
            _arguments = new Dictionary<string, object>();
            foreach(var appSetting in ConfigurationManager.AppSettings)
            {
                if(appSetting is string)
                {
                    _arguments.Add((string)appSetting, ConfigurationManager.AppSettings[(string)appSetting]);
                }
            }
        }

        public AppConfigArgumentProvider() { }

        public AppConfigArgumentProvider(string key, int priority)
        {
            Priority = priority;
            Value = ConfigurationManager.AppSettings[key];
            HasValue = Value != null;
        }

        public override IDictionary<string, object> GetArguments()
        {
            return _arguments;
        }
    }
}
