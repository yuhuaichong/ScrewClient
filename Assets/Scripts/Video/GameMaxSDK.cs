
using DG.Tweening;
using UnityEngine;
using System;
using static MaxSdkBase;
public class GameMaxSDK : SDKBase
{
    private const string MaxSdkKey = "T2zCVLGybGVqThQqT46q2Phft7rEHbH_7OGi_d2tnCtguip0wwRcd7eZPuHTlZmucs1_RxEUvnT7Zg_7AXBkpT";
    private const string InterstitialAdUnitId = "85778e2c1e8d0b86";
    private const string RewardedAdUnitId = "9c58cce79ec3d777";
    int interstitialRetryAttempt;
    Action<bool, int> closeCallBack;
    Action<int, string> errorCallBack;
    public string appToken = "tzu8qddxpm9s";
    public override void Init()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
         {
             Debug.Log("MAX SDK Initialized");
             InitializeRewardedAds();
             InitializeInterstitialAds();
         };
        MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();
        OtherSdkInit();
    }
    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnAdHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnAdDisplayedEvent;
        // Load the first interstitial
        LoadInterstitial();
    }


    private void OnInterstitialLoadedEvent(string arg1, MaxSdkBase.AdInfo info)
    {
        // GameMath.SetMaxInterstitialEcpm?.Invoke((float)info.Revenue);
    }
    private void OnInterstitialFailedEvent(string arg1, MaxSdkBase.ErrorInfo errorInfo)
    {
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        Debug.LogError("Interstitial failed to load with error code: " + errorInfo.Code);

        DOVirtual.DelayedCall((float)retryDelay, () =>
        {
            MaxSdk.LoadInterstitial(InterstitialAdUnitId);
        });
    }
    private void InterstitialChanceGameTime(int v)
    {
        Time.timeScale = v;
        if (v == 1)
        {
            DOVirtual.DelayedCall(1, () =>
{
    MaxSdk.LoadInterstitial(InterstitialAdUnitId);
});

        }
    }
    private void InterstitialFailedToDisplayEvent(string arg1, ErrorInfo info1, AdInfo info2)
    {
        InterstitialChanceGameTime(1);
        closeCallBack?.Invoke(false, 0);
    }
    private void OnInterstitialDismissedEvent(string arg1, AdInfo info)
    {
        InterstitialChanceGameTime(1);//??п?????
        closeCallBack?.Invoke(false, 0);
        closeCallBack = null;
    }
    private void OnInterstitialRevenuePaidEvent(string arg1, AdInfo info)
    {
        InterstitialChanceGameTime(1);//?????棬??л??
        closeCallBack?.Invoke(true, 0);
        closeCallBack = null;
        InterPaidEven(InterstitialAdUnitId, info);
        //  OnRewardedAdRevenuePaidEvent(InterstitialAdUnitId, info);
    }
    private void OnInterstitialClickedEvent(string arg1, AdInfo info)
    {

    }
    private void OnAdHiddenEvent(string arg1, AdInfo info)
    {

    }
    private void OnAdDisplayedEvent(string arg1, AdInfo info)
    {

    }
    void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
    }
    private void OtherSdkInit()
    {

    }
    public override void ShowVideoAd(string adId, Action<bool, int> closeCallBack, Action<int, string> errorCallBack)
    {
        this.closeCallBack = closeCallBack;
        this.errorCallBack = errorCallBack;
        PlayMax();
    }
    private void PlayMax()
    {
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);
        }
        else
        {
            GameTool.CreatTip("当前没有广告可用，请稍后再试。");
            MaxSdk.LoadRewardedAd(RewardedAdUnitId);
        }
    }
    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        //MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        //MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent; // ??????
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        // Load the first RewardedAd
        LoadRewardedAd();
    }
    private void OnRewardedAdLoadedEvent(string arg1, MaxSdkBase.AdInfo info)
    {
        //????????
        //GameMath.CreatTip($"????????,????????ecpm?{info.Revenue}");
        //GameMath.SetMaxEcpm?.Invoke((float)info.Revenue);
    }

    private void OnRewardedAdFailedEvent(string arg1, MaxSdkBase.ErrorInfo errorInfo)
    {

        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

        DOVirtual.DelayedCall((float)retryDelay, () =>
        {
            MaxSdk.LoadRewardedAd(RewardedAdUnitId);
        });
    }
    private void OnRewardedAdFailedToDisplayEvent(string arg1, MaxSdkBase.ErrorInfo info1, MaxSdkBase.AdInfo info2)
    {
        ChanceGameTime(1);
    }

    private void OnRewardedAdDisplayedEvent(string arg1, MaxSdkBase.AdInfo info)
    {
        ChanceGameTime(0);//??????????
    }
    private void OnRewardedAdDismissedEvent(string arg1, MaxSdkBase.AdInfo info)
    {
        ChanceGameTime(1);//??п?????
        closeCallBack?.Invoke(false, 0);
    }
    private void OnRewardedAdReceivedRewardEvent(string arg1, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo info)
    {
        ChanceGameTime(1);
        DOVirtual.DelayedCall((float)0.1f, () =>
 {
     closeCallBack?.Invoke(true, 0);
 });

    }
    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }
    private void ChanceGameTime(int v)
    {
        Time.timeScale = v;
        if (v == 1)
        {
            DOVirtual.DelayedCall((float)1f, () =>
{
    MaxSdk.LoadRewardedAd(RewardedAdUnitId);
});


        }
    }
    public override void ShowInterVideoAd(string adId, Action<bool, int> closeCallBack, Action<int, string> errorCallBack)
    {
        // closeCallBack?.Invoke(true, 0);
        this.closeCallBack = closeCallBack;
        this.errorCallBack = errorCallBack;
        PlayMaxInterstitial();
    }
    private void PlayMaxInterstitial()
    {
        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
        }
        else
        {
            errorCallBack?.Invoke(1, null);
            GameTool.CreatTip("当前没有广告可用，请稍后再试。");
            MaxSdk.LoadInterstitial(InterstitialAdUnitId);
        }
        return;
    }
    //MAX专用广告收入回调
    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double rewardAmount =adInfo.Revenue ;
        double interAmount = 0;

        // 上报到数数分析
        TDAnalyticsManager.Instance.SendAdRevenue(
            "applovin_max_sdk",          // 收入来源
            adInfo.NetworkName,          // 收入渠道
            adInfo.AdUnitIdentifier,     // 收入单位
            "rewarded",      // 收入位置
            adInfo.Revenue,              // 收入金额
            rewardAmount,                // 激励广告金额
            interAmount                  // 插屏广告金额
        );

        // 触发事件，让Tenjin也能上报广告收入
        TenjinAdRevenueData revenueData = new TenjinAdRevenueData(
            adInfo.AdUnitIdentifier,              // 广告单元ID
            adInfo.AdFormat,                      // 广告格式（REWARDED/INTER等）
            adInfo.NetworkName,                   // 广告网络名称
            adInfo.Placement ?? "",               // 广告位置
            adInfo.Revenue,                       // 广告收入（美元）
            "USD",                                // 货币单位
            MaxSdk.GetSdkConfiguration()?.CountryCode ?? "Unknown",  // 国家代码
            adInfo.CreativeIdentifier ?? "",      // 创意ID
            adInfo.RevenuePrecision ?? ""         // 收入精度
        );

        EventManager.Instance.TriggerEvent(GameEvent.OnAdRevenuePaid, revenueData);
    }
    private void InterPaidEven(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

        double interAmount =  adInfo.Revenue;

        // 上报到数数分析
        TDAnalyticsManager.Instance.SendAdRevenue(
            "applovin_max_sdk",          // 收入来源
            adInfo.NetworkName,          // 收入渠道
            adInfo.AdUnitIdentifier,     // 收入单位
            "interstitial",      // 收入位置
            adInfo.Revenue,              // 收入金额
            0,                // 激励广告金额
            interAmount                  // 插屏广告金额
        );

        // 触发事件，让Tenjin也能上报广告收入
        TenjinAdRevenueData revenueData = new TenjinAdRevenueData(
            adInfo.AdUnitIdentifier,              // 广告单元ID
            adInfo.AdFormat,                      // 广告格式（REWARDED/INTER等）
            adInfo.NetworkName,                   // 广告网络名称
            adInfo.Placement ?? "",               // 广告位置
            adInfo.Revenue,                       // 广告收入（美元）
            "USD",                                // 货币单位
            MaxSdk.GetSdkConfiguration()?.CountryCode ?? "Unknown",  // 国家代码
            adInfo.CreativeIdentifier ?? "",      // 创意ID
            adInfo.RevenuePrecision ?? ""         // 收入精度
        );

        EventManager.Instance.TriggerEvent(GameEvent.OnAdRevenuePaid, revenueData);
    }

}
