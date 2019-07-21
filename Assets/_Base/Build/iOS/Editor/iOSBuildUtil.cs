using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode.Custom;
using UnityEngine;

namespace BaseFramework.Build
{
    public class iOSBuildUtil
    {
        #region Build

        [MenuItem("Base/Build/Export iOS XCode Project", false, 2)]
        public static void ExportProject()
        {
            BuildUtil.ExportTarget(BuildTarget.iOS);
        }

        #endregion

        #region Xcode project process

        private static readonly string UNITY_PROJECT_PATH = Path.GetDirectoryName(Application.dataPath);

        private static readonly string XCODE_ENTITLEMENTS_FILE_PATH =
            Path.Combine(PBXProject.GetUnityTargetName(), "project.entitlements");

        private static string _targetGuid = null;
        private static PBXProject _pbxProject = null;

        private static string _xcodeProjectExportPath = null;

        // eg: /Users/apple/exports/iOS/Unity-iPhone.xcodeproj/project.pbxproj
        private static string _pbxProjectPath = null;

        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            InitProjectInfo(path);

            // 设置自动签名
            SetCodeSign();
            // 开启系统Capabilities
            SetSystemCapabilities();
            // 添加Framework
            AddFramework();
            AddFrameworkSearchPath();
            // 添加tbd
            AddLibrary();
            AddLibrarySearchPath();
            // 
            AddHeaderSearchPath();
            // 修改属性
            FixProperty();
            // 修改plist.info
            FixPlist(path);
            // AddFile
            AddFile();

            File.WriteAllText(_pbxProjectPath, _pbxProject.WriteToString());
        }

        [PostProcessBuild(2)]
        public static void OnPostprocessBuild2(BuildTarget buildTarget, string path)
        {
            // open project file
            CommandUtil.ExecuteCommand("open " + path);
        }

        static void InitProjectInfo(string path)
        {
            // eg: /Users/apple/exports/iOS
            _xcodeProjectExportPath = path;

            // eg: /Users/apple/exports/iOS/Unity-iPhone.xcodeproj/project.pbxproj
            _pbxProjectPath = PBXProject.GetPBXProjectPath(path);

            _pbxProject = new PBXProject();
            // proj.ReadFromString(File.ReadAllText(projPath));
            _pbxProject.ReadFromFile(_pbxProjectPath);

            // PBXProject.GetUnityTargetName() = "Unity-iphone"
            _targetGuid = _pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
        }

        static void SetCodeSign()
        {
            _pbxProject.SetTargetAttributes("DevelopmentTeam", BuildProjectSetting.instance.signTeamID);
            _pbxProject.SetTargetAttributes("ProvisioningStyle", "Automatic");
        }

        static void AddFramework()
        {
            List<string> frameworks = BuildProjectSetting.instance.frameworks;
            if (frameworks == null) return;
            for (int i = 0; i < frameworks.Count; ++i)
            {
                _pbxProject.AddFrameworkToProject(_targetGuid, frameworks[i], false);
            }
        }

        static void AddFrameworkSearchPath()
        {
            // pbxProject.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "xxxx");
        }

        static void AddLibrary()
        {
            List<string> libraries = BuildProjectSetting.instance.libraries;
            if (libraries == null) return;
            for (int i = 0; i < libraries.Count; ++i)
            {
                // 参数1 path:相对PBXSourceTree.Sdk的路径
                // 参数2 projectPath:相对 Xcode导出工程的路径
                // PBXSourceTree.Sdk eg：
                //                   /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/
                string fileLibrary = _pbxProject.AddFile("/usr/lib/" + libraries[i],
                                                         "Frameworks/" + libraries[i], PBXSourceTree.Sdk);
                _pbxProject.AddFileToBuild(_targetGuid, fileLibrary);
            }
        }

        static void AddLibrarySearchPath()
        {
            // pbxProject.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", "xxxx");
        }

        static void AddHeaderSearchPath()
        {
            // pbxProject.AddBuildProperty(targetGuid, "HEADER_SEARCH_PATHS", "xxxx");
        }

        static void FixProperty()
        {
            List<KeyValueItem> properties = BuildProjectSetting.instance.properties;
            if (properties == null) return;
            for (int i = 0; i < properties.Count; ++i)
            {
                _pbxProject.AddBuildProperty(_targetGuid, properties[i].key, properties[i].value);
            }
        }

        static void FixPlist(string path)
        {
            List<KeyValueItem> plists = BuildProjectSetting.instance.plists;
            if (plists == null || plists.Count <= 0) return;
            var plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict rootDic = plist.root;

            for (int i = 0; i < plists.Count; ++i)
            {
                rootDic.SetString(plists[i].key, plists[i].value);
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        static void AddFile()
        {
            List<Object> files = BuildProjectSetting.instance.files;
            if (files == null) return;
            for (int i = 0; i < files.Count; ++i)
            {
                // eg:Assets/xxx/xxxx.xxx
                string fileRelativePath = AssetDatabase.GetAssetPath(files[i]);
                string fileName = Path.GetFileName(fileRelativePath);
                string sourceFilePath = Path.Combine(UNITY_PROJECT_PATH, fileRelativePath);
                string destFilePath = Path.Combine(_xcodeProjectExportPath, fileName);
                File.Copy(sourceFilePath, destFilePath);
                // 不调用AddFileToBuild IPA运行时会Crash
                _pbxProject.AddFileToBuild(_targetGuid, _pbxProject.AddFile(destFilePath, fileName));
            }
        }

        static void SetSystemCapabilities()
        {
            List<iOSCapabilityType> capabilities = BuildProjectSetting.instance.capabilities;
            if (capabilities == null) return;
            for (int i = 0; i < capabilities.Count; ++i)
            {
                PBXCapabilityType type = iOSCapabilityTypeHelper.GetPBXCapabilityType(capabilities[i]);
                if (type != null)
                {
                    _pbxProject.AddCapability(_targetGuid,
                                              iOSCapabilityTypeHelper.GetPBXCapabilityType(capabilities[i]),
                                              XCODE_ENTITLEMENTS_FILE_PATH);
                }
            }
        }

        #endregion
    }
}