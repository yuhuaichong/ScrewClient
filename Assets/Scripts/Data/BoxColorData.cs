using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保存箱子颜色数据的静态类
/// </summary>
public static class BoxColorData
{
    /// <summary>
    /// 存储所有箱子的颜色数据，key为箱子名称，value为颜色列表
    /// </summary>
    public static Dictionary<string, List<ScrewColor>> BoxColors = new Dictionary<string, List<ScrewColor>>();
    
    /// <summary>
    /// 存储所有箱子的颜色数据（只读字典）
    /// </summary>
    public static readonly Dictionary<string, List<ScrewColor>> ReadOnlyBoxColors = new Dictionary<string, List<ScrewColor>>();
    
    /// <summary>
    /// 存储所有颜色的集合（只读列表）
    /// </summary>
    public static readonly List<ScrewColor> AllColors = new List<ScrewColor>();
    
    /// <summary>
    /// 清空所有数据
    /// </summary>
    public static void ClearData()
    {
        BoxColors.Clear();
    }
    
    /// <summary>
    /// 添加箱子颜色数据
    /// </summary>
    /// <param name="boxName">箱子名称</param>
    /// <param name="colors">颜色列表</param>
    public static void AddBoxColors(string boxName, List<ScrewColor> colors)
    {
        if (BoxColors.ContainsKey(boxName))
        {
            BoxColors[boxName] = colors;
        }
        else
        {
            BoxColors.Add(boxName, colors);
        }
    }
    
    /// <summary>
    /// 获取指定箱子的颜色数据
    /// </summary>
    /// <param name="boxName">箱子名称</param>
    /// <returns>颜色列表</returns>
    public static List<ScrewColor> GetBoxColors(string boxName)
    {
        if (BoxColors.ContainsKey(boxName))
        {
            return BoxColors[boxName];
        }
        return null;
    }
    
    /// <summary>
    /// 打印所有数据到控制台
    /// </summary>
    public static void PrintAllData()
    {
        Debug.Log("=== BoxColorData 所有数据 ===");
        foreach (var kvp in BoxColors)
        {
            string colorString = "";
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                colorString += kvp.Value[i].ToString();
                if (i < kvp.Value.Count - 1)
                    colorString += ", ";
            }
            Debug.Log($"箱子: {kvp.Key}, 颜色: [{colorString}]");
        }
        Debug.Log("=== 数据打印完成 ===");
    }
}
