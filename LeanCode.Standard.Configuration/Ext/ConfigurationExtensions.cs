using LeanCode.Standard.Logging;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LeanCode.Standard.Configuration
{
    public static class ConfigurationExtensions
    {
        public static string LogConfiguration<TU>() => LogConfiguration<TU>(null);

        public static string LogConfiguration(this object o) => LogConfiguration(o);

        public static string LogConfiguration<TU>(this object o) => $"Configuration for: {Print((TU)o)}";

        public static string Print<T>(this T o)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{1}{0}{1}{{{1}", typeof(T).FullName, Environment.NewLine);

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.CanRead)
                {
                    string ignoreAttribute = null;

                    var ignore = propertyInfo.GetCustomAttributes(true).Any(attribute =>
                    {
                        if (attribute is XmlIgnoreAttribute || attribute is NonSerializedAttribute || attribute is LogIgnoreAttribute)
                        {
                            ignoreAttribute = attribute.GetType().Name;
                            return true;
                        }

                        return false;
                    });

                    if (ignore)
                    {
                        sb.AppendLine($"\t{propertyInfo.Name} = [{ignoreAttribute}] - not read");

                        continue;
                    }

                    if (propertyInfo.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        sb.AppendLine($"\t{propertyInfo.Name} = {Environment.NewLine}\t{{");

                        IEnumerable enumerable = (IEnumerable)propertyInfo.GetValue(propertyInfo.GetGetMethod().IsStatic ? (object)null : o, null);

                        if (enumerable == null)
                        {
                            sb.AppendLine("\t\tnull");
                        }
                        else
                        {
                            var i = 0;

                            foreach (object element in enumerable)
                            {
                                sb.AppendLine($"\t\t[{i++}] = {element}");
                            }
                        }

                        sb.AppendLine("\t}}");
                    }
                    else
                    {
                        if (!propertyInfo.GetGetMethod().IsStatic && o == null)
                        {
                            continue;
                        }

                        if (propertyInfo.GetIndexParameters().Length > 0)
                        {
                            sb.AppendLine($"\t{propertyInfo.Name} = Indexed Property");
                        }
                        else
                        {
                            sb.AppendFormat(
                                propertyInfo.PropertyType == typeof(DateTime) 
                                ? "\t{0} = {1:yyyy-MM-dd HH:mm:ss}{2}" 
                                : "\t{0} = {1}{2}", propertyInfo.Name, propertyInfo.GetValue(propertyInfo.GetGetMethod().IsStatic 
                                    ? (object)null 
                                    : o, null) ?? "null", Environment.NewLine);
                        }
                    }
                }
            }

            sb.AppendLine("}}");

            return sb.ToString();
        }
    }
}
