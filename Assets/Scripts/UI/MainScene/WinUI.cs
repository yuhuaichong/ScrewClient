using cfg;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class WinUI : MainBaseUI
    {
        private Transform buttonTable;
        private Button buttonNext;
        private Button buttonVideo;
        private CanvasGroup canvasGroupNext;
        private CanvasGroup canvasGroupVideo;

        private Transform topTrans;
        private Text piggyCount;
        private Text coinCount;

        private Transform future;
        private Image fillImage;
        private Image fillIcon;
        private Text fillText;
        [SerializeField] private int curCount;//当前解锁的进度
        [SerializeField] private int fillCount = 10;

        public Transform CoinTrans { get; set; }
        public Transform PiggyTrans { get; set; }
        [SerializeField] private ParticleSystem ps1;
        [SerializeField] private ParticleSystem ps2;

        [Header("解锁物体")]
        [SerializeField] private Sprite starPin;
        [SerializeField] private Sprite rope;
        [SerializeField] private Sprite ice;
        [SerializeField] private Sprite door;
        [SerializeField] private Sprite boom;
        [SerializeField] private Sprite chain;
        [SerializeField] private Sprite key;
        [SerializeField] private Sprite clock;
        public Text PiggyText
        {
            get => piggyCount;
        }
        public Text CoinText
        {
            get => coinCount;
        }

        private bool isClick = false;//只允许点击一次


        public Button Next;
        public Button GetMore;
        LanguageText LevelText;
        LanguageText CoinNum;
        LanguageText DoubleCoinText;
        int canGetNum;
        int isDouble;
        LanguageText DesText;
        Image MoneyCoin;
        protected override void Awake()
        {
            base.Awake();
            DesText = tableTransform.Find("DesText").GetComponent<LanguageText>();
            MoneyCoin = tableTransform.Find("MoneyCoin").GetComponent<Image>();
            MoneyCoin.sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_big_{GameTool.dollarIconPath}");
            MoneyCoin.sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_big_{GameTool.dollarIconPath}");
            Next = tableTransform.Find("Next").GetComponent<Button>();
            Next.onClick.AddListener(() =>
            {
                //isDouble = 1;
                //EventManager.Instance.TriggerEvent<int>(GameEvent.GetCoin, canGetNum * isDouble);
                OnNextButtonClicked();
            });
            GetMore = tableTransform.Find("GetMore").GetComponent<Button>();
            GetMore.onClick.AddListener(() =>
            {
                GameVideoContor.ShowVideoAd(VedioAdType.关卡胜利双倍奖励, VedioAdType.关卡胜利双倍奖励.ToString(), delegate (bool isCom, int a)
                {
                    isDouble = 2;
                    EventManager.Instance.TriggerEvent<int>(GameEvent.GetCoin, canGetNum * isDouble);
                    OnNextButtonClicked();
                }, null);

            });
            LevelText = tableTransform.Find("LevelText").GetComponent<LanguageText>();
            CoinNum = tableTransform.Find("CoinNum").GetComponent<LanguageText>();
            DoubleCoinText = tableTransform.Find("GetMore/DoubleCoinText").GetComponent<LanguageText>();
            //// 获取按钮和子物体
            //buttonTable = tableTransform.Find("Table");
            //buttonNext = buttonTable.Find("Button Next").GetComponent<Button>();
            //buttonVideo = buttonTable.Find("Button Video").GetComponent<Button>();

            //// 添加按钮点击事件
            //buttonNext.onClick.AddListener(OnNextButtonClicked);
            //buttonVideo.onClick.AddListener(OnVideoButtonClicked);

            //// 初始化 CanvasGroup（用于控制透明度）
            //canvasGroupNext = buttonNext.gameObject.AddComponent<CanvasGroup>();
            //canvasGroupVideo = buttonVideo.gameObject.AddComponent<CanvasGroup>();

            //topTrans = tableTransform.parent.Find("Top").transform;
            //piggyCount = topTrans.Find("Piggy/Count").GetComponent<Text>();
            //coinCount = topTrans.Find("Total Coin/Count").GetComponent<Text>();

            //PiggyTrans = topTrans.Find("Piggy/Icon Piggy Coin All").transform;
            //CoinTrans = topTrans.Find("Total Coin/Icon Coin All").transform;

            //future = tableTransform.Find("Contain Feature/future").transform;
            //fillImage = future.Find("Image").GetComponent<Image>();
            //fillIcon = future.Find("Icon").GetComponent<Image>();
            //fillText = fillImage.transform.GetComponentInChildren<Text>();

            //// 设置初始状态
            //ResetButtonState();
        }
        private void InitPs()
        {
            //SetPsOrinPos();
            //ps1.gameObject.SetActive(true);
            //ps2.gameObject.SetActive(true);
            //Invoke(nameof(DelaySetPSPos), 0.8f);
        }
        private void ResetButtonState()
        {
            //// 初始化按钮大小和透明度
            //buttonNext.transform.localScale = Vector3.zero;
            //buttonVideo.transform.localScale = Vector3.zero;
            //canvasGroupNext.alpha = 0;
            //canvasGroupVideo.alpha = 0;
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(PlayShowAnimation/*, UIEffectType.Fade*/);
                        GameTool.isWinIng = true;//正在胜利中，不上报失败埋点
            AudioManager.Instance.PlaySFX("Win");
            AudioManager.Instance.PlaySFX("WinNoti");
            DesText.text = GameTool.GetDollarIconAndNum(GameDataManager.CurrentGameData.piggyCount);
            UIManager.Instance.HideUI<BoxNumRewardUI>();
            isClick = false;
            TDAnalyticsManager.Instance.SendPassLevel(GameDataManager.CurrentGameData.levelNum, 1);
            int nowLevel = GameDataManager.CurrentGameData.levelNum;
            LevelText.SetTextWithParameter("关卡{0}", nowLevel);
            canGetNum = GetLevelCanGetCoin(nowLevel);
            CoinNum.text = canGetNum.ToString();
            DoubleCoinText.text = (canGetNum * 2).ToString();
            LevelManager.Instance.AddLevelNum();
            //isClick = false;
            //piggyCount.text = GameDataManager.CurrentGameData.piggyCount.ToString();
            //coinCount.text = GameDataManager.CurrentGameData.coinCount.ToString();

            //InitPs();

            //int curLevelNum = LevelManager.Instance.GetLevleNum();
            //switch (curLevelNum)
            //{
            //    case 1:
            //        //开始解锁 星星螺丝
            //        GameDataManager.ReStartFill(10, starPin.name);
            //        break;
            //    case 11:
            //        //开始解锁 绳子
            //        GameDataManager.ReStartFill(15, rope.name);
            //        break;
            //    case 26:
            //        //开始解锁 冰
            //        GameDataManager.ReStartFill(15, ice.name);
            //        break;
            //    case 41:
            //        //开始解锁 门
            //        GameDataManager.ReStartFill(15, door.name);
            //        break;
            //    case 56:
            //        //开始解锁 炸弹
            //        GameDataManager.ReStartFill(15, boom.name);
            //        break;
            //    case 71:
            //        //开始解锁 锁链
            //        GameDataManager.ReStartFill(15, chain.name);
            //        break;
            //    case 86:
            //        //开始解锁钥匙
            //        GameDataManager.ReStartFill(15, key.name);
            //        break;
            //    case 101:
            //        //开始解锁闹钟
            //        GameDataManager.ReStartFill(15, clock.name);
            //        break;
            //    case 116:
            //        //道具已经解锁完毕
            //        future.parent.gameObject.SetActive(false);
            //        break;
            //}

            //if (curLevelNum >= 10 && GameDataManager.CurrentGameData.isStreaklocked == false)
            //{
            //    //11关开始后开始增加Streak
            //    GameDataManager.AddStreak();
            //}

            //if (curLevelNum >= 20 && GameDataManager.CurrentGameData.isLuckySpinlocked == false)
            //{
            //    //从20关开始增加抽奖票进度
            //    GameDataManager.AddSpinProgress();
            //}

            ////得到当前精灵图片
            //Sprite curSprite = GetSpriteByName(GameDataManager.CurrentGameData.fillSpriteName);
            //fillIcon.sprite = curSprite;
            //LevelManager.Instance.UnlockSprite = curSprite;

            ////增加当前的填充数
            //GameDataManager.AddCurFillCount();
            //curCount = GameDataManager.CurrentGameData.curFillCount;
            //fillCount = GameDataManager.CurrentGameData.allfillCount;

            //fillImage.fillAmount = curCount / (float)fillCount;
            //fillText.text = $"{curCount}/{fillCount}";

            //增加当前的关卡数

        }

        private int GetLevelCanGetCoin(int nowLevel)
        {
            ConfWinReward confWinReward = ConfigModule.Instance.Tables.TbWinReward.GetOrDefault(nowLevel);
            if (confWinReward == null)
            {
                return 500;
            }
            else
            {
                return confWinReward.CaneGetCoinNum;
            }

        }

        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            //按钮消失
            PlayHideAnimation();

            float hideDelayTime = 0.1f;

            //if (LevelManager.Instance.GetLevleNum() <= 5 || true)
            //{
            //    //添加金币，进行数据存储
            //    int prePiggyCount = GameDataManager.CurrentGameData.piggyCount;
            //    int preCoinCount = GameDataManager.CurrentGameData.coinCount;
            //    GameDataManager.AddCoinCount(10, 30);
            //    int curPiggyCount = GameDataManager.CurrentGameData.piggyCount;
            //    int curCoinCount = GameDataManager.CurrentGameData.coinCount;


            //    ItemMoveManager.Instance.MoveCoin(preCoinCount, curCoinCount, prePiggyCount, curPiggyCount);
            //}
            //else
            //{
            //    hideDelayTime = 0;
            //}

            base.HideUI(hideDelayTime, () =>
            {
                ps1.gameObject.SetActive(false);
                ps2.gameObject.SetActive(false);
                HideCallBack();
            });
        }

        public Sprite GetSpriteByName(string name)
        {
            // 遍历所有的Sprite并对比它们的name
            if (starPin.name == name) return starPin;
            else if (rope.name == name) return rope;
            else if (ice.name == name) return ice;
            else if (door.name == name) return door;
            else if (boom.name == name) return boom;
            else if (chain.name == name) return chain;
            else if (key.name == name) return key;
            else if (clock.name == name) return clock;

            // 如果没有找到匹配的Sprite
            Debug.LogWarning($"Sprite with name '{name}' not found.");
            return null;
        }
        /// <summary>
        /// 按钮从小到大的动画
        /// </summary>
        private void PlayShowAnimation()
        {
            // 按钮从小到大并渐显
            Next.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            // canvasGroupNext.DOFade(1, 0.5f);

            GetMore.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            //canvasGroupVideo.DOFade(1, 0.5f);
        }

        private void PlayHideAnimation()
        {
            // 按钮从大到小并渐隐
            Next.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
            // canvasGroupNext.DOFade(0, 0.5f);

            GetMore.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

            //canvasGroupVideo.DOFade(0, 0.5f);
        }

        private void OnNextButtonClicked()
        {
            if (isClick)
                return;
            isClick = true;
    GameTool.isWinIng = false;//胜利状态结束
            LevelManager.Instance.ClearLevel();
            UIManager.Instance.HideUI<WinUI>();
        }

        private void HideCallBack()
        {
            ////关卡小于5，点击next按钮直接去到下一关
            //if (LevelManager.Instance.GetLevleNum() <= 5 || true)
            //{
            LevelManager.Instance.InitLevel();

            //}
            //else
            //{
            //    //返回到主场景

            //    GameManager.Instance.EnterHomeScene();

            //    //添加金币，进行数据存储
            //    int prePiggyCount = GameDataManager.CurrentGameData.piggyCount;
            //    int preCoinCount = GameDataManager.CurrentGameData.coinCount;
            //    GameDataManager.AddCoinCount(10, 30);
            //    int curPiggyCount = GameDataManager.CurrentGameData.piggyCount;
            //    int curCoinCount = GameDataManager.CurrentGameData.coinCount;


            //    ItemMoveManager.Instance.MoveCoin(preCoinCount, curCoinCount, prePiggyCount, curPiggyCount);
            //    //播放星星移动动画
            //    ItemMoveManager.Instance.MoveAndRotateStar();

            //    //从11关开始，移动Arrow
            //    if (LevelManager.Instance.GetLevleNum() >= 11 && GameDataManager.CurrentGameData.isStreaklocked == false)
            //    {
            //        HomeSceneUI.Instance.homeUI.HomeItemMove.MoveArrowAnim();
            //    }

            //    //从20关卡，移动sipn
            //    if (LevelManager.Instance.GetLevleNum() >= 20 && GameDataManager.CurrentGameData.isLuckySpinlocked == false)
            //    {
            //        HomeSceneUI.Instance.homeUI.HomeItemMove.MoveSpinAnim();
            //    }
            //    HomeSceneUI.Instance.homeUI.UpdateHomePlayText();
            //}
        }
        private void OnVideoButtonClicked()
        {
            // 可以在这里定义视频按钮的功能
            Debug.Log("Video button clicked!");
        }
        private void SetPsOrinPos()
        {
            ps1.transform.position = new Vector3(-6, -6, 0);
            ps2.transform.position = new Vector3(6, -6, 0);
        }
        private void DelaySetPSPos()
        {
            ps1.transform.position = new Vector3(-6, 12, 0);
            ps2.transform.position = new Vector3(6, 12, 0);
        }
    }
}
