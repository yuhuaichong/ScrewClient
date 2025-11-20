using System;
using System.Collections.Generic;
using DafultScript;

[Serializable]
public class TimedObjectData
{
    public string objectID;  // 唯一标识符
    public SerializableTime startTime;  // 活动开始时间
    public SerializableTime endTime;  // 活动结束时间
    public SerializableTime leftTime;  // 剩余时间（使用SerializableTime保存）

    // 活动状态标识
    public ActivityState activityStatus;  // 活动状态
    public TimeEventType timeEventType;

    // 构造函数
    public TimedObjectData(string objectID, SerializableTime startTime, SerializableTime endTime, ActivityState status, SerializableTime leftTime, TimeEventType type)
    {
        this.objectID = objectID;
        this.startTime = startTime;
        this.endTime = endTime;
        this.activityStatus = status;
        this.leftTime = leftTime; //?? new SerializableTime(TimeSpan.Zero);  // 如果leftTime为空，则设置为0秒
        this.timeEventType = type;
    }

    // 获取当前倒计时的字符串表示
    public string GetCountdownString()
    {
        return leftTime.ToTimeSpan().ToString(@"dd\天 HH\时 mm\分 ss\秒");
    }
}



[Serializable]
public class GameData
{
    // 游戏关卡数和资源
    public string accoundId;
    public int levelNum;
    public int starCount;
    public int coinCount;
    public float piggyCount;
    public int heartCount;
    public int completeBoxNum;//完成箱子的个数
    public int nextOpenBoxNumRewardPlane;//下一次打开奖励界面时需要收集几个箱子
    public int nextOpenLcukPlane;//下一次打开转盘奖励需要收集几个箱子
    public int nextOpenInterVideoWithEight;//下一次播放点击only0.8时需要播放插屏广告
    public int nowOpenBoxRewardPlaneNum;//打开了几次奖励界面
    public bool isOpenAppraisePlane;//是否打开了评价界面
    public int lastShowTipCount;//上一次播放tip的箱子个数
    public int NextShowTipCount;//下一次收集多少个箱子播放tip
    public DateTime randNumTime;//上一次获取体现信息的时间
    public bool isGetNewPlayerGuite;//是否领取了新手奖励
    public Dictionary<int, bool> levelUpOnState;//关卡上报情况
    public int videoComCount;
    public List<RankData> ranks;
    public DateTime lastGetRankTime;
    public int num1;
    public int num2;
    public float num3;
    public string countryID;
    public SystemLanguage_My currLanguageType;

    //关卡物品解锁进度
    public int curFillCount;
    public int allfillCount;
    public string fillSpriteName;

    // 道具数量
    public int holeItemCount;
    public int rocketItemCount;
    public int doubleBoxItemCount;

    // 道具解锁状态
    public bool isHoleLocked;
    public bool isRocketLocked;
    public bool isDoubleBoxLocked;

    //主界面解锁的按钮
    public bool isDailyRewardLocked;
    public bool isStreaklocked;
    public bool isSkyRacelocked;
    public bool isLuckySpinlocked;

    //关卡解锁的特定物品
    public bool isStarStrewLocked;
    public bool isRopeLocked;
    public bool isIceLocked;
    public bool isDoorLocked;
    public bool isBoomLocked;
    public bool ischainLocked;
    public bool isKeyLocked;
    public bool isClockLocked;

    //每日奖励
    public int curDailyIndex;

    //热气球
    public int curStreakChestIndex;//当前解锁到的宝箱索引
    public int curStreakIndex;//当前的连胜索引
    public int preStreakIndex;//之前点击的索引
    public int unlockStreakChestCount;//当前解锁的宝箱数量

    //抽奖
    public int curSpinCount;
    public int curSpinProgress;

    //贴纸
    public bool isNewSticker; // 是否是新贴纸
    public bool isStikcerChestOpen;
    public int curStickerIndex; // 当前处于第几个贴纸
    public int curButtonIndex; // 当前处于第几批按钮
    public int curB1Index; // 第一个按钮的索引
    public int curB2Index;
    public int curB3Index;

    public List<string> sticker1UnlockList;
    public List<string> sticker2UnlockList;
    public List<string> sticker3UnlockList;
    public List<string> sticker4UnlockList;
    public List<string> sticker5UnlockList;
    public List<string> sticker6UnlockList;
    public List<string> sticker7UnlockList;

    //收集
    public int unlockStickerCount;
    public int completeStikcerCount;

    public Dictionary<int,bool> taskCompleteDci;//任务领取奖励的情况
    public Dictionary<int,bool> taskLevelCompleteDci;//任务领取奖励的情况

    public DateTime lastGetLuckTime;//上次领取抽奖奖励的时间
    public int getLuckNum;//已经抽奖的次数，收集20次箱子可以获得一次抽奖次数
    public bool isLongInGame;//是否进入过游戏
    public DateTime oneLogingInGameTime;//第一次进入游戏的时间
    public Dictionary<int, bool> dayGiftGetStatu;//每日奖励领取情况
    public bool isShowMoneyGuite;//是否显示金钱的新手引导
    public bool isComWithdrawData;
    public string emailName;
    public string email;
    public string eleName;
    public string eleNum;
    public int state;
    public int payChanceSn;

    public bool IsShowTipOne;//是否首次显示了首页弹窗
    public Dictionary<int, bool> dayLoginRewardCompleteDci;
    public DateTime DayLoginRewardGetTime;
    public bool IsCanWIthDraw;
    public Dictionary<int, bool> withDrawTaskCompleteDci;//领取任务奖励的情况
    public List<WithDrawSchedule> withDrawScheduleList;//提现记录
    public Dictionary<int, bool> taskWithDrawDci;//领取任务奖励的情况
}
//时间数据
[Serializable]
public class TimeData
{
    //时间
    public List<TimedObjectData> timedObjects = new List<TimedObjectData>();
    public SerializableTime lastLoginTime;  // 上次登录时间
}
public enum SystemLanguage_My
{
    //
    // 摘要:
    //     Afrikaans.
    Afrikaans = 0,
    //
    // 摘要:
    //     Arabic.
    Arabic = 1,
    //
    // 摘要:
    //     Basque.
    Basque = 2,
    //
    // 摘要:
    //     Belarusian.
    Belarusian = 3,
    //
    // 摘要:
    //     Bulgarian.
    Bulgarian = 4,
    //
    // 摘要:
    //     Catalan.
    Catalan = 5,
    //
    // 摘要:
    //     Chinese.
    Chinese = 6,
    //
    // 摘要:
    //     Czech.
    Czech = 7,
    //
    // 摘要:
    //     Danish.
    Danish = 8,
    //
    // 摘要:
    //     Dutch.
    Dutch = 9,
    //
    // 摘要:
    //     English.
    English = 10,
    //
    // 摘要:
    //     Estonian.
    Estonian = 11,
    //
    // 摘要:
    //     Faroese.
    Faroese = 12,
    //
    // 摘要:
    //     Finnish.
    Finnish = 13,
    //
    // 摘要:
    //     French.
    French = 14,
    //
    // 摘要:
    //     German.
    German = 15,
    //
    // 摘要:
    //     Greek.
    Greek = 16,
    //
    // 摘要:
    //     Hebrew.
    Hebrew = 17,
    Hugarian = 18,
    //
    // 摘要:
    //     Hungarian.
    Hungarian = 18,
    //
    // 摘要:
    //     Icelandic.
    Icelandic = 19,
    //
    // 摘要:
    //     Indonesian.
    Indonesian = 20,
    //
    // 摘要:
    //     Italian.
    Italian = 21,
    //
    // 摘要:
    //     Japanese.
    Japanese = 22,
    //
    // 摘要:
    //     Korean.
    Korean = 23,
    //
    // 摘要:
    //     Latvian.
    Latvian = 24,
    //
    // 摘要:
    //     Lithuanian.
    Lithuanian = 25,
    //
    // 摘要:
    //     Norwegian.
    Norwegian = 26,
    //
    // 摘要:
    //     Polish.
    Polish = 27,
    //
    // 摘要:
    //     Portuguese.
    Portuguese = 28,
    //
    // 摘要:
    //     Romanian.
    Romanian = 29,
    //
    // 摘要:
    //     Russian.
    Russian = 30,
    //
    // 摘要:
    //     Serbo-Croatian.
    SerboCroatian = 31,
    //
    // 摘要:
    //     Slovak.
    Slovak = 32,
    //
    // 摘要:
    //     Slovenian.
    Slovenian = 33,
    //
    // 摘要:
    //     Spanish.
    Spanish = 34,
    //
    // 摘要:
    //     Swedish.
    Swedish = 35,
    //
    // 摘要:
    //     Thai.
    Thai = 36,
    //
    // 摘要:
    //     Turkish.
    Turkish = 37,
    //
    // 摘要:
    //     Ukrainian.
    Ukrainian = 38,
    //
    // 摘要:
    //     Vietnamese.
    Vietnamese = 39,
    //
    // 摘要:
    //     ChineseSimplified.
    ChineseSimplified = 40,
    //
    // 摘要:
    //     ChineseTraditional.
    ChineseTraditional = 41,
    //
    // 摘要:
    //     Unknown.
    Unknown = 42,
    //注意底下是自己加的跟Application.systemLanguage不一样
    //印度
    India = 43,
    //马来西亚
    Malaysia = 44
}