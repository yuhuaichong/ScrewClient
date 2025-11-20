using UnityEngine;
using Spine.Unity;
using Spine;
using System;
public class SpineTool 
{
        public SkeletonAnimation skeletonAnimation;
    public void Init(SkeletonAnimation  skeletonAnimation){
        this.skeletonAnimation=skeletonAnimation;
    }
    public void PlayAnimation(string animationName,bool isLoop,float timeScale){
        TrackEntry track = skeletonAnimation.AnimationState.SetAnimation(0, animationName, isLoop);
        skeletonAnimation.timeScale = timeScale;
        track.Reverse = false;
    } 
    // 添加倒放动画的方法
    public void PlayAnimationReverse(string animationName, bool isLoop = false)
    {
        TrackEntry track = skeletonAnimation.AnimationState.SetAnimation(0, animationName, isLoop);
        track.Reverse = true;
    }

    internal void SetScrewColor(ScrewColor color)
    {
        SkeletonDataAsset skeletonDataAsset = ResourceLoader.Instance.GetScrewSkeleData(color);
        // 给SkeletonAnimation组件换上新的SkeletonDataAsset
        skeletonAnimation.skeletonDataAsset = skeletonDataAsset;

        // 刷新SkeletonAnimation，使改变生效
        skeletonAnimation.Initialize(true);
    }

    internal void SetSortingLayerAndOrderLayer(int sortingLayerID, int sortingOrder)
    {
        if (skeletonAnimation != null)
        {
            var meshRenderer = skeletonAnimation.GetComponent<Renderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingLayerID = sortingLayerID;
                meshRenderer.sortingOrder = sortingOrder;
            }
        }
    }

    internal void SetSortingLayerAndOrderLayer(string sortingLayerName,int sortingOrder=-1)
    {
        if (skeletonAnimation != null)
        {
            var meshRenderer = skeletonAnimation.GetComponent<Renderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sortingLayerName = sortingLayerName;
                if (sortingOrder != -1)
                {
                    meshRenderer.sortingOrder = sortingOrder;
                }
            }
        }
    }
}
