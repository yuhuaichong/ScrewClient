using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using System.Linq;  // 为了使用Select方法
using System;
using DafultScript;
// 添加用于序列化的Vector3结构
[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class LevelData
{
    public string levelName;
    public int levelIndex;
    public List<LayerData> layers = new List<LayerData>();
}

[System.Serializable]
public class LayerData
{
    public int layerIndex;
    public List<GlassData> glasses = new List<GlassData>();
}

[System.Serializable]
public class GlassData
{
    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public string spritePath; // 精灵路径
    public List<HoleData> holes = new List<HoleData>();
}

[System.Serializable]
public class HoleData
{
    public SerializableVector3 position;
    public string holeType; // 固定的几种类型
}

[System.Serializable]
public class LevelConfigData
{
    public Dictionary<string, SavedLevelData> levels = new Dictionary<string, SavedLevelData>();
}

[System.Serializable]
public class BoxData
{
    public string color; // "蓝色", "紫色", "绿色"
    public int sequenceIndex; // 在序列中的位置
}

[System.Serializable]
public class SavedLevelData
{
    public string levelName;
    public List<SavedLayerData> layers = new List<SavedLayerData>();
    public Dictionary<string, int> holeColorCounts = new Dictionary<string, int>();
    public List<BoxData> boxSequence = new List<BoxData>(); // 新增：box序列
}

[System.Serializable]
public class SavedLayerData
{
    public int layerIndex;
    public int unityLayer;
    public List<SavedGlassData> glasses = new List<SavedGlassData>();
}

[System.Serializable]
public class SavedGlassData
{
    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public string spritePath;
    public List<SavedHoleData> holes = new List<SavedHoleData>();
}

[System.Serializable]
public class SavedHoleData
{
    public SerializableVector3 position;
    public string spritePath;
}

public class EditorLevel : EditorWindow
{
    private LevelConfigData levelConfig;
    private string newLevelName = "";
    private Vector2 scrollPosition;
    private string levelJsonPath;
    private GameObject currentLevelObject;
    private string selectedLevelName = "";

    string glassPath = "Assets/Prefab/Hole/glass.prefab";
    string holePath = "Assets/Prefab/Hole/Hole.prefab";

    string glassSpritePath="Assets/Images/R";
    List<string> holeSpritePath=new List<string>()
    {
        "Assets/Images/BoxAndBall/ghim_dark_blue_02",
        "Assets/Images/BoxAndBall/ghim_dark_purple_02",
        "Assets/Images/BoxAndBall/ghim_green_02",
    };
    private bool isSceneCorrect = false;

    // 修改层级列表，按照从低到高的顺序排列
    private readonly string[] layerNames = new string[] 
    {
        "Glass7", "Glass8", "Glass9", "Glass10"
        // 移除了其他层级
    };

    // 添加颜色定义
    private static readonly Color layerTitleColor = new Color(0.35f, 0.65f, 1f); // 浅蓝色
    private static readonly Color glassTitleColor = new Color(0.35f, 0.8f, 0.35f); // 浅绿色
    private static readonly Color holeTitleColor = new Color(0.8f, 0.5f, 0.9f); // 浅紫色
    private static readonly Color addButtonColor = new Color(0.3f, 0.7f, 0.3f); // 绿色
    private static readonly Color deleteButtonColor = new Color(0.7f, 0.3f, 0.3f); // 红色

    // 添加螺丝颜色定义
    private static readonly Color blueHoleColor = new Color(0.2f, 0.4f, 1f); // 蓝色
    private static readonly Color purpleHoleColor = new Color(0.8f, 0.2f, 1f); // 紫色
    private static readonly Color greenHoleColor = new Color(0.2f, 0.8f, 0.2f); // 绿色

    private readonly Dictionary<string, Color> holeColorToGuiColor = new Dictionary<string, Color>()
    {
        {"蓝色", blueHoleColor},
        {"紫色", purpleHoleColor},
        {"绿色", greenHoleColor}
    };

    private readonly Dictionary<string, string> holeColorMap = new Dictionary<string, string>()
    {
        {"Assets/Images/BoxAndBall/ghim_dark_blue_02", "蓝色"},
        {"Assets/Images/BoxAndBall/ghim_dark_purple_02", "紫色"},
        {"Assets/Images/BoxAndBall/ghim_green_02", "绿色"}
    };

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<EditorLevel>("关卡编辑器");
    }

    private void OnEnable()
    {
        levelJsonPath = Application.streamingAssetsPath + "/Level.json";
        CheckCurrentScene();
        LoadLevelConfig();
        
        // 添加编辑器更新事件
        EditorApplication.update += OnEditorUpdate;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        // 移除编辑器更新事件
        EditorApplication.update -= OnEditorUpdate;
        SceneView.duringSceneGui -= OnSceneGUI;

        // 销毁当前关卡预制体
        if (currentLevelObject != null)
        {
            DestroyImmediate(currentLevelObject);
            currentLevelObject = null;
        }
    }

    private void OnEditorUpdate()
    {
        // 如果编辑器处于播放模式或没有当前关卡对象，直接返回
        if (EditorApplication.isPlaying || currentLevelObject == null)
            return;

        // 检查是否有选中的物体
        if (Selection.activeGameObject != null)
        {
            // 检查选中的物体是否属于当前关卡
            if (Selection.activeGameObject.transform.IsChildOf(currentLevelObject.transform))
            {
                // 强制重绘编辑器窗口以更新位置显示
                Repaint();
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        // 如果编辑器处于播放模式或没有当前关卡对象，直接返回
        if (EditorApplication.isPlaying || currentLevelObject == null)
            return;

        // 检查是否正在进行场景操作
        if (Event.current.type == EventType.MouseDrag || 
            Event.current.type == EventType.MouseDown || 
            Event.current.type == EventType.MouseUp)
        {
            // 强制重绘编辑器窗口以更新位置显示
            Repaint();
        }
    }

    private void CheckCurrentScene()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        isSceneCorrect = (currentSceneName == "EditorLevelScene");
        
        if (!isSceneCorrect)
        {
            EditorUtility.DisplayDialog("警告", "请打开 EditorLevelScene 场景！", "确定");
        }
    }

    private void LoadLevelConfig()
    {
        if (File.Exists(levelJsonPath))
        {
            string jsonContent = File.ReadAllText(levelJsonPath);
            levelConfig = JsonConvert.DeserializeObject<LevelConfigData>(jsonContent);
        }
        else
        {
            levelConfig = new LevelConfigData();
            levelConfig.levels = new Dictionary<string, SavedLevelData>();
        }
    }

    private void SaveLevelConfig()
    {
        string jsonContent = JsonConvert.SerializeObject(levelConfig, Formatting.Indented);
        File.WriteAllText(levelJsonPath, jsonContent);
        AssetDatabase.Refresh();
    }

    private void CreateNewLevel(string levelName)
    {
        // 删除当前场景中的Level对象
        if (currentLevelObject != null)
        {
            DestroyImmediate(currentLevelObject);
        }

        // 创建关卡根物体
        GameObject levelObject = new GameObject(levelName);
        currentLevelObject = levelObject;
        Level levelComponent = levelObject.AddComponent<Level>();

        // 创建第一个layer，默认为Glass7
        CreateNewLayer(levelObject);
        
        selectedLevelName = levelName;
    }

    private void CreateNewLayer(GameObject levelObject)
    {
        int layerIndex = levelObject.transform.childCount;
        GameObject layerObject = new GameObject($"layer ({layerIndex})");
        layerObject.transform.SetParent(levelObject.transform);
        Layer layerComponent = layerObject.AddComponent<Layer>();
        
        // 根据已有的layer数量决定新layer的层级
        string layerName;
        if (layerIndex < layerNames.Length)
        {
            layerName = layerNames[layerIndex];
        }
        else
        {
            // 如果超过了最大层数，使用最高层级
            layerName = layerNames[layerNames.Length - 1];
        }
        
        // 设置layer的层级
        layerObject.layer = LayerMask.NameToLayer(layerName);
    }

    private void CreateNewGlass(GameObject layerObject, Vector3 position)
    {
        GameObject glassPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(glassPath);
        if (glassPrefab != null)
        {
            GameObject glassInstance = PrefabUtility.InstantiatePrefab(glassPrefab) as GameObject;
            glassInstance.transform.SetParent(layerObject.transform);
            glassInstance.transform.localPosition = position;

            // 获取父layer的层级名称
            string layerName = LayerMask.LayerToName(layerObject.layer);
            
            // 设置glass的layer和SpriteRenderer
            glassInstance.layer = layerObject.layer;
            var renderer = glassInstance.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = layerName;
            }
        }
    }

    private void CreateNewHole(GameObject glassObject, Vector3 position)
    {
        GameObject holePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(holePath);
        if (holePrefab != null)
        {
            GameObject holeInstance = PrefabUtility.InstantiatePrefab(holePrefab) as GameObject;
            holeInstance.transform.SetParent(glassObject.transform);
            holeInstance.transform.localPosition = position;

            // 获取glass的层级
            string currentLayerName = LayerMask.LayerToName(glassObject.layer);
            
            // 设置Hole的layer为与glass相同
            holeInstance.layer = glassObject.layer;

            // 获取当前层级在layerNames中的索引
            int currentIndex = System.Array.IndexOf(layerNames, currentLayerName);

            holeInstance.transform.Find("Screw").gameObject.layer = glassObject.layer;
            
            // 设置默认紫色精灵
            var imageRenderer = holeInstance.transform.Find("Screw/Image").GetComponent<SpriteRenderer>();
            imageRenderer.sortingLayerName = currentLayerName;
            string defaultSpritePath = "Assets/Images/BoxAndBall/ghim_dark_purple_02.png";
            imageRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(defaultSpritePath);

            var spriteMask = holeInstance.transform.Find("Mask").GetComponent<SpriteMask>();
            spriteMask.frontSortingLayerID = SortingLayer.NameToID(currentLayerName);
            
            if (currentIndex < layerNames.Length - 1)
            {
                spriteMask.backSortingLayerID = SortingLayer.NameToID(layerNames[currentIndex + 1]);
            }
            else
            {
                spriteMask.backSortingLayerID = SortingLayer.NameToID(currentLayerName);
            }
        }
    }

    private void ShowTransformEditor(Transform transform, bool isGlass = false)
    {
        EditorGUI.BeginChangeCheck();
        Vector2 position = EditorGUILayout.Vector2Field("Position", new Vector2(transform.localPosition.x, transform.localPosition.y));
        
        // 只为 glass 显示 Z 轴旋转编辑
        if (isGlass)
        {
            float rotationZ = EditorGUILayout.FloatField("Rotation Z", transform.localEulerAngles.z);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(transform, "Transform Change");
                transform.localPosition = new Vector3(position.x, position.y, 0);
                transform.localEulerAngles = new Vector3(0, 0, rotationZ);
            }
        }
        else
        {
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(transform, "Move Object");
                transform.localPosition = new Vector3(position.x, position.y, 0);
            }
        }
    }

    private void ShowSpriteSelector(SpriteRenderer spriteRenderer, bool isGlass)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(isGlass ? "Glass Sprite" : "Hole Sprite");
        
        // 显示当前选中的Sprite预览
        Rect previewRect = GUILayoutUtility.GetRect(50, 50);
        if (spriteRenderer.sprite != null)
        {
            GUI.DrawTexture(previewRect, spriteRenderer.sprite.texture, ScaleMode.ScaleToFit);
        }

        // 获取可选择的sprite路径列表
        string[] availablePaths;
        if (isGlass)
        {
            // 获取glass文件夹下所有的png文件
            availablePaths = Directory.GetFiles(glassSpritePath, "*.png")
                .Select(path => path.Replace("\\", "/")) // 统一路径分隔符
                .ToArray();
        }
        else
        {
            // 使用预定义的hole sprite路径
            availablePaths = holeSpritePath.Select(path => path + ".png").ToArray();
        }

        // 获取当前选中的索引
        int currentIndex = -1;
        if (spriteRenderer.sprite != null)
        {
            string currentPath = AssetDatabase.GetAssetPath(spriteRenderer.sprite);
            currentIndex = Array.IndexOf(availablePaths, currentPath);
        }

        // 显示下拉选择框
        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUILayout.Popup(currentIndex, availablePaths.Select(Path.GetFileNameWithoutExtension).ToArray());
        if (EditorGUI.EndChangeCheck() && newIndex != -1)
        {
            string selectedPath = availablePaths[newIndex];
            Sprite selectedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(selectedPath);
            if (selectedSprite != null)
            {
                Undo.RecordObject(spriteRenderer, isGlass ? "Change Glass Sprite" : "Change Hole Sprite");
                spriteRenderer.sprite = selectedSprite;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ShowHoleTypeSelector(GameObject holeObject)
    {
        string[] holeTypes = new string[] { "Type1", "Type2", "Type3" }; // 添加你的hole类型
        int currentType = 0; // 获取当前类型
        
        EditorGUI.BeginChangeCheck();
        int newType = EditorGUILayout.Popup("Hole Type", currentType, holeTypes);
        if (EditorGUI.EndChangeCheck())
        {
            // 更改hole类型的逻辑
        }
    }

    private void ShowLayerSelector(GameObject layerObject)
    {
        // 获取当前layer的层级
        int currentIndex = 0;
        string currentLayerName = LayerMask.LayerToName(layerObject.layer);
        for (int i = 0; i < layerNames.Length; i++)
        {
            if (layerNames[i] == currentLayerName)
            {
                currentIndex = i;
                break;
            }
        }

        // 显示层级选择器
        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUILayout.Popup("Sorting Layer", currentIndex, layerNames);
        if (EditorGUI.EndChangeCheck())
        {
            // 设置layer对象的层级
            layerObject.layer = LayerMask.NameToLayer(layerNames[newIndex]);
            SetSortingLayerRecursively(layerObject, layerNames[newIndex]);
        }
    }

    private void SetSortingLayerRecursively(GameObject obj, string sortingLayerName)
    {
        if (obj == null) return;

        foreach (Transform child in obj.transform)
        {
            // 设置glass物体的layer和SpriteRenderer的sortingLayerName
            if (child.name.Contains("glass"))
            {
                // 设置glass物体的layer
                child.gameObject.layer = LayerMask.NameToLayer(sortingLayerName);
                
                // 设置glass的SpriteRenderer的sortingLayerName
                var glassRenderer = child.GetComponent<SpriteRenderer>();
                if (glassRenderer != null)
                {
                    glassRenderer.sortingLayerName = sortingLayerName;
                }

                // 遍历glass的子物体（Hole）
                foreach (Transform holeChild in child)
                {
                    if (holeChild.name.Contains("Hole"))
                    {
                        // 设置Hole的layer
                        holeChild.gameObject.layer = LayerMask.NameToLayer(sortingLayerName);

                        // 获取当前层级在layerNames中的索引
                        int currentIndex = System.Array.IndexOf(layerNames, sortingLayerName);

                        // 设置Screw的layer和Image的sortingLayerName
                        holeChild.transform.Find("Screw").gameObject.layer = LayerMask.NameToLayer(sortingLayerName);
                        holeChild.transform.Find("Screw/Image").GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;

                        // 设置Mask的SpriteMask
                        var spriteMask = holeChild.transform.Find("Mask").GetComponent<SpriteMask>();
                        if (spriteMask != null)
                        {
                            spriteMask.frontSortingLayerID = SortingLayer.NameToID(sortingLayerName);
                            
                            // 如果不是最后一层，就设置back为下一层
                            if (currentIndex < layerNames.Length - 1)
                            {
                                spriteMask.backSortingLayerID = SortingLayer.NameToID(layerNames[currentIndex + 1]);
                            }
                            else
                            {
                                // 如果是最后一层，就用当前层
                                spriteMask.backSortingLayerID = SortingLayer.NameToID(sortingLayerName);
                            }
                        }
                    }
                }
            }
        }
    }

    private void ShowColoredLabel(string text, Color color)
    {
        var originalColor = GUI.color;
        GUI.color = color;
        EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
        GUI.color = originalColor;
    }

    private bool ShowColoredButton(string text, Color color, GUILayoutOption[] options = null)
    {
        var originalColor = GUI.color;
        GUI.color = color;
        bool result = options != null ? 
            GUILayout.Button(text, options) : 
            GUILayout.Button(text, GUILayout.Width(80));
        GUI.color = originalColor;
        return result;
    }

    private void ShowGlassContent(Transform glassTransform, List<GameObject> objectsToDelete)
    {
        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.BeginHorizontal();
            {
                ShowColoredLabel("Glass: " + glassTransform.name, glassTitleColor);
                GUILayout.FlexibleSpace();
                if (ShowColoredButton("删除Glass", deleteButtonColor))
                {
                    objectsToDelete.Add(glassTransform.gameObject);
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.BeginVertical();
            {
                ShowTransformEditor(glassTransform, true);  // 传入 true 表示这是 glass
                
                var spriteRenderer = glassTransform.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    ShowSpriteSelector(spriteRenderer, true);
                }

                EditorGUILayout.Space(5);

                // Holes 区域
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        ShowColoredLabel("Holes", holeTitleColor);
                        GUILayout.FlexibleSpace();
                        if (ShowColoredButton("添加Hole", holeTitleColor))
                        {
                            CreateNewHole(glassTransform.gameObject, Vector3.zero);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.BeginVertical();
                    {
                        foreach (Transform holeTransform in glassTransform)
                        {
                            if (holeTransform.name.Contains("Hole"))
                            {
                                ShowHoleContent(holeTransform, objectsToDelete);
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }

    private void ShowHoleContent(Transform holeTransform, List<GameObject> objectsToDelete)
    {
        EditorGUILayout.BeginVertical("box");
        {
            EditorGUILayout.BeginHorizontal();
            {
                ShowColoredLabel("Hole: " + holeTransform.name, holeTitleColor);
                GUILayout.FlexibleSpace();
                if (ShowColoredButton("删除Hole", deleteButtonColor))
                {
                    objectsToDelete.Add(holeTransform.gameObject);
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            ShowTransformEditor(holeTransform);
            
            var holeSpriteRenderer = holeTransform.transform.Find("Screw/Image").GetComponent<SpriteRenderer>();
            if (holeSpriteRenderer != null)
            {
                ShowSpriteSelector(holeSpriteRenderer, false);
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }

    private Dictionary<string, int> CountHoleColors()
    {
        Dictionary<string, int> colorCounts = new Dictionary<string, int>()
        {
            {"蓝色", 0},
            {"紫色", 0},
            {"绿色", 0}
        };

        if (currentLevelObject == null) return colorCounts;

        // 遍历所有层级
        foreach (Transform layerTransform in currentLevelObject.transform)
        {
            // 遍历所有glass
            foreach (Transform glassTransform in layerTransform)
            {
                // 遍历所有hole
                foreach (Transform holeTransform in glassTransform)
                {
                    if (holeTransform.name.Contains("Hole"))
                    {
                        var holeSpriteRenderer = holeTransform.transform.Find("Screw/Image")?.GetComponent<SpriteRenderer>();
                        if (holeSpriteRenderer != null && holeSpriteRenderer.sprite != null)
                        {
                            string spritePath = AssetDatabase.GetAssetPath(holeSpriteRenderer.sprite);
                            // 移除.png后缀进行比较
                            spritePath = spritePath.Replace(".png", "");
                            if (holeColorMap.ContainsKey(spritePath))
                            {
                                string color = holeColorMap[spritePath];
                                colorCounts[color]++;
                            }
                        }
                    }
                }
            }
        }

        return colorCounts;
    }

    private void ShowColoredHoleCount(string colorName, int count)
    {
        var originalColor = GUI.color;
        GUI.color = holeColorToGuiColor[colorName];
        EditorGUILayout.LabelField($"{colorName}螺丝：{count}个");
        GUI.color = originalColor;
    }

    private void ShowColoredBoxLabel(string prefix, string colorName)
    {
        var originalColor = GUI.color;
        GUI.color = holeColorToGuiColor[colorName];
        EditorGUILayout.LabelField($"{prefix}：{colorName}");
        GUI.color = originalColor;
    }

    private bool ValidateHoleCounts(Dictionary<string, int> colorCounts)
    {
        bool isValid = true;
        string errorMessage = "";

        foreach (var colorCount in colorCounts)
        {
            if (colorCount.Value % 3 != 0)
            {
                isValid = false;
                errorMessage += $"{colorCount.Key}螺丝数量({colorCount.Value})不是3的倍数\n";
            }
        }

        if (!isValid)
        {
            EditorUtility.DisplayDialog("错误", errorMessage + "\n所有颜色的螺丝数量必须是3的倍数！", "确定");
        }

        return isValid;
    }

    private List<BoxData> GenerateBoxSequence()
    {
        List<BoxData> sequence = new List<BoxData>();
        if (currentLevelObject == null) return sequence;

        // 获取所有hole的颜色统计
        var colorCounts = CountHoleColors();
        
        // 创建一个字典来存储每个hole的信息
        Dictionary<Transform, bool> holeVisibility = new Dictionary<Transform, bool>();
        Dictionary<Transform, string> holeColors = new Dictionary<Transform, string>();
        
        // 从最底层开始遍历（Glass7 -> Glass10）
        for (int i = 0; i < layerNames.Length; i++)
        {
            string currentLayer = layerNames[i];
            foreach (Transform layerTransform in currentLevelObject.transform)
            {
                if (layerTransform.gameObject.layer == LayerMask.NameToLayer(currentLayer))
                {
                    foreach (Transform glassTransform in layerTransform)
                    {
                        foreach (Transform holeTransform in glassTransform)
                        {
                            if (holeTransform.name.Contains("Hole"))
                            {
                                // 获取hole的颜色
                                var holeSpriteRenderer = holeTransform.transform.Find("Screw/Image")?.GetComponent<SpriteRenderer>();
                                if (holeSpriteRenderer != null && holeSpriteRenderer.sprite != null)
                                {
                                    string spritePath = AssetDatabase.GetAssetPath(holeSpriteRenderer.sprite).Replace(".png", "");
                                    if (holeColorMap.ContainsKey(spritePath))
                                    {
                                        string color = holeColorMap[spritePath];
                                        holeColors[holeTransform] = color;
                                        
                                        // 检查这个hole是否被上层的glass遮挡
                                        bool isVisible = true;
                                        Vector3 holeWorldPos = holeTransform.position;
                                        
                                        // 检查所有更高层级的glass是否遮挡这个hole
                                        for (int j = i + 1; j < layerNames.Length; j++)
                                        {
                                            string upperLayer = layerNames[j];
                                            foreach (Transform upperLayerTransform in currentLevelObject.transform)
                                            {
                                                if (upperLayerTransform.gameObject.layer == LayerMask.NameToLayer(upperLayer))
                                                {
                                                    foreach (Transform upperGlassTransform in upperLayerTransform)
                                                    {
                                                        // 检查hole的位置是否在glass的范围内
                                                        var glassRenderer = upperGlassTransform.GetComponent<SpriteRenderer>();
                                                        if (glassRenderer != null)
                                                        {
                                                            Bounds glassBounds = glassRenderer.bounds;
                                                            if (glassBounds.Contains(holeWorldPos))
                                                            {
                                                                isVisible = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (!isVisible) break;
                                        }
                                        
                                        holeVisibility[holeTransform] = isVisible;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 根据可见性生成box序列
        int sequenceIndex = 0;
        Dictionary<string, List<Transform>> visibleHolesByColor = new Dictionary<string, List<Transform>>();
        Dictionary<string, List<Transform>> hiddenHolesByColor = new Dictionary<string, List<Transform>>();

        // 初始化颜色分组字典
        foreach (string color in holeColorMap.Values.Distinct())
        {
            visibleHolesByColor[color] = new List<Transform>();
            hiddenHolesByColor[color] = new List<Transform>();
        }

        // 对hole按颜色和可见性分组
        foreach (var hole in holeVisibility)
        {
            string color = holeColors[hole.Key];
            if (hole.Value)
            {
                visibleHolesByColor[color].Add(hole.Key);
            }
            else
            {
                hiddenHolesByColor[color].Add(hole.Key);
            }
        }

        // 计算每种颜色需要的box数量（每种颜色的hole总数除以3）
        Dictionary<string, int> boxesPerColor = new Dictionary<string, int>();
        foreach (var colorCount in colorCounts)
        {
            boxesPerColor[colorCount.Key] = colorCount.Value / 3;
        }

        // 从最上层开始生成box序列（因为最上层的holes应该最先被点击）
        for (int i = layerNames.Length - 1; i >= 0; i--)
        {
            foreach (var colorBoxCount in boxesPerColor)
            {
                string color = colorBoxCount.Key;
                var visibleHoles = visibleHolesByColor[color];
                
                // 找出当前层的可见holes
                var currentLayerHoles = visibleHoles.Where(h => 
                    h.GetComponentInParent<Layer>().gameObject.layer == LayerMask.NameToLayer(layerNames[i]));

                // 为当前层的holes生成box
                int holesInCurrentLayer = currentLayerHoles.Count();
                int boxesNeeded = (holesInCurrentLayer + 2) / 3; // 向上取整

                for (int j = 0; j < boxesNeeded && sequence.Count < boxesPerColor[color]; j++)
                {
                    sequence.Add(new BoxData { 
                        color = color, 
                        sequenceIndex = sequenceIndex++ 
                    });
                }
            }
        }

        // 添加剩余的box（用于被遮挡的holes）
        foreach (var colorBoxCount in boxesPerColor)
        {
            string color = colorBoxCount.Key;
            int remainingBoxes = colorBoxCount.Value - sequence.Count(b => b.color == color);
            
            for (int i = 0; i < remainingBoxes; i++)
            {
                sequence.Add(new BoxData { 
                    color = color, 
                    sequenceIndex = sequenceIndex++ 
                });
            }
        }

        return sequence;
    }

    private void ShowBoxSequence(SavedLevelData levelData)
    {
        if (levelData.boxSequence != null && levelData.boxSequence.Count > 0)
        {
            EditorGUILayout.LabelField("Box顺序：", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40);
            EditorGUILayout.BeginVertical();
            foreach (var box in levelData.boxSequence.OrderBy(b => b.sequenceIndex))
            {
                ShowColoredBoxLabel($"Box {box.sequenceIndex + 1}", box.color);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void OnGUI()
    {
        if (!isSceneCorrect)
        {
            EditorGUILayout.HelpBox("请打开 EditorLevelScene 场景！", MessageType.Error);
            return;
        }

        // 顶部信息区域
        EditorGUILayout.BeginVertical("box");
        {
            // 显示当前选中的关卡
            if (currentLevelObject != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("当前关卡：" + currentLevelObject.name, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("保存关卡数据", GUILayout.Width(100)))
                {
                    var currentColorCounts = CountHoleColors();
                    if (ValidateHoleCounts(currentColorCounts))
                    {
                        SaveCurrentLevel();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // 显示当前关卡的螺丝统计
                EditorGUILayout.Space(5);
                var colorCounts = CountHoleColors();
                EditorGUILayout.LabelField("螺丝统计：", EditorStyles.boldLabel);
                foreach (var colorCount in colorCounts)
                {
                    ShowColoredHoleCount(colorCount.Key, colorCount.Value);
                }

                // 显示当前关卡的box序列
                if (levelConfig.levels.ContainsKey(currentLevelObject.name))
                {
                    EditorGUILayout.Space(10);
                    ShowBoxSequence(levelConfig.levels[currentLevelObject.name]);
                }
            }

            EditorGUILayout.Space(10);

            // 显示已有关卡列表
            EditorGUILayout.LabelField("已有关卡列表：", EditorStyles.boldLabel);
            if (levelConfig != null && levelConfig.levels.Count > 0)
            {
                foreach (var levelPair in levelConfig.levels)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        if (GUILayout.Button(levelPair.Key, GUILayout.Width(200)))
                        {
                            LoadExistingLevel(levelPair.Key);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(5);
                }
            }
            else
            {
                EditorGUILayout.LabelField("暂无关卡数据");
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // 创建新关卡区域
        EditorGUILayout.BeginVertical("box");
        {
            ShowColoredLabel("创建新关卡", layerTitleColor);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            newLevelName = EditorGUILayout.TextField("关卡名称", newLevelName);
            if (ShowColoredButton("创建关卡", layerTitleColor, new[] { GUILayout.Width(100) }))
            {
                if (string.IsNullOrEmpty(newLevelName))
                {
                    EditorUtility.DisplayDialog("错误", "请输入关卡名称！", "确定");
                    return;
                }
                CreateNewLevel(newLevelName);
                newLevelName = "";
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // 层级编辑区域
        if (currentLevelObject != null)
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    ShowColoredLabel("层级编辑", layerTitleColor);
                    GUILayout.FlexibleSpace();
                    if (currentLevelObject.transform.childCount < layerNames.Length)
                    {
                        if (ShowColoredButton("添加新层级", layerTitleColor, new[] { GUILayout.Width(100) }))
                        {
                            CreateNewLayer(currentLevelObject);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(10);

                // 开始滚动视图
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                {
                    List<GameObject> objectsToDelete = new List<GameObject>();

                    foreach (Transform layerTransform in currentLevelObject.transform)
                    {
                        EditorGUILayout.BeginVertical("box");
                        {
                            EditorGUILayout.BeginHorizontal("box");
                            {
                                ShowColoredLabel($"Layer: {layerTransform.name}", layerTitleColor);
                                GUILayout.FlexibleSpace();
                                if (ShowColoredButton("删除层级", deleteButtonColor))
                                {
                                    objectsToDelete.Add(layerTransform.gameObject);
                                    continue;
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(5);
                            ShowLayerSelector(layerTransform.gameObject);
                            EditorGUILayout.Space(5);

                            // Glass 区域
                            EditorGUILayout.BeginVertical("box");
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    ShowColoredLabel("Glass 列表", glassTitleColor);
                                    GUILayout.FlexibleSpace();
                                    if (ShowColoredButton("添加Glass", glassTitleColor))
                                    {
                                        CreateNewGlass(layerTransform.gameObject, Vector3.zero);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space(5);

                                foreach (Transform glassTransform in layerTransform)
                                {
                                    ShowGlassContent(glassTransform, objectsToDelete);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(10);
                    }

                    foreach (var obj in objectsToDelete)
                    {
                        DestroyImmediate(obj);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
    }

    // 添加删除确认对话框的辅助方法
    private bool ConfirmDelete(string objectType)
    {
        return EditorUtility.DisplayDialog("确认删除", 
            $"确定要删除这个{objectType}吗？", 
            "确定", "取消");
    }

    private void LoadExistingLevel(string levelName)
    {
        if (!levelConfig.levels.ContainsKey(levelName))
        {
            Debug.LogError($"Level {levelName} not found in config!");
            return;
        }

        // 删除当前场景中的Level对象
        if (currentLevelObject != null)
        {
            DestroyImmediate(currentLevelObject);
        }

        // 获取保存的关卡数据
        SavedLevelData levelData = levelConfig.levels[levelName];

        // 创建关卡根物体
        GameObject levelObject = new GameObject(levelName);
        currentLevelObject = levelObject;
        Level levelComponent = levelObject.AddComponent<Level>();

        // 创建所有层级
        foreach (var layerData in levelData.layers)
        {
            // 创建layer对象
            GameObject layerObject = new GameObject($"layer ({layerData.layerIndex})");
            layerObject.transform.SetParent(levelObject.transform);
            Layer layerComponent = layerObject.AddComponent<Layer>();
            layerObject.layer = layerData.unityLayer;

            // 创建所有glass
            foreach (var glassData in layerData.glasses)
            {
                GameObject glassPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(glassPath);
                if (glassPrefab != null)
                {
                    GameObject glassInstance = PrefabUtility.InstantiatePrefab(glassPrefab) as GameObject;
                    glassInstance.transform.SetParent(layerObject.transform);
                    glassInstance.transform.localPosition = glassData.position.ToVector3();
                    glassInstance.transform.localEulerAngles = glassData.rotation.ToVector3();
                    glassInstance.layer = layerObject.layer;

                    // 设置glass的sprite
                    var renderer = glassInstance.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(glassData.spritePath);
                        renderer.sortingLayerName = LayerMask.LayerToName(layerObject.layer);
                    }

                    // 创建所有hole
                    foreach (var holeData in glassData.holes)
                    {
                        GameObject holePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(holePath);
                        if (holePrefab != null)
                        {
                            GameObject holeInstance = PrefabUtility.InstantiatePrefab(holePrefab) as GameObject;
                            holeInstance.transform.SetParent(glassInstance.transform);
                            holeInstance.transform.localPosition = holeData.position.ToVector3();
                            holeInstance.layer = layerObject.layer;

                            // 设置hole的sprite和layer
                            string currentLayerName = LayerMask.LayerToName(layerObject.layer);
                            int currentIndex = System.Array.IndexOf(layerNames, currentLayerName);

                            var screwObj = holeInstance.transform.Find("Screw");
                            screwObj.gameObject.layer = layerObject.layer;
                            
                            var imageRenderer = screwObj.Find("Image").GetComponent<SpriteRenderer>();
                            imageRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(holeData.spritePath);
                            imageRenderer.sortingLayerName = currentLayerName;

                            var spriteMask = holeInstance.transform.Find("Mask").GetComponent<SpriteMask>();
                            spriteMask.frontSortingLayerID = SortingLayer.NameToID(currentLayerName);
                            
                            if (currentIndex < layerNames.Length - 1)
                            {
                                spriteMask.backSortingLayerID = SortingLayer.NameToID(layerNames[currentIndex + 1]);
                            }
                            else
                            {
                                spriteMask.backSortingLayerID = SortingLayer.NameToID(currentLayerName);
                            }
                        }
                    }
                }
            }
        }

        selectedLevelName = levelName;
    }

    private void SaveCurrentLevel()
    {
        if (currentLevelObject == null) return;

        SavedLevelData levelData = new SavedLevelData();
        levelData.levelName = currentLevelObject.name;
        levelData.layers = new List<SavedLayerData>();
        levelData.holeColorCounts = CountHoleColors();
        
        // 生成并保存box序列
        levelData.boxSequence = GenerateBoxSequence();

        // 遍历所有层级
        foreach (Transform layerTransform in currentLevelObject.transform)
        {
            SavedLayerData layerData = new SavedLayerData();
            layerData.layerIndex = layerTransform.GetSiblingIndex();
            layerData.unityLayer = layerTransform.gameObject.layer;
            layerData.glasses = new List<SavedGlassData>();

            // 遍历层级下的所有glass
            foreach (Transform glassTransform in layerTransform)
            {
                SavedGlassData glassData = new SavedGlassData();
                glassData.position = new SerializableVector3(glassTransform.localPosition);
                glassData.rotation = new SerializableVector3(glassTransform.localEulerAngles);
                glassData.holes = new List<SavedHoleData>();
                
                var spriteRenderer = glassTransform.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    glassData.spritePath = AssetDatabase.GetAssetPath(spriteRenderer.sprite);
                }

                foreach (Transform holeTransform in glassTransform)
                {
                    if (holeTransform.name.Contains("Hole"))
                    {
                        SavedHoleData holeData = new SavedHoleData();
                        holeData.position = new SerializableVector3(holeTransform.localPosition);
                        
                        var holeSpriteRenderer = holeTransform.transform.Find("Screw/Image")?.GetComponent<SpriteRenderer>();
                        if (holeSpriteRenderer != null && holeSpriteRenderer.sprite != null)
                        {
                            holeData.spritePath = AssetDatabase.GetAssetPath(holeSpriteRenderer.sprite);
                        }
                        
                        glassData.holes.Add(holeData);
                    }
                }

                layerData.glasses.Add(glassData);
            }

            levelData.layers.Add(layerData);
        }

        // 更新或添加关卡数据到字典
        levelConfig.levels[levelData.levelName] = levelData;

        // 保存到文件
        string json = JsonConvert.SerializeObject(levelConfig, Formatting.Indented,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        File.WriteAllText(levelJsonPath, json);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("提示", "关卡数据保存成功！", "确定");
    }
}
