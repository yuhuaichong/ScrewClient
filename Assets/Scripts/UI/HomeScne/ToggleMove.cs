using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // 需要使用 DOTween 插件
namespace DafultScript
{
    public class ToggleMove : MonoBehaviour
    {
        [Header("移动距离和时间")]
        private float moveDistance = 0f; // 向右移动的距离
        private float moveDuration = 0f; // 移动的时间

        private bool isMoved = false; // 标记物体是否已经移动
        private bool isMoving = false;

        private Transform child;
        private RectTransform rectTrans;
        private void Start()
        {
            // 为按钮添加点击事件监听
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    AudioManager.Instance.PlaySFX("Click");
                    TogglePosition();
                });
            }

            rectTrans = GetComponent<RectTransform>();
            child = transform.GetChild(0);
        }

        private void TogglePosition()
        {
            if (isMoved)
            {
                isMoved = false;
                child.gameObject.SetActive(false);
            }
            else
            {
                isMoved = true;
                child.gameObject.SetActive(true);
            }

            //if (isMoving)
            //    return;

            //if (isMoved)
            //{
            //    isMoving = true;
            //    // 如果已经移动，则返回原始位置
            //    rectTrans.DOLocalMoveX(-moveDistance, moveDuration)
            //            .SetEase(Ease.InOutQuad)
            //            .OnComplete(() =>
            //            {
            //                isMoved = false;
            //                child.gameObject.SetActive(false);
            //                isMoving = false;
            //            });
            //}
            //else
            //{
            //    isMoving = true;
            //    rectTrans.DOLocalMoveX(moveDistance, moveDuration)
            //            .SetEase(Ease.InOutQuad)
            //            .OnComplete(() =>
            //            {
            //                isMoved = true;
            //                child.gameObject.SetActive(true);
            //                isMoving = false;
            //            });

            //}

        }
    }
}
