using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DafultScript
{
    public class EmptyHoleManager : MonoBehaviour
    {
        [SerializeField] private List<EmptyHole> holeList = new List<EmptyHole>(); // 主列表
        [SerializeField] private List<EmptyHole> extraHoles = new List<EmptyHole>(); // 额外列表
        [SerializeField] private GameObject emptyHolePrefab; // 额外空槽的预制体
        [SerializeField] private ParticleSystem apearFx;
        [SerializeField] private int curScrewCount = 0; // 当前螺丝计数
        public List<Screw> propEmptyScrew = new List<Screw>();//道具导致的空螺丝
        private void Start()
        {
            // 初始化主列表
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<EmptyHole>() != null)
                {
                    holeList.Add(transform.GetChild(i).GetComponent<EmptyHole>());
                }
            }
            //Debug.LogError("设置位置成功");
            Invoke(nameof(SetPosition), 0.05f);
        }
        void SetPosition()
        {
            transform.localPosition = new Vector3(0, -1.61f, 0);
            //Debug.LogError("设置位置成功");
        }

        /// <summary>
        /// 加入到空的槽中
        /// </summary>
        public void AddToEmptyHole(Screw _screw)
        {
            // 遍历主列表和额外列表
            foreach (EmptyHole hole in GetAllHoles())
            {
                if (hole.IsEmpty())
                {

                    if (_screw.CanRemoveFromGlass())
                    {
                        hole.Screw = _screw;
                        _screw.transform.SetParent(hole.transform);
                        curScrewCount++;
                    }

                    _screw.MoveUp(hole.transform.position, delegate ()
                    {
                        AudioManager.Instance.PlaySFX("InHole");
                    });


                    //添加一个螺丝到空槽之后，判断现在剩余的孔位，如果只有一个孔位了需要显示提示
                    if (curScrewCount == GetAllHoles().Count - 1)
                    {
                        if (extraHoles.Count == 2)//额外的槽位解锁完，显示提示2
                        {
                            EventManager.Instance.TriggerEvent(GameEvent.ShowPro2);
                        }
                        else
                        {
                            EventManager.Instance.TriggerEvent(GameEvent.ShowPro1);
                        }
                    }

                    return;
                }
            }





            Debug.Log("无法加入到空槽中");
        }

        /// <summary>
        /// 从待选区选出小球放入目标箱子
        /// </summary>
        /// <param name="targetBox"></param>
        public void AddToBox(Box targetBox)
        {
            ////如果箱子没有星星洞口
            //if (targetBox.ISStarFull())
            //{
            //    SetScrewToBox(targetBox, true);
            //}
            //else
            //{
            //    //有星星洞口
            //    SetScrewToBox(targetBox, false);
            //}
            SetScrewToBox(targetBox);

        }
        /// <summary>
        /// 遍历容器将螺丝加入到箱子中（是否跳过星星螺丝）
        /// </summary>
        public void SetScrewToBox(Box targetBox, bool skipStarBalls = false)
        {
            //Debug.Log("调用空槽函数");
            foreach (EmptyHole hole in GetAllHoles())
            {
                if (!hole.IsEmpty())
                {
                    // 根据传入的参数决定是否跳过星星小球
                    if (skipStarBalls && hole.Screw.IsStar())
                        continue;

                    //目标的普通小球满了，则不需要继续添加普通螺丝了
                    if (targetBox.ISNormalFull() && !hole.Screw.IsStar())
                        continue;

                    //目标的星星螺丝满了，则不需要继续添加星星螺丝了
                    if (targetBox.ISStarFull() && hole.Screw.IsStar())
                        continue;

                    if (targetBox.BoxColor == hole.Screw.ScrewColor)
                    {
                        targetBox.SetHoleList(hole.Screw);
                        hole.Screw = null;
                        curScrewCount--;
                    }
                }
            }

            //移除一个螺丝到空槽之后，判断现在剩余的孔位，如果只有一个孔位了需要显示提示
            if (curScrewCount == GetAllHoles().Count - 1)
            {
                if (extraHoles.Count == 2)//额外的槽位解锁完，显示提示2
                {
                    EventManager.Instance.TriggerEvent(GameEvent.ShowPro2);
                }
                else
                {
                    EventManager.Instance.TriggerEvent(GameEvent.ShowPro1);
                }
            }
            else
            {
                EventManager.Instance.TriggerEvent(GameEvent.HideAllPro);
            }



            for (int i = propEmptyScrew.Count - 1; i >= 0; i--)
            {
                // 根据传入的参数决定是否跳过星星小球
                if (skipStarBalls && propEmptyScrew[i].IsStar())
                    continue;
                //目标的普通小球满了，则不需要继续添加普通螺丝了
                if (targetBox.ISNormalFull() && !propEmptyScrew[i].IsStar())
                    continue;

                //目标的星星螺丝满了，则不需要继续添加星星螺丝了
                if (targetBox.ISStarFull() && propEmptyScrew[i].IsStar())
                    continue;

                if (targetBox.BoxColor == propEmptyScrew[i].ScrewColor)
                {
                    //Debug.LogError(propEmptyScrew[i].transform.localScale);
                    propEmptyScrew[i].transform.localScale = Vector3.one * 0.44f;
                    //Debug.LogError(propEmptyScrew[i].transform.localScale+"   wqdqdw");
                    //Debug.LogError("从道具出获得螺丝");
                    targetBox.SetHoleList(propEmptyScrew[i], false);
                    propEmptyScrew.Remove(propEmptyScrew[i]);
                }
            }

            if (propEmptyScrew.Count == 0)
            {
                transform.localPosition = new Vector3(transform.position.x, -1.61f, 0);
                //Invoke(nameof(SetPosition), 0.05f);
            }
        }

        /// <summary>
        /// 检查游戏是否失败的函数
        /// </summary>
        public bool CheckGameFailed()
        {
            if (curScrewCount >= GetAllHoles().Count)
            {
                //Debug.Log("游戏失败: 空槽已经满了");
                //Debug.Log("curCount: " + curScrewCount + " listCount: " + GetAllHoles().Count);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 清空所有空槽
        /// </summary>
        public void ClearEmptyHole()
        {
            curScrewCount = 0;
            //transform.position = Vector3.zero;

            foreach (EmptyHole hole in GetAllHoles())
            {
                if (!hole.IsEmpty())
                {
                    Destroy(hole.Screw.gameObject);
                    hole.Screw = null;
                }
            }

            foreach (EmptyHole hole in extraHoles)
            {
                Destroy(hole.gameObject);
            }
            extraHoles.Clear();
            //检查空位，显示提示
            CheckShowTip();
            for (int i = propEmptyScrew.Count - 1; i >= 0; i--)
            {
                Destroy(propEmptyScrew[i].gameObject);
            }
            propEmptyScrew.Clear();
            EventManager.Instance.TriggerEvent(GameEvent.HideAllPro);
            // Invoke(nameof(SetPosition), 0.05f);
        }

        /// <summary>
        /// 激活额外的空槽Buff
        /// </summary>
        public void ActivateExtraHole()
        {
            GameObject newHole = Instantiate(emptyHolePrefab, transform);
            ParticleSystem fx = Instantiate(apearFx, transform);
            EmptyHole extraHole = newHole.GetComponent<EmptyHole>();

            // 动态设置位置（可根据需求调整）
            newHole.transform.localPosition = new Vector3(extraHoles.Count + 3, 6.2f, 0);
            fx.transform.position = newHole.transform.position;
            fx.Play();
            Destroy(fx.gameObject, 1.2f);

            extraHoles.Add(extraHole);
            //检查空位，显示提示
            CheckShowTip();
            //Debug.Log("激活额外的空槽Buff！");

            // 使用Dotween让父物体移动以保持居中
            // transform.DOLocalMoveX(-(extraHoles.Count * 0.75f), 0.35f).SetEase(Ease.OutCubic);
            EventManager.Instance.TriggerEvent(GameEvent.HideAllPro);
        }
        /// <summary>
        /// 检查空位，显示提示
        /// </summary>
        private void CheckShowTip()
        {
            EventManager.Instance.TriggerEvent(GameEvent.ShowHoleTip, extraHoles.Count);
            if (extraHoles.Count < 2)
            {
                transform.DOLocalMoveX(-((extraHoles.Count + 1) * 0.75f), 0.35f).SetEase(Ease.OutCubic);
            }
            else
            {
                transform.DOLocalMoveX(-(extraHoles.Count * 0.75f), 0.35f).SetEase(Ease.OutCubic);
            }

        }

        /// <summary>
        /// 移除额外的空槽Buff
        /// </summary>
        public void DeactivateExtraHole()
        {
            foreach (EmptyHole hole in extraHoles)
            {
                if (!hole.IsEmpty())
                {
                    Destroy(hole.Screw.gameObject); // 清除附加的螺丝
                }
                Destroy(hole.gameObject); // 清除附加的空槽
            }

            extraHoles.Clear();
            //检查空位，显示提示
            CheckShowTip();
            Debug.Log("移除额外的空槽Buff！");
        }

        /// <summary>
        /// 获取所有空槽（主列表 + 额外列表）
        /// </summary>
        private List<EmptyHole> GetAllHoles()
        {
            List<EmptyHole> allHoles = new List<EmptyHole>(holeList);
            allHoles.AddRange(extraHoles);
            return allHoles;
        }

        /// <summary>
        /// 只能加2个洞口
        /// </summary>
        /// <returns></returns>
        public bool CanAddExtraHole()
        {
            if (extraHoles.Count < 2)
                return true;

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        internal void SetEmptyHoleToPropEmpty()
        {
            StartCoroutine(MoveThis());

            //foreach (EmptyHole hole in GetAllHoles())
            //{
            //    if (!hole.IsEmpty())
            //    {
            //        propEmptyScrew.Add(hole.Screw);
            //        hole.Screw.MoveUp(hole.Screw.transform.position+new Vector3(0, 1.15f, 0));
            //        hole.Screw = null;
            //        curScrewCount--;
            //    }
            //}
        }
        IEnumerator MoveThis()
        {
            EventManager.Instance.TriggerEvent(GameEvent.HideAllPro);
            transform.localPosition = new Vector3(transform.position.x, -2f, 0);
            yield return new WaitForEndOfFrame();
            foreach (EmptyHole hole in GetAllHoles())
            {
                if (!hole.IsEmpty())
                {
                    GameObject go = Instantiate(emptyHolePrefab, transform);
                    go.transform.position = hole.Screw.transform.position + new Vector3(0, 0.95f, 0);
                    propEmptyScrew.Add(hole.Screw);
                    hole.Screw.transform.localScale = Vector3.one * 0.35f;
                    hole.Screw.MoveUp(hole.Screw.transform.position + new Vector3(0, 0.95f, 0), delegate ()
                    {
                        Destroy(go);
                    });
                    hole.Screw = null;
                    curScrewCount--;
                }
            }
        }

        internal bool HaveScrew()
        {
            foreach (EmptyHole hole in GetAllHoles())
            {
                if (!hole.IsEmpty())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
