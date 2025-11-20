using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class LoseUI : MainBaseUI
    {
        private Button backBtn;
        private Button tryaginBtn;
        protected Transform containTrans;
        private Button closeButton;
        LanguageText LevelNum;
        LanguageText DesText;
        Image MoneyCoin;
        int loseReviceCount;//复活次数
        private Button CoinToRevive;
        protected override void Awake()
        {
            base.Awake();
            containTrans = tableTransform.Find("Contain").transform;
            tryaginBtn = containTrans.Find("Button TRYAGAIN").transform.GetComponent<Button>();
            tryaginBtn.onClick.AddListener(TryAgain);
            backBtn = containTrans.Find("Button Back").transform.GetComponent<Button>();
            backBtn.onClick.AddListener(BackPreUI);
            CoinToRevive = containTrans.Find("CoinToRevive").transform.GetComponent<Button>();
            CoinToRevive.onClick.AddListener(CoinToReviveOnClick);
            LevelNum = tableTransform.Find("LevelNum").GetComponent<LanguageText>();
            DesText = tableTransform.Find("DesText").GetComponent<LanguageText>();
            MoneyCoin = tableTransform.Find("MoneyCoin").GetComponent<Image>();
            MoneyCoin.sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_big_{GameTool.dollarIconPath}");
        }
        private void CoinToReviveOnClick()
        {
            if (loseReviceCount >= 2 )
            {
                UIManager.Instance.ShowUI<AlertUI>();
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("复活次数不足");
                return;
            }
            if (GameDataManager.DecreaseCoinCount(GameTool.reviveNeedCoin))
            {
                loseReviceCount++;
                GameManager.Instance.isLose = false;
                UIManager.Instance.HideUI<LoseUI>();
                GameManager.Instance.SetEmptyHoleToPropEmpty();//把螺丝放入道具盒
                GameManager.Instance.SetGameState(GameState.Start);
            }
        }
        public void RecovesCount()
        {
            loseReviceCount = 0;
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            //AudioManager.Instance.PlaySFX("Lose");
            base.ShowUI(callback, UIEffectType.Scale);
            if (GameTool.IsOpenLosetIder && false)
            {
                GameVideoContor.ShowInterVideoAd(VedioAdType.失败插屏广告, VedioAdType.失败插屏广告.ToString(), delegate (bool isComplete, int a)
                {
                }, null);
            }
            DesText.text = GameTool.GetDollarIconAndNum(GameDataManager.CurrentGameData.piggyCount);
            LevelNum.SetTextWithParameter("关卡{0}", GameDataManager.CurrentGameData.levelNum);
            GameManager.Instance.isLose = true;
            tryaginBtn.transform.localScale = Vector3.zero;
            DOVirtual.DelayedCall(3, () =>
            {
                tryaginBtn.transform.DOScale(Vector3.one, 0.5f);
            });

        }

        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.HideUI(delaytime, () =>
            {
                if (!backBtn.gameObject.activeSelf)
                    SetBackObj(true);
            }, UIEffectType.Scale);
        }

        private void CloseEvent()
        {
            UIManager.Instance.HideUI<LoseUI>();
            GameManager.Instance.isLose = false;
            //重新开始游戏
            LevelManager.Instance.ReStartGmae();
        }
        private void TryAgain()
        {
            UIManager.Instance.HideUI<LoseUI>();
            GameManager.Instance.isLose = false;
            //重新开始游戏
                   //重新开始游戏
            GameManager.Instance.nowLevelScrewColorData = -1;
            LevelManager.Instance.ReStartGmae();
            TDAnalyticsManager.Instance.SendLoseLevel(GameTool.nowLevel, GameTool.nowProgress);
        }

        /// <summary>
        /// 看广告继续游戏
        /// </summary>
        private void BackPreUI()
        {
            //UIManager.Instance.SwitchToPreviousUI();
            if (loseReviceCount >= 2 && false)
            {
                UIManager.Instance.ShowUI<AlertUI>();
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("复活次数不足");
                return;
            }
            GameVideoContor.ShowVideoAd(VedioAdType.复活, VedioAdType.复活.ToString(), delegate (bool isComplete, int a)
            {
                if (isComplete)
                {
                    loseReviceCount++;
                    GameManager.Instance.isLose = false;
                    UIManager.Instance.HideUI<LoseUI>();
                    GameManager.Instance.SetEmptyHoleToPropEmpty();//把螺丝放入道具盒
                    GameManager.Instance.SetGameState(GameState.Start);
                }
            }, null);
        }

        /// <summary>
        /// 是否显示返回上一级UI
        /// </summary>
        public void SetBackObj(bool val)
        {
            //backBtn.gameObject.SetActive(val);
        }
    }
}
