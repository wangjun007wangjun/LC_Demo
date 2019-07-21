#if UNITY_IOS && UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace BaseFramework.Build
{
    [InitializeOnLoad]
    public static class iOSPodUtil
    {
        private const string TAG = "iOSPodUtil";

        private static string POD_EXECUTABLE = "pod";

        private static string sourcePath = "/_Base/Build/iOS/Res/Podfile";

        static iOSPodUtil()
        {
            BuildUtil.onPostprocessBuild += ExecutePod;
        }

        private static void ExecutePod(BuildTarget buildTarget, string path)
        {
            if (CopyPodFile(path))
            {
                ExecutedPod(path);
            }
        }

        private static bool CopyPodFile(string path)
        {
            // check pod file
            string sourceFilePath = Application.dataPath + sourcePath;
            FileInfo fileInfo = new FileInfo(sourceFilePath);
            if (!fileInfo.Exists)
            {
                Log.E(TAG, "{0} not exists!", sourcePath);
                return false;
            }

            File.Copy(sourceFilePath, Path.Combine(path, "Podfile"));

            return true;
        }

        private static bool ExecutedPod(string projectPath)
        {
            string commandLine = $"cd {projectPath} && {POD_EXECUTABLE} repo update && {POD_EXECUTABLE} update && {POD_EXECUTABLE} install";
            Log.I(TAG, "start executed command\n{0}", commandLine);
            CommandUtil.CommandResult result = CommandUtil.ExecuteCommand(commandLine);

            if (result.resultCode == 0)
            {
                return true;
            }

            Log.E(TAG, "executed command error!\n{0}\n{1}", result.output, result.error);

            return false;
        }
    }
}
#endif
