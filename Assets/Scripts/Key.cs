using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DafultScript;
public class Key : MonoBehaviour
{
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public void InitKeyLayer(string layername, int order)
    {
        sr.sortingLayerName = layername;
        sr.sortingOrder = order;
    }

    //移动到锁的位置
    public void MoveToLock(Lock targetLock, System.Action callback = null)
    {
        transform.parent = GameManager.Instance.transform;
        // 计算目标方向
        Vector3 direction = (targetLock.transform.position - transform.position).normalized;

        // 计算旋转角度（假设钥匙的头部朝下，Transform.down 是头部方向）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 开始移动
        transform.DOMove(targetLock.transform.position, 0.85f).SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                callback?.Invoke();
                targetLock.ShowUnLockFx();
                Destroy(gameObject, 0.01f);
            });
    }

}
