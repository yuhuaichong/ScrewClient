using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
namespace DafultScript
{
    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public class SavedLevelData
    {
        public string levelName;
        public List<SavedLayerData> layers = new List<SavedLayerData>();
        public Dictionary<string, int> holeColorCounts = new Dictionary<string, int>();
        public List<BoxData> boxSequence = new List<BoxData>();
    }

    [System.Serializable]
    public class SavedLayerData
    {
        public int layerIndex;
        public int unityLayer;
        public List<SavedGlassData> glasses = new List<SavedGlassData>();
    }

    [System.Serializable]
    public class SavedGlassData
    {
        public SerializableVector3 position;
        public SerializableVector3 rotation;
        public string spritePath;
        public List<SavedHoleData> holes = new List<SavedHoleData>();
    }

    [System.Serializable]
    public class SavedHoleData
    {
        public SerializableVector3 position;
        public string spritePath;
    }

    [System.Serializable]
    public class BoxData
    {
        public string color;
        public int sequenceIndex;
    }

    [System.Serializable]
    public class LevelConfigData
    {
        public Dictionary<string, SavedLevelData> levels = new Dictionary<string, SavedLevelData>();
    }

    public class LevelManager : MonoBehaviour
    {
        // [SerializeField] private Dictionary<string, GameObject> levels = new Dictionary<string, GameObject>();
        //  [SerializeField] private Dictionary<string, GameObject> boxs = new Dictionary<string, GameObject>();
        [SerializeField] public int levelNum;
        [Header("冰")]
        [SerializeField] private GameObject icePrefab;
        [Header("门")]
        [SerializeField] private GameObject doorPrefab;
        [Header("炸弹")]
        [SerializeField] private GameObject boomPrefab;
        [Header("锁链")]
        [SerializeField] private GameObject chainFxPrefab;
        [Header("门锁")]
        [SerializeField] private GameObject keyPrefab;
        [SerializeField] private GameObject lockPrefab;
        private static LevelManager _instance;
        private GameObject boxlevel;//记录上次的关卡，用于删除
        private GameObject level;//记录上次的关卡，用于删除
        private List<GameObject> keys = new List<GameObject>();
        private List<GameObject> locks = new List<GameObject>();
        public Level CurLevel
        { get; private set; }

        public int needCreatNextLevel;

        public int GetLevleNum()
        {
            return GameDataManager.CurrentGameData.levelNum;
        }
        public Sprite UnlockSprite { get; set; }
        public int allLayerCount;//当前所有关卡所有的层级
        public int loadNextLevel;//无限挑战的关卡
        public int pro2UseCount;//道具2使用次数
        public int pro3UseCount;//道具2使用次数
        public bool isStartMathFlyBoxTime;//开始计算飞行宝箱的时间
        public bool isStartMathTop1Time;//开始计算飞行宝箱的时间
        public float nowTop1Time;//现在顶部弹窗出现的时间
        public static LevelManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LevelManager>();
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(LevelManager).ToString());
                        _instance = singleton.AddComponent<LevelManager>();
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            //levels = ResourceLoader.Instance.Levels;
            //boxs = ResourceLoader.Instance.BoxLevels;
            EventManager.Instance.RegisterEvent<int>(GameEvent.ShowNewScrew, ShowNewScrew);
            EventManager.Instance.RegisterEvent(GameEvent.ScrewGuiteIsOver, ScrewGuiteIsOver);
            EventManager.Instance.RegisterEvent(GameEvent.OneLayerDes, OneLayerDes);
            EventManager.Instance.RegisterEvent(GameEvent.GetNewGuiteGiftt, GetNewGuiteGiftt);
            EventManager.Instance.RegisterEvent<Layer>(GameEvent.OneLayerNoGlass, OneLayerNoGlass);
                        EventManager.Instance.RegisterEvent(GameEvent.LoseLevel, LoseLevel);
        }
        private void OnDestroy()
        {
            EventManager.Instance.UnregisterEvent<int>(GameEvent.ShowNewScrew, ShowNewScrew);
            EventManager.Instance.UnregisterEvent(GameEvent.ScrewGuiteIsOver, ScrewGuiteIsOver);
            EventManager.Instance.UnregisterEvent(GameEvent.OneLayerDes, OneLayerDes);
            EventManager.Instance.UnregisterEvent(GameEvent.GetNewGuiteGiftt, GetNewGuiteGiftt);
            EventManager.Instance.UnregisterEvent<Layer>(GameEvent.OneLayerNoGlass, OneLayerNoGlass);
                        EventManager.Instance.UnregisterEvent(GameEvent.LoseLevel, LoseLevel);
        }
        private void LoseLevel()
        {
            TDAnalyticsManager.Instance.SendLoseLevel(GameTool.nowLevel, GameTool.nowProgress);
        }
        private void OneLayerNoGlass(Layer layer)
        {
            int noHaveCount = 0;
            //for (int i = 0; i < CurLevel.transform.childCount; i++)
            //{
            //    if (CurLevel.transform.GetChild(i).childCount != 0)
            //    {
            //        if (CurLevel.transform.GetChild(i).GetComponent<Layer>() != null)
            //        {
            //            if(CurLevel.transform.GetChild(i).GetComponent<Layer>()!= layer)
            //            {
            //                noHaveCount++;
            //            }
            //        }
            //    }
            //}
            for (int i = CurLevel.transform.childCount - 1; i > +0; i--)
            {
                if (CurLevel.transform.GetChild(i).childCount != 0)
                {
                    if (CurLevel.transform.GetChild(i).GetComponent<Layer>() != null)
                    {
                        if (CurLevel.transform.GetChild(i).GetComponent<Layer>() != layer)
                        {
                            noHaveCount++;
                        }
                    }
                }
                else
                {
                    Destroy(CurLevel.transform.GetChild(i).gameObject);
                }
            }
            //Debug.LogError($"现在还有{noHaveCount}层螺丝");
            if (noHaveCount <= 2)
            {
                Invoke(nameof(CreatNextLevel), 0.005f);//无限关卡，底下生成下一个关的预制体
            }
        }

        private void GetNewGuiteGiftt()
        {
            GameObject hand = Instantiate(ResourceLoader.Instance.GetRes<GameObject>("AssetBundleLocal/NoNeedLoadRes/Prefab/NewPlayerGuite/Hand.prefab"));
            hand.transform.SetParent(level.transform);
            CurLevel.ShowNewScrewGuite(1);//显示新手引导
            CurLevel.SetEndLayer();
            GameManager.Instance.SetBoxGuite();
            GameDataManager.CurrentGameData.isGetNewPlayerGuite = true;
            GameDataManager.Save();
        }

        private void OneLayerDes()
        {
            if (levelNum < 4) return;
            allLayerCount--;
            Debug.LogError($"还有{allLayerCount}个层级");
            if (allLayerCount <= 3)
            {
                CreatNextLevel();//无限关卡，底下生成下一个关的预制体
            }
        }

        private void ScrewGuiteIsOver()
        {
            CurLevel.LockAllScrew();
        }

        private void ShowNewScrew(int obj)
        {
            CurLevel.ShowNewScrewGuite(obj);//显示新手引导
        }

        private void Start()
        {
            // 输出当前关卡号和金币数
            levelNum = GameDataManager.CurrentGameData.levelNum;
        }
        // 提取文件名中的数字部分
        private int ExtractNumber(string name)
        {
            string number = new string(name.Where(char.IsDigit).ToArray());
            return string.IsNullOrEmpty(number) ? 0 : int.Parse(number);
        }

        /// <summary>
        /// 将关卡实例化出来
        /// </summary>
        public async void InitLevel()
        {
            EventManager.Instance.TriggerEvent(GameEvent.SliderValueResver);
            CreatMathCreatFlyBoxTime();//开始计时飞行宝箱
            CreatMathCreatTop1();//开始计时顶部弹窗出现时间
            CreatWithDrawTip();//提现提示
            GameTool.SetNewColor();
            GameManager.Instance.EnterMainScene();
            pro2UseCount = 0;
            pro3UseCount = 0;
            ClearLevel();
            //实例化关卡
            InitLevelGameObj();
            UIManager.Instance.GetUI<LoseUI>().RecovesCount();//重置复活次数，每关只有3次复活机会
            GlobalInit();

            //  UniqueInitObj();
            Invoke(nameof(DelayInitBoxLevel), 0.005f);

            //CheckNewGuite();//检查新手引导,第二关引导玩家点击提现按钮

            if (GameDataManager.CurrentGameData.levelNum == 4)
            {
                EventManager.Instance.TriggerEvent(GameEvent.ShowTip2);
            }

            Invoke(nameof(CreatNextLevel), 0.005f);//无限关卡，底下生成下一个关的预制体
                                                   //await LoadLevelAsync();
        }

        private void CreatWithDrawTip()
        {
            if (levelNum > 1)
            {
                EventManager.Instance.TriggerEvent(GameEvent.ShowWithDrawTip);
            }
        }

        private void CreatMathCreatTop1()
        {
            if (levelNum >= 2 && !isStartMathTop1Time)
            {
                isStartMathTop1Time = true;
                if (levelNum == 2 && !GameDataManager.CurrentGameData.IsShowTipOne)
                {
                    nowTop1Time = 210f;//先出现一次
                    GameDataManager.CurrentGameData.IsShowTipOne = true;
                    GameDataManager.Save();
                }
                StartCoroutine(WaitCreatTop1());
            }
            else
            {
                nowTop1Time = 0;
            }
        }
        IEnumerator WaitCreatTop1()
        {
            float needWaitTime = UnityEngine.Random.Range(180, 210);
           // float needWaitTime = UnityEngine.Random.Range(5, 10);
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
            //     Debug.LogError("开始计时顶部广播");
            while (true)
            {
                nowTop1Time += Time.deltaTime;
                yield return waitForEndOfFrame;
                if (nowTop1Time >= needWaitTime)
                {
                    needWaitTime = UnityEngine.Random.Range(180, 210);
                   //    needWaitTime = UnityEngine.Random.Range(5, 10);
                    nowTop1Time = 0;
                    EventManager.Instance.TriggerEvent(GameEvent.ShowTip1);
                }
            }
        }
        private void CreatMathCreatFlyBoxTime()
        {
            if (levelNum == 4 && !isStartMathFlyBoxTime)
            {
                isStartMathFlyBoxTime = true;
                StartCoroutine(WaitThreeTimeCreatFlyBox());
            }
        }
        IEnumerator WaitThreeTimeCreatFlyBox()
        {
            //Debug.LogError("开始计时飞行宝箱");
            while (true)
            {
                yield return new WaitForSeconds(180);
                //yield return new WaitForSeconds(5);
                EventManager.Instance.TriggerEvent(GameEvent.ShowGift);
            }
        }
        /// <summary>
        /// 检查新手引导,第二关引导玩家点击提现按钮
        /// </summary>
        public void CheckNewGuite()
        {
            //if (GameDataManager.CurrentGameData.levelNum == 1)
            //{
            //    if (!GameDataManager.CurrentGameData.isShowMoneyGuite)
            //    {
            //        GameDataManager.CurrentGameData.isShowMoneyGuite = true;
            if (!GameTool.isNeedCloseMoneyIcon)
            {
                GuiteItem guiteItem = new GuiteItem()
                {
                    dexText = "当前获得的现金，通过关卡后可全部提取",
                    DesImageX = 0,
                    DesImageY = 1074,
                    circleX = 279,
                    circleY = 3000f,
                    handleX = 360,
                    handleY = 3000,
                    isNeedShowButton = true,
                    isNoNeedRationJuXing = false,
                    maskType = 1,
                    x1 = -413.45f,
                    y1 = 1221.34f,
                    x2 = 100f,
                    y2 = 1374f,
                    isNeedShowClikTip = true,
                    TdIndex = 7
                };
                EventManager.Instance.TriggerEvent<GuiteItem>(GameEvent.SetMaskRect, guiteItem);
                //        }
                //    }
            }
        }

        /// <summary>
        /// 生成下一个关的预制体
        /// </summary>
        public void CreatNextLevel()
        {
            if (levelNum >= 4)
            {
                StartCoroutine(StarCreatNextLevel());
            }
        }
        IEnumerator StarCreatNextLevel()
        {
            yield return new WaitForSeconds(0.005f);
            Transform gameParent = GameManager.Instance.transform;
            int needCreatNextLevel = levelNum + (++loadNextLevel);
            //if (needCreatNextLevel ==6)
            //{
            //    needCreatNextLevel = levelNum + (++loadNextLevel);
            //}
            // GameObject levelT = Instantiate(levels["Level " + (levelNum + 1).ToString()]);
            GameObject levelT = Instantiate(ResourceLoader.Instance.GetLevelsOnlyWithNum(needCreatNextLevel));
            levelT.gameObject.SetActive(false);
            if (levelT != null)
            {
                levelT.transform.localPosition = new Vector3(0, -2, 0);
                levelT.transform.SetParent(gameParent);
            }
            int count = levelT.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                levelT.transform.GetChild(0).SetParent(CurLevel.transform);
            }

            // int count= CurLevel.transform.childCount;
            //for (int i= levelT.transform.childCount-1; i>=0; i--)
            //{
            //    levelT.transform.GetChild(i).SetParent(CurLevel.transform);
            //}
            // for(int i=0;i<)

            allLayerCount = CurLevel.InitLayerList();
            Destroy(levelT);
            //  GameObject boxlevelT = Instantiate(boxs["BoxLevel_" + (levelNum + 1).ToString()]);
            GameObject boxlevelT = Instantiate(ResourceLoader.Instance.GetLevelBoxOnlyWithNum(needCreatNextLevel));
            boxlevelT.transform.localPosition = new Vector3(0, 0, 0);

            float sliderValue = UIManager.Instance.GetUI<PopGameSlidePlane>().AwalCanGetValue();
            Debug.LogError($"现在的游戏进度是:" + sliderValue);
            if (sliderValue < GameTool.ChaneGameHard)//游戏进行到多少，改变游戏难度
            {
                // 正常难度，箱子按照正常出现（预制体摆放的顺序）
                int Boxcount = boxlevelT.transform.childCount;
                for (int i = 0; i < Boxcount; i++)
                {
                    if (boxlevelT.transform.GetChild(0).GetComponent<Box>() != null)
                    {
                        GameManager.Instance.boxList.Add(boxlevelT.transform.GetChild(0).GetComponent<Box>());
                        boxlevelT.transform.GetChild(0).localPosition = new Vector3(-16, 12.35f, 0);
                        boxlevelT.transform.GetChild(0).SetParent(GameManager.Instance.boxLevel);
                    }
                    else
                    {
                        Debug.LogError("箱子预制体错误");
                    }
                }
            }
            else
            {
                //难度增加，箱子反序出现
                for (int i = boxlevelT.transform.childCount - 1; i >= 0; i--)
                {
                    if (boxlevelT.transform.GetChild(i).GetComponent<Box>() != null)
                    {
                        GameManager.Instance.boxList.Add(boxlevelT.transform.GetChild(i).GetComponent<Box>());
                        boxlevelT.transform.GetChild(i).localPosition = new Vector3(-16, 12.35f, 0);
                        boxlevelT.transform.GetChild(i).SetParent(GameManager.Instance.boxLevel);

                    }
                }
            }


            Destroy(boxlevelT);
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < CurLevel.transform.childCount; i++)
            {
                CurLevel.transform.GetChild(i).gameObject.SetActive(true);
            }
            //获取现在可以上场的箱子
            int nowBox = GameManager.Instance.GetNowCanGetScrewBox();
            int canUpBoxNum = GameManager.Instance.nowLockBoxPosNum - nowBox;
            for (int i = 0; i < canUpBoxNum; i++)
            {
                GameManager.Instance.SetTheFirstToEmptyPos();
            }
        }
        /// <summary>
        /// 全局生成
        /// </summary>
        private void GlobalInit()
        {
            //  Invoke(nameof(CreateRope), 0.05f);//延迟生成绳子，先让关卡初始化完成
            //Invoke(nameof(CreateIce), 0.05f);//延迟生成冰
            //Invoke(nameof(CreateDoor), 0.05f);
            //Invoke(nameof(CreateBoom), 0.05f);
            //Invoke(nameof(CreateChain), 0.05f);
            //Invoke(nameof(CreateKey), 0.05f);
            //Invoke(nameof(CreateLock), 0.05f);
        }


        public void lockItem()
        {
            if (LevelManager.Instance.GetLevleNum() == 3)
            {
                GameDataManager.AddItemCount(ItemType.Hole, 2);

                ItemMoveManager.Instance.MoveItem(ItemType.Hole, () =>
                {
                    //解锁按钮
                    MainSceneUI.Instance._GamePlayUI.InitButon();
                    //移动完成之后，增加道具的数量
                    MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.Hole);
                    //显示道具使用引导
                    ShowItemUseGuite();
                });
            }
            else if (LevelManager.Instance.GetLevleNum() == 4)
            {

                GameDataManager.AddItemCount(ItemType.Rocket, 2);

                ItemMoveManager.Instance.MoveItem(ItemType.Rocket, () =>
                {
                    //解锁火箭道具
                    MainSceneUI.Instance._GamePlayUI.InitButon();
                    MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.Rocket);
                    //显示道具使用引导
                    ShowItemUseGuite();
                });
            }
            else if (LevelManager.Instance.GetLevleNum() == 5)
            {

                GameDataManager.AddItemCount(ItemType.DoubleBox, 2);

                ItemMoveManager.Instance.MoveItem(ItemType.DoubleBox, () =>
                {
                    MainSceneUI.Instance._GamePlayUI.InitButon();
                    MainSceneUI.Instance._GamePlayUI.UpdateItemCount(ItemType.DoubleBox);
                    //显示道具使用引导
                    ShowItemUseGuite();
                });
            }
        }

        private void ShowItemUseGuite()
        {
            return;
            GuiteItem guiteItem = null;
            if (LevelManager.Instance.GetLevleNum() == 3)
            {
                guiteItem = new GuiteItem()
                {
                    dexText = "解锁道具1",
                    DesImageX = 0,
                    DesImageY = 894,
                    circleX = 279,
                    circleY = 3000f,
                    handleX = 360,
                    handleY = 3000,
                    isNeedShowButton = true,
                    maskType = 1,
                    x1 = -362.72f,
                    y1 = -1177.4f,
                    x2 = -174.8f,
                    y2 = -983f,
                    isNeedShowClikTip = true,
                    TdIndex = 7
                };
            }
            else if (LevelManager.Instance.GetLevleNum() == 4)
            {
                guiteItem = new GuiteItem()
                {
                    dexText = "解锁道具2",
                    DesImageX = 0,
                    DesImageY = 894,
                    circleX = 279,
                    circleY = 3000f,
                    handleX = 360,
                    handleY = 3000,
                    isNeedShowButton = true,
                    maskType = 1,
                    x1 = -94.3f,
                    y1 = -1177.4f,
                    x2 = 93.5f,
                    y2 = -983f,
                    isNeedShowClikTip = true,
                    TdIndex = 7
                };
            }
            else if (LevelManager.Instance.GetLevleNum() == 5)
            {
                guiteItem = new GuiteItem()
                {
                    dexText = "解锁道具3",
                    DesImageX = 0,
                    DesImageY = 894,
                    circleX = 279,
                    circleY = 3000f,
                    handleX = 360,
                    handleY = 3000,
                    isNeedShowButton = true,
                    maskType = 1,
                    x1 = 159.3f,
                    y1 = -1177.4f,
                    x2 = 379.9f,
                    y2 = -983f,
                    isNeedShowClikTip = true,
                    TdIndex = 7
                };
            }

            EventManager.Instance.TriggerEvent<GuiteItem>(GameEvent.SetMaskRect, guiteItem);
        }

        /// <summary>
        /// 特定关卡生成
        /// </summary>
        private void UniqueInitObj()
        {
            MainSceneUI.Instance.SetLevelTip(false);
            Sprite icon = null;

            switch (levelNum)
            {
                case 3:
                    if (!GameDataManager.CurrentGameData.isHoleLocked)
                    {
                        return;
                    }
                    GameDataManager.UnlockItem(ItemType.Hole);
                    Invoke(nameof(lockItem), 0.05f);
                    break;
                case 4:
                    if (!GameDataManager.CurrentGameData.isRocketLocked)
                    {
                        return;
                    }
                    GameDataManager.UnlockItem(ItemType.Rocket);
                    Invoke(nameof(lockItem), 0.05f);
                    break;
                case 5:
                    if (!GameDataManager.CurrentGameData.isDoubleBoxLocked)
                    {
                        return;
                    }
                    GameDataManager.UnlockItem(ItemType.DoubleBox);
                    Invoke(nameof(lockItem), 0.05f);
                    break;
                default:
                    MainSceneUI.Instance.SetLevelTip(false);
                    break;
            }

            //有闹钟
            if (CurLevel.HasClock)
            {
                MainSceneUI.Instance._GamePlayUI.ShowClockUI(CurLevel.GetMinutes, CurLevel.GetSeconds);
                Debug.Log("显示闹钟");
            }
        }

        private async Task LoadLevelAsync()
        {
            if (levelNum % 10 == 0)
                await LoadLevelResources();

            ReleaseLevelResources();
        }

        private GameObject CreateLevelInstance(int levelNum)
        {
            // 暂时固定加载名为"1"的关卡数据
            SavedLevelData levelData = LoadLevelDataFromJson("1");
            if (levelData == null) return null;

            // 创建关卡根物体
            GameObject levelObject = new GameObject("Level");
            Level levelComponent = levelObject.AddComponent<Level>();

            // 创建所有层级
            foreach (var layerData in levelData.layers)
            {
                GameObject layerObject = new GameObject($"layer ({layerData.layerIndex})");
                layerObject.transform.SetParent(levelObject.transform);
                Layer layerComponent = layerObject.AddComponent<Layer>();
                layerObject.layer = layerData.unityLayer;

                // 创建所有glass
                foreach (var glassData in layerData.glasses)
                {
                    GameObject glassPrefab = ResourceLoader.Instance.GetRes<GameObject>("Prefab/Hole/glass.prefab");
                    if (glassPrefab != null)
                    {
                        GameObject glassInstance = Instantiate(glassPrefab, layerObject.transform);
                        glassInstance.transform.localPosition = glassData.position.ToVector3();
                        glassInstance.transform.localEulerAngles = glassData.rotation.ToVector3(); // 设置旋转
                        glassInstance.layer = layerObject.layer;

                        // 设置glass的sprite
                        var renderer = glassInstance.GetComponent<SpriteRenderer>();
                        if (renderer != null)
                        {
                            renderer.sprite = ResourceLoader.Instance.GetRes<Sprite>(glassData.spritePath);
                            renderer.sortingLayerName = LayerMask.LayerToName(layerObject.layer);
                        }

                        // 创建所有hole
                        foreach (var holeData in glassData.holes)
                        {
                            GameObject holePrefab = ResourceLoader.Instance.GetRes<GameObject>("Prefab/Hole/Hole.prefab");
                            if (holePrefab != null)
                            {
                                GameObject holeInstance = Instantiate(holePrefab, glassInstance.transform);
                                holeInstance.transform.localPosition = holeData.position.ToVector3();
                                holeInstance.layer = layerObject.layer;

                                string currentLayerName = LayerMask.LayerToName(layerObject.layer);

                                var screwObj = holeInstance.transform.Find("Screw");
                                screwObj.gameObject.layer = layerObject.layer;

                                var imageRenderer = screwObj.Find("Image").GetComponent<SpriteRenderer>();
                                imageRenderer.sprite = ResourceLoader.Instance.GetRes<Sprite>(holeData.spritePath);
                                imageRenderer.sortingLayerName = currentLayerName;
                                screwObj.GetComponent<Screw>().SetColor(GameTool.DetermineScrewColor(holeData.spritePath));
                                var spriteMask = holeInstance.transform.Find("Mask").GetComponent<SpriteMask>();
                                spriteMask.frontSortingLayerID = SortingLayer.NameToID(currentLayerName);
                                // 直接设置为前一层
                                spriteMask.backSortingLayerID = SortingLayer.NameToID($"Glass{int.Parse(currentLayerName.Replace("Glass", "")) - 1}");
                            }
                        }
                    }
                }
            }

            return levelObject;
        }

        private void InitLevelGameObj()
        {
            Transform gameParent = GameManager.Instance.transform;

            // SavedLevelData levelData = LoadLevelDataFromJson("1");
            // 创建关卡实例
            //level = CreateLevelInstance(levelNum);
            loadNextLevel = 0;
            //level = Instantiate(levels["Level " + levelNum.ToString()]); ;
            level = Instantiate(ResourceLoader.Instance.GetLevelsOnlyWithNum(levelNum));
            AdjustUpOnLevel(levelNum);
            if (level != null)
            {
                level.transform.localPosition = new Vector3(0, -2, 0);
                level.transform.SetParent(gameParent);
                CurLevel = level.GetComponent<Level>();
            }
            CheckIsOneLevel(levelNum);//第一关显示特殊新手引导
            CheckNeedShowTaskUI();//检查是否需要显示任务界面
            boxlevel = Instantiate(ResourceLoader.Instance.GetLevelBoxOnlyWithNum(levelNum));
            boxlevel.name = "BoxLevel";
            boxlevel.transform.SetParent(gameParent);
            boxlevel.transform.localPosition = Vector3.zero;
            TDAnalyticsManager.Instance.SendEnterLevel(levelNum);
        }
        /// <summary>
        /// 检查是否需要显示任务界面    
        /// </summary>
        private void CheckNeedShowTaskUI()
        {
            if (GameTool.isShowTaskUI || GameDataManager.CurrentGameData.completeBoxNum < 8)
            {
                return;
            }
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
        private void AdjustUpOnLevel(int levelNum)
        {
            if (GameDataManager.CurrentGameData.levelUpOnState.ContainsKey(levelNum))
            {
                return;
            }
            GameDataManager.CurrentGameData.levelUpOnState.Add(levelNum, true);
            GameDataManager.Save();
            string evetName = "";
            if (levelNum == 1)
            {
                evetName = "bivg68";
            }
            if (levelNum == 2)
            {
                evetName = "7tbqhh";
            }
            if (levelNum == 3)
            {
                evetName = "ocaff9";
            }
            if (levelNum == 4)
            {
                evetName = "6bbexs";
            }
        }

        /// <summary>
        /// 第一关显示特殊新手引导
        /// </summary>
        /// <param name="levelNum"></param>
        private void CheckIsOneLevel(int levelNum)
        {
            if (levelNum != 1)
            {
                return;
            }

            Invoke(nameof(CreatHand), 0.05f);


        }
        public void CreatHand()
        {

            //if (GameDataManager.CurrentGameData.isGetNewPlayerGuite)
            //{
                GameObject hand = Instantiate(ResourceLoader.Instance.GetRes<GameObject>("AssetBundleLocal/NoNeedLoadRes/Prefab/NewPlayerGuite/Hand.prefab"));
                hand.transform.SetParent(level.transform);
                CurLevel.ShowNewScrewGuite(1);//显示新手引导
                CurLevel.SetEndLayer();
                GameManager.Instance.SetBoxGuite();
            //}
            //else
            //{
            //    UIManager.Instance.ShowUI<NewPlayerGetAGiftPlane>();
            //}
        }

        private GameObject CreatBox(List<BoxData> boxSequence)
        {
            // 创建一个空物体作为所有box的父物体
            GameObject boxLevelObject = new GameObject("BoxLevel");

            // 遍历box序列
            foreach (var boxData in boxSequence)
            {
                // 加载box预制体
                GameObject boxPrefab = ResourceLoader.Instance.GetRes<GameObject>("Prefab/Hole/box.prefab");
                if (boxPrefab != null)
                {
                    // 实例化box
                    GameObject boxInstance = Instantiate(boxPrefab, boxLevelObject.transform);

                    // 设置sprite
                    var spriteRenderer = boxInstance.GetComponent<SpriteRenderer>();
                    string spriteName = "";
                    ScrewColor boxColor = ScrewColor.Purple; // 默认紫色

                    // 根据颜色设置对应的sprite和boxColor
                    switch (boxData.color)
                    {
                        case "蓝色":
                            spriteName = "box_dark_blue3";
                            boxColor = ScrewColor.Blue;
                            break;
                        case "绿色":
                            spriteName = "box_dark_green3";
                            boxColor = ScrewColor.Green;
                            break;
                        case "紫色":
                            spriteName = "box_dark_purple3";
                            boxColor = ScrewColor.Purple;
                            break;
                    }

                    // 加载并设置sprite
                    spriteRenderer.sprite = ResourceLoader.Instance.GetRes<Sprite>($"Assets/Images/BoxAndBall/{spriteName}.png");

                    // 在Start方法调用前设置BoxColor
                    Box boxComponent = boxInstance.GetComponent<Box>();
                    boxComponent.Init(boxColor);
                }
            }

            return boxLevelObject;
        }

        public void ClearLevel()
        {
            if (boxlevel != null)
                Destroy(boxlevel);

            if (level != null)
                Destroy(level);

            foreach (GameObject key in keys)
            {
                Destroy(key);
            }
            keys.Clear();

            foreach (GameObject loc in locks)
            {
                Destroy(loc);
            }
            locks.Clear();

        }
        /// <summary>
        /// 销毁物体时，不能够立即获得数据，需要延迟调用
        /// </summary>
        private void DelayInitBoxLevel()
        {
            GameManager.Instance.InitBoxLevel();
            //初始化关卡数字
            MainSceneUI.Instance.SetLevelNum(levelNum);
        }

        //增加关卡数
        public void AddLevelNum()
        {
            GameDataManager.AddLevelNum();
            levelNum = GameDataManager.CurrentGameData.levelNum;
        }
        public void ReStartGmae()
        {
            InitLevel();
        }

        #region 全局生成
        /// <summary>
        /// 生成绳子（已移除Obi支持）
        /// </summary>
        private void CreateRope()
        {
            // Obi rope functionality has been removed
        }
        /// <summary>
        /// 生成冰
        /// </summary>
        private void CreateIce()
        {
            Level curLevel = level.GetComponent<Level>();
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
                                //遍历小球
                                List<Screw> screwList = curGlass.ScrewList;
                                foreach (Screw curScrew in screwList)
                                {
                                    //生成冰
                                    if (curScrew.IsIceCovered)
                                    {
                                        GameObject curIce = Instantiate(icePrefab, curScrew.transform.parent);
                                        curIce.transform.position = curScrew.transform.position;
                                        curIce.GetComponent<Ice>().SetSortingLayer(curScrew.LayerName);

                                        curScrew.ScrewIce = curIce.GetComponent<Ice>();
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }
        /// <summary>
        /// 生成门
        /// </summary>
        private void CreateDoor()
        {
            Level curLevel = level.GetComponent<Level>();
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
                                    //生成门
                                    if (curScrew.HasDoor)
                                    {
                                        GameObject newDoor = Instantiate(doorPrefab, curScrew.transform.parent);
                                        newDoor.transform.position = curScrew.transform.position;

                                        Door doorScript = newDoor.GetComponent<Door>();
                                        curScrew.ScrewDoor = doorScript;

                                        doorScript.SetClose(curScrew.IsDoorClose);
                                        doorScript.SetLayer(curScrew.LayerName, curScrew.LayerOrder);
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }
        /// <summary>
        /// 生成炸弹
        /// </summary>
        private void CreateBoom()
        {
            Level curLevel = level.GetComponent<Level>();
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
                                    //生成炸弹
                                    if (curScrew.HasBoom)
                                    {
                                        GameObject newBoom = Instantiate(boomPrefab, curScrew.transform.parent);
                                        newBoom.transform.position = curScrew.transform.position;
                                        newBoom.gameObject.SetActive(true);

                                        Boom boomScript = newBoom.GetComponent<Boom>();
                                        boomScript.InitBoom(curScrew.ScrewColor, curScrew.BoomCount, curScrew.LayerName, curScrew.LayerOrder);
                                        curScrew.ScrewBoom = boomScript;
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }
        /// <summary>
        /// 生成锁
        /// </summary>
        private void CreateChain()
        {
            Level curLevel = level.GetComponent<Level>();
            if (curLevel != null && curLevel.HasChain)
            {
                //遍历关卡层级
                List<Layer> layerList = curLevel.LayerList;
                foreach (Layer curLayer in layerList)
                {
                    if (curLayer.HasChain)
                    {
                        //遍历玻璃
                        List<Glass> glassList = curLayer.GlassList;
                        foreach (Glass curGlass in glassList)
                        {
                            if (curGlass.HasChain)
                            {
                                //遍历小球
                                List<Screw> screwList = curGlass.ScrewList;
                                foreach (Screw curScrew in screwList)
                                {
                                    //生成锁
                                    if (curScrew.HasChain)
                                    {
                                        //创建锁（Obi chain functionality removed）
                                        GameObject curChainFx = Instantiate(chainFxPrefab, curScrew.transform.parent);
                                        curChainFx.transform.position = curScrew.transform.position;

                                        curScrew.SetChain(curChainFx.GetComponent<ChainFx>());

                                    }
                                }
                            }

                        }
                    }

                }
            }
        }

        /// <summary>
        /// 生成钥匙
        /// </summary>
        private void CreateKey()
        {
            Level curLevel = level.GetComponent<Level>();
            if (curLevel != null && curLevel.HasKey)
            {
                //遍历关卡层级
                List<Layer> layerList = curLevel.LayerList;
                foreach (Layer curLayer in layerList)
                {
                    if (curLayer.HasKey)
                    {
                        //遍历玻璃
                        List<Glass> glassList = curLayer.GlassList;
                        foreach (Glass curGlass in glassList)
                        {
                            if (curGlass.HasKey)
                            {
                                //遍历小球
                                List<Screw> screwList = curGlass.ScrewList;
                                foreach (Screw curScrew in screwList)
                                {
                                    //生成钥匙
                                    if (curScrew.HasKey)
                                    {
                                        GameObject newKey = Instantiate(keyPrefab, curScrew.transform.parent);
                                        newKey.transform.position = curScrew.transform.position + new Vector3(0, -0.4f, 0);
                                        newKey.SetActive(true);

                                        Key curKey = newKey.GetComponent<Key>();
                                        curScrew.ScrewKey = curKey;
                                        curKey.InitKeyLayer(curScrew.LayerName, curScrew.LayerOrder - 1);

                                        keys.Add(newKey);
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }

        /// <summary>
        /// 生成门锁
        /// </summary>
        private void CreateLock()
        {
            Level curLevel = level.GetComponent<Level>();
            if (curLevel != null && curLevel.HasLock)
            {
                //遍历关卡层级
                List<Layer> layerList = curLevel.LayerList;
                foreach (Layer curLayer in layerList)
                {
                    if (curLayer.HasLock)
                    {
                        //遍历玻璃
                        List<Glass> glassList = curLayer.GlassList;
                        foreach (Glass curGlass in glassList)
                        {
                            if (curGlass.HasLock)
                            {
                                //遍历小球
                                List<Screw> screwList = curGlass.ScrewList;
                                foreach (Screw curScrew in screwList)
                                {
                                    //生成钥匙
                                    if (curScrew.HasLock)
                                    {
                                        GameObject newLock = Instantiate(lockPrefab, curScrew.transform.parent);
                                        newLock.transform.position = curScrew.transform.position;
                                        newLock.SetActive(true);

                                        Lock curLock = newLock.GetComponent<Lock>();
                                        curScrew.ScrewLock = curLock;
                                        curLock.InitLockLayer(curScrew.LayerName, curScrew.LayerOrder + 1);

                                        locks.Add(newLock);
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }
        #endregion

        /// <summary>
        /// 异步加载资源(预加载)
        /// </summary>
        /// <returns></returns>
        private async Task LoadLevelResources()
        {
            int num = levelNum + 10;

            if (num >= 210)
                return;

            await ResourceLoader.Instance.LoadLevelResources(num);
            //levels = ResourceLoader.Instance.Levels;
            //boxs = ResourceLoader.Instance.BoxLevels;
        }

        private void ReleaseLevelResources()
        {
            int num = levelNum % 10;
            //释放之前的资源
            if (num == 1)
            {
                ResourceLoader.Instance.ReleaseLevelResources(NameUtility.SetLastDigitToZero(levelNum));
            }
        }

        private SavedLevelData LoadLevelDataFromJson(string levelName)
        {
            string levelJsonPath = Application.streamingAssetsPath + "/Level.json";
            string jsonContent = null;
            
#if UNITY_ANDROID && !UNITY_EDITOR
            // Android平台使用UnityWebRequest
            using (UnityWebRequest www = UnityWebRequest.Get(levelJsonPath))
            {
                www.SendWebRequest();
                while (!www.isDone) { }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    jsonContent = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                }
                else
                {
                    Debug.LogError($"[LevelManager] Failed to load level json: {www.error}");
                }
            }
#else
            // iOS和Editor平台直接读取文件
            try
            {
                if (File.Exists(levelJsonPath))
                {
                    jsonContent = File.ReadAllText(levelJsonPath);
                }
                else
                {
                    Debug.LogError($"[LevelManager] Level json file not found: {levelJsonPath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[LevelManager] Failed to read level json: {e.Message}");
            }
#endif

            if (!string.IsNullOrEmpty(jsonContent))
            {
                var levelConfig = JsonConvert.DeserializeObject<LevelConfigData>(jsonContent);
                if (levelConfig != null && levelConfig.levels.ContainsKey(levelName))
                {
                    return levelConfig.levels[levelName];
                }
            }

            Debug.LogError($"[LevelManager] Level data not found for level: {levelName}");
            return null;
        }
    }
}
