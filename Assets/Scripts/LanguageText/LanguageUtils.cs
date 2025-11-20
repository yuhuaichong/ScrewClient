using Bright.Serialization;
using cfg;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//多语言工具类
public class LanguageUtils
{
    private static TbLanguage languageTab;
    public static TbLanguage LanguageTab
    {
        get
        {
            if (languageTab == null) languageTab = LoadConf();
            return languageTab;
        }
    }

    private static TbLanguage LoadConf()
    {
        languageTab = new TbLanguage(LoadByteBuf("tblanguage"));
        return languageTab;
    }
    private static ByteBuf LoadByteBuf(string file)
    {
        byte[] bytes;
        bytes = File.ReadAllBytes($"{Application.streamingAssetsPath}/Data/tblanguage.bytes");
        return new ByteBuf(bytes);
    }

    private static SystemLanguage_My languageType = SystemLanguage_My.Unknown;
    public static SystemLanguage_My GetLanguage()
    {
        if (languageType == SystemLanguage_My.Unknown)
        {
            LoadCache();
        }
        return languageType;
    }

    public static void SetLanguage(SystemLanguage_My value)
    {
        languageType = value;
    }
    public static void SaveCache()
    {
        PlayerPrefs.SetInt("CurrentLanguage", (int)languageType);
    }
    public static void LoadCache()
    {
        var cur_language = (SystemLanguage_My)PlayerPrefs.GetInt("CurrentLanguage", (int)SystemLanguage_My.Unknown);
        SetLanguage(cur_language);
    }
    public static void ClearCache()
    {
        PlayerPrefs.DeleteKey("CurrentLanguage");
    }
    public static bool IsArabic()
    {
        return languageType == SystemLanguage_My.Arabic;
    }


    public static void ReloadConfig()
    {
        languageTab = null;
        LoadConf();
    }

    public static string GetLanguage(string id)
    {
        ConfLanguage conf = LanguageTab.GetOrDefault(id);
        if (conf != null)
        {
            switch (GetLanguage())
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
        return $"配置错误：多语言表中id: {id} 不存在";
    }

    //自动清理
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoClearCache()
    {
        languageTab = null;
    }
}