using System;
using UnityEngine;
using DafultScript;
public class TimedObject
{
    public string objectID; // 唯一标识符
    public TimeSpan remainingTime; // 剩余倒计时
    public ActivityState currentState; // 当前活动状态
    public SerializableTime startTime; // 活动开始时间（序列化时间）
    public SerializableTime endTime; // 活动结束后的倒计时（序列化时间）
    public SerializableTime leftTime; // 剩余时间（序列化时间）

    public TimeEventType eventType;

    // 构造函数
    public TimedObject(string id, SerializableTime startTime, SerializableTime endTime, SerializableTime leftTime, ActivityState state, TimeEventType type)
    {
        objectID = id;
        this.startTime = startTime;
        this.endTime = endTime;
        this.leftTime = leftTime;
        currentState = state;
        this.eventType = type;
    }

    // 初始化倒计时，用于游戏启动时恢复状态

    // 更新倒计时
    public void UpdateCountdown()
    {
        if (remainingTime.TotalSeconds > 0)
        {
            remainingTime -= TimeSpan.FromSeconds(Time.deltaTime); // 每帧减少倒计时
        }
        else
        {
            if (currentState == ActivityState.Ongoing)
            {
                HandleCountdownEnd();
            }
            else if (currentState == ActivityState.Ended)
            {
                RestartActivity();
            }
            else if (currentState == ActivityState.Stop)
            {
                //return;
            }
        }
    }

    // 活动结束后的处理
    private void HandleCountdownEnd()
    {
        //Debug.Log($"物体 {objectID} 活动结束，物体将消失");

        switch (eventType)
        {
            case TimeEventType.Streak:
                //关闭条纹按钮
                GameDataManager.SetStreakLockedVal(true);
                HomeSceneUI.Instance.homeUI.SetStreakButton(false);
                remainingTime = endTime.ToTimeSpan();
                GameDataManager.TimeRestartSteak();//重新开始活动(清空之前的活动数据)
                currentState = ActivityState.Ended; // 状态改为结束
                break;

            case TimeEventType.LuckySpin:
                //关闭抽奖按钮
                GameDataManager.SetLuckySpinLockedVal(false);
                HomeSceneUI.Instance.homeUI.SetLuckSpinButton(false);
                
                remainingTime = endTime.ToTimeSpan();
                currentState = ActivityState.Ended; // 状态改为结束
                break;

            case TimeEventType.Heart:
                if (GameDataManager.CurrentGameData.heartCount < 5)
                {
                    // 如果生命小于5，需要一直调用
                    // Heart一直处于开始状态
                    Debug.Log("heart重新开始");
                    EventManager.Instance.TriggerEvent(GameEvent.AddHearEvent);
                    remainingTime = startTime.ToTimeSpan();
                    currentState = ActivityState.Ongoing; // 状态改为重新开始
                }
                else
                {
                    Debug.Log("heart结束");
                    remainingTime = startTime.ToTimeSpan();
                    currentState = ActivityState.Stop; // 状态改为停止
                }
                break;

            case TimeEventType.WireLessHeart:
                // 如果是无线时间，停止计时
                Debug.Log("无限生命停止");
                currentState = ActivityState.Stop;
                remainingTime = TimeSpan.Zero;
                HomeSceneUI.Instance.homeUI.UpdateWirelessHeartIcon();
                break;

            case TimeEventType.DailyReward:
                Debug.Log("重启每日任务");
                EventManager.Instance.TriggerEvent(GameEvent.RestartDailyEvent);
                remainingTime = startTime.ToTimeSpan();
                currentState = ActivityState.Ongoing;
                break;
            default:
                Debug.Log("没有事件可以调用");
                break;
        }

        Debug.Log("调用: " + eventType);

    }

    // 活动重新开始
    private void RestartActivity()
    {

        if (eventType == TimeEventType.Streak)
        {
            //开启条纹按钮
            GameDataManager.SetStreakLockedVal(false);
            HomeSceneUI.Instance.homeUI.SetStreakButton(true);
            remainingTime = startTime.ToTimeSpan();
            GameDataManager.ReStartStreakEvent();
            currentState = ActivityState.Ongoing; // 状态改为进行中
        }
        else if (eventType == TimeEventType.LuckySpin)
        {
            //开启抽奖按钮
            GameDataManager.SetLuckySpinLockedVal(true);
            HomeSceneUI.Instance.homeUI.SetLuckSpinButton(true);
            remainingTime = startTime.ToTimeSpan();
            GameDataManager.RestartLuckySpinEvent();
            currentState = ActivityState.Ongoing; // 状态改为进行中
        }
        else
        {
            Debug.Log("没有事件可以调用");
        }

        Debug.Log("重新开始调用: " + eventType);

        // 重新设置倒计时为活动开始时间

    }

    // 更新剩余时间（从上次登录时间恢复）
    public void UpdateRemainingTime(SerializableTime lastLoginTime)
    {
        //为停止状态的时间，不需要更新
        if (currentState == ActivityState.Stop)
            return;

        remainingTime = leftTime.ToTimeSpan();
        // 计算从上次登录时间到当前时间的时间差
        TimeSpan timeDifference = DateTime.Now - lastLoginTime.ToDateTime();


        //如果时间差还没有让剩余时间减少为0
        if ((remainingTime - timeDifference).TotalSeconds >= 0)
        {
            remainingTime -= timeDifference;
        }
        else
        {
            //剩余的时间差已经为负数了
            if (eventType == TimeEventType.Heart)
            {
                //对于生命事件
                timeDifference -= remainingTime;
                //循环处理，看经过了多少个时间，需要处理几次事件
                while (timeDifference.TotalSeconds > 0 && GameDataManager.CurrentGameData.heartCount < 5)
                {
                    timeDifference -= startTime.ToTimeSpan();
                    GameDataManager.AddHeartCount(1);
                }
                //循环结束时，时间差必为负数
                remainingTime = startTime.ToTimeSpan() + timeDifference;
            }
            else if ((eventType == TimeEventType.Streak && currentState == ActivityState.Ongoing )
                || (eventType == TimeEventType.LuckySpin && currentState == ActivityState.Ongoing))
            {
                //对于条纹事件或是抽奖事件，自动进入恢复活动的倒计时
                timeDifference -= remainingTime;

                //如果已经经过了倒计时的时间
                if ((endTime.ToTimeSpan() - timeDifference).TotalSeconds < 0)
                {
                    //进入活动开始的状态
                    currentState = ActivityState.Ongoing;
                    remainingTime = startTime.ToTimeSpan();
                }
                else
                {
                    //没有超过倒计时的时间
                    remainingTime = endTime.ToTimeSpan();
                    remainingTime -= timeDifference;
                    currentState = ActivityState.Ended;
                }
            }
            else
            {
                remainingTime = TimeSpan.Zero;
            }
        }

        if (remainingTime.TotalSeconds <= 0)
        {
            remainingTime = TimeSpan.Zero;
        }
    }
    // 获取当前倒计时的字符串表示
    public string GetCountdownString()
    {
        return string.Format("{0:D2}天 {1:D2}时 {2:D2}分 {3:D2}秒",
            remainingTime.Days, remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
    }
}
