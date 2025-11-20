using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSDK : SDKBase
{
    public override void ShowVideoAd(string adId, Action<bool, int> closeCallBack, Action<int, string> errorCallBack)
    {
        closeCallBack?.Invoke(true, 0);
    }
    public override void ShowInterVideoAd(string adId, Action<bool, int> closeCallBack, Action<int, string> errorCallBack)
    {
        closeCallBack?.Invoke(true, 0);
        Time.timeScale = 1;
    }
}
