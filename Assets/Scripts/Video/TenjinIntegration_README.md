# Tenjin SDK 集成说明

## 功能概述

本项目已完成 Tenjin SDK 的集成，主要包含以下功能：

### 1. 归因信息获取与重试机制

- 自动获取 Tenjin 归因信息
- 如果归因信息为空或未返回，会自动重试
- 重试间隔：2.5 秒
- 最大重试次数：3 次
- iOS 14+自动请求 ATT（App Tracking Transparency）权限

### 2. 广告收入上报

- 支持 MAX（AppLovin）广告平台的收入上报
- 同时上报到数数分析（ThinkingAnalytics）和 Tenjin
- 支持激励视频广告和插屏广告
- 使用 EventManager 实现解耦设计

## 文件说明

### 核心文件

- `TenJinSdk.cs` - Tenjin SDK 封装类
- `GameMaxSDK.cs` - MAX 广告 SDK 封装类
- `EventManager.cs` - 事件管理器
- `TenjinAdRevenueData` - 广告收入数据类

## 使用方法

### 初始化

确保在 Load 场景或启动场景中添加`TenJinSdk`组件：

```csharp
// TenJinSdk会自动初始化，无需手动调用
// 在Awake时会创建单例并DontDestroyOnLoad
```

### 广告收入上报流程

1. **GameMaxSDK** 在广告收入回调中触发事件：

```csharp
private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
{
    // 1. 上报到数数分析
    TDAnalyticsManager.Instance.SendAdRevenue(...);

    // 2. 创建Tenjin广告收入数据
    TenjinAdRevenueData revenueData = new TenjinAdRevenueData(
        adInfo.AdUnitIdentifier,
        adInfo.AdFormat,
        adInfo.NetworkName,
        adInfo.Placement ?? "",
        adInfo.Revenue,
        "USD",
        MaxSdk.GetSdkConfiguration()?.CountryCode ?? "Unknown",
        adInfo.CreativeIdentifier ?? "",
        adInfo.RevenuePrecision ?? ""
    );

    // 3. 触发事件
    EventManager.Instance.TriggerEvent(GameEvent.OnAdRevenuePaid, revenueData);
}
```

2. **TenJinSdk** 监听事件并上报：

```csharp
// 在Start中注册监听
EventManager.Instance.RegisterEvent<TenjinAdRevenueData>(GameEvent.OnAdRevenuePaid, OnAdRevenuePaid);

// 收到事件后上报给Tenjin
private void OnAdRevenuePaid(TenjinAdRevenueData revenueData)
{
    tenjinInstance.AppLovinImpressionFromJSON(json);
}
```

## 数据流向

```
MAX广告展示
  ↓
OnAdRevenuePaidEvent回调
  ↓
├─→ TDAnalyticsManager（数数分析）
└─→ EventManager.TriggerEvent
      ↓
    TenJinSdk.OnAdRevenuePaid
      ↓
    Tenjin.AppLovinImpressionFromJSON
```

## 重要说明

### 归因信息重试

- 归因信息可能在首次启动时为空，这是正常现象
- 系统会自动重试 3 次，间隔 2.5 秒
- 如果 3 次后仍为空，会记录错误日志但不影响应用运行

### iOS 配置

确保在 Xcode 项目中：

1. 已添加`NSUserTrackingUsageDescription`到 Info.plist
2. Tenjin SDK framework 已正确链接
3. 已配置 App Store Connect 的 SKAdNetwork ID

### 测试建议

1. 在真机上测试归因功能（模拟器可能无法正常工作）
2. 使用 Tenjin 后台的实时事件查看器验证数据上报
3. 检查日志中的"===> Tenjin"前缀信息

## 日志输出

### 成功日志

```
===> App Tracking Transparency Authorization Status: 3
===> Tenjin归因信息获取成功:
  campaign_id: xxxxx
  site_id: xxxxx
===> Tenjin上报广告收入: {"revenue":0.01,"ad_revenue_currency":"USD",...}
```

### 重试日志

```
Tenjin归因信息为空，当前重试次数：1/3
===> Tenjin重试连接和获取归因信息，第1次重试
```

## 技术支持

如需修改 Tenjin 配置：

- SDK Key：在`TenJinSdk.cs`的`TenjinSdkKey`常量中修改
- 重试次数：修改`MaxAttributionRetryCount`常量
- 重试间隔：修改`AttributionRetryInterval`常量（单位：秒）

---

更新时间：2024-11
