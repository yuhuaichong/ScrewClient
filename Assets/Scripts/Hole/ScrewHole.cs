using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DafultScript
{
    public class ScrewHole : MonoBehaviour
    {
        private Screw screw;
        private SpriteRenderer sr;
        private Collider2D cd;
        public Screw HoleScrew
        {
            get => screw;
        }

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            cd = GetComponent<Collider2D>();

        }
        public void InitScrewHole()
        {
            foreach (Transform trans in transform)
            {
                if (trans.GetComponent<Screw>())
                {
                    screw = trans.GetComponent<Screw>();
                    return;
                }
            }
        }

        public void SetHoleFlase()
        {
            if (cd != null)
            {
                cd.isTrigger = true;
                cd.enabled = false;
            }
        }

        public void RocketExplotion()
        {
            if (cd != null)
            {
                sr.enabled = false;
                cd.isTrigger = true;
                cd.enabled = false;
            }
        }

        internal void SetLayer(int v)
        {
            string layerName = $"Glass{v}";
            if (transform.Find("Screw") != null)
            {
                transform.Find("Screw").gameObject.layer = LayerMask.NameToLayer(layerName);
            }
            if (transform.Find("Screw/Image") != null)
            {
                transform.Find("Screw/Image").GetComponent<SpriteRenderer>().sortingLayerName = layerName;
            }
            if (transform.Find("Screw/shadow") != null)
            {
                transform.Find("Screw/shadow").GetComponent<SpriteRenderer>().sortingLayerName = $"Glass{v - 1}";
            }


            screw.SetSpineSort();
            if (transform.Find("Mask") != null)
            {
                SpriteMask spriteMask = transform.Find("Mask").GetComponent<SpriteMask>();
                spriteMask.frontSortingLayerID = SortingLayer.NameToID(layerName); // 使用 Sorting Layer 名称
                spriteMask.backSortingLayerID = SortingLayer.NameToID($"Glass{v - 1}"); // 使用 Sorting Layer 名称
            }

        }

        internal void SetLayer()
        {
            string layerName = $"End";
            if (transform.Find("Screw") != null)
            {
                //  transform.Find("Screw").gameObject.layer = LayerMask.NameToLayer(layerName);
            }
            if (transform.Find("Screw/Image") != null)
            {
                transform.Find("Screw/Image").GetComponent<SpriteRenderer>().sortingLayerName = layerName;
                transform.Find("Screw/Image").GetComponent<SpriteRenderer>().sortingOrder = 20;
            }
            if (transform.Find("Screw/shadow") != null)
            {
                transform.Find("Screw/shadow").GetComponent<SpriteRenderer>().sortingLayerName = $"Glass24";
            }


            screw.SetSpineSort();
            if (transform.Find("Mask") != null)
            {
                SpriteMask spriteMask = transform.Find("Mask").GetComponent<SpriteMask>();
                spriteMask.frontSortingLayerID = SortingLayer.NameToID(layerName); // 使用 Sorting Layer 名称
                spriteMask.backSortingLayerID = SortingLayer.NameToID($"Glass24"); // 使用 Sorting Layer 名称
            }
        }
    }
}
