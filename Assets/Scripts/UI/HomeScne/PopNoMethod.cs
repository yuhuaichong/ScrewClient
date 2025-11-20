using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class PopNoMethod : BaseUI
    {
        Button Close;
        Button ConfirmBut;
        InputField Email;
        protected override void Awake()
        {
            base.Awake();
            Close = tableTransform.Find("Close").GetComponent<Button>();
            ConfirmBut = tableTransform.Find("ConfirmBut").GetComponent<Button>();
            Email = tableTransform.Find("Email").GetComponent<InputField>();
            Close.onClick.AddListener(() =>
            {
                UIManager.Instance.ShowUI<PopEnterInformation>();
                UIManager.Instance.HideUI<PopNoMethod>();
                GameTool.CheakNewPlayerGuite();
            });
            ConfirmBut.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(Email.text))
                {
                    if (GameTool.IsCheckEmail(Email.text.Trim()))
                    {
                        GameDataManager.CurrentGameData.isComWithdrawData = true;
                        GameDataManager.CurrentGameData.state = 3;
                        GameDataManager.CurrentGameData.email = Email.text;
                        UIManager.Instance.ShowUI<PopWithDrawMethod>();
                        UIManager.Instance.HideUI<PopNoMethod>();
                    }
                }

            });
        }
    }
}
