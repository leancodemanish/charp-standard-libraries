using LeanCode.Standard.Configuration;
using System;

namespace LeanCode.Standard.Configuration
{
    public abstract class ArgumentAttribute : Attribute
    {
        public int Priority { get; set; }
        public string Name { get; set; }
        public bool IsEncrypted { get; set; }
        public abstract IArgumentProvider GetValueProvider();
    }
}
