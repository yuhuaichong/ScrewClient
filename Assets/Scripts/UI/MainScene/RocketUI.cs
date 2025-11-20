using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class RocketUI : BaseUI
    {
        private Button buttonColse;
        protected override void Awake()
        {
            base.Awake();
            buttonColse = tableTransform.Find("Button Close").GetComponent<Button>();
            buttonColse.onClick.AddListener(CloseEvent);
        }

        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, UIEffectType.Scale);
        }

        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.HideUI(delaytime, callback, UIEffectType.Scale);
        }

        private void CloseEvent()
        {
            GameManager.Instance.SetRocketClickFalse();
            UIManager.Instance.HideUI<RocketUI>();
        }
    }
}
