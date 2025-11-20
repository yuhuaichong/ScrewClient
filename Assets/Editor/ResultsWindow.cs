using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ResultsWindow : EditorWindow
{
    // 存储加载的文本或组件
    private List<string> loadedTexts;
    private List<Component> loadedComponents;

    // 滚动条位置
    private Vector2 scrollPosition;

    // 设置加载的文本
    public void SetLoadedTexts(List<string> texts)
    {
        loadedTexts = texts;
        loadedComponents = null; // 清空组件列表
    }

    // 设置加载的组件
    public void SetTextComponents(List<Component> components)
    {
        loadedComponents = components;
        loadedTexts = null; // 清空文本列表
    }

    // 清空所有结果
    public void ClearResults()
    {
        loadedTexts = null;
        loadedComponents = null;
    }

    // 窗口绘制
    private void OnGUI()
    {
        if ((loadedTexts == null || loadedTexts.Count == 0) &&
            (loadedComponents == null || loadedComponents.Count == 0))
        {
            GUILayout.Label("未加载任何内容。");
            return;
        }

        // 显示加载的内容数量
        if (loadedTexts != null)
        {
            GUILayout.Label($"已加载 {loadedTexts.Count} 条文本：");
        }
        else if (loadedComponents != null)
        {
            GUILayout.Label($"已找到 {loadedComponents.Count} 个文本组件：");
        }

        // 开始滚动视图
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true));

        // 显示文本内容
        if (loadedTexts != null)
        {
            foreach (var text in loadedTexts)
            {
                GUILayout.Label(text);
            }
        }
        // 显示组件内容
        else if (loadedComponents != null)
        {
            foreach (var component in loadedComponents)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(component.gameObject.name + " - " + component.GetType().Name, GUILayout.Width(300));
                if (GUILayout.Button("选择", GUILayout.Width(60)))
                {
                    // 选中对应的 GameObject
                    Selection.activeObject = component.gameObject;
                }
                GUILayout.EndHorizontal();
            }
        }

        // 结束滚动视图
        GUILayout.EndScrollView();
    }
}