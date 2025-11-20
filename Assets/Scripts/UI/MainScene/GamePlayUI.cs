using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using cfg;
namespace DafultScript
{
    public class GamePlayUI : MonoBehaviour
    {
        private Transform safeArea;
        private Button buttonBack;
        private Button buttonRefresh;
        private Button buttonSetting;
        //private Button ButtonBosPos3;
        //private Button ButtonBosPos4;    
        private LanguageText levelText;
        private GameObject levle1TipText;
        private Vector3 oriTextSize;

        private Transform addHoleTrans;
        private Transform rocketTrans;
        private Transform doubleBoxTrans;

        public Transform HoleTrans { get => addHoleTrans; }
        public Transform RocketTrans { get => rocketTrans; }
        public Transform DoubleBoxTrans { get => doubleBoxTrans; }

        private Transform buttonHolder;
        public Button buttonAddHole;
        private Button buttonRocket;
        private Button buttonDoubleBox;

        public ClockSlider clockSlider;
        private bool hasClock;

        Transform TopArea;
        public Text CoinCount;
        public Text PiggyCount;
        public Image IconCoinAll;
        public Image IconPiggyCoinAll;
        public Button Task;
        public Button ButtonToOpenLuckPlane;
        public Button ButtonToPaiHangPlane;
        public Button Piggy;
        public Button MaxCon;


        SliderBubble SliderBubble;
        private Coroutine bubbleShowCoroutine;
        int nowCoin;
        float nowPiggy=-1;
        Text testText;
        GameObject CoinCountEffect;
        Button OpenWriteDataBut;
        Transform Red;
        Button GmtVideo;
        GameObject PiggyCountEffect;
        private void Awake()
        {
            //EventManager.Instance.RegisterEvent(GameEvent.ShowBoxPos, ShowBoxPos);
            EventManager.Instance.RegisterEvent<Vector2, float>(GameEvent.CreatBoxCompleteCoinEffect, CreatBoxCompleteCoinEffect);
            EventManager.Instance.RegisterEvent<float>(GameEvent.GetDollar, GetDollar);
            EventManager.Instance.RegisterEvent<int>(GameEvent.GetProp1, GetProp1);
            EventManager.Instance.RegisterEvent<int>(GameEvent.GetProp2, GetProp2);
            EventManager.Instance.RegisterEvent<int>(GameEvent.GetProp3, GetProp3);
            EventManager.Instance.RegisterEvent(GameEvent.SetPlayerCoinText, SetPlayerCoinText);
            EventManager.Instance.RegisterEvent(GameEvent.ShowCoinEffect, ShowCoinEffect);
            EventManager.Instance.RegisterEvent<int>(GameEvent.GetCoin, GetCoin);
            EventManager.Instance.RegisterEvent(GameEvent.ShowTaskRed, ShowTaskRed);
            EventManager.Instance.RegisterEvent(GameEvent.HideTaskRed, HideTaskRed);
            GameManager.Instance.SetPos(transform.Find("bg/GamePos").GetComponent<RectTransform>());
            GameTool.SetOutLine(transform.Find("bg").GetComponent<RectTransform>());
            safeArea = transform.Find("bg/SafeArea").transform;
            TopArea = transform.Find("bg/TopArea").transform;
            buttonHolder = transform.Find("bg/Bottom Holder").transform;
            levle1TipText = transform.Find("bg/Tutorial lvl1").gameObject;

            buttonBack = safeArea.Find("Button Back").GetComponent<Button>();
            buttonBack.onClick.AddListener(ShowExitUI);
            buttonRefresh = safeArea.Find("Button Refresh").GetComponent<Button>();
            buttonRefresh.onClick.AddListener(RefreshGame);
            buttonSetting = safeArea.Find("Button Setting").GetComponent<Button>();
            buttonSetting.onClick.AddListener(SettingEvent);
            //ButtonBosPos3 = safeArea.Find("Button BosPos3").GetComponent<Button>();
            //ButtonBosPos3.onClick.AddListener(BosPos3Event);
            //ButtonBosPos4 = safeArea.Find("Button BosPos4").GetComponent<Button>();
            //ButtonBosPos4.onClick.AddListener(BosPos4Event);
            levelText = TopArea.Find("LevelBg/LevelText").GetComponent<LanguageText>();

            addHoleTrans = buttonHolder.Find("hint_AddHole").transform;
            rocketTrans = buttonHolder.Find("hint_Glass").transform;
            doubleBoxTrans = buttonHolder.Find("hint_DoubleBox").transform;

            buttonAddHole = addHoleTrans.Find("unlock").GetComponent<Button>();
            buttonAddHole.onClick.AddListener(AddHole);
            buttonRocket = rocketTrans.Find("unlock").GetComponent<Button>();
            buttonRocket.onClick.AddListener(RocketClick);
            buttonDoubleBox = doubleBoxTrans.Find("unlock").GetComponent<Button>();
            buttonDoubleBox.onClick.AddListener(DoubleBox);

            clockSlider = safeArea.Find("ClockSlider").GetComponent<ClockSlider>();
            oriTextSize = levelText.transform.localScale;



            GmtVideo = transform.Find("bg/GmtVideo").GetComponent<Button>();
            GmtVideo.onClick.AddListener(() =>
            {
               //GMTManager.Instance.ShowH5();
            });
            GmtVideo.gameObject.SetActive(false);

             CoinCount = TopArea.Find("TotalCoin/CoinCount").GetComponent<Text>();
            CoinCountEffect = TopArea.Find("TotalCoin/CoinCountEffect").gameObject;
            IconCoinAll = TopArea.Find("TotalCoin/Icon Coin All").GetComponent<Image>();
            IconPiggyCoinAll = TopArea.Find("Piggy/Icon Piggy Coin All").GetComponent<Image>();
            IconPiggyCoinAll.sprite = GameTool.GetNormalCountryMoneyIcon();
            PiggyCount = TopArea.Find("Piggy/PiggyCount").GetComponent<Text>();
            // 记录文本的初始状态
            _coinTextBaseScale = CoinCount.transform.localScale;
            _piggyTextBaseScale = PiggyCount.transform.localScale;
            _coinTextBaseColor = CoinCount.color;
            _piggyTextBaseColor = PiggyCount.color;
            SliderBubble = safeArea.Find("SliderBubble").GetOrAddComponent<SliderBubble>();

            ButtonToOpenLuckPlane = safeArea.Find("ButtonToOpenLuckPlane").GetOrAddComponent<Button>();
            ButtonToOpenLuckPlane.gameObject.SetActive(false);
            ButtonToOpenLuckPlane.onClick.AddListener(ButtonToOpenLuckPlaneOnClikHandle);

            ButtonToPaiHangPlane = safeArea.Find("ButtonToPaiHangPlane").GetOrAddComponent<Button>();
            ButtonToPaiHangPlane.onClick.AddListener(ButtonToPaiHangPlaneOnClikHandle);

            Piggy = TopArea.Find("Piggy").GetOrAddComponent<Button>();
            Piggy.onClick.AddListener(PiggyOnClikHandle);

            MaxCon = TopArea.Find("MaxCon").GetOrAddComponent<Button>();
            // MaxCon.onClick.AddListener(() =>
            // {
            //     Debug.LogError("MaxConOnClikHandle");
            //     MaxSdk.ShowMediationDebugger();
            // });
            

            //Debug.LogError("2222");
            //钻石按钮
            TopArea.Find("CoinBut").GetComponent<Button>().onClick.AddListener(() =>
            {
                GameTool.CreatTip("继续玩赚取更多钻石(点击钻石提示)");
            });
            //  Debug.LogError("3333");
            Task = TopArea.Find("Task").GetComponent<Button>();
            Task.onClick.AddListener(TaskOnClikHandle);
            Red = Task.transform.Find("Red");

            OpenWriteDataBut = TopArea.Find("OpenWriteDataBut").GetComponent<Button>();
            OpenWriteDataBut.onClick.AddListener(OpenWriteDataButOnClikHandle);
            if (GameTool.isNeedCloseMoneyIcon)
            {
                OpenWriteDataBut.gameObject.SetActive(false);
                ButtonToPaiHangPlane.gameObject.SetActive(false);
            }
            SliderBubble.Init();
            bubbleShowCoroutine = StartCoroutine(CheckAndShowBubble());
            //设置玩家的金币文本数量
            SetPlayerCoinText();

        }
        //private void Update()
        //{
        //    GameManager.Instance.SetPos(transform.Find("bg/GamePos").GetComponent<RectTransform>());
        //}
        private void HideTaskRed()
        {
            Red.gameObject.SetActive(false);
        }

        private void ShowTaskRed()
        {
            Red.gameObject.SetActive(true);
        }

        private void ButtonToPaiHangPlaneOnClikHandle()
        {
            UIManager.Instance.ShowUI<PopRankPlane>();
        }

        private void OpenWriteDataButOnClikHandle()
        {
            if (GameTool.isNeedCloseMoneyIcon) return;
            if (!GameDataManager.CurrentGameData.isComWithdrawData)
            {
                UIManager.Instance.ShowUI<PopEnterInformation>();
            }
            else
            {
                UIManager.Instance.ShowUI<PopWithDrawMethod>();
            }
        }

        private void PiggyOnClikHandle()
        {
            //AppLovinMax.MaxSdk.ShowMediationDebugger();

            if (GameTool.isNeedCloseMoneyIcon) return;
              if (LevelManager.Instance.levelNum == 1)
            {
                TDAnalyticsManager.Instance.SendNewUserGuide(1);
            }
            UIManager.Instance.ShowUI<WithDrawPlane>();
        }

        private void SetPlayerCoinText()
        {
            TDAnalyticsManager.Instance.GetMoney(GameDataManager.CurrentGameData.piggyCount);
            PiggyCount.text = GameTool.GetDollarIconAndNum(GameDataManager.CurrentGameData.piggyCount);
            // 更新金币数量显示
            CoinCount.text = GameDataManager.CurrentGameData.coinCount.ToString();
            if (nowCoin == 0)
            {
                nowCoin = GameDataManager.CurrentGameData.coinCount;
            }
            else
            {
                if (nowCoin != GameDataManager.CurrentGameData.coinCount)
                {
                    //获取金币有飘字效果
                    CreatAddCoinEffect(GameDataManager.CurrentGameData.coinCount - nowCoin);
                    // 文本变化动画（金币）
                    PlayValueChangeAnimation(CoinCount, _coinTextBaseScale, _coinTextBaseColor);
                    nowCoin = GameDataManager.CurrentGameData.coinCount;
                }
            }
            // 美金文本变化动画
            if (Mathf.Abs(nowPiggy - GameDataManager.CurrentGameData.piggyCount) > 0.0001f && nowPiggy != -1)
            {
                PlayValueChangeAnimation(PiggyCount, _piggyTextBaseScale, _piggyTextBaseColor);
                CreatXuYingText(GameDataManager.CurrentGameData.piggyCount - nowPiggy);
            }
            nowPiggy = GameDataManager.CurrentGameData.piggyCount;
            UpdateDoubleButtonText();
            UpdateRocketButtonText();
            UpdateHoleButtonText();
        }
        private void CreatXuYingText(float v)
        {
            if (PiggyCountEffect == null)
            {
                PiggyCountEffect = PiggyCount.gameObject;
            }

            GameObject copy = Instantiate(PiggyCountEffect, PiggyCountEffect.transform.parent);
            DafultScript.PiggyCountEffect piggyEffect = copy.GetOrAddComponent<DafultScript.PiggyCountEffect>();
            piggyEffect.Init(v);
        }
        /// <summary>
        /// 获取金币有飘字效果
        /// </summary>
        /// <param name="v">增加的金钱数量</param>
        private void CreatAddCoinEffect(int v)
        {
            GameObject copy = Instantiate(CoinCountEffect, CoinCountEffect.transform.parent);
            CoinCountEffect CoinCountEffectT = copy.GetOrAddComponent<CoinCountEffect>();
            CoinCountEffectT.Init(v);
        }

        private void ButtonToOpenLuckPlaneOnClikHandle()
        {
            UIManager.Instance.ShowUI<LuckPlane>();
        }

        private void TaskOnClikHandle()
        {
            UIManager.Instance.ShowUI<TaskUI>();
        }

        private void OnDestroy()
        {
            EventManager.Instance.UnregisterEvent(GameEvent.ShowBoxPos, ShowBoxPos);
            EventManager.Instance.UnregisterEvent<Vector2, float>(GameEvent.CreatBoxCompleteCoinEffect, CreatBoxCompleteCoinEffect);
            EventManager.Instance.UnregisterEvent<float>(GameEvent.GetDollar, GetDollar);
            EventManager.Instance.UnregisterEvent<int>(GameEvent.GetProp1, GetProp1);
            EventManager.Instance.UnregisterEvent<int>(GameEvent.GetProp2, GetProp2);
            EventManager.Instance.UnregisterEvent<int>(GameEvent.GetProp3, GetProp3);
            EventManager.Instance.UnregisterEvent(GameEvent.SetPlayerCoinText, SetPlayerCoinText);
            EventManager.Instance.UnregisterEvent<int>(GameEvent.GetCoin, GetCoin);
            EventManager.Instance.UnregisterEvent(GameEvent.ShowTaskRed, ShowTaskRed);
            EventManager.Instance.UnregisterEvent(GameEvent.HideTaskRed, HideTaskRed);
            EventManager.Instance.UnregisterEvent(GameEvent.ShowCoinEffect, ShowCoinEffect);
            // 停止协程
            if (bubbleShowCoroutine != null)
            {
                StopCoroutine(bubbleShowCoroutine);
                bubbleShowCoroutine = null;
            }
        }

        private void ShowCoinEffect()
        {
            // 启动动画
            //StartCoroutine(FadeText());
            //  触发美金获取时的视觉反馈：文本动画 + 图标弹跳
            if (PiggyCount != null)
            {
                PlayValueChangeAnimation(PiggyCount, _piggyTextBaseScale, _piggyTextBaseColor);
            }
            if (IconPiggyCoinAll != null)
            {
                IconPiggyCoinAll.transform.DOKill();
                IconPiggyCoinAll.transform.localScale = Vector3.one;
                IconPiggyCoinAll.transform.DOScale(Vector3.one * 1.2f, 0.12f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        IconPiggyCoinAll.transform.DOScale(Vector3.one, 0.12f).SetEase(Ease.InBack);
                    });
            }
        }

        private IEnumerator FadeText()
        {
            Color originalColor = PiggyCount.color;
            float duration = 1.0f; // 动画持续时间
            float elapsedTime = 0f;

            // 渐隐
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                PiggyCount.color = new Color(originalColor.r, originalColor.g, originalColor.b,
                                              Mathf.Clamp01(1 - (elapsedTime / duration)));
                yield return null;
            }

            // 渐现
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                PiggyCount.color = new Color(originalColor.r, originalColor.g, originalColor.b,
                                              Mathf.Clamp01(elapsedTime / duration));
                yield return null;
            }
        }
        // 文本变化动画：轻微放大并颜色闪烁，再回到初始状态
        private Vector3 _coinTextBaseScale;
        private Vector3 _piggyTextBaseScale = Vector3.one;
        private Color _coinTextBaseColor;
        private Color _piggyTextBaseColor = Color.red;
        private void PlayValueChangeAnimation(Text target, Vector3 baseScale, Color baseColor)
        {
            if (target == null) return;
            target.transform.DOKill();
            target.DOKill();
            target.transform.localScale = baseScale;
            target.color = baseColor;

            // 创建一个新的序列（多次快速闪动，顺序执行，无重叠延迟）
            DG.Tweening.Sequence seq = DOTween.Sequence();
            int flashCount = 4;
            float upDuration = 0.06f;
            float downDuration = 0.06f;
            float interval = 0.02f;
            float scaleMultiplier = 1.2f;
            Color flashColor = new Color(1f, 0.95f, 0.4f, 1f);

            for (int i = 0; i < flashCount; i++)
            {
                // 放大并变亮
                seq.Append(target.transform.DOScale(baseScale * scaleMultiplier, upDuration));
                seq.Join(target.DOColor(flashColor, upDuration));
                // 还原
                seq.Append(target.transform.DOScale(baseScale * 0.8f, downDuration));
                seq.Join(target.DOColor(baseColor, downDuration));
                if (i < flashCount - 1)
                {
                    seq.AppendInterval(interval);
                }

            }

            // 确保最后一次动画完成后，文本返回到基础状态
            seq.OnComplete(() =>
            {
                target.transform.localScale = baseScale;
                target.color = baseColor;
            });
        }

        private void GetCoin(int obj)
        {
            GameTool.UIGetAPosAndCreat2DObjToThisPosAnimator(IconCoinAll.GetComponent<RectTransform>(), "GP_IC_COINS_d1", 10, delegate ()
            {
                GameDataManager.AddCoinCount(obj, 0);
            });
        }

        private void GetProp1(int obj)
        {
            GameTool.UIGetAPosAndCreat2DObjToThisPosAnimator(buttonAddHole.GetComponent<RectTransform>(), "Gamplay_IT_hole_gameplay", obj, delegate ()
            {
                MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.Hole);
            });
        }
        private void GetProp2(int obj)
        {
            GameTool.UIGetAPosAndCreat2DObjToThisPosAnimator(buttonRocket.GetComponent<RectTransform>(), "Gamplay_IT_rocket_gameplay", obj, delegate ()
            {
                MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.Rocket);
            });
        }
        private void GetProp3(int obj)
        {
            GameTool.UIGetAPosAndCreat2DObjToThisPosAnimator(buttonDoubleBox.GetComponent<RectTransform>(), "Gamplay_IT_box_yellow_gameplay", obj, delegate ()
            {
                MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.DoubleBox);
            });
        }

        private IEnumerator CheckAndShowBubble()
        {
            yield break;
            while (true)
            {
                if (!SliderBubble.gameObject.activeSelf)
                {
                    // 等待随机时间
                    yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));

                    // 如果UI还存在（防止在等待过程中UI被销毁）
                    if (SliderBubble != null && this != null)
                    {
                        SliderBubble.ResetBubblePosition();
                    }
                }
                yield return null;
            }
        }
        private void GetDollar(float obj)
        {
                                    DOVirtual.DelayedCall(0.4f, ()=>
            {
                AudioManager.Instance.PlaySFX("OnlyGetMoney");
            });
            GameTool.UIGetAPosAndCreat2DObjToThisPosAnimator(IconPiggyCoinAll.GetComponent<RectTransform>(), $"coin_{GameTool.dollarIconPath}", 10, delegate ()
            {
                GameDataManager.AddCoinCount(0, obj);

                //if (!GameDataManager.CurrentGameData.isShowMoneyGuite)
                //{
                //    GameDataManager.CurrentGameData.isShowMoneyGuite=true;
                //    GuiteItem guiteItem = new GuiteItem()
                //    {
                //        dexText = "点击金钱",
                //        DesImageX = 0,
                //        DesImageY = 894,
                //        circleX = 279,
                //        circleY = 3000f,
                //        handleX = 360,
                //        handleY = 3000,
                //        isNeedShowButton = true,
                //        maskType = 1,
                //        x1 = -18.3f,
                //        y1 = 1220f,
                //        x2 = 257.7f,
                //        y2 = 1315.2f,
                //        isNeedShowClikTip = true,
                //        TdIndex = 7
                //    };
                //  EventManager.Instance.TriggerEvent<GuiteItem>(GameEvent.SetMaskRect, guiteItem);
                //}
            });
            return;
        }
        //private void Update()
        //{
        //    //if (Input.GetKeyDown(KeyCode.Q))
        //    //{
        //    //    GameAnimatorContor.Instance.ShowACoin(IconCoinAll);
        //    //}
        //}
        private void CreatBoxCompleteCoinEffect(Vector2 worldPos, float dollarNum)
        {
            // 1. 获取目标UI元素(IconCoinAll)的世界坐标位置
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, IconPiggyCoinAll.transform.position);
            Vector3 targetWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0));
            targetWorldPoint.z = 0;

            // 2. 在初始位置(worldPos)生成CoinIcon预制体
            GameObject coinSprite = Instantiate(GameAnimatorContor.Instance.DollarIcon.gameObject,
                new Vector3(worldPos.x, worldPos.y, 0),
                Quaternion.identity);
            coinSprite.gameObject.SetActive(true);
            // 3. 使用DOTween制作移动动画
            coinSprite.transform.DOMove(targetWorldPoint, 0.5f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    // 销毁金币物体
                    Destroy(coinSprite);
                    GameDataManager.AddCoinCount(0, dollarNum);//收集一个箱子，给美金
                                                               // 目标金币图标的缩放动画
                    IconPiggyCoinAll.transform.DOScale(Vector3.one * 1.2f, 0.1f)
                        .OnComplete(() =>
                        {
                            IconPiggyCoinAll.transform.DOScale(Vector3.one, 0.1f);
                        });
                });
        }
        private void ShowBoxPos()
        {
            //ButtonBosPos3.gameObject.SetActive(true);
            //ButtonBosPos4.gameObject.SetActive(true);
        }

        private void BosPos3Event()
        {
            //GameManager.Instance.ToLockNewPositon(3);
            //ButtonBosPos3.gameObject.SetActive(false);
        }


        private void BosPos4Event()
        {
            //GameManager.Instance.ToLockNewPositon(4);
            //ButtonBosPos4.gameObject.SetActive(false);
        }


        private void Start()
        {
            SetItemButtonFalse();
            InitButon();


        }
        private void ShowExitUI()
        {

        }
        private void SettingEvent()
        {
            UIManager.Instance.ShowUI<SettingUI>();
        }
        #region 道具按钮
        /// <summary>
        /// 洞口道具
        /// </summary>
        private void AddHole()
        {
            AudioManager.Instance.PlaySFX("Click");
            //没有足够的道具
            if (GameDataManager.CurrentGameData.holeItemCount <= 0)
            {
                UIManager.Instance.ShowUI<ExtraHole>();
                return;
            }

            if (GameManager.Instance.CurScrew != null && GameManager.Instance.CurScrew.IsMoving)
            {
                Debug.Log("当前小球正在移动，无法添加");
                return;
            }

            //没有达到洞口道具上线
            if (MainSceneUI.Instance.emptyHoleManager.CanAddExtraHole())
            {
                GameManager.Instance.AddExtraHole();
                GameDataManager.AddItemCount(ItemType.Hole, -1);
                UpdateHoleButtonText();
                EventManager.Instance.TriggerEvent(GameEvent.HideGuitePlane);
            }
            else
            {
                GameTool.CreatTip("可使用次数不足");
                Debug.Log("洞口道具已经达到上线");
            }


        }

        /// <summary>
        /// 火箭道具
        /// </summary>
        private void RocketClick()
        {
            //没有足够的道具了
            AudioManager.Instance.PlaySFX("Click");
            if (GameDataManager.CurrentGameData.rocketItemCount <= 0)
            {
                UIManager.Instance.ShowUI<ExtraRocket>();
                return;
            }
            if (LevelManager.Instance.pro2UseCount >= GameTool.limtPro2Count)
            {
                GameTool.CreatTip("可使用次数不足");
                return;
            }
            UIManager.Instance.ShowUI<RocketUI>();
            EventManager.Instance.TriggerEvent(GameEvent.HideGuitePlane);
            Invoke(nameof(DelaySetRocketClick), 0.1f);
        }

        //延迟设置为火箭点击状态(避免UI点击事件和Input发生冲突)
        private void DelaySetRocketClick()
        {
            GameManager.Instance.SetISRocketClick(true);
        }

        /// <summary>
        /// 洞口道具
        /// </summary>
        private void DoubleBox()
        {
            AudioManager.Instance.PlaySFX("Click");
            //没有足够的道具了
            if (GameDataManager.CurrentGameData.doubleBoxItemCount <= 0)
            {
                UIManager.Instance.ShowUI<ExtraBox>();
                return;
            }
            if (LevelManager.Instance.pro3UseCount >= GameTool.limtPro3Count)
            {
                GameTool.CreatTip("可使用次数不足");
                return;
            }
            if (GameManager.Instance.CurScrew != null && GameManager.Instance.CurScrew.IsMoving)
            {
                Debug.Log("当前小球正在移动，无法添加");
                return;
            }
            if (!MainSceneUI.Instance.emptyHoleManager.HaveScrew())
            {
                UIManager.Instance.ShowUI<AlertUI>();
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("暂无可移除的螺丝");
                return;
            }

            //解锁一个新的箱子位置
            //GameManager.Instance.ToLockNewPositon();
            LevelManager.Instance.pro3UseCount++;
            GameDataManager.AddItemCount(ItemType.DoubleBox, -1);
            EventManager.Instance.TriggerEvent(GameEvent.HideGuitePlane);
            GameManager.Instance.SetEmptyHoleToPropEmpty();//把螺丝放入道具盒
            UpdateDoubleButtonText();
        }

        public void UpdateRocketText()
        {
            GameDataManager.AddItemCount(ItemType.Rocket, -1);
            UpdateRocketButtonText();
        }

        #endregion
        /// <summary>
        ///  重新开始游戏
        /// </summary>
        private void RefreshGame()
        {

        }

        /// <summary>
        /// 设置关卡数
        /// </summary>
        /// <param name="val"></param>
        public void SetLevelNum(int num)
        {
            if (LevelManager.Instance.CurLevel.HasClock)
                return;

            levelText.gameObject.SetActive(true);
            clockSlider.gameObject.SetActive(false);
            levelText.SetTextWithParameter("<size=40>关卡</size><size=56> {0}</size>", num);
            //Level { 1}   Nível {1}
            // levelText.text = $"<size=40>LV. </size><size=56>{num}</size>";
        }

        /// <summary>
        /// 是否展示提示
        /// </summary>
        /// <param name="val"></param>
        public void SetTips(bool val, string tip = "")
        {
            if (levle1TipText != null)
                levle1TipText.SetActive(val);

            levle1TipText.GetComponentInChildren<Text>().text = tip;
        }

        /// <summary>
        /// 初始化所有的按钮为未解锁状态
        /// </summary>
        private void SetItemButtonFalse()
        {
            addHoleTrans.Find("Lock").gameObject.SetActive(true);
            buttonAddHole.gameObject.SetActive(false);

            rocketTrans.Find("Lock").gameObject.SetActive(true);
            buttonRocket.gameObject.SetActive(false);

            doubleBoxTrans.Find("Lock").gameObject.SetActive(true);
            buttonDoubleBox.gameObject.SetActive(false);
        }

        public void GameUIUnlocked(ItemType type)
        {
            if (type == ItemType.Hole)
            {
                addHoleTrans.Find("Lock").gameObject.SetActive(false);
                buttonAddHole.gameObject.SetActive(true);
            }
            else if (type == ItemType.Rocket)
            {
                rocketTrans.Find("Lock").gameObject.SetActive(false);
                buttonRocket.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 检查是否解锁了相应的道具
        /// </summary>
        public void InitButon()
        {
            //解锁了道具一
            if (GameDataManager.CurrentGameData.isHoleLocked == false)
            {
                addHoleTrans.Find("Lock").gameObject.SetActive(false);
                buttonAddHole.gameObject.SetActive(true);
                UpdateHoleButtonText();
            }
            //解锁了道具二
            if (GameDataManager.CurrentGameData.isRocketLocked == false)
            {
                rocketTrans.Find("Lock").gameObject.SetActive(false);
                buttonRocket.gameObject.SetActive(true);
                UpdateRocketButtonText();
            }
            //解锁了道具三
            if (GameDataManager.CurrentGameData.isDoubleBoxLocked == false)
            {
                doubleBoxTrans.Find("Lock").gameObject.SetActive(false);
                buttonDoubleBox.gameObject.SetActive(true);
                UpdateDoubleButtonText();
            }
        }

        private void UpdateDoubleButtonText()
        {
            buttonDoubleBox.transform.Find("btnAdd/Text Count").GetComponent<Text>().text
                = GameDataManager.CurrentGameData.doubleBoxItemCount == 0 ? "+" : GameDataManager.CurrentGameData.doubleBoxItemCount.ToString();
        }

        private void UpdateRocketButtonText()
        {
            buttonRocket.transform.Find("btnAdd/Text Count").GetComponent<Text>().text
                = GameDataManager.CurrentGameData.rocketItemCount == 0 ? "+" : GameDataManager.CurrentGameData.rocketItemCount.ToString();
        }

        private void UpdateHoleButtonText()
        {
            buttonAddHole.transform.Find("btnAdd/Text Count").GetComponent<Text>().text
                = GameDataManager.CurrentGameData.holeItemCount == 0 ? "+" : GameDataManager.CurrentGameData.holeItemCount.ToString();
        }

        /// <summary>
        /// 设置对应的道具文本
        /// </summary>
        /// <param name="type"></param>
        public void UpdateItemCount(ItemType type)
        {

            switch (type)
            {
                case ItemType.Hole:
                    UpdateHoleButtonText();
                    break;
                case ItemType.Rocket:
                    UpdateRocketButtonText();
                    break;
                case ItemType.DoubleBox:
                    UpdateDoubleButtonText();
                    break;
            }
        }

        //解锁道具
        public void UnlockedItem()
        {
            addHoleTrans.Find("Lock").gameObject.SetActive(false);
            buttonAddHole.gameObject.SetActive(true);
            rocketTrans.Find("Lock").gameObject.SetActive(false);
            buttonRocket.gameObject.SetActive(true);
            doubleBoxTrans.Find("Lock").gameObject.SetActive(false);
            buttonDoubleBox.gameObject.SetActive(true);
        }

        //显示倒计时
        public void ShowClockUI(int m, int s)
        {
            levelText.gameObject.SetActive(false);
            clockSlider.gameObject.SetActive(true);
            clockSlider.SetTime(m, s);
        }

        internal RectTransform GetItemEndPos(ConfItem confItem)
        {
            if (confItem.Sn == 1)
            {
                return IconCoinAll.GetComponent<RectTransform>();
            }
            else if (confItem.Sn == 2)
            {
                return IconPiggyCoinAll.GetComponent<RectTransform>();
            }
            else if (confItem.Sn == 3)
            {
                return buttonAddHole.GetComponent<RectTransform>();
            }
            else if (confItem.Sn == 4)
            {
                return buttonRocket.GetComponent<RectTransform>();
            }
            else if (confItem.Sn == 5)
            {
                return buttonDoubleBox.GetComponent<RectTransform>();
            }
            return IconCoinAll.GetComponent<RectTransform>();
        }
    }
}
