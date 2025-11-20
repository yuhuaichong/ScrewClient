using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YLocalization : MonoBehaviour
{
    public static Lanaguage lanaguage = Lanaguage.Chinese;
    public enum Lanaguage
    {
        Chinese,
        English
    }
    public string chineseStr;
    public string englishStr;

    void Awake()
    {
        //UpdateText();
    }

    public void UpdateText()
    {
        Debug.LogError("设置字体");
        if (GetComponent<Text>() != null)
        {
            if (lanaguage == Lanaguage.Chinese)
            {
                
                GetComponent<Text>().font = ResourceLoader.Instance.NorFont;
            }
            GetComponent<Text>().text = lanaguage == Lanaguage.English ? englishStr : chineseStr;
        }
        else if (GetComponent<TextMeshPro>() != null)
        {
            if (lanaguage == Lanaguage.Chinese)
            {
                GetComponent<TextMeshPro>().font = ResourceLoader.Instance.TmpFont;
            }
            GetComponent<TextMeshPro>().text = lanaguage == Lanaguage.English ? englishStr : chineseStr;
        }
        else if (GetComponent<TextMeshProUGUI>() != null)
        {
            if (lanaguage == Lanaguage.Chinese)
            {
                GetComponent<TextMeshProUGUI>().font = ResourceLoader.Instance.TmpFont;
            }
            GetComponent<TextMeshProUGUI>().text = lanaguage == Lanaguage.English ? englishStr : chineseStr;
        }
    }
}

