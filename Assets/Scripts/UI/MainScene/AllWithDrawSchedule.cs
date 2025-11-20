using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static DafultScript.RecyclingListView;

namespace DafultScript
{

    public class AllWithDrawSchedule : MainBaseUI
    {
        Button Close;
        //GameObject RankItme;
        //Transform content;
        public RecyclingListView scrollList;

        protected override void Awake()
        {
            base.Awake();
            Close = tableTransform.Find("Close").GetComponent<Button>();
            Close.onClick.AddListener(CloseOnClikHandle);

            scrollList = tableTransform.Find("Scroll View").GetComponent<RecyclingListView>();
            scrollList.ChildObj = scrollList.transform.Find("Viewport/Content/RankItme").GetComponent<WithDrawScheduleItem>();
            scrollList.ItemCallback = PopulateItem;
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, effectType);
            scrollList.RowCount = GameDataManager.CurrentGameData.withDrawScheduleList.Count;
            scrollList.ScrollToRow(0, ScrollPosType.Top);
        }
        private void PopulateItem(RecyclingListViewItem item, int rowIndex)
        {
            WithDrawScheduleItem rankitem = item as WithDrawScheduleItem;
            rankitem.Init(rowIndex, GameDataManager.CurrentGameData.withDrawScheduleList[rowIndex]);
        }

        private void CloseOnClikHandle()
        {
            UIManager.Instance.HideUI<AllWithDrawSchedule>();
        }
    }
}
