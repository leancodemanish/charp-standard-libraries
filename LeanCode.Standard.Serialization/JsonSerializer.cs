using Newtonsoft.Json;
using System;

namespace LeanCode.Standard.Serialization
{
    public class JsonSerializer
    {
        public static string ToJSON<T>(T anyObj)
        {
            return JsonConvert.SerializeObject(anyObj);
        }

        public static T Deserialize<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
