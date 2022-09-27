using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BranchPlot//分支剧情
{
    public string name;//1_2 1_3
    public float pro;//概率
    public List<string> words = new List<string>();
    public string reward;//奖励
    // public PlayableAsset pa;//对应的timeline播放片段

    public BranchPlot(string name1, float pro1, List<string> words1, string reward1)//, PlayableAsset pa1
    {
        this.name = name1;
        this.pro = pro1;
        this.words = words1;
        this.reward = reward1;
        // this.pa = pa1;
    }
}

public class Plot
{
    public string aside_Start;//开始旁白
    public string ownerName;//剧情的从属者姓名
    public NPC owner;
    public List<string> condis = new List<string>();
    public List<string> words1 = new List<string>();//选择前的台词
    public List<string> chooses = new List<string>();//选择
    public int id;
    public Dictionary<string, List<BranchPlot>> choose_Branch_Dic = new Dictionary<string, List<BranchPlot>>();
    // public Dictionary<string, List<float>> Choose_Probability_Dic = new Dictionary<string, List<float>>();//一个选择对应多个概率
    // public Dictionary<float, List<string>> Probability_words2_Dic = new Dictionary<float, List<string>>();//一个概率对应一系列台词
    // public Dictionary<float, string> Probability_Reward_Dic = new Dictionary<float, string>();//一个概率对应一个奖励
    public string aside_End;//结尾旁白
    public Plot(string aside_Start, List<string> condis, List<string> words1, List<string> chooses,
    // Dictionary<string, List<float>> Choose_Probability_Dic, Dictionary<float, List<string>> Probability_words2_Dic, Dictionary<float, string> Probability_Reward_Dic,
   Dictionary<string, List<BranchPlot>> choose_Branch_Dic,
    string aside_End,
    string ownerName,
    int ID)
    {
        this.aside_Start = aside_Start;
        this.condis = condis;
        this.words1 = words1;
        this.chooses = chooses;
        // this.Choose_Probability_Dic = Choose_Probability_Dic;
        // this.Probability_words2_Dic = Probability_words2_Dic;
        // this.Probability_Reward_Dic = Probability_Reward_Dic;
        this.choose_Branch_Dic = choose_Branch_Dic;
        this.aside_End = aside_End;
        this.id = ID;
        foreach (var NPC in StoryManager.Instance.NPCs)
        {
            if (NPC.name == ownerName)
            {
                owner = NPC;
            }
        }
    }

    public bool JudgeCondition()
    {
        bool b = true;
        //所有条件必须都满足
        foreach (var condi in condis)
        {
            if (condi == "加班一次")
            {
                bool c = false;
                //检测是否有加班
                foreach (var card in Mechanism.Instance.cardList)
                {
                    if (card.finalTitle.Contains("加班"))
                    {
                        c = true;
                        break;
                    }
                }
                // c = true;//这个是debug用的，记得删除
                if (c)//认识了NPC
                {
                    foreach (var NPC in StoryManager.Instance.NPCs)
                    {
                        if (NPC == owner)
                        {
                            NPC.isKnown = true;
                            // if (!StoryManager.Instance.NPCs_HasFound.Contains(NPC))
                            // {
                            //     StoryManager.Instance.NPCs_HasFound.Add(NPC);
                            // }
                        }
                    }
                }
                //这里注意不能写b=c，因为b=true这样的语句会混淆逻辑
                if (c == false)
                {
                    b = false;
                }

            }
            else if (condi == "认识")
            {
                // Debug.Log(owner.name);
                // Debug.Log(owner.isKnown);
                if (!owner.isKnown)
                {
                    b = false;
                }
            }
            else if (condi.Contains("Period"))
            {

                string[] elements = condi.Split('|', System.StringSplitOptions.RemoveEmptyEntries);

                if (elements[1] != StoryManager.Instance.period.ToString())//如果时期对上了
                {
                    b = false;
                }

            }
            else if (condi.Contains("Probability"))
            {
                string[] elements = condi.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
                float r = Random.Range(0, 1f);
                if (float.Parse(elements[1]) < r)//如果时期对上了
                {
                    b = false;
                }
            }
        }

        return b;
    }
}
