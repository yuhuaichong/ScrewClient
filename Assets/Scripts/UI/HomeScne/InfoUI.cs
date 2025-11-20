using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    private Transform BG;
    private Transform fixWidth;
    private Button close;
    private Transform textClose;
    private void Awake()
    {
        BG = transform.Find("Bg");
        fixWidth = BG.Find("Fix Width");
        close = BG.Find("Button Close").GetComponent<Button>();
        textClose = BG.Find("Contain Text BUtton");
        close.interactable = false;
        close.onClick.AddListener(HideInfo);
    }

    public void ShowInfo()
    {
        AudioManager.Instance.PlaySFX("Click");
        for (int i = 0; i < fixWidth.childCount; i++)
        {
            Transform child = fixWidth.GetChild(i);

            // 获取或添加 CanvasGroup 组件
            CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = child.gameObject.AddComponent<CanvasGroup>();
            }

            // 设置初始透明度为 0
            canvasGroup.alpha = 0f;
        }
        textClose.GetComponent<CanvasGroup>().alpha = 0;

        // 调用父物体的渐显动画，然后在完成后逐个显现子物体
        AnimationUtility.FadeIn(BG, 0.5f, () =>
        {
            // 确保在FadeIn完成后再执行逐个显现的子物体动画
            AnimationUtility.ShowChildrenOneByOne(fixWidth, 1f, 0.25f, () =>
            {
                AnimationUtility.FadeIn(textClose, 0.1f, () => close.interactable = true);
            });
        });
    }

    public void HideInfo()
    {
        AudioManager.Instance.PlaySFX("Click");
        AnimationUtility.FadeOut(BG);
        close.interactable = false;
    }
}
