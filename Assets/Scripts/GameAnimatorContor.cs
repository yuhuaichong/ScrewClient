using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class GameAnimatorContor : MonoBehaviour
    {
        public static GameAnimatorContor Instance;
        public Transform CoinIcon;
        public Transform DollarIcon;

        internal void ShowACoin(Image iconCoinAll)
        {
            // 1. 获取UI元素的屏幕位置
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, iconCoinAll.transform.position);

            // 2. 将屏幕坐标转换为世界坐标
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0));
            worldPoint.z = 0; // 确保在2D平面上

            // 3. 在世界坐标位置生成CoinIcon预制体
            GameObject coinSprite = Instantiate(CoinIcon.gameObject, worldPoint, Quaternion.identity);
        }

        private void Awake()
        {
            Instance = this;
            CoinIcon = transform.Find("CoinIcon");
            DollarIcon = transform.Find("DollarIcon");
            DollarIcon.GetComponent<SpriteRenderer>().sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_{GameTool.dollarIconPath}");
        }
        private void Start()
        {

        }
    }
}
