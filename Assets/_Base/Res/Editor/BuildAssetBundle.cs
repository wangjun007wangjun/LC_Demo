using System.IO;
using UnityEditor;
using UnityEngine;

namespace BaseFramework.Build
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class BuildAssetBundle
    {
        static BuildAssetBundle()
        {
            BuildTargetChangedHelper.changeCallback += BuildAssetBundleAuto;
        }

        public static void BuildAssetBundleAuto(BuildTarget target)
        {
            Log.I("BuildAssetBundle","Build AssetBundle Auto!");
            DoBuildAssetBundle();
        }

        [MenuItem("Base/AssetBundle/Build", priority = 0)]
        public static void DoBuildAssetBundle()
        {
            string assetBundleDirectory = Application.streamingAssetsPath;
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

            BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                            BuildAssetBundleOptions.None,
                                            target);

            AssetDatabase.Refresh();
        }

        [MenuItem("Base/AssetBundle/Clean", priority = 1)]
        public static void CleanAssetBundle()
        {
            string assetBundleDirectory = Application.streamingAssetsPath;
            if (Directory.Exists(assetBundleDirectory))
            {
                Directory.Delete(assetBundleDirectory, true);
            }
        }

        [MenuItem("Base/AssetBundle/Rebuild", priority = 2)]
        public static void RebuildAssetBundle()
        {
            CleanAssetBundle();

            DoBuildAssetBundle();
        }
        
        [MenuItem("Base/AssetBundle/CleanCache", priority = 3)]
        public static void ClearCacheAssetBundle()
        {
            Debug.Log(Caching.ClearCache() ? "Successfully cleaned the cache." : "Cache is being used.");
        }

        [MenuItem("Base/AssetBundle/Export Current Info")]
        public static void ExportCurrentMessage()
        {
            AssetBundleManager.instance.ExportCurrentMessage();
        }
    }
#endif
}
