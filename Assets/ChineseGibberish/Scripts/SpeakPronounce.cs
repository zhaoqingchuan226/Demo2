using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PinYinSpell;

public class SpeakPronounce : MonoBehaviour
{
    // public InputField inputChinese;
    public string s_HanZi = " ";
    public string s_PingYing = " ";
    [HideInInspector]public AudioSource audioSource;


    // void Start()
    // {
    //     audioSource = this.GetComponent<AudioSource>();
    // }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.J))
    //     {
    //         ConvertAndSpeak();
    //     }
    // }
    // 汉字转拼音
    public void OnConvert()
    {
        s_PingYing = PronounceCore.Instance.ConvertPinYin(s_HanZi);
    }

    // 播放拼音
    public void OnSpeak()
    {
        PronounceCore.Instance.Speak(s_PingYing,audioSource);
    }

    // 转换并播放
    public void ConvertAndSpeak()
    {
        string s_PingYing = PronounceCore.Instance.ConvertPinYin(s_HanZi);
        PronounceCore.Instance.Speak(s_PingYing,audioSource);
    }
}
