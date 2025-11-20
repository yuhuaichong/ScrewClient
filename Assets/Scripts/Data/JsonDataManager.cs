using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 单文件数据存储管理类
/// </summary>
public static class JsonDataManager
{
    // 数据默认存储路径（使用 Application.persistentDataPath，适合跨平台）
    private static readonly string DataFolderPath = Path.Combine(Application.persistentDataPath, "Data");
    private static  string GameDataFilePath = Path.Combine(DataFolderPath, "gamedata.json");
    public static readonly string TimeDataFilePath = Path.Combine(DataFolderPath, "timedata.json");

    /// <summary>
    /// 保存数据到指定路径
    /// </summary>
    /// <typeparam name="T">保存的数据类型</typeparam>
    /// <param name="data">数据对象</param>
    /// <param name="filePath">文件路径</param>
    public static void SaveData<T>(T data, string filePath)
    {
        EnsureDataFolderExists();

        try
        {
            string jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, jsonData);
            //Debug.Log($"[JsonDataManager] 数据已成功保存到: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonDataManager] 保存数据失败: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 从指定路径加载数据
    /// </summary>
    /// <typeparam name="T">加载的数据类型</typeparam>
    /// <param name="filePath">文件路径</param>
    /// <returns>加载的数据对象；文件不存在或加载失败时返回 null</returns>
    public static T LoadData<T>(string filePath) where T : class
    {
        EnsureDataFolderExists();
        return null;

        if (File.Exists(filePath))
        {
            try
            {
                string jsonData = File.ReadAllText(filePath);
                
                T data =JsonConvert.DeserializeObject<T>(jsonData);
                Debug.Log($"[JsonDataManager] 数据已成功加载: {filePath}");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonDataManager] 加载数据失败: {e.Message}\n{e.StackTrace}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"[JsonDataManager] 数据文件未找到: {filePath}");
            return null;
        }
    }

    /// <summary>
    /// 确保数据存储文件夹存在
    /// </summary>
    private static void EnsureDataFolderExists()
    {
        if (!Directory.Exists(DataFolderPath))
        {
            Directory.CreateDirectory(DataFolderPath);
            Debug.Log($"[JsonDataManager] 已创建数据文件夹: {DataFolderPath}");
        }
    }

    #region 游戏数据
    /// <summary>
    /// 保存游戏数据
    /// </summary>
    public static void SaveGameData(GameData data)
    {
        SaveData(data, GameDataFilePath);
    }

    /// <summary>
    /// 加载游戏数据
    /// </summary>
    public static GameData LoadGameData()
    {
        GameDataFilePath=  Path.Combine(DataFolderPath, $"{GameTool.isNeedCloseMoneyIcon}gamedata.json");
        return LoadData<GameData>(GameDataFilePath);
    }

    #endregion

    #region 时间数据
    /// <summary>
    /// 保存时间数据
    /// </summary>
    public static void SaveTimeData(TimeData data)
    {
        SaveData(data, TimeDataFilePath);
    }

    /// <summary>
    /// 加载时间数据
    /// </summary>
    public static TimeData LoadTimeData()
    {
        return LoadData<TimeData>(TimeDataFilePath);
    }

    #endregion
}
