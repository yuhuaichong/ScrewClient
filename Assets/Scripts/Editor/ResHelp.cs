using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using DafultScript;
public class ResHelp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [MenuItem("Tools/检测不合格的关卡盒子")]
    public static void CheckInvalidBoxLevels()
    {
        string boxLevelPath = "Assets/AA/BoxLevel";
        string[] prefabFiles = Directory.GetFiles(boxLevelPath, "*.prefab");

        List<string> invalidPrefabs = new List<string>();
        int totalCount = 0;
        int invalidCount = 0;

        Debug.Log("开始检测关卡盒子...");

        foreach (string prefabPath in prefabFiles)
        {
            totalCount++;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogWarning($"无法加载预制体: {prefabPath}");
                continue;
            }

            // 检查每个子对象是否有Box组件，以及Box是否合格
            bool hasInvalidBox = false;
            string invalidReason = "";

            for (int i = 0; i < prefab.transform.childCount; i++)
            {
                Transform child = prefab.transform.GetChild(i);
                Box childBox = child.GetComponent<Box>();

                if (childBox == null)
                {
                    hasInvalidBox = true;
                    invalidReason += $"子对象 {child.name} 没有Box组件 ";
                    continue;
                }

                // 检查Box的子对象数量（空位数量）
                int boxChildCount = childBox.transform.childCount;

                // 检查Box是否有星星洞口
                bool hasStarHole = false;
                for (int j = 0; j < childBox.transform.childCount; j++)
                {
                    Transform holeChild = childBox.transform.GetChild(j);
                    if (holeChild.GetComponent<StarHole>() != null)
                    {
                        hasStarHole = true;
                        break;
                    }
                }

                // 判断Box是否不合格：不是3个空位 或 有星星洞口
                bool isBoxInvalid = (boxChildCount != 3) || hasStarHole;

                if (isBoxInvalid)
                {
                    hasInvalidBox = true;
                    if (boxChildCount != 3) invalidReason += $"Box {child.name} 空位数量不是3个(实际:{boxChildCount}) ";
                    if (hasStarHole) invalidReason += $"Box {child.name} 包含星星洞口 ";
                }
            }

            if (hasInvalidBox)
            {
                invalidCount++;
                invalidPrefabs.Add(prefabPath);
                Debug.Log($"不合格的关卡盒子: {Path.GetFileName(prefabPath)} - {invalidReason}");
            }
        }

        Debug.Log($"检测完成! 总共 {totalCount} 个关卡盒子，不合格的有 {invalidCount} 个");

        if (invalidPrefabs.Count > 0)
        {
            string message = $"发现 {invalidPrefabs.Count} 个不合格的关卡盒子:\n\n";
            foreach (string prefab in invalidPrefabs)
            {
                message += Path.GetFileName(prefab) + "\n";
            }

            EditorUtility.DisplayDialog("检测结果", message, "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("检测结果", "所有关卡盒子都符合要求！", "确定");
        }
    }

    [MenuItem("Tools/检测并删除不合格的关卡盒子")]
    public static void CheckAndDeleteInvalidBoxLevels()
    {
        string boxLevelPath = "Assets/AA/BoxLevel";
        string[] prefabFiles = Directory.GetFiles(boxLevelPath, "*.prefab");

        List<string> invalidPrefabs = new List<string>();
        int totalCount = 0;
        int invalidCount = 0;

        Debug.Log("开始检测关卡盒子...");

        foreach (string prefabPath in prefabFiles)
        {
            totalCount++;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogWarning($"无法加载预制体: {prefabPath}");
                continue;
            }

            // 检查每个子对象是否有Box组件，以及Box是否合格
            bool hasInvalidBox = false;
            string invalidReason = "";

            for (int i = 0; i < prefab.transform.childCount; i++)
            {
                Transform child = prefab.transform.GetChild(i);
                Box childBox = child.GetComponent<Box>();

                if (childBox == null)
                {
                    hasInvalidBox = true;
                    invalidReason += $"子对象 {child.name} 没有Box组件 ";
                    continue;
                }

                // 检查Box的子对象数量（空位数量）
                int boxChildCount = childBox.transform.childCount;

                // 检查Box是否有星星洞口
                bool hasStarHole = false;
                for (int j = 0; j < childBox.transform.childCount; j++)
                {
                    Transform holeChild = childBox.transform.GetChild(j);
                    if (holeChild.GetComponent<StarHole>() != null)
                    {
                        hasStarHole = true;
                        break;
                    }
                }

                // 判断Box是否不合格：不是3个空位 或 有星星洞口
                bool isBoxInvalid = (boxChildCount != 3) || hasStarHole;

                if (isBoxInvalid)
                {
                    hasInvalidBox = true;
                    if (boxChildCount != 3) invalidReason += $"Box {child.name} 空位数量不是3个(实际:{boxChildCount}) ";
                    if (hasStarHole) invalidReason += $"Box {child.name} 包含星星洞口 ";
                }
            }

            if (hasInvalidBox)
            {
                invalidCount++;
                invalidPrefabs.Add(prefabPath);
                Debug.Log($"不合格的关卡盒子: {Path.GetFileName(prefabPath)} - {invalidReason}");
            }
        }

        Debug.Log($"检测完成! 总共 {totalCount} 个关卡盒子，不合格的有 {invalidCount} 个");

        if (invalidPrefabs.Count > 0)
        {
            string message = $"发现 {invalidPrefabs.Count} 个不合格的关卡盒子，是否删除？\n\n不合格的关卡盒子:\n";
            foreach (string prefab in invalidPrefabs)
            {
                message += Path.GetFileName(prefab) + "\n";
            }

            bool shouldDelete = EditorUtility.DisplayDialog("删除不合格关卡盒子", message, "删除", "取消");

            if (shouldDelete)
            {
                int deletedCount = 0;
                foreach (string prefabPath in invalidPrefabs)
                {
                    try
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                        deletedCount++;
                        Debug.Log($"已删除: {Path.GetFileName(prefabPath)}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"删除失败 {prefabPath}: {e.Message}");
                    }
                }

                AssetDatabase.Refresh();
                Debug.Log($"删除完成! 成功删除 {deletedCount} 个不合格的关卡盒子");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("检测结果", "所有关卡盒子都符合要求！", "确定");
        }
    }

    [MenuItem("Tools/检测不合格的关卡")]
    public static void CheckInvalidLevels()
    {
        string levelsPath = "Assets/AA/Levels";
        string[] prefabFiles = Directory.GetFiles(levelsPath, "*.prefab");

        List<string> invalidPrefabs = new List<string>();
        int totalCount = 0;
        int invalidCount = 0;

        Debug.Log("开始检测关卡...");

        foreach (string prefabPath in prefabFiles)
        {
            totalCount++;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogWarning($"无法加载预制体: {prefabPath}");
                continue;
            }

            // 检查是否有Level组件
            Level levelComponent = prefab.GetComponent<Level>();
            if (levelComponent == null)
            {
                Debug.LogWarning($"预制体没有Level组件: {prefabPath}");
                continue;
            }

            // 初始化关卡以获取属性值
            levelComponent.InitLayerList();

            // 检查关卡属性
            bool isInvalid = false;
            string invalidReason = "";

            if (levelComponent.HasIceCovered)
            {
                isInvalid = true;
                invalidReason += "包含冰覆盖 ";
            }

            if (levelComponent.HasDoor)
            {
                isInvalid = true;
                invalidReason += "包含门 ";
            }

            if (levelComponent.HasBoom)
            {
                isInvalid = true;
                invalidReason += "包含爆炸 ";
            }

            if (levelComponent.HasChain)
            {
                isInvalid = true;
                invalidReason += "包含链条 ";
            }

            if (levelComponent.HasKey)
            {
                isInvalid = true;
                invalidReason += "包含钥匙 ";
            }

            if (levelComponent.HasLock)
            {
                isInvalid = true;
                invalidReason += "包含锁 ";
            }

            if (isInvalid)
            {
                invalidCount++;
                invalidPrefabs.Add(prefabPath);
                Debug.Log($"不合格的关卡: {Path.GetFileName(prefabPath)} - {invalidReason}");
            }
        }

        Debug.Log($"检测完成! 总共 {totalCount} 个关卡，不合格的有 {invalidCount} 个");

        if (invalidPrefabs.Count > 0)
        {
            string message = $"发现 {invalidPrefabs.Count} 个不合格的关卡:\n\n";
            foreach (string prefab in invalidPrefabs)
            {
                message += Path.GetFileName(prefab) + "\n";
            }

            EditorUtility.DisplayDialog("检测结果", message, "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("检测结果", "所有关卡都符合要求！", "确定");
        }
    }

    [MenuItem("Tools/检测并删除不合格的关卡")]
    public static void CheckAndDeleteInvalidLevels()
    {
        string levelsPath = "Assets/AA/Levels";
        string[] prefabFiles = Directory.GetFiles(levelsPath, "*.prefab");

        List<string> invalidPrefabs = new List<string>();
        int totalCount = 0;
        int invalidCount = 0;

        Debug.Log("开始检测关卡...");

        foreach (string prefabPath in prefabFiles)
        {
            totalCount++;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogWarning($"无法加载预制体: {prefabPath}");
                continue;
            }

            // 检查是否有Level组件
            Level levelComponent = prefab.GetComponent<Level>();
            if (levelComponent == null)
            {
                Debug.LogWarning($"预制体没有Level组件: {prefabPath}");
                continue;
            }

            // 初始化关卡以获取属性值
            levelComponent.InitLayerList();

            // 检查关卡属性
            bool isInvalid = false;
            string invalidReason = "";

            if (levelComponent.HasIceCovered)
            {
                isInvalid = true;
                invalidReason += "包含冰覆盖 ";
            }

            if (levelComponent.HasDoor)
            {
                isInvalid = true;
                invalidReason += "包含门 ";
            }

            if (levelComponent.HasBoom)
            {
                isInvalid = true;
                invalidReason += "包含爆炸 ";
            }

            if (levelComponent.HasChain)
            {
                isInvalid = true;
                invalidReason += "包含链条 ";
            }

            if (levelComponent.HasKey)
            {
                isInvalid = true;
                invalidReason += "包含钥匙 ";
            }

            if (levelComponent.HasLock)
            {
                isInvalid = true;
                invalidReason += "包含锁 ";
            }

            if (isInvalid)
            {
                invalidCount++;
                invalidPrefabs.Add(prefabPath);
                Debug.Log($"不合格的关卡: {Path.GetFileName(prefabPath)} - {invalidReason}");
            }
        }

        Debug.Log($"检测完成! 总共 {totalCount} 个关卡，不合格的有 {invalidCount} 个");

        if (invalidPrefabs.Count > 0)
        {
            string message = $"发现 {invalidPrefabs.Count} 个不合格的关卡，是否删除？\n\n不合格的关卡:\n";
            foreach (string prefab in invalidPrefabs)
            {
                message += Path.GetFileName(prefab) + "\n";
            }

            bool shouldDelete = EditorUtility.DisplayDialog("删除不合格关卡", message, "删除", "取消");

            if (shouldDelete)
            {
                int deletedCount = 0;
                foreach (string prefabPath in invalidPrefabs)
                {
                    try
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                        deletedCount++;
                        Debug.Log($"已删除: {Path.GetFileName(prefabPath)}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"删除失败 {prefabPath}: {e.Message}");
                    }
                }

                AssetDatabase.Refresh();
                Debug.Log($"删除完成! 成功删除 {deletedCount} 个不合格的关卡");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("检测结果", "所有关卡都符合要求！", "确定");
        }
    }

    [MenuItem("Tools/检测关卡和箱子对应关系")]
    public static void CheckLevelBoxCorrespondence()
    {
        string levelsPath = "Assets/AA/Levels";
        string boxLevelsPath = "Assets/AA/BoxLevel";

        string[] levelFiles = Directory.GetFiles(levelsPath, "*.prefab");
        string[] boxLevelFiles = Directory.GetFiles(boxLevelsPath, "*.prefab");

        // 提取关卡编号和箱子编号
        HashSet<int> levelNumbers = new HashSet<int>();
        HashSet<int> boxLevelNumbers = new HashSet<int>();

        foreach (string levelPath in levelFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(levelPath);
            if (fileName.StartsWith("Level "))
            {
                string numberStr = fileName.Substring("Level ".Length);
                if (int.TryParse(numberStr, out int levelNumber))
                {
                    levelNumbers.Add(levelNumber);
                }
            }
        }

        foreach (string boxLevelFilePath in boxLevelFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(boxLevelFilePath);
            if (fileName.StartsWith("BoxLevel_"))
            {
                string numberStr = fileName.Substring("BoxLevel_".Length);
                if (int.TryParse(numberStr, out int boxLevelNumber))
                {
                    boxLevelNumbers.Add(boxLevelNumber);
                }
            }
        }

        // 检查缺失的对应关系
        List<int> missingBoxLevels = new List<int>();
        List<int> missingLevels = new List<int>();

        foreach (int levelNumber in levelNumbers)
        {
            if (!boxLevelNumbers.Contains(levelNumber))
            {
                missingBoxLevels.Add(levelNumber);
            }
        }

        foreach (int boxLevelNumber in boxLevelNumbers)
        {
            if (!levelNumbers.Contains(boxLevelNumber))
            {
                missingLevels.Add(boxLevelNumber);
            }
        }

        // 输出结果
        string message = "";
        bool hasIssues = false;

        if (missingBoxLevels.Count > 0)
        {
            hasIssues = true;
            message += $"缺少箱子的关卡: {string.Join(", ", missingBoxLevels)}\n";
        }

        if (missingLevels.Count > 0)
        {
            hasIssues = true;
            message += $"缺少关卡的箱子: {string.Join(", ", missingLevels)}\n";
        }

        if (!hasIssues)
        {
            message = "所有关卡和箱子都有对应的配对！";
        }

        Debug.Log($"关卡总数: {levelNumbers.Count}, 箱子总数: {boxLevelNumbers.Count}");
        Debug.Log($"缺少箱子的关卡: {string.Join(", ", missingBoxLevels)}");
        Debug.Log($"缺少关卡的箱子: {string.Join(", ", missingBoxLevels)}");

        EditorUtility.DisplayDialog("关卡箱子对应关系检测", message, "确定");
    }

    [MenuItem("Tools/删除不配对的关卡和箱子")]
    public static void DeleteUnpairedLevelsAndBoxes()
    {
        string levelsPath = "Assets/AA/Levels";
        string boxLevelsPath = "Assets/AA/BoxLevel";

        string[] levelFiles = Directory.GetFiles(levelsPath, "*.prefab");
        string[] boxLevelFiles = Directory.GetFiles(boxLevelsPath, "*.prefab");

        // 提取关卡编号和箱子编号
        Dictionary<int, string> levelNumbers = new Dictionary<int, string>();
        Dictionary<int, string> boxLevelNumbers = new Dictionary<int, string>();

        foreach (string levelPath in levelFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(levelPath);
            if (fileName.StartsWith("Level "))
            {
                string numberStr = fileName.Substring("Level ".Length);
                if (int.TryParse(numberStr, out int levelNumber))
                {
                    levelNumbers[levelNumber] = levelPath;
                }
            }
        }

        foreach (string boxLevelFilePath in boxLevelFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(boxLevelFilePath);
            if (fileName.StartsWith("BoxLevel_"))
            {
                string numberStr = fileName.Substring("BoxLevel_".Length);
                if (int.TryParse(numberStr, out int boxLevelNumber))
                {
                    boxLevelNumbers[boxLevelNumber] = boxLevelFilePath;
                }
            }
        }

        // 找出不配对的资源
        List<string> toDeleteLevels = new List<string>();
        List<string> toDeleteBoxLevels = new List<string>();

        foreach (int levelNumber in levelNumbers.Keys)
        {
            if (!boxLevelNumbers.ContainsKey(levelNumber))
            {
                toDeleteLevels.Add(levelNumbers[levelNumber]);
            }
        }

        foreach (int boxLevelNumber in boxLevelNumbers.Keys)
        {
            if (!levelNumbers.ContainsKey(boxLevelNumber))
            {
                toDeleteBoxLevels.Add(boxLevelNumbers[boxLevelNumber]);
            }
        }

        // 显示删除确认对话框
        string message = "";
        if (toDeleteLevels.Count > 0)
        {
            message += $"要删除的关卡 ({toDeleteLevels.Count} 个):\n";
            foreach (string levelPath in toDeleteLevels)
            {
                message += Path.GetFileName(levelPath) + "\n";
            }
            message += "\n";
        }

        if (toDeleteBoxLevels.Count > 0)
        {
            message += $"要删除的箱子 ({toDeleteBoxLevels.Count} 个):\n";
            foreach (string boxLevelFilePath in toDeleteBoxLevels)
            {
                message += Path.GetFileName(boxLevelFilePath) + "\n";
            }
        }

        if (toDeleteLevels.Count == 0 && toDeleteBoxLevels.Count == 0)
        {
            EditorUtility.DisplayDialog("检测结果", "所有关卡和箱子都有对应的配对！", "确定");
            return;
        }

        bool shouldDelete = EditorUtility.DisplayDialog("删除不配对的资源",
            $"发现 {toDeleteLevels.Count} 个不配对的关卡和 {toDeleteBoxLevels.Count} 个不配对的箱子，是否删除？\n\n{message}",
            "删除", "取消");

        if (shouldDelete)
        {
            int deletedLevels = 0;
            int deletedBoxLevels = 0;

            // 删除不配对的关卡
            foreach (string levelPath in toDeleteLevels)
            {
                try
                {
                    AssetDatabase.DeleteAsset(levelPath);
                    deletedLevels++;
                    Debug.Log($"已删除关卡: {Path.GetFileName(levelPath)}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"删除关卡失败 {levelPath}: {e.Message}");
                }
            }

            // 删除不配对的箱子
            foreach (string boxLevelFilePath in toDeleteBoxLevels)
            {
                try
                {
                    AssetDatabase.DeleteAsset(boxLevelFilePath);
                    deletedBoxLevels++;
                    Debug.Log($"已删除箱子: {Path.GetFileName(boxLevelFilePath)}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"删除箱子失败 {boxLevelFilePath}: {e.Message}");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"删除完成! 成功删除 {deletedLevels} 个关卡和 {deletedBoxLevels} 个箱子");

            EditorUtility.DisplayDialog("删除完成",
    $"成功删除 {deletedLevels} 个关卡和 {deletedBoxLevels} 个箱子", "确定");
        }
    }

    [MenuItem("Tools/检测Addressable可加载关卡")]
    public static void CheckAddressableLoadableLevels()
    {
        Debug.Log("开始检测Addressable可加载关卡...");

        // 检测关卡
        List<int> loadableLevels = new List<int>();
        List<int> loadableBoxLevels = new List<int>();

        // 从1开始检测，假设最多1000关
        for (int i = 1; i <= 1000; i++)
        {
            try
            {
                // 检测关卡
                var levelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"Level {i}");
                levelHandle.WaitForCompletion();
                if (levelHandle.Result != null && levelHandle.Result.Count > 0)
                {
                    loadableLevels.Add(i);
                }
                levelHandle.Release();

                // 检测箱子
                var boxLevelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"BoxLevel_{i}");
                boxLevelHandle.WaitForCompletion();
                if (boxLevelHandle.Result != null && boxLevelHandle.Result.Count > 0)
                {
                    loadableBoxLevels.Add(i);
                }
                boxLevelHandle.Release();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"检测关卡 {i} 时出错: {e.Message}");
            }
        }

        // 找出配对的关卡
        List<int> pairedLevels = new List<int>();
        List<int> unpairedLevels = new List<int>();
        List<int> unpairedBoxLevels = new List<int>();

        foreach (int levelNum in loadableLevels)
        {
            if (loadableBoxLevels.Contains(levelNum))
            {
                pairedLevels.Add(levelNum);
            }
            else
            {
                unpairedLevels.Add(levelNum);
            }
        }

        foreach (int boxLevelNum in loadableBoxLevels)
        {
            if (!loadableLevels.Contains(boxLevelNum))
            {
                unpairedBoxLevels.Add(boxLevelNum);
            }
        }

        // 输出结果
        string message = "";
        message += $"可加载的关卡总数: {loadableLevels.Count}\n";
        message += $"可加载的箱子总数: {loadableBoxLevels.Count}\n";
        message += $"配对的关卡数: {pairedLevels.Count}\n\n";

        if (pairedLevels.Count > 0)
        {
            message += $"配对的关卡: {string.Join(", ", pairedLevels)}\n\n";
        }

        if (unpairedLevels.Count > 0)
        {
            message += $"缺少箱子的关卡: {string.Join(", ", unpairedLevels)}\n";
        }

        if (unpairedBoxLevels.Count > 0)
        {
            message += $"缺少关卡的箱子: {string.Join(", ", unpairedBoxLevels)}\n";
        }

        Debug.Log($"可加载的关卡: {string.Join(", ", loadableLevels)}");
        Debug.Log($"可加载的箱子: {string.Join(", ", loadableBoxLevels)}");
        Debug.Log($"配对的关卡: {string.Join(", ", pairedLevels)}");

        EditorUtility.DisplayDialog("Addressable可加载关卡检测", message, "确定");
    }

    [MenuItem("Tools/生成可加载关卡列表")]
    public static void GenerateLoadableLevelsList()
    {
        Debug.Log("开始生成可加载关卡列表...");

        List<int> loadableLevels = new List<int>();
        List<int> loadableBoxLevels = new List<int>();

        // 从1开始检测，假设最多1000关
        for (int i = 1; i <= 1000; i++)
        {
            try
            {
                // 检测关卡
                var levelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"Level {i}");
                levelHandle.WaitForCompletion();
                if (levelHandle.Result != null && levelHandle.Result.Count > 0)
                {
                    loadableLevels.Add(i);
                }
                levelHandle.Release();

                // 检测箱子
                var boxLevelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"BoxLevel_{i}");
                boxLevelHandle.WaitForCompletion();
                if (boxLevelHandle.Result != null && boxLevelHandle.Result.Count > 0)
                {
                    loadableBoxLevels.Add(i);
                }
                boxLevelHandle.Release();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"检测关卡 {i} 时出错: {e.Message}");
            }
        }

        // 找出配对的关卡
        List<int> pairedLevels = new List<int>();
        foreach (int levelNum in loadableLevels)
        {
            if (loadableBoxLevels.Contains(levelNum))
            {
                pairedLevels.Add(levelNum);
            }
        }

        // 生成代码
        string code = "// 自动生成的可加载关卡列表\n";
        code += "public static class LoadableLevels\n";
        code += "{\n";
        code += "    // 所有可加载的关卡编号\n";
        code += $"    public static readonly int[] AllLevels = new int[] {{ {string.Join(", ", pairedLevels)} }};\n\n";
        code += "    // 最大关卡编号\n";
        code += $"    public static readonly int MaxLevel = {(pairedLevels.Count > 0 ? pairedLevels[pairedLevels.Count - 1] : 0)};\n\n";
        code += "    // 关卡总数\n";
        code += $"    public static readonly int TotalLevels = {pairedLevels.Count};\n\n";
        code += "    // 检查关卡是否可加载\n";
        code += "    public static bool IsLevelLoadable(int levelNum)\n";
        code += "    {\n";
        code += "        return System.Array.IndexOf(AllLevels, levelNum) >= 0;\n";
        code += "    }\n\n";
        code += "    // 根据原始关卡编号获取实际关卡编号\n";
        code += "    public static int GetActualLevelNumber(int originalLevelNum)\n";
        code += "    {\n";
        code += "        if (originalLevelNum <= 0 || originalLevelNum > AllLevels.Length)\n";
        code += "        {\n";
        code += "            Debug.LogWarning($\"原始关卡编号 {originalLevelNum} 超出范围，返回第一个关卡\");\n";
        code += "            return AllLevels.Length > 0 ? AllLevels[0] : 1;\n";
        code += "        }\n";
        code += "        return AllLevels[originalLevelNum - 1];\n";
        code += "    }\n\n";
        code += "    // 根据实际关卡编号获取原始关卡编号\n";
        code += "    public static int GetOriginalLevelNumber(int actualLevelNum)\n";
        code += "    {\n";
        code += "        int index = System.Array.IndexOf(AllLevels, actualLevelNum);\n";
        code += "        if (index >= 0)\n";
        code += "        {\n";
        code += "            return index + 1;\n";
        code += "        }\n";
        code += "        Debug.LogWarning($\"实际关卡编号 {actualLevelNum} 不存在，返回1\");\n";
        code += "        return 1;\n";
        code += "    }\n";
        code += "}\n";

        // 保存到文件
        string filePath = "Assets/Scripts/Gen/LoadableLevels.cs";
        System.IO.File.WriteAllText(filePath, code);

        AssetDatabase.Refresh();

        Debug.Log($"已生成可加载关卡列表，共 {pairedLevels.Count} 个配对关卡");
        Debug.Log($"文件保存到: {filePath}");
        Debug.Log($"可加载关卡: {string.Join(", ", pairedLevels)}");

        EditorUtility.DisplayDialog("生成完成",
            $"已生成可加载关卡列表\n共 {pairedLevels.Count} 个配对关卡\n文件保存到: {filePath}", "确定");
    }



    [MenuItem("Tools/只根据关卡生成关卡列表")]
    public static void GenerateLoadableLeve()
    {
        Debug.Log("开始生成可加载关卡列表...");

        List<int> loadableLevels = new List<int>();

        // 从1开始检测，假设最多1000关
        for (int i = 1; i <= 1000; i++)
        {
            try
            {
                // 检测关卡
                var levelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"Level {i}");
                levelHandle.WaitForCompletion();
                if (levelHandle.Result != null && levelHandle.Result.Count > 0)
                {
                    loadableLevels.Add(i);
                }
                levelHandle.Release();


            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"检测关卡 {i} 时出错: {e.Message}");
            }
        }

        // 找出配对的关卡
        List<int> pairedLevels = new List<int>();
        pairedLevels = loadableLevels;
        //foreach (int levelNum in loadableLevels)
        //{
        //    if (loadableBoxLevels.Contains(levelNum))
        //    {
        //        pairedLevels.Add(levelNum);
        //    }
        //}

        // 生成代码
        string code = "// 自动生成的可加载关卡列表\n";
        code += "public static class LoadableLevels\n";
        code += "{\n";
        code += "    // 所有可加载的关卡编号\n";
        code += $"    public static readonly int[] AllLevels = new int[] {{ {string.Join(", ", pairedLevels)} }};\n\n";
        code += "    // 最大关卡编号\n";
        code += $"    public static readonly int MaxLevel = {(pairedLevels.Count > 0 ? pairedLevels[pairedLevels.Count - 1] : 0)};\n\n";
        code += "    // 关卡总数\n";
        code += $"    public static readonly int TotalLevels = {pairedLevels.Count};\n\n";
        code += "    // 检查关卡是否可加载\n";
        code += "    public static bool IsLevelLoadable(int levelNum)\n";
        code += "    {\n";
        code += "        return System.Array.IndexOf(AllLevels, levelNum) >= 0;\n";
        code += "    }\n\n";
        code += "    // 根据原始关卡编号获取实际关卡编号\n";
        code += "    public static int GetActualLevelNumber(int originalLevelNum)\n";
        code += "    {\n";
        code += "        if (AllLevels.Length == 0)\n";
        code += "        {\n";
        code += "            UnityEngine.Debug.LogWarning(\"没有可用的关卡，返回1\");\n";
        code += "            return 1;\n";
        code += "        }\n";
        code += "        \n";
        code += "        if (originalLevelNum <= 0)\n";
        code += "        {\n";
        code += "            UnityEngine.Debug.LogWarning($\"原始关卡编号 {originalLevelNum} 小于等于0，返回第一个关卡\");\n";
        code += "            return AllLevels[0];\n";
        code += "        }\n";
        code += "        \n";
        code += "        // 如果超出范围，循环返回\n";
        code += "        int index = (originalLevelNum - 1) % AllLevels.Length;\n";
        code += "        return AllLevels[index];\n";
        code += "    }\n\n";
        code += "    // 根据实际关卡编号获取原始关卡编号\n";
        code += "    public static int GetOriginalLevelNumber(int actualLevelNum)\n";
        code += "    {\n";
        code += "        int index = System.Array.IndexOf(AllLevels, actualLevelNum);\n";
        code += "        if (index >= 0)\n";
        code += "        {\n";
        code += "            return index + 1;\n";
        code += "        }\n";
    //    code += "        Debug.LogWarning($\"实际关卡编号 {actualLevelNum} 不存在，返回1\");\n";
        code += "        return 1;\n";
        code += "    }\n";
        code += "}\n";

        // 保存到文件
        string filePath = "Assets/Scripts/Game/LoadableLevels.cs";
        System.IO.File.WriteAllText(filePath, code);

        AssetDatabase.Refresh();

        Debug.Log($"已生成可加载关卡列表，共 {pairedLevels.Count} 个配对关卡");
        Debug.Log($"文件保存到: {filePath}");
        Debug.Log($"可加载关卡: {string.Join(", ", pairedLevels)}");

        EditorUtility.DisplayDialog("生成完成",
            $"已生成可加载关卡列表\n共 {pairedLevels.Count} 个配对关卡\n文件保存到: {filePath}", "确定");
    }

    [MenuItem("Tools/检测可加载关卡对应的箱子儿子数量")]
    public static void CheckLoadableLevelBoxChildren()
    {
        Debug.Log("开始检测可加载关卡对应的箱子儿子数量...");

        List<int> loadableLevels = new List<int>();
        List<int> invalidBoxLevels = new List<int>();

        // 从1开始检测，假设最多1000关
        for (int i = 1; i <= 1000; i++)
        {
            try
            {
                // 检测关卡
                var levelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"Level {i}");
                levelHandle.WaitForCompletion();
                if (levelHandle.Result != null && levelHandle.Result.Count > 0)
                {
                    loadableLevels.Add(i);
                    
                    // 检测对应的箱子
                    var boxLevelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"BoxLevel_{i}");
                    boxLevelHandle.WaitForCompletion();
                    if (boxLevelHandle.Result != null && boxLevelHandle.Result.Count > 0)
                    {
                        // 加载箱子预制体
                        var boxPrefabHandle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>($"BoxLevel_{i}");
                        boxPrefabHandle.WaitForCompletion();
                        GameObject boxPrefab = boxPrefabHandle.Result;
                        
                        if (boxPrefab != null)
                        {
                            bool hasInvalidBox = false;
                            string invalidReason = "";
                            
                            // 检查每个子对象（Box组件）是否有3个子对象
                            for (int j = 0; j < boxPrefab.transform.childCount; j++)
                            {
                                Transform child = boxPrefab.transform.GetChild(j);
                                Box childBox = child.GetComponent<Box>();
                                
                                if (childBox == null)
                                {
                                    hasInvalidBox = true;
                                    invalidReason += $"子对象 {child.name} 没有Box组件 ";
                                    continue;
                                }
                                
                                int boxChildCount = childBox.transform.childCount;
                                if (boxChildCount != 3)
                                {
                                    hasInvalidBox = true;
                                    invalidReason += $"Box {child.name} 有 {boxChildCount} 个子对象，应该是3个 ";
                                }
                            }
                            
                            if (hasInvalidBox)
                            {
                                invalidBoxLevels.Add(i);
                                Debug.LogWarning($"关卡 {i} 对应的箱子: {invalidReason}");
                                Debug.LogWarning($"不合格的盒子关卡: BoxLevel_{i}");
                            }
                            else
                            {
                                Debug.Log($"关卡 {i} 对应的箱子所有Box都有3个子对象，符合要求");
                            }
                        }
                        else
                        {
                            Debug.LogError($"无法加载关卡 {i} 对应的箱子预制体");
                            invalidBoxLevels.Add(i);
                        }
                        
                        boxPrefabHandle.Release();
                    }
                    else
                    {
                        Debug.LogWarning($"关卡 {i} 没有对应的箱子");
                        invalidBoxLevels.Add(i);
                    }
                    
                    boxLevelHandle.Release();
                }
                levelHandle.Release();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"检测关卡 {i} 时出错: {e.Message}");
            }
        }

        // 生成报告
        string message = $"检测完成!\n\n";
        message += $"可加载关卡总数: {loadableLevels.Count}\n";
        message += $"箱子儿子数量不符合要求的关卡数: {invalidBoxLevels.Count}\n\n";

        if (invalidBoxLevels.Count > 0)
        {
            message += "不符合要求的关卡:\n";
            foreach (int levelNum in invalidBoxLevels)
            {
                message += $"关卡 {levelNum}: Box子对象数量不符合要求\n";
            }
            
            // 详细打印每个不合格的关卡信息
            Debug.LogWarning("=== 不合格的盒子关卡详细信息 ===");
            foreach (int levelNum in invalidBoxLevels)
            {
                Debug.LogWarning($"不合格关卡: BoxLevel_{levelNum}");
            }
            Debug.LogWarning("=== 详细信息结束 ===");
        }
        else
        {
            message += "所有可加载关卡的箱子都符合要求（有3个儿子）！";
        }

        Debug.Log($"检测完成! 可加载关卡: {string.Join(", ", loadableLevels)}");
        if (invalidBoxLevels.Count > 0)
        {
            Debug.LogError($"不符合要求的关卡: {string.Join(", ", invalidBoxLevels)}");
        }

        EditorUtility.DisplayDialog("箱子儿子数量检测", message, "确定");
    }

    [MenuItem("Tools/清理并重新生成LoadableLevels")]
    public static void CleanAndRegenerateLoadableLevels()
    {
        Debug.Log("开始清理并重新生成LoadableLevels...");

        List<int> loadableLevels = new List<int>();
        List<int> invalidBoxLevels = new List<int>();
        List<string> levelsToDelete = new List<string>();
        List<string> boxLevelsToDelete = new List<string>();

        // 从1开始检测，假设最多1000关
        for (int i = 1; i <= 1000; i++)
        {
            try
            {
                // 检测关卡
                var levelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"Level {i}");
                levelHandle.WaitForCompletion();
                if (levelHandle.Result != null && levelHandle.Result.Count > 0)
                {
                    // 检测对应的箱子
                    var boxLevelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"BoxLevel_{i}");
                    boxLevelHandle.WaitForCompletion();
                    if (boxLevelHandle.Result != null && boxLevelHandle.Result.Count > 0)
                    {
                        // 加载箱子预制体检查是否符合要求
                        var boxPrefabHandle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>($"BoxLevel_{i}");
                        boxPrefabHandle.WaitForCompletion();
                        GameObject boxPrefab = boxPrefabHandle.Result;
                        
                        if (boxPrefab != null)
                        {
                            bool hasInvalidBox = false;
                            
                            // 检查每个子对象（Box组件）是否有3个子对象
                            for (int j = 0; j < boxPrefab.transform.childCount; j++)
                            {
                                Transform child = boxPrefab.transform.GetChild(j);
                                Box childBox = child.GetComponent<Box>();
                                
                                if (childBox == null || childBox.transform.childCount != 3)
                                {
                                    hasInvalidBox = true;
                                    break;
                                }
                            }
                            
                            if (hasInvalidBox)
                            {
                                invalidBoxLevels.Add(i);
                                // 记录要删除的关卡和箱子
                                levelsToDelete.Add($"Level {i}");
                                boxLevelsToDelete.Add($"BoxLevel_{i}");
                                Debug.LogWarning($"关卡 {i} 的箱子不符合要求，将被删除");
                            }
                            else
                            {
                                loadableLevels.Add(i);
                                Debug.Log($"关卡 {i} 符合要求，保留");
                            }
                        }
                        else
                        {
                            // 无法加载箱子预制体，删除关卡和箱子
                            levelsToDelete.Add($"Level {i}");
                            boxLevelsToDelete.Add($"BoxLevel_{i}");
                            Debug.LogWarning($"关卡 {i} 无法加载箱子预制体，将被删除");
                        }
                        
                        boxPrefabHandle.Release();
                    }
                    else
                    {
                        // 没有对应的箱子，删除关卡
                        levelsToDelete.Add($"Level {i}");
                        Debug.LogWarning($"关卡 {i} 没有对应的箱子，将被删除");
                    }
                    
                    boxLevelHandle.Release();
                }
                levelHandle.Release();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"检测关卡 {i} 时出错: {e.Message}");
            }
        }

        // 显示删除确认对话框
        string deleteMessage = $"检测完成!\n\n";
        deleteMessage += $"符合要求的关卡数: {loadableLevels.Count}\n";
        deleteMessage += $"要删除的关卡数: {levelsToDelete.Count}\n";
        deleteMessage += $"要删除的箱子数: {boxLevelsToDelete.Count}\n\n";

        if (levelsToDelete.Count > 0)
        {
            deleteMessage += "要删除的关卡:\n";
            foreach (string level in levelsToDelete)
            {
                deleteMessage += level + "\n";
            }
            deleteMessage += "\n";
        }

        if (boxLevelsToDelete.Count > 0)
        {
            deleteMessage += "要删除的箱子:\n";
            foreach (string boxLevel in boxLevelsToDelete)
            {
                deleteMessage += boxLevel + "\n";
            }
        }

        bool shouldDelete = EditorUtility.DisplayDialog("清理确认", deleteMessage, "确认删除并生成", "取消");

        if (shouldDelete)
        {
            int deletedLevels = 0;
            int deletedBoxLevels = 0;

            // 删除不符合要求的关卡
            foreach (string levelName in levelsToDelete)
            {
                try
                {
                    // 从 "Level X" 提取数字
                    string levelNumber = levelName.Substring("Level ".Length);
                    string assetPath = $"Assets/AA/Levels/Level {levelNumber}.prefab";
                    
                    if (System.IO.File.Exists(assetPath))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                        deletedLevels++;
                        Debug.Log($"已删除关卡: {assetPath}");
                    }
                    else
                    {
                        Debug.LogWarning($"关卡文件不存在: {assetPath}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"删除关卡失败 {levelName}: {e.Message}");
                }
            }

            // 删除不符合要求的箱子
            foreach (string boxLevelName in boxLevelsToDelete)
            {
                try
                {
                    // 从 "BoxLevel_X" 提取数字
                    string boxNumber = boxLevelName.Substring("BoxLevel_".Length);
                    string assetPath = $"Assets/AA/BoxLevel/BoxLevel_{boxNumber}.prefab";
                    
                    if (System.IO.File.Exists(assetPath))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                        deletedBoxLevels++;
                        Debug.Log($"已删除箱子: {assetPath}");
                    }
                    else
                    {
                        Debug.LogWarning($"箱子文件不存在: {assetPath}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"删除箱子失败 {boxLevelName}: {e.Message}");
                }
            }

            AssetDatabase.Refresh();

            // 生成LoadableLevels脚本
            string code = "// 自动生成的可加载关卡列表\n";
            code += "public static class LoadableLevels\n";
            code += "{\n";
            code += "    // 所有可加载的关卡编号\n";
            code += $"    public static readonly int[] AllLevels = new int[] {{ {string.Join(", ", loadableLevels)} }};\n\n";
            code += "    // 最大关卡编号\n";
            code += $"    public static readonly int MaxLevel = {(loadableLevels.Count > 0 ? loadableLevels[loadableLevels.Count - 1] : 0)};\n\n";
            code += "    // 关卡总数\n";
            code += $"    public static readonly int TotalLevels = {loadableLevels.Count};\n\n";
            code += "    // 检查关卡是否可加载\n";
            code += "    public static bool IsLevelLoadable(int levelNum)\n";
            code += "    {\n";
            code += "        return System.Array.IndexOf(AllLevels, levelNum) >= 0;\n";
            code += "    }\n\n";
            code += "    // 根据原始关卡编号获取实际关卡编号\n";
            code += "    public static int GetActualLevelNumber(int originalLevelNum)\n";
            code += "    {\n";
            code += "        if (AllLevels.Length == 0)\n";
            code += "        {\n";
            code += "            UnityEngine.Debug.LogWarning(\"没有可用的关卡，返回1\");\n";
            code += "            return 1;\n";
            code += "        }\n";
            code += "        \n";
            code += "        if (originalLevelNum <= 0)\n";
            code += "        {\n";
            code += "            UnityEngine.Debug.LogWarning($\"原始关卡编号 {originalLevelNum} 小于等于0，返回第一个关卡\");\n";
            code += "            return AllLevels[0];\n";
            code += "        }\n";
            code += "        \n";
            code += "        // 如果超出范围，循环返回\n";
            code += "        int index = (originalLevelNum - 1) % AllLevels.Length;\n";
            code += "        return AllLevels[index];\n";
            code += "    }\n\n";
            code += "    // 根据实际关卡编号获取原始关卡编号\n";
            code += "    public static int GetOriginalLevelNumber(int actualLevelNum)\n";
            code += "    {\n";
            code += "        int index = System.Array.IndexOf(AllLevels, actualLevelNum);\n";
            code += "        if (index >= 0)\n";
            code += "        {\n";
            code += "            return index + 1;\n";
            code += "        }\n";
            code += "        return 1;\n";
            code += "    }\n";
            code += "}\n";

            // 保存到文件
            string filePath = "Assets/Scripts/Game/LoadableLevels.cs";
            System.IO.File.WriteAllText(filePath, code);

            AssetDatabase.Refresh();

            string resultMessage = $"清理完成!\n\n";
            resultMessage += $"成功删除 {deletedLevels} 个关卡和 {deletedBoxLevels} 个箱子\n";
            resultMessage += $"保留 {loadableLevels.Count} 个符合要求的关卡\n";
            resultMessage += $"已重新生成LoadableLevels.cs文件\n";
            resultMessage += $"文件路径: {filePath}\n\n";
            resultMessage += $"符合要求的关卡: {string.Join(", ", loadableLevels)}";

            Debug.Log($"清理完成! 删除 {deletedLevels} 个关卡和 {deletedBoxLevels} 个箱子");
            Debug.Log($"保留 {loadableLevels.Count} 个符合要求的关卡: {string.Join(", ", loadableLevels)}");

            EditorUtility.DisplayDialog("清理完成", resultMessage, "确定");
        }
    }

    [MenuItem("Tools/删除不配对的箱子")]
    public static void DeleteUnpairedBoxLevels()
    {
        Debug.Log("开始检测并删除不配对的箱子...");

        List<int> loadableLevels = new List<int>();
        List<int> loadableBoxLevels = new List<int>();
        List<string> boxLevelsToDelete = new List<string>();

        // 从1开始检测，假设最多1000关
        for (int i = 1; i <= 1000; i++)
        {
            try
            {
                // 检测关卡
                var levelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"Level {i}");
                levelHandle.WaitForCompletion();
                if (levelHandle.Result != null && levelHandle.Result.Count > 0)
                {
                    loadableLevels.Add(i);
                }
                levelHandle.Release();

                // 检测箱子
                var boxLevelHandle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync($"BoxLevel_{i}");
                boxLevelHandle.WaitForCompletion();
                if (boxLevelHandle.Result != null && boxLevelHandle.Result.Count > 0)
                {
                    loadableBoxLevels.Add(i);
                }
                boxLevelHandle.Release();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"检测关卡 {i} 时出错: {e.Message}");
            }
        }

        // 找出不配对的箱子
        foreach (int boxLevelNum in loadableBoxLevels)
        {
            if (!loadableLevels.Contains(boxLevelNum))
            {
                boxLevelsToDelete.Add($"BoxLevel_{boxLevelNum}");
                Debug.LogWarning($"箱子 BoxLevel_{boxLevelNum} 没有对应的关卡，将被删除");
            }
        }

        // 显示删除确认对话框
        string deleteMessage = $"检测完成!\n\n";
        deleteMessage += $"可加载的关卡数: {loadableLevels.Count}\n";
        deleteMessage += $"可加载的箱子数: {loadableBoxLevels.Count}\n";
        deleteMessage += $"要删除的不配对箱子数: {boxLevelsToDelete.Count}\n\n";

        if (boxLevelsToDelete.Count > 0)
        {
            deleteMessage += "要删除的箱子:\n";
            foreach (string boxLevel in boxLevelsToDelete)
            {
                deleteMessage += boxLevel + "\n";
            }
        }
        else
        {
            deleteMessage += "所有箱子都有对应的关卡，无需删除！";
        }

        bool shouldDelete = EditorUtility.DisplayDialog("删除不配对的箱子", deleteMessage, "确认删除", "取消");

        if (shouldDelete && boxLevelsToDelete.Count > 0)
        {
            int deletedBoxLevels = 0;

            // 删除不配对的箱子
            foreach (string boxLevelName in boxLevelsToDelete)
            {
                try
                {
                    // 从 "BoxLevel_X" 提取数字
                    string boxNumber = boxLevelName.Substring("BoxLevel_".Length);
                    string assetPath = $"Assets/AA/BoxLevel/BoxLevel_{boxNumber}.prefab";
                    
                    if (System.IO.File.Exists(assetPath))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                        deletedBoxLevels++;
                        Debug.Log($"已删除箱子: {assetPath}");
                    }
                    else
                    {
                        Debug.LogWarning($"箱子文件不存在: {assetPath}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"删除箱子失败 {boxLevelName}: {e.Message}");
                }
            }

            AssetDatabase.Refresh();

            string resultMessage = $"删除完成!\n\n";
            resultMessage += $"成功删除 {deletedBoxLevels} 个不配对的箱子\n";
            resultMessage += $"保留 {loadableLevels.Count} 个有对应关卡的箱子";

            Debug.Log($"删除完成! 成功删除 {deletedBoxLevels} 个不配对的箱子");
            Debug.Log($"保留 {loadableLevels.Count} 个有对应关卡的箱子");

            EditorUtility.DisplayDialog("删除完成", resultMessage, "确定");
        }
    }

    [MenuItem("Tools/收集箱子颜色数据")]
    public static void CollectBoxColorData()
    {
        string boxLevelPath = "Assets/AA/BoxLevel";
        string[] prefabFiles = Directory.GetFiles(boxLevelPath, "*.prefab");
        
        // 按照文件名中的数字排序，确保按照文件顺序处理
        Array.Sort(prefabFiles, (a, b) => {
            string nameA = Path.GetFileNameWithoutExtension(a);
            string nameB = Path.GetFileNameWithoutExtension(b);
            
            // 提取数字部分进行排序
            int numberA = ExtractNumberFromFileName(nameA);
            int numberB = ExtractNumberFromFileName(nameB);
            
            return numberA.CompareTo(numberB);
        });

        // 输出排序后的文件名顺序（用于验证）
        Debug.Log("=== 排序后的文件顺序 ===");
        for (int i = 0; i < Math.Min(prefabFiles.Length, 10); i++) // 只显示前10个
        {
            string fileName = Path.GetFileNameWithoutExtension(prefabFiles[i]);
            int number = ExtractNumberFromFileName(fileName);
            Debug.Log($"文件 {i + 1}: {fileName} (数字: {number})");
        }
        Debug.Log("=== 排序验证完成 ===");

        // 清空之前的数据
        BoxColorData.ClearData();

        Debug.Log("开始收集箱子颜色数据...");
        int totalCount = 0;
        int successCount = 0;

        foreach (string prefabPath in prefabFiles)
        {
            totalCount++;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogWarning($"无法加载预制体: {prefabPath}");
                continue;
            }

            string boxName = Path.GetFileNameWithoutExtension(prefabPath);
            List<ScrewColor> boxColors = new List<ScrewColor>();

            // 遍历所有子对象，获取Box组件
            for (int i = 0; i < prefab.transform.childCount; i++)
            {
                Transform child = prefab.transform.GetChild(i);
                Box childBox = child.GetComponent<Box>();

                if (childBox != null)
                {
                    // 获取Box的颜色
                    ScrewColor boxColor = childBox.BoxColor;
                    boxColors.Add(boxColor);
                }
            }

            // 如果找到了颜色数据，保存到静态类中
            if (boxColors.Count > 0)
            {
                BoxColorData.AddBoxColors(boxName, boxColors);
                successCount++;
                
                // 打印当前箱子的颜色信息
                string colorString = "";
                for (int i = 0; i < boxColors.Count; i++)
                {
                    colorString += boxColors[i].ToString();
                    if (i < boxColors.Count - 1)
                        colorString += ", ";
                }
                Debug.Log($"箱子: {boxName}, 颜色: [{colorString}]");
            }
            else
            {
                Debug.LogWarning($"箱子 {boxName} 没有找到Box组件或颜色数据");
            }
        }

        Debug.Log($"收集完成! 总共处理 {totalCount} 个箱子，成功收集 {successCount} 个箱子的颜色数据");
        
        // 打印所有收集到的数据
        BoxColorData.PrintAllData();
        
        // 生成只读数据脚本
        GenerateReadOnlyBoxColorData();
        
        // 显示完成对话框
        string message = $"收集完成!\n\n";
        message += $"总共处理: {totalCount} 个箱子\n";
        message += $"成功收集: {successCount} 个箱子的颜色数据\n";
        message += $"已生成只读数据脚本\n";
        message += $"详细数据请查看控制台输出";
        
        EditorUtility.DisplayDialog("收集箱子颜色数据", message, "确定");
    }

    /// <summary>
    /// 从文件名中提取数字
    /// </summary>
    /// <param name="fileName">文件名（如 "BoxLevel_1"）</param>
    /// <returns>提取的数字，如果提取失败返回0</returns>
    private static int ExtractNumberFromFileName(string fileName)
    {
        try
        {
            // 查找最后一个下划线的位置
            int lastUnderscoreIndex = fileName.LastIndexOf('_');
            if (lastUnderscoreIndex >= 0 && lastUnderscoreIndex < fileName.Length - 1)
            {
                string numberPart = fileName.Substring(lastUnderscoreIndex + 1);
                if (int.TryParse(numberPart, out int number))
                {
                    return number;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"提取文件名数字失败: {fileName}, 错误: {e.Message}");
        }
        return 0;
    }

    /// <summary>
    /// 生成只读的箱子颜色数据脚本
    /// </summary>
    private static void GenerateReadOnlyBoxColorData()
    {
        // 收集所有颜色
        List<ScrewColor> allColors = new List<ScrewColor>();
        Dictionary<string, List<ScrewColor>> readOnlyBoxColors = new Dictionary<string, List<ScrewColor>>();
        
        // 按照文件名中的数字排序处理数据
        var sortedBoxColors = BoxColorData.BoxColors.OrderBy(kvp => ExtractNumberFromFileName(kvp.Key)).ToList();
        
        foreach (var kvp in sortedBoxColors)
        {
            readOnlyBoxColors[kvp.Key] = kvp.Value;
            allColors.AddRange(kvp.Value);
        }
        
        // 生成代码
        string code = "using System.Collections.Generic;\n";
        code += "using UnityEngine;\n\n";
        code += "/// <summary>\n";
        code += "/// 自动生成的只读箱子颜色数据\n";
        code += "/// </summary>\n";
        code += "public static class ReadOnlyBoxColorData\n";
        code += "{\n";
        
        // 生成只读字典
        code += "    /// <summary>\n";
        code += "    /// 所有箱子的颜色数据（只读字典）\n";
        code += "    /// </summary>\n";
        code += "    public static readonly Dictionary<string, List<ScrewColor>> BoxColors = new Dictionary<string, List<ScrewColor>>()\n";
        code += "    {\n";
        
        foreach (var kvp in readOnlyBoxColors)
        {
            code += $"        {{\"{kvp.Key}\", new List<ScrewColor>() {{ ";
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                code += $"ScrewColor.{kvp.Value[i]}";
                if (i < kvp.Value.Count - 1)
                    code += ", ";
            }
            code += " }},\n";
        }
        
        code += "    };\n\n";
        
        // 生成所有颜色列表
        code += "    /// <summary>\n";
        code += "    /// 所有颜色的集合（只读列表）\n";
        code += "    /// </summary>\n";
        code += "    public static readonly List<ScrewColor> AllColors = new List<ScrewColor>()\n";
        code += "    {\n";
        code += "        ";
        
        for (int i = 0; i < allColors.Count; i++)
        {
            code += $"ScrewColor.{allColors[i]}";
            if (i < allColors.Count - 1)
                code += ", ";
            if ((i + 1) % 10 == 0) // 每10个换行
                code += "\n        ";
        }
        
        code += "\n    };\n\n";
        
        // 添加一些实用方法
        code += "    /// <summary>\n";
        code += "    /// 获取指定箱子的颜色数据\n";
        code += "    /// </summary>\n";
        code += "    public static List<ScrewColor> GetBoxColors(string boxName)\n";
        code += "    {\n";
        code += "        if (BoxColors.ContainsKey(boxName))\n";
        code += "        {\n";
        code += "            return BoxColors[boxName];\n";
        code += "        }\n";
        code += "        return null;\n";
        code += "    }\n\n";
        
        code += "    /// <summary>\n";
        code += "    /// 获取所有颜色数量\n";
        code += "    /// </summary>\n";
        code += "    public static int TotalColorCount => AllColors.Count;\n\n";
        
        code += "    /// <summary>\n";
        code += "    /// 获取箱子数量\n";
        code += "    /// </summary>\n";
        code += "    public static int TotalBoxCount => BoxColors.Count;\n";
        
        code += "}\n";
        
        // 保存到文件
        string filePath = "Assets/Scripts/Data/ReadOnlyBoxColorData.cs";
        System.IO.File.WriteAllText(filePath, code);
        
        AssetDatabase.Refresh();
        
        Debug.Log($"已生成只读数据脚本，共 {readOnlyBoxColors.Count} 个箱子，{allColors.Count} 个颜色");
        Debug.Log($"文件保存到: {filePath}");
    }
}
