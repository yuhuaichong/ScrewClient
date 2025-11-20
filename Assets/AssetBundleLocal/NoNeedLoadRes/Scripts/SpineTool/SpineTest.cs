using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class SpineTest : MonoBehaviour
{
     SpineTool spineTool;
    public Transform screw;
    Vector3 starPos;
    void Start()
    {
        spineTool=new SpineTool();
        spineTool.Init(GetComponent<SkeletonAnimation>());
        //screw=GameObject.Find("Square").transform;
        //starPos=screw.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            spineTool.PlayAnimation("animation", false, 1.25f);
            //screw.position=starPos;
            //Vector2 targetPos = new Vector2(screw.position.x, screw.position.y + 1.5f);
            //screw.DOMove(targetPos, 0.5f)
            //.SetEase(Ease.InQuad);
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            spineTool.PlayAnimationReverse("animation", false);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            screw.position = starPos;

        }
    }
}
