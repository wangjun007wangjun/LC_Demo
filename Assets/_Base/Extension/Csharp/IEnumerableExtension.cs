﻿using System;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace BaseFramework
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            if (self.IsNull())
            {
                Log.W(typeof(ArrayExtension), "IEnumerable is null!");
            }
            else
            {
                foreach (T item in self)
                {
                    action(item);
                }
            }

            return self;
        }
    }
}
