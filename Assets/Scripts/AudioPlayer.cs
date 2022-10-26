using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public List<AudioClip> effects;
    public List<AudioClip> bgms;
    public AudioSource bgmPlayer;
    public AudioSource soundEffectPlayer;
    public static AudioPlayer instance = null;


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
        soundEffectPlayer = transform.GetChild(1).GetComponent<AudioSource>();
        bgmPlayer = transform.GetChild(0).GetComponent<AudioSource>();

        Play("bgm", "bgm");
    }

    ///<summary> type: "effect" / "bgm"</summary>
    public void Play(string name, string type = "effect")
    {
        if (type == "effect")
            foreach (AudioClip ac in effects)
            {
                if (name.ToUpper() == ac.name.ToUpper())
                {
                    soundEffectPlayer.clip = ac;
                    soundEffectPlayer.Play();
                    Debug.Log("Sound Effect: " + name);
                    break;
                }
            }
        else if (type == "bgm")
            foreach (AudioClip ac in bgms)
            {
                if (name.ToUpper() ==  ac.name.ToUpper())
                {
                    bgmPlayer.loop = true;
                    bgmPlayer.clip = ac;
                    bgmPlayer.Play();
                    Debug.Log("BGM: " + name);
                    break;
                }
            }
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
