using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ThinkingData.Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    private Transform barTrans;
    private Image progressBar;
    private Text progressText;
    private Text loadingText;
    public AssetReference chineseFontReference;
    public Font ChineseFont;
    AsyncOperationHandle<Font> fontHandle;

    private bool enableBtn = true;
    private float remainingTime;
    private float duration = 0.05f;
    protected void Awake()
    {
        YLocalization.lanaguage = YLocalization.Lanaguage.Chinese;
        barTrans = transform.Find("BG/bar");
        progressBar = barTrans.Find("slider").GetComponent<Image>();
        progressText = barTrans.Find("per").GetComponent<Text>();
        loadingText = barTrans.Find("Loading").GetComponent<Text>();
        //GameObject gmt = new GameObject();
        //gmt.AddComponent<SDKInitializer>();
        //gmt.AddComponent<GMTManager>();
       // DontDestroyOnLoad(gmt);
        DontDestroyOnLoad(transform.parent);

    }

    IEnumerator Start()
    {
        Application.targetFrameRate = 60; // 设置目标帧率为60帧每秒
        // ========== 实际初始化流程 ==========
        // 步骤1: 获取服务器时间
        StartCoroutine(GameTool.TryToGetSceverTime());
        ConfigModule.Instance.StartUp();//读取配置表信息
        LanguageMod.Instance.StartUp();//初始化多语音功能
        yield return GetNeedCloseCoinImage();
        TDAnalyticsManager.Instance.Load();
        GameDataManager.Initialize();//加载数据
        SetNuLLData();//为null值赋初值
        if (string.IsNullOrEmpty(GameDataManager.CurrentGameData.countryID) ||
           !GameTool.IsFitLanguageAndCountyID(GameDataManager.CurrentGameData.currLanguageType, GameDataManager.CurrentGameData.countryID))//如果没有玩家ID,获取国家信息
        {
            yield return GetCountryID();//获取匹配的语言的地区
        }

        yield return CheckNertWork();//获取可以连接的网络      
        LanguageMod.Instance.SetLanguage(GameDataManager.CurrentGameData.currLanguageType);//设置需要显示的语言
        SetLodingIcon();
        SetMoneyIconPath();//设置货币的图标路径
        yield return new WaitForEndOfFrame();
        yield return GameVideoContor.Init();//广告初始化
        //数数上报


        Debug.Log("开始加载资源");
        //await ResourceLoader.Instance.InitializeAsync(default, count =>
        //{
        //    progressText.text = $"{(int)(count * 100)}%";
        //    progressBar.fillAmount = count;
        //});
        //AnimationUtility.FadeOut(barTrans.parent, 1f, () =>
        //{
        //    barTrans.parent.gameObject.SetActive(false);
        //});
        LoadRes();
                TDAnalytics.EnableAutoTrack(TDAutoTrackEventType.AppInstall | TDAutoTrackEventType.AppStart);
        TDAnalytics.EnableAutoTrack(TDAutoTrackEventType.AppEnd,new AutoTrackECB());
        typeof(ExecuteEvents).GetField("s_PointerClickHandler", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, new ExecuteEvents.EventFunction<IPointerClickHandler>(OnPointerClick));
    }

    private void SetLodingIcon()
    {
        Sprite sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/Loding/Loding{(int)(GameDataManager.CurrentGameData.currLanguageType)}.png");
        transform.Find("BG").GetComponent<Image>().sprite = sprite;
        //if (GameDataManager.CurrentGameData.currLanguageType == SystemLanguage_My.English)
        //{
        //    transform.Find("BG/logo").localScale = Vector3.one;
        //}
        //else
        //{
        //    Sprite sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/Loding/Loding{(int)(GameDataManager.CurrentGameData.currLanguageType)}.png");
        //    transform.Find("BG").GetComponent<Image>().sprite = sprite;
        //}
    }

    /// <summary>
    /// 打开网络连接错误界面
    /// </summary>
    public void OpenNetErropPlane()
    {
        if (transform.Find("BG/GameErrotPlane") == null)
        {
            GameObject go = Instantiate(ResourceLoader.Instance.GetResWithPath<GameObject>("Prefab/GameErrotPlane/GameErrotPlane.prefab"), transform.Find("BG"));
            go.name = "GameErrotPlane";
            go.transform.Find("Bg/Des").GetComponent<LanguageText>().text = "网络无法连接，请重试！";
            go.transform.Find("Bg/ScaleButton/LanguageText").GetComponent<LanguageText>().text = "确认";
            go.transform.Find("Bg/Title").GetComponent<LanguageText>().text = "信息";
            go.transform.Find("Bg/ScaleButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                DG.Tweening.Sequence sequence = DOTween.Sequence();

                // 添加缩小动画
                sequence.Append(go.transform.DOScale(Vector3.zero, 0.2f))
                        // 添加等待时间
                        .AppendInterval(0.5f)
                        // 添加变大动画
                        .Append(go.transform.DOScale(Vector3.one * 0.8f, 0.2f));

            });
        }
    }

    IEnumerator CheckNertWork()
    {
        bool isCanGetNet = false;
        while (!isCanGetNet)
        {
            if (GameTool.IsNetworkReachability() || true)
            {
                isCanGetNet = true;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                Debug.LogError("网络可达性检测失败");
                OpenNetErropPlane();
                yield return new WaitForSeconds(5);
            }
        }

    }
    public async void LoadRes()
    {
        await ResourceLoader.Instance.InitializeAsync(default, count =>
        {
            progressText.text = $"{(int)(count * 100)}%";
            progressBar.fillAmount = count;
        });

        AnimationUtility.FadeOut(barTrans.parent, 1f, () =>
        {
            barTrans.parent.gameObject.SetActive(false);
            TDAnalyticsManager.Instance.GameLoad(GameDataManager.CurrentGameData.accoundId, Application.version,
GameTool.GameValue, GameDataManager.CurrentGameData.countryID);
        });
    }


    IEnumerator GetNeedCloseCoinImage()
    {
        bool isGetHttpResult = false;
        string url = "http://www.bbtgame.xyz/xgame?appIndex=19";
        while (!isGetHttpResult)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.LogError(response);
                    // 反序列化
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(response);
                    isGetHttpResult = true;
                    string extStr = responseData.ext;
                    extStr = Regex.Replace(extStr, ",\\s*}", "}");
                    // 现在 extStr 已经是合法 JSON
                    try
                    {
                        ResponseDataA ResponseDataA = JsonUtility.FromJson<ResponseDataA>(extStr);
                        Debug.LogError(ResponseDataA.inOpenInter1);
                        Debug.LogError(ResponseDataA.isOpenInter2);
                        Debug.LogError(ResponseDataA.howManyOpenInter);
                        GameTool.IsOpenOnlyWightIder = ResponseDataA.inOpenInter1;
                        GameTool.IsOpenLosetIder = ResponseDataA.isOpenInter2;
                        GameTool.howManyOpenInter = ResponseDataA.howManyOpenInter;
                        GameTool.isNeedCloseMoneyIcon = responseData.data;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("解析服务器AB面开关失败");
                        GameTool.IsOpenOnlyWightIder = true;
                        GameTool.IsOpenLosetIder = false;
                        GameTool.howManyOpenInter = 3;
                        GameTool.isNeedCloseMoneyIcon = false;
                    }
                }
            }
            if (!isGetHttpResult)
            {
                Debug.LogError("HTTP获取AB面开关失败");
                OpenNetErropPlane();
                yield return new WaitForSeconds(5);
            }
        }
    }

    void OnPointerClick(IPointerClickHandler handler, BaseEventData eventData)
    {
        PointerEventData pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
        if (pointerEventData != null)
        {
            if (!enableBtn) return;

            handler.OnPointerClick(pointerEventData);
            enableBtn = false;
            DOTween.To(() => remainingTime, x => remainingTime = x, 0, duration)
           .SetEase(Ease.Linear).OnComplete(
                delegate () {
                    enableBtn = true;
                }
                );
            EventManager.Instance.TriggerEvent(GameEvent.OpenClikAudio);
            TDAnalyticsManager.Instance.ButtonEvent(pointerEventData.selectedObject);
        }
    }
    private void SetMoneyIconPath()
    {
        GameTool.GetDollarIconAndNum(1);
        string coinPath = "";
        if (GameTool.confPayRegion != null)
        {
            coinPath = GameTool.confPayRegion.CurrencyPath;
        }
        else
        {
            coinPath = "1";
        }
        GameTool.dollarIconPath = coinPath;
        if (GameTool.isNeedCloseMoneyIcon)
        {
            GameTool.dollarIconPath = "AA";
        }
    }

    /// <summary>
    /// 为null值赋初值
    /// </summary>
    private void SetNuLLData()
    {
        if (GameDataManager.CurrentGameData.taskCompleteDci == null)
        {
            GameDataManager.CurrentGameData.taskCompleteDci = new System.Collections.Generic.Dictionary<int, bool>();
        }
        if (GameDataManager.CurrentGameData.taskLevelCompleteDci == null)
        {
            GameDataManager.CurrentGameData.taskLevelCompleteDci = new System.Collections.Generic.Dictionary<int, bool>();
        }
        if (GameDataManager.CurrentGameData.levelUpOnState == null)
        {
            GameDataManager.CurrentGameData.levelUpOnState = new System.Collections.Generic.Dictionary<int, bool>();
        }
        if (!GameDataManager.CurrentGameData.isLongInGame)
        {
            GameDataManager.CurrentGameData.isLongInGame = true;
            GameDataManager.CurrentGameData.oneLogingInGameTime = DateTime.Now;
            GameDataManager.CurrentGameData.dayGiftGetStatu = new System.Collections.Generic.Dictionary<int, bool>
            {
                {1,false },
                {2,false },
                {3,false },
                {4,false },
                {5,false },
                {6,false },
                {7,false },
            };
        }
        if (GameDataManager.CurrentGameData.dayLoginRewardCompleteDci == null)
        {
            GameDataManager.CurrentGameData.dayLoginRewardCompleteDci = new System.Collections.Generic.Dictionary<int, bool>();
        }
        if (GameDataManager.CurrentGameData.withDrawScheduleList == null)
        {
            GameDataManager.CurrentGameData.withDrawScheduleList = new List<WithDrawSchedule>();
        }
        if (GameDataManager.CurrentGameData.withDrawTaskCompleteDci == null)
        {
            GameDataManager.CurrentGameData.withDrawTaskCompleteDci = new System.Collections.Generic.Dictionary<int, bool>();
        }
        if (GameDataManager.CurrentGameData.taskWithDrawDci == null)
        {
            GameDataManager.CurrentGameData.taskWithDrawDci = new System.Collections.Generic.Dictionary<int, bool>();
        }

    }
    /// <summary>
    /// 马来西亚和印度单独设置
    /// </summary>
    /// <param name="countryID"></param>
    private void MaLayAndYinNiSeting(string countryID)
    {
        switch (countryID)
        {
            case "IN"://印度
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.India;
                break;
            case "MY"://马来西亚
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Malaysia;
                break;
        }

    }
    IEnumerator GetCountryID()
    {

        bool isFit = false;

        while (!isFit)
        {
            GameDataManager.CurrentGameData.currLanguageType = getLocalLanguage();
            if (string.IsNullOrEmpty(GameDataManager.CurrentGameData.countryID))
            {
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                string countryCode = currentCulture.Name.Split("-")[1];
                if (countryCode == "PT")
                {
                    countryCode = "BR";
                }
                GameDataManager.CurrentGameData.countryID = countryCode;
                // MaLayAndYinNiSeting(GameDataManager.CurrentGameData.countryID);
                Debug.Log($"国家: {GameDataManager.CurrentGameData.countryID})");
            }
            else
            {
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                string countryCode = currentCulture.Name.Split("-")[1];
                if (countryCode == "IN")
                {
                    GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.India;
                    GameDataManager.CurrentGameData.countryID = "IN";
                }
                else if (countryCode == "MY")
                {
                    GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Malaysia;
                    GameDataManager.CurrentGameData.countryID = "MY";
                }
            }

            if (GameTool.IsFitLanguageAndCountyID(GameDataManager.CurrentGameData.currLanguageType, GameDataManager.CurrentGameData.countryID))
            {
                isFit = true;
            }
            else
            {
                Debug.LogError("地区匹配失败");
                OpenNetErropPlane();//打开网络连接错误界面
                isFit = false;
                yield return new WaitForSeconds(5);
            }
        }



        yield return new WaitForEndOfFrame();
    }
    private void SetLanaguageType(string countryID)
    {
        switch (countryID)
        {
            case "JP":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Japanese;
                break;
            case "BR":
            case "PT":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Portuguese;
                break;
            case "KR":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Korean;
                break;
            case "DE":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.German;
                break;
            case "FR":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.French;
                break;
            case "ES":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Spanish;
                break;
            case "ID":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Indonesian;
                break;
            case "RU":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Russian;
                break;
            case "IN"://印度
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.India;
                break;
            case "VN":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Vietnamese;
                break;
            case "MY"://马来西亚
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Malaysia;
                break;
            case "TH":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Thai;
                break;
            case "TR":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.Turkish;
                break;
            case "CN":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.ChineseSimplified;
                break;
            case "HK":
            case "TW":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.ChineseTraditional;
                break;
            case "US":
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.English;
                break;
            default:
                GameDataManager.CurrentGameData.currLanguageType = SystemLanguage_My.English;
                break;
        }
    }
    [System.Serializable]
    public class IPData
    {
        public string ip;
        public string country;
        public string country_name;
        // 可以根据需要添加其他字段
    }
    private SystemLanguage_My getLocalLanguage()
    {
        switch ((int)Application.systemLanguage)
        {
            case (int)SystemLanguage_My.English:
            //case (int)SystemLanguage_My.Japanese:
            case (int)SystemLanguage_My.Portuguese:
                //case (int)SystemLanguage_My.Spanish:
                //case (int)SystemLanguage_My.Korean:
                //case (int)SystemLanguage_My.German:
                //case (int)SystemLanguage_My.French:
                //case (int)SystemLanguage_My.Indonesian:
                //case (int)SystemLanguage_My.Russian:
                //case (int)SystemLanguage_My.Vietnamese:
                //case (int)SystemLanguage_My.Thai:
                //case (int)SystemLanguage_My.Turkish:

                return (SystemLanguage_My)(int)Application.systemLanguage;
        }
        GameDataManager.CurrentGameData.countryID = "Other";
        return SystemLanguage_My.English;
    }
    private async Task LoadChineseFontAsync()
    {
        if (chineseFontReference != null)
        {
            try
            {
                // 异步加载字体资源
                fontHandle = chineseFontReference.LoadAssetAsync<Font>();

                // 等待加载完成
                Font loadedFont = await fontHandle.Task;

                // 将字体赋值给UI Text组件
                if (loadingText != null)
                {
                    loadingText.font = loadedFont;
                }
                if (progressText != null)
                {
                    progressText.font = loadedFont;
                }


            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load Chinese font: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Chinese font reference is not set.");
        }
    }
}
[System.Serializable]
public class ResponseData
{
    public int code;
    public string msg;
    public bool data;
    public string ext; // 先用string
}

[System.Serializable]
public class ResponseDataA
{
    public bool inOpenInter1;
    public bool isOpenInter2;
    public int howManyOpenInter;
}

