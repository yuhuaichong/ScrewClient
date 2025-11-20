using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DafultScript;
public class StickerButton : MonoBehaviour
{
    [SerializeField] private List<GameObject> unlockList = new List<GameObject>();

    private Image icon;
    private Button clickButton;
    private GameObject compeletedText;
    private Text count;

    private Sticker sticker;
    private StickerUI ui;
    private bool isInit = false;
    private void Awake()
    {
        if(!isInit)
            InitFindItem();
    }
    private void InitFindItem()
    {
        icon = transform.Find("Icon").GetComponent<Image>();
        clickButton = transform.Find("Button").GetComponent<Button>();
        compeletedText = transform.Find("Text").gameObject;
        count = clickButton.transform.Find("Count").GetComponent<Text>();

        clickButton.onClick.AddListener(UnlockEvent);

        isInit = true;
    }
    public void FreshUnlockStickerList(List<GameObject> lis, StickerUI _ui)
    {
        unlockList.Clear();
        unlockList.AddRange(lis);
        ui = _ui;
    }
    public void FreshButton(ButtonItem item)
    {
        if (count == null)
        {
            InitFindItem();
        }
        ImageUtility.SetImageSpriteWithAspect(icon, item.GetSprite(), 180, 180);
        count.text = item.GetCount().ToString();
    }
    private void UnlockEvent()
    {
        AudioManager.Instance.PlaySFX("Click");
        int curStarCount = int.Parse(count.text);

        if (curStarCount > GameDataManager.CurrentGameData.starCount)
        {
            //Debug.Log("没有足够的星星了！");

            return;
        }


        SetButton(false);
        //生成星星
        sticker = StickerManager.Instance.CurSticker;
        int curindex = GameDataManager.CurrentGameData.curButtonIndex;
        //减少星星数
        GameDataManager.DecreaseStarCount(curStarCount);
        //更新文本
        HomeSceneUI.Instance.stickerUI.UpdateStarText();

        StartCoroutine(InitStarCoroutine(0.1f, curStarCount, curindex));

        //解锁对应的贴纸
        GameDataManager.UnlockSticker(unlockList[curindex].name);

       ui.UpdateStikcerProgress();
    }

    /// <summary>
    /// 生成星星的协程
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator InitStarCoroutine(float delayTime, int curStarCount, int curindex)
    {
        bool canCallBack = true;
        for (int i = 0; i < curStarCount; i++)
        {
            StickerManager.Instance.InitStar(unlockList[curindex].transform.position, () =>
            {
                if (canCallBack)
                {
                    //Debug.Log("回调");
                    canCallBack = false;
                    sticker.UnlockObj(unlockList[curindex].name);
                    Invoke(nameof(DelayCheckComplete), 0.5f);
                }
            });

            yield return new WaitForSeconds(delayTime);
        }
    }
    private void DelayCheckComplete()
    {
        if (sticker.CanChange)
        {
            ui.FreshStickerButton();
            sticker.CompleteChange();
        }
    }
    //是否解锁按钮
    public void SetButton(bool val)
    {
        clickButton.gameObject.SetActive(val);
        compeletedText.SetActive(!val);
    }


}
