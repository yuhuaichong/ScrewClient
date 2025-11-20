using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class CanvasAdpat : MonoBehaviour
    {
        private void Awake()
        {
            float x = 1290f / Screen.width;
            float y = 2796f / Screen.height;
            this.transform.GetComponent<CanvasScaler>().matchWidthOrHeight = x > y ? 0 : 1;
        }
    }
}
