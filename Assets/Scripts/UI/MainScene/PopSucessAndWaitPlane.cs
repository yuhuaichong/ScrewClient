using cfg;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class PopSucessAndWaitPlane : BaseUI
    {
        Button sure;
        protected override void Awake()
        {
            base.Awake();
            sure = tableTransform.Find("Sure").GetComponent<Button>();
            sure.onClick.AddListener(() =>
            {
                UIManager.Instance.HideUI<PopSucessAndWaitPlane>();
                GameTool.CheakNewPlayerGuite();
            });
        }
        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.HideUI(delaytime, callback, UIEffectType.Scale);
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, UIEffectType.Scale);
        }
    }
}
