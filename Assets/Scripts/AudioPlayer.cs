using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip positive;
    public AudioClip negative;
    public AudioClip reach;
    public AudioClip gameover;
    public AudioClip win;
    public AudioClip dead;
    
    private AudioSource audioSource;

    public static AudioPlayer instance;
    private AudioSource bgmPlayer;


    //实现全局单例类
    private void Awake() {
        
        if(instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bgmPlayer = transform.GetChild(0).GetComponent<AudioSource>();
    }

    /// <summary> 
    ///  播放音效
    ///  hit, walk, gameover, win
    /// </summary>
    public void Play(string clipName)
    {
        AudioClip tmp = null;
        switch (clipName)
        {
            case "negative":
                tmp = negative;
                break;
            case "reach":
                tmp = reach;
                break;
            case "gameover":
                tmp = gameover;
                break;
            case "win":
                tmp = win;
                break;
            case "dead":
                tmp = dead;
                break;
            case "positive":
                tmp = positive;
                break;
            default:
                tmp = reach;
                break;
        }
        audioSource.clip = tmp;
        audioSource.Play();
    }

    public void SetBgmPlaying(bool play) 
    {
        if (play)
        {
            bgmPlayer.volume = 0.5f;
        }
        else
        {
            bgmPlayer.volume = 0f;
        }
        
    }

    public void ReplayBgm()
    {
        bgmPlayer.Stop();
        bgmPlayer.Play();
        SetBgmPlaying(true);
    }
}
