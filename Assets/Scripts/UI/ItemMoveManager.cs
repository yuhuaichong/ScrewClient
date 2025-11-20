using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.UI;


namespace DafultScript
{
    public enum ItemType
    {
        Hole, Rocket, DoubleBox
    }

    public class ItemMoveManager : MonoBehaviour
    {
        // 单例静态实例
        public static ItemMoveManager Instance { get; private set; }
        //[SerializeField] private Sprite hole;
        //[SerializeField] private Sprite rocket;
        //[SerializeField] private Sprite doubleBox;

        private float dropDistance = 1f;    // 初始向下的距离
        private float moveDuration = 1f;    // 总的运动时间
        private float delayTime = 0.06f;

        private float textTime = 1;
        private float scaleCount = 1.5f;
        private List<Vector3> coinLeftPosList = new List<Vector3>();
        private List<Vector3> coinRightPosList = new List<Vector3>();

        private Transform coinLeftTrans;
        private Transform coinRightTrans;
        private Transform unlockItemTrans;
        private Image itemIcon;
        private Vector3 orginItemPos;

        private Transform starTrans;
        private Vector3 orginStarPos;

        private Transform gameLeft;
        private Transform gameRight;
        private Transform homeLeft;
        private Transform homeRight;
        private Transform starMoveTarget;

        private void Awake()
        {
            // 单例模式实现
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // 如果已经存在实例，销毁当前对象
                return;
            }
            Instance = this;

            // 初始化逻辑
            coinLeftTrans = transform.Find("CoinLeft").transform;
            coinRightTrans = transform.Find("CoinRight").transform;

            unlockItemTrans = transform.Find("Hole").transform;
            orginItemPos = unlockItemTrans.position;
            itemIcon = unlockItemTrans.GetComponent<Image>();

            starTrans = transform.Find("Par Star").transform;
            orginStarPos = starTrans.position;

        }
        private void Start()
        {
            unlockItemTrans.gameObject.SetActive(false);
            starTrans.gameObject.SetActive(false);

            gameLeft = UIManager.Instance.GetUI<WinUI>().CoinTrans;
            gameRight = UIManager.Instance.GetUI<WinUI>().PiggyTrans;

            //starMoveTarget = HomeSceneUI.Instance.homeUI.StarTrans;

            InitCoin(coinLeftTrans, coinLeftPosList);
            InitCoin(coinRightTrans, coinRightPosList);

        }

        private void InitCoin(Transform parent, List<Vector3> InitList)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                child.gameObject.SetActive(false);
                InitList.Add(child.position);
            }
        }
        #region 硬币移动
        public void MoveCoin(int startVal1, int endVal1, int startVal2, int endVal2, System.Action onComplete = null)
        {
            //AudioManager.Instance.PlaySFX("Coin");

            int totalCoins = coinLeftTrans.childCount + coinRightTrans.childCount;
            int completedCoins = 0;

            // 定义局部回调
            Action onChildComplete = () =>
            {
                completedCoins++;
                if (completedCoins >= totalCoins)
                {
                    onComplete?.Invoke();
                }
            };
            Transform leftTarget = gameLeft;
            Transform rightTaget = gameRight;

            //游戏在点击next按钮时，关卡数已经增加了
            if (LevelManager.Instance.GetLevleNum() >= 6 && false)
            {
                leftTarget = homeLeft;
                rightTaget = homeRight;

                if (homeLeft == null)
                {
                    homeLeft = HomeSceneUI.Instance.homeUI.CoinTrans;
                    homeRight = HomeSceneUI.Instance.homeUI.PiggyTrans;
                    leftTarget = homeLeft;
                    rightTaget = homeRight;
                }
            }

            // 开始移动子物体
            MoveChildrenToTarget(coinLeftTrans, leftTarget, coinLeftPosList, onChildComplete);
            MoveChildrenToTarget(coinRightTrans, rightTaget, coinRightPosList, onChildComplete);

            Text text1 = null;
            if (LevelManager.Instance.GetLevleNum() < 6 || true)
            {
                text1 = UIManager.Instance.GetUI<WinUI>().CoinText;
                Text text2 = UIManager.Instance.GetUI<WinUI>().PiggyText;
                TextEffect(text2, startVal2, endVal2);
            }
            else
            {
                text1 = HomeSceneUI.Instance.homeUI.GetCoinText();
            }
            TextEffect(text1, startVal1, endVal1);
        }
        private void MoveChildrenToTarget(Transform parent, Transform target, List<Vector3> InitPos, Action onChildComplete)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                child.gameObject.SetActive(true);
                child.position = InitPos[i];
                // 计算每个子物体的延迟
                float delay = i * delayTime;

                // 调用移动逻辑
                MoveToTarget(child, delay, target.position, onChildComplete);
            }
        }

        private void MoveToTarget(Transform child, float delay, Vector3 targetPos, Action onChildComplete)
        {
            Vector3 startPos = child.position;                      // 初始位置
            Vector3 dropPos = startPos + Vector3.down * dropDistance; // 向下偏移的位置    

            Sequence sequence = DOTween.Sequence();

            sequence.Append(child.DOMove(dropPos, moveDuration * 0.5f).SetEase(Ease.OutQuad));

            sequence.Append(child.DOMove(targetPos, moveDuration * 0.5f).SetEase(Ease.InOutQuad));

            sequence.PrependInterval(delay);

            // 动画完成后回调
            sequence.OnComplete(() =>
            {
                child.gameObject.SetActive(false);
                onChildComplete?.Invoke(); // 每个子物体完成时调用局部回调
            });
        }
        #endregion

        /// <summary>
        /// 变化文本
        /// </summary>
        /// <param name="targetText"></param>
        /// <param name="startValue">开始数量</param>
        /// <param name="endValue">结束数量</param>
        private void TextEffect(Text targetText, int startValue, int endValue)
        {
            // 数值变化
            DOTween.To(() => startValue, x =>
            {
                startValue = x;
                targetText.text = startValue.ToString(); // 更新UI文本
            }, endValue, textTime).SetEase(Ease.Linear).SetDelay(0.15f);

            // 缩放动画
            targetText.transform.DOScale(scaleCount, textTime * 0.75f) // 放大
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    targetText.transform.DOScale(1f, textTime * 0.25f) // 缩回原来大小
                        .SetEase(Ease.InQuad);
                }).SetDelay(0.15f);
        }

        /// <summary>
        /// 洞道具移动
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="callback"></param>
        public void MoveItem(ItemType type, System.Action callback = null)
        {

            //Vector3 movePos = MainSceneUI.Instance._GamePlayUI.HoleTrans.position;
            //itemIcon.sprite = hole;

            //if (type == ItemType.Rocket)
            //{
            //    movePos = MainSceneUI.Instance._GamePlayUI.RocketTrans.position;
            //    itemIcon.sprite = rocket;
            //}
            //else if (type == ItemType.DoubleBox)
            //{
            //    movePos = MainSceneUI.Instance._GamePlayUI.DoubleBoxTrans.position;
            //    itemIcon.sprite = doubleBox;
            //}

            //unlockItemTrans.gameObject.SetActive(true);
            //unlockItemTrans.position = orginItemPos;

            //Sequence sequence = DOTween.Sequence();
            //// 添加移动动画
            //sequence.Append(unlockItemTrans.DOMove(movePos, 0.8f).SetEase(Ease.OutQuad));

            //// 添加缩放动画
            //sequence.Join(unlockItemTrans.DOScale(new Vector3(0.42f, 0.42f, 0.42f), 0.8f).SetEase(Ease.OutQuad));

            //// 在动画全部完成后调用回调
            //sequence.OnComplete(() =>
            //{
            //    unlockItemTrans.gameObject.SetActive(false);
            //    callback?.Invoke();
            //});
        }

        /// <summary>
        /// 星星移动
        /// </summary>
        public void MoveAndRotateStar()
        {
            starTrans.gameObject.SetActive(true);
            starTrans.position = orginStarPos;

            // 目标缩放值
            Vector3 targetScale = new Vector3(1f, 1f, 1f);

            // 计算旋转圈数（360度为一圈）
            float totalRotationTime = moveDuration / 1; // 这里的 1 是旋转一圈的时间，可以根据需求修改
            float totalRotation = 360f * Mathf.Ceil(totalRotationTime);

            // 创建动画序列
            Sequence sequence = DOTween.Sequence();

            // 添加移动动画
            sequence.Join(starTrans.DOMove(starMoveTarget.position, moveDuration)
                .SetEase(Ease.InCubic));

            // 添加旋转动画
            sequence.Join(starTrans.DORotate(new Vector3(0, 0, totalRotation), moveDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.InCubic));

            // 添加缩放动画
            sequence.Join(starTrans.DOScale(targetScale, moveDuration)
                .SetEase(Ease.InCubic));

            // 设置完成回调
            sequence.OnComplete(() =>
            {
                starTrans.gameObject.SetActive(false);
                //Debug.Log("移动、旋转和缩放动画完成！");
            });
        }


        public void TextAddStarCount()
        {
            GameDataManager.CurrentGameData.starCount += 200;
            HomeSceneUI.Instance.homeUI.UpdateResourceText();
        }
    }
}

