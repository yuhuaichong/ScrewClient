using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class TaskRewardUI : MainBaseUI
    {
        Image LittleCoinIcon;
        Text LittleText;
        Image BigCoinIcon;
        Text BigText;
        Button VideoDobleBut;
        Button ThankBut;
        ConfScrewTask confScrewTask;
        ConfTaskWithLevelReward confTaskWithLevelReward;
        float getReward;
        protected override void Awake()
        {
            base.Awake();
            LittleCoinIcon = tableTransform.Find("LittleCoinIcon").GetComponent<Image>();
            LittleText = tableTransform.Find("LittleCoinIcon/LittleText").GetComponent<Text>();
            BigCoinIcon = tableTransform.Find("BigCoinIcon").GetComponent<Image>();
            BigText = tableTransform.Find("BigCoinIcon/BigText").GetComponent<Text>();
            VideoDobleBut = tableTransform.Find("VideoDobleBut").GetComponent<Button>();
            ThankBut = tableTransform.Find("ThankBut").GetComponent<Button>();

            ThankBut.onClick.AddListener(() =>
            {
                GetCoin(getReward);
            });
            VideoDobleBut.onClick.AddListener(() =>
            {
                GameVideoContor.ShowVideoAd(VedioAdType.任务双倍奖励, VedioAdType.任务双倍奖励.ToString(), delegate (bool isComplete, int a)
                {
                    if (isComplete)
                    {
                        GetCoin(getReward * 2);
                    }
                }, null);
            });
        }

        private void GetCoin(float v)
        {
            //EventManager.Instance.TriggerEvent(GameEvent.GetDollar,v);
            EventManager.Instance.TriggerEvent(GameEvent.HideTaskRed);
            if (confScrewTask != null)
            {
                ConfItem confItem = ConfigModule.Instance.Tables.TbItem.GetOrDefault(1);
                GameTool.GetItem(confItem, v, VideoDobleBut.GetComponent<RectTransform>());
                EventManager.Instance.TriggerEvent(GameEvent.GetOneTask, confScrewTask.Sn);
                UIManager.Instance.HideUI<TaskRewardUI>();
            }
            else
            {
                ConfItem confItem = ConfigModule.Instance.Tables.TbItem.GetOrDefault(1);
                GameTool.GetItem(confItem, v, VideoDobleBut.GetComponent<RectTransform>());
                EventManager.Instance.TriggerEvent(GameEvent.GetOneLevelTask, confTaskWithLevelReward.Sn);
                UIManager.Instance.HideUI<TaskRewardUI>();
            }
        }

        public override void SetParm(params object[] parm)
        {
            confTaskWithLevelReward = null;
            confScrewTask = parm[0] as ConfScrewTask;
            getReward = confScrewTask.GetReward;
            //LittleText.text = GameTool.GetDollarIconAndNum(getReward);
            //BigText.text = GameTool.GetDollarIconAndNum(getReward*3);
            LittleText.text = getReward.ToString();
            BigText.text = (getReward * 2).ToString();
        }

        internal void SetLevelParm(ConfTaskWithLevelReward confTaskWithLevelReward)
        {
            confScrewTask = null;
            this.confTaskWithLevelReward = confTaskWithLevelReward;
            getReward = confTaskWithLevelReward.GetReward;
            LittleText.text = confTaskWithLevelReward.GetReward.ToString();
            BigText.text = (confTaskWithLevelReward.GetReward * 2).ToString();
        }
    }
}
