using UnityEditor;
using UnityEngine;
using System;
using DafultScript;
public class PrefabTools : EditorWindow
{
    private string folderPath = "";
    private string nameFilter = "";

    [MenuItem("MyTools/Clean Missing Scripts")]
    public static void ShowMissingScriptsCleaner()
    {
        GetWindow<PrefabTools>("Clean Missing Scripts").SwitchToCleaner();
    }

    [MenuItem("MyTools/Add Scripts to Prefabs")]
    public static void ShowScriptAdder()
    {
        GetWindow<PrefabTools>("Add Scripts to Prefabs").SwitchToAdder();
    }

    [MenuItem("MyTools/Remove Specific Components")]
    public static void ShowComponentRemover()
    {
        GetWindow<PrefabTools>("Remove Components").SwitchToRemover();
    }

    private enum ToolMode { Cleaner, Adder, Remover }
    private ToolMode currentMode = ToolMode.Cleaner;

    private void SwitchToCleaner() => currentMode = ToolMode.Cleaner;
    private void SwitchToAdder() => currentMode = ToolMode.Adder;
    private void SwitchToRemover() => currentMode = ToolMode.Remover;

    private void OnGUI()
    {
        GUILayout.Label(currentMode == ToolMode.Cleaner ? "清除丢失脚本" :
                        currentMode == ToolMode.Adder ? "为预制体添加脚本" :
                        "删除特定组件", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("文件夹路径:", GUILayout.Width(70));
        folderPath = GUILayout.TextField(folderPath, GUILayout.Width(300));
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                folderPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (currentMode == ToolMode.Adder)
        {
            GUILayout.Space(10);
            GUILayout.Label("预制体名称过滤:", GUILayout.Width(120));
            nameFilter = GUILayout.TextField(nameFilter, GUILayout.Width(300));
        }

        GUILayout.Space(10);

        if (GUILayout.Button(currentMode == ToolMode.Cleaner ? "开始清理" :
                            currentMode == ToolMode.Adder ? "开始添加" :
                            "开始删除", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个有效的路径。", "确定");
            }
            else
            {
                if (currentMode == ToolMode.Cleaner)
                {
                    CleanMissingScriptsInFolder(folderPath);
                }
                else if (currentMode == ToolMode.Adder)
                {
                    AddScriptsToPrefabs(folderPath, nameFilter);
                }
                else if (currentMode == ToolMode.Remover)
                {
                    RemoveSpecificComponents(folderPath);
                }
            }
        }
    }

    #region 删除特定组件

    private void RemoveSpecificComponents(string path)
    {
        string[] guids = AssetDatabase.FindAssets("", new[] { path });
        int modifiedCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (obj != null && PrefabUtility.IsPartOfPrefabAsset(obj))
            {
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);

                bool modified = RemoveComponentsRecursively(prefabRoot);

                if (modified)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                    modifiedCount++;
                }

                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        EditorUtility.DisplayDialog("完成", $"处理完成，共处理了 {modifiedCount} 个预制体。", "确定");
    }

    private bool RemoveComponentsRecursively(GameObject obj)
    {
        bool modified = false;

        // 检查是否包含 Screw 脚本
        Screw screwComponent = obj.GetComponent<Screw>();
        if (screwComponent != null)
        {
            // 遍历并删除 `Istrigger == false` 的碰撞体
            Collider2D[] colliders = obj.GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                if (!collider.isTrigger)
                {
                    DestroyImmediate(collider);
                    Debug.Log($"Removed Collider2D from GameObject: {obj.name}", obj);
                    modified = true;
                }
            }
        }

        // 递归检查子物体
        foreach (Transform child in obj.transform)
        {
            if (RemoveComponentsRecursively(child.gameObject))
            {
                modified = true;
            }
        }

        return modified;
    }

    #endregion

    #region 清理丢失脚本

    private void CleanMissingScriptsInFolder(string path)
    {
        string[] guids = AssetDatabase.FindAssets("", new[] { path });
        int removedCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (obj != null)
            {
                if (PrefabUtility.IsPartOfPrefabAsset(obj))
                {
                    GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
                    int removed = RemoveMissingScripts(prefabRoot);
                    removedCount += removed;

                    if (removed > 0)
                    {
                        PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                    }

                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
                else
                {
                    removedCount += RemoveMissingScripts(obj);
                }
            }
        }

        EditorUtility.DisplayDialog("完成", $"清理完成，共清理了 {removedCount} 个丢失脚本。", "确定");
    }

    private int RemoveMissingScripts(GameObject obj)
    {
        int count = 0;
        Transform[] allTransforms = obj.GetComponentsInChildren<Transform>(true);

        foreach (Transform t in allTransforms)
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
            if (removed > 0)
            {
                count += removed;
                EditorUtility.SetDirty(t.gameObject);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return count;
    }

    #endregion

    #region 添加脚本

    private void AddScriptsToPrefabs(string path, string nameFilter)
    {
        string[] guids = AssetDatabase.FindAssets("", new[] { path });
        int modifiedCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (obj != null && (string.IsNullOrEmpty(nameFilter) || obj.name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)))
            {
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);

                AddScriptsRecursively(prefabRoot);
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                PrefabUtility.UnloadPrefabContents(prefabRoot);

                modifiedCount++;
            }
        }

        EditorUtility.DisplayDialog("完成", $"已为 {modifiedCount} 个预制体添加脚本。", "确定");
    }

    private void AddScriptsRecursively(GameObject obj)
    {
        if (obj.name.Contains("Level", StringComparison.OrdinalIgnoreCase))
        {
            if (obj.GetComponent<Level>() == null) obj.AddComponent<Level>();
        }

        foreach (Transform child in obj.transform)
        {
            if (child.name.Contains("Layer", StringComparison.OrdinalIgnoreCase))
            {
                if (child.GetComponent<Layer>() == null) child.gameObject.AddComponent<Layer>();
            }
            else if (child.name.Contains("Glass", StringComparison.OrdinalIgnoreCase))
            {
                if (child.GetComponent<Glass>() == null) child.gameObject.AddComponent<Glass>();
            }
            else if (child.name.Contains("Hole", StringComparison.OrdinalIgnoreCase))
            {
                if (child.GetComponent<ScrewHole>() == null) child.gameObject.AddComponent<ScrewHole>();
            }
            else if (child.name.Contains("Screw", StringComparison.OrdinalIgnoreCase))
            {
                if (child.GetComponent<Screw>() == null) child.gameObject.AddComponent<Screw>();
            }

            AddScriptsRecursively(child.gameObject);
        }
    }

    #endregion
}
