// 自动生成的可加载关卡列表
using System;
using UnityEngine.Analytics;

public static class LoadableLevels
{
    // 所有可加载的关卡编号
    public static readonly int[] AllLevels = new int[] { 1, 4, 7,9, 22, 32, 36, 46, 70, 74, 103, 110, 114, 115, 121, 123, 128, 132, 158, 161, 168, 200 };
    public static readonly int[] AAllLevels = new int[] { 1, 4, 7, 9, 22, 32, 36, 46, 70, 74, 103, 110, 114, 115, 121, 123, 128, 132, 158, 161, 168, 200 };
    // 最大关卡编号
    public static readonly int MaxLevel = 200;

    public static int[] needLoadLevle = new int[0];
    // 关卡总数
    public static readonly int TotalLevels = 22;

    public static bool IsLevelLoadable(int levelNum)
    {
        return System.Array.IndexOf(AllLevels, levelNum) >= 0;
    }

    // 根据原始关卡编号获取实际关卡编号
    public static int GetActualLevelNumber(int originalLevelNum)
    {
        if (GameTool.isNeedCloseMoneyIcon)
        {
            //返回A面关卡
            return ReturnALevel(originalLevelNum);

        }
        if (needLoadLevle.Length == 0)
        {
            needLoadLevle = AllLevels;
        }
        //否则正常返回B面关卡
        if (AllLevels.Length == 0)
        {
            UnityEngine.Debug.LogWarning("没有可用的关卡，返回1");
            return 1;
        }

        if (originalLevelNum <= 0)
        {
            UnityEngine.Debug.LogWarning($"原始关卡编号 {originalLevelNum} 小于等于0，返回第一个关卡");
            return AllLevels[0];
        }

        if (originalLevelNum > TotalLevels)//大于最后一个关卡，加载第一关
        {
            originalLevelNum += GameTool.maxLevelNum - 1;
        }
        //originalLevelNum += 17;
        //if(originalLevelNum>)

        // 如果超出范围，循环返回
        int index = (originalLevelNum - 1) % AllLevels.Length;
        if (originalLevelNum > TotalLevels)
        {
            return needLoadLevle[index];
        }
        else
        {
            return AllLevels[index];
        }
    }

    /// <summary>
    /// 返回A面关卡
    /// </summary>
    private static int ReturnALevel(int originalLevelNum)
    {
        if (AAllLevels.Length == 0)
        {
            UnityEngine.Debug.LogWarning("没有可用的关卡，返回1");
            return 1;
        }
        if (needLoadLevle.Length == 0)
        {
            needLoadLevle = AAllLevels;
        }
        if (originalLevelNum <= 0)
        {
            UnityEngine.Debug.LogWarning($"原始关卡编号 {originalLevelNum} 小于等于0，返回第一个关卡");
            return AAllLevels[0];
        }

        if (originalLevelNum > TotalLevels)
        {
            originalLevelNum += 3;
        }
        //originalLevelNum += 17;
        //if(originalLevelNum>)

        // 如果超出范围，循环返回
        int index = (originalLevelNum - 1) % AAllLevels.Length;
        if (originalLevelNum > TotalLevels)
        {
            return needLoadLevle[index];
        }
        else
        {
            return AAllLevels[index];
        }
    }

    // 根据实际关卡编号获取原始关卡编号
    public static int GetOriginalLevelNumber(int actualLevelNum)
    {
        int index = System.Array.IndexOf(AllLevels, actualLevelNum);
        if (index >= 0)
        {
            return index + 1;
        }
        return 1;
    }

    /// <summary>
    /// 获取第一轮的关卡顺序（保持 AllLevels 原始顺序）
    /// </summary>
    public static int[] GetInitialOrder()
    {
        var arr = new int[AllLevels.Length];
        Array.Copy(AllLevels, arr, AllLevels.Length);
        return arr;
    }

    /// <summary>
    /// 获取一轮洗牌后的关卡顺序（对 AllLevels 进行 Fisher–Yates 洗牌）
    /// </summary>
    public static int[] GetShuffledOrder()
    {
        int[] arrRaw = GameTool.isNeedCloseMoneyIcon ? AAllLevels : AllLevels;

        // 拷贝原始数组（避免直接修改原数据）
        int[] arr = new int[arrRaw.Length];
        Array.Copy(arrRaw, arr, arrRaw.Length);

        // 打乱索引 3 之后的部分
        for (int i = arr.Length - 1; i > 3; i--)
        {
            int j = UnityEngine.Random.Range(3, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]); // 元组交换
        }

        // 结果：前3个不变，后面随机
        needLoadLevle = arr;
        // 返回打乱后的部分（不包含前3个）
        int[] shuffledPart = new int[arr.Length - 3];
        Array.Copy(arr, 3, shuffledPart, 0, shuffledPart.Length);
        return shuffledPart;
    }
}
