using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class NameUtility
{
    // 处理名字的方法
    public static void SaveDataByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Name cannot be null or empty.");
            return;
        }

        // 将名字转为小写
        string lowerCaseName = name.ToLower();

        // 检查名字中的关键字并执行操作
        if (lowerCaseName.Contains("icon"))
        {
            HandleIcon();
        }

        if (lowerCaseName.Contains("heart"))
        {
            HandleHeart();
        }

        if (lowerCaseName.Contains("hole"))
        {
            HandleHole();
        }

        if (lowerCaseName.Contains("rocket"))
        {
            HandleRocket();
        }

        if (lowerCaseName.Contains("doublebox"))
        {
            HandleDoubleBox();
        }
    }

    // 各种关键字的处理方法
    private static void HandleIcon()
    {
        
        // 在这里实现具体的操作逻辑
    }

    private static void HandleHeart()
    {
        Console.WriteLine("Handling 'heart' operation.");
        // 在这里实现具体的操作逻辑
    }

    private static void HandleHole()
    {
        Console.WriteLine("Handling 'hole' operation.");
        // 在这里实现具体的操作逻辑
    }

    private static void HandleRocket()
    {
        Console.WriteLine("Handling 'rocket' operation.");
        // 在这里实现具体的操作逻辑
    }

    private static void HandleDoubleBox()
    {
        Console.WriteLine("Handling 'doubleBox' operation.");
        // 在这里实现具体的操作逻辑
    }

    public static string GetStringByName(string name)
    {
        string curName = name.ToLower();
        if (curName.Contains("heart"))
        {
            return "m";
        }
        else if (curName.Contains("coin"))
        {
            return "";
        }
        else
            return "x";

    }

    public static void SortGameObjectsByName(List<GameObject> gameObjectList, bool sortByNumber = false)
    {
        if (sortByNumber)
        {
            gameObjectList.Sort((a, b) =>
            {
                int numberA = ExtractNumberFromName(a.name);
                int numberB = ExtractNumberFromName(b.name);
                return numberA.CompareTo(numberB);
            });
        }
        else
        {
            gameObjectList.Sort((a, b) => string.Compare(a.name, b.name));
        }
    }

    private static int ExtractNumberFromName(string name)
    {
        string numberPart = new string(name.Where(char.IsDigit).ToArray());
        if (int.TryParse(numberPart, out int number))
        {
            return number;
        }
        return 0; // 默认值
    }

    /// <summary>
    /// 个位数去0
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static int SetLastDigitToZero(int number)
    {
        // 去掉个位数，然后乘以 10
        return (number / 10) * 10;
    }
}

public static class TransFormUtility
{
    /// <summary>
    /// 将特效位置设置到对应 UI 元素上（Canvas 为 Screen Space - Overlay）
    /// </summary>
    /// <param name="effectTransform">特效的 Transform</param>
    /// <param name="uiElement">目标 UI 元素的 RectTransform</param>
    public static void SetEffectPosition(Transform effectTransform, RectTransform uiElement)
    {
        if (effectTransform == null || uiElement == null)
        {
            Debug.LogError("参数不能为空：effectTransform 或 uiElement");
            return;
        }

        // 将 RectTransform 的屏幕坐标转化为世界坐标
        Vector3 worldPosition = uiElement.position;

        // 设置特效的世界坐标
        effectTransform.position = worldPosition;
    }

    /// <summary>
    /// 将一个游戏物体移动到UI物体的位置 (适用于 Canvas 渲染模式为 Screen Space - Overlay)。
    /// </summary>
    /// <param name="gameObject">需要移动的游戏物体 Transform。</param>
    /// <param name="uiObject">目标UI物体 RectTransform。</param>
    /// <param name="zOffset">游戏物体的Z轴偏移量。</param>
    public static void MoveGameObjectToUIPosition(Transform gameObject, RectTransform uiObject, float zOffset = 0f)
    {
        if (gameObject == null || uiObject == null)
        {
            Debug.LogError("GameObject or UIObject is null!");
            return;
        }

        // 获取UI物体在屏幕上的坐标
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(null, uiObject.position);

        // 将屏幕坐标转换为世界坐标
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.nearClipPlane + zOffset));

        // 设置游戏物体的位置
        gameObject.position = worldPosition;
    }

    /// <summary>
    /// 删除指定物体下的所有子物体。
    /// </summary>
    /// <param name="parent">父物体的 Transform。</param>
    public static void DestroyAllChildren(Transform parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("父物体为空，无法删除子物体。");
            return;
        }

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
        }
    }
}

