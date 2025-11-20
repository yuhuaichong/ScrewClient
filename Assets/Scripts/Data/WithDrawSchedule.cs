using System;
using UnityEngine;

public enum WithDrawScheduleState
{
    已完成,
    进行中,
    未进行,
}
public class WithDrawSchedule
{
    public int id;
    public float withDrawMoney;
    public DateTime withDrawTime;
    public WithDrawScheduleState nowSatate;

}
