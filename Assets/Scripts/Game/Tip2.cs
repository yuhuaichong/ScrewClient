using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tip2 : MonoBehaviour
{
    LanguageText Tip1Text1;
    TbGameWithDrawTip TbGameWithDrawTip;
    List<string> allName;
    List<string> allSurName;
    float max;
    float min;
    internal void Init()
    {
        Tip1Text1 = transform.Find("LanguageText").GetComponent<LanguageText>();
        TbGameWithDrawTip = ConfigModule.Instance.Tables.TbGameWithDrawTip;
        allName = new List<string>();
        allSurName = new List<string>();
        foreach (var item in TbGameWithDrawTip.DataList)
        {
            if (!string.IsNullOrEmpty(item.Name))
            {
                allName.Add(item.Name);
            }
            if (!string.IsNullOrEmpty(item.Surname))
            {
                allSurName.Add(item.Surname);
            }
        }
        max = TbGameWithDrawTip.GetOrDefault(2).GetRewardMax;
        min = TbGameWithDrawTip.GetOrDefault(2).GetRewardMin;

        //gameObject.SetActive(false);
        gameObject.transform.localScale = Vector3.zero;
    }
    public void StartShowTip()
    {
        StartCoroutine(ShowTipCon());
    }
    IEnumerator ShowTipCon()
    {
        yield return new WaitForSeconds(2);
        ShowTip();
        while (true)
        {
            yield return new WaitForSeconds(7);
            ShowTip();
        }
    }
    public void ShowTip()
    {
        float value = UnityEngine.Random.Range(min, max);
        string money = GameTool.GetDollarIconAndNum(value);
        string name = allName[UnityEngine.Random.Range(0, allName.Count)] + allSurName[UnityEngine.Random.Range(0, allSurName.Count)];
        GameObject go = Instantiate(gameObject, transform.parent);
        go.transform.localScale = Vector3.one;
        //go.transform.Find("Tip1Text1").GetComponent<LanguageText>().SetTextWithParameterTwo("Congratfulations player {0} successfully withdrawing <size=28><color=red>{1}</color></size>", name, money);
        go.transform.Find("LanguageText").GetComponent<LanguageText>().SetTextWithParameterTwo
            ("Congrafulations <color=red>player_{0}</color> for passing this level and successfully withdrawing <size=28><color=red>{1}</color></size>", name, money);

        StartCoroutine(MoveThis(go));

    }
    IEnumerator MoveThis(GameObject go)
    {
        go.gameObject.SetActive(true);
        RectTransform tipRect = go.GetComponent<RectTransform>();

        // 局部定义变量
        float moveSpeed = 200f; // 每秒移动的像素数
        float yPos = tipRect.anchoredPosition.y; // Y轴位置
        float startX = Screen.width / 2; // 初始X轴位置
        tipRect.anchoredPosition = new Vector2(startX, yPos); // 设置初始位置

        // 无限移动直到完全移出屏幕
        while (tipRect.anchoredPosition.x > -Screen.width / 2 - tipRect.sizeDelta.x)
        {
            // 每帧移动一定的距离
            tipRect.anchoredPosition += new Vector2(-moveSpeed * Time.deltaTime, 0);
            yield return null; // 等待下一帧
        }

        // 确保最终位置在屏幕外
        tipRect.anchoredPosition = new Vector2(-Screen.width / 2 - tipRect.sizeDelta.x / 2, yPos);

        // 隐藏弹窗
        Destroy(go);
    }
}
