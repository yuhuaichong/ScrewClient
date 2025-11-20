using cfg;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftICon : MonoBehaviour
{
    LanguageText languageText;
    Image icon;
    ConfLuckReward confLuckReward;
    ConfItem ConfItem;
    internal void Init(int i)
    {
        languageText = transform.Find("LanguageText").GetComponent<LanguageText>();
        icon=transform.GetComponent<Image>();
        this.confLuckReward=ConfigModule.Instance.Tables.TbLuckReward.GetOrDefault(i);
        //languageText.text = confLuckReward.GetRewardNum.ToString();
     
        this.ConfItem= ConfigModule.Instance.Tables.TbItem.GetOrDefault(confLuckReward.ItemID);
        languageText.text = confLuckReward.NewShowName;
        if (ConfItem.Sn != 2)
        {
            icon.sprite = ResourceLoader.Instance.GetUnlockImageSprite(ConfItem.LuckGetIcon);
        }
        else
        {
            icon.sprite= ResourceLoader.Instance.GetUnlockImageSprite($"coin_{GameTool.dollarIconPath}");
        }
        
        for(int j=0;j< confLuckReward.NewShowIconNum; j++)
        {
            GameObject go = Instantiate(ResourceLoader.Instance.GetResWithPath<GameObject>("Prefab/LuckImageItem.prefab"),transform);
            go.GetComponent<Image>().sprite = icon.sprite;
            go.GetComponent<Image>().SetNativeSize();
        }
        //// 使用 DOTween 延迟调用一个方法
        //DOVirtual.DelayedCall(1f, ()=>
        //{
            StartCoroutine(SetSonPos());
        //});

        icon.SetNativeSize();
    }
    IEnumerator SetSonPos()
    {
        yield return new WaitForEndOfFrame();
        if (transform.childCount == 2)
        {
            transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        }
        else if(transform.childCount == 3)
        {
           // Debug.LogError("22222qdwdwdqdw");
            transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-13, 0);
            transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(13, 0);
        }
        else if (transform.childCount == 4)
        {
            //Debug.LogError("3333333qdwdwdqdw");
            transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-20, -31);
            transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(28, -39);
            transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }
    internal void ToGetGift()
    {
        GameTool.GetItem(ConfItem, confLuckReward.GetRewardNum, icon.GetComponent<RectTransform>());
        if (ConfItem.Sn == 2)
        {
            DOVirtual.DelayedCall(0.4f, () =>
            {
                AudioManager.Instance.PlaySFX("BoxDrop");
            });
        }
    }
}
