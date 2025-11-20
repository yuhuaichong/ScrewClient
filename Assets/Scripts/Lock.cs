using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    private GameObject unlockFx;
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        unlockFx = transform.GetChild(0).gameObject;
        unlockFx.SetActive(false);
    }

    public void InitLockLayer(string layername, int order)
    {
        sr.sortingLayerName = layername;
        sr.sortingOrder = order;
    }

    /// <summary>
    /// 显示解锁特效
    /// </summary>
    public void ShowUnLockFx()
    {
        unlockFx.SetActive(true);
        Destroy(gameObject, 0.2f);
    }
}
