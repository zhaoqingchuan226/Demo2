using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]

public class LACControl : MonoSingleton<LACControl>
{
    [Range(0, 100)]
    public float weight_P = 0;
    [Range(0, 100)]
    public float weight_S = 0;
    [Range(0, 100)]
    public float weight_K = 0;

    public SkinnedMeshRenderer P;
    public SkinnedMeshRenderer S;
    public SkinnedMeshRenderer K;
    public float setWeightTime = 1.2f;
    public float SetButtTime = 0.8f;
    public AnimationCurve ac_larger;
    public AnimationCurve ac_smaller;
    // Start is called before the first frame update4
    public AnimationCurve ac_Emis;
    public AnimationCurve ac_Butt;

    //硬币控制
    public GameObject goldCoin;
    public GameObject silverCoin;
    public GameObject copperCoin;


    [HideInInspector] public List<GameObject> goldCoins = new List<GameObject>();
    [HideInInspector] public List<GameObject> silverCoins = new List<GameObject>();
    [HideInInspector] public List<GameObject> copperCoins = new List<GameObject>();
    public Transform moneyPool;

    public GameObject Face;
    Material FaceMat;

    public GameObject moneyBowl;
    public Vector3 originPos_moneyBowl;
    public Vector3 originPos_HolidayStore;
    public Dialog dialog;
    void Start()
    {
        P.SetBlendShapeWeight(0, weight_P);
        S.SetBlendShapeWeight(0, weight_S);
        K.SetBlendShapeWeight(0, weight_K);
        FaceMat = Face.GetComponent<MeshRenderer>().sharedMaterial;
        dialog = this.gameObject.GetComponent<Dialog>();
        SetFace();
        originPos_moneyBowl = moneyBowl.transform.position;
    }

    public void SetMoneyBowlPos(string s)
    {
        if (s == "origin")
        {
            moneyBowl.transform.position = originPos_moneyBowl;
        }
        else if (s == "holidayStore")
        {
            moneyBowl.transform.position = originPos_HolidayStore;
        }
    }

    // void Update()
    // {
    //     // if (Input.GetKeyDown(KeyCode.V))
    //     // {
    //     //     StartCoroutine(Butt(1000));
    //     // }


    //     // if (Input.GetKeyDown(KeyCode.J))
    //     // {
    //     //     StartCoroutine(SetWeight(P, 30));
    //     // }
    //     // if (Input.GetKeyDown(KeyCode.K))
    //     // {
    //     //     StartCoroutine(SetWeight(P, -30));
    //     // }

    // }


    public IEnumerator SetWeight(string s, float deltaWeight, float timer = 0)
    {

        SkinnedMeshRenderer sm = null;
        switch (s)
        {
            case "P":
                sm = P;
                break;
            case "S":
                sm = S;
                break;
            case "K":
                sm = K;
                break;
            default:
                break;
        }

        if (sm == null)//W的动画不在这里，所以如果传入W，则退出
        {
            yield break;
        }

        float originWeight = sm.GetBlendShapeWeight(0);
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > setWeightTime)
            {
                timer = 0;
                sm.SetBlendShapeWeight(0, deltaWeight + originWeight);
                sm.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_EmisIntensity", 0);
                yield break;
            }
            float factor = 0;
            if (deltaWeight > 0)
            {
                factor = ac_larger.Evaluate(timer / setWeightTime);
            }
            else
            {
                factor = ac_smaller.Evaluate(timer / setWeightTime);
            }
            float weight = Mathf.Lerp(originWeight, deltaWeight + originWeight, factor);
            sm.SetBlendShapeWeight(0, weight);
            // currentAdditive.transform.localScale = new Vector3(factor, factor, factor) * scaleSize;
            Material mat = sm.GetComponent<SkinnedMeshRenderer>().material;
            mat.SetFloat("_EmisIntensity", ac_Emis.Evaluate(timer / setWeightTime));
            float crackFactor = Mathf.Lerp(30, 0, Mathf.Clamp01((deltaWeight + originWeight) / 50f));   //crackFactor=20,weight=0;crackFactor=0,weight=50
            mat.SetFloat("_DetailNormalScale", crackFactor);

            // if (s == "P" || s == "S")
            // {
            //     FaceMat.SetFloat("_Health",(deltaWeight + originWeight)/100f);
            // }

            yield return null;
        }
    }

    public void SetFace(bool isDialog = false)
    {
        // Debug.Log(PlayerData.Instance.physicalHealth);
        float p_health = Mathf.Max(0, (PlayerData.Instance.physicalHealth / (float)PlayerData.Instance.physicalHealthMax));
        float s_health = Mathf.Max(0, (PlayerData.Instance.spiritualHealth / (float)PlayerData.Instance.spiritualHealthMax));
        // Debug.Log(p_health);
        // Debug.Log(s_health);
        float health = Mathf.Min(p_health, s_health);
        health = Mathf.Clamp(health, 0.25f, 0.75f);
        health = health * 2 - 0.5f;
        FaceMat.SetFloat("_Health", health);
        if (!isDialog)
        {
            return;
        }
        float r = Random.Range(0, 1f);
        if (p_health <= 0.25f && s_health <= 0.25f)//濒死提示
        {

            if (r < 0.3f)
            {
                dialog.SetDiaglog("身体快要完全不行了！");
            }
            else if (r < 0.6f)
            {
                dialog.SetDiaglog("怎么摊上你这个不注意健康的主！");
            }
            else
            {
                dialog.SetDiaglog("要系他咧，册那！");
            }
        }
        else if (p_health <= 0.25f)
        {

            if (r < 0.5f)
            {
                dialog.SetDiaglog("体力极差，需要锻炼！");
            }
            else
            {
                dialog.SetDiaglog("男人的右手那么细，多半是废了！");
            }
        }
        else if (s_health <= 0.25f)
        {

            if (r < 0.5f)
            {
                dialog.SetDiaglog("精神极度疲惫！");
            }
            else
            {
                dialog.SetDiaglog("我快要失去理智了！");
            }
        }
        else if (p_health <= 0.5f && s_health <= 0.5f)
        {

            if (r < 0.3f)
            {
                dialog.SetDiaglog("注意身体健康");
            }
            else if (r < 0.6f)
            {
                dialog.SetDiaglog("身体处于亚健康状态");
            }
            else
            {
                dialog.SetDiaglog("不是很舒服");
            }
        }
        else if (p_health <= 0.5f)
        {
            if (r < 0.5f)
            {
                dialog.SetDiaglog("肌肉酸痛");
            }
            else
            {
                dialog.SetDiaglog("身体有点累");
            }
        }
        else if (s_health <= 0.5f)
        {

            if (r < 0.5f)
            {
                dialog.SetDiaglog("有点困了");
            }
            else
            {
                dialog.SetDiaglog("脑阔疼");
            }
        }
        else if (p_health == 1 && s_health == 1)
        {
            if (r < 0.1f)
            {
                dialog.SetDiaglog("健康到了极点");
            }
            else if (r < 0.2f)
            {
                dialog.SetDiaglog("状态棒极了，真不知道当时为什么打不过一条蛇");
            }
            else if (r < 0.3f)
            {
                dialog.SetDiaglog("你让拉奥孔精神焕发");
            }
        }
        else//体力和精力都正常
        {
            if (r < 0.1f)
            {
                dialog.SetDiaglog("保持这个健康状态");
            }
            else if (r < 0.2f)
            {
                dialog.SetDiaglog("拉奥孔是最健壮的男人");
            }
            else if (r < 0.3f)
            {
                dialog.SetDiaglog("身体状态不错");
            }
        }



    }

    public IEnumerator Butt(int n, bool isAudio = true)//屁股拉薪水
    {
        float timer = 0;
        Animator a = this.GetComponent<Animator>();
        a.SetTrigger("go");
        bool isSalary = false;
        while (true)
        {
            if (timer > 1f)
            {
                a.SetTrigger("back");
                yield break;
            }
            if (timer > 0.4f)
            {
                if (!isSalary)
                {
                    isSalary = true;
                    StartCoroutine(GenerateCoin(n, isAudio));
                }
            }
            timer += Time.deltaTime;
            Material mat = K.GetComponent<SkinnedMeshRenderer>().material;
            mat.SetFloat("_VertexOffset", ac_Butt.Evaluate(timer / SetButtTime));
            yield return null;
        }
    }

    public IEnumerator GenerateCoin(int moneyAmount, bool isAudio = true)
    {
        float timer = 0;
        float interval = 0.1f;
        if (isAudio)
        {
            AudioManager.Instance.PlayClip("Coin_drop");
        }

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > interval)
            {
                timer = 0;
                if (moneyAmount >= 10000)
                {
                    moneyAmount -= 10000;
                    GameObject coin = Instantiate(goldCoin, moneyPool);
                    RandomPos(coin);
                    goldCoins.Add(coin);
                    PlayerData.Instance.ChangeMoney(10000);
                }
                else if (moneyAmount >= 5000)
                {
                    moneyAmount -= 5000;
                    GameObject coin = Instantiate(silverCoin, moneyPool);

                    RandomPos(coin);
                    silverCoins.Add(coin);
                    PlayerData.Instance.ChangeMoney(5000);
                }
                else if (moneyAmount >= 1000)
                {
                    moneyAmount -= 1000;
                    GameObject coin = Instantiate(copperCoin, moneyPool);

                    RandomPos(coin);
                    copperCoins.Add(coin);
                    PlayerData.Instance.ChangeMoney(1000);
                }
            }
            if (moneyAmount < 1000)
            {
                yield break;
            }
            yield return null;
        }
    }


    // public int money;//一次给的钱


    public int price = 1000;
    //先尝试全部用铜币付

    public void ReduceCoin(int price)
    {
        if (price <= PlayerData.Instance.playerMoney)//铜币够，全部使用铜币支付
        {
            if (copperCoins.Count * 1000 >= price)
            {
                DestroyCoins("copper", price / 1000);
            }
            else//铜币不够了
            {
                if (copperCoins.Count * 1000 + silverCoins.Count * 5000 >= price)//铜币加银币够，那就先用银币支付，再找钱
                {
                    int n = price / 5000;
                    int r = price % 5000;
                    if (r == 0)//银币刚刚可以支付
                    {
                        DestroyCoins("silver", n);
                    }
                    else//找钱
                    {
                        DestroyCoins("silver", n + 1);
                        StartCoroutine(GenerateCoin(5000 - r,false));
                    }
                }
                else//铜币加银币不够，那就先用金币支付，再找钱
                {
                    int n = price / 10000;
                    int r = price % 10000;
                    if (r == 0)//金币刚刚可以支付
                    {
                        DestroyCoins("gold", n);
                    }
                    else//找钱
                    {
                        DestroyCoins("gold", n + 1);
                        StartCoroutine(GenerateCoin(10000 - r,false));
                    }
                }
            }
        }

    }

    void RandomPos(GameObject g)
    {
        g.transform.position += new Vector3(Random.Range(-0.03f, 0.03f), 0, Random.Range(-0.03f, 0.03f));
        g.transform.eulerAngles += new Vector3(Random.Range(-30, 30f), Random.Range(-30, 30f), Random.Range(-30, 30f));
    }

    void DestroyCoins(string s, int n)
    {
        List<GameObject> coins = new List<GameObject>();
        int m = 0;
        switch (s)
        {
            case "gold":
                coins = goldCoins;
                m = 10000;
                break;
            case "silver":
                coins = silverCoins;
                m = 5000;
                break;
            case "copper":
                coins = copperCoins;
                m = 1000;
                break;
            default:
                break;
        }
        for (int i = 0; i < n; i++)
        {
            Destroy(coins[0]);
            coins.RemoveAt(0);
            PlayerData.Instance.ChangeMoney(-m);
        }
    }

    public IEnumerator moneyBowlMove()
    {
        float timer = 0;
        Vector3 originPos = moneyBowl.transform.position + new Vector3(0, 3, 0);
        Vector3 destPos = moneyBowl.transform.position;
        while (true)
        {
            if (timer > 0.7f)
            {
                moneyBowl.transform.position = destPos;
                yield break;
            }
            timer += Time.deltaTime;
            float lerpFactor = timer / 0.7f;
            moneyBowl.transform.position = Vector3.Lerp(originPos, destPos, lerpFactor);
            yield return null;
        }
    }

    public void O_C_AllMeshes(bool b)
    {
        Face.SetActive(b);
        moneyBowl.SetActive(b);
        P.gameObject.SetActive(b);
        S.gameObject.SetActive(b);
        K.gameObject.SetActive(b);
        LAC_Tree_Control.Instance.O_C_AllMeshes(b);
    }
}
