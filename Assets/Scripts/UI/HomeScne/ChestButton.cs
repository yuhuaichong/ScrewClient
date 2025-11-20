using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class ChestButton : MonoBehaviour
{
    private Transform tip;
    private Button chest;

    // 显示时长
    private float displayDuration = 3f;

    private bool isAnimating = false; // 防止重复调用
    private void Awake()
    {
        tip = transform.Find("Tip");
        chest = transform.Find("Button").GetComponent<Button>();
        chest.onClick.AddListener(ChestClick);
        tip.gameObject.SetActive(false);
    }

    private void ChestClick()
    {
        AudioManager.Instance.PlaySFX("Click");
        // 如果动画正在进行中，直接返回
        if (isAnimating)
            return;
        // 开始动画
        ShowAndHideObject();
    }

    private void ShowAndHideObject()
    {
        // 设置为可见并初始化透明度
        tip.gameObject.SetActive(true);
        isAnimating = true;

        AnimationUtility.FadeIn(tip, 0.25f, () =>
        {
            DOVirtual.DelayedCall(displayDuration, () =>
            {
                AnimationUtility.FadeOut(tip, 0.5f, () => isAnimating = false);
            });

        });

    }
}
