using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameHardParameter 
{
    [Tooltip("多少进度的时候改变箱子的出现顺序")]
    public int haoScheduleChanceBox;
    [Tooltip("移动几个箱子")]
    public int moveHowManyBoxl;
    [Tooltip("移动多少距离")]
    public int haoManyChance;
}

public class GameHardChanceValue : MonoBehaviour
{
    [Header("游戏难度参数配置")]
    [SerializeField]
    public List<GameHardParameter> gameHardParameters = new List<GameHardParameter>();
    public Dictionary<int, GameHardParameter> parDic;
    public static GameHardChanceValue Instance;
    private void Awake()
    {
        Instance = this;
        parDic=new Dictionary<int, GameHardParameter>();
        foreach (var item in gameHardParameters)
        {
            parDic.Add(item.haoScheduleChanceBox, item);
        }
    }

    private void OnValidate()
    {
        // 确保列表不为空
        if (gameHardParameters == null)
        {
            gameHardParameters = new List<GameHardParameter>();
        }
    }

    // 提供一个获取参数的方法
    public GameHardParameter GetHardParameter(int index)
    {
        if (index >= 0 && index < gameHardParameters.Count)
        {
            return gameHardParameters[index];
        }
        return null;
    }
}
