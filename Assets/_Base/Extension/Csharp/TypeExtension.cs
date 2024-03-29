﻿using System;
using System.Text;

namespace BaseFramework
{
#if UNITY_EDITOR
    using UnityEditor;

    public class TypeExtensionExample
    {
        enum TypeExtensionExampleEnum
        {

        }

        [MenuItem("Base/Example/Extension/Type")]
        public static void RunExample()
        {
            bool isSimple = true;

            // true
            isSimple = typeof(string).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "string", isSimple);
            isSimple = typeof(int).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "int", isSimple);
            isSimple = typeof(float).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "flot", isSimple);
            isSimple = typeof(double).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "double", isSimple);
            isSimple = typeof(decimal).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "decimal", isSimple);
            isSimple = typeof(TypeExtensionExampleEnum).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "Enum", isSimple);
            isSimple = typeof(int?).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "int?", isSimple);
            isSimple = typeof(decimal?).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "decimal?", isSimple);
            isSimple = typeof(StringComparison?).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "stringComparsion?", isSimple);
            isSimple = typeof(StringBuilder).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "StringBuilder", isSimple);

            // false
            isSimple = typeof(object).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "object", isSimple);
            isSimple = typeof(UnityEngine.Vector2).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "Vector2", isSimple);
            isSimple = typeof(UnityEngine.Vector2?).IsSimple();
            Log.I(typeof(TypeExtensionExample).Name, "Is simple type of {0} = {1}", "Vector2?", isSimple);

        }
    }
#endif

    public static class TypeExtension
    {
        /// <summary>
        /// 判断是否是简单类型
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsSimple(this Type self)
        {
            if (self.IsGenericType && self.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(self.GetGenericArguments()[0]);
            }
            return self.IsPrimitive
              || self.IsEnum
              || self == typeof(string)
              || self == typeof(decimal)
              || self == typeof(StringBuilder);
        }

        public static string ReadableName(this Type self, string format = "<{0}>")
        {
            if (self.IsGenericType && self.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return self.Name.Split('`')[0] + string.Format(format, self.GetGenericArguments()[0].Name);
            }
            return self.Name;
        }

        public static bool IsTypeof<T>(this Type self) 
        {
            return self == typeof(T);
        }

        public static bool IsNotTypeof<T>(this Type self)
        {
            return self != typeof(T);
        }

        public static bool EqualsType<T>(this Type self)
        {
            return self == typeof(T);
        }

        public static string GetTypeName<T>()
        {
            return typeof(T).Name;
        }
    }
}
