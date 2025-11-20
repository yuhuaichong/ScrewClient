using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DafultScript;

//[RequireComponent(typeof(Rigidbody2D))]
public class Screw : MonoBehaviour
{
    #region 组件
    private Collider2D cd;
    private Animator screwAnim;
    private Animator explosionAnim;
    private SpriteRenderer sr;
    private SpriteRenderer screwsr;
    private SpriteRenderer shadowsr;
    private SpriteRenderer holesr;
    private Rigidbody2D rb;
    private ScrewHole screwHole;
    public ScrewHole _Hole { get => screwHole; }
    #endregion

    #region 基本属性
    [SerializeField] public bool isClicked = false;//是否被点击了
    [SerializeField] private bool isPlaced = false;//是否放入了盒子中
    public bool IsPlaced
    {
        get => isPlaced;
    }
    [SerializeField] private ScrewColor color;//螺丝颜色
    public void SetColor(ScrewColor col)
    {
        color = col;
    }
    #region 星星螺丝
    [Header("星星螺丝")]
    [SerializeField] private bool isStarHole;//是星星螺丝
    public bool IsStar()
    {
        return false;
    }
    public void SetIsStarHole(bool value)
    {
        isStarHole = value;
    }
    #endregion

    #region 链接的螺丝
    [Header("链接的螺丝")]
    [SerializeField] private Screw connectedScrew;//链接的小球
    public Screw ConnectedScrew
    {
        get => connectedScrew;
    }
    #endregion

    #region 门
    [Header("门")]
    [SerializeField] private bool hasDoor;//有门
    public bool HasDoor
    {
        get => hasDoor;
    }
    public void SetDoorFlase()
    {
        hasDoor = false;
        door = null;
    }

    [SerializeField] private bool isClose;//门的状态
    public bool IsDoorClose { get => isClose; }
    private Door door;
    public Door ScrewDoor
    {
        get => door;
        set
        {
            door = value;
        }
    }
    #endregion

    #region 爆炸
    [Header("爆炸")]
    [SerializeField] private bool hasBoom;//是否有炸弹
    [SerializeField] private int boomCount;
    public Boom ScrewBoom { get; set; }

    public int BoomCount { get => boomCount; }
    public bool HasBoom { get => hasBoom; }

    private Vector3 originalScale;
    private Vector3 enlargedScale;
    #endregion

    #region 移动和层级
    [SerializeField]private bool ismoving;
    public bool IsMoving
    {
        get => ismoving;
    }//是否处于移动状态
    public float GetMoveTime
    {
        get
        {
            return moveUpTime + moveTargetTime + moveDownTime;
        }
    } //得到移动的整体时间

    private float moveUpTime;//向上移动的时间
    private float moveTargetTime;//移向目标点的时间
    private float moveDownTime;//向下移动的时间
    private Coroutine rotateCoroutine;
    public int Layer
    {
        get
        {
            return SortingLayer.GetLayerValueFromID(sr.sortingLayerID);
        }
    }    //得到目前处于第几层

    public string LayerName
    {
        get
        {
            return sr.sortingLayerName;
        }
    }

    public int LayerOrder
    {
        get
        {
            return sr.sortingOrder;
        }
    }
    #endregion

    #endregion

    #region 绳子（已移除Obi支持）
    // Obi rope functionality has been removed
    private Vector3 oriPos;
    #endregion

    #region 锁链
    [Header("锁链")]
    [SerializeField] private bool hasChain;
    public bool HasChain { get => hasChain; }
    [SerializeField] List<Screw> chainScrewList = new List<Screw>();//其他的螺丝
    private List<GameObject> mainChainList = new List<GameObject>();
    private Screw mainChainScrew;//锁的主螺丝

    public bool IsOtherChain { get; set; }
    private ChainFx chainFx;
    public void SetChain(ChainFx chainfx)
    {
        chainFx = chainfx;
        chainFx.SetPlayCount(chainScrewList.Count);
    }
    #endregion

    #region 冰
    [Header("是否被冰覆盖")]
    [SerializeField] private bool isIceCovered;
    public void SetIceCoveredFalse()
    {
        isIceCovered = false;
    }
    private Ice ice;

    public Ice ScrewIce
    {
        get => ice;
        set
        {
            ice = value;
        }
    }

    public bool IsIceCovered
    {
        get => isIceCovered;
    }
    #endregion

    [Header("锁/钥匙")]
    [SerializeField] private bool hasKey;
    [SerializeField] public bool hasLock;
    [SerializeField] private Screw lockScrew;//钥匙螺丝，解锁的锁的螺丝
    public Lock ScrewLock { get; set; }
    public Key ScrewKey { get; set; }

    public bool HasKey { get => hasKey; }
    public bool HasLock { get => hasLock; }

    //钥匙螺丝调用的函数
    public void KeyMoveToLcok()
    {
        ScrewKey.MoveToLock(lockScrew.ScrewLock, () => lockScrew.hasLock = false);
    }

    private Glass glass;
    public ScrewColor ScrewColor
    {
        get
        {
            return color;
        }
    }

    
   public SpineTool spineTool;
    private void Awake()
    {
        //得到播放动画的子物体
        screwAnim = transform.Find("Image/Hole").GetComponent<Animator>();
        explosionAnim = GetComponent<Animator>();
        sr = transform.Find("Image").GetComponent<SpriteRenderer>();
        screwsr = transform.Find("Image/Hole").GetComponent<SpriteRenderer>();
        shadowsr = transform.Find("shadow").GetComponent<SpriteRenderer>();
        holesr = transform.parent.GetComponent<SpriteRenderer>();
        screwHole = transform.parent.GetComponent<ScrewHole>();
        sr.drawMode = SpriteDrawMode.Tiled;

        screwsr.sortingLayerName = sr.sortingLayerName;
        screwsr.sortingOrder = 0;

        cd = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        color=GameTool.GetNewColor(color);
        GameObject screwAnimator =Instantiate(ResourceLoader.Instance.GetResWithPath<GameObject>("Prefab/Screw/SpineScrew.prefab"), sr.transform);
        //screwAnimator.transform.localPosition = new Vector3(0, -0.35f,0);
        screwAnimator.transform.localPosition = new Vector3(0, -0.35f, 0);
        sr.enabled = false;
        spineTool =new SpineTool();
        spineTool.Init(screwAnimator.GetComponent<SkeletonAnimation>());
        spineTool.SetSortingLayerAndOrderLayer(sr.sortingLayerID,sr.sortingOrder);
        spineTool.SetScrewColor(color);
        transform.rotation=Quaternion.identity;
    }
    private void Start()
    {
        glass = transform.parent.parent.GetComponent<Glass>();
        moveUpTime = 0.2f;
        //moveTargetTime = 0.24f;
        moveTargetTime = 0.24f;
        moveDownTime = 0.2f;
        oriPos = transform.position;

        originalScale = transform.localScale;
        enlargedScale = originalScale * 1.2f;

        if (door != null)
            door.SetClose(isClose);

        if (!HasGlassCovered())
            shadowsr.enabled = false;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.gravityScale = 0;
    }


    #region 点击
    /// <summary>
    /// 能否从玻璃中移除
    /// </summary>
    /// <returns></returns>
    public bool CanRemoveFromGlass()
    {
        bool canRemove = true;

        ////有链接的螺丝，螺丝不能点击并且没有放入盒子中
        //if (connectedScrew != null && !connectedScrew.CanClick() && !connectedScrew.IsPlaced)
        //    canRemove = false;

        return canRemove;
    }
    /// <summary>
    /// 检查是否可以点击
    /// </summary>
    /// <returns></returns>
    public bool CanClick()
    {
        bool canClick = HasGlassCovered() == false;
        cd.isTrigger = true;

        //if (HasDoor)
        //    canClick = canClick && door.IsClose == false;

        //if (isIceCovered)
        //    canClick = false;

        //if (hasChain)
        //    canClick = false;

        //if (hasLock)
        //    canClick = false;
        if (LevelManager.Instance.levelNum == 1)
        {
            if (HandAnim.instance.level1Glass != this)
            {
                return false;
            }
          
        }

        return canClick && isClicked == false;
    }
    /// <summary>
    /// 是否有玻璃覆盖
    /// </summary>
    public bool HasGlassCovered()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.45f);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Glass>() != null)
            {
                if (Layer < hit.GetComponent<Glass>().Layer)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.45f);
    }
    public void RemoveFromGlass()
    {
        //判断是否是悬空物体
        if (glass != null && glass.ScrewList.Count > 0)
            glass.RemveScrew(this);


        //把洞隐藏
        if (holesr != null && glass.gameObject.name == "glass 0")
        {
            screwHole.RocketExplotion();
        }


    }
    #endregion

    #region 动画
    //设置螺丝的动画
    private void SetScrewAnim(bool boolean)
    {
        if (boolean)
        {
            spineTool.PlayAnimation("animation", false, 2f);
        }
        else
        {
            spineTool.PlayAnimationReverse("animation", false);
        }
    }

    private void EnterBoomAnim()
    {
        explosionAnim.SetBool("EndBoom", true);
    }

    /// <summary>
    /// 爆炸动画
    /// </summary>
    public void DoBoomAnim()
    {
        if (ScrewBoom.gameObject == null)
            return;

        ScrewBoom.DecreaseCount();
        Vector3 orboomScale = ScrewBoom.transform.localScale;
        // 同时放大并改变颜色
        transform.DOScale(enlargedScale, 0.15f).SetEase(Ease.OutQuad) // 先快后慢
            .OnStart(() =>
            {
                explosionAnim.SetBool("EndBoom", true);
                ScrewBoom.transform.DOScale(orboomScale * 1.2f, 0.15f).SetEase(Ease.OutQuad); // 先快后慢
            })
            .OnComplete(() => // 放大完成后执行缩小和颜色恢复
            {
                transform.DOScale(originalScale, 0.15f).SetEase(Ease.OutQuad); // 先快后慢
                ScrewBoom.transform.DOScale(orboomScale, 0.15f).SetEase(Ease.OutQuad); // 先快后慢

                if (!ScrewBoom.CanEnterBoomAnim())
                    explosionAnim.SetBool("EndBoom", false);
            });

        if (ScrewBoom.CanEnterBoomAnim())
            EnterBoomAnim();
    }
    public void DestoryBoom()
    {
        if (ScrewBoom != null)
        {
            explosionAnim.SetBool("EndBoom", false);
            Destroy(ScrewBoom.gameObject);
        }
    }

    /// <summary>
    /// 将阴影的层级设置为本层级
    /// </summary>
    public void SetShadow()
    {
        shadowsr.sortingLayerName = sr.sortingLayerName;
    }

    #endregion

    #region 移动
    /// <summary>
    /// 向上移动的动画
    /// </summary>
    /// <param name="_moveTargetPos"></param>
    public void MoveUp(Vector3 _moveTargetPos, System.Action callback = null)
    {
        isClicked = true;
        sr.sortingLayerName = "End";
        screwsr.sortingLayerName = "End";
        shadowsr.sortingLayerName = "End";
        if (LevelManager.Instance.levelNum == 1)
        {
            spineTool.SetSortingLayerAndOrderLayer("End", 21);
            sr.sortingOrder = 21;
        }
        else
        {
            spineTool.SetSortingLayerAndOrderLayer("End", 10);
        }


        HingeJoint2D[] screwHjs=GetComponents<HingeJoint2D>();

        foreach (var item in screwHjs)
        {
            if (item != null)
            {
                item.connectedBody = null;
                item.enabled = false;
            }
        }


        // 启动协程，等待 ismoving 被置为 false
        StartCoroutine(WaitForNotMovingThenMove(_moveTargetPos, callback));
    }

    /// <summary>
    /// 等待上次移动完成之后
    /// </summary>
    /// <param name="_moveTargetPos"></param>
    private IEnumerator WaitForNotMovingThenMove(Vector3 _moveTargetPos, System.Action callback = null)
    {
        // 等待 ismoving 被置为 false
        while (ismoving)
        {
            //Debug.Log("等待上一次移动");
            yield return null; // 等待下一帧
        }

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        transform.rotation = Quaternion.Euler(Vector3.zero);

        ismoving = true;
        SetScrewAnim(true);
        Vector2 targetPos = new Vector2(transform.position.x, transform.position.y + 1.5f);
        // 动画开始时添加 TrailRenderer
        AddTrailRenderer();
        // 开始移动动画
        transform.DOMove(targetPos, moveUpTime)
           
            .OnComplete(() =>
            {
                //检查是否有链接的螺丝
                //没有则直接移动到目标位置
                if (connectedScrew == null || true)
                {
                    //Debug.Log(color + "没有链接的物体");
                    MoveToTarget(_moveTargetPos, callback);
                }
                else if (connectedScrew != null && connectedScrew.CanClick())
                {
                    //如果有链接的螺丝，需要是可以点击的状态
                    //Debug.Log(color + "拉动其他的物体");
                    MoveToTarget(_moveTargetPos, callback);
                }
                else if (connectedScrew != null && !connectedScrew.CanClick() && connectedScrew.isPlaced)
                {
                    //被拉动的状态
                    //Debug.Log("被拉动");
                    MoveToTarget(_moveTargetPos, callback);
                }
                else if (connectedScrew != null && !connectedScrew.CanClick() && !connectedScrew.isPlaced)
                {
                    //如果有，但是不可以点击(bug，被拉动的小球会移动回[已解决]))
                    //移动回去
                    Debug.Log("链接的小球无法点击");
                    MoveBack();
                }
                else
                {
                    Debug.Log("情况不对，需要补充");
                }
            });
    }
    /// <summary>
    /// 向目标点移动的动画
    /// </summary>
    /// <param name="_moveTargetPos"></param>
    private void MoveToTarget(Vector3 _moveTargetPos, System.Action callback = null)
    {
        //Debug.Log(color + "向目标点移动");
        Vector3 movetarget = new Vector3(_moveTargetPos.x, _moveTargetPos.y + 1f);
        transform.DOMove(movetarget, moveTargetTime)
            .OnComplete(() =>
            {
                // 动画结束时移除 TrailRenderer
                RemoveTrailRenderer();
                SetScrewAnim(false);
                MoveDown(_moveTargetPos, callback);
                //callback?.Invoke();
            });
    }

    // 添加 TrailRenderer 组件并配置
    private void AddTrailRenderer()
    {
        GameObject trailObj = Instantiate(ResourceLoader.Instance.GetResWithPath<GameObject>("Prefab/TrailRender.prefab"), transform);
        trailObj.name = "trailObj";
        trailObj.transform.SetAsLastSibling();
        TrailRenderer trail = trailObj.gameObject.GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = trailObj.gameObject.AddComponent<TrailRenderer>();
        }
        trail.sortingLayerName = "End"; // 设定为对应的层
        if (LevelManager.Instance.levelNum == 1)
        {
            trail.sortingOrder = 21; // 设置为一个较高的值
        }
        else
        {
            trail.sortingOrder = 10; // 设置为一个较高的值
        }
        // 创建宽度曲线
        // 曲线关键帧，时间从0到1，值从1逐渐变成0，带缓动效果
        AnimationCurve widthCurve = new AnimationCurve(
           
            new Keyframe(0f, 0.4f, 0f, 0f),    // t=0，宽度=1，无切线斜率（平滑）

            new Keyframe(1f, 0.2f)                  // 末端宽度0
        );
        trail.widthCurve = widthCurve;
        trail.time = 0.2f;
        Material trailMaterial = ResourceLoader.Instance.GetResWithPath<Material>("Mat/Transparent.mat");
        trail.material = trailMaterial;

        // 设置颜色渐变
        Gradient gradient = new Gradient();
        Color color = GameTool.GetScrewLineRenderColor(this.color);
        gradient.colorKeys = new GradientColorKey[]
        {
        new GradientColorKey(color, 0f),   // 开始为绿色
        new GradientColorKey(color, 1f)      // 结束为红色
        };

        // 你可以添加 Alpha 关键帧，控制渐变透明度
        gradient.alphaKeys = new GradientAlphaKey[]
        {
        new GradientAlphaKey(1f, 0f),  // 开始完全不透明
        new GradientAlphaKey(0f, 1f)   // 结束完全透明
        };

        trail.colorGradient = gradient;
    }

    // 移除 TrailRenderer 组件
    private void RemoveTrailRenderer()
    {
    
        if (transform.Find("trailObj") != null)
        {
            TrailRenderer trail = transform.Find("trailObj").GetComponent<TrailRenderer>();
            if (trail != null)
            {
                Destroy(trail.gameObject);
            }
        }

        //TrailRenderer trail = gameObject.GetComponent<TrailRenderer>();
        //if (trail != null)
        //{
        //    Destroy(trail);
        //}

    }
    /// <summary>
    /// 向下移动的动画
    /// </summary>
    private void MoveDown(Vector3 _moveTargetPos, System.Action callback = null)
    {
        //rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.DOMove(_moveTargetPos, moveDownTime)/*.SetEase(Ease.InQuad)*/.OnComplete(() =>
            {
                ismoving = false;
                cd.enabled = false;
                isPlaced = true;
                callback?.Invoke();
                //检查游戏是否失败
                GameManager.Instance.loseGame();
                if (LevelManager.Instance.levelNum == 1)
                {
                    spineTool.SetSortingLayerAndOrderLayer("End", 21);
                }
                else
                {
                    spineTool.SetSortingLayerAndOrderLayer("End", 1);
                }
                //如果有链接的小球,并且没有放入到目标盒子中，需要把链接的小球设置到目标位置
                //if (connectedScrew != null)
                //{
                //    if (connectedScrew.isPlaced == false)
                //    {
                //        //拉动了链接的螺丝之后，取消链接小球的效果
                //        connectedScrew.isClicked = true;
                //        rope.GetComponents<ObiParticleAttachment>()[0].enabled = false; // 起点绑定
                //        rope.GetComponents<ObiParticleAttachment>()[1].enabled = false; // 终点绑定
                //        Invoke(nameof(DelayReturnRope), 3f);
                //        connectedScrew.connectedScrew = null;

                //        GameManager.Instance.SetScrew(connectedScrew);
                //        connectedScrew = null;
                //    }
                //}
            });
    }
    //移动回去
    private void MoveBack()
    {
        SetScrewAnim(false);
        transform.DOMove(oriPos, moveDownTime)
            .OnComplete(() =>
            {
                isClicked = false;
                ismoving = false;
            });
    }
    #endregion

    #region 锁链
    /// <summary>
    /// 创建锁链，链接自身（已移除Obi支持）
    /// </summary>
    public void CreateChain()
    {
        // Obi chain functionality has been removed
        float angleStep = 360 / chainScrewList.Count;

        for (int i = 0; i < chainScrewList.Count; i++)
        {
            GameObject newChain = Instantiate(ResourceLoader.Instance.GetChain(), transform.parent);
            newChain.transform.position = transform.position;
            newChain.transform.rotation = Quaternion.Euler(0, 0, i * angleStep);

            if (chainScrewList.Count == 2 && i == 1)
                newChain.transform.rotation = Quaternion.Euler(0, 0, 90);

            mainChainList.Add(newChain);
        }
    }
    /// <summary>
    /// 将主螺丝的锁解开（已移除Obi支持）
    /// </summary>
    public void UnlockChain()
    {
        IsOtherChain = false;
        mainChainScrew.chainFx.PlayParticle();//主螺丝 播放粒子效果

        if (mainChainScrew.mainChainList.Count > 0)
        {
            Destroy(mainChainScrew.mainChainList[0]);
            mainChainScrew.mainChainList.RemoveAt(0);
        }

        //主螺丝的列表进行移除
        mainChainScrew.chainScrewList.Remove(this);

        if (mainChainScrew.chainScrewList.Count == 0)
            mainChainScrew.hasChain = false;
    }

    internal void SetSpineSort()
    {
        if(spineTool == null)return;
        spineTool.SetSortingLayerAndOrderLayer(sr.sortingLayerID, sr.sortingOrder);
    }
    #endregion
}
