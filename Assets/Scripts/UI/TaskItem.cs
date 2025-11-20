using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using System;
using DafultScript;

public class TaskItem : MonoBehaviour
{
    Image Icon;
    LanguageText TitleText;
    LanguageText NumText;
    Slider slider;
    LanguageText BoxNumText;
    GameObject NoCanGet;
    Button Get;
    ConfScrewTask confScrewTask;
    public void Init(ConfScrewTask confScrewTask)
    {
        this.confScrewTask=confScrewTask;
        Icon =transform.Find("Icon").GetComponent<Image>();
        TitleText = transform.Find("TitleText").GetComponent<LanguageText>();
        NumText = transform.Find("Icon/NumText").GetComponent<LanguageText>();
        slider = transform.Find("Slider").GetComponent<Slider>();
        BoxNumText = transform.Find("BoxNumText").GetComponent<LanguageText>();
        NoCanGet = transform.Find("NoCanGet").gameObject;
        Get = transform.Find("Get").GetComponent<Button>();

        TitleText.SetTextWithParameter("总共收集{0}个箱子", confScrewTask.AimTarget);

        slider.value = GameDataManager.CurrentGameData.completeBoxNum * 1.0f / confScrewTask.AimTarget;
        BoxNumText.text = $"{GameDataManager.CurrentGameData.completeBoxNum} / {confScrewTask.AimTarget}";
        NumText.text = confScrewTask.GetReward.ToString();
        
        if (GameDataManager.CurrentGameData.completeBoxNum >= confScrewTask.AimTarget)
        {
            NoCanGet.gameObject.SetActive(false);
            Get.gameObject.SetActive(true);
            Get.onClick.AddListener(GetClikcHandle);
        }
        else
        {
            NoCanGet.gameObject.SetActive(true);
            Get.gameObject.SetActive(false);
        }

    }
    public void Init(int k)
    {
        Icon = transform.Find("Icon").GetComponent<Image>();
        TitleText = transform.Find("TitleText").GetComponent<LanguageText>();
        NumText = transform.Find("Icon/NumText").GetComponent<LanguageText>();
        slider = transform.Find("Slider").GetComponent<Slider>();
        BoxNumText = transform.Find("BoxNumText").GetComponent<LanguageText>();
        NoCanGet = transform.Find("NoCanGet").gameObject;
        Get = transform.Find("Get").GetComponent<Button>();
        TitleText.GetComponent<RectTransform>().sizeDelta = new Vector2(750f, 49.995f);
        TitleText.GetComponent<RectTransform>().anchoredPosition = new Vector2(48f,-28.603f);
        if (k == 0)
        {
            ShowLeveFourWithDraw();
        }
        else if (k == 1)
        {
            ShowShowJiBoxWithDraw();
        }
        else if (k == 2)
        {
            ShowLoginSevenWithDraw();
        }
    }
    private void ShowLoginSevenWithDraw()
    {
        //TitleText.SetTextWithParameter("登录{0}天", 7);
        TitleText.languageId = "107";
        int nowLoginDay = GameTool.GetPLayerLoginDay();
        slider.value = nowLoginDay * 1.0f / 7;
        BoxNumText.text = $"{nowLoginDay} / {7}";
        NumText.transform.parent.gameObject.SetActive(false);
        if (nowLoginDay >= 7)
        {
            NoCanGet.gameObject.SetActive(false);
            Get.gameObject.SetActive(true);
            Get.onClick.AddListener(GetClikcHandle);
            Get.onClick.AddListener(() => {
                GameTool.withDrawTaskIndex = 2;
            });
            Get.transform.Find("LanguageText").GetComponent<LanguageText>().languageId = "9";
            if (GameDataManager.CurrentGameData.withDrawTaskCompleteDci.ContainsKey(2))
            {
                Get.gameObject.SetActive(false);
                NoCanGet.gameObject.SetActive(true);
                NoCanGet.transform.Find("LanguageText").gameObject.SetActive(false);
                NoCanGet.transform.Find("getIng").gameObject.SetActive(true);
            }
        }
        else
        {
            NoCanGet.gameObject.SetActive(true);
            Get.gameObject.SetActive(false);
        }
    }

    private void ShowShowJiBoxWithDraw()
    {
        // TitleText.SetTextWithParameter("总共收集{0}个箱子", 1500);
        TitleText.languageId = "106";
        slider.value = GameDataManager.CurrentGameData.completeBoxNum * 1.0f / 1500;
        int needShowNum = GameDataManager.CurrentGameData.completeBoxNum > 1500 ? 1500 : GameDataManager.CurrentGameData.completeBoxNum;
        BoxNumText.text = $"{needShowNum} / {1500}";
        NumText.transform.parent.gameObject.SetActive(false);
        if (GameDataManager.CurrentGameData.completeBoxNum >= 1500)
        {
            NoCanGet.gameObject.SetActive(false);
            Get.gameObject.SetActive(true);
            Get.onClick.AddListener(GetClikcHandle);
            Get.onClick.AddListener(() => {
                GameTool.withDrawTaskIndex = 1;
            });
            Get.transform.Find("LanguageText").GetComponent<LanguageText>().languageId = "9";
            if (GameDataManager.CurrentGameData.withDrawTaskCompleteDci.ContainsKey(1))
            {
                Get.gameObject.SetActive(false);
                NoCanGet.gameObject.SetActive(true);
                NoCanGet.transform.Find("LanguageText").gameObject.SetActive(false);
                NoCanGet.transform.Find("getIng").gameObject.SetActive(true);
            }
        }
        else
        {
            NoCanGet.gameObject.SetActive(true);
            Get.gameObject.SetActive(false);
        }
    }

    private void ShowLeveFourWithDraw()
    {
        //TitleText.SetTextWithParameter("通关{0}关", GameTool.maxLevelNum);
        TitleText.languageId = "105";
        int nowLevel = GameDataManager.CurrentGameData.levelNum - 1;
        slider.value = nowLevel * 1.0f / 4;
        BoxNumText.text = $"{nowLevel} / 4";
        NumText.transform.parent.gameObject.SetActive(false);
        if (nowLevel >= GameTool.maxLevelNum)
        {
            NoCanGet.gameObject.SetActive(false);
            Get.gameObject.SetActive(true);
            Get.onClick.AddListener(GetClikcHandle);
            Get.onClick.AddListener(() => {
                GameTool.withDrawTaskIndex = 0;
            });
            Get.transform.Find("LanguageText").GetComponent<LanguageText>().languageId = "9";
            if (GameDataManager.CurrentGameData.withDrawTaskCompleteDci.ContainsKey(0))
            {
                Get.gameObject.SetActive(false);
                NoCanGet.gameObject.SetActive(true);
                NoCanGet.transform.Find("LanguageText").gameObject.SetActive(false);
                NoCanGet.transform.Find("getIng").gameObject.SetActive(true);
            }
        }
        else
        {
            NoCanGet.gameObject.SetActive(true);
            Get.gameObject.SetActive(false);
        }
    }
    private void GetClikcHandle()
    {
        if (!GameDataManager.CurrentGameData.IsCanWIthDraw)
        {
            GameDataManager.CurrentGameData.IsCanWIthDraw = true;
            GameDataManager.Save();
        }
        UIManager.Instance.HideUI<TaskUI>();
        UIManager.Instance.ShowUI<PopEnterInformation>();
    }
}
