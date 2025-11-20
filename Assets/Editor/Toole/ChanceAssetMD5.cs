using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;

/// <summary>
/// 改变资源MD5值的编辑器工具
/// 通过在文件末尾添加随机注释来改变MD5值，但不影响资源使用
/// </summary>
public class ChangeAssetMD5 : EditorWindow
{
    private string selectedFolderPath = "Assets/";
    private Vector2 scrollPosition;
    private List<string> logMessages = new List<string>();
    private bool isProcessing = false;
    private int processedCount = 0;
    private int totalCount = 0;
    
    // 支持修改的文件类型
    private bool modifyPrefabs = true;
    private bool modifyScripts = true;
    private bool modifyScenes = true;
    private bool modifyMaterials = true;
    private bool modifyAnimations = true;
    private bool modifyImages = true;
    private bool modifyOthers = true;

    [MenuItem("工具/改变资源MD5值")]
    public static void ShowWindow()
    {
        ChangeAssetMD5 window = GetWindow<ChangeAssetMD5>("改变资源MD5值");
        window.minSize = new Vector2(500, 400);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("资源MD5修改工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 文件夹选择
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("选择文件夹:", GUILayout.Width(80));
        selectedFolderPath = EditorGUILayout.TextField(selectedFolderPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择要修改的文件夹", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                // 转换为相对路径
                if (path.StartsWith(Application.dataPath))
                {
                    selectedFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "请选择项目内的文件夹！", "确定");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 文件类型选择
        GUILayout.Label("选择要修改的文件类型:", EditorStyles.boldLabel);
        modifyPrefabs = EditorGUILayout.Toggle("Prefabs (.prefab)", modifyPrefabs);
        modifyScripts = EditorGUILayout.Toggle("Scripts (.cs)", modifyScripts);
        modifyScenes = EditorGUILayout.Toggle("Scenes (.unity)", modifyScenes);
        modifyMaterials = EditorGUILayout.Toggle("Materials (.mat)", modifyMaterials);
        modifyAnimations = EditorGUILayout.Toggle("Animations (.anim, .controller)", modifyAnimations);
        modifyImages = EditorGUILayout.Toggle("Images (.png, .jpg, .jpeg)", modifyImages);
        modifyOthers = EditorGUILayout.Toggle("其他文本资源 (.asset, .json, etc)", modifyOthers);

        EditorGUILayout.Space();

        // 进度显示
        if (isProcessing)
        {
            EditorGUILayout.HelpBox($"处理中... {processedCount}/{totalCount}", MessageType.Info);
            EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18), (float)processedCount / totalCount, $"{processedCount}/{totalCount}");
        }

        EditorGUILayout.Space();

        // 执行按钮
        GUI.enabled = !isProcessing;
        if (GUILayout.Button("开始修改MD5", GUILayout.Height(30)))
        {
            ProcessFolder();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        // 日志显示
        GUILayout.Label("处理日志:", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        foreach (string log in logMessages)
        {
            EditorGUILayout.LabelField(log, EditorStyles.wordWrappedLabel);
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("清空日志"))
        {
            logMessages.Clear();
        }
    }

    private void ProcessFolder()
    {
        if (!Directory.Exists(selectedFolderPath))
        {
            EditorUtility.DisplayDialog("错误", "选择的文件夹不存在！", "确定");
            return;
        }

        logMessages.Clear();
        isProcessing = true;
        processedCount = 0;
        totalCount = 0;

        try
        {
            // 获取所有文件
            List<string> filesToProcess = GetFilesToProcess(selectedFolderPath);
            totalCount = filesToProcess.Count;

            AddLog($"找到 {totalCount} 个文件待处理");

            if (totalCount == 0)
            {
                AddLog("没有找到符合条件的文件");
                isProcessing = false;
                return;
            }

            // 处理每个文件
            foreach (string filePath in filesToProcess)
            {
                ProcessFile(filePath);
                processedCount++;
                
                // 更新进度
                if (processedCount % 10 == 0)
                {
                    EditorUtility.DisplayProgressBar("处理中", $"正在处理 {processedCount}/{totalCount}", (float)processedCount / totalCount);
                }
            }

            AddLog($"完成！共处理 {processedCount} 个文件");
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("完成", $"成功修改了 {processedCount} 个文件的MD5值！", "确定");
        }
        catch (Exception e)
        {
            AddLog($"错误: {e.Message}");
            EditorUtility.DisplayDialog("错误", e.Message, "确定");
        }
        finally
        {
            isProcessing = false;
            EditorUtility.ClearProgressBar();
        }
    }

    private List<string> GetFilesToProcess(string folderPath)
    {
        List<string> files = new List<string>();
        string[] allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

        foreach (string file in allFiles)
        {
            string extension = Path.GetExtension(file).ToLower();
            
            // 排除.meta文件
            if (extension == ".meta")
                continue;

            bool shouldProcess = false;

            if (modifyPrefabs && extension == ".prefab")
                shouldProcess = true;
            else if (modifyScripts && extension == ".cs")
                shouldProcess = true;
            else if (modifyScenes && extension == ".unity")
                shouldProcess = true;
            else if (modifyMaterials && extension == ".mat")
                shouldProcess = true;
            else if (modifyAnimations && (extension == ".anim" || extension == ".controller"))
                shouldProcess = true;
            else if (modifyImages && (extension == ".png" || extension == ".jpg" || extension == ".jpeg"))
                shouldProcess = true;
            else if (modifyOthers && (extension == ".asset" || extension == ".json" || extension == ".txt" || extension == ".xml"))
                shouldProcess = true;

            if (shouldProcess)
            {
                files.Add(file);
            }
        }

        return files;
    }

    private void ProcessFile(string filePath)
    {
        try
        {
            // 获取原始MD5
            string originalMD5 = CalculateMD5(filePath);

            string extension = Path.GetExtension(filePath).ToLower();
            
            // 判断是文本文件还是二进制文件
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                // 处理图片等二进制文件
                ProcessBinaryFile(filePath, originalMD5);
            }
            else
            {
                // 处理文本文件
                ProcessTextFile(filePath, originalMD5);
            }
        }
        catch (Exception e)
        {
            AddLog($"✗ 处理失败 {Path.GetFileName(filePath)}: {e.Message}");
        }
    }

    private void ProcessTextFile(string filePath, string originalMD5)
    {
        // 读取文件内容
        string content = File.ReadAllText(filePath, Encoding.UTF8);
        
        // 生成随机注释（对于不同类型的文件使用不同的注释格式）
        string extension = Path.GetExtension(filePath).ToLower();
        string randomComment = GenerateComment(extension);

        // 在文件末尾添加注释
        content += randomComment;

        // 写回文件
        File.WriteAllText(filePath, content, Encoding.UTF8);

        // 计算新的MD5
        string newMD5 = CalculateMD5(filePath);

        AddLog($"✓ {Path.GetFileName(filePath)} - MD5已改变");
        AddLog($"  原MD5: {originalMD5}");
        AddLog($"  新MD5: {newMD5}");
    }

    private void ProcessBinaryFile(string filePath, string originalMD5)
    {
        // 读取二进制文件
        byte[] fileBytes = File.ReadAllBytes(filePath);

        // 生成随机数据（在文件末尾添加一些不影响显示的数据）
        string randomString = Guid.NewGuid().ToString();
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string metadata = $"MD5_CHANGE_{randomString}_{timestamp}";
        byte[] metadataBytes = Encoding.UTF8.GetBytes(metadata);

        // 创建新的字节数组
        byte[] newFileBytes = new byte[fileBytes.Length + metadataBytes.Length];
        Array.Copy(fileBytes, 0, newFileBytes, 0, fileBytes.Length);
        Array.Copy(metadataBytes, 0, newFileBytes, fileBytes.Length, metadataBytes.Length);

        // 写回文件
        File.WriteAllBytes(filePath, newFileBytes);

        // 计算新的MD5
        string newMD5 = CalculateMD5(filePath);

        AddLog($"✓ {Path.GetFileName(filePath)} - MD5已改变 (图片)");
        AddLog($"  原MD5: {originalMD5}");
        AddLog($"  新MD5: {newMD5}");
    }

    private string GenerateComment(string extension)
    {
        // 生成随机字符串
        string randomString = Guid.NewGuid().ToString();
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        switch (extension)
        {
            case ".cs":
                return $"\n// MD5_CHANGE_{randomString}_{timestamp}";
            
            case ".prefab":
            case ".unity":
            case ".mat":
            case ".anim":
            case ".controller":
            case ".asset":
                // Unity的YAML格式，添加注释
                return $"\n# MD5_CHANGE_{randomString}_{timestamp}";
            
            case ".json":
            case ".txt":
            case ".xml":
                // 这些文件在末尾添加空白注释
                return $"\n<!-- MD5_CHANGE_{randomString}_{timestamp} -->";
            
            default:
                return $"\n# MD5_CHANGE_{randomString}_{timestamp}";
        }
    }

    private string CalculateMD5(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    private void AddLog(string message)
    {
        logMessages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        Repaint();
    }
}
