using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DafultScript;
namespace DafultScript
{
    public class NeedLockBoxPos : MonoBehaviour, IPointerClickHandler
    {
        int index;
        public void Init(int index)
        {
            this.index = index;
            gameObject.AddComponent<BoxCollider2D>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            UIManager.Instance.ShowUI<VideoToLockBox>();
            VideoToLockBox videoToLockBox = UIManager.Instance.GetUI<VideoToLockBox>();
            videoToLockBox.Init(index, gameObject);


        }

    }
}
