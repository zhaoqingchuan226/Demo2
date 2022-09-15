﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PinYinSpell;
using System.Collections;

[System.Serializable]
public class AllSoundClips
{
    public AudioClip _b;
    public AudioClip _p;
    public AudioClip _m;
    public AudioClip _f;
    public AudioClip _d;
    public AudioClip _t;
    public AudioClip _n;
    public AudioClip _l;
    public AudioClip _g;
    public AudioClip _k;
    public AudioClip _h;
    public AudioClip _j;
    public AudioClip _q;
    public AudioClip _x;
    public AudioClip _zh;
    public AudioClip _ch;
    public AudioClip _sh;
    public AudioClip _r;
    public AudioClip _z;
    public AudioClip _c;
    public AudioClip _s;
    public AudioClip _y;
    public AudioClip _w;
    public AudioClip _a;
    public AudioClip _o;
    public AudioClip _e;
    public AudioClip _i;
    public AudioClip _u;
    public AudioClip _v;
    public AudioClip _ai;
    public AudioClip _ei;
    public AudioClip _ui;
    public AudioClip _ao;
    public AudioClip _ou;
    public AudioClip _iu;
    public AudioClip _ie;
    public AudioClip _ve;
    public AudioClip _er;
    public AudioClip _an;
    public AudioClip _en;
    public AudioClip _in;
    public AudioClip _un;
    public AudioClip _vn;
    public AudioClip _ang;
    public AudioClip _eng;
    public AudioClip _ing;
    public AudioClip _ong;
    public AudioClip __;

    public Dictionary<string, AudioClip> ClipsDict()
    {
        return new Dictionary<string, AudioClip> {
            { "b",_b }, { "p",_p }, { "m",_m }, { "f",_f }, { "d",_d }, { "t",_t },
            { "n",_n }, { "l",_l }, { "g",_g }, { "k",_k }, { "h",_h }, { "j",_j },
            { "q",_q }, { "x",_x }, { "zh",_zh }, { "ch",_ch }, { "sh",_sh }, { "r",_r },
            { "z",_z }, { "c",_c }, { "s",_s }, { "y",_y }, { "w",_w }, { "a",_a },
            { "o",_o }, { "e",_e }, { "i",_i }, { "u",_u }, { "v",_v }, { "ai",_ai },
            { "ei",_ei }, { "ui",_ui }, { "ao",_ao }, { "ou",_ou }, { "iu",_iu }, { "ie",_ie },
            { "ve",_ve }, { "er",_er }, { "an",_an }, { "en",_en }, { "in",_in }, { "un",_un },
            { "vn",_vn }, { "ang",_ang }, { "eng",_eng }, { "ing",_ing }, { "ong",_ong }, {"_", __}
        };
    }
}

public class PronounceCore : MonoSingleton<PronounceCore>
{
    // AudioSource audioSource;

    string[] syllables = { "a", "ai", "an", "ang", "ao", "b", "c", "ch", "d", "e"
            , "ei", "en", "eng", "er", "f", "g", "h", "i", "ie", "in", "ing", "iu", "j"
            , "k", "l", "m", "n", "o", "ong", "ou", "p", "q", "r", "s", "sh", "t"
            , "u", "ui", "un", "v", "ve", "vn", "w", "x", "y", "z", "zh", "_", };

    Dictionary<string, AudioClip> clips;
    Coroutine lastCo;

    public string audioPath = "PinYinAudio";
    [Tooltip("每个音节开头去掉一部分")]
    [Range(0, 0.5f)]
    public float trimBegin = 0;
    [Tooltip("每个音节结尾去掉一部分")]
    [Range(0.5f, 1f)]
    public float trimEnd = 1;

    public AllSoundClips allClips;

    void Start()
    {
        // audioSource = GetComponent<AudioSource>();
        clips = allClips.ClipsDict();
    }

    public string ConvertPinYin(string chinese)
    {
        string s = Spell.PreProcess(chinese);
        string ret = Spell.MakePinYin(s, SpellOptions.AddSpace | SpellOptions.EnableUnicodeLetter);
        return ret;
    }

    public void Speak(string pinyin, AudioSource audioSource)
    {
        pinyin = pinyin.Trim();
        string[] ss = pinyin.Split();
        if (lastCo != null)
        {
            StopCoroutine(lastCo);
        }
        lastCo = StartCoroutine(CoSpeak(ss, audioSource));
    }

    float oneWordTime = 0.06f;
    List<AudioSource> audioSources = new List<AudioSource>();
    IEnumerator CoSpeak(string[] source, AudioSource audioSource)//核心函数
    {
        audioSources.Add(audioSource);
        foreach (string p in source)
        {
            if (!clips.ContainsKey(p) || clips[p] == null)
            {
                // Debug.Log("跳过未知音节：" + p + ";");
                continue;
            }
            AudioClip clip = clips[p];


            audioSource.clip = clip;
            audioSource.time = trimBegin * oneWordTime;// clip.length *
            audioSource.Play();





            yield return new WaitForSeconds(Mathf.Max(oneWordTime * trimEnd - audioSource.time, 0));



            if (audioSource.transform)
            {
                audioSource.Stop();
                audioSources.Remove(audioSource);
            }


            yield return new WaitForSeconds(Mathf.Max(oneWordTime - oneWordTime * trimEnd + audioSource.time, 0));
            // clip.length /1.8f * trimEnd - audioSource.time
        }
    }

    public void StopAllAudio()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.Stop();
        }
        audioSources.Clear();
        StopAllCoroutines();
    }

    public Dialog dialog;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.J))
    //     {
    //         Test();
    //     }
    // }
    // void Test()
    // {
    //     dialog.SetDiaglog("搞你搞你    ");
    // }
}
