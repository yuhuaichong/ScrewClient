using Unity.VisualScripting;
using UnityEngine;

public class HandAnim : MonoBehaviour
{
    private GameObject child1; // 第一个子物体
    private GameObject child2; // 第二个子物体
    [SerializeField] private float switchInterval = 0.2f; // 切换的时间间隔
    [SerializeField] private float xoffset = 1;
    [SerializeField] private float yoffset = -1;
    public Screw level1Glass;
    private float timer = 0f; // 计时器
    private bool isChild1Active = true; // 当前状态
    int index;
    public static HandAnim instance;
    private void Awake()
    {
        instance = this;
        EventManager.Instance.RegisterEvent<Screw,int>(GameEvent.ShowPlayerOneLevelGuite, ShowPlayerOneLevelGuite);
    }
    private void OnDestroy()
    {
        EventManager.Instance.UnregisterEvent<Screw,int>(GameEvent.ShowPlayerOneLevelGuite, ShowPlayerOneLevelGuite);
    }
    public void ShowPlayerOneLevelGuite(Screw screw,int index)
    {
        this.level1Glass=screw;
        this.index = index;
        transform.position= level1Glass.transform.position;
        ////child1.SetActive(isChild1Active);
        //child1.transform.position = level1Glass.transform.position + new Vector3(xoffset, yoffset, 0);
        ////child2.SetActive(!isChild1Active);
        //child2.transform.position = level1Glass.transform.position + new Vector3(xoffset, yoffset, 0);
    }
    void Start()
    {
        //child1 = transform.GetChild(0).gameObject;
        //child2 = transform.GetChild(1).gameObject;

        //// 初始化时确保一个显示，一个隐藏
        //child1.SetActive(true);
        //child2.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            // 时间到，切换状态
            isChild1Active = !isChild1Active;

            if (level1Glass  && !level1Glass.isClicked )
            {
                //child1.SetActive(isChild1Active);
                //child1.transform.position = level1Glass.transform.position + new Vector3(xoffset, yoffset, 0);
                //child2.SetActive(!isChild1Active);
                //child2.transform.position = level1Glass.transform.position + new Vector3(xoffset, yoffset, 0);
            }
            else 
            {
                if (index < 3)
                {
                    EventManager.Instance.TriggerEvent<int>(GameEvent.ShowNewScrew, index+1);
                }
                else
                {
                    Destroy(gameObject);
                    EventManager.Instance.TriggerEvent(GameEvent.ScrewGuiteIsOver);
                }
            }

            timer = 0f; // 重置计时器
        }
    }
}
