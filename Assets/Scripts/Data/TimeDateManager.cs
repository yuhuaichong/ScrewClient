using System;
using System.Collections.Generic;
using UnityEngine;

public static class TimeDataManager
{
    // 当前的时间数据
    public static TimeData CurrentTimeData { get; private set; }

    // 初始化
    public static void Initialize()
    {
        // 尝试加载时间数据
        LoadTimeData();

        // 如果加载失败，初始化数据
        if (CurrentTimeData == null)
        {
            ResetTimeData();
            SaveTimeData();
        }
    }

    // 保存时间数据
    public static void SaveTimeData()
    {
        JsonDataManager.SaveTimeData(CurrentTimeData);
    }

    // 加载时间数据
    public static void LoadTimeData()
    {
        CurrentTimeData = JsonDataManager.LoadTimeData(); ;
    }

    // 重置时间数据
    public static void ResetTimeData()
    {
        CurrentTimeData = new TimeData
        {
            timedObjects = new List<TimedObjectData>(),
            lastLoginTime = new SerializableTime(DateTime.Now)
        };
    }

    #region 时间管理
    public static void AddTimeDataList(TimedObjectData obj)
    {
        CurrentTimeData.timedObjects.Add(obj);
        //Save();
    }

    #endregion
}
