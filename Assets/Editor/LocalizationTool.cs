using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class LocalizationTool : EditorWindow
{
    // 存储找到的文本组件
    private List<Component> textComponents = new List<Component>();

    // 存储加载的文本
    private List<string> loadedTexts = new List<string>();

    // 数据文件夹路径
    private string dataFolderPath;

    // 添加菜单项
    [MenuItem("MyTools/本地化工具/本地化")]
    public static void ShowWindow()
    {
        Debug.Log("本地化工具窗口已打开"); // 调试日志

        // 获取或创建窗口
        var window = GetWindow<LocalizationTool>("本地化工具");

        // 设置窗口大小和位置
        window.minSize = new Vector2(400, 600); // 最小大小
        window.position = new Rect(100, 100, 400, 300); // 初始位置和大小

        // 显示窗口
        window.Show();
    }

    // 初始化路径
    private void OnEnable()
    {
        dataFolderPath = Path.Combine(Application.persistentDataPath, "Data");
    }

    // 窗口绘制
    private void OnGUI()
    {
        // 查找场景中的文本
        if (GUILayout.Button("查找场景中的文本"))
        {
            FindAllTextInScene();
            ShowResultsWindow();
        }

        // 加载 Language.txt 文件
        if (GUILayout.Button("加载 Language.txt"))
        {
            LoadLanguageFile();
            ShowResultsWindow();
        }

        // 导出文本到 Language.txt
        if (GUILayout.Button("导出文本到 Language.txt"))
        {
            ExportTextToTxt();
            ShowResultsWindow();
        }

        // 将加载的文本赋值给 YLocalization 组件
        if (GUILayout.Button("赋值给 YLocalization 组件"))
        {
            AssignTextToYLocalization();
        }
    }

    // 查找场景中所有的文本组件（包括未激活的）
    private void FindAllTextInScene()
    {
        // 清空之前的记录
        textComponents.Clear();

        // 查找场景中所有的 GameObject（包括未激活的）
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // 只处理场景中的对象（排除预制件和资源）
            if (obj.scene.IsValid())
            {
                // 查找 Text 组件
                Text text = obj.GetComponent<Text>();
                if (text != null)
                {
                    textComponents.Add(text);
                }

                // 查找 TextMeshPro 组件
                TextMeshPro tmp = obj.GetComponent<TextMeshPro>();
                if (tmp != null)
                {
                    textComponents.Add(tmp);
                }

                // 查找 TextMeshProUGUI 组件
                TextMeshProUGUI tmpUI = obj.GetComponent<TextMeshProUGUI>();
                if (tmpUI != null)
                {
                    textComponents.Add(tmpUI);
                }
            }
        }

        // 打印日志
        Debug.Log($"找到 {textComponents.Count} 个文本组件");
    }

    // 加载 Language.txt 文件
    private void LoadLanguageFile()
    {
        // 清空之前的记录
        loadedTexts.Clear();

        // 文件路径
        string filePath = Path.Combine(dataFolderPath, "Language.txt");

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            Debug.LogError($"文件不存在：{filePath}");
            return;
        }

        // 读取文件内容
        string[] lines = File.ReadAllLines(filePath);

        // 解析每一行
        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                loadedTexts.Add(line);
            }
        }

        // 打印日志
        Debug.Log($"已加载 {loadedTexts.Count} 条文本。");
    }

    // 导出文本到 Language.txt
    private void ExportTextToTxt()
    {
        if (textComponents == null || textComponents.Count == 0)
        {
            Debug.LogWarning("没有找到任何文本组件，请先点击查找按钮。");
            return;
        }

        // 创建数据文件夹（如果不存在）
        if (!Directory.Exists(dataFolderPath))
        {
            Directory.CreateDirectory(dataFolderPath);
        }

        // 构建文本内容
        StringBuilder txtContent = new StringBuilder();
        for (int i = 0; i < textComponents.Count; i++)
        {
            string textContent = GetTextContent(textComponents[i]);
            txtContent.AppendLine($"ID_{i + 1}: {textContent}#{textContent}");
        }

        // 写入文件
        string filePath = Path.Combine(dataFolderPath, "Language.txt");
        File.WriteAllText(filePath, txtContent.ToString());

        // 打印日志
        Debug.Log($"文本已导出到: {filePath}");

        // 将导出结果加载到 ResultsWindow
        loadedTexts.Clear();
        loadedTexts.AddRange(File.ReadAllLines(filePath));
    }

    // 显示结果窗口
    private void ShowResultsWindow()
    {
        // 创建结果窗口
        ResultsWindow resultsWindow = GetWindow<ResultsWindow>("查找结果");

        // 清空之前的结果
        resultsWindow.ClearResults();

        // 设置新的结果
        if (loadedTexts.Count > 0)
        {
            resultsWindow.SetLoadedTexts(loadedTexts);
        }
        else if (textComponents.Count > 0)
        {
            resultsWindow.SetTextComponents(textComponents);
        }
    }

    // 将加载的文本赋值给 YLocalization 组件
    private void AssignTextToYLocalization()
    {
        if (loadedTexts == null || loadedTexts.Count == 0)
        {
            Debug.LogWarning("没有加载任何文本，请先点击加载按钮。");
            return;
        }

        // 遍历加载的文本
        for (int i = 0; i < loadedTexts.Count; i++)
        {
            // 解析文本内容
            string[] parts = loadedTexts[i].Split(new[] { ':', '#' }, 3);
            if (parts.Length == 3)
            {
                string id = parts[0].Trim(); // ID
                string englishStr = parts[1].Trim(); // 英文字符
                string chineseStr = parts[2].Trim(); // 中文字符

                // 获取对应的文本组件
                if (i < textComponents.Count)
                {
                    var component = textComponents[i];

                    // 获取 YLocalization 组件
                    YLocalization yLocalization = component.GetComponent<YLocalization>();
                    if (yLocalization != null)
                    {
                        // 赋值给 YLocalization 组件
                        yLocalization.englishStr = englishStr;
                        yLocalization.chineseStr = chineseStr;
                        Debug.Log($"已赋值给 {id}: {englishStr} -> {chineseStr}");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到 {id} 对应的 YLocalization 组件。");
                    }
                }
            }
        }

        // 打印日志
        Debug.Log("文本已赋值给所有 YLocalization 组件。");
    }

    // 获取文本内容
    private string GetTextContent(Component component)
    {
        if (component is Text text)
        {
            return text.text;
        }
        else if (component is TextMeshPro tmp)
        {
            return tmp.text;
        }
        else if (component is TextMeshProUGUI tmpUI)
        {
            return tmpUI.text;
        }
        return string.Empty;
    }
}