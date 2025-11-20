using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 激励视频类型
/// </summary>
public enum VedioAdType
{
    none,
    复活,
    失败购买盒子,
    收集盒子奖励金币,
    关卡胜利双倍奖励,
    购买电钻,
    购买榔头,
    购买清除孔位,
    任务双倍奖励,
    主界面点击盒子解锁空位,
    飞行宝箱双倍奖励,
    收集盒子奖励金币和金钱,
    弹窗奖励插屏广告,
    失败插屏广告
}
public static class GameVideoContor 
{
   static SDKBase sdkBase;

    public static IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        
#if UNITY_EDITOR 
        sdkBase = new EditorSDK();
#elif UNITY_ANDROID
        // sdkBase = new GameMaxSDK();
        sdkBase = new EditorSDK();
#elif UNITY_IOS
        // iOS平台使用GameMaxSDK，并初始化TenJinSdk
        sdkBase = new GameMaxSDK();
#else
        sdkBase = new EditorSDK();
#endif
        
        sdkBase.Init();
    }
    public static void ShowVideoAd(VedioAdType vedioAdType, string adId, Action<bool, int> closeCallBack, Action<int, string> errorCallBack)
    {


        sdkBase.ShowVideoAd(adId, delegate (bool a, int b) {
            if (a)
            {
                EventManager.Instance.TriggerEvent(GameEvent.OneVideoCom);
                TDAnalyticsManager.Instance.IncentiveAD(vedioAdType.ToString());
                //GameMath.AdComplete?.Invoke();//广告播放完成计数
            }
            closeCallBack?.Invoke(a, b);
        }, errorCallBack);//展示广告
    }
    public static void ShowInterVideoAd(VedioAdType vedioAdType, string adId, Action<bool, int> closeCallBack, Action<int, string> errorCallBack)
    {
        Time.timeScale = 0;
        sdkBase.ShowInterVideoAd(adId, delegate (bool a, int b) {
            if (a)
            {
                EventManager.Instance.TriggerEvent(GameEvent.OneVideoCom);
                TDAnalyticsManager.Instance.InterstitiaAD(vedioAdType);
                //GameMath.AdComplete?.Invoke();//广告播放完成计数
            }
            closeCallBack?.Invoke(a, b);
        }, errorCallBack);//展示广告
    }
}
