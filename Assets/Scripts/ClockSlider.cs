using UnityEngine;
using UnityEngine.UI;
using DafultScript;
public class ClockSlider : MonoBehaviour
{
    private Text countdownText; // 用于显示倒计时的文本
    private Slider countdownSlider; // 用于显示倒计时进度
    [SerializeField]private int minutes = 0; // 倒计时的分钟
    [SerializeField]private int seconds = 30; // 倒计时的秒数

    private float totalTime; // 总时间（秒）
    private float currentTime; // 当前剩余时间

    private void Awake()
    {
        countdownText = transform.Find("Fill Area/Text").GetComponent<Text>();
        countdownSlider = GetComponent<Slider>();
    }

    public void SetTime(int m, int s)
    {
        minutes = m;
        seconds = s;
        totalTime = minutes * 60 + seconds;
        currentTime = totalTime;

        // 初始化 Slider 最大值和初始值
        countdownSlider.maxValue = totalTime;
        countdownSlider.value = totalTime;

        int minutesLeft = Mathf.FloorToInt(currentTime / 60);
        int secondsLeft = Mathf.FloorToInt(currentTime % 60);

        countdownText.text = $"{minutesLeft:00}:{secondsLeft:00}";
    }

    void Update()
    {
        if (currentTime > 0 && GameManager.Instance._GameState == GameState.Start)
        {
            // 减少时间
            currentTime -= Time.deltaTime;

            // 更新倒计时文本
            UpdateCountdownText();

            // 更新 Slider 进度
            countdownSlider.value = currentTime;

            // 确保时间不为负值
            if (currentTime <= 0)
            { 
                currentTime = 0;
                //倒计时结束游戏结束
                //UIManager.Instance.ShowUI<TimeUpUI>();
                //UIManager.Instance.ShowUI<LoseUI>();
            }
        }
    }

    /// <summary>
    /// 更新倒计时文本
    /// </summary>
    void UpdateCountdownText()
    {
        int minutesLeft = Mathf.FloorToInt(currentTime / 60);
        int secondsLeft = Mathf.FloorToInt(currentTime % 60);

        countdownText.text = $"{minutesLeft:00}:{secondsLeft:00}";
    }
}
