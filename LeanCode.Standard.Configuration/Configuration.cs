using LeanCode.Standard.Logging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace LeanCode.Standard.Configuration
{
    /// <summary>
    /// The design is to have a Configuration class that initializes itself for each assembly 
    /// 
    /// Syntax:
    /// All derived Configuration should be called Configuration and be distinguished by the namespace.
    /// That guarantees consistency and easiness of finding and using the configuration.
    /// Each of the config switches is a property of the class. You can use also complex types for the
    /// properties and have custom code that construct them if you wish (by overriding base Configuration)
    /// 
    /// The properties can be initialized using various overriding sources of values for the properties with their priorities.
    /// Each property can be initialized in a number of ways – from command line, app config, external files, database, etc.. 
    /// We can add new providers as we need. 
    /// 
    /// There are attributes to mark the property which values providers should be used to initialize the property. 
    /// Each provider have its priority which enables overrides. 
    /// i.e.CommandLine attribute overrides all the other providers because it's got the highest default priority.
    /// 
    /// However for some custom case you can change priorities of the providers if you need so 
    /// and i.e. have custom file overriding the app config values - the default is reverse.
    /// Check the classes implementing <see cref="IArgumentProvider"/> interface to see what provider are
    /// already implemented and their priorities on their corresponding <see cref="ArgumentAttribute"/> attribute.
    /// 
    /// The order of the attributes above properties doesn’t matter but the practice is to put them in order of their priorities 
    /// for increased readability. 
    /// 
    /// The below class is the implementation of the auto-initialization.
    /// The implementation might look quite tricky but the usage is very simple 
    /// and you don’t need to know the guts of it to use it. See the examples how it's used for the class
    /// above or in the AtlasServiceHost project - AtlasServiceHost.Configuration.
    /// </summary>
    /// <typeparam name="T">The custom Configuration class for a library</typeparam>
    [Serializable]
    public abstract class Configuration<T>
    {
        private static ILogger logger = LoggerFactoryFacade.GetLogger(typeof(Configuration<T>));

        private string configurationType;

        /// <summary>
        /// Configurations will be singleton, no harm in instantiating a logger per instance
        /// </summary>
        protected Configuration()
        {
            // call to GetType here to get the derived type and not the executing type
            configurationType = GetType().Name;
        }

        /// <summary>
        /// Auto initializes the properties of the T configuration class based on the attributes of the properties.
        /// </summary>
        protected virtual void AutoInitialize()
        {
            foreach (var memberInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!memberInfo.CanWrite)
                {
                    continue;
                }

                var attributes = memberInfo.GetCustomAttributes(typeof(ArgumentAttribute), true).Cast<ArgumentAttribute>().ToList();

                attributes.Sort((a1, a2) => a1.Priority - a2.Priority);

                var initialized = false;

                foreach (var attribute in attributes)
                {
                    if (string.IsNullOrEmpty(attribute.Name))
                    {
                        attribute.Name = memberInfo.Name;
                    }

                    var provider = attribute.GetValueProvider();

                    if (provider.HasValue)
                    {
                        memberInfo.SetValue(this, Convert(memberInfo, provider.Value, attribute.IsEncrypted), null);
                        initialized = true;
                        break;
                    }
                }

                if (initialized)
                {
                    continue;
                }

                foreach (DefaultValueAttribute attribute in memberInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true))
                {
                    var convertedValue = Convert(memberInfo, attribute.Value, false);

                    try
                    {
                        memberInfo.SetValue(this, convertedValue, null);
                    }
                    catch (Exception e)
                    {
                        LogAndThrow(memberInfo, convertedValue, e);
                    }

                    break;
                }
            }
        }

        public static object Convert(MemberInfo memberInfo, object value, bool isEncrypted)
        {
            var type = memberInfo is FieldInfo ? ((FieldInfo)memberInfo).FieldType : ((PropertyInfo)memberInfo).PropertyType;

            return TypeExtensions.Convert(type, value, isEncrypted);
        }

        public static TU Convert<TU>(object value, bool isEncrypted = false)
        {
            if (value == null)
            {
                return default(TU);
            }

            var type = typeof(TU);

            return (TU)Convert(type, value, isEncrypted);
        }

        protected static void LogAndThrow(PropertyInfo memberInfo, object convertedValue, Exception e)
        {
            var errorMessage = $"Cannot initialise a config value for property {memberInfo.Name}. Value {convertedValue}";

            throw new ConfigurationException(errorMessage, e);
        }

        public void LogConfiguration()
        {
            var info = this.LogConfiguration<T>();
            logger.Info(info);
        }
    }
}