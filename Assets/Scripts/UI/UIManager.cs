using System.Collections.Generic;
using UnityEngine;

namespace DafultScript
{


    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private List<BaseUI> uiInstances = new List<BaseUI>();
        [SerializeField] private Transform gamePopUpTrans;

        private BaseUI currentUI; // 当前显示的 UI
        private BaseUI previousUI; // 上一个显示的 UI

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // 查找所有继承自 BaseUI 的实例并存储到列表中
            BaseUI[] foundUIs = FindObjectsOfType<BaseUI>();
            uiInstances.AddRange(foundUIs);

            for (int i = 0; i < gamePopUpTrans.childCount; i++)
            {
                if (gamePopUpTrans.GetChild(i).GetComponent<BaseUI>())
                    uiInstances.Add(gamePopUpTrans.GetChild(i).GetComponent<BaseUI>());
            }
        }

        /// <summary>
        /// 显示指定类型的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ShowUI<T>() where T : BaseUI
        {
            foreach (var ui in uiInstances)
            {
                if (ui is T)
                {
                    // 如果当前 UI 存在，更新 previousUI
                    if (currentUI != null && currentUI != ui)
                    {
                        previousUI = currentUI;
                    }

                    // 显示目标 UI
                    currentUI = ui;
                    currentUI.gameObject.SetActive(true);
                    currentUI.ShowUI();
                    break;
                }
            }
        }

        /// <summary>
        /// 隐藏指定类型的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delayTime"></param>
        /// <param name="callback"></param>
        public void HideUI<T>(float delayTime = 0, System.Action callback = null) where T : BaseUI
        {
            foreach (var ui in uiInstances)
            {
                if (ui is T)
                {
                    ui.HideUI(delayTime, callback);
                    break;
                }
            }
        }

        /// <summary>
        /// 获取指定类型的UI实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUI<T>() where T : BaseUI
        {
            foreach (var ui in uiInstances)
            {
                if (ui is T)
                {
                    return ui as T;
                }
            }
            return null;
        }

        /// <summary>
        /// 切换到上一个 UI
        /// </summary>
        public void SwitchToPreviousUI()
        {
            if (previousUI != null)
            {
                // 隐藏当前 UI
                if (currentUI != null)
                {
                    currentUI.HideUI();
                }

                // 切换到上一个 UI
                var temp = currentUI;
                currentUI = previousUI;
                previousUI = temp;

                // 显示新的 currentUI
                currentUI.gameObject.SetActive(true);
                currentUI.ShowUI();
            }
            else
            {
                Debug.LogWarning("没有上一个 UI 可以切换！");
            }
        }
    }
}
