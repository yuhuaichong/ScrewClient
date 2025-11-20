using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;

public class ChangeScriptEncodingFormat
{
    // 添加一个右键菜单。
    // % 按下ctrl时显示菜单。（Windows: control, macOS: command）
    // & 按下alt时显示菜单。(Windows/Linux: alt, macOS: option)
    // _ 按下shift时显示菜单。(Windows/Linux/macOS: shift)
    [MenuItem("Assets/脚本改格式：GB2312->UTF8无BOM %g", false, 100)]
    private static void CustomMenu()
    {
        UnityEngine.Object selectedObject = Selection.activeObject;

        if (selectedObject != null)
        {
            string relativeAssetPath = AssetDatabase.GetAssetPath(selectedObject);
            string projectPath = Path.GetDirectoryName(Application.dataPath);
            string absoluteAssetPath = Path.Combine(projectPath, relativeAssetPath);

            // 判断是文件还是文件夹
            if (Directory.Exists(absoluteAssetPath))
            {
                Debug.Log($"选中的是文件夹：{relativeAssetPath}");
                ProcessDirectory(absoluteAssetPath);
            }
            else
            {
                ProcessSingleFile(absoluteAssetPath);
            }
        }
        else
        {
            Debug.LogWarning("没有选中任何对象.");
        }
    }

    // 如果项目视图中有选中的对象，则启用右键菜单项
    [MenuItem("Assets/脚本改格式：GB2312->UTF8无BOM %g", true)]
    private static bool ValidateCustomMenu()
    {
        return Selection.activeObject != null;
    }

    /// <summary>
    /// 判断该文件是否是CSharp文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static bool IsCSharpFile(string fileName)
    {
        // 获取文件扩展名（包括点）
        string fileExtension = Path.GetExtension(fileName);

        // 将扩展名转换为小写并与 ".cs" 进行比较
        if (fileExtension.ToLower() == ".cs")
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 文件格式转码：检测编码并转换为UTF8
    /// </summary>
    public static void ChangeFormat(string sourceFilePath)
    {
        try
        {
            // 先读取文件的二进制数据
            byte[] fileData = File.ReadAllBytes(sourceFilePath);
            
            // 检测文件的编码
            Encoding detectedEncoding = DetectTextEncoding(fileData);
            
            if (detectedEncoding == null)
            {
                Debug.LogWarning($"无法检测文件编码，默认使用GB2312处理：{sourceFilePath}");
                detectedEncoding = Encoding.GetEncoding("GB2312");
            }

            // 使用检测到的编码读取文本
            string content = detectedEncoding.GetString(fileData);
            
            // 检查文件是否已经是UTF8编码
            if (detectedEncoding.Equals(Encoding.UTF8))
            {
                Debug.Log($"文件已经是UTF8编码，无需转换：{sourceFilePath}");
                return;
            }

            // 转换为UTF8无BOM编码并写入
            File.WriteAllText(sourceFilePath, content, new UTF8Encoding(false));
            Debug.Log($"成功将文件从 {detectedEncoding.EncodingName} 转换为 UTF8：{sourceFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"处理文件时出错 {sourceFilePath}: {ex.Message}");
        }
    }

    /// <summary>
    /// 检测文本文件的编码
    /// </summary>
    private static Encoding DetectTextEncoding(byte[] fileData)
    {
        if (fileData == null || fileData.Length == 0) return null;

        // 检测BOM标记
        if (fileData.Length >= 3)
        {
            // UTF8 BOM: EF BB BF
            if (fileData[0] == 0xEF && fileData[1] == 0xBB && fileData[2] == 0xBF)
                return Encoding.UTF8;
            
            // UTF16 LE BOM: FF FE
            if (fileData[0] == 0xFF && fileData[1] == 0xFE)
                return Encoding.Unicode;
            
            // UTF16 BE BOM: FE FF
            if (fileData[0] == 0xFE && fileData[1] == 0xFF)
                return Encoding.BigEndianUnicode;
        }

        // 尝试检测UTF8编码（无BOM）
        if (IsUtf8(fileData))
            return Encoding.UTF8;

        // 如果不是UTF8，假定为GB2312
        return Encoding.GetEncoding("GB2312");
    }

    /// <summary>
    /// 检查是否是有效的UTF8编码
    /// </summary>
    private static bool IsUtf8(byte[] data)
    {
        int charByteCounter = 1;
        byte curByte;

        for (int i = 0; i < data.Length; i++)
        {
            curByte = data[i];

            if (charByteCounter == 1)
            {
                if (curByte >= 0x80)
                {
                    while (((curByte <<= 1) & 0x80) != 0)
                        charByteCounter++;
                    
                    if (charByteCounter == 1 || charByteCounter > 6)
                        return false;
                }
            }
            else
            {
                if ((curByte & 0xC0) != 0x80)
                    return false;
                
                charByteCounter--;
            }
        }

        return charByteCounter == 1;
    }

    /// <summary>
    /// 处理单个文件
    /// </summary>
    private static void ProcessSingleFile(string absolutePath)
    {
        string fileName = Path.GetFileName(absolutePath);
        if (IsCSharpFile(fileName))
        {
            Debug.Log($"处理文件：{absolutePath}");
            ChangeFormat(absolutePath);
        }
        else
        {
            Debug.Log($"跳过非C#文件：{absolutePath}");
        }
    }

    /// <summary>
    /// 递归处理文件夹
    /// </summary>
    private static void ProcessDirectory(string directoryPath)
    {
        try
        {
            // 获取当前目录下的所有文件
            string[] files = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);
            
            if (files.Length == 0)
            {
                Debug.Log($"文件夹 {directoryPath} 中没有找到C#文件");
                return;
            }

            int processedCount = 0;
            foreach (string file in files)
            {
                ChangeFormat(file);
                processedCount++;
            }
            AssetDatabase.Refresh();
            Debug.Log($"文件夹处理完成，共处理了 {processedCount} 个文件");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"处理文件夹时出错：{ex.Message}");
        }
    }
}
