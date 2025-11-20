using System;
using System.Collections.Generic;

/// <summary>
/// 事件类型的枚举，用于区分不同的事件
/// </summary>
public enum GameEvent
{
    UpdateDailyEvent,
    // 添加更多事件类型...
    UpdateStreakEvent,
    OpenChestEvent,

    //用于时间管理器调用
    AddHearEvent,
    RestartDailyEvent,//重置每日奖励
    ShowBoxPos,
    CreatBoxCompleteCoinEffect,
    ShowBoxNumReward,
    GetDollar,//获取金钱
    OnLanguageChange,
    GetOneTask,//领取一个任务奖励
    GetProp1,
    SetPlayerCoinText,
    ShowPlayerOneLevelGuite,
    ShowNewScrew,
    ScrewGuiteIsOver,
    SetMaskRect,
    HideGuitePlane,
    HideAddBoxBut,
    GetCoin,
    GetProp2,
    GetProp3,
    GetOneLevelTask,
    OpenClikAudio,
    ShowTaskRed,
    HideTaskRed,
    ShowTip1,
    ShowTip2,
    OneLayerDes,
    OneBoxCom,
    ShowGift,
    BocComChanceSliderValue,
    SliderValueResver,
    GetNewGuiteGiftt,
    ShowPro2,
    ShowPro1,
    HideAllPro,
    ShowWithDrawTip,
    ShowHoleTip,
    ShowCoinEffect,
    OneLayerNoGlass,
    OneVideoCom,
    GetOneDayLoginReward,
    RefTaskUI,
    LoseLevel,
    OnAdRevenuePaid//广告收入事件，用于Tenjin上报
}

/// <summary>
/// 一个简单的无参事件管理器
/// </summary>
public class EventManager
{
    private static readonly EventManager _instance = new EventManager();
    public static EventManager Instance => _instance;

    // 存储不同类型的事件字典
    private Dictionary<GameEvent, Delegate> eventDictionary;

    // 私有构造函数，确保单例模式
    private EventManager()
    {
        eventDictionary = new Dictionary<GameEvent, Delegate>();
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void RegisterEvent(GameEvent eventType, Action listener)
    {
        if (!eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = null;
        }
        eventDictionary[eventType] = (Action)eventDictionary[eventType] + listener;
    }

    /// <summary>
    /// 注册单参数事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void RegisterEvent<T>(GameEvent eventType, Action<T> listener)
    {
        if (!eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = null;
        }
        eventDictionary[eventType] = (Action<T>)eventDictionary[eventType] + listener;
    }

    /// <summary>
    /// 注册双参数事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void RegisterEvent<T1, T2>(GameEvent eventType, Action<T1, T2> listener)
    {
        if (!eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = null;
        }
        eventDictionary[eventType] = (Action<T1, T2>)eventDictionary[eventType] + listener;
    }

    /// <summary>
    /// 注册三参数事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void RegisterEvent<T1, T2, T3>(GameEvent eventType, Action<T1, T2, T3> listener)
    {
        if (!eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = null;
        }
        eventDictionary[eventType] = (Action<T1, T2, T3>)eventDictionary[eventType] + listener;
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void UnregisterEvent(GameEvent eventType, Action listener)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = (Action)eventDictionary[eventType] - listener;
            if (eventDictionary[eventType] == null)
            {
                eventDictionary.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 注销单参数事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void UnregisterEvent<T>(GameEvent eventType, Action<T> listener)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = (Action<T>)eventDictionary[eventType] - listener;
            if (eventDictionary[eventType] == null)
            {
                eventDictionary.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 注销双参数事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void UnregisterEvent<T1, T2>(GameEvent eventType, Action<T1, T2> listener)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = (Action<T1, T2>)eventDictionary[eventType] - listener;
            if (eventDictionary[eventType] == null)
            {
                eventDictionary.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 注销三参数事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void UnregisterEvent<T1, T2, T3>(GameEvent eventType, Action<T1, T2, T3> listener)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = (Action<T1, T2, T3>)eventDictionary[eventType] - listener;
            if (eventDictionary[eventType] == null)
            {
                eventDictionary.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    public void TriggerEvent(GameEvent eventType)
    {
        if (eventDictionary.ContainsKey(eventType) && eventDictionary[eventType] != null)
        {
            ((Action)eventDictionary[eventType])();
        }
    }

    /// <summary>
    /// 触发单参数事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="arg">参数</param>
    public void TriggerEvent<T>(GameEvent eventType, T arg)
    {
        if (eventDictionary.ContainsKey(eventType) && eventDictionary[eventType] != null)
        {
            ((Action<T>)eventDictionary[eventType])(arg);
        }
    }

    /// <summary>
    /// 触发双参数事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="arg1">参数1</param>
    /// <param name="arg2">参数2</param>
    public void TriggerEvent<T1, T2>(GameEvent eventType, T1 arg1, T2 arg2)
    {
        if (eventDictionary.ContainsKey(eventType) && eventDictionary[eventType] != null)
        {
            ((Action<T1, T2>)eventDictionary[eventType])(arg1, arg2);
        }
    }

    /// <summary>
    /// 触发三参数事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="arg1">参数1</param>
    /// <param name="arg2">参数2</param>
    /// <param name="arg3">参数3</param>
    public void TriggerEvent<T1, T2, T3>(GameEvent eventType, T1 arg1, T2 arg2, T3 arg3)
    {
        if (eventDictionary.ContainsKey(eventType) && eventDictionary[eventType] != null)
        {
            ((Action<T1, T2, T3>)eventDictionary[eventType])(arg1, arg2, arg3);
        }
    }
}
