using UnityEngine.UI;
namespace DafultScript
{
    public class ExtraRocket : MainBaseUI
    {
        private Button coinButton;
        private Button freeButton;
        private Button closeButton;

        protected override void Awake()
        {
            base.Awake();
            closeButton = tableTransform.Find("Button Close").GetComponent<Button>();
            coinButton = tableTransform.Find("Button Gold").GetComponent<Button>();
            freeButton = tableTransform.Find("Button Free").GetComponent<Button>();
            tableTransform.transform.Find("Button Gold/Text (Legacy)").GetComponent<Text>().text = GameTool.getOnePropNeedCoin.ToString();
            closeButton.onClick.AddListener(CloseEvent);
            coinButton.onClick.AddListener(CoinEvent);
            freeButton.onClick.AddListener(FreeEvent);
            // tableTransform.Find("CanGetDesText").GetComponent<LanguageText>().SetTextWithParameter("观看广告获得{0}个道具", 1);
        }

        private void CloseEvent()
        {
            UIManager.Instance.HideUI<ExtraRocket>();
        }

        private void CoinEvent()
        {
            if (GameDataManager.DecreaseCoinCount(GameTool.getOnePropNeedCoin))
            {
                GameDataManager.AddItemCount(ItemType.Rocket, 1);
                EventManager.Instance.TriggerEvent(GameEvent.GetProp2, 1);
                UIManager.Instance.HideUI<ExtraRocket>();
            }
        }

        private void FreeEvent()
        {

            GameVideoContor.ShowVideoAd(VedioAdType.购买榔头, VedioAdType.购买榔头.ToString(), delegate (bool isComplete, int a)
            {
                if (isComplete)
                {
                    GameDataManager.AddItemCount(ItemType.Rocket, 1);
                    EventManager.Instance.TriggerEvent(GameEvent.GetProp2, 1);
                    UIManager.Instance.HideUI<ExtraRocket>();
                }
            }, null);


        }
    }
}
