using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DafultScript;
public class CanvasHomeItemMove : MonoBehaviour
{
    private RectTransform Arrow;
    private RectTransform Spin;
    private Vector3 originArrowPos;
    private Vector3 originSpinPos;
    private Image canClickImage;

    private void Awake()
    {
        Arrow = transform.Find("Arrow").GetComponent<RectTransform>();
        Spin = transform.Find("Spin").GetComponent<RectTransform>();
        canClickImage = transform.Find("CanClickImage").GetComponent<Image>();
        canClickImage.enabled = false;

        originArrowPos = Arrow.position;
        originSpinPos = Spin.position;
        Arrow.localScale = Vector3.one;
    }

    public void PlayMoveAnim()
    {
        // 传入目标位置的Transform，分别为Arrow和Spin调用移动动画方法
        MoveItemWithFx(Arrow, originArrowPos, HomeSceneUI.Instance.homeUI.StreakTransfom);
        MoveItemWithFx(Spin, originSpinPos, HomeSceneUI.Instance.homeUI.LuckySpinTrans);
    }
    //移动汽油
    public void MoveArrowAnim()
    {
     
    }
    //移动抽奖票
    public void MoveSpinAnim()
    {
        MoveItemWithFx(Spin, originSpinPos, HomeSceneUI.Instance.homeUI.LuckySpinTrans);
    }
    private void MoveItemWithFx(RectTransform itemToMove, Vector3 originPos, Transform targetTrans, System.Action callback = null)
    {
        //移动时不可以点击
        canClickImage.enabled = true;

        GameObject apreaFx = ResourceLoader.Instance.GetFxGameObject("Effect Appear");

        // 获取目标位置
        Vector3 targetPos = targetTrans.position;

        itemToMove.position = originPos; // 这里可以设置物体的初始位置
        itemToMove.gameObject.SetActive(true);

        GameObject curFx = Instantiate(apreaFx);
        curFx.transform.localScale = Vector3.one * 0.6f;

        // 处理特效的UI位置
        TransFormUtility.MoveGameObjectToUIPosition(curFx.transform, itemToMove);
        curFx.GetComponent<ParticleSystem>().Play();
        Destroy(curFx, 0.8f);

        // 先放大到原来的原来的大小
        AnimationUtility.ScaleUpAndFadeIn(itemToMove, 0.5f, () =>
        {
            // 移动物体到目标位置
            AnimationUtility.MoveUIObjectToTarget(itemToMove, targetTrans, 0.5f, () =>
            {
                GameObject curFx = Instantiate(apreaFx);
                curFx.transform.localScale = Vector3.one * 0.6f;
                TransFormUtility.MoveGameObjectToUIPosition(curFx.transform, itemToMove);
                curFx.GetComponent<ParticleSystem>().Play();

                Destroy(curFx, 0.8f);
                itemToMove.gameObject.SetActive(false);

                //移动完成之后，可以点击
                callback?.Invoke();
                canClickImage.enabled = false;
            });
        });
    }
}
