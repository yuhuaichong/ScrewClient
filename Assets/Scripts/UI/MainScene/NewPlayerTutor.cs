using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GuiteItem
{
    public string dexText;
    public float DesImageX;
    public float DesImageY;
    public float circleX;
    public float circleY;
    public float handleX;
    public float handleY;
    public bool isNoNeedRatioFenBianLv;
    public bool isNoNeedRationJuXing;
    public bool isNeedShowButton;
    public Action buttonAction;

    public int maskType;
    public float x1, y1, x2, y2;

    public bool isNeedShowClikTip;
    public float TdIndex;//数数上传参数

    public float tipTextPosX;
    public float tipTextPosY;
}
namespace DafultScript
{
    public class NewPlayerTutor : MainBaseUI
    {
        RectTransform Circle;
        RectTransform NeedShowCircle;
        RectTransform TipPar;
        LanguageText TipText;
        MaskItem maskItem;
        MaskShaderSet maskShaderSet;
        Material guiteMaterial;
        protected override void Awake()
        {
            base.Awake();
            NeedShowCircle = tableTransform.Find("NeedShowCircle").GetComponent<RectTransform>();
            TipPar = tableTransform.Find("TipPar").GetComponent<RectTransform>();
            TipText = tableTransform.Find("TipPar/TipText").GetComponent<LanguageText>();
            Circle = tableTransform.Find("Circle").GetComponent<RectTransform>();
            maskItem = bg.gameObject.AddComponent<MaskItem>();
            maskItem.Init(Circle);
            maskShaderSet = bg.gameObject.AddComponent<MaskShaderSet>();
            guiteMaterial = bg.GetComponent<Image>().material;
            maskShaderSet.Init(guiteMaterial);

            EventManager.Instance.RegisterEvent<Screw, int>(GameEvent.ShowPlayerOneLevelGuite, ShowPlayerOneLevelGuite);
            EventManager.Instance.RegisterEvent(GameEvent.ScrewGuiteIsOver, ScrewGuiteIsOver);
            EventManager.Instance.RegisterEvent<GuiteItem>(GameEvent.SetMaskRect, SetMaskRect);
        }
        private void OnDestroy()
        {
            EventManager.Instance.UnregisterEvent<Screw, int>(GameEvent.ShowPlayerOneLevelGuite, ShowPlayerOneLevelGuite);
            EventManager.Instance.UnregisterEvent(GameEvent.ScrewGuiteIsOver, ScrewGuiteIsOver);
            EventManager.Instance.UnregisterEvent<GuiteItem>(GameEvent.SetMaskRect, SetMaskRect);
        }

        private void OnEnable()
        {
            EventManager.Instance.RegisterEvent(GameEvent.HideGuitePlane, HideGuitePlane);
        }
        private void OnDisable()
        {
            EventManager.Instance.UnregisterEvent(GameEvent.HideGuitePlane, HideGuitePlane);
        }

        private void HideGuitePlane()
        {
            HideUI();
        }

        private void ScrewGuiteIsOver()
        {
            HideUI(0, delegate ()
            {
                //UIManager.Instance.ShowUI<NewGuiteComplete>();
            });
        }

        private void ShowPlayerOneLevelGuite(Screw screw, int index)
        {
            ShowUI();
            GameManager.Instance.SetGameState(GameState.Start);
            Circle.sizeDelta = new Vector2(100, 100);
            maskShaderSet.SetMaskPos(0, 0, 0, false);
            NeedShowCircle.anchoredPosition = new Vector2(5000, 0);
            if (index == 1)
            {
                TipText.text = "点击拆卸与盒子颜色匹配的螺丝";
            }
            else if (index == 2)
            {
                TipText.text = "拆卸3个同颜色螺丝进入匹配的盒子";
            }
            else if (index == 3)
            {
                TipText.text = "每集满1个盒子可以获得现金奖励";
            }

            //maskShaderSet.SetMaskPosJu(0, 0, 0, 0, Circle, guiteItem.isNoNeedRationJuXing);
            // 获取螺丝在视口中的位置（0-1范围）
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(screw.transform.position);

            // 转换到面板坐标（因为面板铺满全屏，所以可以直接使用）
            float x = (viewportPoint.x - 0.5f) * bg.rect.width;
            float y = (viewportPoint.y - 0.5f) * bg.rect.height;

            Circle.anchoredPosition = new Vector2(x, y);
            //NeedShowCircle.anchoredPosition = new Vector2(x, y);
        }

        public void SetMaskRect(GuiteItem guiteItem)
        {
            ShowUI();
            maskShaderSet.SetMaskPosJu(guiteItem.x1, guiteItem.y1, guiteItem.x2, guiteItem.y2, Circle, guiteItem.isNoNeedRationJuXing);
            NeedShowCircle.anchoredPosition = Circle.anchoredPosition;
            TipPar.anchoredPosition = new Vector2(guiteItem.DesImageX, guiteItem.DesImageY);
            TipText.text = guiteItem.dexText;
        }

        public override void SetParm(params object[] parm)
        {
            base.SetParm(parm);
        }
    }
}
