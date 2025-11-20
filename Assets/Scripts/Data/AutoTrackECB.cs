// 1.自动采集事件回调实现
using System;
using System.Collections.Generic;
using ThinkingData.Analytics;
using UnityEngine;
public class AutoTrackECB : TDAutoTrackEventHandler
{
    public Dictionary<string, object> GetAutoTrackEventProperties(int type, Dictionary<string, object> properties)
    {
        if (!GameTool.isWinIng)
        {
                EventManager.Instance.TriggerEvent(GameEvent.LoseLevel);
           //  TDAnalyticsManager.Instance.SendLoseLevel(GameTool.nowLevel, GameTool.nowProgress);
        }
        return new Dictionary<string, object>()
        {
          
        };
    }
}