using System;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class VideoToLockBox : MainBaseUI
    {
        private Button coinButton;
        private Button freeButton;
        private Button closeButton;
        int index;
        GameObject box;
        protected override void Awake()
        {
            base.Awake();
            closeButton = tableTransform.Find("Button Close").GetComponent<Button>();
            coinButton = tableTransform.Find("Button Gold").GetComponent<Button>();
            freeButton = tableTransform.Find("Button Free").GetComponent<Button>();
            tableTransform.transform.Find("Button Gold/Text (Legacy)").GetComponent<Text>().text = GameTool.getOnePropNeedCoin.ToString();
            closeButton.onClick.AddListener(CloseEvent);
            coinButton.onClick.AddListener(CoinEvent);
            freeButton.onClick.AddListener(FreeEvent);
            //tableTransform.Find("CanGetDesText").GetComponent<LanguageText>().SetTextWithParameter("观看广告获得{0}个道具", 1);
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Scale)
        {
            base.ShowUI(callback, UIEffectType.Scale);
        }
        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Scale)
        {
            base.HideUI(delaytime, callback, UIEffectType.Scale);
        }
        internal void Init(int index, GameObject box)
        {
            this.index = index;
            this.box = box;
        }

        private void CloseEvent()
        {
            UIManager.Instance.HideUI<VideoToLockBox>();
        }

        private void CoinEvent()
        {
            if (GameDataManager.DecreaseCoinCount(GameTool.getOnePropNeedCoin))
            {
                GameManager.Instance.ToLockNewPositon(index);
                box.SetActive(false);
                UIManager.Instance.HideUI<VideoToLockBox>();
            }
        }

        private void FreeEvent()
        {
            GameVideoContor.ShowVideoAd(VedioAdType.主界面点击盒子解锁空位, $"螺丝盒{index - 1}", delegate (bool isComplete, int a)
            {
                if (isComplete)
                {
                    GameManager.Instance.ToLockNewPositon(index);
                    box.SetActive(false);
                    UIManager.Instance.HideUI<VideoToLockBox>();
                }
            }, null);

        }
    }
}
