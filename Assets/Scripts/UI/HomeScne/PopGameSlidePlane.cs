using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class PopGameSlidePlane : BaseUI
    {
        Slider Slider;
        LanguageText SliveValueText;
        Button Setting;
        int nowComNum;
        Dictionary<int, int> boxChanceValue;

        int nowLevel = -1;
        bool isShowRed;
        Tween pulseTween;
        float nowComNumFloat;
        protected override void Awake()
        {
            base.Awake();
            boxChanceValue = new Dictionary<int, int>();
            Slider = tableTransform.Find("Slider").GetComponent<Slider>();
            SliveValueText = tableTransform.Find("SliveValueText").GetComponent<LanguageText>();
            Setting = tableTransform.Find("Setting").GetComponent<Button>();
            Setting.onClick.AddListener(SettingOnClikHandele);
            EventManager.Instance.RegisterEvent<int, int>(GameEvent.BocComChanceSliderValue, BocComChanceSliderValue);
            EventManager.Instance.RegisterEvent(GameEvent.SliderValueResver, SliderValueResver);
        }
        private void OnDestroy()
        {
            EventManager.Instance.UnregisterEvent<int, int>(GameEvent.BocComChanceSliderValue, BocComChanceSliderValue);
            EventManager.Instance.UnregisterEvent(GameEvent.SliderValueResver, SliderValueResver);
        }
        private void SliderValueResver()
        {
            Slider.value = 0;
            nowComNum = 0;
            SliveValueText.text = $"{Slider.value * 100}%";
            boxChanceValue.Clear();
        }

        private void BocComChanceSliderValue(int arg1, int arg2)
        {
            nowLevel = arg1;
            if (arg1 != 0)
            {
                Slider.value = arg2 * 1.0f / arg1;
                SliveValueText.text = $"{(int)(Slider.value * 100)}%";
            }
            else
            {
                nowComNum += 1;
                float value = CalculateProgress(nowComNum);
                nowComNumFloat = value;
                Slider.value = value;
                //Debug.Log(Slider.value+"现在的进度");
                Invoke(nameof(RefreshUI), 0.1f);

            }
        }
        private void RefreshUI()
        {
            float nowProgress = (CalculateProgress(nowComNum) * 100);
            if (nowProgress > 95)
            {
                if (!isShowRed)
                {
                    ShowRed();
                }
            }
            else
            {
                if (isShowRed)
                {
                    ShowWhite();
                }
            }
            //   SliveValueText.text = $"{(CalculateProgress(nowComNum) * 100)}%(实际进度：{nowComNum}%)";
            SliveValueText.text = $"{CalculateProgress(nowComNum) * 100}%";
        }
        private void ShowRed()
        {
            isShowRed = true;

            // 设置颜色为红色
            SliveValueText.color = Color.red;
            foreach (Outline item in SliveValueText.transform.GetComponents<Outline>())
            {
                item.enabled = false;
            }
            // 启动呼吸动画（持续放大缩小）
            pulseTween?.Kill();
            pulseTween = SliveValueText.transform.DOScale(1.5f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void ShowWhite()
        {
            isShowRed = false;

            // 停止动画并恢复默认大小
            pulseTween?.Kill();
            SliveValueText.transform.localScale = Vector3.one;
            foreach (Outline item in SliveValueText.transform.GetComponents<Outline>())
            {
                item.enabled = true;
            }
            // 恢复为白色
            SliveValueText.color = Color.white;
        }
        /// <summary>
        /// 计算分段式进度
        /// 前95%：每收集1个盒子增加1%
        /// 96%：需要收集5个盒子才增加1%
        /// 97%：需要收集10个盒子才增加1%
        /// 98%：需要收集15个盒子才增加1%
        /// 99%：需要收集25个盒子才增加1%
        /// 99%以后不再增加进度显示
        /// </summary>
        /// <param name="collectedBoxes">已收集的盒子数量</param>
        /// <returns>进度值(0-0.99)</returns>
        private float CalculateProgress(int collectedBoxes)
        {
            // 前95个盒子：每1个盒子增加1%
            if (collectedBoxes <= 95)
            {
                return collectedBoxes * 1.0f / 100;
            }

            // 96%：需要再收集5个盒子
            if (collectedBoxes < 100) // 95 + 5 = 100
            {
                return 0.95f;
            }

            // 97%：需要再收集10个盒子
            if (collectedBoxes < 110) // 95 + 5 + 10 = 110
            {
                return 0.96f;
            }

            // 98%：需要再收集15个盒子
            if (collectedBoxes < 125) // 95 + 5 + 10 + 15 = 125
            {
                return 0.97f;
            }

            // 99%：需要再收集25个盒子
            if (collectedBoxes < 150) // 95 + 5 + 10 + 15 + 25 = 150
            {
                return 0.98f;
            }

            // 99%以后不再增加进度显示
            return 0.99f;
        }

        public int GetSliderVlaue()
        {


            // float a = nowComNum;

            int b = nowComNum;

            // Debug.LogError("现在游戏 的进度是" + b+"  Slider.value的值为:  "+a);
            if (boxChanceValue.ContainsKey(b))
            {
                return -1;
            }
            else
            {
                boxChanceValue.Add(b, 0);
                return b;
            }
        }

        public int AwalCanGetValue()
        {
            // float a = Slider.value * 100;
            if (nowLevel < 4)
            {
                int b = (int)(Slider.value * 100);
                return b;
            }
            else
            {
                int b = nowComNum;
                return b;
            }

        }
        private void SettingOnClikHandele()
        {
            UIManager.Instance.ShowUI<SettingUI>();
        }
    }
}
