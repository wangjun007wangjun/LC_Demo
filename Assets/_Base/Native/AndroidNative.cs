using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BaseFramework
{
    public class AndroidNative
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject _currentActivity;
        public static AndroidJavaObject currentActivity =>
            _currentActivity ?? (_currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                                    .GetStatic<AndroidJavaObject>("currentActivity"));
#endif

        public static AndroidJavaObject ConvertDictToHashMap(Dictionary<string, string> dict)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (dict == null)
            {
                return null;
            }
            AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");
            foreach (KeyValuePair<string, string> pair in dict)
            {
                map.Call<string>("put", pair.Key, pair.Value);
            }
            return map;
#else
            return null;
#endif
        }

        public static class AssetBundleUtil
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            private static AndroidJavaClass _assetBundleUtil;
            public static AndroidJavaClass assetBundleUtil =>
                _assetBundleUtil ?? new AndroidJavaClass("com.android4Unity.util.AssetBundleUtil");
#endif

            public static byte[] ReadAllBytes(string name)
            {
#if UNITY_ANDROID && !UNITY_EDITOR

                if (name.IsNullOrEmpty())
                    return null;
                return assetBundleUtil.CallStatic<byte[]>("loadBytes", name);
#endif
                return File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, name));
            }
        }

        public static class Notification
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            private static AndroidJavaClass _notificationClass;
            public static AndroidJavaClass notificationClass =>
                _notificationClass ?? new AndroidJavaClass("com.android4Unity.util.NotificationUtil");
#endif
            
            public static void Init(string unityPlayerActivityName)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                notificationClass.CallStatic("initContextInfo",
                                             AndroidNative.currentActivity,
                                             unityPlayerActivityName);
#endif
            }
            
            public static void Set(string message, int addDay, int hour)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                DateTime nowTime = DateTime.Now;
                int year = nowTime.Year;
                int month = nowTime.Month;
                int day = nowTime.Day;

                DateTime newDate = new DateTime(year, month, day, hour, 0, 0);
                newDate = newDate.AddDays(addDay);
                Set(DateUtil.CurrentTimeMillis(newDate, true), 1, Application.productName, message);
#endif
            }

            public static void Set(long triggerAtMillis,
                                    int type,
                                    string title,
                                    string text)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                notificationClass.CallStatic("setNotification", triggerAtMillis, type, title, text);
#endif
            }

            public static void Set(string message, int addDay, int intervalDay, int hour)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                DateTime nowTime = DateTime.Now;
                int year = nowTime.Year;
                int month = nowTime.Month;
                int day = nowTime.Day;

                DateTime newDate = new DateTime(year, month, day, hour, 0, 0);
                newDate = newDate.AddDays(addDay);

                long intervalMillis = intervalDay * 24 * 60 * 60 * 1000;

                Set(DateUtil.CurrentTimeMillis(newDate, true),
                    intervalMillis,
                    1,
                    Application.productName,
                    message);
#endif
            }
            
            public static void Set(string titles, string messages, int addDay, int intervalDay, int hour, string loopKey, int loopIndex = 0)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                DateTime nowTime = DateTime.Now;
                int year = nowTime.Year;
                int month = nowTime.Month;
                int day = nowTime.Day;

                DateTime newDate = new DateTime(year, month, day, hour, 0, 0);
                newDate = newDate.AddDays(addDay);

                long intervalMillis = intervalDay * 24 * 60 * 60 * 1000;

                Set(DateUtil.CurrentTimeMillis(newDate, true),
                    intervalMillis,
                    1,
                    titles,
                    messages,
                    loopKey, loopIndex);
#endif
            }

            public static void Set(long triggerAtMillis,
                                    long intervalMillis,
                                    int type,
                                    string title,
                                    string text)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                notificationClass.CallStatic("setNotificationRepeating", triggerAtMillis, intervalMillis, type, title, text);
#endif
            }
            
            public static void Set(long triggerAtMillis,
                                   long intervalMillis,
                                   int type,
                                   string title,
                                   string text,
                                   string loopKey,
                                   int loopIndex = 0)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                notificationClass.CallStatic("setNotificationRepeating",
                                             triggerAtMillis, intervalMillis,
                                             type, title, text, loopKey, loopIndex);
#endif
            }

            /// <summary>
            /// clean and cancel
            /// </summary>
            public static void Clean()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                notificationClass.CallStatic("cancelNotification");
#endif
            }
        }

        public static class Local
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            private static AndroidJavaClass _localUtil;
            public static AndroidJavaClass localUtil =>
                _localUtil ?? new AndroidJavaClass("com.android4Unity.util.LocaleUtil");
#endif
            public static string GetCountry(string defaultCountry = "us")
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return localUtil.CallStatic<string>("GetCountry");
#endif
                return defaultCountry;
            }

            public static string GetLanguage(string defaultLanguage = "en")
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return localUtil.CallStatic<string>("GetLanguage");
#endif
                return defaultLanguage;
            }
        }
            
    }
}