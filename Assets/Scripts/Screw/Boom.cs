using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Boom : MonoBehaviour
{
    private SpriteRenderer sr;
    private TextMeshPro text;
    private SortingGroup sg;
    private GameObject explosion;
    private ParticleSystemRenderer psr;
    private int curCount;

    public int GetCount()
    {
        return curCount;
    }
    public void InitBoom(ScrewColor color, int count, string layername, int order)
    {
        sr = transform.Find("Image").GetComponent<SpriteRenderer>();
        psr = transform.Find("Image").GetChild(0).GetComponent<ParticleSystemRenderer>();

        text = transform.Find("Text").GetComponent<TextMeshPro>();
        explosion = transform.Find("Explosion Bomb").gameObject;
        sg = transform.Find("Text").GetComponent<SortingGroup>();

        explosion.SetActive(false);
        curCount = count;
        text.text = count.ToString();

        sr.sprite = ResourceLoader.Instance.GetBoomSpriteByColor(color);
        sr.sortingLayerName = layername;
        sr.sortingOrder = order - 1;
        sg.sortingLayerName = layername;
        sg.sortingOrder = order + 1;

        psr.sortingLayerName = layername;
        psr.sortingOrder = order;
    }
    public void DecreaseCount()
    {
        curCount--;
        text.text = curCount.ToString();

        if (curCount == 0)
        {
            explosion.SetActive(true);
            Invoke(nameof(DelayShowReviveUI), 0.8f);
        }
    }

    public bool CanEnterBoomAnim()
    {
        return curCount == 1;
    }

    private void DelayShowReviveUI()
    {
        Destroy(explosion);
        Destroy(gameObject);
    }

}
