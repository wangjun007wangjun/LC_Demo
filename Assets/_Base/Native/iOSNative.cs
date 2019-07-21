using System;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace BaseFramework
{
    public class iOSNative
    {

        public static class SystemSetting
        {
#if UNITY_IOS
        [DllImport ("__Internal")]
        public static extern string GetSettingsURL();

        [DllImport ("__Internal")]
        public static extern void OpenSettings();
#endif
        }

        public static class SystemShare
        {
#if UNITY_IOS
            [DllImport("__Internal")]
            public static extern void shareImage(string imagePath, string text);

            [DllImport("__Internal")]
            public static extern void shareText(string text, string url);
#endif

        }

        public static class Notification
        {
            public static void OpenSystemSetting()
            {
#if UNITY_IOS && !UNITY_EDITOR
                Application.OpenURL(iOSNative.SystemSetting.GetSettingsURL());
#endif
            }
            
            public static void Register()
            {
#if UNITY_IOS && !UNITY_EDITOR
                NotificationServices.RegisterForNotifications(NotificationType.Alert |
                                                              NotificationType.Badge |
                                                              NotificationType.Sound);
#endif
            }

            public static bool IsEnabled()
            {
#if UNITY_IOS && !UNITY_EDITOR
                return NotificationServices.enabledNotificationTypes > 0;
#endif
                return false;
            }

            public static void Set(string message,
                                                int addDay,
                                                int hour,
                                                bool isRepeatDay,
                                                bool isWeek = false)
            {
#if UNITY_IOS && !UNITY_EDITOR
                DateTime nowTime = DateTime.Now;
                int year = nowTime.Year;
                int month = nowTime.Month;
                int day = nowTime.Day;

                DateTime newDate = new DateTime(year, month, day, hour, 0, 0);
                newDate = newDate.AddDays(addDay);

                Set(message, newDate, isRepeatDay, isWeek);
#endif
            }

            //本地推送 你可以传入一个固定的推送时间
            public static void Set(string message, DateTime newDate, bool isRepeatDay, bool isWeek = false)
            {
#if UNITY_IOS && !UNITY_EDITOR
                //推送时间需要大于当前时间
                if (newDate <= DateTime.Now) return;

                LocalNotification localNotification = new LocalNotification
                {
                    fireDate = newDate, alertBody = message, applicationIconBadgeNumber = 1
                };
                if (isRepeatDay)
                {
                    localNotification.repeatInterval = isWeek ? CalendarUnit.Week : CalendarUnit.Day;
                }

                localNotification.soundName = LocalNotification.defaultSoundName;
                NotificationServices.ScheduleLocalNotification(localNotification);
#endif
            }

            /// <summary>
            /// clean and cancel
            /// </summary>
            public static void Clean()
            {
#if UNITY_IOS && !UNITY_EDITOR
                LocalNotification l = new LocalNotification {applicationIconBadgeNumber = -1};
                NotificationServices.CancelAllLocalNotifications();
                NotificationServices.ClearLocalNotifications();
#endif
            }
        }

        public static class Local
        {
#if UNITY_IOS
            [DllImport ("__Internal")]
            public static extern string getLanguage();
        
            [DllImport ("__Internal")]
            public static extern string getCountry();
#endif
        }
    }
}