using Bright.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public  class ConfigModule
{
    private static readonly ConfigModule _instance = new ConfigModule();
    public static ConfigModule Instance => _instance;

    public cfg.Tables Tables;

    public void StartUp()
    {

         Tables = new cfg.Tables(LoadByteBuf);
    }
    private static ByteBuf LoadByteBuf(string file)
    {
        string path = file.ToLower().Replace('.', '_');
        byte[] bytes = null;
        
#if UNITY_ANDROID && !UNITY_EDITOR
        // Android平台使用UnityWebRequest（因为文件在APK内部）
        string filePath = Path.Combine(Application.streamingAssetsPath, "Data", path + ".bytes");
        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
        {
            www.SendWebRequest();
            while (!www.isDone) { }
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                bytes = www.downloadHandler.data;
                Debug.Log($"[ConfigModule] 成功加载配置文件: {path}");
            }
            else
            {
                Debug.LogError($"[ConfigModule] 加载配置文件失败: {path}, 错误: {www.error}");
            }
        }
#else
        // iOS和Editor平台直接读取文件
        string filePath = Path.Combine(Application.streamingAssetsPath, "Data", path + ".bytes");
        try
        {
            if (File.Exists(filePath))
            {
                bytes = File.ReadAllBytes(filePath);
                Debug.Log($"[ConfigModule] 成功加载配置文件: {path}");
            }
            else
            {
                Debug.LogError($"[ConfigModule] 配置文件不存在: {filePath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ConfigModule] 读取配置文件异常: {path}, 错误: {e.Message}");
        }
#endif
        
        if (bytes == null)
        {
            Debug.LogError($"[ConfigModule] 配置文件加载失败，返回空数据: {path}");
        }
        
        return new ByteBuf(bytes);
    }
}
