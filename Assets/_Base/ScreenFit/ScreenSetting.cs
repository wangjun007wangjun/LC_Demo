using System.IO;
using UnityEditor;
using UnityEngine;

public class ScreenSetting : ScriptableObject
{
    public float idealScreenSizeMin = 720;
    public float idealScreenSizeMax = 1280;
    public float defaultMatch = 0.5f;

    #region ScreenSetting

    private const string SETTING_PATH = "Assets/_Base/ScreenFit/Resources";
    private const string SETTING_NAME = "ScreenSetting";

    private static readonly string SETTING_FILE_PATH =
        Path.Combine(SETTING_PATH, SETTING_NAME + ".asset");

    #endregion

    #region singleton

    private static ScreenSetting _instance;

    public static ScreenSetting instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindSetting();
            }

#if UNITY_EDITOR
            if (!_instance)
            {
                _instance = CreateSettingForce();
            }
#endif

            return _instance;
        }
    }

    private static ScreenSetting FindSetting()
    {
        ScreenSetting result = null;

        result = Resources.Load<ScreenSetting>("ScreenSetting");

#if UNITY_EDITOR
        if (!result)
        {
            result = AssetDatabase.LoadAssetAtPath<ScreenSetting>(SETTING_FILE_PATH);
        }
#endif

        return result;
    }

#if UNITY_EDITOR
    [MenuItem("Base/Screen/Edit Screen Setting", false, 0)]
    public static void CreateSetting()
    {
        if (instance != null)
        {
            Selection.activeObject = instance;
        }
    }

    private static ScreenSetting CreateSettingForce()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(SETTING_PATH);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        ScreenSetting setting = CreateInstance<ScreenSetting>();
        AssetDatabase.CreateAsset(setting, SETTING_FILE_PATH);

        return setting;
    }
#endif

    #endregion
}