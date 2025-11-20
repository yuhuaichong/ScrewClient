using System;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using DafultScript;
namespace DafultScript
{
    public class RankItme : RecyclingListViewItem
    {
        // Image PaiMing;
        LanguageText RankLevelText;
        LanguageText RankName;
        LanguageText Level;
        Image RewType;
        Image Image;
        LanguageText RewNum;
        SkeletonGraphic PaiMingSpine;
        bool isFInd;
        internal void Init(int k, RankData rankData)
        {
            if (!isFInd)
            {
                isFInd = true;
                //PaiMing=transform.Find("PaiMing").GetComponent<Image>();
                PaiMingSpine = transform.Find("PaiMingSpine").GetComponent<SkeletonGraphic>();
                RewType = transform.Find("RewType").GetComponent<Image>();
                Image = transform.Find("RewType/Image").GetComponent<Image>();
                RankLevelText = transform.Find("RankLevelText").GetComponent<LanguageText>();
                RankName = transform.Find("RankName").GetComponent<LanguageText>();
                Level = transform.Find("Level").GetComponent<LanguageText>();
                RewNum = transform.Find("RewNum").GetComponent<LanguageText>();
            }


            if (k <= 2)
            {
                PaiMingSpine.gameObject.SetActive(true);
                //PaiMing.sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/PopRankPlane/Rank{k + 1}.png");
                string skinName = GetName(k);
                PaiMingSpine.Skeleton.SetSkin(skinName);
                PaiMingSpine.AnimationState.SetAnimation(0, "animation", true); // 第一个参数是图层索引，第二个是动画名称，第三个是是否循环
                                                                                //PaiMingSpine.Skeleton.SetSlotsToSetupPose(); // 更新槽到设置姿势
                                                                                //PaiMingSpine.Initialize(true); // 重新初始化 SkeletonGraphic
                RankLevelText.gameObject.SetActive(false);
            }
            else
            {
                PaiMingSpine.gameObject.SetActive(false);
                RankLevelText.gameObject.SetActive(true);
                RankLevelText.text = (k + 1).ToString();
            }
            RankName.text = rankData.name;
            Level.text = rankData.level.ToString();
            RewNum.text = GameTool.GetDollarIconAndNum(rankData.RewardNum);
            Image.sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_{GameTool.dollarIconPath}");
        }

        private string GetName(int k)
        {
            if (k == 0)
            {
                return "金";
            }
            else if (k == 1)
            {
                return "银";
            }
            else
            {
                return "铜";
            }
        }
    }
}
