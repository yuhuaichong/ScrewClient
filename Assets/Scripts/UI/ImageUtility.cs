using UnityEngine;
using UnityEngine.UI;

public static class ImageUtility
{
    /// <summary>
    /// 设置图片的Sprite，同时调整其大小以保持原始比例，并限制最大尺寸。
    /// </summary>
    /// <param name="image">目标Image组件。</param>
    /// <param name="sprite">要设置的Sprite。</param>
    /// <param name="maxWidth">图片的最大宽度。</param>
    /// <param name="maxHeight">图片的最大高度。</param>
    public static void SetImageSpriteWithAspect(Image image, Sprite sprite, float maxWidth, float maxHeight)
    {
        if (image == null || sprite == null)
        {
            Debug.LogWarning("Image or Sprite is null!");
            return;
        }

        // 设置图片的Sprite
        image.sprite = sprite;

        // 获取Sprite的原始尺寸
        float spriteWidth = sprite.rect.width;
        float spriteHeight = sprite.rect.height;

        // 计算原始比例
        float aspectRatio = spriteWidth / spriteHeight;

        // 计算目标尺寸，保持比例并限制在最大宽高内
        float targetWidth = spriteWidth;
        float targetHeight = spriteHeight;

        if (targetWidth > maxWidth)
        {
            targetWidth = maxWidth;
            targetHeight = targetWidth / aspectRatio;
        }

        if (targetHeight > maxHeight)
        {
            targetHeight = maxHeight;
            targetWidth = targetHeight * aspectRatio;
        }

        // 设置Image的RectTransform大小
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
    }
}
