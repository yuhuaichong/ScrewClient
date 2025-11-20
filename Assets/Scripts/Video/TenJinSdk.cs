using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.iOS;

public class TenJinSdk : MonoBehaviour
{
    private const string TenjinSdkKey = "ZYJXAWVAMXOXGG3YCMZZBKFCVEZ7WXZK";
    public static TenJinSdk Instance;
    
    private BaseTenjin tenjinInstance;
    private int attributionRetryCount = 0;
    private const int MaxAttributionRetryCount = 3;
    private const float AttributionRetryInterval = 2.5f; // 2-3秒，取中间值2.5秒
    
    // 归因信息上报相关
    private const int MaxAttributionReportCount = 5; // 最多上报5次
    private const float AttributionReportInterval = 5f; // 每5秒上报一次
    private int attributionReportCount = 0;
    private Dictionary<string, object> savedAttributionData = null;
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start() 
    {
        TenjinConnect();
        // 注册监听广告收入事件
        EventManager.Instance.RegisterEvent<TenjinAdRevenueData>(GameEvent.OnAdRevenuePaid, OnAdRevenuePaid);
    }

    void OnDestroy()
    {
        // 取消注册事件
        EventManager.Instance.UnregisterEvent<TenjinAdRevenueData>(GameEvent.OnAdRevenuePaid, OnAdRevenuePaid);
    }

    void OnApplicationPause(bool pauseStatus) 
    {
        if (!pauseStatus) 
        {
            TenjinConnect();
        }
    }

    public void TenjinConnect() 
    {
        tenjinInstance = Tenjin.getInstance(TenjinSdkKey);
        
#if UNITY_IOS
        if (new Version(Device.systemVersion).CompareTo(new Version("14.0")) >= 0)
        {
            tenjinInstance.RequestTrackingAuthorizationWithCompletionHandler((status) => {
                Debug.Log("===> App Tracking Transparency Authorization Status: " + status);
                tenjinInstance.Connect();
                GetAttribution(tenjinInstance);
                // 订阅AppLovin广告收入（自动上报方式，可选）
                // tenjinInstance.SubscribeAppLovinImpressions();
            });
        }
        else
        {
            tenjinInstance.Connect();
            GetAttribution(tenjinInstance);
            // 订阅AppLovin广告收入（自动上报方式，可选）
            // tenjinInstance.SubscribeAppLovinImpressions();
        }
#else
        tenjinInstance.Connect();
        GetAttribution(tenjinInstance);
#endif
    }
    
    void GetAttribution(BaseTenjin instance)
    {
        instance.GetAttributionInfo((Dictionary<string, string> data) =>
        {
            // 检查归因信息是否为空
            if (data == null || data.Count == 0)
            {
                Debug.LogWarning($"Tenjin归因信息为空，当前重试次数：{attributionRetryCount}/{MaxAttributionRetryCount}");
                
                // 如果归因信息为空且未达到最大重试次数，则重试
                if (attributionRetryCount < MaxAttributionRetryCount)
                {
                    attributionRetryCount++;
                    StartCoroutine(RetryGetAttribution(instance));
                }
                else
                {
                    Debug.LogError("Tenjin归因信息获取失败，已达到最大重试次数");
                    attributionRetryCount = 0; // 重置重试计数
                    
                    // 尝试从本地读取归因信息并上报
                    LoadAndReportLocalAttribution();
                }
            }
            else
            {
                // 成功获取归因信息
                Debug.Log("===> Tenjin归因信息获取成功:");
                
                bool hasAdNetwork = false;
                string adNetwork = "";
                Dictionary<string, object> list = new Dictionary<string, object>();
                
                foreach (var obj in data)
                {
                    Debug.Log($"  {obj.Key}: {obj.Value}");
                    
                    if (obj.Key.Equals("ad_network"))
                    {
                        adNetwork = obj.Value.ToLower();
                        hasAdNetwork = true;
                        Debug.Log("===> ad_network: " + obj.Value);
                    }
                    
                    list.Add(obj.Key, obj.Value);
                }
                
                // 检查本地是否已有归因信息
                Dictionary<string, object> localData = LoadAttributionDataFromLocal();
                
                // 如果有ad_network信息，则上报
                if (hasAdNetwork)
                {
                    if (localData != null && localData.Count > 0)
                    {
                        // 本地已有归因信息，使用本地旧的数据
                        Debug.Log("===> 本地已有归因信息，使用本地旧数据上报");
                        savedAttributionData = localData;
                        
                        // 第一次上报
                        ReportAttributionToTD(localData);
                        
                        // 启动定时上报协程（后续上报4次，共5次）
                        attributionReportCount = 1; // 已上报1次
                        StartCoroutine(ReportAttributionPeriodically());
                    }
                    else
                    {
                        // 本地没有归因信息，保存新获取的归因信息
                        Debug.Log("===> 本地没有归因信息，保存新获取的数据");
                        SaveAttributionDataToLocal(list);
                        
                        // 保存到成员变量，用于后续定时上报
                        savedAttributionData = list;
                        
                        // 第一次上报
                        ReportAttributionToTD(list);
                        
                        // 启动定时上报协程（后续上报4次，共5次）
                        attributionReportCount = 1; // 已上报1次
                        StartCoroutine(ReportAttributionPeriodically());
                    }
                }
                else
                {
                    // 当前获取的归因信息中没有ad_network
                    Debug.Log("===> Tenjin归因信息中没有ad_network");
                    
                    // 检查本地是否有旧数据
                    if (localData != null && localData.Count > 0 && localData.ContainsKey("ad_network"))
                    {
                        // 本地有旧数据且包含ad_network，使用本地旧数据上报
                        Debug.Log("===> 检测到本地有旧归因数据，使用本地旧数据上报");
                        savedAttributionData = localData;
                        
                        // 第一次上报
                        ReportAttributionToTD(localData);
                        
                        // 启动定时上报协程（后续上报4次，共5次）
                        attributionReportCount = 1; // 已上报1次
                        StartCoroutine(ReportAttributionPeriodically());
                    }
                    else
                    {
                        Debug.Log("===> 本地也没有可用的归因数据，不进行上报");
                    }
                }
                
                attributionRetryCount = 0; // 重置重试计数
            }
        });
    }
    
    /// <summary>
    /// 重试获取归因信息的协程
    /// </summary>
    IEnumerator RetryGetAttribution(BaseTenjin instance)
    {
        yield return new WaitForSeconds(AttributionRetryInterval);
        
        Debug.Log($"===> Tenjin重试连接和获取归因信息，第{attributionRetryCount}次重试");
        instance.Connect();
        GetAttribution(instance);
    }
    
    /// <summary>
    /// 保存归因信息到本地
    /// </summary>
    private void SaveAttributionDataToLocal(Dictionary<string, object> data)
    {
        try
        {
            // 保存归因信息的键列表
            List<string> keys = new List<string>();
            
            // 将每个键值对保存到PlayerPrefs
            foreach (var kvp in data)
            {
                string key = "TenjinAttribution_" + kvp.Key;
                PlayerPrefs.SetString(key, kvp.Value?.ToString() ?? "");
                keys.Add(kvp.Key);
            }
            
            // 保存键列表，用于后续读取
            PlayerPrefs.SetString("TenjinAttribution_Keys", string.Join(",", keys));
            PlayerPrefs.Save();
            Debug.Log("===> Tenjin归因信息已保存到本地");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存Tenjin归因信息失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 从本地读取归因信息
    /// </summary>
    private Dictionary<string, object> LoadAttributionDataFromLocal()
    {
        try
        {
            // 读取键列表
            string keysString = PlayerPrefs.GetString("TenjinAttribution_Keys", "");
            
            if (string.IsNullOrEmpty(keysString))
            {
                Debug.Log("===> 本地没有保存的归因信息");
                return null;
            }
            
            Dictionary<string, object> data = new Dictionary<string, object>();
            string[] keys = keysString.Split(',');
            
            // 读取每个键值对
            foreach (string key in keys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    string fullKey = "TenjinAttribution_" + key;
                    string value = PlayerPrefs.GetString(fullKey, "");
                    
                    if (!string.IsNullOrEmpty(value))
                    {
                        data.Add(key, value);
                    }
                }
            }
            
            if (data.Count > 0)
            {
                Debug.Log($"===> 从本地读取到 {data.Count} 条归因信息");
            }
            
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"读取本地归因信息失败: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 从本地读取归因信息并上报到数数
    /// </summary>
    private void LoadAndReportLocalAttribution()
    {
        Dictionary<string, object> localData = LoadAttributionDataFromLocal();
        
        if (localData != null && localData.Count > 0)
        {
            // 检查是否有ad_network
            bool hasAdNetwork = localData.ContainsKey("ad_network");
            
            if (hasAdNetwork)
            {
                Debug.Log("===> 使用本地归因信息上报到数数");
                
                // 保存到成员变量
                savedAttributionData = localData;
                
                // 第一次上报
                ReportAttributionToTD(localData);
                
                // 启动定时上报协程（后续上报4次，共5次）
                attributionReportCount = 1; // 已上报1次
                StartCoroutine(ReportAttributionPeriodically());
            }
            else
            {
                Debug.Log("===> 本地归因信息中没有ad_network，不进行上报");
            }
        }
        else
        {
            Debug.LogWarning("===> 本地没有可用的归因信息");
        }
    }
    
    /// <summary>
    /// 上报归因信息到数数分析
    /// </summary>
    private void ReportAttributionToTD(Dictionary<string, object> data)
    {
        try
        {
            TDAnalyticsManager.Instance.TenJinSetUseData(data);
            Debug.Log($"===> Tenjin归因信息已上报到数数分析，第{attributionReportCount}次");
        }
        catch (Exception e)
        {
            Debug.LogError($"上报Tenjin归因信息到数数分析失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 定时上报归因信息的协程
    /// </summary>
    IEnumerator ReportAttributionPeriodically()
    {
        while (attributionReportCount < MaxAttributionReportCount)
        {
            yield return new WaitForSeconds(AttributionReportInterval);
            
            attributionReportCount++;
            
            if (savedAttributionData != null)
            {
                ReportAttributionToTD(savedAttributionData);
            }
            else
            {
                Debug.LogWarning("===> savedAttributionData为空，停止定时上报");
                break;
            }
        }
        
        if (attributionReportCount >= MaxAttributionReportCount)
        {
            Debug.Log($"===> Tenjin归因信息已完成{MaxAttributionReportCount}次上报");
        }
    }
    
    /// <summary>
    /// 广告收入事件回调，用于上报Tenjin
    /// </summary>
    /// <param name="revenueData">广告收入数据</param>
    private void OnAdRevenuePaid(TenjinAdRevenueData revenueData)
    {
        if (tenjinInstance == null)
        {
            Debug.LogWarning("Tenjin实例未初始化，无法上报广告收入");
            return;
        }
        
        // 方法1：使用JsonUtility序列化（推荐）
        TenjinAdImpressionJson impressionData = new TenjinAdImpressionJson(revenueData);
        string json = JsonUtility.ToJson(impressionData);
        
        Debug.Log($"===> Tenjin上报广告收入: {json}");
        
        // 上报给Tenjin
        tenjinInstance.AppLovinImpressionFromJSON(json);
    }
}

/// <summary>
/// Tenjin广告收入数据类，用于EventManager传递
/// </summary>
public class TenjinAdRevenueData
{
    public string adUnitId;
    public string adFormat;
    public string networkName;
    public string placement;
    public double revenue;
    public string currency;
    public string country;
    public string creativeId;
    public string revenuePrecision;
    
    public TenjinAdRevenueData(string adUnitId, string adFormat, string networkName, string placement, double revenue, 
        string currency = "USD", string country = "", string creativeId = "", string revenuePrecision = "")
    {
        this.adUnitId = adUnitId;
        this.adFormat = adFormat;
        this.networkName = networkName;
        this.placement = placement;
        this.revenue = revenue;
        this.currency = currency;
        this.country = country;
        this.creativeId = creativeId;
        this.revenuePrecision = revenuePrecision;
    }
}

/// <summary>
/// Tenjin广告展示JSON数据结构（用于JsonUtility序列化）
/// 注意：字段名必须与Tenjin要求的JSON key完全一致
/// </summary>
[System.Serializable]
public class TenjinAdImpressionJson
{
    public double revenue;
    public string ad_revenue_currency;
    public string country;
    public string network_name;
    public string ad_unit_id;
    public string format;
    public string placement;
    public string network_placement;
    public string creative_id;
    public string revenue_precision;
    
    public TenjinAdImpressionJson(TenjinAdRevenueData data)
    {
        revenue = data.revenue;
        ad_revenue_currency = data.currency;
        country = data.country;
        network_name = data.networkName;
        ad_unit_id = data.adUnitId;
        format = data.adFormat;
        placement = data.placement;
        network_placement = data.placement;
        creative_id = data.creativeId;
        revenue_precision = data.revenuePrecision;
    }
}
