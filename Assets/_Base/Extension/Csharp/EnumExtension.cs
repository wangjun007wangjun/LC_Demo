using System;

namespace BaseFramework
{
#if UNITY_EDITOR
    using UnityEditor;

    public class EnumExtensionExample
    {
        private enum ExampleEnum
        {
            One = 1, Two, Three
        }

        [MenuItem("Base/Example/Extension/Enum")]
        public static void RunExample()
        {
            ExampleEnum example = "THREE".ToEnum<ExampleEnum>();

            Log.I(typeof(EnumExtensionExample).Name, "ExampleEnum.TWO = " + (int)ExampleEnum.Two);
            Log.I(typeof(EnumExtensionExample).Name, "example = " + example);
            Log.I(typeof(EnumExtensionExample).Name, "2 = " + (ExampleEnum)2);
            Log.I(typeof(EnumExtensionExample).Name, "default = " + default(ExampleEnum));
        }
    }
#endif

    public static class EnumExtension
    {
        public static T ToEnum<T>(this string self)
        {
            if(self.Any())
            {
                return (T)Enum.Parse(typeof(T), self);
            }

            return default(T);
        }
    }
}
