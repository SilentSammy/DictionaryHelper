using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;

namespace DictionaryHelper
{
    public static class DictionaryUtilities
    {
        // Allow recursiveness
        public static Dictionary<string, object> GetPropertyValues(this object target)
        {
            var d = new Dictionary<string, object>();
            foreach (var p in target.GetType().GetProperties())
                d[p.Name] = p.GetValue(target);
            return d;
        }
        public static Dictionary<T1, T2> MergeDictionaries<T1, T2>(params IDictionary<T1, T2>[] dictionaries)
        {
            var md = new Dictionary<T1, T2>();
            foreach (var d in dictionaries.Reverse())
                foreach (var kvp in d)
                    md[kvp.Key] = kvp.Value;
            return md;
        }
        public static Dictionary<T1, T2> Translate<T1, T2>(Dictionary<T1, T2> dict, Dictionary<T1, T1> translations)
        {
            var td = new Dictionary<T1, T2>();
            foreach (var kvp in dict)
            {
                if (translations.ContainsKey(kvp.Key))
                    td[translations[kvp.Key]] = kvp.Value;
                else td[kvp.Key] = kvp.Value;
            }
            return td;
        }
    }
}
