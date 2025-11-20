using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DafultScript
{
    public class PiggyCountEffect : MonoBehaviour
    {
        Text languageText;

        internal void Init(float v)
        {
            gameObject.SetActive(true);
            languageText = GetComponent<Text>();

            // 格式化金钱显示
            if (v > 0)
            {
                languageText.text = "+" + GameTool.GetDollarIconAndNum(v);
            }
            else
            {
                languageText.text = "-" + GameTool.GetDollarIconAndNum(Mathf.Abs(v));
            }

            // 设置初始颜色
            Color startColor = languageText.color;
            startColor.a = 1f;
            languageText.color = startColor;

            // 获取RectTransform组件
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 startPos = rectTransform.anchoredPosition;
            Vector2 targetPos = startPos + new Vector2(0, 150f); // 向上移动100像素

            // 创建动画序列
            DG.Tweening.Sequence seq = DOTween.Sequence();

            // 同时进行移动和透明度变化
            seq.Append(rectTransform.DOAnchorPos(targetPos, 2f).SetEase(Ease.OutQuad));
            seq.Join(languageText.DOFade(0f, 2f).SetEase(Ease.InQuad));

            // 动画完成后销毁物体
            seq.OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}
