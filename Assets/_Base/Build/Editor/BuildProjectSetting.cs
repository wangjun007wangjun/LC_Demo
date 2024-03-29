using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode.Custom;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

// ReSharper disable InconsistentNaming

namespace BaseFramework.Build
{
    public class BuildProjectSetting : ScriptableObject
    {
        [Header("iOS project setting")]
        [Space(5)]
        [Header("version:")]
        public string iOSVersionName;
        public int iOSBuildCode;
        [Space(5)]
        [Header("exportPath:")]
        public string iOSExportPath;
        [Header("sign team id:")]
        public string signTeamID;
        [Space(5)]
        [Header("capabilities:")]
        public List<iOSCapabilityType> capabilities;
        [Header("frameworks:")]
        public List<string> frameworks;
        [Header("libraries:")]
        public List<string> libraries;
        [Header("properties:")]
        public List<KeyValueItem> properties;
        [Header("plists:")]
        public List<KeyValueItem> plists;
        [Header("Add files, path is relative to Assets/")]
		public List<Object> files;

        [Space(50)]
        [Header("version:")]
        public string androidVersionName;
        public int androidVersionCode;
        [Header("Android project setting")]
        public string androidExportPath;

        #region BuildProjectSetting
        private const string BUILD_PROJECT_SETTING_PATH = "Assets/_Base/Build/ProjectSetting";
        private const string BUILD_PROJECT_SETTING_NAME = "BuildProjectSetting";

        private static readonly string BUILD_PROJECT_SETTING_FILE_PATH = 
            Path.Combine(BUILD_PROJECT_SETTING_PATH, BUILD_PROJECT_SETTING_NAME + ".asset");

        #region singleton

        private static BuildProjectSetting _instance;

        public static BuildProjectSetting instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindSetting();
                }

                if (!_instance)
                {
                    _instance = CreateSettingForce();
                }

                return _instance;
            }
        }

        #endregion

        #region Creaete Build Project Setting

        [MenuItem("Base/Build/Edit Build Project Setting", false, 0)]
        public static void CreateSetting()
        {
            if (instance != null)
            {
                Selection.activeObject = instance;
            }
        }

        private static BuildProjectSetting CreateSettingForce()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(BUILD_PROJECT_SETTING_PATH);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            BuildProjectSetting setting = CreateInstance<BuildProjectSetting>();
            AssetDatabase.CreateAsset(setting, BUILD_PROJECT_SETTING_FILE_PATH);

            return setting;
        }
        #endregion

        #region Find Build Project Setting

        private static BuildProjectSetting FindSetting()
        {
            BuildProjectSetting result = null;

            // Resources.FindObjectsOfTypeAll查找该类型的所有对象
            // 这个函数可以返回加载的Unity物体的任意类型，包含游戏对象、预制体、材质、网格、纹理等等
            BuildProjectSetting[] buildProjectSettings = Resources.FindObjectsOfTypeAll<BuildProjectSetting>();
            if (buildProjectSettings != null && buildProjectSettings.Length > 0)
            {
                result = buildProjectSettings[0];
            }

            if (result)
            {
                return result;
            }
            
            string assetPath = Path.Combine(BUILD_PROJECT_SETTING_FILE_PATH);
            result = AssetDatabase.LoadAssetAtPath<BuildProjectSetting>(assetPath);

            return result;
        }

        #endregion
        #endregion

        #region Clear Build Project Setting
        [MenuItem("Base/Build/Clear Build Project Setting", false, 0)]
        private static void ClearSetting()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(BUILD_PROJECT_SETTING_PATH);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            AssetDatabase.DeleteAsset(BUILD_PROJECT_SETTING_FILE_PATH);
            _instance = null;
        }
        #endregion

        #region Reset Build Project Setting
        [MenuItem("Base/Build/Reset Build Project Setting", false, 0)]
        private static void ResetSetting()
        {
            ClearSetting();
            CreateSetting();
        }
        #endregion
    }

    [System.Serializable]
    public class KeyValueItem
    {
        public string key;
        public string value;
    }
}
