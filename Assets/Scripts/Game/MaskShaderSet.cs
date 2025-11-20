using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MaskShaderSet : MonoBehaviour
{
    public Material material;

    Vector4 endVec;
    Vector4 nowVec;

    public float ratio;

    float width;
    float length;
    internal void Init(Material material)
    {
        this.material = material;
        endVec = material.GetVector("_Origin");
        material.SetVector("_Origin", new Vector4(0, 0, 0, 0));
        endVec = Vector4.zero;
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        length = cam.pixelHeight;
        width = cam.pixelWidth;
        SetRatio();//如果宽了没有问题，长了就需要适配
    }

    private void SetRatio()
    {


        float float1 = width / length;
        float float2 = 1290f / 2796f;
        if (float1 < float2)
        {
            float f1 = width / 1290f;
            float f2 = length / f1;
            ratio = (f2 - 2796f) / 2;
            //float y2 = manger.width * 2796f / 1290f;
            //ratio =  (manger.length-y2)/2;
        }
        else
        {
            ratio = 0;
        }
    }

    public void SetMaskPos(float x, float y, float rad, bool isNoNeedRationJuXing)
    {
        //isNoNeedRationJuXing = false;
        material.SetFloat("_MaskType", 0f);
        if (!isNoNeedRationJuXing)
        {
            if (y < 0)
            {
                y -= ratio;
            }
            else
            {
                y += ratio;
            }
        }
        material.SetVector("_Origin", new Vector4(x, y, rad, 20));
    }
    IEnumerator ShowMask()
    {
        float time1 = 0;
        while (time1 < 1 && gameObject.activeSelf)
        {
            time1 += Time.deltaTime * 2;
            material.SetVector("_Origin", Vector4.Lerp(endVec, nowVec, time1));
            yield return new WaitForEndOfFrame();
        }
        endVec = nowVec;
    }
    public void SetMaskPosJu(float x, float y, float z, float j, RectTransform rect,bool isNoNeedRationJuXing = false)
    {

        //isNoNeedRationJuXing = false;
        material.SetFloat("_MaskType", 1f);
        if (!isNoNeedRationJuXing)
        {

            if (y < 0)
            {
                y -= ratio;
            }
            else
            {
                y += ratio;
            }
            if (j < 0)
            {
                j -= ratio;
            }
            else
            {
                j += ratio;
            }
        }

        material.SetVector("_Origin", new Vector4(x, y, z, j));

        // 设置RectTransform的位置和大小
        if (rect != null)
        {
            // 计算宽高
            float width = Mathf.Abs(z - x);
            float height = Mathf.Abs(j - y);
            rect.sizeDelta = new Vector2(width, height);

            // 计算中心点位置
            float centerX = (x + z) * 0.5f;
            float centerY = (y + j) * 0.5f;
            rect.anchoredPosition = new Vector2(centerX, centerY);
        }
    }

    internal Vector2 GetPos(Vector2 pos)
    {
        if (pos.y < 0)
        {
            pos.y -= ratio;
        }
        else
        {
            pos.y += ratio;
        }
        return pos;
    }
}
