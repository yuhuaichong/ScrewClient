using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private SpriteRenderer srBehind;
    private SpriteRenderer sr;

    [SerializeField]private bool isClose;//是否关门
    public bool IsClose
    {
        get => isClose;
    }
    public void SetLayer(string name, int order)
    {
        srBehind.sortingLayerName = name;
        sr.sortingLayerName = name;

        srBehind.sortingOrder = order - 1;
        sr.sortingOrder = order + 1;
    }
    public void SetClose(bool val)
    {
        isClose = val;
        InitDoor();
    }
    public void OperateDoor()
    {
        isClose = !isClose;
        SetSpirte();
    }
    private void InitDoor()
    {
        srBehind = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sr = transform.GetChild(1).GetComponent<SpriteRenderer>();
        gameObject.SetActive(true);

        SetSpirte();
    }

    private void SetSpirte()
    {
        if (isClose)
            sr.gameObject.SetActive(true);
        else
            sr.gameObject.SetActive(false);
    }
    public void CloseDoor()
    {
        srBehind.gameObject.SetActive(false);
        sr.gameObject.SetActive(false);
        Destroy(gameObject);
    }


}
