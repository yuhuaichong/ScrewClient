using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static DafultScript.RecyclingListView;

namespace DafultScript
{
    public class RankData
    {
        public string name;
        public int level;
        public float RewardNum;
    }
    public class PopRankPlane : MainBaseUI
    {
        Button Close;
        //GameObject RankItme;
        //Transform content;
        bool isShowData;
        public RecyclingListView scrollList;
        List<RankData> ranks;

        protected override void Awake()
        {
            base.Awake();
            Close = tableTransform.Find("Close").GetComponent<Button>();
            Close.onClick.AddListener(CloseOnClikHandle);
            //RankItme = tableTransform.Find("RankItme").gameObject;
            //RankItme.gameObject.SetActive(false);
            //content = tableTransform.Find("Scroll View/Viewport/Content");
            //无限滑动列表
            scrollList = tableTransform.Find("Scroll View").GetComponent<RecyclingListView>();
            scrollList.ChildObj = scrollList.transform.Find("Viewport/Content/RankItme").GetComponent<RankItme>();
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, effectType);
            if (isShowData)
            {
                scrollList.ScrollToRow(0, ScrollPosType.Top);
            }
            else
            {
                isShowData = true;
                // 列表item更新回调
                scrollList.ItemCallback = PopulateItem;
                CreateList();
            }
        }
        public void CreateList()
        {
            ranks = GameTool.GetRankData();

            // 设置数据，此时列表会执行更新
            scrollList.RowCount = ranks.Count;
            //for (int i = 0; i < ranks.Count; i++)
            //{
            //    GameObject go = Instantiate(RankItme, content);
            //    go.SetActive(true);
            //    int k = i;
            //    go.GetOrAddComponent<RankItme>().Init(k, ranks[k]);
            //}
        }
        private void PopulateItem(RecyclingListViewItem item, int rowIndex)
        {
            RankItme rankitem = item as RankItme;
            rankitem.Init(rowIndex, ranks[rowIndex]);
        }

        private void CloseOnClikHandle()
        {
            UIManager.Instance.HideUI<PopRankPlane>();
        }
    }
}
