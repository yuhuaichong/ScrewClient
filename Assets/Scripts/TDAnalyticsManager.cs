using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ThinkingData.Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class TDAnalyticsManager
{
    //开关
#if UNITY_EDITOR 
    bool isOpenTD = true;
#else
    bool isOpenTD = true;
#endif
    public static TDAnalyticsManager tDAnalyticsManager;
    public static TDAnalyticsManager Instance
    {
        get
        {
            if (tDAnalyticsManager == null)
            {
                tDAnalyticsManager = new TDAnalyticsManager();
            }
            return tDAnalyticsManager;
        }
    }

    //玩家id
    private string accoundId = "";
    //游戏版本
    private string GameVersion = "";
    //游戏编号，区分上线平台
    private string GameValue = "";
    public void Load()
    {
        if (GameTool.isNeedCloseMoneyIcon)
        {
            isOpenTD = false;
        }
    }
    bool isSetUseData;
    //游戏加载
    public void GameLoad(string accoundId, string GameVersion, string GameValue, string CountryId)
    {
        //Debug.LogError("GameLoad:"+ accoundId);
        if (!isOpenTD) return;
        if (!isSetUseData)
        {
            isSetUseData = true;
            //设置公共事件属性以后，每个事件都会带有公共事件属性
            Dictionary<string, object> superProperties = new Dictionary<string, object>();
            this.GameVersion = GameVersion;
            superProperties["GameVersion"] = GameVersion;
            superProperties["CountryId"] = CountryId;
            if (this.accoundId == "")
            {
                this.accoundId = accoundId;
                superProperties["account_id"] = accoundId;//字符串
                TDAnalytics.Login(accoundId);
            }
            superProperties["appcode"] = "IOS@CUBE";
            TDAnalytics.SetSuperProperties(superProperties);//设置公共事件属性
            TDAnalytics.UserSet(new Dictionary<string, object>() {
                 { "account_id", this.accoundId },
                { "GameVersion", GameVersion },
                { "CountryId", CountryId },
                { "appcode", "IOS@CUBE" } }); //设置用户属性
        }

        Game_Load();
    }
    //public void UserSetFromAdjust(AdjustSdk.AdjustAttribution adjustAttribution)
    //{
    //    TDAnalytics.UserSet(new Dictionary<string, object>() { { "TrackerToken", adjustAttribution.TrackerToken },
    //        { "TrackerName", adjustAttribution.TrackerName }, { "Network", adjustAttribution. Network},
    //        { "Campaign", adjustAttribution.Campaign } ,{ "Adgroup", adjustAttribution.Adgroup }
    //    ,{ "Creative", adjustAttribution.Creative } ,{ "ClickLabel", adjustAttribution.ClickLabel }
    //    ,{ "CostType", adjustAttribution.CostType }
    //    ,{ "CostAmount", adjustAttribution.CostAmount }
    //    ,{ "CostCurrency", adjustAttribution.CostCurrency } ,{ "FbInstallReferrer", adjustAttribution.FbInstallReferrer } }); //设置用户属性
    //}

    public void SendAdRevenue(string 收入来源, string 收入渠道, string 收入单位, string 收入位置, double 收入金额, double RewardAmount, double InterAmount)
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("Income", 收入来源);
        properties.Add("Channel", 收入渠道);
        properties.Add("Unit", 收入单位);
        properties.Add("Position", 收入位置);
        properties.Add("Amount", 收入金额);
        properties.Add("RewardAmount", RewardAmount);
        properties.Add("InterAmount", InterAmount);
        TDAnalytics.Track("CG_2DScrew_AdRevenue", properties);
                Debug.LogError($"上报广告收入情况:Position:{收入位置},Amount:{收入金额},RewardAmount:{RewardAmount},InterAmount:{InterAmount}");
    }

    //游戏首次加载
    public void GameFirstLoad()
    {
        if (!isOpenTD || CheckSaveKey("GameFirstLoad")) return;
        SetUseData();//设置公共事件属性
        Dictionary<string, object> properties = new Dictionary<string, object>();
        TDAnalytics.Track("CG_2DScrew_Install", properties);
        SavePrefs("GameFirstLoad", "GameFirstLoad");
    }

    private void SetUseData()
    {
        if (!isSetUseData)
        {
            isSetUseData = true;
            //设置公共事件属性以后，每个事件都会带有公共事件属性
            Dictionary<string, object> superProperties = new Dictionary<string, object>();
            this.GameVersion = GameVersion;
            superProperties["GameVersion"] = GameVersion;
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string countryCode = currentCulture.Name.Split("-")[1];
            superProperties["CountryId"] = countryCode;
            if (this.accoundId == "")
            {
                this.accoundId = accoundId;
                superProperties["account_id"] = accoundId;//字符串
                TDAnalytics.Login(accoundId);
            }
            superProperties["appcode"] = "IOS@CUBE";
            TDAnalytics.SetSuperProperties(superProperties);//设置公共事件属性
            TDAnalytics.UserSet(new Dictionary<string, object>()
             {
                 { "account_id", this.accoundId },
                { "GameVersion", GameVersion },
                 { "CountryId", countryCode },
                { "appcode", "IOS@CUBE" }
            }); //设置用户属性
        }
    }

    public void TenJinSetUseData(Dictionary<string, object> dic)
    {
        TDAnalytics.SetSuperProperties(dic);//设置公共事件属性
        TDAnalytics.UserSet(dic);
    }

    //游戏加载
    public void Game_Load()
    {
        if (!isOpenTD) return;
        Dictionary<string, object> properties = new Dictionary<string, object>();
        TDAnalytics.Track("CG_2DScrew_Loading", properties);
        //Debug.LogError("<color=yellow>上报CG_2DScrew_Load</color>");
        if (isDayFirstLoad())
        {
            Dictionary<string, object> properties2 = new Dictionary<string, object>();
            TDAnalytics.Track("CG_2DScrew_DayFirstLoad", properties2);
        }
    }



    float nowMoney = 0;
    //获得金钱
    public void GetMoney(float allMoney)
    {
        if (!isOpenTD) return;
        if (nowMoney == allMoney)
        {
            return;
        }
        nowMoney = allMoney;
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("allMoney", allMoney);
        TDAnalytics.Track("CG_2DScrew_GetMoney", properties);


    }




    //进入游戏上报游戏显示的语言和选择的题库
    public void SendUILanguageAndCounyQuestion(string UILanguage, string PlayerCountryID, bool isFit)
    {
        if (!isOpenTD) return;
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("UILanguage", UILanguage);//UI显示的语言

        properties.Add("PlayerCountryID", PlayerCountryID);//读取手机的地区

        properties.Add("isFit", isFit);//读取手机的地区

        Debug.LogError($"显示的UI语言为:<color=yellow>{UILanguage}</color>, 手机的地区为:<color=yellow>{PlayerCountryID}</color>");

        TDAnalytics.Track("CG_2DScrew_UIAndCountry", properties);
    }

    //激励广告
    public void IncentiveAD(string AdPosition)
    {
        if (!isOpenTD) return;
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("AdPosition", AdPosition);
        TDAnalytics.Track("CG_2DScrew_RewardAD", properties);
        // Debug.LogError($"<color=yellow>上报GuangZhou_ScrewIncentiveAD,激励广告的类型为{Type}</color>");
    }

    //插屏广告
    public void InterstitiaAD(VedioAdType Type)
    {
        if (!isOpenTD) return;
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("AdType", Type.ToString());
        TDAnalytics.Track("CG_2DScrew_InterpolationAD", properties);
        //Debug.LogError("<color=yellow>上报GuangZhou_ScrewInterpolationAD,插屏广告</color>");
    }


    public void ButtonEvent(GameObject buttonObj)
    {
        if (!isOpenTD) return;
        if (buttonObj == null) return;
        List<string> pathList = new List<string>();
        pathList.Add(buttonObj.name);
        Transform trans = buttonObj.transform.parent;
        while (trans.parent != null)
        {
            pathList.Add(trans.gameObject.name);
            trans = trans.parent;
        }
        pathList.Reverse();
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("buttonPath", string.Join("/", pathList));
        TDAnalytics.Track("CG_2DScrew_ButtonClick", properties);
    }

    private bool CheckSaveKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    private void SavePrefs(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    private bool isDayFirstLoad()
    {
        if (CheckSaveKey("DayTime"))
        {
            System.DateTime dateTime = DateTime.Parse(PlayerPrefs.GetString("DayTime"));
            if (DateTime.Now.Date == dateTime.Date)
            {
                return false;
            }
        }
        SavePrefs("DayTime", System.DateTime.Now.ToString());
        return true;
    }



    public void SendEnterLevel(int level, float levelRate = 0)
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("levelname", level);
        properties.Add("levelRate", levelRate);
        TDAnalytics.Track("CG_2DScrew_EnterLevel", properties);
                        Debug.LogError($"玩家进入关卡，上报数据levelname：{level}，levelRate{levelRate}");
    }
    public void SendPassLevel(int level, float levelRate)
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("levelname", level);
        properties.Add("levelRate", levelRate);
        TDAnalytics.Track("CG_2DScrew_PassLevel", properties);
                        Debug.LogError($"玩家通过关卡，上报数据levelname：{level}，levelRate{levelRate}");
    }
    public void SendLoseLevel(int level, float levelRate)
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("levelname", level);
        properties.Add("levelRate", levelRate);
        TDAnalytics.Track("CG_2DScrew_LoseLevel", properties);
                Debug.LogError($"玩家失败，上报数据levelname：{level}，levelRate{levelRate}");
    }

    internal void SendNewUserGuide(int stepIndex)
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("stepIndex", stepIndex);
        TDAnalytics.Track("CG_2DScrew_NewUserGuide", properties);
    }
    public void SendWithdraw()
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();
        TDAnalytics.Track("CG_2DScrew_Withdraw", properties);
    }
}

