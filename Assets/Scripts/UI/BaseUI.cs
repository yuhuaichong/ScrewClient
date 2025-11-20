using UnityEngine;
using DG.Tweening;
namespace DafultScript
{
    public class BaseUI : MonoBehaviour
    {
        public float animationTime = 0.5f; // 动画时间
        private CanvasGroup canvasGroup;
        protected RectTransform bg;
        protected RectTransform tableTransform;

        Vector3 starPos;
        public enum UIEffectType
        {
            Slide, // 从上往下
            Scale,    // 从小到大
            Fade   //渐显
        }
        protected virtual void Awake()
        {
            bg = transform.Find("BG").GetComponent<RectTransform>();
            tableTransform = transform.Find("BG/Table").GetComponent<RectTransform>();
            starPos = tableTransform.localPosition;
            GameTool.SetOutLine(tableTransform);
            canvasGroup = tableTransform.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = tableTransform.gameObject.AddComponent<CanvasGroup>();
        }
        public virtual void SetParm(params object[] parm)
        {

        }
        public virtual void ShowUI(System.Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            //播放音效
            AudioManager.Instance.PlaySFX("Click");

            bg.gameObject.SetActive(true);
            canvasGroup.alpha = 0;

            if (effectType == UIEffectType.Slide)
            {
                // 设置初始位置为屏幕顶部
                tableTransform.localPosition = new Vector3(0, Screen.height, 0);
                // 动画：从上往下
                //tableTransform.DOLocalMoveY(0, animationTime).SetEase(Ease.OutBack);
                tableTransform.DOLocalMove(starPos, animationTime).SetEase(Ease.OutBack);
            }
            else if (effectType == UIEffectType.Scale)
            {
                // 设置初始为缩小状态
                tableTransform.localScale = Vector3.zero;
                // 动画：从小到大
                tableTransform.DOScale(Vector3.one, animationTime).SetEase(Ease.OutBack);
            }
            else if (effectType == UIEffectType.Fade)
            {
                // 设置初始位置为屏幕左边
                tableTransform.localPosition = new Vector3(Screen.width, 0, 0);
                // 动画：从左往右
                tableTransform.DOLocalMove(starPos, animationTime).SetEase(Ease.OutBack);
            }
            // 渐显
            canvasGroup.DOFade(1, animationTime).OnComplete(() => callback?.Invoke());
        }
        public virtual void HideUI(float delaytime = 0, System.Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            AudioManager.Instance.PlaySFX("Click");

            if (effectType == UIEffectType.Slide)
            {
                // 动画：从下往上
                tableTransform.DOLocalMoveY(Screen.height, animationTime).SetEase(Ease.InBack).SetDelay(delaytime);
            }
            else if (effectType == UIEffectType.Scale)
            {
                // 动画：从大到小
                tableTransform.DOScale(Vector3.zero, animationTime).SetEase(Ease.InBack).SetDelay(delaytime);
            }
            else if (effectType == UIEffectType.Fade)
            {

            }
            // 渐隐
            canvasGroup.DOFade(0, animationTime).SetDelay(delaytime).OnComplete(() =>
            {
                callback?.Invoke();
            });

            // 动画完成后禁用UI
            Invoke(nameof(DisableUI), animationTime + delaytime);
        }
        private void DisableUI()
        {
            bg.gameObject.SetActive(false);
        }
    }
}
