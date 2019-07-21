using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BaseFramework.Lunch
{
    [InitializeOnLoad]
    public class GameLaunch
    {
        private static readonly string KEY = Application.identifier.ToUpper() + "_ORIGIN_SCENE_PATH";

        private static string originScenePath
        {
            get => EditorPrefs.GetString(KEY);
            set => EditorPrefs.SetString(KEY, value);
        }

        static GameLaunch()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        private static void PlayModeStateChanged(PlayModeStateChange playModeState)
        {
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    SceneAsset targetScene = LaunchSetting.instance.lunchScene;
                    if (targetScene != null 
                        && !EditorSceneManager.GetActiveScene().name.Equals(targetScene.name))
                    {
                        originScenePath = EditorSceneManager.GetActiveScene().path;
                        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(targetScene));
                    }
                }
            }

            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode) return;
            if(!string.IsNullOrEmpty(originScenePath))
            {
                EditorSceneManager.OpenScene(originScenePath);
            }
        }
    }
}
