//
//  Copyright (c) 2022 Tenjin. All rights reserved.
//

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

public class BuildPostProcessor : MonoBehaviour
{

    [MenuItem("Assets/Tenjin/Export Unity Package")]
    public static void ExportTenjinUnityPackage()
    {
        string exportedFileName = "TenjinUnityPackage.unitypackage";
        string assetsPath = "Assets";
        List<string> tenjinAssets = new List<string>();

        // Editor files
        tenjinAssets.Add(assetsPath + "/Editor/BuildPostProcessor.cs");
        tenjinAssets.Add(assetsPath + "/Editor/Dependencies.xml");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/Editor/TenjinAssetSelector.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/Editor/TenjinEditorPrefs.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/Editor/TenjinPackager.cs");

        // Gradle Templates
        tenjinAssets.Add(assetsPath + "/Plugins/Android/GradleTemplates/m2repository.gradle");

        // iOS dependencies
        tenjinAssets.Add(assetsPath + "/Plugins/iOS/GADUAdNetworkExtras.h");
        tenjinAssets.Add(assetsPath + "/Plugins/iOS/TenjinSDK.h");
        tenjinAssets.Add(assetsPath + "/Plugins/iOS/TenjinSDK.xcframework.zip");
        tenjinAssets.Add(assetsPath + "/Plugins/iOS/TenjinUnityInterface.h");
        tenjinAssets.Add(assetsPath + "/Plugins/iOS/TenjinUnityInterface.mm");

        // Tenjin files
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/AndroidTenjin.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/AppStoreType.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/BaseTenjin.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/CspManager.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/DebugTenjin.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/iOSTenjin.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/Tenjin.cs");

        // Integration scripts
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/TenjinAdMobIntegration.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/TenjinAppLovinIntegration.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/TenjinCASIntegration.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/TenjinHyperBidIntegration.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/TenjinIronSourceIntegration.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/TenjinTopOnIntegration.cs");
        tenjinAssets.Add(assetsPath + "/Tenjin/Scripts/TenjinTradPlusIntegration.cs");

        // Tenjin prefab
        tenjinAssets.Add(assetsPath + "/Tenjin/tenjin.unitypackage.manifest");

        // Export package
        AssetDatabase.ExportPackage(
            tenjinAssets.ToArray(),
            exportedFileName,
            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Interactive);
    }
        
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            BuildiOS(path);
        }
        else if (buildTarget == BuildTarget.Android)
        {
            BuildAndroid(path);
        }
    }

    private static void BuildAndroid(string path = "")
    {
        Debug.Log("TenjinSDK: Starting Android Build");
    }

    private static void BuildiOS(string path = "")
    {
#if UNITY_IOS
        Debug.Log("TenjinSDK: Starting iOS Build");

        string projectPath = Path.Combine(path, "Unity-iPhone.xcodeproj/project.pbxproj");
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
        string buildTarget = project.GetUnityFrameworkTargetGuid();
#else
        string buildTarget = project.TargetGuidByName("Unity-iPhone");
#endif

        AddFrameworksToProject(project, buildTarget);
        AddLinkerFlags(project, buildTarget);
        UpdatePlist(path);
        
        // 先尝试直接修复（如果 Pods 已经存在）
        FixInMobiPrivacyManifestDirect(path);
        
        // 添加自动修复脚本到 Build Phase（用于 pod install 后的构建）
        // 注意：必须添加到主 Target（Unity-iPhone），而不是 UnityFramework Target
#if UNITY_2019_3_OR_NEWER
        string mainTargetGuid = project.GetUnityMainTargetGuid();
#else
        string mainTargetGuid = project.TargetGuidByName("Unity-iPhone");
#endif
        AddInMobiPrivacyManifestFixScript(project, mainTargetGuid, path);

        File.WriteAllText(projectPath, project.WriteToString());
#endif  
    }

    private static void FixInMobiPrivacyManifestDirect(string buildPath)
    {
        // 直接修复 InMobi SDK 的隐私清单文件（如果 Pods 目录已存在）
        string[] inmobiPrivacyPaths = new string[]
        {
            Path.Combine(buildPath, "Pods/InMobiSDK/InMobiSDK.xcframework/ios-arm64/InMobiSDK.framework/PrivacyInfo.xcprivacy"),
            Path.Combine(buildPath, "Pods/InMobiSDK/InMobiSDK.xcframework/ios-arm64_x86_64-simulator/InMobiSDK.framework/PrivacyInfo.xcprivacy")
        };

        string fixedPrivacyManifest = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
	<key>NSPrivacyCollectedDataTypes</key>
	<array/>
	<key>NSPrivacyAccessedAPITypes</key>
	<array>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategorySystemBootTime</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>35F9.1</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategoryDiskSpace</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>E174.1</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategoryUserDefaults</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>CA92.1</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategoryFileTimestamp</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>C617.1</string>
			</array>
		</dict>
	</array>
</dict>
</plist>";

        foreach (string privacyPath in inmobiPrivacyPaths)
        {
            if (File.Exists(privacyPath))
            {
                try
                {
                    File.WriteAllText(privacyPath, fixedPrivacyManifest);
                    Debug.Log($"✅ 已直接修复 InMobi SDK 隐私清单: {privacyPath}");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"⚠️ 无法修复 InMobi SDK 隐私清单 {privacyPath}: {e.Message}");
                }
            }
        }
    }

    private static void AddInMobiPrivacyManifestFixScript(PBXProject project, string targetGuid, string buildPath)
    {
        // 创建修复脚本文件
        string scriptPath = Path.Combine(buildPath, "fix_inmobi_privacy.sh");
        string scriptContent = @"#!/bin/bash
# 自动修复 InMobi SDK 隐私清单文件
# 此脚本会在每次 Xcode 构建时运行

FIXED_PRIVACY_MANIFEST='<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
	<key>NSPrivacyCollectedDataTypes</key>
	<array/>
	<key>NSPrivacyAccessedAPITypes</key>
	<array>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategorySystemBootTime</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>35F9.1</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategoryDiskSpace</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>E174.1</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategoryUserDefaults</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>CA92.1</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategoryFileTimestamp</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>C617.1</string>
			</array>
		</dict>
	</array>
</dict>
</plist>'

# InMobi SDK 隐私清单文件路径
INMOBI_PATHS=(
    ""${PROJECT_DIR}/Pods/InMobiSDK/InMobiSDK.xcframework/ios-arm64/InMobiSDK.framework/PrivacyInfo.xcprivacy""
    ""${PROJECT_DIR}/Pods/InMobiSDK/InMobiSDK.xcframework/ios-arm64_x86_64-simulator/InMobiSDK.framework/PrivacyInfo.xcprivacy""
)

# 修复每个隐私清单文件
for PRIVACY_PATH in ""${INMOBI_PATHS[@]}""
do
    if [ -f ""$PRIVACY_PATH"" ]; then
        echo ""[Fix InMobi] Fixing Privacy Manifest: $PRIVACY_PATH""
        echo ""$FIXED_PRIVACY_MANIFEST"" > ""$PRIVACY_PATH""
    fi
done

echo ""[Fix InMobi] Privacy Manifest fix completed""
";

        File.WriteAllText(scriptPath, scriptContent);
        
        // 设置脚本可执行权限（macOS）
        try
        {
            var chmodProcess = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/bin/chmod",
                Arguments = $"+x \"{scriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });
            chmodProcess?.WaitForExit();
            Debug.Log($"✅ 已创建 InMobi 隐私清单修复脚本: {scriptPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ 无法设置脚本执行权限，请手动运行: chmod +x {scriptPath}. 错误: {e.Message}");
        }

        // 直接修改 project.pbxproj 文件来添加 Build Phase
        try
        {
            AddShellScriptBuildPhaseToProject(project, targetGuid, scriptPath, buildPath);
            Debug.Log("✅ 已添加 InMobi 隐私清单修复脚本到 Xcode Build Phase");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ 无法自动添加 Build Phase，请手动在 Xcode 中添加: {e.Message}");
            Debug.LogWarning($"脚本位置: {scriptPath}");
            Debug.LogWarning("请在 Xcode 中: Target > Build Phases > + > New Run Script Phase");
            Debug.LogWarning($"添加脚本: bash \"{Path.GetFileName(scriptPath)}\"");
        }
    }

    private static void AddShellScriptBuildPhaseToProject(PBXProject project, string targetGuid, string scriptPath, string buildPath)
    {
        // 直接修改 project.pbxproj 文件来添加 Build Phase
        string projectPbxPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
        
        if (!File.Exists(projectPbxPath))
        {
            Debug.LogWarning($"⚠️ Xcode 项目文件不存在: {projectPbxPath}");
            return;
        }
        
        string projectContent = File.ReadAllText(projectPbxPath);
        
        // 检查是否已经添加过（通过名称检查）
        if (projectContent.Contains("Fix InMobi Privacy Manifest"))
        {
            Debug.Log("✅ InMobi 隐私清单修复脚本已存在，跳过添加");
            return;
        }
        
        // 生成唯一的 GUID（Xcode 使用的格式：24 位十六进制）
        System.Random random = new System.Random();
        string buildPhaseGuid = "";
        for (int i = 0; i < 24; i++)
        {
            buildPhaseGuid += random.Next(0, 16).ToString("X");
        }
        
        string scriptRelativePath = Path.GetFileName(scriptPath);
        
        // 构建 Shell Script Build Phase（调用脚本文件）
        // 使用绝对路径，确保在任何情况下都能找到脚本
        string shellScriptCall = $"bash \\\"${{PROJECT_DIR}}/{scriptRelativePath}\\\"";
        
        string shellScriptBuildPhase = $"\n\t\t{buildPhaseGuid} /* Fix InMobi Privacy Manifest */ = {{\n\t\t\tisa = PBXShellScriptBuildPhase;\n\t\t\tbuildActionMask = 2147483647;\n\t\t\tfiles = (\n\t\t\t);\n\t\t\tinputPaths = (\n\t\t\t);\n\t\t\tname = \"Fix InMobi Privacy Manifest\";\n\t\t\toutputPaths = (\n\t\t\t);\n\t\t\trunOnlyForDeploymentPostprocessing = 0;\n\t\t\tshellPath = /bin/sh;\n\t\t\tshellScript = \"{shellScriptCall}\";\n\t\t}};";

        // 1. 添加 Build Phase 定义到 PBXShellScriptBuildPhase section
        string shellScriptSectionPattern = @"(/\* Begin PBXShellScriptBuildPhase section \*/)(.*?)(/\* End PBXShellScriptBuildPhase section \*/)";
        var shellScriptRegex = new System.Text.RegularExpressions.Regex(shellScriptSectionPattern, System.Text.RegularExpressions.RegexOptions.Singleline);
        
        if (shellScriptRegex.IsMatch(projectContent))
        {
            projectContent = shellScriptRegex.Replace(projectContent,
                match => match.Groups[1].Value + match.Groups[2].Value + shellScriptBuildPhase + "\n\t\t" + match.Groups[3].Value
            );
        }
        else
        {
            // 如果不存在 Shell Script Build Phase 部分，添加它
            string insertPoint = "/* End PBXResourcesBuildPhase section */";
            if (projectContent.Contains(insertPoint))
            {
                projectContent = projectContent.Replace(insertPoint, 
                    insertPoint + "\n\n/* Begin PBXShellScriptBuildPhase section */" + shellScriptBuildPhase + "\n/* End PBXShellScriptBuildPhase section */"
                );
            }
            else
            {
                Debug.LogWarning("⚠️ 无法找到插入点来添加 PBXShellScriptBuildPhase section");
            }
        }
        
        // 2. 添加到 target 的 buildPhases 列表
        // 使用更宽松的正则表达式来匹配 Target 定义
        // Target GUID 格式通常是 24 位十六进制，后面可能有注释
        string escapedTargetGuid = System.Text.RegularExpressions.Regex.Escape(targetGuid);
        string targetPattern = $@"({escapedTargetGuid}\s+\/\*[^*]*\*\/\s*=\s+\{{[^}}]*?buildPhases\s*=\s*\()([^)]*)(\);)";
        var targetRegex = new System.Text.RegularExpressions.Regex(targetPattern, System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        bool addedToTarget = false;
        if (targetRegex.IsMatch(projectContent))
        {
            projectContent = targetRegex.Replace(projectContent, 
                match => {
                    string buildPhasesList = match.Groups[2].Value.TrimEnd();
                    if (!buildPhasesList.Contains(buildPhaseGuid))
                    {
                        // 找到 [CP] 相关的脚本之后插入（确保在 pod install 之后运行）
                        // 如果没有找到，则添加到列表末尾
                        string insertionPoint = "\t\t\t\t" + buildPhaseGuid + " /* Fix InMobi Privacy Manifest */,";
                        
                        if (buildPhasesList.Contains("[CP]"))
                        {
                            // 在最后一个 [CP] 脚本之后插入
                            var lines = buildPhasesList.Split('\n');
                            int lastCpIndex = -1;
                            for (int i = lines.Length - 1; i >= 0; i--)
                            {
                                if (lines[i].Contains("[CP]"))
                                {
                                    lastCpIndex = i;
                                    break;
                                }
                            }
                            
                            if (lastCpIndex >= 0)
                            {
                                // 在 [CP] 脚本之后插入
                                string newBuildPhases = "";
                                for (int i = 0; i <= lastCpIndex; i++)
                                {
                                    newBuildPhases += lines[i] + "\n";
                                }
                                newBuildPhases += insertionPoint + "\n";
                                for (int i = lastCpIndex + 1; i < lines.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(lines[i].Trim()))
                                    {
                                        newBuildPhases += lines[i] + "\n";
                                    }
                                }
                                buildPhasesList = newBuildPhases.TrimEnd();
                            }
                            else
                            {
                                // 备用：添加到末尾
                                if (buildPhasesList.Length > 0 && !buildPhasesList.EndsWith("\n"))
                                {
                                    buildPhasesList += "\n";
                                }
                                buildPhasesList += insertionPoint;
                            }
                        }
                        else
                        {
                            // 没有 [CP] 脚本，添加到 Resources 之后或列表开头
                            if (buildPhasesList.Contains("Resources"))
                            {
                                buildPhasesList = buildPhasesList.Replace("Resources */,", "Resources */,\n" + insertionPoint);
                            }
                            else
                            {
                                // 添加到列表末尾
                                if (buildPhasesList.Length > 0 && !buildPhasesList.EndsWith("\n"))
                                {
                                    buildPhasesList += "\n";
                                }
                                buildPhasesList += insertionPoint;
                            }
                        }
                        addedToTarget = true;
                    }
                    return match.Groups[1].Value + buildPhasesList + "\n\t\t\t" + match.Groups[3].Value;
                }
            );
        }
        
        if (!addedToTarget)
        {
            Debug.LogWarning($"⚠️ 无法找到 Target (GUID: {targetGuid}) 的 buildPhases 列表，尝试备用方法...");
            
            // 备用方法：直接查找 Target GUID 并添加
            string targetGuidPattern = $@"({escapedTargetGuid}[^}}]*buildPhases\s*=\s*\()([^)]*)(\);)";
            var targetGuidRegex = new System.Text.RegularExpressions.Regex(targetGuidPattern, System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            if (targetGuidRegex.IsMatch(projectContent))
            {
                projectContent = targetGuidRegex.Replace(projectContent,
                    match => {
                        string buildPhasesList = match.Groups[2].Value.TrimEnd();
                        if (!buildPhasesList.Contains(buildPhaseGuid))
                        {
                            if (buildPhasesList.Length > 0 && !buildPhasesList.EndsWith("\n"))
                            {
                                buildPhasesList += "\n";
                            }
                            buildPhasesList += $"\t\t\t\t{buildPhaseGuid} /* Fix InMobi Privacy Manifest */,";
                            addedToTarget = true;
                        }
                        return match.Groups[1].Value + buildPhasesList + "\n\t\t\t" + match.Groups[3].Value;
                    }
                );
            }
        }
        
        if (addedToTarget)
        {
            File.WriteAllText(projectPbxPath, projectContent);
            Debug.Log($"✅ 成功添加 InMobi 隐私清单修复脚本到 Build Phase (GUID: {buildPhaseGuid})");
        }
        else
        {
            Debug.LogWarning($"⚠️ 无法将 Build Phase 添加到 Target。请手动在 Xcode 中添加：");
            Debug.LogWarning($"   1. 打开 Xcode 项目");
            Debug.LogWarning($"   2. 选择 Target: Unity-iPhone");
            Debug.LogWarning($"   3. Build Phases > + > New Run Script Phase");
            Debug.LogWarning($"   4. 脚本内容: bash \"${{PROJECT_DIR}}/{scriptRelativePath}\"");
            Debug.LogWarning($"   5. 将新 Phase 拖到最上方（在 Compile Sources 之前）");
        }
    }

#if UNITY_IOS
    [PostProcessBuild(50)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            EmbedSignFramework(path);
        }
    }

    public static void EmbedSignFramework(string path)
    {
        string projPath = PBXProject.GetPBXProjectPath(path);
        if (!File.Exists(projPath))
        {
            Debug.LogError("Project file does not exist: " + projPath);
            return;
        }
        
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));

        // Get the target GUID
        string unityFrameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();
        string targetGuid = proj.GetUnityMainTargetGuid();

        string zipPathInUnity = "Assets/Plugins/iOS/TenjinSDK.xcframework.zip";
        string extractionPath = Path.Combine(path, "Frameworks");
        string frameworkPath = Path.Combine(extractionPath, "TenjinSDK.xcframework");

        if (Directory.Exists(frameworkPath))
        {
            Directory.Delete(frameworkPath, true);
        }

        try
        {
            ZipFile.ExtractToDirectory(zipPathInUnity, extractionPath);

            // Delete --MACOSX metadata folder
            string macosxMetaFolder = Path.Combine(extractionPath, "__MACOSX");
            if (Directory.Exists(macosxMetaFolder))
            {
                Directory.Delete(macosxMetaFolder, true);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to extract zip file: " + e.Message);
            return;
        }

        // Add the .xcframework to the Xcode project and embed it in the main target
        AddFrameworkToXcodeProject(proj, targetGuid, unityFrameworkTargetGuid, frameworkPath);
        
        File.WriteAllText(projPath, proj.WriteToString());
    }

    private static void AddFrameworkToXcodeProject(PBXProject proj, string targetGuid, string unityFrameworkTargetGuid, string frameworkPath)
    {
        string fileGuid = proj.AddFile(frameworkPath, "Frameworks/TenjinSDK.xcframework");
        proj.AddFileToEmbedFrameworks(targetGuid, fileGuid);

        string fileGuidForUnityFramework = proj.AddFile(frameworkPath, "Frameworks/TenjinSDK.xcframework");
        proj.AddFileToBuildSection(targetGuid, proj.GetFrameworksBuildPhaseByTarget(targetGuid), fileGuid);
        proj.AddFileToBuildSection(unityFrameworkTargetGuid, proj.GetFrameworksBuildPhaseByTarget(unityFrameworkTargetGuid), fileGuidForUnityFramework);
    }

    private static void AddFrameworksToProject(PBXProject project, string buildTarget)
    {
        List<string> frameworks = new List<string>
        {
            "AdServices.framework",
            "AdSupport.framework",
            "AppTrackingTransparency.framework",
            "StoreKit.framework"
        };

        foreach (string framework in frameworks)
        {
            Debug.Log("TenjinSDK: Adding framework: " + framework);
            project.AddFrameworkToProject(buildTarget, framework, true);
        }
    }

    private static void AddLinkerFlags(PBXProject project, string buildTarget)
    {
        Debug.Log("TenjinSDK: Adding -ObjC flag to other linker flags (OTHER_LDFLAGS)");
        project.AddBuildProperty(buildTarget, "OTHER_LDFLAGS", "-ObjC");
    }

    private static void UpdatePlist(string path)
    {
        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();
            
        plist.ReadFromFile(plistPath);

        plist.root.SetString("NSUserTrackingUsageDescription", 
                "We request to track data to enhance ad performance and user experience. Your privacy is respected.");

        File.WriteAllText(plistPath, plist.WriteToString());
    }

#endif
}
