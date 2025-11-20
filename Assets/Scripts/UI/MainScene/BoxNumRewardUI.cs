using cfg;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class BoxNumRewardUI : MainBaseUI
    {
        Image DollarIcon;
        Text RewardNumText;
        Text LittleText;
        Text BigText;
        LanguageText OnlyCanGetText;
        Image BeiShuoSlider;
        Image LittleCoinIcon;
        Button VideoGet;
        Button Close;
        Button ThankBut;
        RectTransform BigCoinIcon;
        float canGetDollar;
        int canGetCoin;


        float dollarNum;
        float nowLiderNum;
        bool isMove;
        public float speed = 1f; // 可以在Inspector中调整速度
        Text CoinCanGetNum;
        ConfItem dollayItem;
        ConfItem CoinItem;
        bool isClik;
        float onlyCanGetDollar;
        bool isComInter;

        protected override void Awake()
        {
            base.Awake();


            VideoGet = transform.Find("BG/Table/VideoDobleBut").GetComponent<Button>();
            Close = transform.Find("BG/Table/Close").GetComponent<Button>();
            ThankBut = transform.Find("BG/Table/ThankBut").GetComponent<Button>();
            LittleText = transform.Find("BG/Table/LittleCoinIcon/LittleText").GetComponent<Text>();
            BigText = transform.Find("BG/Table/BigCoinIcon/BigText").GetComponent<Text>();
            BigCoinIcon = transform.Find("BG/Table/BigCoinIcon").GetComponent<RectTransform>();
            OnlyCanGetText = transform.Find("BG/Table/ThankBut/OnlyCanGetText").GetComponent<LanguageText>();
            LittleCoinIcon = transform.Find("BG/Table/LittleCoinIcon").GetComponent<Image>();
            LittleCoinIcon.sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_big_{GameTool.dollarIconPath}");
            VideoGet.onClick.AddListener(VideoGetClikHandle);
            ThankBut.onClick.AddListener(ThankButClikHandle);
            dollayItem = ConfigModule.Instance.Tables.TbItem.GetOrDefault(2);
            CoinItem = ConfigModule.Instance.Tables.TbItem.GetOrDefault(1);
            // Close.onClick.AddListener(CloseClikHandle);

        }

        private void ThankButClikHandle()
        {
            //DOVirtual.DelayedCall(0.3f, () =>
            //{
            //    AudioManager.Instance.PlaySFX("ContCoinAudio");
            //});
            bool isNeedShowInterVideo = CheckInterVideo();//检查是否需要播放插屏广告
            Debug.LogError($"是否需要打开插屏广告:{isNeedShowInterVideo}");
            if (isNeedShowInterVideo)
            {
                GameVideoContor.ShowInterVideoAd(VedioAdType.弹窗奖励插屏广告, VedioAdType.弹窗奖励插屏广告.ToString(), delegate (bool isComplete, int a)
                {
                    if (isComplete)
                    {
                        isComInter = true;
                    }
                    else
                    {
                        isComInter = false;
                    }

                }, delegate (int a, string b)
                {

                });
            }
            else
            {

            }

            GameManager.Instance.StartCoroutine(WaitComInter());
            UIManager.Instance.HideUI<BoxNumRewardUI>();
        }
        IEnumerator WaitComInter()
        {
            yield return new WaitForSeconds(0.1f);
            if (isComInter)
            {
                Debug.Log("获得插屏奖励");
                GameTool.GetItem(dollayItem, canGetDollar, LittleCoinIcon.GetComponent<RectTransform>());
                GameTool.GetItem(CoinItem, canGetCoin, BigCoinIcon.GetComponent<RectTransform>());
                DOVirtual.DelayedCall(0.4f, () =>
                {
                    AudioManager.Instance.PlaySFX("OnlyGetMoney");
                });
                UIManager.Instance.HideUI<BoxNumRewardUI>();
            }
            else
            {
                Debug.Log("正常获得奖励奖励");
                EventManager.Instance.TriggerEvent<float>(GameEvent.GetDollar, onlyCanGetDollar);
                UIManager.Instance.HideUI<BoxNumRewardUI>();
            }
            isComInter = false;
        }
        private bool CheckInterVideo()
        {
            if (GameDataManager.CurrentGameData.nextOpenInterVideoWithEight <= GameDataManager.CurrentGameData.nowOpenBoxRewardPlaneNum &&
                GameDataManager.CurrentGameData.nextOpenInterVideoWithEight != 0)
            {
                int value = GameTool.howManyOpenInter;
                //int value = UnityEngine.Random.Range(2, 4);
                GameDataManager.CurrentGameData.nextOpenInterVideoWithEight = GameDataManager.CurrentGameData.nowOpenBoxRewardPlaneNum + value;
                if (GameTool.IsOpenOnlyWightIder)
                {
                    return true;
                }
            }
            return false;
        }

        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            if (GameDataManager.CurrentGameData.nextOpenInterVideoWithEight == 0)
            {
                int value = GameTool.howManyOpenInter;
                GameDataManager.CurrentGameData.nextOpenInterVideoWithEight = GameDataManager.CurrentGameData.nowOpenBoxRewardPlaneNum + value;
            }
            GameDataManager.CurrentGameData.nowOpenBoxRewardPlaneNum++;
            base.ShowUI(callback, UIEffectType.Scale);
            isClik = false;
            AudioManager.Instance.PlaySFX("BoxNumReward");
            canGetDollar = GameTool.GetBoxRewCanGetDollar();
            canGetCoin = GameTool.GetBoxRewCanGetCoin();
            LittleText.text = GameTool.GetDollarIconAndNum(canGetDollar);
            BigText.text = $"x{canGetCoin}";
            onlyCanGetDollar = GameTool.GetBoxComplete(LevelManager.Instance.levelNum);
            OnlyCanGetText.SetTextWithParameter("Only {0}", GameTool.GetDollarIconAndNum(onlyCanGetDollar));
        }

        private void CloseClikHandle()
        {
            AudioManager.Instance.PlaySFX("Click");
            //isMove = false;
            UIManager.Instance.HideUI<BoxNumRewardUI>();
        }
        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.HideUI(delaytime, callback, UIEffectType.Scale);
        }
        private void GetPlayerDollar(float dollarNum)
        {
            if (isClik) return;
            isClik = true;
            UIManager.Instance.HideUI<BoxNumRewardUI>();
            EventManager.Instance.TriggerEvent<float>(GameEvent.GetDollar, dollarNum);
        }

        private void VideoGetClikHandle()
        {
            AudioManager.Instance.PlaySFX("Click");
            if (isClik) return;
            GameVideoContor.ShowVideoAd(VedioAdType.收集盒子奖励金币和金钱, VedioAdType.收集盒子奖励金币和金钱.ToString(), delegate (bool isComplete, int a)
            {
                if (isComplete)
                {
                    //DOVirtual.DelayedCall(0.3f, () =>
                    //{
                    //    AudioManager.Instance.PlaySFX("ContCoinAudio");
                    //});
                    isClik = true;
                    GameTool.GetItem(dollayItem, canGetDollar, LittleCoinIcon.GetComponent<RectTransform>());
                    GameTool.GetItem(CoinItem, canGetCoin, BigCoinIcon.GetComponent<RectTransform>());
                    DOVirtual.DelayedCall(0.4f, () =>
                    {
                        AudioManager.Instance.PlaySFX("OnlyGetMoney");
                    });
                    UIManager.Instance.HideUI<BoxNumRewardUI>();
                }
            }, null);
        }

        private float GetNowBeiShu()
        {
            // 将0-1范围平均分为5份
            float sectionSize = 1f / 5f; // 每份大小是0.2

            // 计算当前在第几个区间
            int section = Mathf.FloorToInt(nowLiderNum / sectionSize);

            // 确保section在0-4之间
            section = Mathf.Clamp(section, 0, 4);

            // 返回对应倍数（区间+1）
            return section + 1;

            /* 具体区间对应关系：
            0.0-0.2 -> 1倍
            0.2-0.4 -> 2倍
            0.4-0.6 -> 3倍
            0.6-0.8 -> 4倍
            0.8-1.0 -> 5倍
            */
        }
    }
}
