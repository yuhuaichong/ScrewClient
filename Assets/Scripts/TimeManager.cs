using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DafultScript;
public enum ActivityState
{
    Ongoing, Ended, Stop
}

public enum TimeEventType
{ 
    Streak,
    LuckySpin,
    Heart,
    WireLessHeart,//无限生命
    DailyReward
}

public class TimeManager : MonoBehaviour
{
    private Dictionary<string, TimedObject> timedObjects = new Dictionary<string, TimedObject>();  // 存储多个物体的倒计时管理器
    public bool IsWireLessHeart { get; set; }
    public TimedObject GetTimeObj(TimeEventType type)
    {
        foreach (TimedObject obj in timedObjects.Values)
        {
            if (type == obj.eventType)
            {
                return obj;
            }
        }

        return null;
    }

    private bool isLoaded = false;

    private static TimeManager _instance;

    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试在场景中找到一个现有的实例
                _instance = FindObjectOfType<TimeManager>();

                // 如果场景中没有实例，则创建一个新的GameObject并添加TimeManager组件
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("TimeManager");
                    _instance = singletonObject.AddComponent<TimeManager>();

                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // 防止重复实例化
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // 如果已经存在一个实例，销毁重复的
        }
        //AddDefaultTime();
        if (isLoaded == false)
        {
            LoadTimedObjects();
        }

        StartCoroutine(nameof(SaveDataInterval));
    }

    private void Update()
    {
        foreach (TimedObject timedObject in timedObjects.Values)
        {
            if(timedObject.currentState != ActivityState.Stop)
                timedObject.UpdateCountdown();  // 更新倒计时
        }


    }

    private void OnApplicationQuit()
    {
        Debug.Log("游戏退出保存时间数据");
        SaveTimedObjects();
        GameDataManager.Save();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("应用程序已经停止，保存数据.");
            SaveTimedObjects();
            GameDataManager.Save();

            StopCoroutine(SaveDataInterval());//暂停保存时间数据的协程
        }
        else
        {
            Debug.Log("应用进程已经恢复.");
            //foreach (TimedObject timeobj in timedObjects.Values)
            //{
            //    timeobj.UpdateRemainingTime(TimeDataManager.CurrentTimeData.lastLoginTime);
            //}
            ReLoadTimeObjct();
            StartCoroutine(nameof(SaveDataInterval));//开始保存时间数据的协程
        }
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("应用进程失去焦点，保存数据");
            SaveTimedObjects();
            GameDataManager.Save();
        }
    }

    private void ReLoadTimeObjct()
    {
        foreach (var timedObjectData in TimeDataManager.CurrentTimeData.timedObjects)
        {
            if (timedObjects.ContainsKey(timedObjectData.objectID))
            {
                timedObjects[timedObjectData.objectID].leftTime = timedObjectData.leftTime;
                timedObjects[timedObjectData.objectID].UpdateRemainingTime(TimeDataManager.CurrentTimeData.lastLoginTime);
            }
        }
    }
    public void LoadTimedObjects()
    {
        isLoaded = true;
        foreach (var timedObjectData in TimeDataManager.CurrentTimeData.timedObjects)
        {
            // 创建一个新的 TimedObject，并使用从保存数据中加载的参数初始化
            TimedObject timedObject = new TimedObject(
                timedObjectData.objectID,
                timedObjectData.startTime,
                timedObjectData.endTime,
                timedObjectData.leftTime,
                timedObjectData.activityStatus,
                timedObjectData.timeEventType
            );
            // 将初始化后的 timedObject 添加到 timedObjects 字典
            timedObjects.Add(timedObjectData.objectID, timedObject);
            // 使用 lastLoginTime 更新剩余时间
            timedObject.UpdateRemainingTime(TimeDataManager.CurrentTimeData.lastLoginTime);
        }
        //Debug.Log(timedObjects.Count);
    }
    private void SaveTimedObjects()
    {
        List<TimedObjectData> timedObjectDataList = new List<TimedObjectData>();

        // 将 timedObjects 转换为 TimedObjectData 并保存到列表
        foreach (var timedObject in timedObjects.Values)
        {
            TimedObjectData timedObjectData = new TimedObjectData(
                timedObject.objectID,
                timedObject.startTime,
                timedObject.endTime,
                timedObject.currentState,
                new SerializableTime(timedObject.remainingTime), // 使用 leftTime 进行存储
                timedObject.eventType
            );
            timedObjectDataList.Add(timedObjectData);
        }
        if (timedObjectDataList.Count > 0)
        {
            // 保存到 CurrentGameData
            TimeDataManager.CurrentTimeData.timedObjects = timedObjectDataList;
        }

        // 保存 lastLoginTime
        TimeDataManager.CurrentTimeData.lastLoginTime = new SerializableTime(DateTime.Now);

        // 保存数据到文件
        TimeDataManager.SaveTimeData();
    }
    #region 添加默认数据
    public void AddDefaultTime()
    {
        AddDefaultHeart();
        AddDefaultWirelessHeart();
        AddDefualtDailyReward();
        AddDefualtLuckySpinTime();
        AddDefualtStickerTime();
        TimeDataManager.SaveTimeData();
    }
    private void AddDefaultHeart()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(0, 15, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("Heart", time, time,
            ActivityState.Stop, time, TimeEventType.Heart));
    }
    private void AddDefaultWirelessHeart()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(0, 1, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("WirelessHeart", time, time,
            ActivityState.Stop, time, TimeEventType.WireLessHeart));
    }
    private void AddDefualtDailyReward()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(24, 0, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("DailyReward", time, time,
            ActivityState.Stop, time, TimeEventType.DailyReward));
    }
    private void AddDefualtStickerTime()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(7, 0, 0, 0));
        SerializableTime endtime = new SerializableTime(new TimeSpan(2, 0, 0, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("Streak", time, endtime,
            ActivityState.Stop, time, TimeEventType.Streak));
    }
    private void AddDefualtLuckySpinTime()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(7, 0, 0, 0));
        SerializableTime endtime = new SerializableTime(new TimeSpan(2, 0, 0, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("LuckySpin", time, endtime,
            ActivityState.Stop, time, TimeEventType.LuckySpin));
    }
    #endregion

    #region 间隔时间保存时间数据
    /// <summary>
    /// 协程：每隔 2 秒执行一次保存操作
    /// </summary>
    /// <returns></returns>
    private IEnumerator SaveDataInterval()
    {
        while (true)  // 无限循环，每隔 2 秒保存一次
        {
            // 保存数据（这里调用你自己的保存数据方法）
            SaveTimedObjects();

            // 等待 2 秒
            yield return new WaitForSeconds(2.5f);
        }
    }

    #endregion

    /// <summary>
    /// 恢复无限生命
    /// </summary>
    public void RecoverWirelessHeart(int h, int m, int s)
    {
        TimedObject wirelessTime = GetTimeObj(TimeEventType.WireLessHeart);
        wirelessTime.currentState = ActivityState.Ongoing;
        wirelessTime.remainingTime = new TimeSpan(h, m, s);
    }

    /// <summary>
    /// 增加无限生命的时间
    /// </summary>
    /// <param name="h"></param>
    /// <param name="m"></param>
    /// <param name="s"></param>
    public void AddWirelessHeartTime(int h, int m, int s)
    {
        TimedObject wirelessTime = GetTimeObj(TimeEventType.WireLessHeart);

        //如果为停止状态，归零之前的剩余时间
        if (wirelessTime.currentState == ActivityState.Stop)
            wirelessTime.remainingTime = TimeSpan.Zero;

        wirelessTime.currentState = ActivityState.Ongoing;
        wirelessTime.remainingTime += new TimeSpan(h, m, s);

        HomeSceneUI.Instance.homeUI.UpdateWirelessHeartIcon();
    }

    /// <summary>
    /// 开始条纹计时
    /// </summary>
    public void StartStreakTime()
    {
        TimedObject streakTime = GetTimeObj(TimeEventType.Streak);
        streakTime.remainingTime = streakTime.startTime.ToTimeSpan();
        streakTime.currentState = ActivityState.Ongoing;
    }

    /// <summary>
    /// 开始抽奖计时
    /// </summary>
    public void StartLuckySpinTime()
    {
        TimedObject streakTime = GetTimeObj(TimeEventType.LuckySpin);
        streakTime.remainingTime = streakTime.startTime.ToTimeSpan();
        streakTime.currentState = ActivityState.Ongoing;
    }

    /// <summary>
    /// 开始每日计时
    /// </summary>
    public void StartDailyTime()
    {
        TimedObject streakTime = GetTimeObj(TimeEventType.DailyReward);
        streakTime.remainingTime = streakTime.startTime.ToTimeSpan();
        streakTime.currentState = ActivityState.Ongoing;
    }
}

