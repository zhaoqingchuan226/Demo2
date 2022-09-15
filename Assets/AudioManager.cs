using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoSingleton<AudioManager>
{
    [HideInInspector] public AudioListener AudioListener;
    [HideInInspector] public Slider AudioSlider;
    public AudioSource audioSource_bgm;
    public AudioSource audioSource_game;
    // [HideInInspector] public AudioClip BGM0;
    // [HideInInspector] public AudioClip button0;
    // [HideInInspector] public AudioClip button1;
    [HideInInspector] public Dictionary<string, AudioClip> Audio_Dic = new Dictionary<string, AudioClip>();
    float timer;
    // bool isTwo = false;
    void Awake()
    {
        FindAllAudioResource();
        AudioListener = Camera.main.GetComponent<AudioListener>();
        audioSource_bgm.volume = 0.4f;
        PlayClip("BGM1", "BGM");
    }
    void LoadAudio(string AudioName)
    {
        Audio_Dic.Add(AudioName, Resources.Load<AudioClip>(AudioName));
    }
    void FindAllAudioResource()
    {
        LoadAudio("BGM0");
        LoadAudio("BGM1");
        LoadAudio("BGM2");
        LoadAudio("button0");
        LoadAudio("button1");
        LoadAudio("button2");
        LoadAudio("create0");
        LoadAudio("create1");
        LoadAudio("recover0");
        LoadAudio("buff0");
        LoadAudio("boom0");
        LoadAudio("kill0");
        LoadAudio("TD0");
        LoadAudio("magic0");
        LoadAudio("fall0");
    }

    void Update()
    {

    }
    public void QuickPlayClip(string clip)//给按钮用的
    {
        // audioSource_game.volume = volu;
        // audioSource_game.loop = isLoop;
        audioSource_game.PlayOneShot(Audio_Dic[clip]);
    }

    public void PlayClip(string clip, string audioSource = "game")
    {
        AudioSource aus = new AudioSource();
        if (audioSource == "game")
        {
            aus = audioSource_game;
            aus.PlayOneShot(Audio_Dic[clip]);
        }
        else if (audioSource == "BGM")
        {
            aus = audioSource_bgm;
            aus.Stop();
            aus.clip = Audio_Dic[clip];
            aus.Play();
        }
        // aus.volume = volu;
        // aus.loop = isLoop;

        // switch (clip)
        // {
        //     case "BGM0":

        //         break;
        //     case "button0":
        //         audioSource.PlayOneShot(button0);
        //         break;
        //     case "button1":
        //         audioSource.PlayOneShot(button1);
        //         break;
        //     default:
        //         break;
        // }
    }
    public void PlayBGM2()
    {
        audioSource_bgm.Stop();
        audioSource_bgm.volume = 0.4f;
        PlayClip("BGM2", "BGM");
    }

}
