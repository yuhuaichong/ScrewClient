using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DafultScript
{
    public class Layer : MonoBehaviour
    {
        [SerializeField] private List<Glass> glassList = new List<Glass>();
        public List<Glass> GlassList
        {
            get => glassList;
        }
        [SerializeField] private bool hasConnected;
        public bool HasConnected
        {
            get
            {
                return hasConnected;
            }
        }
        [SerializeField] private bool hasIceCovered;
        public bool HasIceCoverd
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
        public int allGlassCount;
        public bool isNoGlass;
        private void Awake()
        {

        }
        private void Start()
        {

        }
        public void InitGlassList()
        {
            allGlassCount = 0;
            glassList.Clear();
            foreach (Transform trans in transform)
            {
                Glass glass = trans.GetComponent<Glass>();
                if (glass != null)
                {
                    //初始化glass
                    glass.InitScrewList(this);
                    allGlassCount++;
                    glassList.Add(glass);
                    if (hasConnected == false && glass.HasConnect)
                    {
                        hasConnected = true;
                    }

                    if (hasIceCovered == false && glass.HasIceCovered)
                    {
                        hasIceCovered = true;
                    }

                    if (hasDoor == false && glass.HasDoor)
                        hasDoor = true;

                    if (glass.HasBoom && hasBoom == false)
                        hasBoom = true;

                    if (glass.HasChain && hasChain == false)
                        hasChain = true;

                    if (glass.HasKey && hasKey == false)
                        hasKey = true;

                    if (glass.HasLock && hasLock == false)
                        hasLock = true;
                }
            }
        }

        internal void SetLayer(int v)
        {
            string layerName = $"Glass{v}";
            foreach (Glass item in glassList)
            {
                item.gameObject.layer = LayerMask.NameToLayer(layerName);
                SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) continue;
                spriteRenderer.sortingLayerName = layerName;
                spriteRenderer.sortingOrder = 2;
                item.SetLayer(v);
            }
        }

        internal void RemOneGlass()
        {
            allGlassCount--;
            if (allGlassCount == 0)
            {
                isNoGlass = true;
                EventManager.Instance.TriggerEvent(GameEvent.OneLayerNoGlass, this);
            }
        }

        internal void SetEndLayer()
        {
            string layerName = $"End";
            foreach (Glass item in glassList)
            {
                //   item.gameObject.layer = LayerMask.NameToLayer(layerName);
                SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) continue;
                spriteRenderer.sortingLayerName = layerName;
                spriteRenderer.sortingOrder = 20;
                item.SetLayer();
            }
        }
    }
}
