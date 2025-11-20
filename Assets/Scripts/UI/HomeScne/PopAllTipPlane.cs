using System;
using Unity.VisualScripting;
using UnityEngine;
namespace DafultScript
{
    public class PopAllTipPlane : BaseUI
    {
        RectTransform tip1;
        RectTransform tip2;
        RectTransform WithDrawTipBg;
        Tip1 Tip1;
        Tip2 Tip2;
        protected override void Awake()
        {
            base.Awake();
            EventManager.Instance.RegisterEvent(GameEvent.ShowTip1, ShowTip1);
            EventManager.Instance.RegisterEvent(GameEvent.ShowTip2, ShowTip2);
            Tip1 = tableTransform.Find("Tip1").gameObject.AddComponent<Tip1>();
            Tip1.Init();
            Tip2 = tableTransform.Find("Tip2").gameObject.AddComponent<Tip2>();
            Tip2.Init();

            if (GameTool.isNeedCloseMoneyIcon)
            {
                transform.parent.Find("WithDrawTipBgPar").gameObject.SetActive(false);
            }
        }
        private void OnDestroy()
        {
            EventManager.Instance.UnregisterEvent(GameEvent.ShowTip1, ShowTip1);
            EventManager.Instance.UnregisterEvent(GameEvent.ShowTip2, ShowTip2);
        }

        private void ShowTip2()
        {
            if (GameTool.isNeedCloseMoneyIcon) return;
            Tip2.StartShowTip();
            EventManager.Instance.UnregisterEvent(GameEvent.ShowTip2, ShowTip2);
        }

        private void ShowTip1()
        {
            if (GameTool.isNeedCloseMoneyIcon) return;
            Tip1.ShowTip();
        }

        public void SetPos(RectTransform tip1, RectTransform tip2, RectTransform WithDrawTipBg)
        {
            this.tip1 = tip1;
            this.tip2 = tip2;
            this.WithDrawTipBg = WithDrawTipBg;
            tip1.SetParent(transform);
            tip2.SetParent(transform);
            WithDrawTipBg.SetParent(transform);
        }
    }
}
