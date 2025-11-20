#!/bin/bash
# 自动修复 InMobi SDK 隐私清单文件
# 此脚本会在每次 Xcode 构建时运行（如果添加到 Build Phase）

FIXED_PRIVACY_MANIFEST='<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>NSPrivacyCollectedDataTypes</key>
	<array/>
	<key>NSPrivacyAccessedAPITypes</key>
	<array>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategorySystemBootTime</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>35F9.1</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategoryDiskSpace</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>E174.1</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategoryUserDefaults</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>CA92.1</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyAccessedAPIType</key>
			<string>NSPrivacyAccessedAPICategoryFileTimestamp</string>
			<key>NSPrivacyAccessedAPITypeReasons</key>
			<array>
				<string>C617.1</string>
			</array>
		</dict>
	</array>
</dict>
</plist>'

# InMobi SDK 隐私清单文件路径
INMOBI_PATHS=(
    "${PROJECT_DIR}/Pods/InMobiSDK/InMobiSDK.xcframework/ios-arm64/InMobiSDK.framework/PrivacyInfo.xcprivacy"
    "${PROJECT_DIR}/Pods/InMobiSDK/InMobiSDK.xcframework/ios-arm64_x86_64-simulator/InMobiSDK.framework/PrivacyInfo.xcprivacy"
)

# 修复每个隐私清单文件
for PRIVACY_PATH in "${INMOBI_PATHS[@]}"
do
    if [ -f "$PRIVACY_PATH" ]; then
        echo "[Fix InMobi] Fixing Privacy Manifest: $PRIVACY_PATH"
        echo "$FIXED_PRIVACY_MANIFEST" > "$PRIVACY_PATH"
    fi
done

echo "[Fix InMobi] Privacy Manifest fix completed"
