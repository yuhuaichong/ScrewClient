using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace DafultScript
{
    public class TaskUI : MainBaseUI
    {
        Transform taskItem;
        Transform LevellTaskItem;
        Transform Content;
        Button close;

        Toggle Left;
        Toggle right;
        int state;
        protected override void Awake()
        {
            base.Awake();
            state = 1;
            taskItem = tableTransform.Find("TaskItem");
            LevellTaskItem = tableTransform.Find("LevellTaskItem");
            close = tableTransform.Find("Close").GetComponent<Button>();
            close.onClick.AddListener(CloseOnClikHandle);
            Content = tableTransform.Find("Scroll View/Viewport/Content");
            Left = tableTransform.Find("Left").GetComponent<Toggle>();
            right = tableTransform.Find("Right").GetComponent<Toggle>();
            Left.onValueChanged.AddListener((arg) =>
            {
                if (arg)
                {
                    state = 1;
                    AudioManager.Instance.PlaySFX("Click");
                    ShowData();
                }
            });
            right.onValueChanged.AddListener((arg) =>
            {
                if (arg)
                {
                    state = 2;
                    ShowData();
                    AudioManager.Instance.PlaySFX("Click");
                }
            });
            EventManager.Instance.RegisterEvent<int>(GameEvent.GetOneTask, GetOneTask);
            EventManager.Instance.RegisterEvent<int>(GameEvent.GetOneLevelTask, GetOneLevelTask);
            EventManager.Instance.RegisterEvent(GameEvent.RefTaskUI, RefTaskUI);
            MathRed();
            if (GameTool.isNeedCloseMoneyIcon)
            {
                right.gameObject.SetActive(false);
                Left.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,530.8f);
            }
        }

        private void MathRed()
        {
            if (!GameTool.isGetNowSevertTimeSucess) return;
            if (GameDataManager.CurrentGameData.DayLoginRewardGetTime.Date != GameTool.nowSevertTime.Date)
            {
                EventManager.Instance.TriggerEvent(GameEvent.ShowTaskRed);
                return;
            }
            return;
            for (int i = 0; i < ConfigModule.Instance.Tables.TbScrewTask.DataList.Count; i++)
            {
                if (GameDataManager.CurrentGameData.taskCompleteDci.ContainsKey(ConfigModule.Instance.Tables.TbScrewTask.DataList[i].Sn))
                {
                    continue;
                }
                if (GameDataManager.CurrentGameData.completeBoxNum >= ConfigModule.Instance.Tables.TbScrewTask.DataList[i].AimTarget)
                {
                    EventManager.Instance.TriggerEvent(GameEvent.ShowTaskRed);
                    return;
                }
            }
            int count = 0;
            for (int i = 0; i < ConfigModule.Instance.Tables.TbTaskWithLevelReward.DataList.Count; i++)
            {
                if (GameDataManager.CurrentGameData.taskLevelCompleteDci.ContainsKey(ConfigModule.Instance.Tables.TbTaskWithLevelReward.DataList[i].Sn))
                {
                    continue;
                }
                if (GameDataManager.CurrentGameData.levelNum > ConfigModule.Instance.Tables.TbTaskWithLevelReward.DataList[i].Sn)
                {
                    EventManager.Instance.TriggerEvent(GameEvent.ShowTaskRed);
                    return;
                }
                count++;
                if (count >= 10)
                {
                    break;
                }
            }

        }

        private void CloseOnClikHandle()
        {
            UIManager.Instance.HideUI<TaskUI>();
        }

        private void OnDestroy()
        {
            EventManager.Instance.UnregisterEvent<int>(GameEvent.GetOneTask, GetOneTask);
            EventManager.Instance.UnregisterEvent<int>(GameEvent.GetOneLevelTask, GetOneLevelTask);
            EventManager.Instance.UnregisterEvent(GameEvent.RefTaskUI, RefTaskUI);
        }
        private void RefTaskUI()
        {
            ShowData();
        }

        private void GetOneLevelTask(int obj)
        {
            GameDataManager.CurrentGameData.taskLevelCompleteDci.Add(obj, true);
            ShowData();
        }

        /// <summary>
        /// 领取任务奖励，刷新界面
        /// </summary>
        /// <param name="obj"></param>
        private void GetOneTask(int obj)
        {
            if (GameDataManager.CurrentGameData.taskCompleteDci.ContainsKey(obj))
            {
                Debug.LogError("领取奖励出错");
            }
            else
            {
                GameDataManager.CurrentGameData.taskCompleteDci.Add(obj, true);
                for (int i = 0; i < Content.childCount; i++)
                {
                    Destroy(Content.GetChild(i).gameObject);
                }
                for (int i = 0; i < ConfigModule.Instance.Tables.TbScrewTask.DataList.Count; i++)
                {
                    if (GameDataManager.CurrentGameData.taskCompleteDci.ContainsKey(ConfigModule.Instance.Tables.TbScrewTask.DataList[i].Sn))
                    {
                        continue;
                    }
                    GameObject go = GameObject.Instantiate(taskItem.gameObject, Content);
                    go.SetActive(true);
                    go.GetOrAddComponent<TaskItem>().Init(ConfigModule.Instance.Tables.TbScrewTask.DataList[i]);
                }
            }
            MathRed();
        }

        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, effectType);

            ShowData();
        }

        private void ShowData()
        {
            if (state == 2)
            {
                ShowNumBox();
            }
            else
            {
                ShowLevelTask();
            }

            MathRed();
        }

        private void ShowLevelTask()
        {
            for (int i = 0; i < Content.childCount; i++)
            {
                Destroy(Content.GetChild(i).gameObject);
            }
            int count = 0;
            for (int i = 0; i < ConfigModule.Instance.Tables.TbDayLoginReward.DataList.Count; i++)
            {
                GameObject go = GameObject.Instantiate(taskItem.gameObject, Content);
                go.SetActive(true);
                go.GetOrAddComponent<LevellTaskItem>().Init(ConfigModule.Instance.Tables.TbDayLoginReward.DataList[i]);
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
        }

        public void ShowNumBox()
        {
            for (int i = 0; i < Content.childCount; i++)
            {
                Destroy(Content.GetChild(i).gameObject);
            }
            for (int i = 0; i < 3; i++)
            {
                int k = i;
                GameObject go = GameObject.Instantiate(taskItem.gameObject, Content);
                go.SetActive(true);
                go.GetOrAddComponent<TaskItem>().Init(k);
            }
        }
    }
}
