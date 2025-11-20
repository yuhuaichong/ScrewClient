using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace DafultScript
{
    public class AppraisePlaneUI : BaseUI
    {
        Button Later;
        Button Start;
        public System.Action callback;
        
#if UNITY_IOS && !UNITY_EDITOR
        // 只调用系统原生评价弹窗（SKStoreReviewController）
        [DllImport("__Internal")]
        private static extern void _RequestInAppReview();
#endif

        protected override void Awake()
        {
            base.Awake();
            Later = tableTransform.Find("Later").GetComponent<Button>();
            Start = tableTransform.Find("Start").GetComponent<Button>();


            tableTransform.Find("Bg/Title").GetComponent<LanguageText>().text = "评价我们";
            tableTransform.Find("Later/LanguageText").GetComponent<LanguageText>().text = "以后";
            tableTransform.Find("Start/LanguageText").GetComponent<LanguageText>().text = "5星";
            Later.onClick.AddListener(() =>
            {
                UIManager.Instance.HideUI<AppraisePlaneUI>();
                GameDataManager.CurrentGameData.isOpenAppraisePlane = true;
                GameDataManager.Save();
                callback?.Invoke();
            });
            Start.onClick.AddListener(() =>
            {
                GameDataManager.CurrentGameData.isOpenAppraisePlane = true;
                GameDataManager.Save();
                UIManager.Instance.HideUI<AppraisePlaneUI>();
                MainSceneUI.Instance.StartCoroutine(ShowGoogleAppraise());
                callback?.Invoke();

            });
        }
        
        IEnumerator ShowGoogleAppraise()
        {
            yield return new WaitForEndOfFrame();
            
#if UNITY_IOS && !UNITY_EDITOR
            // 只使用系统原生评价弹窗（SKStoreReviewController）
            // 注意：系统可能不会显示弹窗（iOS 限制：一年最多 3 次）
            // 如果系统不显示，就不会打开任何界面
            Debug.Log("[AppStoreReview] Requesting in-app review");
            _RequestInAppReview();
#elif UNITY_EDITOR
            Debug.Log("[AppStoreReview] App Store review is only available on iOS devices");
#else
            Debug.Log("[AppStoreReview] Platform not supported");
#endif
        }
    }
}
