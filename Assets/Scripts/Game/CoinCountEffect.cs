using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CoinCountEffect : MonoBehaviour
{
    Text languageText;
    internal void Init(int v)
    {
       // Debug.Log("生成金币特效");
        gameObject.SetActive(true);
        languageText=GetComponent<Text>();
        if (v > 0)
        {
            languageText.text = "+" + v.ToString();
        }
        else
        {
            languageText.text = "-" + Mathf.Abs(v).ToString();
        }
        Vector3 targetPos = transform.position + new Vector3(0, 1, 0);
        transform.DOMove(targetPos, 1f)
              .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }


}
