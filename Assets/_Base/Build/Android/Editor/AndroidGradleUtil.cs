#if UNITY_ANDROID && UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BaseFramework.Build
{
    [InitializeOnLoad]
    public static class AndroidGradleUtil
    {
        private const string SOURCE_PATH = "/_Base/Build/Android/Res/mainTemplate.gradle";
        private static readonly string TARGET_PATH = Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle");

        static AndroidGradleUtil()
        {
            BuildUtil.onPreProcessBuild += CopyGradle;        
        }

        private static void CopyGradle()
        {
            Log.I("AndroidGradleUtil", "Copy gradle file");

            FileInfo fileInfo = new FileInfo(TARGET_PATH);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }
            File.Copy(Application.dataPath + SOURCE_PATH, TARGET_PATH);
            AssetDatabase.Refresh();
        }

        [MenuItem("Base/Build/Editor Android Gradle")]
        public static void EditorGradle()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>("Assets" + SOURCE_PATH);
        }
    }
}
#endif