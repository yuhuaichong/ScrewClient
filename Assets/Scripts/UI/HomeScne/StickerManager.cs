using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using DafultScript;
public class StickerManager : MonoBehaviour
{
    // 静态实例，用于全局访问
    public static StickerManager Instance { get; private set; }
    [SerializeField] private GameObject buildingFx;
    [SerializeField] private GameObject smokeFx;
    [SerializeField] private Transform starTrans;
    [SerializeField] private GameObject star;
    [SerializeField] private List<Sticker> stickerList = new List<Sticker>();
    [SerializeField] private int curStickIndex;
    public Sticker CurSticker
    {
        get
        {

            if (stickerList.Count == 0)
            {
                InitStickerList();
            }
            return stickerList[GameDataManager.CurrentGameData.curStickerIndex];
        }
            
    }
    public Sticker NextSticker
    {
        get => stickerList[GameDataManager.CurrentGameData.curStickerIndex + 1];
    }
    public GameObject GetBuildingFx()
    {
        return buildingFx;
    }
    private ObjectPool starPool;

    /// <summary>
    /// 已经到达最后一个贴纸
    /// </summary>
    /// <returns></returns>
    public bool IsLastSticker()
    {
        return GameDataManager.CurrentGameData.curStickerIndex >= stickerList.Count - 1;
    }
    public void ChangeNextSticker()
    {
        if (IsLastSticker())
        {
            Debug.Log("已没有下一个贴纸");
            return;
        }
        CurSticker.gameObject.SetActive(false);
        GameDataManager.AddStickerIndex();
        CurSticker.gameObject.SetActive(true);
        //初始化贴纸列表
        CurSticker.InitSticker(GameDataManager.GetUnlockStickerList());
        
        curStickIndex = GameDataManager.CurrentGameData.curStickerIndex;//用于显示
    }
    private void Awake()
    {
        // 确保只有一个 StickerManager 实例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 销毁重复的实例
        }
        else
        {
            Instance = this;  // 设置当前实例为单例
        }
    }

    /// <summary>
    /// 初始化贴纸列表
    /// </summary>
    private void InitStickerList()
    {
        foreach (Transform c in transform)
        {
            stickerList.Add(c.GetComponent<Sticker>());
        }
        starPool = new ObjectPool(star, 6, starTrans);

        for (int i = 0; i < stickerList.Count; i++)
        {
            if (i == GameDataManager.CurrentGameData.curStickerIndex)
            {
                stickerList[i].InitSticker(GameDataManager.GetUnlockStickerList());
                stickerList[i].gameObject.SetActive(true);
            }
            else
                stickerList[i].gameObject.SetActive(false);
        }

        curStickIndex = GameDataManager.CurrentGameData.curStickerIndex;//用于显示
    }

    // 复制star并使用DOTween移动
    public void InitStar(Vector3 targetPosition, System.Action callback = null)
    {
        // 从对象池中获取一个新的star实例
        GameObject newStar = starPool.GetObj();

        RectTransform newStarRect = newStar.GetComponent<RectTransform>();
        RectTransform starTransRect = HomeSceneUI.Instance.homeUI.StarTrans.GetComponent<RectTransform>();

        // 将 StarTrans 的世界坐标转换为 newStar 所在父对象的局部坐标
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            newStarRect.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, starTransRect.position),
            null,
            out localPoint
        );

        // 设置 newStar 的 anchoredPosition
        newStarRect.anchoredPosition = localPoint;

        // 使用DOTween来实现移动动画，先快后慢
        newStar.transform.DOMove(targetPosition, 1.2f)  // 移动时间2秒
            .SetEase(Ease.InOutSine)// 先快后慢的效果，InOutSine缓动
            .OnComplete(() =>
            {
                //显示烟雾动画
                GameObject smoke = Instantiate(smokeFx, starTrans);
                smoke.transform.position = newStar.transform.position;

                starPool.ReturnToPool(newStar);
                Destroy(smoke, 1f);
                callback?.Invoke();
            });

    }

    //收集调用
    /// <summary>
    /// 展示贴纸
    /// </summary>
    /// <param name="index"></param>
    public void ShowStikcerByIndex(int index)
    {
        for (int i = 0; i < stickerList.Count; i++)
        {
            if (i == index)
            {
                stickerList[i].gameObject.SetActive(true);
            }
            else
                stickerList[i].gameObject.SetActive(false);
        }

        stickerList[index].ShowUnlockedItem();
    }

    /// <summary>
    /// 显示当前贴纸
    /// </summary>
    public void ShowCurSticker()
    {
        CurSticker.gameObject.SetActive(true);
        //初始化贴纸列表
        CurSticker.InitSticker(GameDataManager.GetUnlockStickerList());
    }

    public bool IsStickerCompeleted()
    {
        if (curStickIndex == stickerList.Count - 1 && CurSticker.IsCompleted())
        {
            return true;
        }

        return false;
    }
}
