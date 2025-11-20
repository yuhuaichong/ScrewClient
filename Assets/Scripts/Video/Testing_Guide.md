# Tenjin SDK 测试指南

## 📋 测试前准备

### 1. 构建配置

```
Platform: iOS
Build Configuration: Release（用于测试广告）
```

### 2. Xcode 配置检查

- [ ] Info.plist 中已添加 `NSUserTrackingUsageDescription`
- [ ] Tenjin SDK framework 已正确链接
- [ ] SKAdNetwork IDs 已配置

### 3. 设备要求

- iOS 真机（推荐 iOS 14+）
- 非越狱设备
- 已连接互联网

## 🧪 测试步骤

### 测试 1：归因信息获取

**目的：** 验证 Tenjin 能正常初始化并获取归因信息

**步骤：**

1. 清理应用数据（卸载后重新安装）
2. 启动应用
3. 在 Xcode 控制台查看日志

**预期结果：**

```
===> App Tracking Transparency Authorization Status: 3
===> Tenjin归因信息获取成功:
  campaign_id: xxxxx
  ...
```

**如果看到重试：**

```
Tenjin归因信息为空，当前重试次数：1/3
===> Tenjin重试连接和获取归因信息，第1次重试
```

这是正常的！系统会自动重试 3 次。

---

### 测试 2：归因信息重试机制

**目的：** 验证归因信息为空时会自动重试

**步骤：**

1. 在飞行模式下启动应用
2. 观察日志输出
3. 恢复网络连接

**预期结果：**

- 首次获取失败
- 看到重试日志（间隔约 2.5 秒）
- 最多重试 3 次

---

### 测试 3：激励视频广告收入上报

**目的：** 验证激励视频广告收入同时上报到数数和 Tenjin

**步骤：**

1. 触发激励视频广告（例如：双倍奖励）
2. 完整观看广告
3. 检查 Xcode 控制台日志

**预期日志：**

```
// 数数分析上报（已有）
[ThinkingData] SendAdRevenue: ...

// Tenjin上报（新增）
===> Tenjin上报广告收入: {
    "revenue": 0.01,
    "ad_revenue_currency": "USD",
    "network_name": "admob",
    "ad_unit_id": "9c58cce79ec3d777",
    "format": "REWARDED",
    ...
}
```

**验证方式：**

1. 打开 Tenjin 后台：https://www.tenjin.io/
2. 进入实时事件查看器
3. 查找 Ad Impression 事件
4. 确认收入数据准确

---

### 测试 4：插屏广告收入上报

**目的：** 验证插屏广告收入也能正常上报

**步骤：**

1. 触发插屏广告（例如：关卡失败）
2. 等待广告播放完成或关闭
3. 检查日志

**预期结果：**
同测试 3，但`format`字段为`INTER`或`INTERSTITIAL`

---

### 测试 5：ATT 权限请求（iOS 14+）

**目的：** 验证 iOS 14+设备上会正确请求 ATT 权限

**步骤：**

1. 确保设备 iOS 版本 ≥ 14.0
2. 首次启动应用
3. 观察是否弹出权限请求

**预期结果：**

1. 弹出系统 ATT 权限对话框
2. 无论用户选择允许或拒绝，Tenjin 都能正常连接
3. 日志显示权限状态：

```
===> App Tracking Transparency Authorization Status: 3  // 3=允许
===> App Tracking Transparency Authorization Status: 2  // 2=拒绝
```

---

## 🔍 调试技巧

### 1. 查看完整日志

在 Xcode 中：

```
Product > Scheme > Edit Scheme > Run > Arguments
添加启动参数: -com.apple.CoreData.SQLDebug 1
```

### 2. 过滤 Tenjin 日志

在 Xcode 控制台搜索框输入：

```
Tenjin
```

### 3. 测试归因信息

如果归因信息一直为空：

- 这是正常的（用户可能不是通过广告安装）
- 不影响广告收入上报功能
- 可以使用 Tenjin 测试链接进行测试

---

## ✅ 测试检查清单

### 基础功能

- [ ] Tenjin SDK 正常初始化
- [ ] ATT 权限正确请求（iOS 14+）
- [ ] 归因信息获取（成功或重试）
- [ ] TenJinSdk GameObject 已创建并 DontDestroyOnLoad

### 广告收入上报

- [ ] 激励视频广告收入上报到 Tenjin
- [ ] 插屏广告收入上报到 Tenjin
- [ ] 同时上报到数数分析（不影响原有功能）
- [ ] Tenjin 后台能看到 Ad Impression 事件

### 异常情况

- [ ] 网络断开时不崩溃
- [ ] 归因信息为空时能正常重试
- [ ] 重试 3 次后停止（不无限重试）
- [ ] Tenjin 未初始化时有警告日志

### 性能检查

- [ ] 启动时间无明显延长
- [ ] 内存占用正常
- [ ] 不影响广告展示

---

## 🐛 常见问题

### Q1: 看不到 Tenjin 日志

**A:** 确保在 iOS 平台编译，编辑器模式下不会初始化 TenJinSdk

### Q2: 归因信息一直为空

**A:** 这是正常的！用户可能不是通过广告安装。系统会重试 3 次后继续运行。

### Q3: Tenjin 后台看不到数据

**A:**

1. 等待 1-5 分钟（数据有延迟）
2. 确认设备时间正确
3. 检查 Tenjin SDK Key 是否正确
4. 查看实时事件查看器而不是报表

### Q4: 广告收入数据不匹配

**A:**

1. 确认 MAX SDK 已正确初始化
2. 检查`OnRewardedAdRevenuePaidEvent`是否被调用
3. 对比数数分析的数据

### Q5: ATT 权限对话框不显示

**A:**

- iOS 版本 < 14.0 不会显示（正常）
- 已经授权过（设置 > 隐私 > 跟踪 中查看）
- Info.plist 缺少 NSUserTrackingUsageDescription

---

## 📞 支持

如遇到问题：

1. 检查 Xcode 控制台日志
2. 查看`Implementation_Summary.md`了解架构
3. 查看`TenjinIntegration_README.md`了解详细功能
4. 联系 Tenjin 技术支持

---

**最后更新：** 2024-11-13
