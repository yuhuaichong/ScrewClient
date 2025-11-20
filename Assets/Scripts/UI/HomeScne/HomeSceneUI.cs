using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DafultScript
{
    public class HomeSceneUI : MonoBehaviour
    {
        public static HomeSceneUI Instance { get; private set; }

        public HomeUI homeUI;
        public StickerUI stickerUI;

        public void EnterSticker()
        {
            homeUI.EnterSticker();
            stickerUI.EnterSticker();
        }
        public void ExitSticker()
        {
            homeUI.ExitSticker();
            stickerUI.ExitSticker();
        }

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);

            Instance = this;
        }

        public void SetHomeScene(bool val)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(val);
            }
        }
    }
}
