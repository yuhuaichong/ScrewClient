using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LanguageLoaderTool : EditorWindow
{
    //// 数据文件夹路径
    //private string dataFolderPath;

    //// 存储加载的文本
    //private List<string> loadedTexts = new List<string>();

    //// 添加菜单项
    //[MenuItem("MyTools/本地化工具/加载 Language.txt")]
    //public static void ShowWindow()
    //{
    //    // 显示主窗口
    //    GetWindow<LanguageLoaderTool>("加载 Language.txt");
    //}

    //// 初始化路径
    //private void OnEnable()
    //{
    //    dataFolderPath = Path.Combine(Application.persistentDataPath, "Data");
    //}

    //// 主窗口绘制
    //private void OnGUI()
    //{
    //    // 添加加载按钮
    //    if (GUILayout.Button("加载 Language.txt"))
    //    {
    //        LoadLanguageFile();
    //        ShowResultsWindow();
    //    }
    //}

    //// 加载 Language.txt 文件
    //private void LoadLanguageFile()
    //{
    //    // 清空之前的记录
    //    loadedTexts.Clear();

    //    // 文件路径
    //    string filePath = Path.Combine(dataFolderPath, "Language.txt");

    //    // 检查文件是否存在
    //    if (!File.Exists(filePath))
    //    {
    //        Debug.LogError($"文件不存在：{filePath}");
    //        return;
    //    }

    //    // 读取文件内容
    //    string[] lines = File.ReadAllLines(filePath);

    //    // 解析每一行
    //    foreach (var line in lines)
    //    {
    //        if (!string.IsNullOrEmpty(line))
    //        {
    //            loadedTexts.Add(line);
    //        }
    //    }

    //    // 打印日志
    //    Debug.Log($"已加载 {loadedTexts.Count} 条文本。");
    //}

    //// 显示结果窗口
    //private void ShowResultsWindow()
    //{
    //    // 创建结果窗口
    //    ResultsWindow resultsWindow = GetWindow<ResultsWindow>("加载结果");
    //    resultsWindow.SetLoadedTexts(loadedTexts);
    //}
}