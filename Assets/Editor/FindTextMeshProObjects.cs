using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

public class FindTextMeshProObjects : EditorWindow
{
    private List<GameObject> textMeshProObjects = new List<GameObject>();
    private Vector2 scrollPosition; // 用于滑动条的位置

    [MenuItem("Tools/Find TextMeshPro Objects")]
    public static void ShowWindow()
    {
        GetWindow<FindTextMeshProObjects>("Find TextMeshPro Objects");
    }

    private void OnGUI()
    {
        // 查找按钮
        if (GUILayout.Button("Find TextMeshPro Objects"))
        {
            FindObjects();
        }

        // 显示查找结果
        if (textMeshProObjects.Count > 0)
        {
            GUILayout.Label("Found TextMeshPro Objects:");

            // 计算剩余高度
            float remainingHeight = position.height - GUILayoutUtility.GetLastRect().height - 10; // 减去按钮和标签的高度，留出一些边距

            // 开始滚动视图（禁用水平滑动条）
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(remainingHeight));

            foreach (var obj in textMeshProObjects)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"{obj.name} - {GetHierarchyPath(obj)}");
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                EditorGUILayout.EndHorizontal();
            }

            // 结束滚动视图
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No TextMeshPro Objects found.");
        }
    }

    private void FindObjects()
    {
        textMeshProObjects.Clear();

        // 查找所有 GameObject（包括未激活的）
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var obj in allObjects)
        {
            // 检查是否包含 TextMeshProUGUI 或 TextMeshPro 组件
            if (obj.GetComponent<TextMeshProUGUI>() != null || obj.GetComponent<TextMeshPro>() != null)
            {
                textMeshProObjects.Add(obj);
            }
        }
    }

    private string GetHierarchyPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }
}