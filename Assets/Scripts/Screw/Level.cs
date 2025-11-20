using System;
using System.Collections.Generic;
using UnityEngine;
namespace DafultScript
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private List<Layer> layerList = new List<Layer>();

        [SerializeField] private bool hasIceCovered;
        public bool HasIceCovered
        {
            get => hasIceCovered;
        }
        [SerializeField] private bool hasDoor;
        public bool HasDoor { get => hasDoor; }
        [SerializeField] private bool hasBoom;
        public bool HasBoom { get => hasBoom; }

        [SerializeField] private bool hasChain;
        public bool HasChain { get => hasChain; }
        [SerializeField] private bool hasKey;
        public bool HasKey { get => hasKey; }
        [SerializeField] private bool hasLock;
        public bool HasLock { get => hasLock; }

        [Header("倒计时")]
        [SerializeField] private bool hasClock;
        public bool HasClock { get => hasClock; }
        [SerializeField] private int minutes;
        [SerializeField] private int seconds;

        [Header("是否是困难")]
        [SerializeField] private bool isHard;
        public bool IsHard { get => isHard; }
        public int GetMinutes { get => minutes; }
        public int GetSeconds { get => seconds; }

        public List<Layer> LayerList
        {
            get => layerList;
        }

        private void Start()
        {
            InitLayerList();
        }

        public int InitLayerList()
        {
            layerList.Clear();
            int layer = 24;
            foreach (Transform tran in transform)
            {
                Layer curLayer = tran.GetComponent<Layer>();

                if (curLayer != null /*&& !curLayer.isNoGlass*/ && curLayer.transform.childCount > 0)
                {
                    //初始化layer
                    curLayer.InitGlassList();
                    curLayer.SetLayer(layer--);//设置层级
                    if (layer < 3)
                    {
                        layer = 3;
                    }
                    layerList.Add(tran.GetComponent<Layer>());

                    if (curLayer.HasIceCoverd && hasIceCovered == false)
                        hasIceCovered = true;

                    if (curLayer.HasDoor && hasDoor == false)
                        hasDoor = true;

                    if (curLayer.HasBoom && hasBoom == false)
                        hasBoom = true;

                    if (curLayer.HasChain && hasChain == false)
                        hasChain = true;

                    if (curLayer.HasKey && hasKey == false)
                        hasKey = true;

                    if (curLayer.HasLock && hasLock == false)
                        hasLock = true;
                }
            }
            return layerList.Count;
        }

        internal void ShowNewScrewGuite(int v)
        {


            foreach (Layer item in LayerList)
            {
                foreach (Glass item1 in item.GlassList)
                {
                    foreach (Screw item2 in item1.ScrewList)
                    {
                        item2.hasLock = true;
                    }
                }
            }

            // Debug.LogError(v + "下标:::" + LayerList[0].GlassList[0].ScrewList.Count + "一共有几个");
            //Screw screw = LayerList[0].GlassList[v-1].ScrewList[0];
            Screw screw = LayerList[0].GlassList[0].ScrewList[0];
            screw.hasLock = false;
            EventManager.Instance.TriggerEvent<Screw, int>(GameEvent.ShowPlayerOneLevelGuite, screw, v);
        }

        internal void LockAllScrew()
        {
            foreach (Layer item in LayerList)
            {
                foreach (Glass item1 in item.GlassList)
                {
                    foreach (Screw item2 in item1.ScrewList)
                    {
                        item2.hasLock = false;
                    }
                }
            }
        }

        internal void SetEndLayer()
        {
            foreach (Transform tran in transform)
            {
                Layer curLayer = tran.GetComponent<Layer>();
                if (curLayer != null)
                    curLayer.SetEndLayer();
            }
        }
    }
}
