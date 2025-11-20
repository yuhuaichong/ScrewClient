using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class AlertUI : BaseUI
    {
        private Text alertText;
        private bool isShow = false;
        GameObject TextPar;
        protected override void Awake()
        {
            base.Awake();
            alertText = tableTransform.Find("TextPar/Text").GetComponent<Text>();
            TextPar = tableTransform.Find("TextPar").gameObject;
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            //if (isShow == false)
            //{
            //    isShow = true;
            //    base.ShowUI(callback, UIEffectType.Scale);
            //    Invoke(nameof(DelayHideAlert), 1f);
            //}
        }
        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            //base.HideUI(delaytime, () =>
            //{
            //    isShow = false;
            //}, UIEffectType.Scale);
        }

        public void SetAlertText(string s)
        {
            //alertText.text = s;
            CreatNewTip(s);
        }

        private void CreatNewTip(string s)
        {
            GameObject go = Instantiate(TextPar, tableTransform);
            go.SetActive(true);
            go.transform.Find("Text").GetComponent<LanguageText>().text = s;
            RectTransform rect = go.GetComponent<RectTransform>();
            float currentValue = 0f;
            // 从 0 动画到 300，持续 5 秒
            DOTween.To(() => currentValue, x => currentValue = x, 200, 1.5f).OnUpdate(() =>
            {
                rect.anchoredPosition = new Vector2(0, currentValue);
            });
            Destroy(go, 1.5f);
        }

        private void DelayHideAlert()
        {
            UIManager.Instance.HideUI<AlertUI>();
        }
    }
}
