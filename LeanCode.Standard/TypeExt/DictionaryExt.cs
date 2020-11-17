using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanCode.Standard.Collections.Extensions
{
    public static class DictionaryExt
    {
        public static bool ContainsKeyIgnoreCase<T>(this IDictionary<string, T> dictionary, ref string key)
        {
            var originalkey = key;

            if (dictionary.ContainsKey(key))
            {
                return true;
            }

            key = key.ToLower();

            if (dictionary.ContainsKey(key))
            {
                return true;
            }

            key = key.ToUpper();

            if (dictionary.ContainsKey(key))
            {
                return true;
            }

            var checkKey = key;

            var foundKey = dictionary.Keys.SingleOrDefault(k => string.Compare(k, checkKey, StringComparison.CurrentCultureIgnoreCase) == 0);

            if (foundKey != null)
            {
                key = foundKey;

                return true;
            }

            key = originalkey;

            return false;
        }
    }
}
