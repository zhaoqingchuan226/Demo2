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
    宣发中,
    宣发后,
    日常2,
    舞会前,
    舞会中,
    舞会后,
    日常3,
    G3A前,
    G3A中,
    G3A后,
    日常4,
    败露前,
    败露中,
    败露后,
    日常5,
    绝境前,
    绝境中,
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
    public GameObject chooseButtonPrefab;
    public GameObject wordsPannel;
    public GameObject choosePannel;
    public GameObject chooseGroup;
    public GameObject asidePannel;
    public TextMeshProUGUI asideText;
    public TextMeshProUGUI NPCNameText;
    public TextMeshProUGUI wordsText;



    public Period period = Period.日常1;
    public int week = 1;
    public int periodDays = 4;
    public float asideTime = 2f;
    public TextMeshProUGUI weekText;
    public TextMeshProUGUI periodText;
    public TextAsset StoryData;
    public List<NPC> NPCs = new List<NPC>();
    public List<NPC> NPCs_HasFound = new List<NPC>();//已经遇到的NPC，将会与这些NPC发生各种各样的剧情
    public List<Plot> plotsThisWeek = new List<Plot>();//这周所会发生的所有剧情
    public Plot plotNow;//现在正在发生的剧情
    bool input;//如何切换下一段对话
    bool isPlotProcessing;//剧情是否在进行
    PlotPhase plotPhase;//剧情现阶段执行到哪一步
    float pro;             //选择后所被发配到的概率
    public float timer = 0;
    bool isFirstThisPlotPhase = true;
    List<GameObject> buttons = new List<GameObject>();

    [Space]
    public bool isBeginAni = true;//是否有开场动画
    [HideInInspector] public List<string> BlackBeginWords = new List<string>();
    //黑衣人的对话框
    public GameObject BlackWordsObj;
    public TextMeshProUGUI BlackWordsText;

    //PD

    public PlayableDirector pd;
    public List<PlayableAsset> pas = new List<PlayableAsset>();

    //black sws
    public splineMove sm;

    //SwitchBall
    public GameObject switchBall;
    [HideInInspector] GameObject switchBall_Instance;
    public List<Transform> switchBall_trans = new List<Transform>();
    public float switchBallExTime = 1.5f;
    float timer_switchBall;

    void Awake()
    {
        LoadStoryDataFromCSV();
        isPlotProcessing = false;
    }
    void Start()
    {
        UpdataUI();
        asidePannel.SetActive(false);
        choosePannel.SetActive(false);
        wordsPannel.SetActive(false);
        NPCs_HasFound.Add(NPCs[0]);//默认就把非雨加到了剧情中去


    }
    void Update()
    {
        input = Input.GetMouseButtonDown(0);
        if (isPlotProcessing)
        {
            if (plotPhase == PlotPhase.aside_Start)
            {
                timer += Time.deltaTime;
                if (timer > asideTime)
                {
                    timer = 0;
                    asidePannel.SetActive(false);
                    plotPhase++;
                    isFirstThisPlotPhase = true;
                }
                else
                {
                    if (isFirstThisPlotPhase)
                    {
                        asidePannel.SetActive(true);
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
                    asidePannel.SetActive(false);
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
                        asidePannel.SetActive(true);
                        asideText.text = plotNow.aside_End;
                        isFirstThisPlotPhase = false;
                    }

                }
            }
            else if (plotPhase == PlotPhase.words1)
            {
                if (isFirstThisPlotPhase)
                {
                    wordsPannel.SetActive(true);
                    isFirstThisPlotPhase = false;
                    wordsText.text = plotNow.words1[0];
                }
                if (input)
                {
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
                    words2 = plotNow.Probability_words2_Dic[pro];
                    wordsText.text = words2[0];
                }
                if (input)
                {
                    List<string> words2 = new List<string>();
                    words2 = plotNow.Probability_words2_Dic[pro];
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
        List<float> pros = plotNow.Choose_Probability_Dic[choose];

        //不同选择随机概率
        float r = Random.Range(0f, 1f);
        string reward = null;
        if (pros.Count == 1)
        {
            pro = pros[0];
            reward = plotNow.Probability_Reward_Dic[pros[0]];
        }
        else if (pros.Count == 2)
        {
            if (r < pros[0])
            {
                pro = pros[0];
                reward = plotNow.Probability_Reward_Dic[pros[0]];
            }
            else
            {
                pro = pros[1];
                reward = plotNow.Probability_Reward_Dic[pros[1]];
            }
        }
        else if (pros.Count == 3)
        {
            if (r < pros[0])
            {
                pro = pros[0];
                reward = plotNow.Probability_Reward_Dic[pros[0]];
            }
            else if (r >= pros[0] && r < (pros[0] + pros[1]))
            {
                pro = pros[1];
                reward = plotNow.Probability_Reward_Dic[pros[1]];
            }
            else
            {
                pro = pros[2];
                reward = plotNow.Probability_Reward_Dic[pros[2]];
            }
        }
        //奖励物品
        Debug.Log(reward);

        //
        foreach (var button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
        plotPhase++;
        isFirstThisPlotPhase = true;
    }


    public void WeekAdd()
    {
        if (period == Period.绝境后)
        {
            return;
        }

        //日常1：1-4
        //事件前：5-6
        //事件：6
        //事件后：7-8
        //日常2：9-12


        if (week % (2 * periodDays) == 4 || week % (2 * periodDays) == 0 || (week % (2 * periodDays) == 6 && period.ToString().Contains("中")))
        {
            period++;
            week++;
        }
        else if (week % (2 * periodDays) == 6 && period.ToString().Contains("前"))
        {
            period++;
        }

        else
        {
            week++;
        }

        // if (week % periodDays == 0 && !period.ToString().Contains("后"))
        // {
        //     period++;
        // }
        // else if (period.ToString().Contains("后"))
        // {
        //     period++;
        //     week++;
        // }
        // else
        // {
        //     week++;
        // }

        UpdataUI();
        GetPlotThisWeek();//获取这周要发生的剧情
        ExecutePlotThisWeek();//执行这周要发生的剧情
    }
    void UpdataUI()
    {
        weekText.text = week.ToString();
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


    void ExecutePlotThisWeek()
    {
        if (plotsThisWeek.Count > 0)
        {
            plotNow = plotsThisWeek[0];
            isPlotProcessing = true;
            plotPhase = PlotPhase.aside_Start;
            plotsThisWeek.RemoveAt(0);
        }
        else
        {
            plotNow = null;
            isPlotProcessing = false;
            Debug.Log("没有剧情可以执行了");
        }
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
                Dictionary<string, List<float>> Choose_Probability_Dic = new Dictionary<string, List<float>>();
                Dictionary<float, List<string>> Probability_words2_Dic = new Dictionary<float, List<string>>();
                Dictionary<float, string> Probability_Reward_Dic = new Dictionary<float, string>();

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
                        List<float> pros = new List<float>();
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
                                string reward = elements3[5];
                                pros.Add(pro);

                                List<string> words2 = new List<string>();
                                for (int b = 7; b < elements3.Length; b++)
                                {
                                    if (elements3[b] != "" && elements3[b] != "\r")
                                    {
                                        words2.Add(elements3[b]);
                                    }
                                }
                                Probability_words2_Dic.Add(pro, words2);
                                Probability_Reward_Dic.Add(pro, reward);
                            }
                        }
                        Choose_Probability_Dic.Add(choose, pros);
                    }
                    else if (elements2[1] == "Aside_End")
                    {
                        Aside_End = elements2[7];
                        break;
                    }
                }
                Plot plot = new Plot(Aside_Start, condis, words1, chooses, Choose_Probability_Dic, Probability_words2_Dic, Probability_Reward_Dic, Aside_End, owner);

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
                    }
                }

            }
        }

    }

    public void SetPlayableAsset(string s)
    {
        foreach (var pa in pas)
        {
            if (s == pa.name)
            {
                pd.playableAsset = pa;
            }
        }
    }

    public void BeginAni()
    {
        Mechanism.Instance.playState = PlayState.Black;
        SetPlayableAsset("Begin");
        pd.Play();
    }
    public void KillAni()
    {
        Mechanism.Instance.CloseGlobalUI();
        // Mechanism.Instance.playState = PlayState.Black;
        SetPlayableAsset("Kill");
        pd.Play();
    }

    public void KillPos()//前往一个pos
    {
        sm.gameObject.transform.position = new Vector3(-0.839f, -0.741f, 1.08f);
        sm.gameObject.transform.rotation = Quaternion.Euler(0, 78.27f, 0);
    }

    public void UpdateBlackWords()//更新黑衣人的文本
    {
        if (BlackBeginWords.Count > 0)
        {
            BlackWordsObj.SetActive(true);
            BlackWordsText.text = BlackBeginWords[0];
            BlackBeginWords.RemoveAt(0);
        }
        else
        {
            BlackWordsObj.SetActive(false);
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


}
