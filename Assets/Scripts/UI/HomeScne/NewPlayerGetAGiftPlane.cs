using System;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class NewPlayerGetAGiftPlane : MainBaseUI
    {
        Image SmaliiIcon;
        Image BigIcon;
        LanguageText DollarText;
        LanguageText LanguageText;
        LanguageText LanguageTextBut;
        Button GetNowBut;
        protected override void Awake()
        {
            return;
            base.Awake();
            SmaliiIcon = tableTransform.Find("SmaliiIcon").GetComponent<Image>();
            BigIcon = tableTransform.Find("BigIcon").GetComponent<Image>();
            DollarText = tableTransform.Find("DollarText").GetComponent<LanguageText>();
            LanguageText = tableTransform.Find("Title/LanguageText").GetComponent<LanguageText>();
            LanguageTextBut = tableTransform.Find("GetNowBut/LanguageTextBut").GetComponent<LanguageText>();
            GetNowBut = tableTransform.Find("GetNowBut").GetComponent<Button>();
            LanguageText.text = "新手礼包";
            LanguageTextBut.text = "立即获取";
            SmaliiIcon.sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_{GameTool.dollarIconPath}");
            if (GameTool.isNeedCloseMoneyIcon)
            {
                BigIcon.sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_big_{GameTool.dollarIconPath}");
            }
            else
            {
                BigIcon.sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/NewPlayerGift/{GameTool.dollarIconPath.Replace("_0", "")}.png");
            }
            BigIcon.SetNativeSize();
            if (GameDataManager.CurrentGameData.currLanguageType == SystemLanguage_My.English)
            {
                DollarText.text = GameTool.GetDollarIconAndNum(200, 0);
            }
            else
            {
                DollarText.text = GameTool.GetDollarIconAndNum(40, 0);
            }
            GetNowBut.onClick.AddListener(GetNowButOnClikHandele);
        }

        private void GetNowButOnClikHandele()
        {
            UIManager.Instance.HideUI<NewPlayerGetAGiftPlane>();
            if (GameDataManager.CurrentGameData.currLanguageType == SystemLanguage_My.English)
            {
                EventManager.Instance.TriggerEvent(GameEvent.GetDollar, 200f);
            }
            else
            {
                EventManager.Instance.TriggerEvent(GameEvent.GetDollar, 40f);
            }

            EventManager.Instance.TriggerEvent(GameEvent.GetNewGuiteGiftt);
        }
    }
}
