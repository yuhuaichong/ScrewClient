using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class UnusedMaterialFinder : EditorWindow
{
    private string searchPath = "Assets"; // 默认搜索路径
    private Vector2 scrollPosition; // 用于滚动条
    private List<Material> unusedMaterials = new List<Material>(); // 存储未引用的材料

    [MenuItem("Tools/Find Unused Materials")]
    public static void ShowWindow()
    {
        GetWindow<UnusedMaterialFinder>("Unused Material Finder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find Unused Materials", EditorStyles.boldLabel);

        // 输入搜索路径
        searchPath = EditorGUILayout.TextField("Search Path", searchPath);

        // 查找按钮
        if (GUILayout.Button("Find Unused Materials"))
        {
            FindUnusedMaterials();
        }

        // 显示未引用的材料
        GUILayout.Label("Unused Materials:", EditorStyles.boldLabel);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        foreach (var material in unusedMaterials)
        {
            GUILayout.BeginHorizontal();

            // 显示材料名称
            GUILayout.Label(material.name, GUILayout.Width(200));

            // 定位按钮
            if (GUILayout.Button("Select", GUILayout.Width(100)))
            {
                Selection.activeObject = material;
                EditorGUIUtility.PingObject(material);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void FindUnusedMaterials()
    {
        unusedMaterials.Clear();

        // 获取指定路径下的所有材料
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { searchPath });
        List<Material> materials = new List<Material>();

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            materials.Add(material);
        }

        // 获取所有资源
        string[] allAssetGuids = AssetDatabase.FindAssets("t:Object");
        HashSet<Material> referencedMaterials = new HashSet<Material>();

        foreach (string guid in allAssetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string[] dependencies = AssetDatabase.GetDependencies(path, false);

            foreach (string dependencyPath in dependencies)
            {
                // 加载依赖资源
                Object dependency = AssetDatabase.LoadAssetAtPath<Object>(dependencyPath);
                if (dependency is Material)
                {
                    referencedMaterials.Add(dependency as Material);
                }
            }
        }

        // 找出未引用的材料
        unusedMaterials = materials.Except(referencedMaterials).ToList();
    }
}