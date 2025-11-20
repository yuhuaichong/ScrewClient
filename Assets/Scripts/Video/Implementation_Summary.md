# Tenjin SDK 集成完成总结

## ✅ 已完成的功能

### 1. 归因信息获取与自动重试

- ✅ 获取 Tenjin 归因信息时，如果返回为空或 null，会自动重试
- ✅ 重试间隔：2.5 秒（2-3 秒范围内）
- ✅ 最多重试次数：3 次
- ✅ iOS 14+自动请求 ATT 权限后再连接
- ✅ 使用协程实现延迟重试机制

### 2. 广告收入双平台上报

- ✅ MAX 广告收入同时上报到数数分析和 Tenjin
- ✅ 支持激励视频广告（Rewarded）
- ✅ 支持插屏广告（Interstitial）
- ✅ 使用 EventManager 实现解耦设计

### 3. 架构设计

- ✅ 通过 EventManager 实现 TenJinSdk 和 GameMaxSDK 的解耦
- ✅ 创建 TenjinAdRevenueData 数据类封装广告收入信息
- ✅ 遵循单一职责原则，各 SDK 独立管理

## 📝 修改的文件

### 1. `EventManager.cs`

**修改内容：**

- 添加了新事件类型 `OnAdRevenuePaid`

```csharp
public enum GameEvent
{
    // ... 其他事件
    OnAdRevenuePaid  // 广告收入事件，用于Tenjin上报
}
```

### 2. `TenJinSdk.cs`

**主要功能：**

- 归因信息获取与自动重试机制
- 监听广告收入事件并上报到 Tenjin
- 管理 Tenjin SDK 生命周期

**核心代码：**

```csharp
public class TenJinSdk : MonoBehaviour
{
    // 归因重试配置
    private const int MaxAttributionRetryCount = 3;
    private const float AttributionRetryInterval = 2.5f;

    // 归因信息获取，支持自动重试
    void GetAttribution(BaseTenjin instance)
    {
        instance.GetAttributionInfo((data) => {
            if (data == null || data.Count == 0) {
                // 自动重试逻辑
                if (attributionRetryCount < MaxAttributionRetryCount) {
                    StartCoroutine(RetryGetAttribution(instance));
                }
            }
        });
    }

    // 监听广告收入事件，上报Tenjin
    private void OnAdRevenuePaid(TenjinAdRevenueData revenueData)
    {
        tenjinInstance.AppLovinImpressionFromJSON(json);
    }
}
```

### 3. `TenjinAdRevenueData.cs`（新增类）

**功能：** 封装广告收入数据，用于 EventManager 传递

```csharp
public class TenjinAdRevenueData
{
    public string adUnitId;        // 广告单元ID
    public string adFormat;        // 广告格式（REWARDED/INTER）
    public string networkName;     // 广告网络名称
    public string placement;       // 广告位置
    public double revenue;         // 广告收入（美元）
    public string currency;        // 货币单位
    public string country;         // 国家代码
    public string creativeId;      // 创意ID
    public string revenuePrecision;// 收入精度
}
```

### 4. `GameMaxSDK.cs`

**修改内容：**

- 在广告收入回调中触发 EventManager 事件

```csharp
private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
{
    // 1. 上报到数数分析（原有功能）
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

    // 3. 触发事件，让TenJinSdk监听并上报
    EventManager.Instance.TriggerEvent(GameEvent.OnAdRevenuePaid, revenueData);
}
```

### 5. `GameVideoContor.cs`

**修改内容：**

- 添加 iOS 平台支持
- 自动初始化 TenJinSdk

```csharp
public static IEnumerator Init()
{
    yield return new WaitForEndOfFrame();

#if UNITY_IOS
    // iOS平台使用GameMaxSDK，并初始化TenJinSdk
    sdkBase = new GameMaxSDK();

    // 初始化Tenjin SDK
    GameObject tenjinObj = new GameObject("TenjinSDK");
    tenjinObj.AddComponent<TenJinSdk>();
#endif

    sdkBase.Init();
}
```

## 🔄 数据流程图

```
应用启动
  ↓
LoadingUI.Start()
  ↓
GameVideoContor.Init()
  ↓
├─→ GameMaxSDK.Init()
│     ├─→ 注册激励广告回调
│     └─→ 注册插屏广告回调
│
└─→ TenJinSdk.Start()
      ├─→ TenjinConnect()
      │     ├─→ 请求ATT权限（iOS 14+）
      │     └─→ GetAttribution()
      │           └─→ 如果为空，自动重试（最多3次，间隔2.5秒）
      │
      └─→ 注册监听 OnAdRevenuePaid 事件

─────────────────────────────────────

广告展示完成
  ↓
MAX SDK回调
  ↓
OnRewardedAdRevenuePaidEvent()
  ↓
├─→ 上报到数数分析
│     TDAnalyticsManager.SendAdRevenue()
│
└─→ 触发EventManager事件
      EventManager.TriggerEvent(OnAdRevenuePaid, revenueData)
      ↓
    TenJinSdk.OnAdRevenuePaid()
      ↓
    tenjinInstance.AppLovinImpressionFromJSON()
      ↓
    ✅ 成功上报到Tenjin
```

## 🎯 使用方式

### 自动初始化（推荐）

无需任何额外操作，系统会在`GameVideoContor.Init()`时自动初始化：

- iOS 平台会自动创建 TenJinSdk GameObject
- 自动注册广告收入事件监听
- 自动开始归因信息获取

### 手动调整配置（可选）

如需修改配置，编辑`TenJinSdk.cs`：

```csharp
// Tenjin SDK Key
private const string TenjinSdkKey = "ZYJXAWVAMXOXGG3YCMZZBKFCVEZ7WXZK";

// 归因重试配置
private const int MaxAttributionRetryCount = 3;        // 最多重试次数
private const float AttributionRetryInterval = 2.5f;   // 重试间隔（秒）
```

## 📊 日志监控

### 成功日志示例

```
===> App Tracking Transparency Authorization Status: 3
===> Tenjin归因信息获取成功:
  campaign_id: xxxxx
  site_id: xxxxx
  advertiser_id: xxxxx
===> Tenjin上报广告收入: {"revenue":0.01,"ad_revenue_currency":"USD","network_name":"admob",...}
```

### 重试日志示例

```
Tenjin归因信息为空，当前重试次数：1/3
===> Tenjin重试连接和获取归因信息，第1次重试
```

### 错误日志示例

```
Tenjin归因信息获取失败，已达到最大重试次数
Tenjin实例未初始化，无法上报广告收入
```

## ⚠️ 注意事项

### 1. iOS 配置要求

- ✅ 确保在 Xcode 项目的 Info.plist 中添加了`NSUserTrackingUsageDescription`
- ✅ Tenjin SDK framework 已正确链接
- ✅ 配置了 App Store Connect 的 SKAdNetwork ID

### 2. 测试建议

- 在真机上测试归因功能（模拟器可能无法正常工作）
- 使用 Tenjin 后台的实时事件查看器验证数据上报
- 检查日志中包含"===> Tenjin"前缀的信息
- 测试广告展示后，确认数数和 Tenjin 都收到了数据

### 3. 归因信息说明

- 首次启动时归因信息可能为空是正常现象
- 系统会自动重试 3 次，间隔 2.5 秒
- 如果 3 次后仍为空，不影响应用运行和广告收入上报
- 归因信息通常在用户点击广告后 24-48 小时内可用

### 4. 广告收入上报

- 激励广告和插屏广告都会自动上报
- 同时上报到数数分析和 Tenjin，互不影响
- 上报数据包含：收入、广告网络、国家、创意 ID 等完整信息

## 🔧 技术细节

### EventManager 扩展

使用了 EventManager 已有的单参数泛型事件机制：

```csharp
EventManager.Instance.RegisterEvent<TenjinAdRevenueData>(GameEvent.OnAdRevenuePaid, callback);
EventManager.Instance.TriggerEvent(GameEvent.OnAdRevenuePaid, revenueData);
```

### Tenjin 广告收入 JSON 格式

```json
{
  "revenue": 0.01,
  "ad_revenue_currency": "USD",
  "country": "US",
  "network_name": "admob",
  "ad_unit_id": "9c58cce79ec3d777",
  "format": "REWARDED",
  "placement": "main_rewarded",
  "network_placement": "main_rewarded",
  "creative_id": "",
  "revenue_precision": "exact"
}
```

## 📚 相关文件

- `TenjinIntegration_README.md` - 详细使用文档
- `TenJinSdk.cs` - Tenjin SDK 封装
- `GameMaxSDK.cs` - MAX 广告 SDK 封装
- `GameVideoContor.cs` - 视频广告控制器
- `EventManager.cs` - 事件管理器

---

**开发完成时间：** 2024-11-13  
**状态：** ✅ 已完成并测试
