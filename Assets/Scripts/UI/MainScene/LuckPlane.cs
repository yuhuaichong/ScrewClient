using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace DafultScript
{
    public class LuckPlane : MainBaseUI
    {
        Button close;
        private Transform BigPan;
        Button VideoGet;
        Button NormalGet;
        List<GiftICon> giftIcons;
        private bool isSpinning = false;
        private float radius = 300f; // 圆的半径，可以根据实际UI大小调整
        int allWeight;
        LanguageText RemingContText;
        int canGetCount;
        bool isCreatItem;
        protected override void Awake()
        {
            base.Awake();
            giftIcons = new List<GiftICon>();
            close = tableTransform.transform.Find("Close").GetComponent<Button>();
            BigPan = tableTransform.Find("BigPan");
            VideoGet = tableTransform.transform.Find("VideoGet").GetComponent<Button>();
            NormalGet = tableTransform.Find("NormalGet").GetComponent<Button>();
            RemingContText = tableTransform.Find("RemingContText").GetComponent<LanguageText>();



            VideoGet.onClick.AddListener(() =>
            {
                GameVideoContor.ShowVideoAd(VedioAdType.none, VedioAdType.none.ToString(), delegate (bool isComplete, int a)
                {
                    if (isComplete)
                    {
                        LuckToGift();
                    }
                }, null);
            });
            NormalGet.onClick.AddListener(delegate ()
            {
                // int canGetCount = GameDataManager.CurrentGameData.completeBoxNum / GameTool.collectHowManeyBoxGetOneLuck;

                if (canGetCount > 0)
                {
                    canGetCount--;
                    LuckToGift();
                }
                else
                {
                    //UIManager.Instance.ShowUI<AlertUI>();
                    //UIManager.Instance.GetUI<AlertUI>().SetAlertText("次数不足！");
                }

            });
            close.onClick.AddListener(() =>
            {
                UIManager.Instance.HideUI<LuckPlane>();
            });
        }

        private void LuckToGift()
        {
            if (isSpinning) return; // 防止重复点击
            close.gameObject.SetActive(false);
            isSpinning = true;

            // 随机选择一个奖励（0-7）

            int randomIndex = GetLuckIndex();
            //randomIndex = 3;

            // 计算需要旋转的角度
            // 基础圈数 * 360 + 目标奖励的位置角度
            float targetAngle = 360 * 5 + (randomIndex * (360f / 8) + 22.5f);
            AudioManager.Instance.PlaySFX("LcukPlane");
            // 使用DOTween创建旋转动画
            BigPan.transform.DORotate(new Vector3(0, 0, targetAngle), 6.3f, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad) // 使用OutQuart缓动效果，模拟转盘减速
                .OnComplete(() =>
                {
                    isSpinning = false;
                    Debug.Log($"恭喜抽中第{randomIndex + 1}号奖励！");
                    // 这里可以添加中奖效果和奖励发放逻辑
                    giftIcons[randomIndex].ToGetGift();
                    GameDataManager.CurrentGameData.lastGetLuckTime = DateTime.Now;
                    SetButState();
                    DOVirtual.DelayedCall(1, () =>
                    {
                        UIManager.Instance.HideUI<LuckPlane>();

                    });
                });
        }

        private int GetLuckIndex()
        {
            if (allWeight == 0)
            {
                foreach (var item in ConfigModule.Instance.Tables.TbLuckReward.DataMap)
                {
                    allWeight += (int)item.Value.Weight;
                }
            }

            int n = UnityEngine.Random.Range(0, allWeight);

            int nowWeight = 0;
            int luckIndex = 0;
            foreach (var item in ConfigModule.Instance.Tables.TbLuckReward.DataMap)
            {
                nowWeight += (int)item.Value.Weight;

                if (n > nowWeight)
                {
                    luckIndex++;
                }
                else
                {
                    break;
                }

            }
            Debug.LogError($"随机的数字是{n},获得的结果是{luckIndex}");
            return luckIndex;
        }

        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, effectType);
            canGetCount = 1;
            SetButState();
            close.gameObject.SetActive(true);
            if (isCreatItem) return;
            isCreatItem = true;
            for (int i = 1; i <= 8; i++)
            {
                Transform go = BigPan.transform.Find($"GiftICon{i}");
                GiftICon giftICon = go.GetOrAddComponent<GiftICon>();
                giftICon.Init(i);

                // 计算每个图标的角度和位置
                float angle = (i - 1) * (360f / 8) + 22.5f; // 将360度平均分成8份
                float radian = angle * Mathf.Deg2Rad; // 将角度转换为弧度

                // 计算在圆上的位置
                float x = Mathf.Sin(radian) * radius;
                float y = Mathf.Cos(radian) * radius;

                // 设置位置和旋转
                RectTransform rectTransform = go.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(x, y);
                go.localRotation = Quaternion.Euler(0, 0, -angle); // 负角度是因为Unity UI的旋转方向

                giftIcons.Add(giftICon);
            }
        }

        private void SetButState()
        {
            NormalGet.gameObject.SetActive(true);
            VideoGet.gameObject.SetActive(false);
            //int canGetCount = GameDataManager.CurrentGameData.completeBoxNum / GameTool.collectHowManeyBoxGetOneLuck;
            // RemingContText.SetTextWithParameter("剩余 {0} 次", canGetCount - GameDataManager.CurrentGameData.getLuckNum);
            // RemingContText.SetTextWithParameter("剩余 {0} 次", canGetCount);
            //if (GameDataManager.CurrentGameData.completeBoxNum/GameTool.collectHowManeyBoxGetOneLuck <= GameDataManager.CurrentGameData.getLuckNum)
            //{
            //    NormalGet.gameObject.SetActive(false);
            //    VideoGet.gameObject.SetActive(true);
            //}
            //if (GameDataManager.CurrentGameData.lastGetLuckTime != null)
            //{
            //    if (GameTool.CheckIsOneDay(GameDataManager.CurrentGameData.lastGetLuckTime, DateTime.Now))
            //    {
            //        NormalGet.gameObject.SetActive(false);
            //        VideoGet.gameObject.SetActive(true);
            //    }
            //}
        }
    }
}
