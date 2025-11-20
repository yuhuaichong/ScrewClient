using cfg;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DafultScript;
public static class GameTool 
{
    public static string GameValue = "ChangSha_Screw";
    public static  int OneScrowGetCoinNum = 50;//收集一个螺丝给金钱
    public static  int haoManyBoxGetDollar = 9;//收集多少个箱子，触发金钱广告
    public static int collectHowManeyBoxGetOneLuck = 20;//收集多少箱子获得一次抽奖机会
    //public static int collectHowManeyBoxShowBubble = 21;//收集多少箱子生成一个飞行宝箱
    public static string dollarIconPath;//货币的图片路径标识
    public static int getOnePropNeedCoin = 100;//购买一个道具需要多少金币
    public static ConfPayRegion confPayRegion;//地区货币信息
    public static bool isNeedCloseMoneyIcon = false;//是否需要隐藏金钱图标
    public static int howManeyToOpenAppraisePlane= 90;//收集多少个箱子打开评价界面
    public static int UICangetCoinNum=3000;//收集盒子一定数量弹出界面获得金币
    public static int minCoinNum = 10;//可以获得的最小值
    public static int maxCoinNum = 300;//可以获得的最小值
    public static int GiftCanGetCoin = 40;//飞行宝箱可以获得的钻石数量
    public static int limtPro2Count = 1;//道具2使用限制次数
    public static int limtPro3Count = 2;//道具3使用限制次数
    public static float ChaneGameHard = 97f;
    public static bool IsOpenOnlyWightIder;//是否打开弹窗奖励的插屏广告
    public static bool IsOpenLosetIder;//是否打开失败的插屏广告
    public static int howManyOpenInter;//几个奖励界面打开插屏广告
    public static int maxLevelNum = 4;//最大关卡
    public static bool isWinIng;//正在胜利中
    public static int reviveNeedCoin = 500;//复活一次需要多少金币
    public static bool isShowTaskUI = false;//这次登录是否显示任务界面
    public static int withDrawTaskIndex = -1;//领取任务的索引
    internal static WithDrawSchedule withDrawSchedule;//当前提现记录
    public static int nowLevel
    {
        get
        {
            if (LevelManager.Instance != null)
            {
                if (LevelManager.Instance.levelNum == -1 && GameDataManager.CurrentGameData != null)
                {
                    return GameDataManager.CurrentGameData.levelNum;
                }
                return LevelManager.Instance.levelNum;
            }
            else if (GameDataManager.CurrentGameData != null)
            {
                return GameDataManager.CurrentGameData.levelNum;
            }
            return -1;
        }
    }//当前关卡
    public static int nowProgress
    {
        get
        {
            if (UIManager.Instance != null)
            {
                return UIManager.Instance.GetUI<PopGameSlidePlane>().AwalCanGetValue();
            }
            return -1;
        }
    }//当前进度
    /// <summary>
    /// 根据金钱，返回玩家所在地区的金币符号金钱
    /// </summary>
    /// <param name="moneyNum">美元数量</param>
    /// <param name="Decimal">小数点保留位数</param>
    /// <returns></returns>
    public static string GetDollarIconAndNum(float moneyNum, int Decimal = -1)
    {
        if (confPayRegion == null)
        {
            if (GameDataManager.CurrentGameData.countryID == "PT")
            {
                GameDataManager.CurrentGameData.countryID = "BR";
            }
            confPayRegion = ConfigModule.Instance.Tables.TbPayRegion.GetOrDefault(GameDataManager.CurrentGameData.countryID);
            if (confPayRegion == null)//PayRegion表内没有这个国家的支付信息，默认为Others
            {
                confPayRegion = ConfigModule.Instance.Tables.TbPayRegion.GetOrDefault("Others");
            }
        }
        if (confPayRegion != null)
        {
            if (Decimal == -1)
            {
                if (isNeedCloseMoneyIcon)
                {
                    return $"{(moneyNum * confPayRegion.ExchangeRate).ToString($"F{confPayRegion.Decimal}")}";
                }
                else
                {
                    return $"{confPayRegion.CurrencyMark}{(moneyNum * confPayRegion.ExchangeRate).ToString($"F{confPayRegion.Decimal}")}";
                }
            }
            else
            {
                if (isNeedCloseMoneyIcon)
                {
                    return $"{(moneyNum * confPayRegion.ExchangeRate).ToString($"F{Decimal}")}";
                }
                else
                {
                    return $"{confPayRegion.CurrencyMark}{(moneyNum * confPayRegion.ExchangeRate).ToString($"F{Decimal}")}";
                }
            }
        }
        else
        {
            if (isNeedCloseMoneyIcon)
            {
                return $"{moneyNum:F2}";
            }
            else
            {
                return $"${moneyNum:F2}";
            }
        }

    }

    public static Sprite GetNormalCountryMoneyIcon()
    {
        return ResourceLoader.Instance.GetUnlockImageSprite($"coin_{dollarIconPath}");
        //return ResourceLoader.Instance.GetUnlockImageSprite("boom");
    }

    /// <summary>
    /// 收集箱子奖励的给的美元数量
    /// </summary>
    /// <returns></returns>
    internal static float GetBowRewardDollar()
    {
        return 2.1f;//先固定给2.1美元
    }
    public static string ReturnColorText(object text,string color)
    {
        return $"<color=#{color}>{text}</color>";
    }
    internal static bool CheckIsOneDay(DateTime nowTime, DateTime loginTime)
    {
        // 判断两者的年、月和日是否相同
        return nowTime.Year == loginTime.Year &&
               nowTime.Month == loginTime.Month &&
               nowTime.Day == loginTime.Day;
    }

    internal static int GetNowDay(DateTime now, DateTime oneLogingInGameTime)
    {
        DateTime dateNow = now.Date;
        DateTime dateLogin = oneLogingInGameTime.Date;

        int daysDifference = (dateNow - dateLogin).Days;

        // 返回天数差+1，且天数差负数时返回1（可根据需求调整）
        if (daysDifference < 0)
            return 1;  // 如果 now 早于登录时间，按同一天处理或改为其他逻辑

        return daysDifference + 1;
    }
    /// <summary>
    /// 给一个UI位置然后，生成一定数量的2d物体的动画
    /// </summary>
    /// <param name="uiPos">ui的位置</param>
    /// <param name="icon">图标路径</param>
    /// <param name="num">数量</param>
    public static void UIGetAPosAndCreat2DObjToThisPosAnimator(RectTransform uiPos, string icon, int num, Action action = null, RectTransform startUIPos = null)
    {
        // 获取UI元素的世界坐标位置作为目标位置
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, uiPos.transform.position);
        Vector3 targetWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0));
        targetWorldPoint.z = 0;

        // 获取起始位置（如果提供了startUIPos就用它，否则使用屏幕中心）
        Vector3 startWorldPoint;
        if (startUIPos != null)
        {
            // 将起始UI位置转换为世界坐标
            Vector2 startScreenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, startUIPos.transform.position);
            startWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(startScreenPoint.x, startScreenPoint.y, 0));
            startWorldPoint.z = 0;
        }
        else
        {
            // 默认使用屏幕中心点
            startWorldPoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        }

        int coinCount = num; // 增加金币数量，使效果更丰富
        List<GameObject> coins = new List<GameObject>();

        Sprite sprite=ResourceLoader.Instance.GetUnlockImageSprite(icon);
        // 创建多个金币并设置初始位置
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coin = GameObject.Instantiate(GameAnimatorContor.Instance.DollarIcon.gameObject);
            coin.gameObject.SetActive(true);
            coins.Add(coin);
            coin.GetComponent<SpriteRenderer>().sprite = sprite;
            // 设置初始位置（在起始点附近随机位置）
            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-0.5f, 0.5f),
                UnityEngine.Random.Range(-0.5f, 0.5f),
                0
            );
            coin.transform.position = startWorldPoint + randomOffset;

            // 计算随机散开方向和距离
            float randomAngle = UnityEngine.Random.Range(0f, 360f);
            float randomRadius = UnityEngine.Random.Range(1f, 3f); // 随机散开距离
            Vector3 offset = new Vector3(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomRadius,
                Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomRadius,
                0
            );

            // 创建散开的动画序列
            DG.Tweening.Sequence sequence = DOTween.Sequence();

            // 第一段：散开动画（时间随机）
            float scatterTime = UnityEngine.Random.Range(0.4f, 0.6f);
            sequence.Append(coin.transform.DOMove(coin.transform.position + offset, scatterTime)
                .SetEase(Ease.OutCirc));

            // 第二段：随机等待时间
            sequence.AppendInterval(UnityEngine.Random.Range(0.1f, 0.3f));

            // 第三段：聚集到目标位置（时间随机）
            float gatherTime = UnityEngine.Random.Range(0.5f, 0.7f);
            sequence.Append(coin.transform.DOMove(targetWorldPoint, gatherTime)
                .SetEase(Ease.InOutQuad));
        }

        // 等待所有金币动画完成后更新UI和销毁金币
        DOVirtual.DelayedCall(1.5f, () =>
        {
            // 销毁所有金币
            foreach (var coin in coins)
            {
                GameObject.Destroy(coin);
            }

            action?.Invoke();

            // UI反馈动画
            uiPos.transform.DOScale(Vector3.one * 1.2f, 0.1f)
                .OnComplete(() =>
                {
                    uiPos.transform.DOScale(Vector3.one, 0.1f);
                });
        });
    }
    /// <summary>
    /// 获得item表内的物品
    /// </summary>
    /// <param name="confItem">获得的item</param>
    /// <param name="getRewardNum">获得的数量</param>
    /// <param name="rectTransform">动画的起始点</param>
    internal static void GetItem(ConfItem confItem, float getRewardNum, RectTransform rectTransform)
    {
        RectTransform endPos = MainSceneUI.Instance._GamePlayUI.GetItemEndPos(confItem);
        int newShowAnimatorIconNum=0;
        if(confItem.Sn==1 || confItem.Sn == 2)
        {
            newShowAnimatorIconNum = 10;
        }
        else
        {
            newShowAnimatorIconNum = (int)getRewardNum;
        }
        string getIcon = "";
        if (confItem.Sn == 2)
        {
            getIcon = $"coin_{dollarIconPath}";
        }
        else
        {
            getIcon = confItem.GetIcon;
        }
        if (confItem.Sn == 1)
        {
            DOVirtual.DelayedCall(0.4f, ()=>
            {
                AudioManager.Instance.PlaySFX("getCoin");
            });

        }
        UIGetAPosAndCreat2DObjToThisPosAnimator(endPos, getIcon, newShowAnimatorIconNum, delegate ()
        {
            GameDataManager.AddItemNum(confItem, getRewardNum);
        }, rectTransform);
    }
    public static ScrewColor DetermineScrewColor(string spriteName)
    {
        if (spriteName.Contains("light_blue")) return ScrewColor.LightBlue;
        else if (spriteName.Contains("light_purple")) return ScrewColor.LightPurple;
        else if (spriteName.Contains("blue")) return ScrewColor.Blue;
        else if (spriteName.Contains("gray")) return ScrewColor.Grey;
        else if (spriteName.Contains("red")) return ScrewColor.Red;
        else if (spriteName.Contains("yellow")) return ScrewColor.Yellow;
        else if (spriteName.Contains("purple")) return ScrewColor.Purple;
        else if (spriteName.Contains("pink")) return ScrewColor.Pink;
        else if (spriteName.Contains("orange")) return ScrewColor.Orange;
        else if (spriteName.Contains("green")) return ScrewColor.Green;

        return ScrewColor.Grey; // 默认值
    }

    public static void CreatTip(string v)
    {
       // EventManager.Instance.TriggerEvent(GameEvent.ShowTip, v);
        UIManager.Instance.ShowUI<AlertUI>();
        UIManager.Instance.GetUI<AlertUI>().SetAlertText(v);
    }

    internal static string GetColorPath(ScrewColor screwColor)
    {
       switch(screwColor)
       {
            case ScrewColor.Blue:
            return "蓝色";

                    case ScrewColor.Red:
            return "红色";

                    case ScrewColor.Green:
            return "绿色";

                    case ScrewColor.Grey:
            return "灰色";

                    case ScrewColor.LightBlue:
            return "浅蓝色";

                    case ScrewColor.Orange:
            return "橙色";

                    case ScrewColor.Purple:
            return "紫色";

                    case ScrewColor.Yellow:
            return "黄色";

                    case ScrewColor.Pink:
            return "粉色";

                    case ScrewColor.LightPurple:
            return "棕色";
            default:
            return "棕色";
       }
    }

    internal static Vector3 GetBoxPos(int i)
    {
        if (i == 0)
        {
            return new Vector3(0, 0.18f, 0);
        }
        else if (i == 1)
        {
            return new Vector3(-0.59f, -0.19f, 0);
        }
        else if (i == 2)
        {
            return new Vector3(0.59f, -0.19f, 0);
        }
        return Vector3.zero;
    }

    internal static Color GetScrewLineRenderColor(ScrewColor color)
    {
        switch (color)
        {
            case ScrewColor.Red:
                return Color.red;
            case ScrewColor.Blue:
                return Color.blue;
            case ScrewColor.Pink:
                return Color.magenta; // Unity没有直接的粉色，可以使用常见的颜色替代
            case ScrewColor.Yellow:
                return Color.yellow;
            case ScrewColor.Purple:
                return new Color(0.5f, 0f, 0.5f); // 自定义紫色
            case ScrewColor.Orange:
                return new Color(1f, 0.5f, 0f); // 自定义橙色
            case ScrewColor.Green:
                return Color.green;
            case ScrewColor.Grey:
                return Color.grey;
            case ScrewColor.LightBlue:
                return new Color(0.68f, 0.85f, 0.9f); // 自定义浅蓝色
            case ScrewColor.LightPurple:
                return new Color(0.65f, 0.16f, 0.16f); // 自定义棕色
            default:
                return Color.white; // 默认返回白色
        }
    }

    internal static float GetBoxComplete(int levelNum)
    {

        if (!IsNetworkReachability() )
        {
            return 0;
        }
        float canGetMoney = 0;
        if(!isNeedCloseMoneyIcon)
        {
            // 计算奖励
            canGetMoney = 0.1f * Mathf.Pow(2, levelNum - 1);
        }
        else
        {
            // 计算奖励
            canGetMoney = 10f * Mathf.Pow(2, levelNum - 1);
        }
       // Debug.LogError($"收集箱子获得奖励是{canGetMoney}");
        return canGetMoney;
    }

    internal static bool IsCheckTele(string text)
    {
        if(text.Length < 5 || text.Length > 13)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    internal static bool IsCheckEmail(string text)
    {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!regex.IsMatch(text))
        {
            GameTool.CreatTip("请检查信息（输入框无内容时的提示信息）");
            return false;
        }
        else
        {
            return true;
        }
    }

    public static void SetOutLine(RectTransform par)
    {
        // 确保传递的父物体不为空
        if (par == null) return;

        // 遍历父物体下的所有子节点
        foreach (Transform child in par)
        {
            // 获取当前子节点的 LanguageText 组件
            LanguageText languageText = child.GetComponent<LanguageText>();

            if (languageText != null)
            {
                // 获取当前子节点的所有 Outline 组件
                Outline[] outlines = languageText.GetComponents<Outline>();

                if (outlines.Length > 0)
                {
                    foreach (Outline outline in outlines)
                    {
                        // 检查 Outline 的 distance
                        if (outline.effectDistance.x > 2 || outline.effectDistance.y > 2)
                        {
                            // 设置 Outline 的 distance 为 (2, -2)
                            outline.effectDistance = new Vector2(2, -2);
                        }
                    }

                    // 如果只挂载了一个 Outline 组件
                    if (outlines.Length == 1)
                    {
                        // 创建一个新的 Outline 组件
                        Outline newOutline = languageText.gameObject.AddComponent<Outline>();
                        newOutline.effectColor = outlines[0].effectColor;
                        newOutline.effectDistance = outlines[0].effectDistance; // 使用相同的 distance
                    }
                }
            }

            // 递归调用以遍历子节点
            SetOutLine(child as RectTransform);
        }
    }

    internal static Sprite GetBoxNap(Box box)
    {
        string napPath = "";

        // 根据 Box 的颜色生成路径
        switch (box.BoxColor)
        {
            case ScrewColor.Red:
                napPath = "红色-盖子";
                break;
            case ScrewColor.Blue:
                napPath = "蓝色-盖子";
                break;
            case ScrewColor.Pink:
                napPath = "粉色-盖子"; // 自定义颜色名
                break;
            case ScrewColor.Yellow:
                napPath = "黄色-盖子";
                break;
            case ScrewColor.Purple:
                napPath = "紫色-盖子";
                break;
            case ScrewColor.Orange:
                napPath = "橙色-盖子";
                break;
            case ScrewColor.Green:
                napPath = "绿色-盖子";
                break;
            case ScrewColor.Grey:
                napPath = "灰色-盖子";
                break;
            case ScrewColor.LightBlue:
                napPath = "浅蓝色-盖子";
                break;
            case ScrewColor.LightPurple:
                napPath = "棕色-盖子";
                break;
            default:
                napPath = "浅蓝色-盖子"; // 默认情况下的盖子路径
                break;
        }

        // 拼接最终的路径
        Sprite sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/BoxImage/{napPath}.png");
        return sprite;
    }

    internal static string GetTodayDaye()
    {
        // 获取当前日期
        DateTime today = DateTime.Today;

        // 格式化日期为 (MM/dd/yyyy)
        string formattedDate = today.ToString("M/d/yyyy");

        return formattedDate;
    }

    internal static float GetTodayComNum()
    {
        // 获取当前时间
        DateTime currentTime = DateTime.Now;

        // 检查是否需要重新生成随机数据
        if (GameDataManager.CurrentGameData.randNumTime == default(DateTime) ||
            (currentTime - GameDataManager.CurrentGameData.randNumTime).TotalHours >= 1)
        {
            GameDataManager.CurrentGameData.randNumTime=DateTime.Now;
            GameDataManager.CurrentGameData.num1 = UnityEngine.Random.Range(10000, 12000);
            GameDataManager.CurrentGameData.num2 = UnityEngine.Random.Range(10, 50);
            GameDataManager.CurrentGameData.num3 = UnityEngine.Random.Range(200f, 500f);
            return GameDataManager.CurrentGameData.num1;
        }
        else
        {
            return GameDataManager.CurrentGameData.num1;
        }
    }

    internal static int GetTodayAverageAttemptsNum()
    {
        return GameDataManager.CurrentGameData.num2;
    }

    internal static float GetTodayAverageWithDralNum()
    {
        return GameDataManager.CurrentGameData.num3;
    }

    internal static int GetRandowCount()
    {
        return UnityEngine.Random.Range(10, 20);
    }
    /// <summary>
    /// 网络可达性
    /// </summary> 
    /// <returns></returns>
    internal static bool IsNetworkReachability()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                return true;
            default:
//                Debug.LogError("未连接网络");
                return false;
        }
    }

internal static float GetBoxRewCanGetDollar()
    {
        if (!IsNetworkReachability())
            return 0;
        if (isNeedCloseMoneyIcon)
        {
            return 80;
        }
        else
        {
            return 8;
        }
  
    }

    internal static int GetBoxRewCanGetCoin()
    {
        return UnityEngine.Random.Range(20, 26);
    }

    internal static List<RankData> GetRankData()
    {
        return GameDataManager.GetankData();
    }

    internal static BosChanceData GetNowBoxChanceData()
    {
        int vlaue = UIManager.Instance.GetUI<PopGameSlidePlane>().GetSliderVlaue();
        if (GameHardChanceValue.Instance.parDic.ContainsKey(vlaue))
        {
            BosChanceData bosChanceData=new BosChanceData();
            bosChanceData.isNeedChance = true;
            bosChanceData.moveHowNumBox = GameHardChanceValue.Instance.parDic[vlaue].moveHowManyBoxl;
            bosChanceData.moveHowDic = GameHardChanceValue.Instance.parDic[vlaue].haoManyChance;
            return bosChanceData;
        }
        return null;
    }

    private static Dictionary<ScrewColor, ScrewColor> colorMapping = new Dictionary<ScrewColor, ScrewColor>();

    internal static void SetNewColor()
    {
        // 清空旧的映射
        colorMapping.Clear();
        
        // 获取所有可用的颜色
        List<ScrewColor> availableColors = new List<ScrewColor>() 
        { 
            ScrewColor.Blue, 
            ScrewColor.Purple, 
            ScrewColor.Green,
            ScrewColor.Red,
            ScrewColor.Pink,
            ScrewColor.Yellow,
            ScrewColor.Orange,
            ScrewColor.Grey,
            ScrewColor.LightBlue,
            ScrewColor.LightPurple
        };

        // 为每个颜色创建随机映射
        foreach (ScrewColor originalColor in availableColors.ToList())
        {
            // 从剩余颜色中随机选择一个
            int randomIndex = UnityEngine.Random.Range(0, availableColors.Count);
            ScrewColor mappedColor = availableColors[randomIndex];
            
            // 添加映射
            colorMapping[originalColor] = mappedColor;
            
            // 从可用列表中移除已使用的颜色
            availableColors.RemoveAt(randomIndex);
        }

        //// 打印映射关系以便调试
        //Debug.Log("新的颜色映射关系：");
        //foreach (var mapping in colorMapping)
        //{
        //    Debug.Log($"{mapping.Key} -> {mapping.Value}");
        //}
    }

    internal static ScrewColor GetNewColor(ScrewColor boxColor)
    {
        // 如果没有映射关系，返回原始颜色
        if (!colorMapping.ContainsKey(boxColor) )
        {
            return boxColor;
        }
        
        return colorMapping[boxColor];
    }

    internal static Sprite GetNewBoxColorImage(ScrewColor color)
    {
        string napPath = "";

        // 根据 Box 的颜色生成路径
        switch (color)
        {
            case ScrewColor.Red:
                napPath = "box_red3";
                break;
            case ScrewColor.Blue:
                napPath = "box_dark_blue3";
                break;
            case ScrewColor.Pink:
                napPath = "box_pink3"; // 自定义颜色名
                break;
            case ScrewColor.Yellow:
                napPath = "box_yellow3";
                break;
            case ScrewColor.Purple:
                napPath = "box_dark_purple3";
                break;
            case ScrewColor.Orange:
                napPath = "box_orange3";
                break;
            case ScrewColor.Green:
                napPath = "box_dark_green3";
                break;
            case ScrewColor.Grey:
                napPath = "box_gray3";
                break;
            case ScrewColor.LightBlue:
                napPath = "box_light_blue3";
                break;
            case ScrewColor.LightPurple:
                napPath = "box_light_purple3";
                break;
            default:
                napPath = "box_light_blue3"; // 默认情况下的盖子路径
                break;
        }

        // 拼接最终的路径
        Sprite sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/BoxImage/{napPath}.png");
        return sprite;
    }

    internal static bool IsFitLanguageAndCountyID(SystemLanguage_My currLanguageType, string countryID)
    {
        Debug.LogError("需要匹配的语言是：" + currLanguageType + " 需要匹配的国家是：" + countryID);

        bool isFit = false;

        if (currLanguageType == SystemLanguage_My.English)
        {
            HashSet<string> englishCountries = new HashSet<string>
            {
                "US", "GB", "AU", "CA", "NZ", "IE"
                , "Other"

            };
            isFit = englishCountries.Contains(countryID);
        }
        else
        {
            switch (currLanguageType)
            {
                case SystemLanguage_My.Portuguese:
                    isFit = countryID == "PT" || countryID == "BR"; break;
                default:
                    isFit = false; break;
            }
        }

        string chineseLanguageName = ConvertLanguageToChinese(currLanguageType);
        string chineseCountryName = ConvertCountryToChinese(countryID);
        TDAnalyticsManager.Instance.SendUILanguageAndCounyQuestion(chineseLanguageName, chineseCountryName, isFit);

        return isFit;
    }
    private static string ConvertCountryToChinese(string countryID)
    {
        switch (countryID)
        {
            case "US": return "美国";
            case "GB": return "英国";
            case "AU": return "澳大利亚";
            case "CA": return "加拿大";
            case "NZ": return "新西兰";
            case "IE": return "爱尔兰";
            case "JP": return "日本";
            case "PT": return "葡萄牙";
            case "BR": return "巴西";
            case "ES": return "西班牙";
            case "MX": return "墨西哥";
            case "AR": return "阿根廷";
            case "KR": return "韩国";
            case "DE": return "德国";
            case "AT": return "奥地利";
            case "CH": return "瑞士";
            case "FR": return "法国";
            case "BE": return "比利时";
            case "ID": return "印度尼西亚";
            case "RU": return "俄罗斯";
            case "VN": return "越南";
            case "TH": return "泰国";
            case "TR": return "土耳其";
            case "IN": return "印度";
            case "MY": return "马来西亚";
            case "Other": return "其他";
            default: return countryID;
        }
    }
    private static string ConvertLanguageToChinese(SystemLanguage_My language)
    {
        switch (language)
        {
            case SystemLanguage_My.English: return "英语";
            case SystemLanguage_My.Japanese: return "日语";
            case SystemLanguage_My.Portuguese: return "葡萄牙语";
            case SystemLanguage_My.Spanish: return "西班牙语";
            case SystemLanguage_My.Korean: return "韩语";
            case SystemLanguage_My.German: return "德语";
            case SystemLanguage_My.French: return "法语";
            case SystemLanguage_My.Indonesian: return "印尼语";
            case SystemLanguage_My.Russian: return "俄语";
            case SystemLanguage_My.Vietnamese: return "越南语";
            case SystemLanguage_My.Thai: return "泰语";
            case SystemLanguage_My.Turkish: return "土耳其语";
            case SystemLanguage_My.India: return "印度语";
            case SystemLanguage_My.Malaysia: return "马来语";
            default: return language.ToString();
        }
    }
    internal static void CheakNewPlayerGuite()
    {
        if (LevelManager.Instance.levelNum == 1)
        {
            GameManager.Instance.WinGame();
        }
    }

    internal static string GetColorText(string v, string name)
    {
        return $"<color={v}>{name}</color>";
    }
    public static bool isGetNowSevertTimeSucess = false;//是否获取服务器时间成功
    public static System.DateTime nowSevertTime;//服务器时间
    static bool useServerTime = false;//是否使用服务器时间
    static int nowCount;
    static int maxCount = 5;


    internal static int GetPLayerLoginDay()
    {
        if (isGetNowSevertTimeSucess)
        {
            //使用服务器时间，算领取的天数
            int nowGetCount = GameDataManager.CurrentGameData.dayLoginRewardCompleteDci.Count;
            if (GameDataManager.CurrentGameData.DayLoginRewardGetTime.Date != nowSevertTime.Date)
            {
                nowGetCount++;
            }
            return nowGetCount;
        }
        else
        {
            //登录时长，不使用服务器时间
            int count = GameDataManager.CurrentGameData.dayLoginRewardCompleteDci.Count;
            return count;
        }

    }
    internal static IEnumerator TryToGetSceverTime()
    {
        nowCount++;
        const string timeServerUrl = "https://google.com";

        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(timeServerUrl))
        {
            request.timeout = 15;
            Debug.Log($"尝试从服务器获取时间: {timeServerUrl}");
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                string dateHeader = request.GetResponseHeader("Date");
                if (!string.IsNullOrEmpty(dateHeader))
                {
                    if (System.DateTime.TryParse(dateHeader, out System.DateTime parsedTime))
                    {
                        System.DateTime serverTime = parsedTime.ToUniversalTime();
                        Debug.Log($"成功解析服务器时间: {serverTime}");
                        isGetNowSevertTimeSucess = true;
                        nowSevertTime = serverTime;
                        useServerTime = true;
                    }
                    else
                    {
                        Debug.LogWarning($"解析服务器 Date 头失败: {dateHeader}");
                        isGetNowSevertTimeSucess = false;
                    }
                }
                else
                {
                    Debug.LogWarning("服务器未返回 Date 头");
                    isGetNowSevertTimeSucess = false;
                }
            }
            else
            {
                string errorMsg = "获取服务器时间失败";
                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
                {
                    errorMsg += $" (连接错误: {request.error})";
                }
                else if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
                {
                    errorMsg += $" (协议错误: {request.responseCode} - {request.error})";
                }
                else if (request.result == UnityEngine.Networking.UnityWebRequest.Result.DataProcessingError)
                {
                    errorMsg += $" (数据处理错误: {request.error})";
                }
                else
                {
                    errorMsg += $" (未知错误: {request.result} - {request.error})";
                }
                Debug.LogWarning(errorMsg);
                isGetNowSevertTimeSucess = false;
            }
        }

        if (isGetNowSevertTimeSucess)
        {
            GameHardChanceValue.Instance.StartCoroutine(MathNowTime());
        }
        else
        {
            if (nowCount < maxCount)
            {
                GameHardChanceValue.Instance.StartCoroutine(TryToGetSceverTime());
            }
            else
            {
                useServerTime = false;
            }
        }
    }
    static IEnumerator MathNowTime()
    {
        WaitForSeconds wait = new WaitForSeconds(1);
        while (true)
        {
            yield return wait;
            nowSevertTime = nowSevertTime.AddSeconds(1);
        }
    }
    internal static DateTime GetCurrentTime()
    {
        if (isGetNowSevertTimeSucess)
        {
            return nowSevertTime;
        }
        else
        {
            return DateTime.Now;
        }
    }

    internal static bool CheckNoGetReward(int nowLoginDay)
    {
        if (isGetNowSevertTimeSucess)
        {
            //使用服务器时间,如果领取时间和服务器时间不同，则可以获得奖励
            if (GameDataManager.CurrentGameData.DayLoginRewardGetTime.Date != nowSevertTime.Date)
            {
                if (nowLoginDay > GameDataManager.CurrentGameData.dayLoginRewardCompleteDci.Count)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            if (GameDataManager.CurrentGameData.dayLoginRewardCompleteDci.Count == 0)//不使用服务器时间，只有第一天可以获得奖励
            {
                return true;
            }
            return false;
        }
    }
}
/// <summary>
/// 服务器时间响应数据结构
/// </summary>
[System.Serializable]
public class ServerTimeResponse
{
    public int code;
    public string msg;
    public long data;
    public object rdata;
}

/// <summary>
/// WorldTimeAPI 响应数据结构
/// </summary>
[System.Serializable]
public class WorldTimeApiResponse
{
    public string utc_offset;
    public string timezone;
    public int day_of_week;
    public int day_of_year;
    public string datetime;
    public string utc_datetime;
    public long unixtime;
    public int raw_offset;
    public int week_number;
    public bool dst;
    public string abbreviation;
    public int dst_offset;
    public string dst_from;
    public string dst_until;
    public string client_ip;
}
