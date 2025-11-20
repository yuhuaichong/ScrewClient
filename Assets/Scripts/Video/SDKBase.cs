using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public   class SDKBase
{
    public virtual void Init() { }
    public virtual void ShowVideoAd(string adId, Action<bool, int> closeCallBack, Action<int, string> errorCallBack) { }//展示广告
    public virtual void ShowInterVideoAd(string adId, Action<bool, int> closeCallBack, Action<int, string> errorCallBack) { }//展示插屏广告
}
