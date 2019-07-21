using System;

namespace BaseFramework
{
    public static class Log
    {
        public enum LogLevel
        {
            Verbose = 2,
            Debug = 3,
            Info = 4,
            Warn = 5,
            Error = 6,
            Assert = 7
        }

        public static LogLevel logLevel { private get; set; } = LogLevel.Verbose;

        public static bool logUnityStack { private get; set; } = false;

        private const string TAG_FORMAT = "[Unity-{0}]";

        private static void GetLogInfo(object obj, object msg, out string tag, out string message, params object[] args)
        {
            if (obj is Type type)
            {
                tag = TAG_FORMAT.Format((object)type.ReadableName());
            }
            else
            {
                tag = obj.GetLogTag(TAG_FORMAT);
            }

            tag = tag.AddSuffix(" ");

            message = msg != null ? msg.ToString().Format(args) : tag;
        }

        public static void V(object obj, object msg, params object[] args)
        {
            if (logLevel > LogLevel.Verbose)
            {
                return;
            }

            GetLogInfo(obj, msg, out string tag, out string message, args);

            if (logUnityStack)
            {
                UnityEngine.Debug.Log(tag + " " + message);
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log(tag + " " + message);
#elif UNITY_ANDROID
                Log2Android.V(tag, message);
#elif UNITY_IOS
                Log2iOS.v(tag + " " + message);
#endif
            }
        }


        public static void D(object obj, object msg, params object[] args)
        {
            if (logLevel > LogLevel.Debug)
            {
                return;
            }

            GetLogInfo(obj, msg, out string tag, out string message, args);

            if (logUnityStack)
            {
                UnityEngine.Debug.Log(tag + " " + message);
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log(tag + " " + message);
#elif UNITY_ANDROID
                Log2Android.D(tag, message);
#elif UNITY_IOS
                Log2iOS.d(tag + " " + message);
#endif
            }
        }

        public static void I(object obj, object msg, params object[] args)
        {
            if (logLevel > LogLevel.Info)
            {
                return;
            }

            GetLogInfo(obj, msg, out string tag, out string message, args);

            if (logUnityStack)
            {
                UnityEngine.Debug.Log(tag + " " + message);
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log(tag + " " + message);
#elif UNITY_ANDROID
                Log2Android.I(tag, message);
#elif UNITY_IOS
                Log2iOS.i(tag + " " + message);
#endif
            }
        }

        public static void W(object obj, object msg, params object[] args)
        {
            if (logLevel > LogLevel.Warn)
            {
                return;
            }

            GetLogInfo(obj, msg, out string tag, out string message, args);

            if (logUnityStack)
            {
                UnityEngine.Debug.LogWarning(tag + " " + message);
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning(tag + " " + message);
#elif UNITY_ANDROID
                Log2Android.W(tag, message);
#elif UNITY_IOS
                Log2iOS.w(tag + " " + message);
#endif
            }
        }

        public static void E(object obj, object msg, params object[] args)
        {
            if (logLevel > LogLevel.Error)
            {
                return;
            }

            GetLogInfo(obj, msg, out string tag, out string message, args);

            if (logUnityStack)
            {

                UnityEngine.Debug.LogError(tag + " " + message);
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogError(tag + " " + message);
#elif UNITY_ANDROID
                Log2Android.E(tag, message);
#elif UNITY_IOS
                Log2iOS.e(tag + " " + message);
#endif
            }
        }

        public static void E(this Exception self, object msg, params object[] args)
        {
            E((object)self, msg, args);
        }
    }
}
