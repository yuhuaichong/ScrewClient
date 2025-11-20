using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace DafultScript
{
    public enum GameState
{
    Stop, Start
}
public class BosChanceData
{
    public bool isNeedChance;
    public int moveHowNumBox;
    public int moveHowDic;
}

    public class GameManager : MonoBehaviour
    {
        #region 组件
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject singleton = new GameObject("GameManager");
                        instance = singleton.AddComponent<GameManager>();
                    }
                }
                return instance;
            }
        }

        public Transform boxLevel;

        private RocketSpawner rocketController; //火箭生成
        private EmptyHoleManager emptyHoleManager;//空槽
        #endregion

        #region 箱子
        public List<Box> boxList = new List<Box>();//箱子的列表
        public List<Box> nowCanSetScrewBoxList = new List<Box>();//箱子的列表
        public int nowNeedOnStageBoxIndex;//现在需要上场的箱子下标
        public int completeBox;


        public Dictionary<Vector2, bool> boxPosPosition = new Dictionary<Vector2, bool>()
    {       
        // { new Vector2(1.63f,10.2f),false },
        //{ new Vector2(4.92f,10.2f),false },
        //{ new Vector2(-1.673f,10.2f),false },
        //{ new Vector2(-4.87f,10.2f),false },

    { new Vector2(1.64f, 10.2f), false },
    { new Vector2(4.92f, 10.2f), false },
    { new Vector2(-1.64f, 10.2f), false },
  { new Vector2(-4.92f, 10.2f), false },

              //{ new Vector2(-4.95f,10.65f),false },
              //{ new Vector2(-1.59f,10.65f),false },
              // { new Vector2(1.71f,10.65f),false },
              //  { new Vector2(4.87f,10.65f),false },
    };//挂载添加位置
        public int nowLockBoxPosNum = 2;//解锁了几个箱子的位置，默认2个
        public bool isLockThree;//解锁了第三个
        public bool isLockFour;//解锁了第四个
        private bool isRocketClick;//当前是否处于火箭点击的状态
        public void SetRocketClickFalse()
        {
            isRocketClick = false;
        }
        [Range(0f, 1f)]
        [SerializeField] private float screenFraction = 0.125f; // 从顶部向下的位置比例，默认是 1/8
        private Vector3 boxMoveLeftPos;
        private Vector3 boxMoveCenterPos;
        private Vector3 boxMoveRightPos;
        private Vector3 boxCenterLeftPos;
        public Vector3 boxCenterTopPos;
        private Vector3 boxCenterRightPos;
        public Vector3 RightPos
        {
            get
            {
                return boxMoveRightPos;
            }
        }
        #endregion

        private GameState gameState;//当前游戏状态
        public GameState _GameState { get => gameState; }
        public void SetGameState(GameState state)
        {
            gameState = state;
        }


        [SerializeField] private GameObject HomeScene;
        [SerializeField] private GameObject NeedLockBoxPos1;
        [SerializeField] private GameObject NeedLockBoxPos2;
        [SerializeField] private GameObject EmptyTip;
        public bool isLose;
        public Screw CurScrew { get; private set; }//记录当前小球是否在移动
        public void SetISRocketClick(bool value)
        {
            isRocketClick = value;
        }
        public int HardStart;
        public int hardOver;
        public List<ScrewColor> nowLevelScrewSort;
        public int nowLevelScrewColorData;
        private Queue<int> levelQueue;                // 关卡顺序队列（第一轮顺序，其后每轮洗牌）
        private List<ScrewColor> colorBuffer;         // 未来颜色缓冲（仅包含尚未消费的颜色）
        private const int windowCap = 120;            // 难度调整窗口大小
        private const int refillThreshold = 80;       // 低于该阈值则补充缓冲
        private const int minBufferSize = 120;        // 每次补充的目标缓冲最小长度
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            nowLevelScrewSort = new List<ScrewColor>();
            rocketController = GetComponent<RocketSpawner>();
            NeedLockBoxPos1 = transform.Find("NeedLockBoxPos1").gameObject;
            NeedLockBoxPos1.AddComponent<NeedLockBoxPos>().Init(3);
            NeedLockBoxPos2 = transform.Find("NeedLockBoxPos2").gameObject;
            NeedLockBoxPos2.AddComponent<NeedLockBoxPos>().Init(4);
            EmptyTip = transform.Find("EmptyTip").gameObject;
            EmptyTip.AddComponent<EmptyTip>().Init();
            int count = 0;
            foreach (var item in boxPosPosition)
            {
                count++;
                if (count == 2)
                {
                    NeedLockBoxPos1.transform.localPosition = item.Key;
                    //           Debug.LogError(NeedLockBoxPos1.transform.localPosition + "广告箱子的局部位置");
                }
                else if (count == 1)
                {
                    NeedLockBoxPos2.transform.localPosition = item.Key;
                    //            Debug.LogError(NeedLockBoxPos2.transform.localPosition + "广告箱子的局部位置");
                }
            }


        }


        private void Start()
        {

            LoadAssetText();//加载资源测试


            emptyHoleManager = transform.Find("EmptyHoleManager").GetComponent<EmptyHoleManager>();

            if (gameState == GameState.Stop)
                emptyHoleManager.gameObject.SetActive(false);

            EnterMainScene();
            LevelManager.Instance.InitLevel();
            //检查是否需要打开每日奖励界面
            //StartCoroutine(CheckNeedOpenDayPlane());
            //检查是否需要打开评价界面
            StartCoroutine(CheckNeedOpenAppRaisePlane());
            //计算箱子移动的点位
            CalculatePositions();
            EventManager.Instance.RegisterEvent<int>(GameEvent.HideAddBoxBut, HideAddBoxBut);
            EventManager.Instance.RegisterEvent<Box>(GameEvent.OneBoxCom, OneBoxCom);
        }
        private void OnDestroy()
        {
            EventManager.Instance.UnregisterEvent<int>(GameEvent.HideAddBoxBut, HideAddBoxBut);
            EventManager.Instance.UnregisterEvent<Box>(GameEvent.OneBoxCom, OneBoxCom);
        }

        private void OneBoxCom(Box obj)
        {
            //if (LevelManager.Instance.levelNum <= 3) return;
            ////boxList.Remove(obj);
            //if (boxList.Count - nowNeedOnStageBoxIndex <= 2)
            //{
            //    LevelManager.Instance.CreatNextLevel();
            //}
        }

        private void HideAddBoxBut(int obj)
        {
            if (obj == 3)
            {
                NeedLockBoxPos1.SetActive(false);
            }
            else
            {
                NeedLockBoxPos2.SetActive(false);
            }
        }

        private void LoadAssetText()
        {
            //GameObject go=Instantiate( ResourceLoader.Instance.GetRes<GameObject>("Prefab/Hole/box.prefab"));
            //Instantiate(ResourceLoader.Instance.GetFxGameObject("Expolosion"));
        }
        /// <summary>
        /// 检查是否需要打开评价界面
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckNeedOpenAppRaisePlane()
        {
            if (GameDataManager.CurrentGameData.levelNum <= 2)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
            if (!GameDataManager.CurrentGameData.isOpenAppraisePlane && false)
            {
                if (GameDataManager.CurrentGameData.completeBoxNum >= GameTool.howManeyToOpenAppraisePlane)
                {
                    UIManager.Instance.ShowUI<AppraisePlaneUI>();
                    UIManager.Instance.GetUI<AppraisePlaneUI>().callback = () =>
                    {
                        int loginday = GameDataManager.CurrentGameData.dayLoginRewardCompleteDci.Count;
                        if (loginday < 7)
                        {
                            if (GameTool.isGetNowSevertTimeSucess &&
                            GameDataManager.CurrentGameData.DayLoginRewardGetTime.Date != GameTool.nowSevertTime.Date)
                            {
                                UIManager.Instance.ShowUI<TaskUI>();
                                GameTool.isShowTaskUI = true;
                            }

                        }
                    };
                }
                else
                {
                    int loginday = GameDataManager.CurrentGameData.dayLoginRewardCompleteDci.Count;
                    if (loginday < 7)
                    {
                        if (GameTool.isGetNowSevertTimeSucess &&
                        GameDataManager.CurrentGameData.DayLoginRewardGetTime.Date != GameTool.nowSevertTime.Date)
                        {
                            UIManager.Instance.ShowUI<TaskUI>();
                            GameTool.isShowTaskUI = true;
                        }
                    }
                }
            }
            else
            {
                int loginday = GameDataManager.CurrentGameData.dayLoginRewardCompleteDci.Count;
                if (loginday < 7)
                {
                    if (GameTool.isGetNowSevertTimeSucess && !GameTool.isShowTaskUI &&
                    GameDataManager.CurrentGameData.DayLoginRewardGetTime.Date != GameTool.nowSevertTime.Date)
                    {
                        UIManager.Instance.ShowUI<TaskUI>();
                        GameTool.isShowTaskUI = true;
                    }
                }
            }
        }
        IEnumerator CheckNeedOpenDayPlane()
        {
            yield return new WaitForSeconds(1);
            int nowDay = GameTool.GetNowDay(DateTime.Now, GameDataManager.CurrentGameData.oneLogingInGameTime);
            foreach (var item in GameDataManager.CurrentGameData.dayGiftGetStatu)
            {
                if (item.Key <= nowDay && !item.Value)
                {
                    yield break;
                }
            }
        }

        // 检测点击的位置并发送射线
        private void Update()
        {
            ////  GameText();
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    AppLovinMax.MaxSdk.ShowMediationDebugger();
            //}
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    AdjustEvent adjustEvent = new AdjustEvent("9d3s7q");
            //    Adjust.TrackEvent(adjustEvent);
            //    Debug.LogError("Adjuset测试");
            //}

            if (Input.GetMouseButtonDown(0)) // 当鼠标左键被按下
            {
                if (gameState == GameState.Stop)
                {
                    //Debug.Log("游戏状态为结束状态，无法点击");
                    return;
                }
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 将屏幕坐标转换为世界坐标
                Vector3 rayOrigin = new Vector3(mousePosition.x, mousePosition.y, 0);

                #region 火箭效果点击
                //点击发射火箭
                if (isRocketClick)
                {
                    Collider2D[] glass = Physics2D.OverlapCircleAll(rayOrigin, 0.1f);
                    int maxLayerName = 0;
                    Glass needRoke = null;
                    //点击到玻璃才发射火箭
                    foreach (Collider2D obj in glass)
                    {
                        if (obj.GetComponent<Glass>() != null && obj.GetComponent<Glass>().IsExplosion == false)
                        {
                            Glass glass1 = obj.GetComponent<Glass>();
                            if (glass1.Layer >= maxLayerName)
                            {
                                maxLayerName = glass1.Layer;
                                needRoke = glass1;
                            }
                        }
                    }
                    if (needRoke != null)
                    {
                        AudioManager.Instance.PlaySFX("ScrewClick");
                        LevelManager.Instance.pro2UseCount++;
                        // Debug.LogError($"点击到的玻璃是{obj.name}");
                        rocketController.SpawnRocket(new Vector3(rayOrigin.x, rayOrigin.y, -20), needRoke);
                        isRocketClick = false;
                        UIManager.Instance.HideUI<RocketUI>();
                        MainSceneUI.Instance._GamePlayUI.UpdateRocketText();
                        return;
                    }

                }
                #endregion

                //点击放入小球
                //将射中的第一个小球放入槽中
                Collider2D[] hits = Physics2D.OverlapCircleAll(rayOrigin, 0.001f);
                foreach (var hit in hits)
                {
                    if (hit.GetComponent<Screw>() != null)
                    {
                        Screw curScrew = hit.GetComponent<Screw>();
                        if (curScrew.CanClick())
                        {
                            CurScrew = curScrew;

                            #region 从玻璃中移除螺丝
                            //从玻璃中移除螺丝
                            if (curScrew.CanRemoveFromGlass())
                            {
                                curScrew.RemoveFromGlass();

                                //if (curScrew.ConnectedScrew != null)
                                //    curScrew.ConnectedScrew.RemoveFromGlass();

                                #region 全局效果检查
                                //CheckIceBreak();//是否有冰可以进行破坏

                                //if (curScrew.HasDoor)//如果有门，需要将当前的门设置为关闭
                                //{
                                //    curScrew.ScrewDoor.CloseDoor();
                                //    curScrew.SetDoorFlase();
                                //}

                                //CheckDoor();//是否有门

                                //if (curScrew.HasBoom)//如果有炸弹需要将炸弹消除
                                //{
                                //    curScrew.DestoryBoom();
                                //}
                                //CheckBoom();//是否有炸弹

                                //if (curScrew.IsOtherChain) //是否有锁链
                                //{
                                //    curScrew.UnlockChain();
                                //}

                                //if (curScrew.HasKey)
                                //{
                                //    curScrew.KeyMoveToLcok();
                                //}
                                #endregion
                            }
                            else
                                Debug.Log(curScrew.ScrewColor + "无法从玻璃中移除！");
                            #endregion

                            AudioManager.Instance.PlaySFX("ScrewClick");
                            SetScrew(curScrew);//设置球到相应的槽中
                            return;
                        }
                        else
                        {
                            //Debug.Log("小球无法点击 : " + curScrew.ScrewColor);
                        }
                    }
                }

            }
        }

        private void GameText()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GamePlayUI gamePlayUI = MainSceneUI.Instance._GamePlayUI;
                NewPlayerTutor newPlayerTutor = UIManager.Instance.GetUI<NewPlayerTutor>();
                GuiteItem guiteItem = new GuiteItem()
                {
                    dexText = "这里可以升级士兵战力，解锁士兵种类！",
                    DesImageX = 0,
                    DesImageY = 894,
                    circleX = 279,
                    circleY = 3000f,
                    handleX = 360,
                    handleY = 3000,
                    isNeedShowButton = true,
                    maskType = 1,
                    x1 = 241.1f,
                    y1 = 1216.1f,
                    x2 = 396.7f,
                    y2 = 1312.2f,
                    isNeedShowClikTip = true,
                    TdIndex = 7
                };
                newPlayerTutor.SetMaskRect(guiteItem);
            }
        }
        #region 螺丝
        /// <summary>
        /// 检查球与当前槽颜色是相等
        /// </summary>
        private bool CheckBoxCrewColor(Box checkBox, ScrewColor _color)
        {
            return checkBox.BoxColor == _color;
        }
        /// <summary>
        /// 设置球到当前槽的相应位置
        /// </summary>
        public void SetScrew(Screw screw)
        {
            for (int i = 0; i < nowCanSetScrewBoxList.Count; i++)
            {
                Box nowBox = nowCanSetScrewBoxList[i];

                if (CheckBoxCrewColor(nowBox, screw.ScrewColor) && nowBox.CanSetThisScrew(screw)) //颜色一样，并且可以放进去
                {
                    //与这个箱子的颜色相同
                    SetboxWihtScrew(nowBox, screw);
                    return;
                }
            }
            emptyHoleManager.AddToEmptyHole(screw);
        }
        private void SetboxWihtScrew(Box box, Screw screw)
        {
            //检查颜色是否一致
            if (CheckBoxCrewColor(box, screw.ScrewColor) && box.IsMoving == false)
            {
                //是星星螺丝
                if (screw.IsStar())
                {
                    SetStarScrew(box, screw);
                }
                else
                {
                    SetNormalScrew(box, screw);
                }
            }
            else
            {
                //颜色不一致
                emptyHoleManager.AddToEmptyHole(screw);
            }

        }
        /// <summary>
        /// 安置普通螺丝
        /// </summary>
        private void SetNormalScrew(Box box, Screw screw)
        {
            //普通螺丝
            //普通洞口已满
            if (box.ISNormalFull())
            {
                emptyHoleManager.AddToEmptyHole(screw);
            }
            else
            {
                box.SetHoleList(screw);
            }
        }
        /// <summary>
        /// 安置星星螺丝
        /// </summary>
        private void SetStarScrew(Box box, Screw screw)
        {
            //箱子星星洞口是否已满
            if (box.ISStarFull())
            {
                emptyHoleManager.AddToEmptyHole(screw);
            }
            else
            {
                box.SetHoleList(screw);
            }
        }
        #endregion

        #region 箱子
        /// <summary>
        /// 更换当前的槽
        /// </summary>
        public void ChangeBox()
        {
            ////展示玩家提现成功弹窗
            //if (GameDataManager.CurrentGameData.lastShowTipCount == 0)
            //{
            //    GameDataManager.CurrentGameData.lastShowTipCount = GameDataManager.CurrentGameData.completeBoxNum;
            //    GameDataManager.CurrentGameData.NextShowTipCount = UnityEngine.Random.Range(3, 5);
            //}
            //else
            //{
            //    Debug.LogError("检测是否应该播放弹窗");
            //    if (GameDataManager.CurrentGameData.completeBoxNum >= GameDataManager.CurrentGameData.lastShowTipCount + GameDataManager.CurrentGameData.NextShowTipCount)
            //    {
            //        GameDataManager.CurrentGameData.lastShowTipCount = GameDataManager.CurrentGameData.completeBoxNum;
            //        GameDataManager.CurrentGameData.NextShowTipCount = UnityEngine.Random.Range(3, 5);
            //        EventManager.Instance.TriggerEvent(GameEvent.ShowTip1);
            //    }
            //}
            //检查是否满足有结束的条件
            if (CheckGameCompleted() /*|| true*/)
            {
                if (GameDataManager.CurrentGameData.levelNum == 1 && !GameTool.isNeedCloseMoneyIcon)
                {
                    //第一关胜利显示特殊引导
                    LevelManager.Instance.CheckNewGuite();
                }
                else
                {
                    //游戏完成执行的逻辑
                    WinGame();
                }
                return;
            }
            //收集一定数量的箱子后，显示转盘
            if (GameDataManager.CurrentGameData.nextOpenLcukPlane == 0)
            {
                GameDataManager.CurrentGameData.nextOpenLcukPlane = GameDataManager.CurrentGameData.completeBoxNum + UnityEngine.Random.Range(20, 46);
                GameDataManager.Save();
            }

            if (GameDataManager.CurrentGameData.completeBoxNum >= GameDataManager.CurrentGameData.nextOpenLcukPlane)
            {
                if (!GameTool.isNeedCloseMoneyIcon)
                {
                    if (!isLose)
                    {
                        UIManager.Instance.ShowUI<LuckPlane>();
                    }
                    GameDataManager.CurrentGameData.nextOpenLcukPlane = GameDataManager.CurrentGameData.completeBoxNum + UnityEngine.Random.Range(20, 46);
                    GameDataManager.Save();
                }
            }

            ////收集一定数量的箱子后，显示飞行宝箱
            //if (GameDataManager.CurrentGameData.completeBoxNum % GameTool.collectHowManeyBoxShowBubble == 0 && GameDataManager.CurrentGameData.completeBoxNum != 0)
            //{
            //    EventManager.Instance.TriggerEvent(GameEvent.ShowGift);
            //}
            //收集一定数量的箱子后，显示奖励界面
            if (GameDataManager.CurrentGameData.nextOpenBoxNumRewardPlane == 0)
            {
                GameDataManager.CurrentGameData.nextOpenBoxNumRewardPlane = GameDataManager.CurrentGameData.completeBoxNum + UnityEngine.Random.Range(5, GameTool.haoManyBoxGetDollar);
                GameDataManager.Save();
            }

            if (GameDataManager.CurrentGameData.completeBoxNum >= GameDataManager.CurrentGameData.nextOpenBoxNumRewardPlane)
            {
                GameDataManager.CurrentGameData.nextOpenBoxNumRewardPlane =
                    GameDataManager.CurrentGameData.completeBoxNum + UnityEngine.Random.Range(5, GameTool.haoManyBoxGetDollar);
                if (!isLose)
                {
                    EventManager.Instance.TriggerEvent(GameEvent.ShowBoxNumReward);
                }
                GameDataManager.Save();
            }
            //没有额外的箱子
           StartCoroutine( ChangeSingleBox());
        }

        private void ShowNewPlayGuite()
        {
            GuiteItem guiteItem = new GuiteItem()
            {
                dexText = "每次获得的现金存放到此处，通过关卡后全部提取",
                DesImageX = 0,
                DesImageY = 1074,
                circleX = 279,
                circleY = 3000f,
                handleX = 360,
                handleY = 3000,
                isNeedShowButton = true,
                isNoNeedRationJuXing = true,
                maskType = 1,
                x1 = -260.4f,
                y1 = 1219.7f,
                x2 = 258f,
                y2 = 1382.4f,
                isNeedShowClikTip = true,
                TdIndex = 7
            };
            EventManager.Instance.TriggerEvent<GuiteItem>(GameEvent.SetMaskRect, guiteItem);
        }

        IEnumerator ChangeSingleBox()
        {
            // Debug.LogError("收集了一个箱子");
            completeBox++;
            //获取现在可以上场的箱子
            yield return new WaitForEndOfFrame();
            int nowBox = GetNowCanGetScrewBox();
            int canUpBoxNum = nowLockBoxPosNum - nowBox;
            for (int i = 0; i < canUpBoxNum; i++)
            {
                SetTheFirstToEmptyPos();
            }
            //SetTheFirstToEmptyPos();
        }
        /// <summary>
        /// 根据屏幕比例计算点位
        /// </summary>
        private void CalculatePositions()
        {
            // 获取主摄像机
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main Camera not found!");
                return;
            }

            // 获取屏幕宽高
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 根据屏幕比例计算纵向的 Y 坐标
            float yPos = screenHeight * (1 - screenFraction);

            // 屏幕空间点位
            Vector3 screenPosLeft = new Vector3(0, yPos, 0);                     // 最左侧点
            Vector3 screenPosCenter = new Vector3(screenWidth / 2, yPos, 0);    // 中间点
            Vector3 screenPosRight = new Vector3(screenWidth, yPos, 0);         // 最右侧点
            Vector3 screenPosTop = new Vector3(0, screenHeight, 0);         // 最高点
            Vector3 screenPosCenterLeft = new Vector3(screenWidth * 5 / 16, yPos, 0); // 左侧 1/4
            Vector3 screenPosCenterRight = new Vector3(screenWidth * 11 / 16, yPos, 0); // 右侧 3/4

            // 转换到世界空间
            boxMoveLeftPos = mainCamera.ScreenToWorldPoint(screenPosLeft);
            boxMoveCenterPos = mainCamera.ScreenToWorldPoint(screenPosCenter);
            boxMoveRightPos = mainCamera.ScreenToWorldPoint(screenPosRight) + Vector3.right * 6;
            boxCenterTopPos = mainCamera.ScreenToWorldPoint(screenPosTop);
            boxCenterLeftPos = mainCamera.ScreenToWorldPoint(screenPosCenterLeft);
            boxCenterRightPos = mainCamera.ScreenToWorldPoint(screenPosCenterRight);

            // 确保 Z 值为 0 (适配 2D 平面)
            boxMoveLeftPos.z = 0;
            boxMoveCenterPos.z = 0;
            boxMoveRightPos.z = 0;
            boxCenterLeftPos.z = 0;
            boxCenterRightPos.z = 0;

        }

        /// <summary>
        /// 在场景视图中绘制点用于调试
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(boxMoveLeftPos, 0.3f); // 最左侧点
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(boxMoveCenterPos, 0.3f); // 中间点
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(boxMoveRightPos, 0.3f); // 最右侧点
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(boxCenterLeftPos, 0.3f); // 左侧 1/4 点
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(boxCenterRightPos, 0.3f); // 右侧 1/4 点
        }
        #endregion

        #region 游戏相关
        public void AddSpaceToReviewGame()
        {

            if (!isLockFour)
            {
                GameManager.Instance.ToLockNewPositon(4);
            }
            else if (!isLockThree)
            {
                GameManager.Instance.ToLockNewPositon(3);
            }

            gameState = GameState.Start;

        }
        public void EnterHomeScene()
        {
            LevelManager.Instance.ClearLevel();
            MainSceneUI.Instance.SetMainScene(false);
            HomeSceneUI.Instance.SetHomeScene(true);
            emptyHoleManager.gameObject.SetActive(false);

            //生命值不足，显示生命值不足的UI
            if (GameDataManager.CurrentGameData.heartCount == 0)
            {

            }

            int levelNum = LevelManager.Instance.GetLevleNum();
            switch (levelNum)
            {
                case 6:
                    if (GameDataManager.CurrentGameData.isDailyRewardLocked == false)
                        break;

                    //解锁每日
                    GameDataManager.UnlockHomeButton("daily");

                    TimeManager.Instance.StartDailyTime();//开始每日计时

                    break;

                case 10:
                    //第10关解锁Streak
                    if (GameDataManager.CurrentGameData.isStreaklocked == false)
                        break;


                    GameDataManager.UnlockHomeButton("streak");

                    HomeSceneUI.Instance.homeUI.FreshHomeButton();
                    TimeManager.Instance.StartStreakTime();//开始条纹计时
                    break;

                case 20:
                    //解锁抽奖
                    if (GameDataManager.CurrentGameData.isLuckySpinlocked == false)
                        break;

                    GameDataManager.UnlockHomeButton("luckyspin");
                    HomeSceneUI.Instance.homeUI.FreshHomeButton();
                    TimeManager.Instance.StartLuckySpinTime();
                    break;
                case 30:
                    //解锁飞机
                    break;

                    //if (GameDataManager.CurrentGameData.isSkyRacelocked == false)
                    //    break;

                    //GameDataManager.UnlockHomeButton("sky");
                    //HomeSceneUI.Instance.homeUI.FreshHomeButton();
                    //break;
            }

            //更新homeUI
            //HomeSceneUI.Instance.homeUI.UpdateHomeUI();

            gameState = GameState.Stop;
        }
        public void EnterMainScene()
        {
            HomeSceneUI.Instance.SetHomeScene(false);
            MainSceneUI.Instance.SetMainScene(true);
            emptyHoleManager.gameObject.SetActive(true);
            emptyHoleManager.ClearEmptyHole();
            gameState = GameState.Start;
            SetNowScrewColorSort();
            //HomeSceneUI.Instance.homeUI.winStreakUI.InitStreakPos();
            //LevelManager.Instance.InitLevel();
        }

        private void SetNowScrewColorSort()
        {
            if (LevelManager.Instance.levelNum == -1)
            {
                LevelManager.Instance.levelNum = GameDataManager.CurrentGameData.levelNum;
            }
            if (nowLevelScrewColorData == LevelManager.Instance.levelNum) return;
            nowLevelScrewColorData = LevelManager.Instance.levelNum;
            nowLevelScrewSort = new List<ScrewColor>();
            int needCreatNextLevel = LoadableLevels.GetActualLevelNumber(LevelManager.Instance.levelNum);

            if (LevelManager.Instance.levelNum < GameTool.maxLevelNum)
            {
                // 小于4关时，获取当前关卡的颜色
                nowLevelScrewSort = ReadOnlyBoxColorData.GetBoxColors($"BoxLevel_{needCreatNextLevel}");
            }
            else
            {
                // 循环关卡：改为构建关卡队列与颜色缓冲
                InitializeLoopingLevelPipeline();
            }
        }        // 初始化循环关卡的队列与缓冲
        private void InitializeLoopingLevelPipeline()
        {
            if (levelQueue == null) levelQueue = new Queue<int>();
            levelQueue.Clear();
            // 第一轮从“当前实际关卡”开始，按 AllLevels 顺序到结尾，再回到开头
            int startActualLevel = LoadableLevels.GetActualLevelNumber(LevelManager.Instance.levelNum);
            int[] allLevels = LoadableLevels.AllLevels;
            if (GameTool.isNeedCloseMoneyIcon)
            {
                allLevels = LoadableLevels.AAllLevels;
            }
            int startIndexInAll = System.Array.IndexOf(allLevels, startActualLevel);
            if (startIndexInAll < 0) startIndexInAll = 0;
            for (int i = startIndexInAll; i < allLevels.Length; i++)
            {
                levelQueue.Enqueue(allLevels[i]);
            }
            // for (int i = 0; i < startIndexInAll; i++)
            // {
            //     levelQueue.Enqueue(LoadableLevels.AllLevels[i]);
            // }
            if (colorBuffer == null) colorBuffer = new List<ScrewColor>();
            colorBuffer.Clear();
            EnsureColorBuffer();
            // 首次获取窗口时打印
            int windowSize = Math.Min(windowCap, colorBuffer.Count);
            if (windowSize > 0)
            {
                List<ScrewColor> windowList = new List<ScrewColor>(windowSize);
                for (int i = 0; i < windowSize; i++) windowList.Add(colorBuffer[i]);
                DebugPrintColorWindow("InitialWindow", windowList);
            }
        }
        // 调试：打印窗口内颜色顺序
        private void DebugPrintColorWindow(string tag, List<ScrewColor> windowList)
        {
            int count = windowList.Count;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append($"[ColorWindow-{tag}] size={count} -> ");
            int maxPrint = Math.Min(count, windowCap);
            for (int i = 0; i < maxPrint; i++)
            {
                sb.Append(windowList[i].ToString());
                if (i < maxPrint - 1) sb.Append(", ");
            }
            Debug.Log(sb.ToString());
        }
        // 确保颜色缓冲不低于最小值
        private void EnsureColorBuffer()
        {
            int guard = 0;
            while (colorBuffer.Count < minBufferSize && guard < LoadableLevels.TotalLevels * 4)
            {
                if (levelQueue.Count == 0)
                {
                    // 生成下一轮洗牌顺序
                    foreach (var lv in LoadableLevels.GetShuffledOrder()) levelQueue.Enqueue(lv);
                }
                if (levelQueue.Count == 0) break;
                int levelId = levelQueue.Dequeue();
#if UNITY_EDITOR
                Debug.Log($"加载{levelId}关卡的箱子");
#endif
                var colors = ReadOnlyBoxColorData.GetBoxColors($"BoxLevel_{levelId}");
                if (colors != null && colors.Count > 0)
                {
                    colorBuffer.AddRange(colors);
                }
                guard++;
            }
        }
        /// <summary>
        /// 检查游戏是否结束的函数
        /// </summary>
        private bool CheckGameCompleted()
        {
            // Debug.LogError($"一共收集了{completeBox}个箱子，一共有{boxList.Count}个箱子");
            if (LevelManager.Instance.levelNum <= 3)
            {
                EventManager.Instance.TriggerEvent<int, int>(GameEvent.BocComChanceSliderValue, boxList.Count, completeBox + 1);
                return completeBox == boxList.Count - 1;
            }
            else
            {
                EventManager.Instance.TriggerEvent<int, int>(GameEvent.BocComChanceSliderValue, 0, completeBox + 1);
                return false;
            }
        }

        /// <summary>
        /// 初始化关卡(记录关卡数据)
        /// </summary>
        public void InitBoxLevel()
        {
            //清空操作
            EnterMainScene();
            boxList.Clear();
            boxLevel = null;
            nowCanSetScrewBoxList.Clear();//现在可以承载螺丝的箱子
            completeBox = 0;
            nowNeedOnStageBoxIndex = 0;
            nowLockBoxPosNum = 2;
            isLockFour = false;
            isLockThree = false;
            EventManager.Instance.TriggerEvent(GameEvent.ShowBoxPos);
            NeedLockBoxPos1.gameObject.SetActive(true);
            NeedLockBoxPos2.gameObject.SetActive(true);
            foreach (var key in boxPosPosition.Keys.ToList())
            {
                boxPosPosition[key] = false;
            }
            //extraBox = null;
            //isDoubleBox = false;
            //curBoxIndex = 0;

            //初始化
            boxLevel = transform.Find("BoxLevel").transform;
            boxLevel.transform.localPosition = new Vector3(0, 0, 0);
            for (int i = 0; i < boxLevel.childCount; i++)
            {
                if (boxLevel.GetChild(i).GetComponent<Box>() != null)
                    boxList.Add(boxLevel.GetChild(i).GetComponent<Box>());
            }
            //curBox = boxList[0];

            //设置box到相应的位置
            for (int i = 0; i < boxLevel.childCount; i++)
            {
                if (i < nowLockBoxPosNum)
                {
                    //把最前面的箱子放到一个空位上
                    SetTheFirstToEmptyPos();
                }
                else
                {
                    boxList[i].transform.position = new Vector3(-16, 11.5f, 0);
                }
            }

            gameState = GameState.Start;


        }

        public void SetBoxGuite()
        {
            if (LevelManager.Instance.levelNum == 1)
            {
                NeedLockBoxPos1.gameObject.SetActive(false);
                NeedLockBoxPos2.gameObject.SetActive(false);
                nowCanSetScrewBoxList[0].transform.DOKill();
                nowCanSetScrewBoxList[0].MoveToLocalTarget(new Vector2(0, 10.65f), delegate
                {
                    emptyHoleManager.AddToBox(nowCanSetScrewBoxList[0]);
                });
                SpriteRenderer spriteRenderer = nowCanSetScrewBoxList[0].GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "End";
                spriteRenderer.sortingOrder = 20;
                return;
            }
        }
        /// <summary>
        /// 把最前面的箱子放到一个空位上
        /// </summary>
        public void SetTheFirstToEmptyPos(bool isNeedChanceColor = false)
        {
            if (nowLockBoxPosNum <= GetNowCanGetScrewBox())
            {
                return;
            }
            Vector2 pos = GetTheRightPos();
            if (pos == Vector2.zero) return;
            SortboxList();//难度改变，改变箱子出现的顺序
            CheckBoxNum();//检查箱子的数量，如果没有了，复制一个出来
            if (nowNeedOnStageBoxIndex >= boxList.Count) return;
            Box needOnStaga = boxList[nowNeedOnStageBoxIndex];
            ScrewColor screwColor = GetNowColor(LevelManager.Instance.levelNum, nowNeedOnStageBoxIndex);
            needOnStaga.SetBoxColor(GameTool.GetNewColor(screwColor));
            nowNeedOnStageBoxIndex++;
            if (isNeedChanceColor)
            {
                //int nowGamePro = UIManager.Instance.GetUI<PopGameSlidePlane>().AwalCanGetValue();
                //if (nowGamePro >= 97)
                //{
                //    ScrewColor color = LevelManager.Instance.GetNowLevelScrewGame();
                //    needOnStaga.SetBoxColor(color);
                //}
                //else
                //{
                if (nowCanSetScrewBoxList.Count > 0)
                {
                    List<ScrewColor> screwColors = GetNowUpBoxColor();
                    int n = UnityEngine.Random.Range(0, screwColors.Count);
                    ScrewColor color = screwColors[n];
                    needOnStaga.SetBoxColor(color);
                    Debug.LogError("卡点改变箱子颜色+改变的颜色是:" + color.ToString());
                }
                // }
            }
            boxPosPosition[pos] = true;
            needOnStaga.MoveToLocalTarget(pos, delegate
            {
                emptyHoleManager.AddToBox(needOnStaga);
            });
            needOnStaga.SetPos(pos);
            nowCanSetScrewBoxList.Add(needOnStaga);
        }
        /// <summary>
        /// 难度改变，改变箱子出现的顺序
        /// </summary>
        private void CheckBoxNum()
        {
            if (LevelManager.Instance.levelNum <= 3) return;
            if (boxList.Count == nowNeedOnStageBoxIndex)
            {
                GameObject game = Instantiate(ResourceLoader.Instance.GetResWithPath<GameObject>("Prefab/greenbox_3.prefab"),
                    boxList[nowNeedOnStageBoxIndex - 1].gameObject.transform.parent);
                game.transform.position = new Vector3(-16, 11.5f, 0);
                Box box = game.GetComponent<Box>();
                box.moveDuration = 0.3f;
                boxList.Add(box);
            }
        }

        private List<ScrewColor> GetNowUpBoxColor()
        {
            List<ScrewColor> screwColors = new List<ScrewColor>();
            for (int i = 0; i < nowCanSetScrewBoxList.Count; i++)
            {
                if (nowCanSetScrewBoxList[i] != null)
                {
                    screwColors.Add(nowCanSetScrewBoxList[i].BoxColor);
                }
            }
            return screwColors;
        }

        private ScrewColor GetNowColor(int levelNum, int nowNeedOnStageBoxIndex)
        {
            if (LevelManager.Instance.levelNum < GameTool.maxLevelNum)
            {
                if (nowLevelScrewSort.Count == 0)
                {
                    SetNowScrewColorSort();
                }
                if (nowLevelScrewSort != null && nowLevelScrewSort.Count > 0)
                {
                    int actualIndex = nowNeedOnStageBoxIndex % nowLevelScrewSort.Count;
                    return nowLevelScrewSort[actualIndex];
                }
                Debug.LogWarning($"GetNowColor: 没有可用的颜色数据，返回默认颜色");
                return ScrewColor.Red;
            }

            // 循环关卡：从颜色缓冲消费
            if (colorBuffer == null || levelQueue == null)
            {
                InitializeLoopingLevelPipeline();
            }
            EnsureColorBuffer();
            if (colorBuffer.Count == 0)
            {
                Debug.LogWarning("colorBuffer为空，返回默认颜色");
                return ScrewColor.Red;
            }
            ScrewColor color = colorBuffer[0];
            colorBuffer.RemoveAt(0);
            if (colorBuffer.Count < refillThreshold)
            {
                EnsureColorBuffer();
            }
            return color;
        }

        /// <summary>
        /// 难度改变，改变箱子出现的顺序
        /// </summary>
        private void SortboxList()
        {
            if (LevelManager.Instance.levelNum <= GameTool.maxLevelNum - 1) return;
            BosChanceData bosChanceData = GameTool.GetNowBoxChanceData();
            if (bosChanceData != null && bosChanceData.isNeedChance)
            {
                //卡点设置
                HardStart = UIManager.Instance.GetUI<PopGameSlidePlane>().AwalCanGetValue();
                hardOver = bosChanceData.moveHowDic + HardStart;
                int startIndex = 0; // 在即将出现的缓冲窗口内起点
                int moveCount = bosChanceData.moveHowNumBox;
                int moveDistance = bosChanceData.moveHowDic;

                if (colorBuffer == null || levelQueue == null)
                {
                    InitializeLoopingLevelPipeline();
                }
                // 确保窗口可用
                EnsureColorBuffer();
                int windowSize = Math.Min(windowCap, colorBuffer.Count);
                if (windowSize <= 0 || moveCount <= 0 || moveDistance <= 0)
                {
                    Debug.Log($"颜色顺序调整参数无效或窗口为空：moveCount={moveCount}, moveDistance={moveDistance}, windowSize={windowSize}");
                    return;
                }
                // Clamp 移动数量
                moveCount = Math.Min(moveCount, windowSize);

                // 窗口内回绕目标索引
                int targetStartIndex = (startIndex + moveDistance) % windowSize;

                // 保存窗口段
                List<ScrewColor> windowList = new List<ScrewColor>(windowSize);
                for (int i = 0; i < windowSize; i++) windowList.Add(colorBuffer[i]);

                // 打印改变前窗口
                DebugPrintColorWindow("BeforeReorder", windowList);

                // 待移动片段
                List<ScrewColor> toMove = new List<ScrewColor>();
                for (int i = 0; i < moveCount; i++) toMove.Add(windowList[(startIndex + i) % windowSize]);

                // 从后往前移除原片段
                for (int i = moveCount - 1; i >= 0; i--)
                {
                    int removeIndex = (startIndex + i) % windowList.Count;
                    windowList.RemoveAt(removeIndex);
                }
                // 在目标位置插入
                for (int i = 0; i < toMove.Count; i++)
                {
                    int insertIndex = (targetStartIndex + i) % (windowList.Count + 1);
                    windowList.Insert(insertIndex, toMove[i]);
                }

                // 写回窗口到 colorBuffer 前段
                for (int i = 0; i < windowSize; i++) colorBuffer[i] = windowList[i];

                // 打印改变后窗口
                DebugPrintColorWindow("AfterReorder", windowList);
            }
        }
        private Vector2 GetTheRightPos()
        {
            int count = 1;
            foreach (var item in boxPosPosition)
            {
                bool isUnlocked = false;

                // 检查是否是默认解锁的后两个位置（下标2,3）
                if (count == 3 || count == 4)
                {
                    isUnlocked = true;
                }
                // 检查额外解锁的位置（第一个或第二个位置）
                else if (nowLockBoxPosNum > 2)
                {
                    if (count == 1 && isLockFour)
                    {
                        isUnlocked = true;
                    }
                    else if (count == 2 && isLockThree)
                    {
                        isUnlocked = true;
                    }
                }

                // 如果位置未被占用且已解锁，返回这个位置
                if (!item.Value && isUnlocked)
                {
                    return item.Key;
                }
                count++;
            }
            return Vector2.zero;
        }


        /// <summary>
        /// 退出游戏场景
        /// </summary>
        public void ExitGame()
        {
            gameState = GameState.Stop;
            LevelManager.Instance.ClearLevel();


        }
        /// <summary>
        /// 游戏胜利的逻辑
        /// </summary>
        public void WinGame()
        {
            //Debug.Log("游戏胜利");
            gameState = GameState.Stop;
            UIManager.Instance.ShowUI<WinUI>();
        }
        /// <summary>
        /// 游戏失败(空槽满了)
        /// </summary>
        public void loseGame()
        {
            if (emptyHoleManager.CheckGameFailed() && gameState == GameState.Start)
            {
                gameState = GameState.Stop;

                //检查是否还有可以解锁的box
                if (false && nowLockBoxPosNum < 4)
                {

                }
                else
                {
                    UIManager.Instance.GetUI<LoseUI>().SetBackObj(false);
                    UIManager.Instance.ShowUI<LoseUI>();
                    AudioManager.Instance.PlaySFX("Lose");
                }
            }

        }
        #endregion

        #region 道具

        /// <summary>
        /// 道具一的调用函数（启用额外的洞口）
        /// </summary>
        public void AddExtraHole()
        {
            emptyHoleManager.ActivateExtraHole();
        }

        /// <summary>
        /// 道具三的移动函数
        /// </summary>
        public void MoveObjectsToPositions(GameObject moveLeftObj, GameObject moveRightObj, Vector3 leftPos, Vector3 rightPos, System.Action callBack = null, float time1 = 0.3f, float time2 = 0.2f)
        {
            // 同时启动两个 DOTween 动画
            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(moveLeftObj.transform.DOMove(leftPos, time1).SetEase(Ease.InOutSine));
            moveSequence.Join(moveRightObj.transform.DOMove(rightPos, time2).SetEase(Ease.InOutSine));
            //动画完成后的回调
            moveSequence.OnComplete(() =>
            {
                callBack?.Invoke();
            });
        }
        /// <summary>
        /// 启用额外的箱子
        /// </summary>
        public void ActiveExtraBox()
        {
            //if (curBoxIndex + 1 < boxList.Count)
            //{
            //    //将之前的箱子设置为额外的盒子
            //    extraBox = curBox;
            //    //将下一个盒子作为当前盒子
            //    curBox = boxList[++curBoxIndex];

            //    isDoubleBox = true;
            //    MoveObjectsToPositions(curBox.gameObject, extraBox.gameObject, boxCenterLeftPos, boxCenterRightPos, () =>
            //    {
            //        //尝试将螺丝加入到下一个箱子中
            //        emptyHoleManager.AddToBox(curBox);
            //    });

            //}
            //else
            //{
            //    Debug.Log("以及没有额外的箱子了");
            //}
        }

        public bool CanAddExtraBox()
        {
            //if (isDoubleBox)
            //{
            //    Debug.Log("已经有额外的箱子了");
            //    return false;
            //}
            //else if (curBox.IsMoving)
            //{
            //    Debug.Log("当前箱子正在移动");
            //    return false;
            //}

            return true;
        }
        #endregion

        #region 全局效果
        private void CheckIceBreak()
        {
            Level curLevel = LevelManager.Instance.CurLevel;
            if (curLevel != null && curLevel.HasIceCovered)
            {
                //遍历关卡层级
                List<Layer> layerList = curLevel.LayerList;
                foreach (Layer curLayer in layerList)
                {
                    if (curLayer.HasIceCoverd)
                    {
                        //遍历玻璃
                        List<Glass> glassList = curLayer.GlassList;
                        foreach (Glass curGlass in glassList)
                        {
                            if (curGlass.HasIceCovered)
                            {
                                //遍历螺丝
                                List<Screw> screwList = curGlass.ScrewList;
                                foreach (Screw curScrew in screwList)
                                {
                                    //查看螺丝是否有冰，并且可以进行点击
                                    if (curScrew.IsIceCovered && curScrew.HasGlassCovered() == false)
                                    {
                                        //检查冰是否已经破坏完全
                                        if (curScrew.ScrewIce.IceBreak())
                                        {
                                            curScrew.SetIceCoveredFalse();
                                        }
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }

        private void CheckDoor()
        {
            Level curLevel = LevelManager.Instance.CurLevel;
            if (curLevel != null && curLevel.HasDoor)
            {
                //遍历关卡层级
                List<Layer> layerList = curLevel.LayerList;
                foreach (Layer curLayer in layerList)
                {
                    if (curLayer.HasDoor)
                    {
                        //遍历玻璃
                        List<Glass> glassList = curLayer.GlassList;
                        foreach (Glass curGlass in glassList)
                        {
                            if (curGlass.HasDoor)
                            {
                                //遍历小球
                                List<Screw> screwList = curGlass.ScrewList;
                                foreach (Screw curScrew in screwList)
                                {
                                    //查看是否有门 && 并且没有被玻璃覆盖
                                    if (curScrew.HasDoor && curScrew.HasGlassCovered() == false)
                                    {

                                        curScrew.ScrewDoor.OperateDoor();
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }

        private void CheckBoom()
        {
            Level curLevel = LevelManager.Instance.CurLevel;
            if (curLevel != null && curLevel.HasBoom)
            {
                //遍历关卡层级
                List<Layer> layerList = curLevel.LayerList;
                foreach (Layer curLayer in layerList)
                {
                    if (curLayer.HasBoom)
                    {
                        //遍历玻璃
                        List<Glass> glassList = curLayer.GlassList;
                        foreach (Glass curGlass in glassList)
                        {
                            if (curGlass.HasBoom)
                            {
                                //遍历小球
                                List<Screw> screwList = curGlass.ScrewList;
                                foreach (Screw curScrew in screwList)
                                {
                                    //有炸弹，并且没有被覆盖
                                    if (curScrew.HasBoom && curScrew.HasGlassCovered() == false)
                                    {
                                        curScrew.DoBoomAnim();
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }
        /// <summary>
        /// 解锁一个新的箱子位置
        /// </summary>
        internal void ToLockNewPositon(int value)
        {
            if (nowLockBoxPosNum < 4)
            {
                nowLockBoxPosNum++;
                if (value == 4)
                {
                    isLockFour = true;
                    EventManager.Instance.TriggerEvent<int>(GameEvent.HideAddBoxBut, 4);
                }
                else if (value == 3)
                {
                    isLockThree = true;
                    EventManager.Instance.TriggerEvent<int>(GameEvent.HideAddBoxBut, 3);
                }

                int nowGamePro = UIManager.Instance.GetUI<PopGameSlidePlane>().AwalCanGetValue();
                if (nowGamePro >= HardStart && nowGamePro <= hardOver && nowGamePro != 0)
                {
                    if (nowGamePro < 97)
                    {
                        int n = UnityEngine.Random.Range(0, 100);
                        if (n < 50)
                        {
                            SetTheFirstToEmptyPos(true);
                        }
                        else
                        {
                            SetTheFirstToEmptyPos(false);
                        }
                    }
                    else
                    {
                        SetTheFirstToEmptyPos(true);
                    }
                }
                else
                {
                    SetTheFirstToEmptyPos();
                }
            }
        }
        /// <summary>
        /// 把螺丝放入道具盒
        /// </summary>
        internal void SetEmptyHoleToPropEmpty()
        {
            emptyHoleManager.SetEmptyHoleToPropEmpty();
        }

        internal void AddCompleteOneBox()
        {
            GameDataManager.AddCompleteBoxCount();
        }

        internal void SetPos(RectTransform uiRectTransform)
        {
            // 获取UI的位置（以屏幕坐标为单位）
            Vector3 uiScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, uiRectTransform.position);
            // 将屏幕坐标转换为世界坐标
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(uiScreenPosition);
            // 将Z轴设置为物体的当前Z值，保持在同一平面上
            worldPosition.z = transform.position.z; // 假设2D物体在同一Z轴平面上
                                                    // 设置物体的位置
                                                    //transform.position = worldPosition;
            StartCoroutine(SetPosUIWithWord(worldPosition));
        }
        IEnumerator SetPosUIWithWord(Vector3 worldPosition)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            float x = 1290f / Screen.width;
            float y = 2796f / Screen.height;
            if (x > y)
            {
                transform.position = worldPosition;
            }
            //  this.transform.GetComponent<CanvasScaler>().matchWidthOrHeight = x > y ? 0 : 1;
        }

        internal int GetNowCanGetScrewBox()
        {
            int count = 0;
            for (int i = 0; i < nowCanSetScrewBoxList.Count; i++)
            {
                Box nowBox = nowCanSetScrewBoxList[i];

                if (nowBox != null)
                {
                    if (!nowBox.IsMoving)
                    {
                        count++;
                    }
                }
            }
            return count;
        }


        #endregion
    }
}
