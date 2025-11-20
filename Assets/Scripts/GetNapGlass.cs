using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
namespace DafultScript
{
    public class GetNapGlass : MonoBehaviour
    {


        [SerializeField] private GameObject napglassPrefab;
        private static GetNapGlass instance;

        private float moveTime;
        public static GetNapGlass Instance
        {
            get
            {
                if (instance == null)
                {
                    // 查找已存在的实例
                    instance = FindObjectOfType<GetNapGlass>();
                    // 如果没有找到，创建一个新的实例
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject("GetNapGlass");
                        instance = singletonObject.AddComponent<GetNapGlass>();
                    }
                }
                return instance;
            }
        }
        private GameObject curNapGlass;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject); // 确保在场景切换时不会被销毁
            }
            else
            {
                Destroy(gameObject); // 销毁多余的实例
            }
        }

        private void Start()
        {
            moveTime = 0.25f;
        }
        /// <summary>
        /// 生成盖子的函数
        /// </summary>
        public void InitNapGlassPrefab(Box _box, System.Action callback = null)
        {
            //DOVirtual.DelayedCall(moveTime, delegate () 
            //{
            //    callback?.Invoke();
            //});
            //return;//不生成这个盖了
            GameObject newNapGlass = Instantiate(napglassPrefab);
            // newNapGlass.GetComponent<SpriteRenderer>().sprite = GetFitNap(_box);
            newNapGlass.GetComponent<SpriteRenderer>().sprite = GameTool.GetBoxNap(_box);
            newNapGlass.GetComponent<SpriteRenderer>().sortingOrder = 10;

            //newNapGlass.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetFItGlass(_box);
            //newNapGlass.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 9;

            newNapGlass.transform.SetParent(_box.transform);
            newNapGlass.transform.position = new Vector3(_box.transform.position.x, _box.transform.position.y + 5.5f);

            curNapGlass = newNapGlass;
            //盖的位置与实际有偏差
            MoveToBox(new Vector3(_box.transform.position.x, _box.transform.position.y + 0.2f), callback);
        }
        private Sprite GetFItGlass(Box _box)
        {
            return null;
        }
        private Sprite GetFitNap(Box _box)
        {
            return ResourceLoader.Instance.GetBoxBoardSprite(_box.BoxColor, _box.HoleCount);
        }
        /// <summary>
        /// 向箱子移动的函数
        /// </summary>
        /// 传入箱子完成装箱的函数
        private void MoveToBox(Vector3 targetPos, System.Action callback = null)
        {
            if (curNapGlass == null) return;
            curNapGlass.transform.DOMove(targetPos, moveTime)
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }

    }
}
