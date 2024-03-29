﻿using System;

namespace BaseFramework
{
    public static class ArrayExtension
    {
        public static bool Any<T>(this T[] self)
        {
            return IsNotNullAndEmpty(self);
        }

        public static bool IsNullOrEmpty<T>(this T[] self)
        {
            return self.IsNull() || self.Length == 0;
        }

        public static bool IsNotNullAndEmpty<T>(this T[] self)
        {
            return !self.IsNullOrEmpty();
        }

        public static T[] ForEach<T>(this T[] self, Action<int, T> action)
        {
            if (self.IsNull())
            {
                Log.W(typeof(ArrayExtension), $"{typeof(T)} Array is null!");
            }
            else
            {
                for (int i = 0; i < self.Length; ++i)
                {
                    action(i, self[i]);
                }
            }

            return self;
        }

        public static bool IsEmptyOrNull<T>(this T[,] self)
        {
            return self.IsNull() || self.Length == 0;
        }

        public static bool IsNotEmptyAndNull<T>(this T[,] self)
        {
            return !self.IsEmptyOrNull();
        }

        public static bool Any<T>(this T[,] self)
        {
            return IsNotEmptyAndNull(self);
        }

        public static bool IsNullOrEmpty<T>(this T[,] self)
        {
            return self.IsNull() || self.Length == 0;
        }

        public static bool IsNotNullAndEmpty<T>(this T[,] self)
        {
            return !self.IsNullOrEmpty();
        }

        public static T[,] ForEach<T>(this T[,] self, Action<int, int, T> action)
        {
            if (self.IsNull())
            {
                Log.W(typeof(ArrayExtension), "Array is null!");
            }
            else
            {
                for (int i = 0; i < self.GetLength(0); ++i)
                {
                    for (int j = 0; j < self.GetLength(1); ++j)
                    {
                        action(i, j, self[i, j]);
                    }
                }
            }

            return self;
        }

        public static T[,] ForEach<T>(this T[,] self, Action<T, T> action)
        {
            if (self.IsNull())
            {
                Log.W(typeof(ArrayExtension), "Array is null!");
            }
            else
            {
                for (int i = 0; i < self.GetLength(0); ++i)
                {
                    action(self[i, 0], self[i, 1]);
                }
            }

            return self;
        }
        
        public static T[,] ForEach<T>(this T[,] self, Action<int, T, T> action)
        {
            if (self.IsNull())
            {
                Log.W(typeof(ArrayExtension), "Array is null!");
            }
            else
            {
                for (int i = 0; i < self.GetLength(0); ++i)
                {
                    action(i, self[i, 0], self[i, 1]);
                }
            }

            return self;
        }

        public static T[] Copy<T>(this T[] self)
        {
            if (self.IsNotNullAndEmpty())
            {
                T[] temp = new T[self.Length];
                Array.Copy(self, temp, self.Length);
                return temp;
            }
            return null;
        }
    }
}
