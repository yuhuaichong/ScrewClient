using System;
using UnityEngine;
using DafultScript;

namespace DafultScript
{
    public class MainSceneUI : MonoBehaviour
    {
        // 静态实例
        public static MainSceneUI Instance { get; private set; }

        private GamePlayUI gamePlayUI;
        public EmptyHoleManager emptyHoleManager;

        public Canvas CanvasPopUp;

        public GamePlayUI _GamePlayUI
        {
            get => gamePlayUI;
        }
        Transform Prop1Tip;
        Transform Prop2Tip;
        Transform WithDrawTipBg;

        private void Awake()
        {
            // 检查是否已有实例
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // 设置单例实例
            Instance = this;

            gamePlayUI = transform.Find("Canvas Game/PopUpGamePlay").GetComponent<GamePlayUI>();
            CanvasPopUp = transform.Find("Canvas PopUp").GetComponent<Canvas>();
            Prop1Tip = transform.Find("Canvas PopUp/PropTip/Prop1Tip");
            Prop2Tip = transform.Find("Canvas PopUp/PropTip/Prop2Tip");
            WithDrawTipBg = transform.Find("Canvas PopUp/WithDrawTipBgPar/WithDrawTipBg");
            EventManager.Instance.RegisterEvent(GameEvent.ShowBoxNumReward, ShowBoxNumReward);
            EventManager.Instance.RegisterEvent(GameEvent.ShowPro1, ShowPro1);
            EventManager.Instance.RegisterEvent(GameEvent.ShowPro2, ShowPro2);
            EventManager.Instance.RegisterEvent(GameEvent.HideAllPro, HideAllPro);
            EventManager.Instance.RegisterEvent(GameEvent.ShowWithDrawTip, ShowWithDrawTip);
            HideAllPro();
        }

        private void ShowWithDrawTip()
        {
            WithDrawTipBg.gameObject.SetActive(true);
        }

        private void HideAllPro()
        {
            Prop1Tip.gameObject.SetActive(false);
            Prop2Tip.gameObject.SetActive(false);
        }

        private void ShowPro1()
        {
            Prop1Tip.gameObject.SetActive(true);
            Prop2Tip.gameObject.SetActive(false);
        }

        private void ShowPro2()
        {
            Prop1Tip.gameObject.SetActive(false);
            Prop2Tip.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            EventManager.Instance.UnregisterEvent(GameEvent.ShowBoxNumReward, ShowBoxNumReward);
            EventManager.Instance.UnregisterEvent(GameEvent.ShowPro1, ShowPro1);
            EventManager.Instance.UnregisterEvent(GameEvent.ShowPro2, ShowPro2);
            EventManager.Instance.UnregisterEvent(GameEvent.HideAllPro, HideAllPro);
            EventManager.Instance.UnregisterEvent(GameEvent.ShowWithDrawTip, ShowWithDrawTip);
        }
        private void ShowBoxNumReward()
        {
            if (LevelManager.Instance.levelNum < 4) return;
            UIManager.Instance.ShowUI<BoxNumRewardUI>();
        }

        public void SetLevelNum(int val)
        {
            gamePlayUI.SetLevelNum(val);
        }

        public void SetLevelTip(bool val, string tip = "")
        {
            gamePlayUI.SetTips(val, tip);
        }

        public void SetMainScene(bool val)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<Canvas>())
                    transform.GetChild(i).gameObject.SetActive(val);
            }
        }


    }
}