
using UnityEngine;


public class MaskItem : MonoBehaviour, ICanvasRaycastFilter
{

    public RectTransform rect;

    public bool isray;
    public void Init(RectTransform rectTransform)
    {
        rect = rectTransform;
    }
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {

        isray = !RectTransformUtility.RectangleContainsScreenPoint(rect, sp, eventCamera);
        return isray;

    }
}
