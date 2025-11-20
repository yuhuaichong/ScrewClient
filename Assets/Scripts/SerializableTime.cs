using System;
using Newtonsoft.Json;

[Serializable]
public class SerializableTime
{
    [JsonProperty] public long seconds; // 以秒为单位存储 TimeSpan
    [JsonProperty] public string dateTime; // 存储 DateTime 的字符串形式（ISO 8601 格式）

    // 构造函数，接受 TimeSpan 对象
    public SerializableTime(TimeSpan timeSpan)
    {
        seconds = (long)timeSpan.TotalSeconds;
        dateTime = null; // 不需要存储具体 DateTime
    }

    // 构造函数，接受 DateTime 对象
    public SerializableTime(DateTime dateTime)
    {
        seconds = 0; // 不需要存储具体秒数
        this.dateTime = dateTime.ToString("o"); // ISO 8601 格式
    }

    // 用于 JSON 反序列化的构造函数
    [JsonConstructor]
    public SerializableTime(long seconds, string dateTime)
    {
        this.seconds = seconds;
        this.dateTime = dateTime;
    }

    // 将 SerializableTime 转换为 TimeSpan
    public TimeSpan ToTimeSpan()
    {
        return TimeSpan.FromSeconds(seconds);
    }

    // 将 SerializableTime 转换为 DateTime
    public DateTime ToDateTime()
    {
        if (!string.IsNullOrEmpty(dateTime))
        {
            return DateTime.Parse(dateTime); // 从存储的 ISO 8601 格式解析
        }
        else
        {
            return DateTime.Now.AddSeconds(seconds); // 如果只存储秒数，则从当前时间推算
        }
    }

    // 静态方法：从 TimeSpan 创建 SerializableTime
    public static SerializableTime FromTimeSpan(TimeSpan timeSpan)
    {
        return new SerializableTime(timeSpan);
    }

    // 静态方法：从 DateTime 创建 SerializableTime
    public static SerializableTime FromDateTime(DateTime dateTime)
    {
        return new SerializableTime(dateTime);
    }

    // 输出时间的字符串表示
    public override string ToString()
    {
        if (!string.IsNullOrEmpty(dateTime))
        {
            return dateTime; // 如果有 DateTime，则返回 DateTime 的字符串
        }
        else
        {
            return ToTimeSpan().ToString(); // 否则返回 TimeSpan 的字符串
        }
    }
}
