using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseFramework
{
    public static class StringExtension
    {
        public static bool Any(this string self)
        {
            return IsNotNullAndEmpty(self);
        }
        
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        public static bool IsNotNullAndEmpty(this string self)
        {
            return !string.IsNullOrEmpty(self);
        }

        public static string UppercaseFirst(this string self)
        {
            if (self.IsNullOrEmpty())
                return self;
            return char.ToUpper(self[0]) + self.Substring(1);
        }

        public static string LowercaseFirst(this string self)
        {
            if (self.IsNullOrEmpty())
                return self;
            return char.ToLower(self[0]) + self.Substring(1);
        }

        public static string Format(this string self, params object[] args)
        {
            if (self.IsNullOrEmpty() || args == null || args.Length == 0)
            {
                return self;
            }
            
            try
            {
                return string.Format(self, args);
            }
            catch
            {
                return self;
            }
        }

        public static StringBuilder Append(this string self, string appendStr)
        {
            return new StringBuilder(self).Append(appendStr);
        }

        public static StringBuilder AppendFormat(this string self, string appendFormat, params object[] args)
        {
            return new StringBuilder(self).AppendFormat(appendFormat, args);
        }

        public static string AddPrefix(this string self, object prefix)
        {
            return prefix == null ? self : self.AddPrefix(prefix);
        }

        public static string AddPrefix(this string self, string prefix)
        {
            return prefix.Append(self).ToString();
        }

        public static string AddSuffix(this string self, object suffix)
        {
            return suffix == null ? self : self.AddSuffix(suffix.ToString());
        }

        public static string AddSuffix(this string self, string suffix)
        {
            return self.Append(suffix).ToString();
        }

        public static string Join(this string self, string target, string split = null)
        {
            return self + (split == null ? target : (split + target));
        }

        public static bool EqualsIgnoreCase(this string self, string target)
        {
            if (self.IsNull() || target.IsNull())
            {
                return false;
            }

            return self.ToLower().Equals(target.ToLower());
        }

        public static bool StartsWithIgnoreCase(this string self, string target)
        {
            if (self.IsNull() || target.IsNull())
            {
                return false;
            }

            return self.ToLower().StartsWith(target.ToLower());
        }

        public static bool EndsWithIgnoreCase(this string self, string target)
        {
            if (self.IsNull() || target.IsNull())
            {
                return false;
            }

            return self.ToLower().EndsWith(target.ToLower());
        }

        public static string ReplaceIgnoreCase(this string self, char oldValue, char newValue)
        {
            return self.IsNull()
                       ? self
                       : self.Replace(char.ToLower(oldValue), newValue)
                             .Replace(char.ToUpper(oldValue), newValue);
        }

        public static string ReplaceIgnoreCase(this string self, string oldValue, string newValue)
        {
            if (self.IsNull() || oldValue.IsNull() || newValue.IsNull())
            {
                return self;
            }

            return self.Replace(oldValue.ToUpper(), newValue).Replace(oldValue.ToLower(), newValue);
        }

        public static int ToInt(this string self, int defaultValue = 0)
        {
            return int.TryParse(self, out int retValue) ? retValue : defaultValue;
        }

        public static float ToFloat(this string self, float defaultValue = 0)
        {
            return float.TryParse(self, out float retValue) ? retValue : defaultValue;
        }

        public static bool IsDateTime(this string self, string dateFormat)
        {
            return DateTime.TryParseExact(self, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _);
        }

        public static DateTime ToDateTime(this string self, DateTime defaultValue = default(DateTime))
        {
            return DateTime.TryParse(self, out DateTime retValue) ? retValue : defaultValue;
        }

        public static string Reverse(this string self)
        {
            char[] chars = self.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        public static bool Match(this string self, string pattern)
        {
            return Regex.IsMatch(self, pattern);
        }

        public static IEnumerable<T> SplitTo<T>(this string self, params char[] separator) where T : IConvertible
        {
            return self.Split(separator, StringSplitOptions.None).Select(s => (T)Convert.ChangeType(s, typeof(T)));
        }

        public static IEnumerable<T> SplitTo<T>(this string self, StringSplitOptions options, params char[] separator) where T : IConvertible
        {
            return self.Split(separator, options).Select(s => (T)Convert.ChangeType(s, typeof(T)));
        }

        public static string Cut(this string self, int length)
        {
            if (string.IsNullOrEmpty(self))
                return string.Empty;
            if (length < 0)
                return string.Empty;
            return self.Substring(0, Math.Min(length, self.Length));
        }

        public static string CutLast(this string self, int length)
        {
            if (string.IsNullOrEmpty(self))
                return string.Empty;
            if (length < 0)
                return string.Empty;
            return self.Substring(Math.Max(0, self.Length - length));
        }

        /// <summary>
        /// 是否存在中文字符
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool HasChinese(this string self)
        {
            return Regex.IsMatch(self, @"[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 删除特定字符
        /// </summary>
        /// <param name="self"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static string RemoveString(this string self, params string[] targets)
        {
            return targets.Aggregate(self, (current, t) => current.Replace(t, string.Empty));
        }
    }
}
