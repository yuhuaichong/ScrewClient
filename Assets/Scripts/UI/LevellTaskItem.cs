using cfg;
using DafultScript;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LevellTaskItem : MonoBehaviour
{
    Image Icon;
    LanguageText TitleText;
    LanguageText NumText;
    Slider slider;
    LanguageText BoxNumText;
    GameObject NoCanGet;
    Button Get;
    ConfDayLoginReward confDayLoginReward;
    Transform NanDu1;
    Transform NanDu2;
    internal void Init(ConfDayLoginReward confDayLoginReward)
    {
        this.confDayLoginReward = confDayLoginReward;
        Icon = transform.Find("Icon").GetComponent<Image>();
        TitleText = transform.Find("TitleText").GetComponent<LanguageText>();
        NumText = transform.Find("Icon/NumText").GetComponent<LanguageText>();
        slider = transform.Find("Slider").GetComponent<Slider>();
        BoxNumText = transform.Find("BoxNumText").GetComponent<LanguageText>();
        NoCanGet = transform.Find("NoCanGet").gameObject;
        Get = transform.Find("Get").GetComponent<Button>();

        //TitleText.SetTextWithParameter("登录{0}天", confDayLoginReward.DayLogin);
        TitleText.languageId = confDayLoginReward.TitleId;
        TitleText.alignment = TextAnchor.MiddleCenter;
        slider.gameObject.SetActive(false);
        BoxNumText.gameObject.SetActive(false);
        TitleText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-126.52f, -66.76f);
        TitleText.GetComponent<RectTransform>().sizeDelta = new Vector2(373.97f, 49.99f);
        Icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(105f, -66f);
        Icon.transform.localScale = Vector3.one * 1.5f;
        NoCanGet.GetComponent<RectTransform>().anchoredPosition = new Vector2(406.22f, -91f);
        Get.GetComponent<RectTransform>().anchoredPosition = new Vector2(409, -68f);
        Get.transform.localScale = Vector3.one * 0.7F;
        int nowLoginDay = GameTool.GetPLayerLoginDay();//获取玩家登录天数
        slider.value = nowLoginDay * 1.0f / confDayLoginReward.DayLogin;
        BoxNumText.text = $"{Math.Max(nowLoginDay, confDayLoginReward.DayLogin)} / {confDayLoginReward.DayLogin}";
        if (GameDataManager.CurrentGameData.currLanguageType == SystemLanguage_My.Portuguese &&
        confDayLoginReward.GetRewardId == 2)
        {
            NumText.text = (confDayLoginReward.GetNum / 5 * GameTool.confPayRegion.ExchangeRate).ToString();
        }
        else if (confDayLoginReward.GetRewardId == 1)
        {
            NumText.text = confDayLoginReward.GetNum.ToString();
        }
        else if (confDayLoginReward.GetRewardId == 2)
        {
            NumText.text = (confDayLoginReward.GetNum * GameTool.confPayRegion.ExchangeRate).ToString();
        }

        if (confDayLoginReward.GetRewardId != 1)
        {
            Icon.sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_{GameTool.dollarIconPath}");
        }

        if (nowLoginDay == confDayLoginReward.DayLogin && GameTool.CheckNoGetReward(nowLoginDay))//等于今天并且没有领取
        {
            NoCanGet.gameObject.SetActive(false);
            Get.gameObject.SetActive(true);
            Get.onClick.AddListener(GetClikcHandle);
            Get.GetComponent<RectTransform>().anchoredPosition = new Vector2(383, -68f);
        }
        else if (nowLoginDay < confDayLoginReward.DayLogin)
        {
            NoCanGet.gameObject.SetActive(true);
            Get.gameObject.SetActive(false);
        }
        else
        {
            NoCanGet.gameObject.SetActive(true);
            Get.gameObject.SetActive(false);
            NoCanGet.transform.Find("LanguageText").gameObject.SetActive(false);
            NoCanGet.transform.Find("getIng").gameObject.SetActive(true);
        }
    }
    private void GetClikcHandle()
    {
        EventManager.Instance.TriggerEvent(GameEvent.HideTaskRed);
        if (confDayLoginReward.GetRewardId == 2)
        {
            if (GameDataManager.CurrentGameData.currLanguageType == SystemLanguage_My.Portuguese &&
confDayLoginReward.GetRewardId == 2)
                EventManager.Instance.TriggerEvent<float>(GameEvent.GetDollar, confDayLoginReward.GetNum / 5f);//仅获得金钱用这个
            else
            {
                EventManager.Instance.TriggerEvent<float>(GameEvent.GetDollar, confDayLoginReward.GetNum);//仅获得金钱用这个
            }
        }
        else
        {
            ConfItem CoinItem = ConfigModule.Instance.Tables.TbItem.GetOrDefault(1);

            GameTool.GetItem(CoinItem, confDayLoginReward.GetNum, Get.GetComponent<RectTransform>());//获得金币用这个
        }
        EventManager.Instance.TriggerEvent<int>(GameEvent.GetOneDayLoginReward, confDayLoginReward.Sn);
    }
}
