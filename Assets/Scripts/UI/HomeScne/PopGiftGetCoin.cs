using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class PopGiftGetCoin : MainBaseUI
    {
        Button NorGet;
        Button VideoGet;
        LanguageText NumText;
        LanguageText NumTip;
        protected override void Awake()
        {
            base.Awake();
            NorGet = tableTransform.Find("NorGet").GetComponent<Button>();
            VideoGet = tableTransform.Find("VideoGet").GetComponent<Button>();
            NumText = tableTransform.Find("Icon2/NumText").GetComponent<LanguageText>();
            NumTip = tableTransform.Find("NumTip").GetComponent<LanguageText>();
            NorGet.onClick.AddListener(NorGetOnClikHandle);
            VideoGet.onClick.AddListener(VideoGetOnClikHandle);
            NumText.text = $"x{GameTool.GiftCanGetCoin}";
            //NumTip.SetTextWithParameter("Watch an ad to get {0} Diamonds", GameTool.GiftCanGetCoin);
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, UIEffectType.Scale);
        }
        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.HideUI(delaytime, callback, UIEffectType.Scale);
        }
        private void VideoGetOnClikHandle()
        {
            GameVideoContor.ShowVideoAd(VedioAdType.飞行宝箱双倍奖励, "幸运宝箱", delegate (bool isComplete, int a)
            {
                if (isComplete)
                {
                    DOVirtual.DelayedCall(0.4f, () =>
                    {
                        AudioManager.Instance.PlaySFX("getCoin");
                    });
                    int coin = GetRandomCoin();
                    UIManager.Instance.HideUI<PopGiftGetCoin>();
                    EventManager.Instance.TriggerEvent<int>(GameEvent.GetCoin, coin);
                }
            }, null);
        }

        private void NorGetOnClikHandle()
        {
            //int coin = GetRandomCoin();
            UIManager.Instance.HideUI<PopGiftGetCoin>();
            //EventManager.Instance.TriggerEvent<int>(GameEvent.GetCoin, coin);
        }
        public int GetRandomCoin()
        {
            return GameTool.GiftCanGetCoin;
        }
    }
}
