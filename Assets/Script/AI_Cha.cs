using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum AI_Cha_Type
{
    AI,
    Leader,
    Player
}

//掌管所有AI 主角 leader的动画和对话
public class AI_Cha : MonoBehaviour
{
    //全局
    public AI_Cha_Type aI_Cha_Type = AI_Cha_Type.AI;
    public int id;//0猪 1鹅 2狗 3马 //4领导
    // public bool isLeader = false;//是不是领导
    public string Name;
    //储存一切AI的数据
    [HideInInspector] public AIData AIData = null;

    [Space]
    [Space]
    [Space]
    //chess模式下的小布偶
    public GameObject Mini;//mini的总物体（空，用来存放和mini有关的一切物体）
    [HideInInspector] public GameObject AI_Mini = null;//用来尔虞我诈的小布偶模型
    public AnimationCurve anic_MiniMove;//小布偶的起落曲线
    public GameObject Box;//装小布偶的盒子模型
    [HideInInspector] Animator animator_Box;
    [HideInInspector] public Dialog dialog_Mini;

    //chess模式下的送屎
    public SimpleEffect se;

    [Space]
    [Space]
    [Space]
    //周末会议下的小布偶
    public GameObject Mini_Weekend;//总的外部
    public GameObject AI_Weekend;//周末会议的人物模型
    [HideInInspector] Animator animator_Weekend;
    public GameObject KPIBar;//周末会议的KPI条长度和排名数字
    public TextMeshPro KPIBar_Text;
    public Material healthMat;//面部红润和发黑
    public GameObject jewel;//象征W的宝石
    Material workAbilityMat;//工作能力用宝石表示，到时候调节颜色
    [HideInInspector] public Dialog dialog_Mini_Weekend;
    public ParticleSystem pm_upgrade;
    [HideInInspector] public Animator animator_Mini_Weekend;

    [Space]
    [Space]
    [Space]
    //Story场景中的大人物
    public GameObject Big;//Big的总物体（空，用来存放和Big有关的一切物体）
    public GameObject AI_Big;//人物模型
    [HideInInspector] Animator animator_Big;
    public Dialog dialog_Big;

    void Awake()
    {
        // animator_Cha = AI_Big.gameObject.GetComponent<Animator>();
        // animator_Mini = AI_Mini.gameObject.GetComponent<Animator>();


        // Mini_Weekend.TryGetComponent<Dialog>(out dialog_Mini_Weekend);
        if (aI_Cha_Type == AI_Cha_Type.AI)
        {
            dialog_Mini_Weekend = Mini_Weekend.GetComponent<Dialog>();
            workAbilityMat = jewel.GetComponent<MeshRenderer>().material;
            animator_Mini_Weekend = AI_Weekend.GetComponent<Animator>();
            animator_Box = Box.GetComponent<Animator>();
            dialog_Mini = Mini.GetComponent<Dialog>();
            animator_Big = AI_Big.GetComponent<Animator>();
            StartCoroutine(DelayWork());
        }
        else if (aI_Cha_Type == AI_Cha_Type.Player)
        {
            workAbilityMat = jewel.GetComponent<MeshRenderer>().material;
        }
        else if (aI_Cha_Type == AI_Cha_Type.Leader)
        {
            dialog_Mini_Weekend = Mini_Weekend.GetComponent<Dialog>();
            animator_Box = Box.GetComponent<Animator>();
            dialog_Mini = Mini.GetComponent<Dialog>();
        }
    }
    Coroutine last_co = null;
    //!!!!!!!!!!!!!!Chess!!!!!!!!!!!!!!
    public void Mini_BeDebuffed(int id1)//布偶被debuff的动画 //id是卡牌的id
    {
        // Debug.Log(this.AIData.name + this.Name + "卡牌" + id1.ToString());
        if (AI_Mini != null)
        {
            Destroy(AI_Mini);
        }
        AI_Mini = null;


        //每个特定的动画
        if (aI_Cha_Type == AI_Cha_Type.Leader)
        {
            GameObject g = AIMechanism.Instance.SearchAIPoses(id1, true);
            if (g == null)
            {
                return;
            }
            else
            {
                AI_Mini = Instantiate(g, Mini.transform);
                AI_Mini.transform.localPosition = new Vector3(0, -0.05f, 0.018f);
                animator_Box.SetTrigger("Open");
            }

        }
        else if (aI_Cha_Type == AI_Cha_Type.AI)
        {
            AI_Mini = Instantiate(AIMechanism.Instance.SearchAIPoses(id1), Mini.transform);
            AI_Mini.transform.localPosition = new Vector3(0, -0.05f, 0.018f);
            animator_Box.SetTrigger("Open");
        }

        DelaySpeak(id1);

        //确定头

        if (aI_Cha_Type == AI_Cha_Type.AI)
        {
            SetHead();
        }

        //升起来落下去的协程动画
        if (AI_Mini != null)//领导有的时候不会播放
        {
            if (last_co != null)
            {
                StopCoroutine(last_co);
            }

            last_co = StartCoroutine(AI_Mini_Move());
        }


        //debuff字：KPI减少之类的(不一定有)

    }

    void SetHead()//只显示ID对的那个头
    {
        List<int> ns = new List<int> { 0, 1, 2, 3 };
        ns.Remove(id);
        AI_Mini.transform.Find("Head" + id).gameObject.SetActive(true);
        foreach (var n in ns)
        {
            AI_Mini.transform.Find("Head" + n.ToString()).gameObject.SetActive(false);
        }
    }

    public IEnumerator AI_Mini_Move()
    {
        float timer = 0;
        Vector3 originPos = AI_Mini.transform.localPosition;

        while (true)
        {
            if (timer > (2 * Mechanism.Instance.DebuffAITime - 0.04f))
            {
                animator_Box.SetTrigger("Close");//关盒子
                AI_Mini.transform.localPosition = originPos;
                if (AI_Mini != null)
                {
                    Destroy(AI_Mini);
                }
                AI_Mini = null;
                yield break;
            }
            timer += Time.deltaTime;
            float factor = timer / (2 * Mechanism.Instance.DebuffAITime - 0.04f);
            AI_Mini.transform.localPosition = originPos + new Vector3(0, anic_MiniMove.Evaluate(factor) * 0.1f, 0);
            yield return null;
        }
    }
    void DelaySpeak(int id1)
    {
        StartCoroutine(Delay_Speak(id1));
    }
    IEnumerator Delay_Speak(int id1)
    {
        Debug.Log("DelaySpeak0");
        yield return new WaitForSeconds(Mechanism.Instance.DebuffAITime / 2);
        Debug.Log("DelaySpeak1");
        Speak(id1);
        yield break;
    }
    void Speak(int id1)
    {
        if (aI_Cha_Type == AI_Cha_Type.Leader)
        {
            switch (id1)
            {
                case 31:
                    dialog_Mini.SetDiaglog("抓到摸鱼的了！");
                    break;
                case 38:
                    dialog_Mini.SetDiaglog("厕所是你堵住的？");
                    break;
                default:
                    break;
            }
        }
        else if (aI_Cha_Type == AI_Cha_Type.AI)
        {
            switch (id1)
            {
                case 31:
                    dialog_Mini.SetDiaglog("对不起    ");
                    break;
                case 32:
                    dialog_Mini.SetDiaglog("我的绩效呢？");
                    break;
                case 33:
                    dialog_Mini.SetDiaglog("想干一架吗！？");
                    break;
                case 35:
                    dialog_Mini.SetDiaglog("身体好难受！");
                    break;
                case 37:
                    dialog_Mini.SetDiaglog("为什么欺负我？");
                    break;
                case 38:
                    dialog_Mini.SetDiaglog("呜呜呜，不是我");
                    break;
                case 54:
                    dialog_Mini.SetDiaglog("福报！福报！");
                    break;
                case 56:
                    dialog_Mini.SetDiaglog("我是卷王！");
                    break;
                case 59:
                    dialog_Mini.SetDiaglog("不要啊，再给我一次机会！");
                    break;
                case 60:
                    dialog_Mini.SetDiaglog("咳......咳！我还能工作");
                    break;
                default:
                    break;
            }
        }

    }

    //!!!!!!!!!!!!!!Chess送屎!!!!!!!!!!!!!!
    void Speak_SendShit(int id1)
    {
        dialog_Mini.SetDiaglog("搞你搞你    ");
        // switch (id1)
        // {
        //     case 10000:
        //         dialog_Mini.SetDiaglog("搞你");
        //         break;
        // case 32:
        //     dialog_Mini.SetDiaglog("我的绩效呢？");
        //     break;
        // case 33:
        //     dialog_Mini.SetDiaglog("想干一架吗！？");
        //     break;
        // case 35:
        //     dialog_Mini.SetDiaglog("身体好难受！");
        //     break;
        // case 37:
        //     dialog_Mini.SetDiaglog("为什么欺负我？");
        //     break;
        // case 38:
        //     dialog_Mini.SetDiaglog("呜呜呜，不是我");
        //     break;
        // case 54:
        //     dialog_Mini.SetDiaglog("福报！福报！");
        //     break;
        // case 56:
        //     dialog_Mini.SetDiaglog("我是卷王！");
        //     break;
        // case 59:
        //     dialog_Mini.SetDiaglog("不要啊，再给我一次机会！");
        //     break;
        // case 60:
        //     dialog_Mini.SetDiaglog("咳......咳！我还能工作");
        //     break;
        //     default:
        //         break;

        // }

    }
    public void SendShit(Card card)//送屎特效
    {
        StartCoroutine(DelaySendShit(card));
    }
    IEnumerator DelaySendShit(Card card)//延迟播放特效
    {
        yield return new WaitForSeconds(0.5f);
        //生成特定的模型
        if (AI_Mini != null)
        {
            Destroy(AI_Mini);
        }
        AI_Mini = null;

        AI_Mini = Instantiate(AIMechanism.Instance.AISendShitPose, Mini.transform);
        SetHead();
        AI_Mini.transform.localPosition = new Vector3(0, -0.05f, 0.018f);
        //说骚话（可能）

        //开盒子
        animator_Box.SetTrigger("Open");
        //模型动画
        StartCoroutine(AI_Mini_Move());
        yield return new WaitForSeconds(Mechanism.Instance.DebuffAITime);
        Speak_SendShit(card.id);
        if (se != null)
        {
            se.InstantiateCard(card, 0f);
        }
        yield break;
    }


    //!!!!!!!!!!!!!!WeekendMeeting!!!!!!!!!!!!!!


    //KPIBAR的变化动画，kpiBar的localscale的z倒250是最大的
    public void KPIBar_Ex()
    {
        ResetValue();//重置一些计时器之类的参数
        if (aI_Cha_Type != AI_Cha_Type.Leader)
        {
            StartCoroutine(KPIBar_Exchange());
            ChangeFace();
        }

    }
    IEnumerator KPIBar_Exchange()
    {
        float timer = 0f;
        Vector3 originScale = KPIBar.transform.localScale;
        Vector3 destScale = originScale;
        int originKPI = int.Parse(KPIBar_Text.text);
        int destKPI = 0;
        if (aI_Cha_Type == AI_Cha_Type.AI)
        {
            destKPI = AIData.KPI;
            destScale.z = 250f * AIData.KPI / (float)AIMechanism.Instance.KPITopThisWeek;
        }
        else if (aI_Cha_Type == AI_Cha_Type.Player)
        {
            destKPI = PlayerData.Instance.KPI;
            destScale.z = 250f * PlayerData.Instance.KPI / (float)AIMechanism.Instance.KPITopThisWeek;
        }

        while (true)
        {
            if (timer > 2f)
            {
                KPIBar.transform.localScale = destScale;
                KPIBar_Text.text = destKPI.ToString();
                yield break;
            }
            timer += Time.deltaTime;
            float factor = timer / 2f;
            KPIBar.transform.localScale = Vector3.Lerp(originScale, destScale, factor);
            KPIBar_Text.text = ((int)Mathf.Lerp(originKPI, destKPI, factor)).ToString();
            yield return null;
        }
    }

    void ChangeFace()
    {
        float health = 0;
        if (aI_Cha_Type == AI_Cha_Type.AI)
        {
            health = Mathf.Min(AIData.physicalHealth / (float)AIData.physicalHealthMax, AIData.spiritualHealth / (float)AIData.spiritualHealthMax);
        }
        else if (aI_Cha_Type == AI_Cha_Type.Player)
        {
            health = Mathf.Min(PlayerData.Instance.physicalHealth / (float)PlayerData.Instance.physicalHealthMax, PlayerData.Instance.spiritualHealth / (float)PlayerData.Instance.spiritualHealthMax);
        }

        healthMat.SetFloat("_Health", health);
    }

    public void workAbilityMatChangeColor()//象征职位等级的宝石变色
    {
        int n = 1;
        if (aI_Cha_Type == AI_Cha_Type.AI)
        {
            n = AIData.postLevel;
        }
        else if (aI_Cha_Type == AI_Cha_Type.Player)
        {
            n = PlayerData.Instance.postLevel;
        }
        switch (n)
        {
            case 1:
                workAbilityMat.color = new Color(210f / 255f, 210f / 255f, 210f / 255f, 1);
                break;
            case 2:
                workAbilityMat.color = new Color(175f / 255f, 239f / 255f, 96f / 255f, 1);
                break;
            case 3:
                workAbilityMat.color = new Color(35f / 255f, 130f / 255f, 236f / 255f, 1);
                break;
            case 4:
                workAbilityMat.color = new Color(179f / 255f, 33f / 255f, 180f / 255f, 1);
                break;
            case 5:
                workAbilityMat.color = new Color(255f / 255f, 195f / 255f, 50f / 255f, 1);
                break;
            default:
                workAbilityMat.color = Color.white;
                break;
        }
    }

    public void PostLevelUpdate()
    {
        if (AIData.AI_PostLevel())
        {
            dialog_Mini_Weekend.SetDialog_Delay("我要升级岗位！", 0.5f);
            //特效
            pm_upgrade.Stop();
            pm_upgrade.Play();

            //宝石变色
            workAbilityMatChangeColor();
            //做动作
            animator_Mini_Weekend.SetTrigger("Handup");
        }
    }

    public void HealthSpeak(bool isRandom = true)//说自己的健康信息
    {
        float r = 0f;
        if (isRandom)
        {
            r = Random.Range(0, 1f);
        }
        else
        {
            r = 0f;
        }

        float p_health = AIData.physicalHealth / (float)AIData.physicalHealthMax;
        float s_health = AIData.spiritualHealth / (float)AIData.spiritualHealthMax;
        if (p_health <= 0.25f && s_health <= 0.25f)//濒死提示
        {
            if (r < 0.3f)
            {
                dialog_Mini_Weekend.SetDiaglog("身体快要完全不行了！");
            }
        }
        else if (p_health <= 0.25f)
        {

            if (r < 0.3f)
            {
                dialog_Mini_Weekend.SetDiaglog("体力极差！");
            }

        }
        else if (s_health <= 0.25f)
        {

            if (r < 0.3f)
            {
                dialog_Mini_Weekend.SetDiaglog("精神疲惫！");
            }

        }
        else if (p_health <= 0.5f && s_health <= 0.5f)
        {
            if (r < 0.3f)
            {
                dialog_Mini_Weekend.SetDiaglog("不是很舒服");
            }
        }
        else if (p_health <= 0.5f)
        {
            if (r < 0.3f)
            {
                dialog_Mini_Weekend.SetDiaglog("肌肉酸痛");
            }
        }
        else if (s_health <= 0.5f)
        {

            if (r < 0.3f)
            {
                dialog_Mini_Weekend.SetDiaglog("有点困了");
            }
        }
        else if (p_health == 1 && s_health == 1)
        {
            if (r < 0.3f)
            {
                dialog_Mini_Weekend.SetDiaglog("极其健康");
            }
        }
        else//体力和精力都正常
        {
            if (r < 0.3f)
            {
                dialog_Mini_Weekend.SetDiaglog("身体状态不错");
            }
        }
    }

    [HideInInspector] public Dictionary<string, string> names_reasons_Dic = new Dictionary<string, string>();
    [HideInInspector] public List<string> NewAiNames = new List<string>();
    public void LeaderWeekendSpeak()
    {
        if (names_reasons_Dic.Count == 0 && NewAiNames.Count == 0)
        {
            dialog_Mini_Weekend.SetDiaglog("周末了，开会！");
        }
        else
        {
            string s = null;
            //新来的
            for (var i = 0; i < NewAiNames.Count; i++)
            {
                s += NewAiNames[i] + "复活了，并换了一个人格" + "\r\n";
                NewAiNames.Remove(NewAiNames[i]);
            }

            //离开的
            List<string> names = new List<string>(names_reasons_Dic.Keys);

            for (int i = 0; i < names.Count; i++)
            {
                s += names[i] + "因" + names_reasons_Dic[names[i]] + "离开了" + "\r\n";
                names_reasons_Dic.Remove(names[i]);
            }

            dialog_Mini_Weekend.SetDiaglog(s);
        }


    }

    float timer = 0f;
    float T = 4f;
    bool isFirstUpgrade = true;
    bool isFirstStart = true;
    public void ResetValue()
    {
        timer = 0f;
        isFirstUpgrade = true;
        isFirstStart = true;
        T = 4f;
    }

    private void Update()
    {
        if (aI_Cha_Type == AI_Cha_Type.AI)
        {
            if (AIData != null)
            {
                timer += Time.deltaTime;
                if (WeekendMeetingAll.Instance.weekendPhase == weekendPhase.Start)
                {
                    if (isFirstStart)
                    {
                        isFirstStart = false;
                        HealthSpeak(false);
                    }
                    else
                    {
                        if (timer > T)
                        {
                            timer = 0;
                            T = Random.Range(3, 5f);
                            HealthSpeak();
                        }
                    }
                }
                else if (WeekendMeetingAll.Instance.weekendPhase == weekendPhase.PostUpgrade)
                {
                    if (isFirstUpgrade)
                    {
                        isFirstUpgrade = false;
                        // dialog_Mini_Weekend.SetDiaglog("升职信息");
                        //升职函数
                        PostLevelUpdate();

                    }
                }
            }

        }
        else if (aI_Cha_Type == AI_Cha_Type.Leader)
        {
            if (WeekendMeetingAll.Instance.weekendPhase == weekendPhase.LeaderTime)
            {
                timer += Time.deltaTime;
                if (isFirstStart)
                {
                    isFirstStart = false;
                    LeaderWeekendSpeak();//辞退通知
                }

                if (timer > T)
                {
                    timer = 0;
                    WeekendMeetingAll.Instance.weekendPhase = weekendPhase.PostUpgrade;
                }
            }

        }


    }


    //!!!!!!!!!!!!!!Big!!!!!!!!!!!!!!
    IEnumerator DelayWork()
    {
        yield return new WaitForSeconds(Random.Range(0, 1f));
        animator_Big.SetTrigger("Work");
        yield break;
    }
}
