using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Dialog : MonoBehaviour
{
    public bool isUI = false;

    public TextMeshPro text;
    public TextMeshProUGUI text_UI;
    public GameObject dialog_obj;
    // public float time = 2f;
    public float switchTime = 0.15f;
    Vector3 originScale;
    float eachWordTime = 0.3f;//每个字的时间
    AudioSource audioSource;
    SpeakPronounce sp;



    void Start()
    {
        originScale = dialog_obj.transform.localScale;
        dialog_obj.transform.localScale = Vector3.zero;
        dialog_obj.SetActive(false);

        dialog_obj.TryGetComponent<AudioSource>(out AudioSource au);

        if (au != null)
        {
            audioSource = au;
        }

        dialog_obj.TryGetComponent<SpeakPronounce>(out SpeakPronounce sp1);
        if (sp1 != null)
        {
            sp = sp1;

        }
        if (au != null && sp != null)
        {
            sp.audioSource = au;
            sp.audioSource.volume = 0.4f;
        }
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.J))
    //     {
    //         string s = "......大家开会了，您不急，嘿嘿";
    //         SetDiaglog(s);
    //     }
    //     if (Input.GetKeyDown(KeyCode.K))
    //     {
    //         string s = "大家开会了，您不急，嘿嘿";
    //         SetDiaglog(s);
    //     }

    // }
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.D))
    //     {
    //         SetDiaglog("你好啊帅哥");
    //     }
    //     if (Input.GetKeyDown(KeyCode.F))
    //     {
    //         SetDiaglog("你好");
    //     }
    //     if (Input.GetKeyDown(KeyCode.G))
    //     {
    //         SetDiaglog("听着败类，我们又见面了，虽然上次还是在上次");
    //     }
    // }
    public IEnumerator DialogAni(float time)
    {

        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                dialog_obj.transform.localScale = Vector3.zero;
                if (audioSource != null && audioSource.gameObject.activeInHierarchy)
                {
                    audioSource.Stop();
                }
                yield break;
            }
            if (timer < switchTime)
            {
                dialog_obj.transform.localScale = originScale * timer / switchTime;
            }
            else if (timer > time - switchTime)
            {
                dialog_obj.transform.localScale = originScale * (1 - (timer - time + switchTime) / switchTime);
            }
            else
            {
                dialog_obj.transform.localScale = originScale;
            }

            yield return null;
        }
    }
    public IEnumerator DialogAni_O()
    {

        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;

            if (timer < switchTime)
            {
                dialog_obj.transform.localScale = originScale * timer / switchTime;
            }
            else
            {
                dialog_obj.transform.localScale = originScale;
                yield break;
            }

            yield return null;
        }
    }


    // public IEnumerator DialogAni_C()
    // {

    //     float timer = 0;
    //     while (true)
    //     {
    //         timer += Time.deltaTime;

    //         if (timer < switchTime)
    //         {
    //             dialog_obj.transform.localScale = originScale * (1 - timer / switchTime);
    //         }
    //         else
    //         {
    //             dialog_obj.transform.localScale = Vector3.zero;
    //             yield break;
    //         }

    //         yield return null;
    //     }
    // }



    public void CloseDialog()
    {
        if (audioSource != null && audioSource.gameObject.activeInHierarchy)
        {
            audioSource.Stop();
        }
        StopAllCoroutines();
        dialog_obj.transform.localScale = Vector3.zero;
        dialog_obj.SetActive(false);
    }
    public void SetDiaglog(string s, bool isUnClose = false)//是否不需要关闭 在teachmanager中为true
    {
        if (this.gameObject.activeInHierarchy)
        {
            dialog_obj.SetActive(true);
            if (isUI)
            {
                text_UI.text = s;
            }
            else
            {
                text.text = s;
            }

            float time = s.ToCharArray().Length * eachWordTime;
            time += 2 * switchTime;

            if (sp != null && audioSource != null)
            {
                sp.s_HanZi = s;
                sp.ConvertAndSpeak();
            }

            audioSource.Stop();
            this.StopAllCoroutines();
            if (!isUnClose)
            {
                StartCoroutine(DialogAni(time));
            }
            else
            {
                StartCoroutine(DialogAni_O());
            }

        }
    }

    public void SetDialog_Delay(string s, float time)
    {
        StartCoroutine(Delay(s, time));
    }
    IEnumerator Delay(string s, float time)
    {
        yield return new WaitForSeconds(time);
        SetDiaglog(s);
        yield break;
    }

    // 
    public void Speak(int id, string situation)
    {
        if (situation == "1_3" && FieldManager.Instance.isSpeakIn1_3)
        {
            return;
        }
        if (FieldManager.Instance.cardID_cardDialogs_Dic.ContainsKey(id))
        {

            List<cardDialog> cardDialogs = FieldManager.Instance.cardID_cardDialogs_Dic[id];
            if (cardDialogs.Count > 0)
            {

                for (int i = 0; i < cardDialogs.Count; i++)
                {
                    if (situation == cardDialogs[i].situation)
                    {
                        float r1 = Random.Range(0, 1f);
                        float r2 = Random.Range(0, 1f);
                        if (cardDialogs[i].pro_do > r1)
                        {
                            if (situation == "1_3")
                            {
                                FieldManager.Instance.isSpeakIn1_3 = true;
                            }
                            SetDiaglog(cardDialogs[i].s);
                            if (cardDialogs[i].pro_des > r2)
                            {
                                cardDialogs.Remove(cardDialogs[i]);
                            }
                            break;
                        }

                    }
                }
            }
        }

    }

}
