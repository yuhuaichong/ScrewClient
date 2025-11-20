# Unity Tools for Visual Studio Code 扩展安装指南

由于 Cursor 的扩展市场可能无法直接搜索到 `visualstudiotoolsforunity.vstuc` 扩展，需要手动安装。

## 方法一：从 VS Code Marketplace 手动下载安装（推荐）

### 步骤 1：下载扩展文件
1. 访问 VS Code Marketplace：
   https://marketplace.visualstudio.com/items?itemName=visualstudiotoolsforunity.vstuc

2. 点击页面右侧的 "Download Extension" 按钮，下载 `.vsix` 文件

### 步骤 2：在 Cursor 中安装
1. 打开 Cursor
2. 按 `Cmd+Shift+P` 打开命令面板
3. 输入 `Extensions: Install from VSIX...`
4. 选择下载的 `.vsix` 文件
5. 等待安装完成

## 方法二：使用命令行安装（如果 Cursor 支持）

如果 Cursor 支持命令行安装扩展，可以尝试：

```bash
# 首先下载扩展（替换为实际下载链接）
curl -L "https://marketplace.visualstudio.com/_apis/public/gallery/publishers/visualstudiotoolsforunity/vsextensions/vstuc/latest/vspackage" -o unity-tools.vsix

# 然后使用 Cursor 安装（如果支持）
cursor --install-extension unity-tools.vsix
```

## 方法三：安装其他必要的扩展

即使 Unity Tools 扩展无法安装，您也可以安装以下扩展来获得基本的 Unity 开发支持：

1. **C#** (`ms-dotnettools.csharp`) - C# 语言支持
2. **C# Dev Kit** (`ms-dotnettools.csdevkit`) - 完整的 C# 开发工具包
3. **Unity Code Snippets** (`kleber-swf.unity-code-snippets`) - Unity 代码片段

这些扩展在 Cursor 的扩展市场中应该可以找到。

## 验证安装

安装完成后：
1. 重启 Cursor
2. 打开 Unity 项目
3. 检查是否出现 Unity 相关的功能（如调试配置等）
4. 尝试在 C# 文件中使用代码补全功能

## 注意事项

- 如果扩展安装后仍然无法正常工作，可能需要：
  - 确保 Unity 编辑器已设置 Cursor 为外部脚本编辑器
  - 在 Unity 中点击 "Regenerate project files"
  - 重启 Cursor 和 Unity 编辑器






