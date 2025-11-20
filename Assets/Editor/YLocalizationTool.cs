using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class YLocalizationTool : EditorWindow
{
    // 存储找到的挂载了 YLocalization 脚本的物体
    private List<GameObject> yLocalizationObjects = new List<GameObject>();

    // 添加菜单项
    [MenuItem("MyTools/本地化工具/查找 YLocalization 物体")]
    public static void ShowWindow()
    {
        // 显示窗口
        GetWindow<YLocalizationTool>("查找 YLocalization 物体");
    }

    // 窗口绘制
    private void OnGUI()
    {
        // 添加按钮
        if (GUILayout.Button("查找场景中的 YLocalization 物体"))
        {
            FindAllYLocalizationObjects();
            ShowResultsWindow();
        }
    }

    // 查找场景中所有挂载了 YLocalization 脚本的物体（包括未激活的）
    private void FindAllYLocalizationObjects()
    {
        // 清空之前的记录
        yLocalizationObjects.Clear();

        // 查找场景中所有的 GameObject（包括未激活的）
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // 只处理场景中的对象（排除预制件和资源）
            if (obj.scene.IsValid())
            {
                // 查找挂载了 YLocalization 脚本的物体
                YLocalization yLocalization = obj.GetComponent<YLocalization>();
                if (yLocalization != null)
                {
                    yLocalizationObjects.Add(obj);
                }
            }
        }

        // 打印日志
        Debug.Log($"找到 {yLocalizationObjects.Count} 个挂载了 YLocalization 脚本的物体");
    }

    // 显示结果窗口
    private void ShowResultsWindow()
    {
        // 创建结果窗口
        YLocalizationResultsWindow resultsWindow = GetWindow<YLocalizationResultsWindow>("YLocalization 查找结果");
        resultsWindow.SetYLocalizationObjects(yLocalizationObjects);
    }
}

// 结果窗口
public class YLocalizationResultsWindow : EditorWindow
{
    private List<GameObject> yLocalizationObjects;
    private Vector2 scrollPosition; // 滚动条位置

    // 设置 YLocalization 物体
    public void SetYLocalizationObjects(List<GameObject> objects)
    {
        yLocalizationObjects = objects;
    }

    // 窗口绘制
    private void OnGUI()
    {
        if (yLocalizationObjects == null || yLocalizationObjects.Count == 0)
        {
            GUILayout.Label("未找到任何挂载了 YLocalization 脚本的物体。");
            return;
        }

        // 显示找到的物体数量
        GUILayout.Label($"找到的 YLocalization 物体数量: {yLocalizationObjects.Count}");

        // 开始滚动视图
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true));

        // 遍历并显示找到的物体
        foreach (var obj in yLocalizationObjects)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(obj.name, GUILayout.Width(300));
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                // 选中对应的 GameObject
                Selection.activeObject = obj;
            }
            GUILayout.EndHorizontal();
        }

        // 结束滚动视图
        GUILayout.EndScrollView();
    }
}