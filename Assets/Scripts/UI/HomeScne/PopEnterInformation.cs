using cfg;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace DafultScript
{
    public class PopEnterInformation : BaseUI
    {
        Button ButWen;
        ToggleGroup TogPar;
        Toggle ToggleT;

        InputField EmailName;
        InputField Email;
        InputField TeleName;
        InputField tele;
        Text TeleNum;
        Button ConfirmBut;
        int state;
        int payChance;
        Button Close;
        protected override void Awake()
        {
            base.Awake();
            ButWen = tableTransform.Find("MethTipPar/ButWen").GetComponent<Button>();
            TogPar = tableTransform.Find("MethTipPar/TogPar").GetComponent<ToggleGroup>();
            ToggleT = tableTransform.Find("MethTipPar/TogPar/Toggle").GetComponent<Toggle>();
            EmailName = tableTransform.Find("EmailPar/EmailName").GetComponent<InputField>();
            Email = tableTransform.Find("EmailPar/Email").GetComponent<InputField>();
            TeleName = tableTransform.Find("TelePar/TeleName").GetComponent<InputField>();
            tele = tableTransform.Find("TelePar/tele").GetComponent<InputField>();
            TeleNum = tableTransform.Find("TelePar/TeleNumPar/TeleNum").GetComponent<Text>();
            ConfirmBut = tableTransform.Find("ConfirmBut").GetComponent<Button>();
            Close = tableTransform.Find("Close").GetComponent<Button>();
            ConfirmBut.onClick.AddListener(ConfirmButOnClikHandle);
            Close.onClick.AddListener(CloseOnClikHandle);
            ButWen.onClick.AddListener(ButWenOnClikHandle);

            string s = GameTool.confPayRegion.Channels;
            string[] qiaodaos = s.Split(',');
            foreach (var item in qiaodaos)
            {
                GameObject go = Instantiate(ToggleT.gameObject, TogPar.transform);
                go.gameObject.SetActive(true);
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.group = TogPar;
                go.AddComponent<MethTog>().Init(item, this);
                toggle.onValueChanged.AddListener(delegate (bool argo)
                {
                    if (argo)
                    {
                        int n = int.Parse(item);
                        ChanePayType(n);
                    }
                });
            }

            TogPar.transform.GetChild(1).GetComponent<Toggle>().isOn = true;
        }

        private void ButWenOnClikHandle()
        {
            UIManager.Instance.HideUI<PopEnterInformation>();
            UIManager.Instance.ShowUI<PopNoMethod>();
        }

        private void CloseOnClikHandle()
        {
            UIManager.Instance.HideUI<PopEnterInformation>();
            GameTool.CheakNewPlayerGuite();
        }

        private void ConfirmButOnClikHandle()
        {
            if (state == 1)
            {
                if (string.IsNullOrEmpty(EmailName.text))
                {
                    GameTool.CreatTip("请检查信息（输入框无内容时的提示信息）");
                    return;
                }
                string semail = Email.text.Trim();
                // 使用正则表达式验证邮箱格式
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!regex.IsMatch(semail))
                {
                    GameTool.CreatTip("条件不足（输入名和电话点击提现的提示）");
                    return;
                }
                //成功提交邮箱
                SuessComEmail();
            }
            else
            {
                if (string.IsNullOrEmpty(TeleName.text))
                {
                    GameTool.CreatTip("请检查信息（输入框无内容时的提示信息）");
                    return;
                }
                string phone = tele.text.Trim();
                if (string.IsNullOrEmpty(phone) || phone.Length < 5 || phone.Length > 13)
                {
                    GameTool.CreatTip("条件不足（输入名和电话点击提现的提示）");
                    return;
                }
                // 检查是否只包含数字
                foreach (char c in phone)
                {
                    if (!char.IsDigit(c))
                    {
                        GameTool.CreatTip("条件不足（输入名和电话点击提现的提示）");
                        return;
                    }
                }
                //成功提交电话
                SuessComElephone();
            }
        }

        private void SuessComElephone()
        {
            GameDataManager.CurrentGameData.isComWithdrawData = true;
            GameDataManager.CurrentGameData.state = 2;
            GameDataManager.CurrentGameData.eleName = TeleName.text;
            GameDataManager.CurrentGameData.eleNum = tele.text;
            GameDataManager.CurrentGameData.payChanceSn = payChance;
            GameDataManager.Save();
            UIManager.Instance.HideUI<PopEnterInformation>();
            UIManager.Instance.ShowUI<PopWithDrawMethod>();
        }

        private void SuessComEmail()
        {
            GameDataManager.CurrentGameData.isComWithdrawData = true;
            GameDataManager.CurrentGameData.state = 1;
            GameDataManager.CurrentGameData.email = Email.text;
            GameDataManager.CurrentGameData.emailName = EmailName.text;
            GameDataManager.CurrentGameData.payChanceSn = payChance;
            GameDataManager.Save();
            UIManager.Instance.HideUI<PopEnterInformation>();
            UIManager.Instance.ShowUI<PopWithDrawMethod>();
        }
        public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.HideUI(delaytime, callback, UIEffectType.Scale);
        }
        public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
        {
            base.ShowUI(callback, UIEffectType.Scale);
        }

        private void ChanePayType(int n)
        {
            payChance = n;
            ConfPayChannel confPayChannel = ConfigModule.Instance.Tables.TbPayChannel.GetOrDefault(n);
            if (confPayChannel.Info == 2)
            {
                tele.transform.parent.gameObject.SetActive(true);
                Email.transform.parent.gameObject.SetActive(false);
                TeleNum.text = $"+{GameTool.confPayRegion.CountryCode}";
                state = 2;
            }
            else
            {
                Email.transform.parent.gameObject.SetActive(true);
                tele.transform.parent.gameObject.SetActive(false);
                state = 1;
            }
        }
    }
}
