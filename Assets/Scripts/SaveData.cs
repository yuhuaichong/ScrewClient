
public class SaveData
{
    // 静态实例
    private static SaveData _instance;

    // 单例访问器
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SaveData();
            }
            return _instance;
        }
    }

    // 私有构造函数，防止外部实例化
    private SaveData() 
    {
        HoleLocked = true;
        RocketLocked = true;
        DoubleBoxLocked = true;
    }

    // 属性
    public int PiggyCount { get; private set; }
    public int CoinCount { get; private set; }
    public int HoleItem { get; private set; }
    public int StarCount { get; private set; }

    public int RocketItem { get; private set; }
    public int DoubleBoxItem { get; private set; }

    public bool HoleLocked { get; private set; }
    public bool RocketLocked { get; private set; }
    public bool DoubleBoxLocked { get; private set; }
    // 方法
    public void AddPiggyCoinCount()
    {
        CoinCount += 10;
        PiggyCount += 30;
    }

    public void SetHolItemCount(int val)
    {
        HoleItem += val;
    }



    public void SetStarCount(int val)
    {
        StarCount += val;
    }

    public void SetRocketCount(int val)
    {
        RocketItem += val;
    }

}
