using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestEffect : MonoBehaviour
{
    private GameObject openObj;
    private GameObject closeObj;

    private bool isInit = false;
    private void Awake()
    {
        if (isInit)
            return;

        openObj = transform.Find("Chest/Open").gameObject;
        closeObj = transform.Find("Chest/Close").gameObject;
    }


    public void SetChest(bool val)
    {
        if (openObj == null)
        {
            openObj = transform.Find("Chest/Open").gameObject;
            closeObj = transform.Find("Chest/Close").gameObject;
        }
        openObj.SetActive(val);
        closeObj.SetActive(!val);
    }

    //初始化宝箱
    public void InitChestSprite(Sprite openSprite, Sprite closeSprite)
    {
        if (openObj == null)
        {
            openObj = transform.Find("Chest/Open").gameObject;
            closeObj = transform.Find("Chest/Close").gameObject;
            isInit = true;
        }
        openObj.GetComponent<SpriteRenderer>().sprite = openSprite;
        closeObj.GetComponent<SpriteRenderer>().sprite = closeSprite;
    }
}
