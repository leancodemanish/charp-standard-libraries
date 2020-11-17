using LeanCode.Standard.Configuration.Providers;
using System;

namespace LeanCode.Standard.Configuration.Attributes
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AppConfigArgumentAttribute : ArgumentAttribute
    {
        public AppConfigArgumentAttribute()
        {
            Priority = 2;
        }

        public override IArgumentProvider GetValueProvider()
        {
            return new AppConfigArgumentProvider(Name, Priority);
        }
    }
}
