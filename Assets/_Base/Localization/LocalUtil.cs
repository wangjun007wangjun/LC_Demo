namespace BaseFramework
{
    public class LocalUtil
    {
        public static string GetCountry(string defaultCountry = "us")
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidNative.Local.GetCountry(defaultCountry);
#elif UNITY_IOS && !UNITY_EDITOR
            iOSNative.Local.getCountry();
#endif
            return defaultCountry;
        }

        public static string GetLanguage(string defaultLanguage = "en")
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidNative.Local.GetLanguage(defaultLanguage);
#elif UNITY_IOS && !UNITY_EDITOR
            iOSNative.Local.getLanguage();
#endif
            return defaultLanguage;
        }
    }
}