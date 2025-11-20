using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using DafultScript;
namespace DafultScript
{
    public class EmptyTip : MonoBehaviour, IPointerClickHandler
    {
        [Header("动画参数")]
        [SerializeField] private float pulseScale = 1.3f; // 脉冲缩放比例
        [SerializeField] private float pulseDuration = 0.7f; // 脉冲周期
        [SerializeField] private float bounceHeight = 0.3f; // 弹跳高度
        [SerializeField] private float bounceDuration = 1.2f; // 弹跳周期

        private Vector3 originalScale;
        private Vector3 originalPosition;
        private bool isAnimating = false;
        private Sequence animationSequence;

        public void Init()
        {
            gameObject.AddComponent<BoxCollider2D>();
            EventManager.Instance.RegisterEvent<int>(GameEvent.ShowHoleTip, ShowHoleTip);

            // 保存原始变换
            originalScale = transform.localScale;
            originalPosition = transform.localPosition;
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnregisterEvent<int>(GameEvent.ShowHoleTip, ShowHoleTip);
            // 停止动画
            if (animationSequence != null)
            {
                animationSequence.Kill();
            }
        }

        private void ShowHoleTip(int obj)
        {
            if (obj == 0)
            {
                gameObject.SetActive(true);
                transform.localPosition = new Vector3(3.84f, 7.69f, 0);
                StartTipAnimation();
            }
            else if (obj == 1)
            {
                gameObject.SetActive(true);
                transform.localPosition = new Vector3(4.4f, 7.7f, 0);
                StartTipAnimation();
            }
            else if (obj == 2)
            {
                gameObject.SetActive(false);
                StopTipAnimation();
            }
        }

        /// <summary>
        /// 开始提示动画
        /// </summary>
        private void StartTipAnimation()
        {
            if (isAnimating) return;

            isAnimating = true;
            originalPosition = transform.localPosition;

            // 停止之前的动画
            if (animationSequence != null)
            {
                animationSequence.Kill();
            }

            // 创建组合动画：脉冲缩放 + 上下弹跳
            animationSequence = DOTween.Sequence();

            // 脉冲缩放动画
            animationSequence.Append(transform.DOScale(originalScale * pulseScale, pulseDuration * 0.8f)
                .SetEase(Ease.InOutQuad));
            animationSequence.Append(transform.DOScale(originalScale, pulseDuration * 0.8f)
                .SetEase(Ease.InOutQuad));

            //// 上下弹跳动画
            //animationSequence.Join(transform.DOLocalMoveY(originalPosition.y + bounceHeight, bounceDuration * 0.5f)
            //    .SetEase(Ease.OutQuad));
            //animationSequence.Append(transform.DOLocalMoveY(originalPosition.y, bounceDuration * 0.5f)
            //    .SetEase(Ease.InQuad));

            // 循环播放
            animationSequence.SetLoops(-1, LoopType.Restart);
        }

        /// <summary>
        /// 停止提示动画
        /// </summary>
        private void StopTipAnimation()
        {
            isAnimating = false;

            if (animationSequence != null)
            {
                animationSequence.Kill();
            }

            // 恢复原始状态
            transform.localScale = originalScale;
            transform.localPosition = originalPosition;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UIManager.Instance.ShowUI<ExtraHole>();
        }
    }
}