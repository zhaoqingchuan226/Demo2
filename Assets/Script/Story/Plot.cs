using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plot
{
    public string aside_Start;//开始旁白
    public string ownerName;//剧情的从属者姓名
    public NPC owner;
    public List<string> condis = new List<string>();
    public List<string> words1 = new List<string>();//选择前的台词
    public List<string> chooses = new List<string>();//选择

    public Dictionary<string, List<float>> Choose_Probability_Dic = new Dictionary<string, List<float>>();//一个选择对应多个概率
    public Dictionary<float, List<string>> Probability_words2_Dic = new Dictionary<float, List<string>>();//一个概率对应一系列台词
    public Dictionary<float, string> Probability_Reward_Dic = new Dictionary<float, string>();//一个概率对应一个奖励
    public string aside_End;//结尾旁白
    public Plot(string aside_Start, List<string> condis, List<string> words1, List<string> chooses,
    Dictionary<string, List<float>> Choose_Probability_Dic, Dictionary<float, List<string>> Probability_words2_Dic, Dictionary<float, string> Probability_Reward_Dic,
    string aside_End,
    string ownerName
    )
    {
        this.aside_Start = aside_Start;
        this.condis = condis;
        this.words1 = words1;
        this.chooses = chooses;
        this.Choose_Probability_Dic = Choose_Probability_Dic;
        this.Probability_words2_Dic = Probability_words2_Dic;
        this.Probability_Reward_Dic = Probability_Reward_Dic;
        this.aside_End = aside_End;
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
                b = true;//之后再写
                foreach (var NPC in StoryManager.Instance.NPCs)
                {
                    if (NPC==owner)
                    {
                        NPC.isKnown = true;
                    }
                }
            }
            else if (condi == "认识")
            {
                Debug.Log(owner.name);
                Debug.Log(owner.isKnown);
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
        }
        return b;
    }
}
