using System;
using System.Collections.Generic;

namespace BaseFramework
{
    public static class DictionaryExtension 
    {
        public static bool IsEmptyOrNull<TK, TV>(this IDictionary<TK, TV> self)
        {
            return self.IsNull() || self.Count == 0;
        }

        public static bool IsNotEmptyAndNull<TK, TV>(this IDictionary<TK, TV> self)
        {
            return !self.IsEmptyOrNull();
        }

        public static bool IsNullOrEmpty<TK, TV>(this IDictionary<TK, TV> self)
        {
            return self.IsNull() || self.Count == 0;
        }

        public static bool IsNotNullAndEmpty<TK, TV>(this IDictionary<TK, TV> self)
        {
            return !self.IsNullOrEmpty();
        }

        public static IDictionary<TK, TV> ForEach<TK, TV>(this IDictionary<TK, TV> self, Action<TK, TV> action)
        {
            if (self.IsNull())
            {
                Log.W(typeof(DictionaryExtension), "Dictionary is null");
            }
            else
            {
                foreach(KeyValuePair<TK, TV> item in self)
                {
                    action(item.Key, item.Value);
                }
            }

            return self;
        }

        public static IDictionary<TK, TV> AddRange<TK, TV>(this IDictionary<TK, TV> self,
                                                      IDictionary<TK, TV> sourceDic,
                                                      bool isOverride = false)
        {
            if (self.IsNull())
            {
                Log.W(typeof(DictionaryExtension), "Dictionary is null");
            } 
            else
            {
                if(sourceDic.IsEmptyOrNull())
                {
                    Log.W(typeof(DictionaryExtension), "Dictionary is null");
                    return self;
                }

                foreach (KeyValuePair<TK, TV> item in sourceDic)
                {
                    if(self.ContainsKey(item.Key))
                    {
                        if(isOverride)
                        {
                            self[item.Key] = item.Value;
                        }
                    }
                    else
                    {
                        self.Add(item.Key, item.Value);
                    }
                }
            }

            return self;
        }

        public static string ToCustomString<TK, TV>(this IDictionary<TK, TV> self)
        {
            if (self.IsEmptyOrNull())
            {
                return "{}";
            }

            return $"{{{string.Join(",", self)}}}";
        }
    }
}
