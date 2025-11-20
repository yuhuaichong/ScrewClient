using cfg;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class PopWithDrawMethod : BaseUI
    {
        Image Icon;
        LanguageText SaveData;
        Button WithDraw;
        Button Back;
        Button Close;
        protected override void Awake()
        {
            base.Awake();
            Icon = tableTransform.Find("Title (1)/Icon").GetComponent<Image>();
            SaveData = tableTransform.Find("Title (1)/SaveData").GetComponent<LanguageText>();
            WithDraw = tableTransform.Find("Title (1)/WithDraw").GetComponent<Button>();
            Back = tableTransform.Find("Title (1)/Back").GetComponent<Button>();
            Close = tableTransform.Find("Close").GetComponent<Button>();
            WithDraw.onClick.AddListener(() =>
            {
                if (GameTool.nowLevel != 1)//第一关有其他条件判断
                {
                    if (!GameDataManager.CurrentGameData.IsCanWIthDraw || GameDataManager.CurrentGameData.piggyCount == 0)
                    {
                        GameTool.CreatTip("条件不足（输入名和电话点击提现的提示）");
                        return;
                    }
                    if (GameTool.withDrawTaskIndex == -1)
                    {
                        GameTool.CreatTip("条件不足（输入名和电话点击提现的提示）");
                        return;
                    }
                    else if (GameDataManager.CurrentGameData.taskCompleteDci.ContainsKey(GameTool.withDrawTaskIndex))
                    {
                        GameTool.CreatTip("条件不足（输入名和电话点击提现的提示）");
                        return;
                    }
                }
                if (GameTool.withDrawTaskIndex != -1)
                {
                    GameDataManager.CurrentGameData.withDrawTaskCompleteDci[GameTool.withDrawTaskIndex] = true;
                }
                CreatWithDrawSchedule();//生成提现记录
                GameDataManager.CurrentGameData.piggyCount = 0;
                GameDataManager.Save();
                EventManager.Instance.TriggerEvent(GameEvent.SetPlayerCoinText);
                UIManager.Instance.HideUI<PopWithDrawMethod>();
                UIManager.Instance.ShowUI<PopSucessAndWaitPlane>();
                TDAnalyticsManager.Instance.SendWithdraw();
            });
            Back.onClick.AddListener(() =>
            {
                GameDataManager.CurrentGameData.isComWithdrawData = false;
                UIManager.Instance.HideUI<PopWithDrawMethod>();
                UIManager.Instance.ShowUI<PopEnterInformation>();
            });
            Close.onClick.AddListener(() =>
            {
                UIManager.Instance.HideUI<PopWithDrawMethod>();
                GameTool.CheakNewPlayerGuite();
            });
        }
        private void CreatWithDrawSchedule()
        {
            GameDataManager.CurrentGameData.taskCompleteDci.Add(GameTool.withDrawTaskIndex, true);
            WithDrawSchedule withDrawSchedule = new WithDrawSchedule();
            withDrawSchedule.withDrawMoney = GameDataManager.CurrentGameData.piggyCount;
            withDrawSchedule.withDrawTime = GameTool.GetCurrentTime();
            withDrawSchedule.nowSatate = WithDrawScheduleState.进行中;
            withDrawSchedule.id = (int)(DateTime.Now.Ticks % int.MaxValue);
            GameDataManager.AddWithDrawSchedule(withDrawSchedule);//生成一个提现记录
        }
        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.HideUI(delaytime, callback, UIEffectType.Scale);
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, UIEffectType.Scale);
            if (GameDataManager.CurrentGameData.state != 3)
            {
                ConfPayChannel confPayChannel = ConfigModule.Instance.Tables.TbPayChannel.GetOrDefault(GameDataManager.CurrentGameData.payChanceSn);
                Icon.sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/PayType/530 - 335/{confPayChannel.PicPath}.png");
                if (GameDataManager.CurrentGameData.state == 1)
                {
                    SaveData.text = GameDataManager.CurrentGameData.email;
                }
                else if (GameDataManager.CurrentGameData.state == 2)
                {
                    SaveData.text = GameDataManager.CurrentGameData.eleNum;
                }
            }
            else
            {
                SaveData.text = GameDataManager.CurrentGameData.email;
                Icon.sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/PayType/530 - 335/qita.png");
            }

        }
    }
}
