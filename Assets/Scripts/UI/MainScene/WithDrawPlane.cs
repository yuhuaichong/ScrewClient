using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class WithDrawPlane : MainBaseUI
    {
        Button close;
        Button WithDrawBut;
        LanguageText DayData;
        LanguageText Num1;
        LanguageText Num2;
        LanguageText Num3;
        LanguageText Title;
        GameObject SliderItem;
        List<GameObject> CreatSliderItems;
        Coroutine Coroutine;

        ConfPayRegion TbPayRegion;
        List<ConfPayChannel> confPayChannels;
        List<string> allName;
        List<string> allSurName;
        TbGameWithDrawTip TbGameWithDrawTip;
        float max;
        float min;

        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, UIEffectType.Scale);

            DayData.SetTextWithParameter("关卡4通关状态 ({0})", GameTool.GetTodayDaye());
            Num1.text = $"{GameTool.GetTodayComNum()}";
            Num2.text = $"{GameTool.GetTodayAverageAttemptsNum()}";
            Num3.text = $"{GameTool.GetDollarIconAndNum(GameTool.GetTodayAverageWithDralNum())}";
            Title.SetTextWithParameter("关卡{0}", LevelManager.Instance.levelNum);
            CreatSliderItems.Clear();
            Coroutine = StartCoroutine(CreatSliderItemsIem());
            tableTransform.Find("WithDrawNum").GetComponent<LanguageText>().text = GameTool.GetDollarIconAndNum(GameDataManager.CurrentGameData.piggyCount);
            if (LevelManager.Instance.levelNum == 1)
            {
                ShowNewPlayerGuite();
            }
        }
        public void ShowNewPlayerGuite()
        {
            GuiteItem guiteItem = new GuiteItem()
            {
                dexText = "提现",
                DesImageX = 0,
                DesImageY = 10749,
                circleX = 279,
                circleY = 3000f,
                handleX = 360,
                handleY = 3000,
                isNeedShowButton = true,
                isNoNeedRationJuXing = true,
                maskType = 1,
                x1 = -350.9f,
                y1 = -228.56f,
                x2 = 339.66f,
                y2 = -34.5f,
                isNeedShowClikTip = true,
                TdIndex = 7
            };
            EventManager.Instance.TriggerEvent<GuiteItem>(GameEvent.SetMaskRect, guiteItem);
        }
        IEnumerator CreatSliderItemsIem()
        {
            while (true) // 无限循环，持续生成 SliderItems
            {
                // 复制一个 SliderItem
                GameObject newSliderItem = Instantiate(SliderItem, SliderItem.transform.parent);
                string name = "_"+allName[UnityEngine.Random.Range(0, allName.Count)]+ allSurName[UnityEngine.Random.Range(0, allSurName.Count)];
                newSliderItem.transform.Find("TextData1").GetComponent<LanguageText>().SetTextWithParameter("恭喜<color=red>玩家{0}</color>完成第4关", name);
                float value = UnityEngine.Random.Range(min, max);
                string money = GameTool.GetDollarIconAndNum(value);
                int count = GameTool.GetRandowCount();
                newSliderItem.transform.Find("TextData2").GetComponent<LanguageText>().SetTextWithParameterTwo
                    (
                    "(尝试<color=#FFEB3B>{0}</color>次)，提现<color=#FFEB3B>{1}</color>美金", count, money
                    );
                //支付图标
                if (TbPayRegion == null)
                {
                    TbPayRegion = GameTool.confPayRegion;
                    confPayChannels = new List<ConfPayChannel>();
                    string[] s = TbPayRegion.Channels.Split(',');
                    foreach (var item in s)
                    {
                        confPayChannels.Add(ConfigModule.Instance.Tables.TbPayChannel.GetOrDefault(int.Parse(item)));
                    }
                }
                int n = UnityEngine.Random.Range(0, confPayChannels.Count);
                newSliderItem.transform.Find("PayType").GetComponent<Image>().sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/PayType/92-92/{confPayChannels[n].PicPath}_s.png");

                CreatSliderItems.Add(newSliderItem); // 将新对象添加到列表中

                // 设置初始位置
                Vector3 startPosition = newSliderItem.transform.localPosition;
                newSliderItem.transform.localPosition = startPosition;

                // 向上移动的目标位置
                Vector3 targetPosition = startPosition + new Vector3(0, 300f, 0); // 100f 可以根据需要调整

                // 每个 SliderItem 的移动时间
                float moveDuration = 4f; // 1秒内移动到目标位置
                float elapsedTime = 0f;

                // 启动新的协程来处理新生成的 SliderItem 的移动
                StartCoroutine(MoveSliderItem(newSliderItem, startPosition, targetPosition, moveDuration));

                // 等待 1 秒后继续生成下一个 SliderItem
                yield return new WaitForSeconds(2f);
            }
        }
        IEnumerator MoveSliderItem(GameObject sliderItem, Vector3 startPosition, Vector3 targetPosition, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;

                sliderItem.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

                elapsedTime += Time.deltaTime;
                yield return null; // 等待下一帧
            }

            // 移动完成，确保到达目标位置
            sliderItem.transform.localPosition = targetPosition;

            // 从集合中移除并销毁该实例
            CreatSliderItems.Remove(sliderItem);
            Destroy(sliderItem);
        }
        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.HideUI(delaytime, callback, UIEffectType.Scale);
            StopAllCoroutines();
            foreach (var item in CreatSliderItems)
            {
                Destroy(item);
            }

        }
        protected override void Awake()
        {
            base.Awake();
            CreatSliderItems = new List<GameObject>();
            close = tableTransform.Find("Close").GetComponent<Button>();
            close.onClick.AddListener(CloseOnClikHandle);

            WithDrawBut = tableTransform.Find("WithDrawBut").GetComponent<Button>();
            WithDrawBut.onClick.AddListener(WithDrawButOnClikHandle);

            DayData = tableTransform.Find("Textpar/DayData").GetComponent<LanguageText>();
            Num1 = tableTransform.Find("Textpar/DataItem1/Num1").GetComponent<LanguageText>();
            Num2 = tableTransform.Find("Textpar/DataItem2/Num2").GetComponent<LanguageText>();
            Num3 = tableTransform.Find("Textpar/DataItem3/Num3").GetComponent<LanguageText>();
            Title = tableTransform.Find("Title").GetComponent<LanguageText>();
            SliderItem = tableTransform.Find("Textpar/Mask/SliderItem").gameObject;
            tableTransform.Find("BigIcon").GetComponent<Image>().sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_big_{GameTool.dollarIconPath}");


            TbGameWithDrawTip = ConfigModule.Instance.Tables.TbGameWithDrawTip;
            allName = new List<string>();
            allSurName = new List<string>();
            foreach (var item in TbGameWithDrawTip.DataList)
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    allName.Add(item.Name);
                }
                if (!string.IsNullOrEmpty(item.Surname))
                {
                    allSurName.Add(item.Surname);
                }
            }
            max = TbGameWithDrawTip.GetOrDefault(3).GetRewardMax;
            min = TbGameWithDrawTip.GetOrDefault(3).GetRewardMin;
        }

        private void WithDrawButOnClikHandle()
        {
            UIManager.Instance.HideUI<WithDrawPlane>();
            UIManager.Instance.ShowUI<PopEnterInformation>();
            if (LevelManager.Instance.levelNum == 1)
            {
                TDAnalyticsManager.Instance.SendNewUserGuide(2);
                EventManager.Instance.TriggerEvent(GameEvent.HideGuitePlane);
            }
        }

        private void CloseOnClikHandle()
        {
            UIManager.Instance.HideUI<WithDrawPlane>();
            GameTool.CheakNewPlayerGuite();
        }
    }
}
