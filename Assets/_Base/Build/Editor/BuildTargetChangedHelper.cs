using System;
using UnityEditor;
using UnityEditor.Build;

namespace BaseFramework.Build
{
    public class BuildTargetChangedHelper : IActiveBuildTargetChanged
    {
        public static Action<BuildTarget> changeCallback;

        public int callbackOrder => 0;

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            changeCallback?.Invoke(newTarget);
        }

        public static void Switch2BuildTarget(BuildTarget buildTarget)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, buildTarget);
        }
    }
}
