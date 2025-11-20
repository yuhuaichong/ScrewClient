using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageMod 
{
    private static readonly LanguageMod _instance = new LanguageMod();
    public static LanguageMod Instance => _instance;
    private SystemLanguage_My currLanguageType;
    public Dictionary<string, string> chineseWithIdl;//代码赋值直接用中文，先遍历配置表获得中文和ID的对应关系 
    public void StartUp()
    {
        chineseWithIdl = new Dictionary<string, string>();
        foreach (var item in ConfigModule.Instance.Tables.TbLanguage.DataList)
        {
            chineseWithIdl.Add(item.ChineseS, item.Sn);
        }
    }


    // 获取指定文本
    public string GetText(string id)
    {
        ConfLanguage conf = ConfigModule.Instance.Tables.TbLanguage.GetOrDefault(id);
        if (conf != null)
        {
            // Debug.LogError(currLanguageType);
            switch (currLanguageType)
            {
                case SystemLanguage_My.ChineseSimplified:
                case SystemLanguage_My.Chinese:
                    return conf.ChineseS;
                //case SystemLanguage_My.ChineseTraditional:
                //    return conf.ChineseT;
                case SystemLanguage_My.English:
                    return conf.English;
                case SystemLanguage_My.Japanese:
                    return conf.Japanese;
                case SystemLanguage_My.Korean:
                    return conf.Korean;
                case SystemLanguage_My.Russian:
                    return conf.Russian;
                case SystemLanguage_My.Portuguese:
                    return conf.Portuguese;
                case SystemLanguage_My.Spanish:
                    return conf.Spanish;
                case SystemLanguage_My.German:
                    return conf.German;
                case SystemLanguage_My.French:
                    return conf.French;
                case SystemLanguage_My.Indonesian:
                    return conf.Indonesia;
                case SystemLanguage_My.Vietnamese:
                    return conf.Vietnamese;
                case SystemLanguage_My.Thai:
                    return conf.Thailand;
                case SystemLanguage_My.Turkish:
                    return conf.Turkey;
                case SystemLanguage_My.India:
                    return conf.India;
                case SystemLanguage_My.Malaysia:
                    return conf.Malaysia;
                default:
                    return conf.English;
            }
        }
        return "";
    }

    public SystemLanguage_My GetLanguage()
    {
        return currLanguageType;
    }

    public void SetLanguage(SystemLanguage_My value)
    {
        currLanguageType = value;
        SaveCache();
        EventManager.Instance.TriggerEvent(GameEvent.OnLanguageChange);
    }
    public void SaveCache()
    {
        PlayerPrefs.SetInt("CurrentLanguage", (int)currLanguageType);
    }
    public void LoadCache()
    {
        currLanguageType = (SystemLanguage_My)PlayerPrefs.GetInt("CurrentLanguage", (int)SystemLanguage_My.Chinese);
    }
    public bool IsArabic()
    {
        return currLanguageType == SystemLanguage_My.Arabic;
    }

}
