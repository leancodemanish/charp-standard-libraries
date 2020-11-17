using LeanCode.Standard.Configuration;
using System;

namespace LeanCode.Standard.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CommandLineArgumentAttribute : ArgumentAttribute
    {
        public CommandLineArgumentAttribute()
        {
            Priority = 1;
        }

        public override IArgumentProvider GetValueProvider()
        {
            return new CommandLineArgumentProvider(Name, Priority);
        }
    }
}
