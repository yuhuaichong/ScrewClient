using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using DafultScript;

public class SliderBubble : MonoBehaviour
{
    Button button;
    RectTransform rectTransform;
    Vector2 moveDirection;
    float moveSpeed = 150f;
    bool isMoving = false;
    float halfScreenWidth;
    float halfScreenHeight;
    float bubbleWidth;
    float bubbleHeight;

    ConfBubbleCanGetReward confBubbleCanGetReward;
    ConfItem confItem;
    

    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        button.onClick.AddListener(ButtonOnClikHandle);
        
        // 初始化尺寸信息（使用Canvas尺寸）
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        halfScreenWidth = canvasRect.rect.width * 0.5f;
        halfScreenHeight = canvasRect.rect.height * 0.5f;
        
        bubbleWidth = rectTransform.rect.width;
        bubbleHeight = rectTransform.rect.height;
    }

    internal void Init()
    {
        gameObject.SetActive(false); // 初始化时隐藏
        EventManager.Instance.RegisterEvent(GameEvent.ShowGift, ShowGift);
    }
    private void OnDestroy()
    {
        EventManager.Instance.UnregisterEvent(GameEvent.ShowGift, ShowGift);
    }

    private void ShowGift()
    {
        if(gameObject.activeSelf) return;
        transform.SetParent(UIManager.Instance.GetUI<LoseUI>().transform.parent.Find("WithDrawTipBgPar"));
        gameObject.SetActive(true); // 激活宝箱
        StartCoroutine(FlyInEffect());
    }
    private IEnumerator FlyInEffect()
    {
        // 获取 RectTransform
        RectTransform rectTransform = GetComponent<RectTransform>();

        // 设置初始位置
        float halfScreenWidth = Screen.width / 2f;
        float width = rectTransform.rect.width;
        Vector2 startPosition = new Vector2(-halfScreenWidth - width / 2, -1152);
        Vector2 endPosition = new Vector2(halfScreenWidth+300 + width / 2, -1152);

        // 设置初始位置
        rectTransform.anchoredPosition = startPosition;

        // 飞行时间和飘动幅度
        float flightDuration = 15.0f; // 飞行总时间
        float timer = 0.0f; // 计时器
        float waveFrequency = 10f; // 振动频率
        float waveAmplitude = 50.0f; // 振动幅度

        while (timer < flightDuration)
        {
            // 计算当前进度 (0 到 1)
            float progress = timer / flightDuration;

            // 平滑插值计算位置
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, progress);

            // 添加飘动效果
            float waveOffset = Mathf.Sin(progress * waveFrequency * Mathf.PI) * waveAmplitude;
            rectTransform.anchoredPosition += new Vector2(0, waveOffset);

            // 增加计时器
            timer += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保最后位置正确
        rectTransform.anchoredPosition = endPosition;

        // 动画结束后可以选择隐藏宝箱或执行其他逻辑
         gameObject.SetActive(false);
    }

    public void ResetBubblePosition()
    {
        // 设置初始位置（底部随机位置）
        // x: -halfScreenWidth 到 halfScreenWidth
        // y: -halfScreenHeight (底部)
        float randomX = UnityEngine.Random.Range(-halfScreenWidth + bubbleWidth/2, halfScreenWidth - bubbleWidth/2);
        rectTransform.anchoredPosition = new Vector2(randomX, -halfScreenHeight - bubbleHeight);

        // 设置初始移动方向（随机斜向上）
        float randomAngle = 140;
        moveDirection = new Vector2(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        ).normalized;

        isMoving = true;
        gameObject.SetActive(true);

        int n=UnityEngine.Random.Range(0,ConfigModule.Instance.Tables.TbBubbleCanGetReward.DataList.Count);
        this.confBubbleCanGetReward = ConfigModule.Instance.Tables.TbBubbleCanGetReward.DataMap[n+1];
        this.confItem= ConfigModule.Instance.Tables.TbItem.DataMap[confBubbleCanGetReward.ItemID];
        transform.Find("gift").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite(confItem.UIIcon);
         transform.Find("gift").GetComponent<Image>().SetNativeSize();
    }
    public void ButtonOnClikHandle()
    {
        gameObject.SetActive(false);
        UIManager.Instance.ShowUI<PopGiftGetCoin>();

    }
}

