using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeRiight : MonoBehaviour
{
    private Button NoAdsButton;
    private Button PiggyBankButton;
    private Button DailyBonusButton;
    private Button LuckySpinButton;

    private void Awake()
    {
        NoAdsButton = transform.Find("Button No Ads").GetComponent<Button>();
        PiggyBankButton = transform.Find("Button Piggy Bank").GetComponent<Button>();
        DailyBonusButton = transform.Find("Button Daily Bonus").GetComponent<Button>();
        LuckySpinButton = transform.Find("Button LuckySpin").GetComponent<Button>();

        PiggyBankButton.onClick.AddListener(PiggyEvent);
    }

    private void PiggyEvent()
    {

    }


}
