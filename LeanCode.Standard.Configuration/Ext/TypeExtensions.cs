using LeanCode.Standard.Encryption;
using LeanCode.Standard.Logging;
using Newtonsoft.Json;
using System;
using LeanCode.Standard.TypeExt;

namespace LeanCode.Standard.Configuration
{
    public static class TypeExtensions
    {
        private static ILogger logger = LoggerFactoryFacade.GetLogger(typeof(TypeExtensions));

        public static bool IsParsable(this Type type)
        {
            var tryParseMethod = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });

            return tryParseMethod != null;
        }

        public static object TryParse(Type type, object value) => TryParse(type, value, true);

        public static TY TryParse<TY>(string s) => (TY)TryParse(typeof(TY), s);

        public static object TryParse(Type type, object value, bool throwException)
        {
            if (value == null)
            {
                return null;
            }

            value = value.ToString();

            var tryParseMethod = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });

            if (tryParseMethod == null)
            {
                if (throwException)
                {
                    throw new ConfigurationException($"Type {type.FullName} doesn't have TryParse method so cannot convert.");
                }

                return null;
            }

            object[] args = { value, null };

            var parsed = (bool)tryParseMethod.Invoke(null, args);

            if (parsed)
            {
                return args[1];
            }

            throw new ConfigurationException($"Cannot parse value {value} in to {type.FullName} type.");
        }

        public static object Convert(Type type, object value, bool isEncrypted)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (value == null)
            {
                return null;
            }

            if (isEncrypted && value is string)
            {
                value = AesEncryption.Decrypt((string)value);
            }

            if (type == value.GetType())
            {
                return value;
            }

            if (type == typeof(string))
            {
                return value;
            }

            if (type == typeof(DateTime))
            {
                var parsedDateTime = value.ToString().TryParseAsDateTime();

                if (parsedDateTime.HasValue)
                {
                    return parsedDateTime.Value;
                }

                throw new ConfigurationException($"Config setting of type {type} cannot be initialized as the value {value} is not in yyyyMMdd format.");
            }

            if (type.IsEnum)
            {
                try
                {
                    return Enum.Parse(type, value.ToString(), true);
                }
                catch (Exception e)
                {
                    throw new ConfigurationException($"Config setting of type {type} cannot be initialized as the value {value} is not a valid enum value.", e);
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>).GetGenericTypeDefinition())
            {
                var typeParameterType = type.GetGenericArguments()[0];
                if (typeParameterType == value.GetType())
                {
                    return value;
                }
            }

            if (typeof(IJsonSerialisedConfig).IsAssignableFrom(type))
            {
                var serialisedValue = value.ToString();

                try
                {
                    var deserialise = JsonConvert.DeserializeObject(serialisedValue, type);

                    return deserialise;
                }
                catch (Exception ex)
                {
                    logger.Error($"Failed to deserialise a configration property marked as ISerialisedConfig, property value: {value}");

                    return null;
                }
            }

            try
            {
                if (!(value is string) && value is IConvertible)
                {
                    return System.Convert.ChangeType(value, type);
                }
            }
            catch (InvalidCastException)
            {
            }

            var convertedValue = TryParse(type, value, false);

            if (convertedValue != null)
            {
                return convertedValue;
            }

            return value;
        }
    }
}
