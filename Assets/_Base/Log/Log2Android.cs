#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;

namespace BaseFramework
{

    public class Log2Android
    {
        private const string ANDROID_LOG_CLASS = "android.util.Log";

        private static AndroidJavaClass _logClass;
        private static AndroidJavaClass logClass
        {
            get { return _logClass ?? (_logClass = new AndroidJavaClass(ANDROID_LOG_CLASS)); }
        }

        public static void V(string tag, string message)
        {
            logClass.CallStatic<int>("v", tag, message);
        }

        public static void D(string tag, string message)
        {
            logClass.CallStatic<int>("d", tag, message);
        }

        public static void I(string tag, string message)
        {
            logClass.CallStatic<int>("i", tag, message);
        }

        public static void W(string tag, string message)
        {
            logClass.CallStatic<int>("w", tag, message);
        }

        public static void E(string tag, string message)
        {
            logClass.CallStatic<int>("e", tag, message);
        }
    }
}
#endif
