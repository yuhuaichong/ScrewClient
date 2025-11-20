using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnusedResourceChecker : EditorWindow
{
    private string targetFolderPath = "Assets/"; // 默认路径
    private Vector2 scrollPosition;
    private List<string> unusedResources = new List<string>();

    [MenuItem("Tools/Check Unused Resources")]
    public static void ShowWindow()
    {
        GetWindow<UnusedResourceChecker>("Unused Resource Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Check Unused Resources", EditorStyles.boldLabel);

        // 选择文件夹路径
        GUILayout.Label("Target Folder Path:");
        targetFolderPath = GUILayout.TextField(targetFolderPath);
        if (GUILayout.Button("Select Folder"))
        {
            targetFolderPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
        }

        // 开始检查
        if (GUILayout.Button("Check Unused Resources"))
        {
            CheckUnusedResources();
        }

        // 显示未引用的资源列表
        if (unusedResources.Count > 0)
        {
            GUILayout.Label("Unused Resources:", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            foreach (var resource in unusedResources)
            {
                GUILayout.BeginHorizontal();

                // 显示资源路径
                GUILayout.Label(resource, GUILayout.Width(400));

                // 添加“选择”按钮
                if (GUILayout.Button("Select", GUILayout.Width(80)))
                {
                    SelectResource(resource);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("All resources are used in the scene or prefabs.");
        }
    }

    private void CheckUnusedResources()
    {
        unusedResources.Clear();

        // 获取目标文件夹下的所有图片资源
        string[] imagePaths = Directory.GetFiles(targetFolderPath, "*.png", SearchOption.AllDirectories);

        // 遍历所有图片资源
        foreach (var imagePath in imagePaths)
        {
            string assetPath = "Assets" + imagePath.Substring(Application.dataPath.Length);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (texture == null)
            {
                Debug.LogWarning($"Failed to load texture at path: {assetPath}");
                continue;
            }

            // 检查场景和预制体是否引用该资源
            if (!IsTextureUsedInScene(texture) && !IsTextureUsedInPrefabs(texture))
            {
                unusedResources.Add(assetPath);
            }
        }
    }

    private bool IsTextureUsedInScene(Texture2D texture)
    {
        // 获取场景中所有游戏对象（包括未激活的）
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var obj in allObjects)
        {
            // 检查Renderer组件
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && material.mainTexture == texture)
                    {
                        return true;
                    }
                }
            }

            // 检查UI Image组件
            Image image = obj.GetComponent<Image>();
            if (image != null && image.sprite != null && image.sprite.texture == texture)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTextureUsedInPrefabs(Texture2D texture)
    {
        // 获取Assets文件夹下所有预制体
        string[] prefabPaths = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);

        foreach (var prefabPath in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Failed to load prefab at path: {prefabPath}");
                continue;
            }

            // 检查预制体是否引用该资源
            if (IsTextureUsedInGameObject(prefab, texture))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTextureUsedInGameObject(GameObject gameObject, Texture2D texture)
    {
        // 检查Renderer组件
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                if (material != null && material.mainTexture == texture)
                {
                    return true;
                }
            }
        }

        // 检查UI Image组件
        Image image = gameObject.GetComponent<Image>();
        if (image != null && image.sprite != null && image.sprite.texture == texture)
        {
            return true;
        }

        // 递归检查子物体
        foreach (Transform child in gameObject.transform)
        {
            if (IsTextureUsedInGameObject(child.gameObject, texture))
            {
                return true;
            }
        }

        return false;
    }

    private void SelectResource(string resourcePath)
    {
        // 定位到资源
        Object resource = AssetDatabase.LoadAssetAtPath<Object>(resourcePath);
        if (resource != null)
        {
            EditorGUIUtility.PingObject(resource);
            Selection.activeObject = resource;
        }
        else
        {
            Debug.LogWarning($"Failed to locate resource at path: {resourcePath}");
        }
    }
}