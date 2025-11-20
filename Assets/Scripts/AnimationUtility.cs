using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public static class AnimationUtility
{
    private static float delayBetweenChildren = 0.08f;
    //private static float duration = 0.5f;
    /// <summary>
    /// 让一个UI元素不断地从原始位置移动到右侧指定位置
    /// </summary>
    /// <param name="target">移动的目标物体</param>
    /// <param name="offsetX">向右移动的世界坐标偏移量</param>
    /// <param name="duration">移动到目标位置的时长</param>
    public static void RepeatMoveToRight(Transform target, float offsetX = 12f, float duration = 2f)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // 记录原始位置（世界坐标）
        Vector3 originalPosition = target.position;

        // 计算目标位置（世界坐标）
        Vector3 targetPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y, originalPosition.z);

        // 循环播放移动动画：从原始位置移动到目标位置，再回到原始位置重启
        target.DOMove(targetPosition, duration)
              .SetEase(Ease.Linear)           // 线性移动
              .SetLoops(-1, LoopType.Restart) // 无限次循环，从头开始
              .OnStepComplete(() => target.position = originalPosition); // 每次循环重置位置，避免误差
    }
    /// <summary>
    /// Q弹从右往左入场动画
    /// </summary>
    public static void PlayEnterFromRight(Transform target, float startOffsetX = 20f, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // 记录目标位置
        Vector3 targetPosition = target.position;

        // 设置初始位置在右侧偏移
        target.position = new Vector3(targetPosition.x + startOffsetX, targetPosition.y, targetPosition.z);

        // 执行动画：从右侧移动到目标位置，带弹性效果
        target.DOMove(targetPosition, duration)
              .SetEase(Ease.OutBack)
              .OnStart(() => FadeIn(target, duration))
              .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// Q弹从左往右退场动画
    /// </summary>
    public static void PlayExitToRight(Transform target, float endOffsetX = 20f, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // 记录原始位置
        Vector3 originalPosition = target.position;

        // 计算目标退场位置
        Vector3 exitPosition = new Vector3(originalPosition.x + endOffsetX, originalPosition.y, originalPosition.z);

        // 执行动画：从当前位置移动到右侧，带快到慢的效果
        target.DOMove(exitPosition, duration)
              .SetEase(Ease.InQuad)
              .OnStart(() => FadeOut(target, duration))
              .OnComplete(() =>
              {
                  target.position = originalPosition;
                  onComplete?.Invoke();
              });
    }

    /// <summary>
    /// Q弹从左往右入场动画
    /// </summary>
    public static void PlayEnterFromLeft(Transform target, float startOffsetX = -20f, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // 记录目标位置
        Vector3 targetPosition = target.position;

        // 设置初始位置在左侧偏移
        target.position = new Vector3(targetPosition.x + startOffsetX, targetPosition.y, targetPosition.z);

        // 执行动画：从左侧移动到目标位置，带弹性效果
        target.DOMove(targetPosition, duration)
              .SetEase(Ease.OutBack)
              .OnStart(() => FadeIn(target, duration))
              .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// Q弹从右往左退场动画
    /// </summary>
    public static void PlayExitToLeft(Transform target, float endOffsetX = -20f, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // 记录原始位置
        Vector3 originalPosition = target.position;

        // 计算目标退场位置
        Vector3 exitPosition = new Vector3(originalPosition.x + endOffsetX, originalPosition.y, originalPosition.z);

        // 执行动画：从当前位置移动到左侧，带快到慢的效果
        target.DOMove(exitPosition, duration)
              .SetEase(Ease.InQuad)
              .OnStart(() => FadeOut(target, duration))
              .OnComplete(() =>
              {
                  target.position = originalPosition;
                  onComplete?.Invoke();
              });
    }

    /// <summary>
    /// 渐显目标物体
    /// </summary>
    public static void FadeIn(Transform target, float duration = 0.5f, Action onComplete = null)
    {
        // 获取或添加 CanvasGroup
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }
        target.gameObject.SetActive(true);
        // 设置初始透明度为 0
        canvasGroup.alpha = 0f;

        // 渐显动画
        canvasGroup.DOFade(1f, duration)
                   .SetEase(Ease.Linear)
                   .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 渐隐目标物体
    /// </summary>
    public static void FadeOut(Transform target, float duration = 0.5f, Action onComplete = null)
    {
        // 获取或添加 CanvasGroup
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        onComplete += () => target.gameObject.SetActive(false);
        // 渐隐动画
        canvasGroup.DOFade(0f, duration)
                   .SetEase(Ease.Linear)
                   .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 子物体从下往上入场，带弹跳效果
    /// </summary>
    /// <param name="parent">父物体 Transform</param>
    /// <param name="startOffsetY">初始位置的 Y 轴偏移</param>
    /// <param name="onComplete">动画完成后的回调（可选）</param>
    /// <summary>
    public static void ChildrenDownToUp(Transform parent, List<Vector3> originPosList, float startOffsetY = -20f, float duration = 0.5f, System.Action onComplete = null)
    {
        // 获取所有子物体
        int childCount = parent.childCount;
        // 创建序列
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // 记录初始位置和目标位置
            Vector3 originalPosition = originPosList[i];
            Vector3 startPosition = new Vector3(originalPosition.x, originalPosition.y + startOffsetY, originalPosition.z);

            // 设置初始位置
            child.position = startPosition;
            // 动画：从初始位置到目标位置，带弹跳效果
            sequence.Insert(
                i * delayBetweenChildren, // 设置每个子物体的延迟时间
                child.DOMove(originalPosition, duration)
                     .SetEase(Ease.OutQuad) // 缓动
                     .OnStart(() => FadeIn(child, duration)) // 动画开始时渐显
            );
        }

        // 动画完成后的回调
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 子物体从原位置向下退场
    /// </summary>
    /// <param name="parent">父物体 Transform</param>
    /// <param name="endOffsetY">目标位置的 Y 轴偏移</param>
    /// <param name="onComplete">动画完成后的回调（可选）</param>
    /// <summary>
    public static void ChildrenUpToDown(Transform parent, List<Vector3> originPosList, float endOffsetY = -20f, float duration = 0.5f, System.Action onComplete = null)
    {
        // 获取所有子物体
        int childCount = parent.childCount;
        // 创建序列
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // 记录初始位置
            Vector3 originalPosition = child.position;

            // 计算目标位置
            Vector3 endPosition = new Vector3(originalPosition.x, originalPosition.y + endOffsetY, originalPosition.z);

            // 动画：从原位置移动到目标位置，带缓动效果
            sequence.Insert(
                i * delayBetweenChildren, // 设置子物体的延迟时间
                child.DOMove(endPosition, duration)
                .OnStart(() => FadeOut(child, duration))
                     .SetEase(Ease.InQuad) // 快速向下消失
                     .OnComplete(() =>
                     {
                         // 动画完成后重置为初始位置
                         child.position = originalPosition;
                     })
            );
        }

        // 动画完成后的回调
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 逐个显现所有子物体
    /// </summary>
    /// <param name="parent">父物体</param>
    /// <param name="duration">每个子物体的渐显时长</param>
    /// <param name="delayBetweenChildren">每个子物体之间的延迟时间</param>
    /// <param name="onComplete">所有子物体动画完成后的回调</param>
    public static void ShowChildrenOneByOne(Transform parent, float duration = 0.5f, float delayBetweenChildren = 0.08f, Action onComplete = null)
    {
        int childCount = parent.childCount;

        parent.gameObject.SetActive(true);
        // 创建一个序列，用于按顺序执行每个子物体的渐显动画
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // 获取或添加 CanvasGroup 组件
            CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = child.gameObject.AddComponent<CanvasGroup>();
            }

            // 设置初始透明度为 0
            canvasGroup.alpha = 0f;

            // 依次执行每个子物体的渐显动画
            sequence.Insert(
                i * delayBetweenChildren, // 设置每个子物体之间的延迟
                canvasGroup.DOFade(1f, duration).SetEase(Ease.Linear)
            );
        }

        // 所有子物体动画完成后的回调
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 放大然后缩小的动画效果。
    /// </summary>
    /// <param name="target">需要执行动画的物体 Transform。</param>
    /// <param name="scaleMultiplier">放大的比例倍数。</param>
    /// <param name="scaleDuration">放大和缩小的动画时长。</param>
    /// <param name="onComplete">动画完成后的回调（可选）。</param>
    public static void PlayScaleBounce(Transform target, float scaleMultiplier = 1.2f, float scaleDuration = 0.3f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // 记录原始的缩放比例
        Vector3 originalScale = target.localScale;

        // 创建序列动画
        Sequence sequence = DOTween.Sequence();

        // 放大动画
        sequence.Append(target.DOScale(originalScale * scaleMultiplier, scaleDuration).SetEase(Ease.OutQuad));

        // 缩小动画
        sequence.Append(target.DOScale(originalScale, scaleDuration).SetEase(Ease.InQuad));

        // 动画完成回调
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 将物体移动到指定位置
    /// </summary>
    /// <param name="target">需要移动的物体 Transform</param>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="duration">移动的持续时间</param>
    /// <param name="onComplete">动画完成后的回调（可选）</param>
    public static void MoveToPosition(Transform target, Vector3 targetPosition, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // 使用 DOTween 进行位置动画
        target.DOMove(targetPosition, duration)
              .SetEase(Ease.InQuad)
              .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 向上移动并渐隐的动画
    /// </summary>
    /// <param name="target">要进行动画的目标 RectTransform</param>
    /// <param name="moveDistance">向上移动的距离</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="onComplete">动画完成后的回调，可选</param>
    public static void MoveUpAndFadeOut(RectTransform target, float duration = 0.8f, float moveDistance = 100, System.Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null. Cannot perform animation.");
            return;
        }
        Vector3 initialPosition = target.position;
        Vector3 initialScale = target.localScale;

        // 确保目标有 CanvasGroup，用于控制透明度
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        // 初始状态，确保完全显示
        canvasGroup.alpha = 1f;

        // 记录初始位置
        Vector3 startPosition = target.anchoredPosition;

        // 创建 DOTween 动画
        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DOAnchorPosY(startPosition.y + moveDistance, duration).SetEase(Ease.OutQuad)) // 向上移动
                .Join(canvasGroup.DOFade(0, duration).SetEase(Ease.Linear)) // 渐隐
                .OnComplete(() =>
                {
                    target.position = initialPosition;
                    target.localScale = initialScale;
                    // 动画完成后的处理
                    onComplete?.Invoke();
                });

        // 启用动画
        sequence.Play();
    }

    /// <summary>
    /// 缩小并渐隐的动画
    /// </summary>
    /// <param name="target">要进行动画的目标 Transform</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="onComplete">动画完成后的回调，可选</param>
    public static void ScaleDownAndFadeOut(Transform target, float duration = 0.8f, System.Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null. Cannot perform animation.");
            return;
        }

        // 获取初始位置和大小
        Vector3 initialPosition = target.position;
        Vector3 initialScale = target.localScale;

        // 确保目标有 CanvasGroup，用于控制透明度
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        // 初始状态，确保完全显示
        canvasGroup.alpha = 1f;

        // 创建 DOTween 动画
        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DOScale(Vector3.zero, duration).SetEase(Ease.InQuad)) // 缩小
                .Join(canvasGroup.DOFade(0, duration).SetEase(Ease.Linear)) // 渐隐
                .OnComplete(() =>
                {
                    // 动画完成后恢复到原来的位置和大小
                    target.position = initialPosition;
                    target.localScale = initialScale;

                    // 调用完成后的回调
                    onComplete?.Invoke();
                });

        // 启用动画
        sequence.Play();
    }

    /// <summary>
    /// 放大并渐显的动画（带弹跳效果）。
    /// </summary>
    /// <param name="target">要进行动画的目标 Transform</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="onComplete">动画完成后的回调，可选</param>
    public static void ScaleUpAndFadeIn(Transform target, float duration = 0.8f, System.Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null. Cannot perform animation.");
            return;
        }

        // 获取初始位置和大小
        Vector3 initialScale = target.localScale;

        // 确保目标有 CanvasGroup，用于控制透明度
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        // 初始状态，确保完全隐藏
        canvasGroup.alpha = 0f;
        target.localScale = Vector3.zero;

        // 创建 DOTween 动画
        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DOScale(initialScale, duration).SetEase(Ease.OutBounce)) // 放大（带弹跳效果）
                .Join(canvasGroup.DOFade(1, duration * 0.5f).SetEase(Ease.Linear)) // 渐显（时长为总时长的一半）
                .OnComplete(() =>
                {
                    // 确保动画完成后透明度和缩放都设置正确
                    canvasGroup.alpha = 1f;
                    target.localScale = initialScale;

                    // 调用完成后的回调
                    onComplete?.Invoke();
                });

        // 启用动画
        sequence.Play();
    }

    /// <summary>
    /// 将物体移动到指定位置，并且动态调整父级
    /// </summary>
    public static void MoveToPositionWithParentChange(Transform target, Transform newParent, Vector3 localTargetPosition, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null || newParent == null)
        {
            Debug.LogError("Target or newParent is null!");
            return;
        }

        //Vector3 oriScale = target.localScale;
        // 改变父级
        target.SetParent(newParent);

        //target.localScale = oriScale;
        // 移动到新父级下的目标位置
        target.DOLocalMove(localTargetPosition, duration)
              .SetEase(Ease.InQuad)
              .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 依次显现列表中的物体，并添加放大效果。
    /// </summary>
    /// <param name="objects">物体列表。</param>
    /// <param name="fadeDuration">每个物体的渐显时长。</param>
    /// <param name="scaleDuration">每个物体的放大时长。</param>
    /// <param name="delayBetweenObjects">物体之间的显现延迟时间。</param>
    /// <param name="onComplete">所有物体显现完后的回调。</param>
    public static void ShowObjectsOneByOneWithScale(List<GameObject> objects, float fadeDuration = 0.5f, float scaleDuration = 0.5f, float delayBetweenObjects = 0.1f, System.Action onComplete = null)
    {
        if (objects == null || objects.Count == 0)
        {
            Debug.LogError("Object list is empty or null.");
            return;
        }

        // 创建一个序列，用于控制动画
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < objects.Count; i++)
        {
            GameObject obj = objects[i];

            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();

            // 如果没有 CanvasGroup 组件，添加一个
            if (canvasGroup == null)
            {
                canvasGroup = obj.AddComponent<CanvasGroup>();
            }

            // 确保物体开始时是透明并缩小的
            canvasGroup.alpha = 0f;

            Vector3 oriScale = obj.transform.localScale;
            obj.transform.localScale = Vector3.zero; // 从缩小开始

            // 渐显和放大动画
            sequence.Insert(i * delayBetweenObjects, canvasGroup.DOFade(1f, fadeDuration));
            sequence.Insert(i * delayBetweenObjects, obj.transform.DOScale(oriScale, scaleDuration).SetEase(Ease.OutBack)); // 放大带弹性缓动
        }

        // 设置动画完成后的回调
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 将UI物体移动到另一个UI物体的位置，支持跨Canvas操作。
    /// </summary>
    /// <param name="movingObject">需要移动的物体 Transform。</param>
    /// <param name="targetObject">目标位置的物体 Transform。</param>
    /// <param name="duration">移动动画的时长。</param>
    /// <param name="onComplete">动画完成后的回调，可选。</param>
    public static void MoveUIObjectToTarget(Transform movingObject, Transform targetObject, float duration = 0.5f, System.Action onComplete = null)
    {
        if (movingObject == null || targetObject == null)
        {
            Debug.LogError("MovingObject or TargetObject is null!");
            return;
        }

        // 获取目标物体在屏幕上的坐标
        RectTransform targetRect = targetObject as RectTransform;
        RectTransform movingRect = movingObject as RectTransform;

        if (targetRect == null || movingRect == null)
        {
            Debug.LogError("Both MovingObject and TargetObject must be RectTransforms.");
            return;
        }

        Canvas targetCanvas = targetObject.GetComponentInParent<Canvas>();
        Canvas movingCanvas = movingObject.GetComponentInParent<Canvas>();

        if (targetCanvas == null || movingCanvas == null)
        {
            Debug.LogError("Both MovingObject and TargetObject must belong to a Canvas.");
            return;
        }

        // 获取目标物体的屏幕坐标
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, targetRect.position);

        // 将屏幕坐标转换为移动物体所在Canvas的局部坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            movingRect.parent as RectTransform,
            screenPosition,
            movingCanvas.worldCamera,
            out Vector2 localPosition
        );

        // 执行移动动画
        movingRect.DOLocalMove(localPosition, duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 动画化更新文本内容。
    /// </summary>
    /// <param name="text">要更新的文本组件。</param>
    /// <param name="startValue">起始数字。</param>
    /// <param name="endValue">结束数字。</param>
    /// <param name="duration">动画持续时间（秒）。</param>
    public static void AnimateText(Text text, float startValue, float endValue, float duration, Action onComplete = null)
    {
        if (text == null)
        {
            Debug.LogError("Text component is null. Please provide a valid Text component.");
            return;
        }

        float currentValue = startValue;

        DOTween.To(() => currentValue, x => currentValue = x, endValue, duration)
            .OnUpdate(() =>
            {
                text.text = $"{Mathf.RoundToInt(currentValue)}%";
            })
            .OnComplete(() => onComplete?.Invoke());
    }
}
