using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayItem : MonoBehaviour
{
    LanguageText languageText;    
   // ConfDayReward confDayReward;
    ConfItem confItem;
    ConfItem confItem2;
    int thisDay;
    public void Init(int i)
    {
        //thisDay = i;
        //languageText = transform.Find("LanguageText").GetComponent<LanguageText>();
        //this.confDayReward = ConfigModule.Instance.Tables.TbDayReward.GetOrDefault(i);
        //languageText.text = confDayReward.GetRewardNum.ToString();
        //this.confItem = ConfigModule.Instance.Tables.TbItem.GetOrDefault(confDayReward.ItemID);
        //transform.Find("Icon") .GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite(confItem.UIIcon);

        //if (confDayReward.ItemID2 != 0)
        //{
        //    this.confItem2 = ConfigModule.Instance.Tables.TbItem.GetOrDefault(confDayReward.ItemID2);
        //    transform.Find("Icon2").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite(confItem2.UIIcon);
        //    transform.Find("LanguageText2").GetComponent<LanguageText>().text= confDayReward.GetRewardNum2.ToString();
        //}
    }
    /// <summary>
    /// 领取奖励
    /// </summary>
    /// <param name="beishu">奖励的倍数</param>
    internal void GetGift(int beishu)
    {
        if (!GameDataManager.CurrentGameData.dayGiftGetStatu[thisDay])
        {
            GameDataManager.CurrentGameData.dayGiftGetStatu[thisDay] = true;
        }
        else
        {
            Debug.LogError("奖励领取过了");
        }



        //GameTool.GetItem(confItem, confDayReward.GetRewardNum* beishu, transform.Find("Icon").GetComponent<RectTransform>());
        //if (confItem2 != null)
        //{
        //    GameTool.GetItem(confItem2, confDayReward.GetRewardNum2 * beishu, transform.Find("Icon2").GetComponent<RectTransform>());
        //}
    }

    internal void SetNowState(int nowDay)
    {
        //已领取
        //今天正好领取
        //时间过了，需要看广告领取
        //正常显示，无法操作

        if (thisDay > nowDay)
        {
            //正常显示
        }
        else if (thisDay == nowDay)
        {
            if (GameDataManager.CurrentGameData.dayGiftGetStatu[thisDay])
            {
                //正常显示
            }
            else
            {
                //今天正好领取
            }
        }
        else if (thisDay < nowDay)
        {
            if (GameDataManager.CurrentGameData.dayGiftGetStatu[thisDay])
            {
                //已领取
            }
            else
            {
                //时间过了，需要看广告领取
            }
        }
    }
}
