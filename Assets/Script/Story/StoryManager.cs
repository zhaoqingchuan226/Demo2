using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using SWS;

//剧情时期
public enum Period
{
    日常1,
    宣发前,
    // 宣发中,
    宣发后,
    日常2,
    舞会前,
    // 舞会中,
    舞会后,
    日常3,
    G3A前,
    // G3A中,
    G3A后,
    日常4,
    败露前,
    // 败露中,
    败露后,
    日常5,
    绝境前,
    // 绝境中,
    绝境后
}

public enum PlotPhase
{
    aside_Start,
    words1,
    chooses,
    words2,
    aside_End,
}

public class StoryManager : MonoSingleton<StoryManager>
{
    //传入的UI对象
    public GameObject StoryAll;
    public GameObject chooseButtonPrefab;
    public GameObject wordsPannel;
    public GameObject choosePannel;
    public GameObject chooseGroup;
    public GameObject asidePannel;
    public TextMeshProUGUI asideText;
    public TextMeshProUGUI NPCNameText;
    public TextMeshProUGUI wordsText;



    public Period period = Period.日常1;
    // public int week = 1;
    public int periodDays = 4;//四周为一个周期
    public float asideTime = 2f;
    // public TextMeshProUGUI weekText;
    public TextMeshProUGUI periodText;
    public TextAsset StoryData;
    public List<NPC> NPCs = new List<NPC>();
    public List<NPC> NPCs_HasFound = new List<NPC>();//已经遇到的NPC，将会与这些NPC发生各种各样的剧情
    public List<Plot> plotsThisWeek = new List<Plot>();//这周所会发生的所有剧情
    public Plot plotNow;//现在正在发生的剧情
    bool input;//如何切换下一段对话
    bool isPlotProcessing;//剧情是否在进行
    PlotPhase plotPhase;//剧情现阶段执行到哪一步
    // float pro;             //选择后所被发配到的概率
    BranchPlot bpNow;//现在所在在分支
    public float timer = 0;
    bool isFirstThisPlotPhase = true;
    List<GameObject> buttons = new List<GameObject>();
    public List<GameObject> NPC_GameObjs = new List<GameObject>();
    public GameObject RewardPanel;
    public TextMeshProUGUI RewardText;

    //开场动画
    [Space]
    public bool isBeginAni = true;//是否有开场动画
    [HideInInspector] public List<string> BlackBeginWords = new List<string>();
    [HideInInspector] public Dictionary<string, string> BlackBeginWords_owner_Dic = new Dictionary<string, string>();//台词和它的主人,M为自己，B为黑衣人

    //黑衣人的对话框(其实是玩家的)
    // public GameObject BlackWordsObj;
    // public TextMeshProUGUI BlackWordsText;

    //PD
    public PlayableDirector pd;
    public List<PlayableAsset> pas = new List<PlayableAsset>();

    //black animator
    public Animator animator_black;

    //black sws
    public splineMove sm;

    //blakc dialog
    public Dialog blackDialog;//看得见黑衣人的时候用的dialog
    public Dialog blackDialog2;//看不见黑衣人的时候用的dialog
    public Dialog playerDialog;
    //character sws

    //SwitchBall
    public GameObject switchBall;
    [HideInInspector] GameObject switchBall_Instance;
    public List<Transform> switchBall_trans = new List<Transform>();
    public float switchBallExTime = 1.5f;
    float timer_switchBall;

    public bool isChaAniFinished = false;

    void Awake()
    {
        StoryAll.SetActive(false);
        RewardPanel.SetActive(false);
        LoadStoryDataFromCSV();


        isPlotProcessing = false;
        // foreach (var NPC_GameObj in NPC_GameObjs)
        // {
        //     NPC_GameObj.SetActive(false);
        // }



    }
    float timer_sit = 0f;
    void Start()
    {
        // UpdataUI();
        asidePannel.SetActive(false);
        choosePannel.SetActive(false);
        wordsPannel.SetActive(false);
        NPCs_HasFound.Add(NPCs[0]);//到时候把所有人的都加进去，就好
                                   // foreach (var plot in NPCs[0].plots)
                                   // {
                                   //     Debug.Log(plot.id);
                                   // }
                                   //开局坐好
        SetPlayableAsset("StartSit");
        pd.Play();
        StartCoroutine(Sit());
    }
    IEnumerator Sit()
    {
        yield return new WaitForSeconds(5f);
        NPC_SitDesk(NPC_GameObjs[0]);
        NPC_GameObjs[0].SetActive(false);
        yield break;
    }

    public void O_NPC(string NPCName)//signal用//其实输入的不是NPC的名字，而是预制体名称
    {
        O_C_NPC(NPCName, true);
    }

    public void C_NPC(string NPCName)
    {
        O_C_NPC(NPCName, false);
    }

    void O_C_NPC(string NPCName, bool b)//激活，取消激活NPC
    {
        foreach (var NPC_GameObj in NPC_GameObjs)
        {
            if (NPC_GameObj.name == NPCName)
            {
                NPC_GameObj.SetActive(b);
                break;
            }
        }
    }

    IEnumerator OpenAsidePanel(float timer = 0, bool isOpenCha = true)//isOpenCha为ture则打开角色，反之则关闭
    {
        bool isFirstMid = false;
        while (true)
        {
            if (timer > asideTime)
            {
                asidePannel.SetActive(false);
                yield break;
            }
            float factor = timer / asideTime;
            if (factor > 0.5f && !isFirstMid)//进程过一半激活人物
            {
                isFirstMid = true;

                if (isOpenCha)
                {
                    O_NPC("Cha_" + plotNow.owner.type.ToString());
                    sm.gameObject.SetActive(false);
                    Mechanism.Instance.chessBoard.SetActive(false);
                    LACControl.Instance.O_C_AllMeshes(false);
                    AIMechanism.Instance.O_C_AllAIModel(false);
                }
                else
                {
                    C_NPC("Cha_" + plotNow.owner.type.ToString());
                    sm.gameObject.SetActive(true);
                    Mechanism.Instance.chessBoard.SetActive(true);
                    LACControl.Instance.O_C_AllMeshes(true);
                    AIMechanism.Instance.O_C_AllAIModel(true);
                }

            }
            timer += Time.deltaTime;
            factor = Mathf.Clamp01((-2 * Mathf.Abs(factor - 0.5f) + 1f) * 1.2f);
            Image img = asidePannel.GetComponentInChildren<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, factor);
            asideText.color = new Color(asideText.color.r, asideText.color.g, asideText.color.b, factor);
            yield return null;
        }
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     SetPlayableAsset("KPIKill");
        //     pd.Play();
        // }
        // else if(Input.GetKeyDown(KeyCode.L))
        // {
        //     SetPlayableAsset("HealthKill");
        //     pd.Play();
        // }


        input = Input.GetMouseButtonDown(0);
        if (isPlotProcessing)
        {
            if (plotPhase == PlotPhase.aside_Start)
            {
                timer += Time.deltaTime;
                if (timer > asideTime)
                {
                    timer = 0;
                    plotPhase++;
                    isFirstThisPlotPhase = true;
                }
                else
                {
                    if (isFirstThisPlotPhase)
                    {
                        StoryAll.SetActive(true);
                        asidePannel.SetActive(true);
                        StartCoroutine(OpenAsidePanel(0f, true));
                        asideText.text = plotNow.aside_Start;
                        isFirstThisPlotPhase = false;
                    }

                }
            }
            else if (plotPhase == PlotPhase.aside_End)
            {
                timer += Time.deltaTime;
                if (timer > asideTime)
                {
                    timer = 0;
                    plotPhase = PlotPhase.aside_Start;
                    isFirstThisPlotPhase = true;
                    isPlotProcessing = false;
                    //如果还有其他剧情，则执行
                    ExecutePlotThisWeek();
                }
                else
                {
                    if (isFirstThisPlotPhase)
                    {
                        SendReward();
                        // C_NPC("Cha_" + plotNow.owner.type.ToString());
                        // Debug.Log("Cha_"+plotNow.owner.type.ToString());
                        asidePannel.SetActive(true);
                        StartCoroutine(OpenAsidePanel(0f, false));
                        asideText.text = plotNow.aside_End;
                        isFirstThisPlotPhase = false;
                    }
                }
            }
            else if (plotPhase == PlotPhase.words1)
            {
                if (isFirstThisPlotPhase)
                {
                    PlayAniPart1();//开始放动画
                    wordsPannel.SetActive(true);
                    isFirstThisPlotPhase = false;
                    wordsText.text = plotNow.words1[0];
                }
                if (input && isChaAniFinished)
                {
                    isChaAniFinished = false;
                    ResumeAni();//角色动画继续放
                    int index = plotNow.words1.IndexOf(wordsText.text);
                    if (index < (plotNow.words1.Count - 1))//如果不是最后一句话
                    {
                        wordsText.text = plotNow.words1[index + 1];
                    }
                    else
                    {
                        plotPhase++;
                        isFirstThisPlotPhase = true;
                    }
                }
            }

            else if (plotPhase == PlotPhase.words2)
            {
                if (isFirstThisPlotPhase)
                {
                    isFirstThisPlotPhase = false;
                    List<string> words2 = new List<string>();
                    words2 = bpNow.words;
                    wordsText.text = words2[0];
                }
                if (input && isChaAniFinished)
                {
                    isChaAniFinished = false;
                    ResumeAni();//角色动画继续放
                    List<string> words2 = new List<string>();
                    words2 = bpNow.words;
                    int index = words2.IndexOf(wordsText.text);
                    if (index < (words2.Count - 1))//如果不是最后一句话
                    {
                        wordsText.text = words2[index + 1];
                    }
                    else
                    {
                        wordsPannel.SetActive(false);
                        plotPhase++;
                        isFirstThisPlotPhase = true;
                    }



                }
            }
            else if (plotPhase == PlotPhase.chooses)
            {
                if (isFirstThisPlotPhase)
                {
                    isFirstThisPlotPhase = false;
                    choosePannel.SetActive(true);
                    foreach (var choose in plotNow.chooses)
                    {
                        GameObject button = Instantiate(chooseButtonPrefab, chooseGroup.transform);
                        button.GetComponentInChildren<TextMeshProUGUI>().text = choose;
                        buttons.Add(button);
                    }

                }
            }


        }
    }


    public void OnClickButton(TextMeshProUGUI tmp)
    {
        string choose = tmp.text;
        // List<float> pros = plotNow.Choose_Probability_Dic[choose];
        List<BranchPlot> bps = plotNow.choose_Branch_Dic[choose];

        //不同选择随机概率
        float r = Random.Range(0f, 1f);
        string reward = null;
        if (bps.Count == 1)
        {
            bpNow = bps[0];
            // pro = bps[0].pro;
            reward = bps[0].reward;
        }
        else if (bps.Count == 2)
        {
            if (r < bps[0].pro)
            {
                bpNow = bps[0];
                // pro = bps[0].pro;
                reward = bps[0].reward;
            }
            else
            {
                bpNow = bps[1];
                // pro = bps[1].pro;
                reward = bps[1].reward;
            }
        }
        // else if (pros.Count == 3)//目前没有三个分选项的，所以不用管
        // {
        //     if (r < pros[0])
        //     {
        //         pro = pros[0];
        //         reward = plotNow.Probability_Reward_Dic[pros[0]];
        //     }
        //     else if (r >= pros[0] && r < (pros[0] + pros[1]))
        //     {
        //         pro = pros[1];
        //         reward = plotNow.Probability_Reward_Dic[pros[1]];
        //     }
        //     else
        //     {
        //         pro = pros[2];
        //         reward = plotNow.Probability_Reward_Dic[pros[2]];
        //     }
        // }
        //奖励桌面配置，之后可以写成复杂的函数
        // Debug.Log(reward);

        //
        foreach (var button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
        plotPhase++;
        isFirstThisPlotPhase = true;

        //播放分支后动画
        PlayAniPart2();
    }


    public void StartPlot()//触发所有剧情的函数
    {
        if (period == Period.绝境后)
        {
            return;
        }

        int week = Mechanism.Instance.week;
        //日常1：1-4
        //事件前：5-6
        //事件：6
        //事件后：7-8
        //日常2：9-12

        //不正规的时间计算
        if (week % (2 * periodDays) == 5 || (week % (2 * periodDays) == 1 && week != 1) || (week % (2 * periodDays) == 7))//&& period.ToString().Contains("中")
        {
            period++;
        }
        // else if (week % (2 * periodDays) == 6 && period.ToString().Contains("前"))
        // {
        //     period++;
        // }
        else
        {

        }


        // FindNewNPC();
        //正规的时间计算
        // if (week % (2 * periodDays) == 4 || week % (2 * periodDays) == 0 || (week % (2 * periodDays) == 6 && period.ToString().Contains("中")))
        // {
        //     period++;
        //     week++;
        // }
        // else if (week % (2 * periodDays) == 6 && period.ToString().Contains("前"))
        // {
        //     period++;
        // }

        // else
        // {
        //     week++;
        // }

        // // if (week % periodDays == 0 && !period.ToString().Contains("后"))
        // // {
        // //     period++;
        // // }
        // // else if (period.ToString().Contains("后"))
        // // {
        // //     period++;
        // //     week++;
        // // }
        // // else
        // // {
        // //     week++;
        // // }
        // Debug.Log(week.ToString() + StoryManager.Instance.period); 
        UpdataUI();
        GetPlotThisWeek();//获取这周要发生的剧情
        ExecutePlotThisWeek();//执行这周要发生的剧情
    }
    void UpdataUI()
    {
        // weekText.text = week.ToString();
        periodText.text = period.ToString();
    }



    void GetPlotThisWeek()//找到这周可能发生的事件,添加进入
    {
        plotsThisWeek.Clear();
        foreach (var NPC_HasFound in NPCs_HasFound)
        {
            for (int i = 0; i < NPC_HasFound.plots.Count; i++)
            {
                if (NPC_HasFound.plots[i].JudgeCondition())
                {
                    plotsThisWeek.Add(NPC_HasFound.plots[i]);
                    NPC_HasFound.plots.Remove(NPC_HasFound.plots[i]);//已经体验过的剧情不会重复出现
                    break;//一个NPC一周只有一段剧情，这里为了防止一个NPC一周两段剧情
                }
            }
        }
    }

    public void SetIsProcessing(bool b)//给“过去”动画的最后一帧用，激活UI界面开始交互故事，并播放分支前动画
    {
        isPlotProcessing = b;
    }
    void ExecutePlotThisWeek()
    {
        if (plotsThisWeek.Count > 0)
        {
            plotNow = plotsThisWeek[0];

            isPlotProcessing = true;
            plotPhase = PlotPhase.aside_Start;
            plotsThisWeek.RemoveAt(0);
        }
        else//进入这里有两种可能，一种是什么剧情都没有，另一种则是播放完成所有的剧情
        {
            if (plotNow != null)//如果有播放完成的剧情，则播放回去的动画
            {
                // switch (plotNow.owner.type)
                // {
                //     case Type.OverLoad:
                //         SetPlayableAsset("BackOverload");
                //         pd.Play();
                //         break;
                //     default:
                //         break;
                // }
                EndPlot();
                plotNow = null;
            }
            else//如果什么剧情都没有
            {
                Mechanism.Instance.phase++;
                Mechanism.Instance.playState = PlayState.Chess;
            }


            isPlotProcessing = false;
            //如果没有剩余的剧情了，那么就退出这个界面
            StoryAll.SetActive(false);

        }
    }

    public void EndPlot()//给signal用的
    {
        Mechanism.Instance.phase++;
        Mechanism.Instance.playState = PlayState.Chess;
        CameraManager.Instance.SetVirtualCam("BlackCam");
    }

    void SendReward()
    {
        string s = null;
        switch (bpNow.reward)
        {
            case "招魂仪":
                s = AutoDDReward(bpNow.reward);
                SendDDtoPlaterData(1);
                Debug.Log("OJBK");
                break;
            case "提神发夹":
                s = AutoDDReward(bpNow.reward);
                SendDDtoPlaterData(2);
                break;
            default:
                break;
        }
        if (s != null)
        {
            DesktopDecorationStore.Instance.GenerateDDs();
        }


        if (bpNow.reward.Contains("card"))
        {
            string[] elements = bpNow.reward.Split('|');
            int cardID = int.Parse(elements[1]);
            int cardAmount = int.Parse(elements[2]);
            string cardName = null;
            for (var i = 0; i < cardAmount; i++)
            {
                Card c = CardStore.Instance.SearchCard(cardID);
                c.isNew = true;
                // InstantiateEffect(this, kidCard, interval * i);
                PlayerData.Instance.playerCards.Add(c);
                PlayerData.Instance.SortCards();
                cardName = c.title;
            }
            s = AutoCardReward(cardID, cardAmount, cardName);
        }
        if (s == null)
        {
            s = "无事发生";
        }
        StartCoroutine(OpenRewardPanel(s));

    }

    void SendDDtoPlaterData(int DDid)
    {
        PlayerData.Instance.dds.Add(DesktopDecorationStore.Instance.SearchDD(DDid));
    }
    string AutoDDReward(string DD)
    {
        return "获得了 " + "“" + DD + "”";
    }
    string AutoCardReward(int cardID, int cardAmount, string cardName)
    {
        return "获得了 " + cardAmount.ToString() + "张" + "“" + cardName + "”";
    }
    IEnumerator OpenRewardPanel(string s, float openTime = 3f, float timer = 0)
    {
        RewardPanel.SetActive(true);
        RewardText.text = s;
        while (true)
        {
            if (timer > openTime)
            {
                RewardPanel.SetActive(false);
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void PlayAniPart1()//播放分支前动画
    {
        SetPlayableAsset(plotNow.id.ToString());
        pd.Play();
    }
    public void PlayAniPart2()//播放分支动画
    {
        SetPlayableAsset(bpNow.name);
        pd.Play();
    }

    public void PauseAni()
    {
        isChaAniFinished = true;
        Debug.Log("Pause");
        pd.Pause();
    }

    public void ResumeAni()
    {
        pd.Resume();
    }

    void LoadStoryDataFromCSV()
    {
        string[] dataRows = StoryData.text.Split("\r\n", System.StringSplitOptions.RemoveEmptyEntries);//按行分割
        for (int i = 0; i < dataRows.Length; i++)
        {
            string[] elements = dataRows[i].Split(',');
            if (elements[0] == "NPC")
            {
                string name = elements[1];
                string type_string = elements[2];
                Type type;
                switch (type_string)
                {
                    case "过载":
                        type = Type.OverLoad;
                        break;
                    case "精技":
                        type = Type.Skilled;
                        break;
                    case "暗算":
                        type = Type.Snaky;
                        break;
                    case "忍耐":
                        type = Type.Tolerant;
                        break;
                    case "摸鱼":
                        type = Type.Fishlike;
                        break;
                    case "善舞":
                        type = Type.Sociable;
                        break;
                    case "隐世":
                        type = Type.Flexible;
                        break;
                    case "叛乱":
                        type = Type.Revolting;
                        break;
                    default:
                        type = Type.OverLoad;
                        break;
                }
                NPCs.Add(new NPC(name, type));
            }
            else if (elements[0] == "Plot")
            {
                string owner = elements[1];
                List<string> condis = new List<string>();


                //条件
                for (int j = 2; j < elements.Length; j++)
                {
                    if (elements[j] != "" && elements[j] != "\r")//最后一个条件是/r回车
                    {
                        condis.Add(elements[j]);
                    }

                }

                //台词们
                string Aside_Start = null;
                string Aside_End = null;
                List<string> words1 = new List<string>();
                List<string> chooses = new List<string>();
                Dictionary<string, List<BranchPlot>> choose_Branch_Dic = new Dictionary<string, List<BranchPlot>>();
                int ID = 0;

                for (int k = 1; k < 100; k++)
                {
                    string[] elements2 = dataRows[i + k].Split(',');
                    if (elements2[1] == "Aside_Start")
                    {
                        Aside_Start = elements2[7];
                    }
                    else if (elements2[1] == "Words1")
                    {
                        for (int a = 2; a < elements2.Length; a++)
                        {
                            if (elements2[a] != "" && elements2[a] != "\r")//最后一个条件是/r回车
                            {
                                words1.Add(elements2[a]);
                            }
                        }
                    }
                    else if (elements2[1] == "Choose")
                    {
                        string choose = elements2[7];
                        chooses.Add(choose);
                        List<BranchPlot> bps = new List<BranchPlot>();
                        // List<float> pros = new List<float>();
                        for (int l = 1; l < 80; l++)
                        {
                            string[] elements3 = dataRows[i + k + l].Split(',');

                            if (elements3[2] != "pro")
                            {
                                break;
                            }
                            else
                            {
                                float pro = float.Parse(elements3[3]);
                                string name = elements3[4];
                                string reward = elements3[5];
                                // PlayableAsset pa = SearchPlayableAsset(name);
                                // pros.Add(pro);
                                List<string> words2 = new List<string>();
                                for (int b = 7; b < elements3.Length; b++)
                                {
                                    if (elements3[b] != "" && elements3[b] != "\r")
                                    {
                                        words2.Add(elements3[b]);
                                    }
                                }
                                bps.Add(new BranchPlot(name, pro, words2, reward));
                                // Probability_words2_Dic.Add(pro, words2);
                                // Probability_Reward_Dic.Add(pro, reward);
                            }
                        }
                        choose_Branch_Dic.Add(choose, bps);
                        // Choose_Probability_Dic.Add(choose, pros);
                    }
                    else if (elements2[1] == "Aside_End")
                    {
                        Aside_End = elements2[7];
                        // break;
                    }
                    else if (elements2[1] == "ID")
                    {
                        ID = int.Parse(elements2[2]);
                        break;
                    }
                }
                Plot plot = new Plot(Aside_Start, condis, words1, chooses, choose_Branch_Dic, Aside_End, owner, ID);

                foreach (var NPC in NPCs)
                {
                    if (NPC.name == owner)
                    {
                        NPC.plots.Add(plot);
                    }
                }
            }
            else if (elements[0] == "BlackBeginWords")
            {
                for (var j = 1; j < 50; j++)
                {
                    string[] elements3 = dataRows[i + j].Split(',', System.StringSplitOptions.RemoveEmptyEntries);
                    if (elements3.Length > 0)
                    {
                        BlackBeginWords.Add(elements3[0]);
                        BlackBeginWords_owner_Dic.Add(elements3[0], elements3[1]);
                    }
                }

            }
        }

    }
    /*
        void PlayerMove(string pathContainer)
        {
            switch (pathContainer)
            {
                case "Overload":
                    player_sm.pathContainer = pathManagers[0];
                    player_sm.speed = 2f;
                    break;
                case "OverloadBack":
                    player_sm.pathContainer = pathManagers[1];
                    player_sm.speed = 2f;
                    break;
                default:
                    break;
            }
            float timer = 0;
            player_sm.StartMove();
            player_sm.Pause();
            StartCoroutine(DelayStart(0.5f, timer));

        }
        IEnumerator DelayStart(float delayTime, float timer)
        {
            while (true)
            {
                if (timer > delayTime)
                {
                    player_sm.StartMove();
                    yield break;
                }
                timer += Time.deltaTime;
                yield return null;
            }
        }


        public void PlayerMove_Go(string pathContainer)//主角的移动
        {
            player_sm.reverse = false;
            PlayerMove(pathContainer);
        }

        public void PlayerMove_Back(string pathContainer)//主角的移动
        {
            player_sm.reverse = false;
            PlayerMove(pathContainer);
        }

    */
    //动画部分
    public void SetPlayableAsset(string s)
    {
        pd.playableAsset = SearchPlayableAsset(s);
        // foreach (var pa in pas)
        // {
        //     if (s == pa.name)
        //     {
        //  pd.playableAsset =  pa;
        //         break;
        //     }
        // }
    }


    public PlayableAsset SearchPlayableAsset(string s)
    {
        PlayableAsset paa = null;
        foreach (var pa in pas)
        {
            if (s == pa.name)
            {
                paa = pa;
                break;
            }
        }
        return paa;
    }

    public void BeginAni()//开场动画
    {
        Mechanism.Instance.playState = PlayState.Black;
        SetPlayableAsset("Begin2");
        pd.Play();
    }
    public void KillAni()//分两种情况讨论
    {

        Mechanism.Instance.CloseGlobalUI();
        if (Mechanism.Instance.deathReason == "Health")
        {
            SetPlayableAsset("HealthKill");
            pd.Play();
        }
        else if (Mechanism.Instance.deathReason == "KPI")
        {
            SetPlayableAsset("KPIKill");
            pd.Play();
        }
        // Mechanism.Instance.playState = PlayState.Black;

    }




    public void KillPos()//前往一个pos
    {
        sm.gameObject.transform.position = new Vector3(-0.839f, -0.741f, 1.08f);
        sm.gameObject.transform.rotation = Quaternion.Euler(0, 78.27f, 0);
    }

    public void BlackSpeak_Delay(string s, float time)
    {
        StartCoroutine(Delay(s, time));
    }

    IEnumerator Delay(string s, float time)
    {
        yield return new WaitForSeconds(time);
        BlackSpeak(s);
        yield break;
    }
    public void BlackSpeak(string s)//黑衣人根据视角使用不用dialog对话框
    {
        if (CameraManager.Instance.cb.ActiveVirtualCamera.Name != "BlackCam" && CameraManager.Instance.cb.ActiveVirtualCamera.Name != "FreeCam")
        {
            blackDialog2.SetDiaglog(s);
        }
        else
        {
            blackDialog.SetDiaglog(s);
        }
    }

    public void Speak_Teach(string s, string owner = "B")//黑衣人根据视角使用不用dialog对话框
    {
        Debug.Log(CameraManager.Instance.cb.ActiveVirtualCamera.Name);
        if (owner == "B")
        {
            if (CameraManager.Instance.cb.ActiveVirtualCamera.Name != "BlackCam" && CameraManager.Instance.cb.ActiveVirtualCamera.Name != "FreeCam")
            {
                blackDialog2.SetDiaglog(s, true);
            }
            else
            {
                blackDialog.SetDiaglog(s, true);
            }
        }
        else if (owner == "M")
        {
            playerDialog.SetDiaglog(s, true);
        }

    }

    public void CloseAllDialogs()
    {
        blackDialog.CloseDialog();
        blackDialog2.CloseDialog();
        playerDialog.CloseDialog();
    }

    public void UpdateBlackWords()//更新黑衣人的文本
    {
        if (BlackBeginWords.Count > 0)
        {
            if (BlackBeginWords_owner_Dic[BlackBeginWords[0]] == "M")
            {
                // BlackWordsObj.SetActive(true);
                // BlackWordsText.text = BlackBeginWords[0];
                playerDialog.SetDiaglog(BlackBeginWords[0]);

            }
            else if (BlackBeginWords_owner_Dic[BlackBeginWords[0]] == "B")
            {
                // BlackWordsObj.SetActive(false);


                BlackSpeak(BlackBeginWords[0]);
            }
            else if (BlackBeginWords_owner_Dic[BlackBeginWords[0]] == "A0")
            {
                AIMechanism.Instance.AI_Chas[0].dialog_Big.SetDiaglog(BlackBeginWords[0]);
            }
            BlackBeginWords.RemoveAt(0);

        }
        else
        {
            // BlackWordsObj.SetActive(false);
        }


    }



    public void BlackMove()
    {
        sm.StartMove();
    }

    public void OpenSwicthBall(int transform_Index)
    {
        switchBall_Instance = Instantiate(switchBall, switchBall_trans[transform_Index]);
        StartCoroutine(SwitchBallLarger());
    }

    public void CloseSwicthBall(int transform_Index)
    {
        switchBall_Instance.transform.position = switchBall_trans[transform_Index].position;
        switchBall_Instance.GetComponent<MeshRenderer>().material.SetFloat("_isColor", 1);
        StartCoroutine(SwitchBallSmaller());
    }

    IEnumerator SwitchBallLarger()
    {
        while (true)
        {
            if (timer_switchBall > switchBallExTime)
            {
                timer_switchBall = 0;
                switchBall_Instance.GetComponent<MeshRenderer>().material.SetFloat("_isColor", 0);
                yield break;
            }

            timer_switchBall += Time.deltaTime;
            float factor = timer_switchBall / switchBallExTime;
            float scale = Mathf.Lerp(0, 20, factor);
            switchBall_Instance.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }


    IEnumerator SwitchBallSmaller()
    {
        while (true)
        {
            if (timer_switchBall > switchBallExTime)
            {
                timer_switchBall = 0;
                Destroy(switchBall_Instance);
                yield break;
            }

            timer_switchBall += Time.deltaTime;
            float factor = timer_switchBall / switchBallExTime;
            float scale = Mathf.Lerp(0, 15, 1 - factor);
            switchBall_Instance.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    public bool O_C_BeginAni()
    {

        isBeginAni = !isBeginAni;

        return isBeginAni;
    }

    public void Stranger_SitSofa()//把stranger塞到位置上坐着
    {
        sm.gameObject.transform.position = new Vector3(-0.1111711f, -0.741f, -4.71612f);
        sm.gameObject.transform.rotation = Quaternion.Euler(0, -10.919f, 0);
    }

    public void Stranger_SitDesk()
    {
        sm.gameObject.transform.position = new Vector3(-0.1111711f, -0.741f, -0.412f);
        sm.gameObject.transform.rotation = Quaternion.Euler(0, -10.919f, 0);
    }

    // public void OpenNpcGameObj(string s)
    // {
    //     foreach (var NPC_GameObj in NPC_GameObjs)
    //     {
    //         if (NPC_GameObj.name != s)
    //         {
    //             NPC_GameObj.SetActive(false);
    //         }
    //         else
    //         {
    //             NPC_GameObj.SetActive(true);
    //         }
    //     }
    //     sm.gameObject.SetActive(false);//关闭神秘人
    // }
    public void NPC_SitDesk(GameObject g)
    {
        g.transform.position = new Vector3(-0.1111711f, -0.741f, -0.412f);
        g.transform.rotation = Quaternion.Euler(0, -10.919f, 0);
    }


}
