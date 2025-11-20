using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DafultScript;
namespace DafultScript
{
    public class StickerUI : MonoBehaviour
    {
        private GameObject BG;

        private Transform panelDevice;
        private Transform topTrans;
        private Image sliderImage;
        private Text sliderText;
        private Button starButton;
        private Text starText;

        private Transform ButtonHolder;
        private StickerButton b1;
        private StickerButton b2;
        private StickerButton b3;

        private List<Vector3> originPosList = new List<Vector3>();
        private Transform process;
        private Button close;
        [SerializeField] private Sticker curSticker;
        private void Awake()
        {
            BG = transform.Find("BG").gameObject;
            panelDevice = transform.Find("BG/Save Area/Panel Device");
            topTrans = panelDevice.Find("Top");
            sliderImage = topTrans.Find("Process/slider").GetComponent<Image>();
            sliderText = topTrans.Find("Process/slider/Text").GetComponent<Text>();
            starButton = topTrans.Find("Button Star").GetComponent<Button>();
            starText = starButton.transform.Find("Text").GetComponent<Text>();
            starButton.onClick.AddListener(StarEvent);

            ButtonHolder = panelDevice.Find("Botton");
            b1 = ButtonHolder.Find("B1").GetComponent<StickerButton>();
            b2 = ButtonHolder.Find("B2").GetComponent<StickerButton>();
            b3 = ButtonHolder.Find("B3").GetComponent<StickerButton>();

            process = topTrans.Find("Process");
            close = topTrans.Find("Button Close").GetComponent<Button>();
            close.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySFX("Click");
                CloseEvent();
            });

            for (int i = 0; i < ButtonHolder.childCount; i++)
            {
                originPosList.Add(ButtonHolder.GetChild(i).position);
            }

        }
        private void Start()
        {
            FreshStickerButton();
        }
        //更新按钮图标
        public void FreshStickerButton()
        {
            curSticker = StickerManager.Instance.CurSticker;

            if (curSticker.IsCompleted())
            {
                Debug.Log("当前地图已经完成");
                CompeleteEvent();
                return;
            }

            FreshUnlockStickerEvent();
            b1.FreshButton(curSticker.GetB1Item());
            b2.FreshButton(curSticker.GetB2Item());
            b3.FreshButton(curSticker.GetB3Item());

            SetButtonClick();
        }
        private void FreshUnlockStickerEvent()
        {
            b1.FreshUnlockStickerList(curSticker.GetB1List(), this);
            b2.FreshUnlockStickerList(curSticker.GetB2List(), this);
            b3.FreshUnlockStickerList(curSticker.GetB3List(), this);
        }

        /// <summary>
        /// 完成之后调用的事件
        /// </summary>
        private void CompeleteEvent()
        {
            //如果已经打开过宝箱，返回
            if (GameDataManager.CurrentGameData.isStikcerChestOpen)
            {
                return;
            }
            //退出场景
            HomeSceneUI.Instance.ExitSticker();
            //弹出完成UI
            //增加收集完毕的列表
            GameDataManager.AddCompleteStickerList();
            //设置宝箱为未开启状态
            GameDataManager.SetStikcerChestOpen(false);

            if (StickerManager.Instance.IsLastSticker())
            {
                Debug.Log("已经是最后一个贴纸了");
                return;
            }
            //切换按钮
            HomeSceneUI.Instance.homeUI.SetStickerButton(false);
        }

        //将按钮设置为可以点击
        public void SetButtonClick()
        {
            b1.SetButton(GameDataManager.CurrentGameData.curB1Index <= GameDataManager.CurrentGameData.curButtonIndex);
            b2.SetButton(GameDataManager.CurrentGameData.curB2Index <= GameDataManager.CurrentGameData.curButtonIndex);
            b3.SetButton(GameDataManager.CurrentGameData.curB3Index <= GameDataManager.CurrentGameData.curButtonIndex);
        }
        /// <summary>
        /// 更新贴纸进度
        /// </summary>
        public void UpdateStikcerProgress()
        {
            sliderImage.fillAmount = GameDataManager.GetUnlockStickerList().Count / 9.0f;
            sliderText.text = $"{GameDataManager.GetUnlockStickerList().Count} / 9";
        }

        //进入ui和退出的函数
        public void EnterSticker()
        {
            BG.SetActive(true);
            if (StickerManager.Instance.IsLastSticker() && GameDataManager.CurrentGameData.curButtonIndex == 3)
            {
                ButtonHolder.gameObject.SetActive(false);
            }
            AnimationUtility.ChildrenDownToUp(ButtonHolder, originPosList);
            AnimationUtility.PlayEnterFromRight(close.transform);
            AnimationUtility.FadeIn(process);

            //更新贴纸进度
            UpdateStarText();
            UpdateStikcerProgress();
        }

        public void ExitSticker()
        {
            AnimationUtility.ChildrenUpToDown(ButtonHolder, originPosList);
            AnimationUtility.PlayExitToRight(close.transform);
            AnimationUtility.FadeOut(process, 0.5f, () => BG.SetActive(false));
            HomeSceneUI.Instance.homeUI.UpdateHomeStickerSlider();
        }
        private void CloseEvent()
        {
            HomeSceneUI.Instance.ExitSticker();
        }

        private void StarEvent()
        {

        }

        /// <summary>
        /// 更新贴纸UI中的文本
        /// </summary>
        public void UpdateStarText()
        {
            starText.text = GameDataManager.CurrentGameData.starCount.ToString();
        }

    }
}