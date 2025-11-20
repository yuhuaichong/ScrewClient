using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Queue<GameObject> pool;  // 存储对象的队列
    private GameObject prefab;       // 用于实例化的预制体
    private Transform parent; // 父节点，用于管理对象层级
    /// <summary>
    /// 构造函数。
    /// </summary>
    /// <param name="prefab">需要实例化的预制体。</param>
    /// <param name="initialCount">初始化对象池时的数量。</param>
    /// <param name="parent">父节点（可选）。</param>
    public ObjectPool(GameObject prefab, int initialCount, Transform parent = null)
    {
        pool = new Queue<GameObject>();
        this.prefab = prefab;
        this.parent = parent;

        // 初始化对象池
        for (int i = 0; i < initialCount; i++)
        {
            GameObject obj = Object.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// 获取对象。如果池中没有对象，则创建新对象。
    /// </summary>
    /// <returns>池中的对象。</returns>
    public GameObject GetObj()
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Object.Instantiate(prefab, parent);
        }

        return obj;
    }

    /// <summary>
    /// 回收对象，将其重新加入池中。
    /// </summary>
    /// <param name="obj">需要回收的对象。</param>
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
