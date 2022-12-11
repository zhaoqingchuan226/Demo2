using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NeedType//Need的类型，月中还是月末
{
    Normal,
    MoothEnd
}

public enum NormalTemplate//Need的类型，月中还是月末
{
    无,
    能力,
    体力,
    精力,
    绩效
}
public enum MoothEndTemplate//Need的类型，月中还是月末
{
    无,
    绩效,
    身体,//剩余的体力
    精神,//剩余的精力
    态度,//消耗的精力
    卷度,//消耗的体力
    能力
}
public class Need
{

    public NeedType needType;
    public NormalTemplate normalTemplate;
    public MoothEndTemplate moothEndTemplate;
    public int K_Origin = 0;//原始的难度
    public int K = 0;
    // public int P = 0;
    // public int S = 0;
    public int W = 0;
    public float P_Remain = 1;
    public float S_Remain = 1;
    public float P_Consume = 0;
    public float S_Consume = 0;
    public string des;
    public string reward = null;//达成小目标的奖励
    List<NormalTemplate> normalTemplates_All = new List<NormalTemplate>
  { NormalTemplate.能力,
    NormalTemplate.体力,
   NormalTemplate.精力
   };

    List<MoothEndTemplate> moothEndTemplates_All = new List<MoothEndTemplate>
  { MoothEndTemplate.绩效,
    MoothEndTemplate.身体,//剩余的体力
    MoothEndTemplate.精神,//剩余的精力
    MoothEndTemplate.态度,//消耗的精力
    MoothEndTemplate.卷度,//消耗的体力
    MoothEndTemplate.能力
   };
    public List<string> testResults = new List<string>();
    public string results;
    void ResetAll()
    {
        K_Origin = 0;//原始的难度
        K = 0;
        // P = 0;
        // S = 0;
        W = 0;
        P_Remain = 1;
        S_Remain = 1;
        P_Consume = 0;
        S_Consume = 0;
        testResults.Clear();
        results = null;
    }



    public Need(
        int K_Origin1, NeedType needType1,
    MoothEndTemplate moothEndTemplate1 = MoothEndTemplate.无,
    NormalTemplate normalTemplate1 = NormalTemplate.无
    )//自动生成新的KPI需求
    {
        ResetAll();
        K_Origin = K_Origin1;
        int week = Mechanism.Instance.week;
        if (needType1 == NeedType.MoothEnd)
        {
            needType = NeedType.MoothEnd;
            if (moothEndTemplate1 == MoothEndTemplate.无)
            {
                moothEndTemplate = moothEndTemplates_All[Random.Range(0, moothEndTemplates_All.Count)];
            }
            else
            {
                moothEndTemplate = moothEndTemplate1;
            }

            switch (moothEndTemplate)
            {
                case MoothEndTemplate.绩效:
                    K = K_Origin;
                    break;
                case MoothEndTemplate.身体:
                    K = (int)(K_Origin * 0.7f);
                    P_Remain = 0.8f;
                    break;
                case MoothEndTemplate.精神:
                    K = (int)(K_Origin * 0.7f);
                    S_Remain = 0.8f;
                    break;
                case MoothEndTemplate.态度:
                    K = (int)(K_Origin * 0.8f);
                    S_Consume = 0.6f;
                    break;
                case MoothEndTemplate.卷度:
                    K = (int)(K_Origin * 0.8f);
                    P_Consume = 0.6f;
                    break;
                case MoothEndTemplate.能力:
                    K = (int)(K_Origin * 0.7f);
                    W = 40 * (week / 4 + 1);//week / 4 + 1是月数
                    break;
                default:
                    break;
            }
            AutoDes(moothEndTemplate);
        }
        else
        {
            needType = NeedType.Normal;
            if (normalTemplate1 == NormalTemplate.无)
            {
                normalTemplate = normalTemplates_All[Random.Range(0, normalTemplates_All.Count)];
            }
            else
            {
                normalTemplate = normalTemplate1;
            }

            switch (normalTemplate)
            {
                case NormalTemplate.绩效:
                    K = K_Origin / 4 * (week % 4);
                    // reward = "Money";
                    break;
                case NormalTemplate.能力:
                    W = 10 * week;
                    break;
                case NormalTemplate.体力:
                    P_Remain = 0.8f;
                    break;
                case NormalTemplate.精力:
                    S_Remain = 0.8f;
                    break;
                default:
                    break;
            }
            AutoDes(normalTemplate);
        }
    }
    void AutoDes(NormalTemplate normalTemplate1)
    {
        des = null;
        des += "小目标：" + normalTemplate1.ToString();
        des += "\r\n";
        des += "<color=white>";
        switch (normalTemplate1)
        {
            case NormalTemplate.体力:
                des += "体力剩余" + P_Remain * 100 + "%";
                break;
            case NormalTemplate.精力:
                des += "精力剩余" + S_Remain * 100 + "%";
                break;
            case NormalTemplate.能力:
                des += "能力达到" + W;
                break;
            case NormalTemplate.绩效:
                des += "绩效达到" + K;
                break;
            default:
                break;
        }
        des += "</color>";
    }
    void AutoDes(MoothEndTemplate moothEndTemplate1)
    {
        des = null;
        // des += moothEndTemplate1.ToString() + "检测:" + "\r\n";

        switch (moothEndTemplate1)
        {
            case MoothEndTemplate.身体:
                des += "体力剩余" + P_Remain * 100 + "%";
                des += "\r\n";
                break;
            case MoothEndTemplate.精神:
                des += "精力剩余" + S_Remain * 100 + "%";
                des += "\r\n";
                break;
            case MoothEndTemplate.卷度:
                des += "体力消耗" + P_Consume * 100 + "%";
                des += "\r\n";
                break;
            case MoothEndTemplate.态度:
                des += "精力消耗" + S_Consume * 100 + "%";
                des += "\r\n";
                break;
            case MoothEndTemplate.能力:
                des += "能力达到" + W;
                des += "\r\n";
                break;
            default:
                break;
        }

        des += "绩效达到" + K;
    }

    public bool TestIsOK()
    {
        int week = Mechanism.Instance.week;
        bool b = false;
        if (needType == NeedType.MoothEnd)
        {
            switch (moothEndTemplate)
            {
                case MoothEndTemplate.身体:
                    b = TestProperty("P_Remain") && TestProperty("K");
                    break;
                case MoothEndTemplate.精神:
                    b = TestProperty("S_Remain") && TestProperty("K");
                    break;
                case MoothEndTemplate.卷度:
                    b = TestProperty("P_Consume") && TestProperty("K");
                    break;
                case MoothEndTemplate.态度:
                    b = TestProperty("S_Consume") && TestProperty("K");
                    break;
                case MoothEndTemplate.能力:
                    b = TestProperty("W") && TestProperty("K");
                    break;
                default:
                    break;
            }
            results += "本月" + moothEndTemplate.ToString() + "测试";
            if (b)
            {
                results += "达标";
            }
            else
            {
                results += "未达标";
            }
            results += "\r\n";

            for (int i = 0; i < testResults.Count; i++)
            {
                results += testResults[i];
                if (i != testResults.Count - 1)
                {
                    results += "\r\n";
                }
            }

        }
        else
        {
            switch (normalTemplate)
            {
                case NormalTemplate.体力:
                    b = TestProperty("P_Remain");
                    break;
                case NormalTemplate.精力:
                    b = TestProperty("S_Remain");
                    break;
                case NormalTemplate.能力:
                    b = TestProperty("W");
                    break;
                case NormalTemplate.绩效:
                    b = TestProperty("K");
                    break;
                default:
                    break;
            }
            results += "小目标：";


            for (int i = 0; i < testResults.Count; i++)
            {
                results += testResults[i];
                if (i != testResults.Count - 1)
                {
                    results += "\r\n";
                }
            }
        }
        return b;
    }

    bool TestProperty(string pro)
    {
        bool b = false;
        string s = null;
        switch (pro)
        {
            case "P_Remain":
                b = PlayerData.Instance.physicalHealth / PlayerData.Instance.physicalHealthMax >= P_Remain;
                s += "体力剩余";
                break;
            case "S_Remain":
                b = PlayerData.Instance.spiritualHealth / PlayerData.Instance.spiritualHealthMax >= S_Remain;
                s += "精力剩余";
                break;
            case "P_Consume":
                b = 1 - PlayerData.Instance.physicalHealth / PlayerData.Instance.physicalHealthMax >= P_Consume;
                s += "体力消耗";
                break;
            case "S_Consume":
                b = 1 - PlayerData.Instance.spiritualHealth / PlayerData.Instance.spiritualHealthMax >= S_Consume;
                s += "精力消耗";
                break;
            case "W":
                b = PlayerData.Instance.workAbility >= W;
                s += "能力";
                break;
            case "K":
                b = PlayerData.Instance.KPI >= K;
                s += "绩效";
                break;
            default:
                break;
        }
        if (b)
        {
            s += "达标";
        }
        else
        {
            s += "未达标";
            s = "<color=red>" + s + "</color>";
        }
        testResults.Add(s);
        return b;
    }

    public string TargetTiTle()
    {
        string s = null;
        switch (moothEndTemplate)
        {
            case MoothEndTemplate.绩效:
                s += "绩效疯狗";
                break;
            case MoothEndTemplate.身体:
                s += "健康壮汉";
                break;
            case MoothEndTemplate.精神:
                s += "麻木走狗";
                break;
            case MoothEndTemplate.态度:
                s += "养心专家";
                break;
            case MoothEndTemplate.卷度:
                s += "思维卷王";
                break;
            case MoothEndTemplate.能力:
                s += "技术大牛";
                break;
            default: break;
        }
        return s;
    }
}
