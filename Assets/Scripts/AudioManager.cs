using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Sound
{
    public string name; // 音效的名字，用于标识
    public AudioClip clip; // 对应的音频文件
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    public AudioClip bgmClip; // 背景音乐文件
    private AudioSource bgmSource; // 背景音乐的 AudioSource
    public float bgmVolume = 1f; // 背景音乐音量

    [Header("Sound Effects")]
    public List<Sound> soundEffects; // 游戏音效列表
    private Dictionary<string, AudioClip> sfxDictionary; // 音效字典
    private List<AudioSource> sfxSources = new List<AudioSource>(); // 音效池
    public float sfxVolume = 1f; // 游戏音效音量
    public int sfxPoolSize = 8; // 音效池大小

    private bool canPlaySFX = true; // 控制音效是否能播放

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitAudioSources();
        InitSoundEffects();

        // 自动播放背景音乐（如果有设置）
        if (bgmClip != null)
        {
            PlayBGM();
        }
        EventManager.Instance.RegisterEvent(GameEvent.OpenClikAudio, OpenClikAudio);
    }
    private void OnDestroy()
    {
        EventManager.Instance.UnregisterEvent(GameEvent.OpenClikAudio, OpenClikAudio);
    }
    private void OpenClikAudio()
    {
        PlaySFX(sfxDictionary["Click"]);
    }

    private void InitAudioSources()
    {
        // 初始化背景音乐 AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        // 初始化音效池
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
            sfxSources.Add(sfxSource);
        }
    }

    private void InitSoundEffects()
    {
        // 将音效列表转换为字典
        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (var sound in soundEffects)
        {
            if (!sfxDictionary.ContainsKey(sound.name))
            {
                sfxDictionary.Add(sound.name, sound.clip);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound name found: {sound.name}");
            }
        }
    }

    #region BGM Methods
    public void PlayBGM()
    {
        if (bgmClip == null)
        {
            Debug.LogWarning("No background music clip assigned!");
            return;
        }

        bgmSource.clip = bgmClip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
    #endregion

    #region SFX Methods
    public void PlaySFX(string name)
    {
        // 如果音效被禁用，则不播放音效
        if (!canPlaySFX) return;
        if(name== "Click")//按钮点击音效，方式替换
        {
            return;
        }

        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            foreach (var sfxSource in sfxSources)
            {
                if (!sfxSource.isPlaying)
                {
                    //Debug.LogError($"传入的名字{name},播放的音效名字{clip.name}");
                    sfxSource.clip = clip;
                    sfxSource.volume = sfxVolume;
                    sfxSource.Play();
                    return;
                }
            }

            Debug.LogWarning("All SFX sources are busy. Consider increasing the pool size.");
        }
        else
        {
            Debug.LogWarning($"Sound effect not found: {name}");
        }
    }
    public void PlaySFX(AudioClip audioClip)
    {
        // 如果音效被禁用，则不播放音效
        if (!canPlaySFX) return;

            foreach (var sfxSource in sfxSources)
            {
                if (!sfxSource.isPlaying)
                {
                    //Debug.LogError($"传入的名字{name},播放的音效名字{clip.name}");
                    sfxSource.clip = audioClip;
                    sfxSource.volume = sfxVolume;
                    sfxSource.Play();
                    return;
                }
            }
    }


    public void StopAllSFX()
    {
        foreach (var sfxSource in sfxSources)
        {
            if (sfxSource.isPlaying)
            {
                sfxSource.Stop();
            }
        }
    }

    // 禁用所有音效
    public void DisableSFX()
    {
        canPlaySFX = false;
    }

    // 启用所有音效
    public void EnableSFX()
    {
        canPlaySFX = true;
    }
    #endregion

    #region Volume Control
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        foreach (var sfxSource in sfxSources)
        {
            sfxSource.volume = sfxVolume;
        }
    }
    #endregion

    //[ContextMenu("删除存储的游戏数据")] // 右键脚本时显示菜单项
    //public void DeleteData()
    //{
    //    JsonDataManager.DeleteData();
    //}
}
