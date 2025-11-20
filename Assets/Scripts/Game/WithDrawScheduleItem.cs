using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
public class WithDrawScheduleItem : RecyclingListViewItem
{


    LanguageText schedData;
    LanguageText schedMoney;

    Image Image;

    bool isFInd;
    Button LookScheduleBut;
    public void Init(int rowIndex, WithDrawSchedule withDrawSchedule)
    {
        if (!isFInd)
        {

            isFInd = true;
            //PaiMing=transform.Find("PaiMing").GetComponent<Image>();
            Image = transform.Find("Image").GetComponent<Image>();
            Image.sprite = ResourceLoader.Instance.GetUnlockImageSprite($"coin_{GameTool.dollarIconPath}");
            schedData = transform.Find("schedData").GetComponent<LanguageText>();
            schedMoney = transform.Find("schedMoney").GetComponent<LanguageText>();

            LookScheduleBut = transform.Find("ShowTipBut").GetComponent<Button>();
            LookScheduleBut.onClick.AddListener(() =>
            {
                GameTool.CreatTip("提现处理中，请等待");
            });
        }

        schedData.text = $"{withDrawSchedule.withDrawTime.ToString("yyyy-MM-dd")}";
        // Image.sprite = GameTool.GetNormalMoneyIcon();
        schedMoney.text = GameTool.GetDollarIconAndNum(withDrawSchedule.withDrawMoney);

    }
}
