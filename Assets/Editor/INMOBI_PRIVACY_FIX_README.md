# InMobi 隐私清单修复说明

## 问题描述

InMobi SDK 的 `PrivacyInfo.xcprivacy` 文件缺少以下 Required Reason API 声明：
- `SystemBootTime` (35F9.1)
- `FileTimestamp` (C617.1)

导致 Xcode 显示警告："Privacy Manifest Undeclared Reasons"。

## 自动修复方案

Unity 构建脚本会自动：
1. 创建修复脚本：`fix_inmobi_privacy.sh`
2. 尝试自动添加到 Xcode Build Phase
3. 在每次构建时自动修复 InMobi 的隐私清单

## 如果自动添加失败

如果自动添加 Build Phase 失败，请手动添加：

### 步骤：

1. **打开 Xcode 项目**
   - 打开 `.xcworkspace` 文件（不是 `.xcodeproj`）

2. **选择 Target**
   - 选择 `Unity-iPhone` Target

3. **添加 Build Phase**
   - 打开 `Build Phases` 标签
   - 点击左上角的 `+` 按钮
   - 选择 `New Run Script Phase`

4. **配置脚本**
   - 展开新的 Run Script Phase
   - 在脚本框中输入：
   ```bash
   bash "${PROJECT_DIR}/fix_inmobi_privacy.sh"
   ```
   - 取消勾选 `For install builds only`
   - 将脚本名称改为：`Fix InMobi Privacy Manifest`

5. **调整执行顺序**
   - 将新的 Build Phase 拖到 **CocoaPods 相关脚本之后**
   - 确保在 `[CP] Check Pods Manifest.lock` 或类似脚本之后
   - 确保在 `Compile Sources` 之前

## 验证

构建项目后，检查日志中是否有：
```
[Fix InMobi] Fixing Privacy Manifest: ...
[Fix InMobi] Privacy Manifest fix completed
```

## 注意事项

- **每次从 Unity 导出后**：如果运行了 `pod install`，InMobi 的隐私清单会被重置
- **必须运行脚本**：确保 Build Phase 在每次构建时都会执行
- **测试环境**：在模拟器上可能看不到警告，需要在真机上测试或上传到 App Store Connect 验证

