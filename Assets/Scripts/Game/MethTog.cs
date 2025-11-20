using cfg;
using UnityEngine;
using UnityEngine.UI;
using DafultScript;

public class MethTog : MonoBehaviour
{
    ConfPayChannel confPayChannel;
    public void Init(string paySn, PopEnterInformation popEnterInformation)
    {
        confPayChannel = ConfigModule.Instance.Tables.TbPayChannel.GetOrDefault(int.Parse( paySn));
        transform.Find("Icon").GetComponent<Image>().sprite = ResourceLoader.Instance.GetResWithPath<Sprite>($"UI/PayType/244-154/{confPayChannel.PicPath}.png");

    }
}
