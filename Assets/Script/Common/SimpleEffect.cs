using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleEffect : MonoBehaviour
{
    //UI和普通的区别
    //1、Text不同
    //2、普通用材质，UI用image

    // public bool isUI = false;
    // public Image img;
    float timer_Bebuffed;
    float timer_Buff;
    float timer_Des;
    float timer_Bounce;

    public float scaleSize = 1.3f;
    public float scaleTime = 0.75f;
    Vector3 originLocalScale;

    // [HideInInspector] public TextMeshPro tmp;
    TextMeshProUGUI tmp_UI;
    // Color originTmpColor;

    [HideInInspector] public bool isBebuffed = false;
    [HideInInspector] public bool isBuff = false;
    [HideInInspector] public bool isDes = false;

    public GameObject Card_Fly;
    public AnimationCurve anic;
    public GameObject connectBuff;
    public GameObject L;
    public Color buffColor;

    //FX
    public GameObject Bebuffed_FX;
    public GameObject Buff_FX;
    public GameObject Instantiate_FX;
    public GameObject Destory_FX_Prefab;
    List<GameObject> Destory_FXs = new List<GameObject>();
    bool isDesAudioPlayed = false;
    Dictionary<GameObject, Transform> destory_FX_DestTrans_Dic = new Dictionary<GameObject, Transform>();//每一个目标点对应一个毁灭FX
    public Transform Des_StartPos;//发射毁灭FX的起点
    public Transform Des_EndPos;//被FX特效毁灭的终点
    public GameObject TD_FX;
    float timer_TD;
    [HideInInspector] public bool isTD = false;
    public GameObject AIDefFont;//AI的反制提示
    float timer_AIDef;
    [HideInInspector] public bool isAIDef = false;
    public AnimationCurve anic_AIDef;

    public GameObject Triplet_FX;


    private void Start()
    {
        if (connectBuff != null)
        {
            connectBuff.SetActive(false);
        }

        if (L != null)
        {
            L.SetActive(false);
        }
        if (Bebuffed_FX != null)
        {
            Bebuffed_FX.SetActive(false);
        }
        if (Buff_FX != null)
        {
            Buff_FX.SetActive(false);
        }
        if (Instantiate_FX != null)
        {
            Instantiate_FX.SetActive(false);
        }

        originLocalScale = transform.localScale;
        // if (isUI)
        // {
        //     tmp_UI = GetComponentInChildren<TextMeshProUGUI>();
        // }
        // else
        // {
        // tmp = GetComponentInChildren<TextMeshPro>();
        // }
        // if (isUI)
        // {
        //     originMatColor = img.color;
        // }
        // else
        // {


        // }


        // if (isUI)
        // {
        //     originTmpColor = tmp_UI.color;
        // }
        // else
        // {
        // originTmpColor = tmp.color;
        // }


    }
    public IEnumerator BeBuffed()//被buff会变大
    {
        while (true)
        {
            if (timer_Bebuffed > scaleTime)
            {
                timer_Bebuffed = 0;
                gameObject.transform.localScale = originLocalScale;
                isBebuffed = false;
                Bebuffed_FX.SetActive(false);
                yield break;
            }
            if (isBebuffed == false)//第一次
            {
                Bebuffed_FX.SetActive(true);
                RestartParticleSystem(Bebuffed_FX);
            }
            isBebuffed = true;
            timer_Bebuffed += Time.deltaTime;
            float n = -Mathf.Abs((2 * scaleSize - 2) / scaleTime * timer_Bebuffed + 1 - scaleSize) + scaleSize;

            gameObject.transform.localScale = new Vector3(n * originLocalScale[0], n * originLocalScale[1], n * originLocalScale[2]);
            yield return null;
        }

    }
    public void RestartParticleSystem(GameObject g)
    {
        ParticleSystem p = g.GetComponent<ParticleSystem>();
        p.Pause();
        p.Play();
    }
    public IEnumerator Buff()//buff别人会变色
    {
        while (true)
        {
            if (timer_Buff > scaleTime)
            {
                timer_Buff = 0;
                // if (isUI)
                // {
                //     img.color = originMatColor;
                // }
                // else
                // {

                if (L != null)
                {
                    L.SetActive(false);
                }
                Buff_FX.SetActive(false);
                // }

                isBuff = false;
                yield break;
            }
            if (isBuff == false)//第一次
            {
                AudioManager.Instance.PlayClip("buff0");
                Buff_FX.SetActive(true);
                RestartParticleSystem(Buff_FX);
            }
            isBuff = true;
            timer_Buff += Time.deltaTime;
            float lerpFactor = -Mathf.Abs(2 / scaleTime * timer_Buff - 1) + 1;
            // if (isUI)
            // {
            //     img.color = Color.Lerp(originMatColor, buffColor, lerpFactor);
            // }
            // else
            // {
            if (L != null && !L.activeInHierarchy)
            {
                L.SetActive(true);
            }

            // }

            yield return null;
        }
    }
    public IEnumerator Destroy(Color color, List<Transform> destTranss)//被毁灭字体会变红
    {
        while (true)
        {
            if (timer_Des > scaleTime)
            {
                // foreach (var Destory_FX in Destory_FXs)
                // {
                //     destory_FX_DestTrans_Dic[Destory_FX].GetComponentInParent<SimpleEffect>().tmp.color = color;
                // }

                foreach (var Destory_FX1 in Destory_FXs)
                {
                    Destroy(Destory_FX1);
                }

                Destory_FXs.Clear();
                destory_FX_DestTrans_Dic.Clear();
                timer_Des = 0;
                isDes = false;
                yield break;
            }
            if (isDes == false)//第一次
            {

                foreach (var destTrans in destTranss)
                {
                    GameObject Destory_FX = Instantiate(Destory_FX_Prefab, Des_StartPos);
                    Destory_FXs.Add(Destory_FX);
                    destory_FX_DestTrans_Dic.Add(Destory_FX, destTrans);
                }
            }

            isDes = true;
            timer_Des += Time.deltaTime;

            if (timer_Des >= scaleTime * 0.583f && !isDesAudioPlayed)
            {
                isDesAudioPlayed = true;
                AudioManager.Instance.PlayClip("kill0");
            }

            float lerpFactor_move = timer_Des / (scaleTime * 0.583f);
            // float lerpFactor_Fontcolor = (timer_Des - scaleTime * 0.833f) / (scaleTime * 0.167f);
            foreach (var Destory_FX in Destory_FXs)
            {
                if (timer_Des <= scaleTime * 0.583f)//0.7s
                {
                    Destory_FX.transform.position = Vector3.Lerp(Des_StartPos.position, destory_FX_DestTrans_Dic[Destory_FX].position, lerpFactor_move);

                    Destory_FX.transform.position += new Vector3(0, (-4 * Mathf.Pow((lerpFactor_move - 0.5f), 2) + 1) * 0.1f, 0);
                }
            }



            //FX

            yield return null;
        }
    }

    public IEnumerator TD()
    {
        while (true)
        {
            if (timer_TD > scaleTime)
            {
                yield break;
            }
            if (isTD == false)
            {
                AudioManager.Instance.PlayClip("TD0");
                TD_FX.SetActive(true);
            }
            isTD = true;
            timer_TD += Time.deltaTime;
            float lerpFactor = timer_TD / scaleTime;
            // this.tmp.color = Color.Lerp(originTmpColor, Color.yellow, lerpFactor);
            yield return null;
        }
    }
    public void InstantiateCard(Card kidCard, float delayTime)
    {
        AudioManager.Instance.PlayClip("boom0");
        if (!Instantiate_FX.activeInHierarchy)
        {
            Instantiate_FX.SetActive(true);
            // Instantiate_FX.GetComponent<ParticleSystem>().Play();
        }
        RestartParticleSystem(Instantiate_FX);

        GameObject card_Fly = Instantiate(Card_Fly, Mechanism.Instance.InstantiateCardsGroup.transform);

        card_Fly.GetComponent<CardDisplayPersonalGameAni>().card = kidCard;

        Vector3 startWorldPos = this.transform.position + new Vector3(0, 0.05f, 0);//

        Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f));
        dir.Normalize();
        Vector3 destworldPos1 = startWorldPos + 0.2f * new Vector3(dir.x, dir.y, 0);
        Vector3 destworldPos2 = Mechanism.Instance.FishPool.transform.position;
        Mechanism.Instance.StartCoroutine(card_Fly.GetComponent<CardFly>().Fly(startWorldPos, destworldPos1, destworldPos2, delayTime, 1f, dir));
    }
    public float bounceTime = 1f;
    public IEnumerator Fly()//飞出来抛物线
    {
        Vector3 originPos = Mechanism.Instance.fishMouth.position;
        Vector3 destPos = transform.TransformPoint(Vector3.zero);//本地转世界坐标
        while (true)
        {
            if (timer_Bounce > bounceTime)
            {
                AudioManager.Instance.PlayClip("fall0");
                StartCoroutine(Bounce());
                timer_Bounce = 0;
                yield break;
            }
            timer_Bounce += Time.deltaTime;
            float lerpFactor = timer_Bounce / bounceTime;
            Vector3 pos = Vector3.Lerp(originPos, destPos, lerpFactor);
            float factor2 = anic.Evaluate(lerpFactor);
            pos.y = originPos.y * (1 - factor2) + destPos.y * factor2;
            this.transform.position = pos;
            yield return null;
        }
    }

    public AnimationCurve anic_bounce;
    public IEnumerator Bounce()//跳跃
    {
        float timer = 0f;
        while (true)
        {
            if (timer > 0.3f)
            {
                this.transform.localPosition = Vector3.zero;
                yield break;
            }
            timer += Time.deltaTime;
            float factor = anic_bounce.Evaluate(timer / 0.3f);
            this.transform.localPosition = new Vector3(0, factor, 0) * 0.08f;
            yield return null;
        }
    }

    public void OpenConnectBuff()
    {
        if (connectBuff != null)
        {
            connectBuff.SetActive(true);
        }
    }
    public void CloseConnectBuff()
    {
        if (connectBuff != null)
        {
            connectBuff.SetActive(false);
        }
    }

    public IEnumerator AIDef()
    {
        while (true)
        {
            if (timer_AIDef > scaleTime)
            {
                timer_AIDef = 0;
                isAIDef = false;
                AIDefFont.SetActive(false);
                yield break;
            }
            timer_AIDef += Time.deltaTime;
            if (!isAIDef)
            {
                AIDefFont.SetActive(true);
            }
            isAIDef = true;
            float lerpFactor = timer_AIDef / scaleTime;

            TextMeshPro tmpp = AIDefFont.GetComponent<TextMeshPro>();
            tmpp.color = new Color(1, 0, 0, anic_AIDef.Evaluate(lerpFactor));
            yield return null;
        }
    }

    public void PlayTripletBuff()
    {
        Triplet_FX.SetActive(true);
    }
}
