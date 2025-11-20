using System;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using System.Collections.Generic;
using System.Collections;
using Spine.Unity;

public class Tip1 : MonoBehaviour
{
    Image PayType;
    LanguageText Tip1Text1;
    LanguageText Tip1Text2;
    TbGameWithDrawTip TbGameWithDrawTip;
    ConfPayRegion TbPayRegion;
    List<ConfPayChannel> confPayChannels;
    List<string> allName;
    List<string> allSurName;
    float max;
    float min;
    public float moveDistance = 500; // 要移动的距离
    public float moveDuration = 1f; // 移动持续时间
    public float displayDuration = 4.5f; // 显示持续时间
    private RectTransform tipPopupRect;
    SkeletonGraphic skeletonGraphic;
    internal void Init()
    {
        // 获取自身的 RectTransform
        tipPopupRect = transform as RectTransform;
        TbGameWithDrawTip = ConfigModule.Instance.Tables.TbGameWithDrawTip;
        allName = new List<string>();
        allSurName = new List<string>();
        foreach (var item in TbGameWithDrawTip.DataList)
        {
            if (!string.IsNullOrEmpty(item.Name))
            {
                allName.Add(item.Name);
            }
            if (!string.IsNullOrEmpty(item.Surname))
            {
                allSurName.Add(item.Surname);
            }
        }
        //Debug.LogError(allName.Count);
        //Debug.LogError(allSurName.Count);
        PayType = transform.Find("PayType").GetComponent<Image>();
        Tip1Text1 = transform.Find("Tip1Text1").GetComponent<LanguageText>();
        Tip1Text2 = transform.Find("Tip1Text2").GetComponent<LanguageText>();
        skeletonGraphic = transform.Find("ske").GetComponent<SkeletonGraphic>();
        max = TbGameWithDrawTip.GetOrDefault(1).GetRewardMax;
        min = TbGameWithDrawTip.GetOrDefault(1).GetRewardMin;

    }
    public void ShowTip()
    {
        if (TbPayRegion == null)
        {
            TbPayRegion = GameTool.confPayRegion;
            confPayChannels = new List<ConfPayChannel>();
            string[] s = TbPayRegion.Channels.Split(',');
            foreach (var item in s)
            {
                confPayChannels.Add(ConfigModule.Instance.Tables.TbPayChannel.GetOrDefault(int.Parse(item)));
            }
        }
        int n = UnityEngine.Random.Range(0, confPayChannels.Count);
        PayType.sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/PayType/92-92/{confPayChannels[n].PicPath}_s.png");
        string name = allName[UnityEngine.Random.Range(0, allName.Count)]  + allSurName[UnityEngine.Random.Range(0, allSurName.Count)];
        Tip1Text1.SetTextWithParameter("恭喜玩家_{0}通过", GameTool.GetColorText("#FF0000",name));
        float value = UnityEngine.Random.Range(min, max);
        string money = GameTool.GetDollarIconAndNum(value);
        Tip1Text2.SetTextWithParameter("通过第4关成功提现<size=40><color=red>{0}</color></size>", money);
        skeletonGraphic.AnimationState.SetAnimation(0, "Campaignrepel1_4_6", false);
        gameObject.SetActive(true);
        // 设置弹窗初始位置
        tipPopupRect.anchoredPosition = new Vector2(0, 771f); // 初始化位置在屏幕顶部外

        gameObject.SetActive(true); // 显示弹窗

        StartCoroutine(MoveAndHideTip());
    }

    private IEnumerator MoveAndHideTip()
    {

        AudioManager.Instance.PlaySFX("top1");
        // 目标位置
        Vector2 targetPosition = new Vector2(0, 261f);

        float elapsedTime = 0f;

        // 移动弹窗
        while (elapsedTime < moveDuration)
        {
            tipPopupRect.anchoredPosition = Vector2.Lerp(tipPopupRect.anchoredPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保弹窗最终位置正确
        tipPopupRect.anchoredPosition = targetPosition;

        // 等待 3 秒
        yield return new WaitForSeconds(displayDuration);

        // 隐藏弹窗
        gameObject.SetActive(false);
    }

}
