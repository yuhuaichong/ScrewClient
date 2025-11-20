using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScrewColor
{ 
    Red, Blue, Pink, Yellow, Purple, Orange, Green, Grey, LightBlue, LightPurple
}

public class Hole : MonoBehaviour
{
    [SerializeField] protected ScrewColor color;
    
    protected bool hasScrew;
    public Screw HoleScrew { get; set; }
    public bool HasScrew
    {
        get
        {
            return hasScrew;
        }
        set
        {
            hasScrew = value;
        }
    }
    public void InitColor(ScrewColor _color)
    {
        color = _color;
    }

    protected void Start()
    {
        hasScrew = false;
    }
}
