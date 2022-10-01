using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//厚黑欲
public struct Personality//厚黑欲
{
    public int thick;
    public int black;
    public int greed;
}
//流派
public enum Type//流派
{
    OverLoad,//过载
    Skilled,//精技
    Snaky,//暗算
    Tolerant,//忍耐
    Fishlike,//摸鱼
    Sociable,//善舞
    Flexible,//隐世
    Revolting//叛乱

}

//PSWK的消耗或者生产能力
public struct PSWK_Ability
{
    public int P;
    public int S;
    public int W;
    public int K;
}

public class Buffer//每周会复制一份PSWK_Ability
{
    public int P = 0;
    public int S = 0;
    public int W = 0;
    public int K = 0;

    public Buffer()
    {
        this.P = 0;
        this.S = 0;
        this.W = 0;
        this.K = 0;
    }

    public Buffer(PSWK_Ability pSWK_Ability)
    {
        this.P = pSWK_Ability.P;
        this.S = pSWK_Ability.S;
        this.W = pSWK_Ability.W;
        this.K = pSWK_Ability.K;
    }
}

public class AIData//对应PlayerData
{
    public int AIid;//是第几个机器人
    //外显属性
    public string name;
    public int physicalHealth = 100;
    public int physicalHealthMax = 100;
    public int spiritualHealth = 100;
    public int spiritualHealthMax = 100;
    public int workAbility = 10;
    public int postLevel = 1;
    public int KPI = 0;
    public int ranking = 1;
    public int favor = 100;
    public int joinWeek;//哪一周加入的
    public float ATK_Desire = 0;//进攻欲望，0-100
    public float Def_Ability = 0;//反制主角debuff的能力，0-100

    //内藏属性
    // [HideInInspector] public Personality personality;
    [HideInInspector] public Type type;
    [HideInInspector] public PSWK_Ability PSWK_Ability;
    [HideInInspector] public Buffer buffer;


    public AIData(int AIid1)//根据玩家信息，环境卷度生成AI
    {
        this.AIid = AIid1;
        this.buffer = new Buffer();
        this.joinWeek = Mechanism.Instance.week;

        //自动生成生成流派
        AI_Personlity_Type();
        //自动生成名字
        AI_Name();
        //自动生成P、S、W数据，还有一些潜藏数据ATK_Desire Def_Ability
        AI_PSW_Max();
        //自动根据W生成职位
        AI_PostLevel();
        //自动生成改变各项基础数值(PSWK)的能力
        AI_PSWK_Ability();
    }
    void AI_Name()//根据人物生成名字
    {
        // if (AIMechanism.Instance.Type_AINames_Dic[type].Count > 0)
        // {
        //     List<string> list = AIMechanism.Instance.Type_AINames_Dic[type];
        //     string n = list[Random.Range(0, list.Count)];
        //     this.name = n;
        //     AIMechanism.Instance.Type_AINames_Dic[type].Remove(n);
        // }
        // else
        // {
        //     this.name = "无名指" + Random.Range(1, 1000);
        // }
        this.name = AIMechanism.Instance.AI_Chas[this.AIid].Name;
    }
    void AutoTypeProDic_PDF(Dictionary<Type, float> Type_Pro_Dic_PDF)//各个流派的比例
    {
        //75个
        Type_Pro_Dic_PDF.Add(Type.Fishlike, 15);
        Type_Pro_Dic_PDF.Add(Type.OverLoad, 30);
        Type_Pro_Dic_PDF.Add(Type.Skilled, 15);
        Type_Pro_Dic_PDF.Add(Type.Snaky, 15);

        //25个
        Type_Pro_Dic_PDF.Add(Type.Tolerant, 10);
        Type_Pro_Dic_PDF.Add(Type.Sociable, 5);
        Type_Pro_Dic_PDF.Add(Type.Flexible, 7);
        Type_Pro_Dic_PDF.Add(Type.Revolting, 3);

        //环境越卷，后期流派就越多,会告诉玩家卷没有用。前期确实卷了有用，因为越卷后期流派越多，可以暴揍后期流派。但是后期就没用了
        Type_Pro_Dic_PDF[Type.Sociable] += (int)(Mechanism.Instance.envirRollValue * 0.05f);
        Type_Pro_Dic_PDF[Type.Flexible] += (int)(Mechanism.Instance.envirRollValue * 0.05f);
        Type_Pro_Dic_PDF[Type.Revolting] += (int)(Mechanism.Instance.envirRollValue * 0.05f);
    }

    void AutoTypeProDic_CDF(Dictionary<Type, float> Type_Pro_Dic_PDF, Dictionary<Type, float> Type_Pro_Dic_CDF)
    {
        float n = 0;

        n += Type_Pro_Dic_PDF[Type.Fishlike];
        Type_Pro_Dic_CDF.Add(Type.Fishlike, n);
        n += Type_Pro_Dic_PDF[Type.OverLoad];
        Type_Pro_Dic_CDF.Add(Type.OverLoad, n);
        n += Type_Pro_Dic_PDF[Type.Skilled];
        Type_Pro_Dic_CDF.Add(Type.Skilled, n);
        n += Type_Pro_Dic_PDF[Type.Snaky];
        Type_Pro_Dic_CDF.Add(Type.Snaky, n);
        n += Type_Pro_Dic_PDF[Type.Tolerant];
        Type_Pro_Dic_CDF.Add(Type.Tolerant, n);
        n += Type_Pro_Dic_PDF[Type.Sociable];
        Type_Pro_Dic_CDF.Add(Type.Sociable, n);
        n += Type_Pro_Dic_PDF[Type.Flexible];
        Type_Pro_Dic_CDF.Add(Type.Flexible, n);
        n += Type_Pro_Dic_PDF[Type.Revolting];
        Type_Pro_Dic_CDF.Add(Type.Revolting, n);

        List<Type> keyList = new List<Type>(Type_Pro_Dic_CDF.Keys);
        // foreach (var key in keyList)
        // {
        //     Debug.Log(key.ToString());
        // }
        for (int i = 0; i < keyList.Count; i++)
        {
            Type_Pro_Dic_CDF[keyList[i]] = Type_Pro_Dic_CDF[keyList[i]] / n;
        }
    }


    void AI_Personlity_Type()//影响因素，环境卷度
    {
        Dictionary<Type, float> Type_Pro_Dic_PDF = new Dictionary<Type, float>();
        AutoTypeProDic_PDF(Type_Pro_Dic_PDF);
        Dictionary<Type, float> Type_Pro_Dic_CDF = new Dictionary<Type, float>();
        AutoTypeProDic_CDF(Type_Pro_Dic_PDF, Type_Pro_Dic_CDF);

        float r = Random.Range(0, 1f);
        if (r < Type_Pro_Dic_CDF[Type.Fishlike])
        {
            type = Type.Fishlike;
        }
        else if (r < Type_Pro_Dic_CDF[Type.OverLoad])
        {
            type = Type.OverLoad;
        }
        else if (r < Type_Pro_Dic_CDF[Type.Skilled])
        {
            type = Type.Skilled;
        }
        else if (r < Type_Pro_Dic_CDF[Type.Snaky])
        {
            type = Type.Snaky;
        }
        else if (r < Type_Pro_Dic_CDF[Type.Tolerant])
        {
            type = Type.Tolerant;
        }
        else if (r < Type_Pro_Dic_CDF[Type.Sociable])
        {
            type = Type.Sociable;
        }
        else if (r < Type_Pro_Dic_CDF[Type.Flexible])
        {
            type = Type.Flexible;
        }
        else
        {
            type = Type.Revolting;
        }


    }

    //
    void AI_PSW_Max()//影响因素 玩家强度
    {
        this.physicalHealthMax = (int)(PlayerData.Instance.physicalHealthMax);//* Random.Range(0.8f, 1.2f)
        this.physicalHealth = this.physicalHealthMax;
        this.spiritualHealthMax = (int)(PlayerData.Instance.spiritualHealthMax);//* Random.Range(0.8f, 1.2f)
        this.spiritualHealth = this.spiritualHealthMax;
        this.workAbility = (int)(PlayerData.Instance.workAbility);//* Random.Range(0.8f, 1.2f)
        this.KPI = 0;

        switch (this.type)
        {
            case Type.Fishlike:
                this.ATK_Desire = 10;
                this.Def_Ability = 0;
                this.physicalHealthMax = (int)(this.physicalHealthMax * 1.2f);
                this.spiritualHealthMax = (int)(this.spiritualHealthMax * 1.2f);
                this.workAbility = (int)(this.workAbility * 0.5f);
                break;
            case Type.Flexible:
                this.ATK_Desire = 0;
                this.Def_Ability = 100;
                this.spiritualHealthMax = (int)(this.spiritualHealthMax * 1.2f);
                break;
            case Type.OverLoad:
                this.ATK_Desire = 10;
                this.Def_Ability = 0;
                this.physicalHealthMax = (int)(this.physicalHealthMax * 0.9f);
                this.spiritualHealthMax = (int)(this.spiritualHealthMax * 0.9f);
                this.workAbility = (int)(this.workAbility * 0.7f);
                break;
            case Type.Revolting:
                this.ATK_Desire = 20;
                this.Def_Ability = 40;
                this.workAbility = (int)(this.workAbility * 1.3f);
                break;
            case Type.Skilled:
                this.ATK_Desire = 0;
                this.Def_Ability = 0;
                this.workAbility = (int)(this.workAbility * 1.5f);
                break;
            case Type.Snaky:
                this.ATK_Desire = 80;
                this.Def_Ability = 0;
                this.spiritualHealthMax = (int)(this.spiritualHealthMax * 1.2f);
                this.workAbility = (int)(this.workAbility * 0.5f);
                break;
            case Type.Sociable:
                this.ATK_Desire = 20;
                this.Def_Ability = 40;
                this.workAbility = (int)(this.workAbility * 1.3f);
                break;
            case Type.Tolerant:
                this.ATK_Desire = 10;
                this.Def_Ability = 20;
                break;
            default:
                break;
        }

        //受流派和周数影响的算法，暂时不写
        /*
           OverLoad,//过载
            Skilled,//精技
            Snaky,//暗算
            Tolerant,//忍耐
            Fishlike,//摸鱼
            Sociable,//善舞
            Flexible,//隐世
            Revolting//叛乱
            */
        // switch (this.type)
        // {
        //     case Type.
        // }

    }

    void AI_PSWK_Ability()//影响因素：职位等级， AI流派因游戏时间而出现的特性，（每个流派在前中后期都有不同的发挥）//其实还和熟练度有关，不过刚来的人没有熟练度，这个可以每周增加
    {
        int week = Mechanism.Instance.week;
        if (Mechanism.Instance.week <= 2)//2*工作 1*青椒
        {
            PSWK_Ability.P = -30;
            PSWK_Ability.S = -30;
            PSWK_Ability.W = 20;
            PSWK_Ability.K = 40;
        }
        else
        {
            PSWK_Ability.P = (int)(Mechanism.Instance.physicalHealthAverage);//* Random.Range(0.8f, 1.2f)
            PSWK_Ability.P = Mathf.Min(-30, PSWK_Ability.P);

            PSWK_Ability.S = (int)(Mechanism.Instance.spiritualHealthAverage);//* Random.Range(0.8f, 1.2f)
            PSWK_Ability.S = Mathf.Min(-30, PSWK_Ability.S);

            PSWK_Ability.W = (int)(Mechanism.Instance.workAbilityAverage * 1.1f);//* Random.Range(0.8f, 1.2f)
            PSWK_Ability.K = (int)(Mechanism.Instance.KPIAverage * 1.1f);//* Random.Range(0.8f, 1.2f)
        }
        switch (this.type)
        {
            case Type.Fishlike:
                PSWK_Ability.P = 0;
                PSWK_Ability.S = 0;
                PSWK_Ability.W = (int)(PSWK_Ability.W * 0.5f);
                PSWK_Ability.K = (int)(PSWK_Ability.K * 0.5f);
                break;
            case Type.Flexible:
                PSWK_Ability.P = (int)(PSWK_Ability.P * 0.7f);
                PSWK_Ability.S = (int)(PSWK_Ability.S * 0.7f);
                PSWK_Ability.K = (int)(PSWK_Ability.K * 0.9f);
                break;
            case Type.OverLoad:
                PSWK_Ability.P = (int)(PSWK_Ability.P * 1.2f);
                PSWK_Ability.S = (int)(PSWK_Ability.S * 1.2f);
                PSWK_Ability.W = (int)(PSWK_Ability.W * 0.7f);
                PSWK_Ability.K = (int)(PSWK_Ability.K * 1.2f);
                break;
            case Type.Revolting:
                PSWK_Ability.W = (int)(PSWK_Ability.W * 1.3f);
                PSWK_Ability.K = (int)(PSWK_Ability.K * 1.2f * (1 + week * 0.02f));
                break;
            case Type.Skilled:
                PSWK_Ability.W = (int)(PSWK_Ability.W * 1.5f);
                if (week <= 20)
                {
                    PSWK_Ability.K = (int)(PSWK_Ability.K * 1.1f * (1 + week * 0.02f));
                }
                else
                {
                    PSWK_Ability.K = (int)(PSWK_Ability.K * 1.2f);
                }
                break;
            case Type.Snaky:
                PSWK_Ability.W = (int)(PSWK_Ability.W * 0.5f);
                PSWK_Ability.K = (int)(PSWK_Ability.K * 0.5f);
                break;
            case Type.Sociable:
                PSWK_Ability.W = (int)(PSWK_Ability.W * 1.3f);
                PSWK_Ability.K = (int)(PSWK_Ability.K * 1.2f * (1 + week * 0.02f));
                break;
            case Type.Tolerant:
                break;
            default:
                break;
        }

        //受流派和周数影响的算法，暂时不写
    }
    public bool AI_PostLevel()//自动升级
    {
        bool b = false;
        if (this.postLevel < 5)
        {
            if (this.workAbility >= Mechanism.Instance.postUpgradeNeeds[this.postLevel - 1])
            {
                b = true;
            }
        }

        //根据工作能力来判断职位等级
        if (this.workAbility < Mechanism.Instance.postUpgradeNeeds[0])
        {
            this.postLevel = 1;
        }
        else if (this.workAbility >= Mechanism.Instance.postUpgradeNeeds[0] && this.workAbility < Mechanism.Instance.postUpgradeNeeds[1])
        {
            this.postLevel = 2;
        }
        else if (this.workAbility >= Mechanism.Instance.postUpgradeNeeds[1] && this.workAbility < Mechanism.Instance.postUpgradeNeeds[2])
        {
            this.postLevel = 3;
        }
        else if (this.workAbility >= Mechanism.Instance.postUpgradeNeeds[2] && this.workAbility < Mechanism.Instance.postUpgradeNeeds[3])
        {
            this.postLevel = 4;
        }
        else if (this.workAbility >= Mechanism.Instance.postUpgradeNeeds[3])
        {
            this.postLevel = 5;
        }

        return b;

    }

    public void AI_PSWK_Ability_Exchange_EveryWeek()//每周增长的PSWK_Ability
    {
        // int week = Mechanism.Instance.week;
        float n;
        if (Mechanism.Instance.week <= 1 + 4 * 4)
        {
            n = (Mechanism.Instance.KPI_Up_PerWeek_1_Min + Mechanism.Instance.KPI_Up_PerWeek_1_Max) / 2;
        }
        else
        {
            n = (Mechanism.Instance.KPI_Up_PerWeek_9_Min + Mechanism.Instance.KPI_Up_PerWeek_9_Max) / 2;
        }
        PSWK_Ability.P = (int)(PSWK_Ability.P * 1.02);//这部分还是需要看卡牌的统计数据
        PSWK_Ability.S = (int)(PSWK_Ability.S * 1.02);
        PSWK_Ability.W = (int)(PSWK_Ability.W * n);
        PSWK_Ability.K = (int)(PSWK_Ability.K * n);//这一项还会受环境卷度的影响
                                                   //受流派和周数影响的算法，暂时不写
    }
    public void AI_PSW_Max_Exchange_EveryWeek()//用buffer改变pswk 并且计算玩家释放的debuff！！！！！！！！！！！！！！！！！！！
    {
        int week = Mechanism.Instance.week;
        int physicalHealthMaxAdd = (int)(physicalHealthMax * 0.05f);//* (1 + 0.1f * week)
        physicalHealthMax += physicalHealthMaxAdd;
        int spiritualHealthMaxAdd = (int)(spiritualHealthMax * 0.05f);//* (1 + 0.1f * week)
        spiritualHealthMax += spiritualHealthMaxAdd;
        physicalHealth = Mathf.Clamp(physicalHealth + physicalHealthMaxAdd + buffer.P, -10000, physicalHealthMax);
        spiritualHealth = Mathf.Clamp(spiritualHealth + spiritualHealthMaxAdd + buffer.S, -10000, spiritualHealthMax);
        workAbility += buffer.W;
        KPI += buffer.K;//* Random.Range(0.5f, 1.5f)
    }


}
public class AIMechanism : MonoSingleton<AIMechanism>//对应Mechanism
{
    // public
    public List<AI_Cha> AI_Chas = new List<AI_Cha>();
    public AI_Cha Leader;
    // [HideInInspector] public List<AIData> AIDatas = new List<AIData>();
    public List<GameObject> AI_poses = new List<GameObject>();//AI打击主角，AI被主角打击的pos
    public List<GameObject> Leader_poses = new List<GameObject>();//领导的pos

    //即将都用不到的文字属性
    // public List<TextMeshPro> names = new List<TextMeshPro>();
    // // public List<TextMeshPro> names_W = new List<TextMeshPro>();//工作日的名字
    // public List<GameObject> names_W_Gameobject = new List<GameObject>();//上一项所在的gameobject，后面可以做缩放

    // public List<TextMeshPro> physicalHealthTexts = new List<TextMeshPro>();
    // public List<TextMeshPro> spiritualHealthTexts = new List<TextMeshPro>();
    // public List<TextMeshPro> workAbilityTexts = new List<TextMeshPro>();
    // public List<TextMeshPro> postLevelTexts = new List<TextMeshPro>();
    // public List<TextMeshPro> KPITexts = new List<TextMeshPro>();
    // public List<TextMeshPro> rankingTexts = new List<TextMeshPro>();
    // public List<TextMeshPro> favorTexts = new List<TextMeshPro>();
    public GameObject AISendShitPose;

    //AI
    [Range(0, 1)]
    public float AISendCardpro_Global = 0.5f;

    public int AI_Appear_Week = 5;

    //AI自动命名
    // public TextAsset AIDataAsset;
    // public Dictionary<Type, List<string>> Type_AINames_Dic = new Dictionary<Type, List<string>>();

    void Awake()
    {
        // LoadAINameFormData();
    }
    /*
    void LoadAINameFormData()
    {
        string[] dataRows = AIDataAsset.text.Split("\r\n", System.StringSplitOptions.RemoveEmptyEntries);
        Type_AINames_Dic.Add(Type.Fishlike, new List<string>());
        Type_AINames_Dic.Add(Type.Flexible, new List<string>());
        Type_AINames_Dic.Add(Type.OverLoad, new List<string>());
        Type_AINames_Dic.Add(Type.Revolting, new List<string>());
        Type_AINames_Dic.Add(Type.Skilled, new List<string>());
        Type_AINames_Dic.Add(Type.Snaky, new List<string>());
        Type_AINames_Dic.Add(Type.Sociable, new List<string>());
        Type_AINames_Dic.Add(Type.Tolerant, new List<string>());
        foreach (var dataRow in dataRows)
        {
            string[] elements = dataRow.Split(',');
            if (elements[0] == "")//排除空行的影响
            {
                continue;
            }
            switch (elements[1])
            {
                case "摸鱼":
                    Type_AINames_Dic[Type.Fishlike].Add(elements[0]);
                    break;
                case "隐世":
                    Type_AINames_Dic[Type.Flexible].Add(elements[0]);
                    break;
                case "过载":
                    Type_AINames_Dic[Type.OverLoad].Add(elements[0]);
                    break;
                case "叛乱":
                    Type_AINames_Dic[Type.Revolting].Add(elements[0]);
                    break;
                case "精技":
                    Type_AINames_Dic[Type.Skilled].Add(elements[0]);
                    break;
                case "暗算":
                    Type_AINames_Dic[Type.Snaky].Add(elements[0]);
                    break;
                case "善舞":
                    Type_AINames_Dic[Type.Sociable].Add(elements[0]);
                    break;
                case "忍耐":
                    Type_AINames_Dic[Type.Tolerant].Add(elements[0]);
                    break;
                default:
                    break;
            }

        }

    }

*/
    public GameObject SearchAIPoses(int id, bool isLeader = false)//寻找AI小布偶的动作
    {
        GameObject tmp = null;
        List<GameObject> prefabs;
        if (isLeader)
        {
            prefabs = Leader_poses;
        }
        else
        {
            prefabs = AI_poses;
        }

        foreach (var AI_pose in prefabs)
        {
            if ("Pose" + id.ToString() == AI_pose.name)
            {
                tmp = AI_pose;
                break;
            }
        }
        // if (tmp == null)
        // {
        //     tmp = AI_poses[0];
        // }
        return tmp;
    }






    void UpdateAIData2UGUI()//更新现有AI数据到UI，现在不太一样了
    {
        return;
        // foreach (var AI_Cha in AI_Chas)
        // {
        //     AIData AIData = AI_Cha.AIData;
        //     if (AIData != null)
        //     {
        //         names[AIData.AIid].text = AIData.name;
        //         // names_W[AIData.AIid].text = AIData.name;
        //         physicalHealthTexts[AIData.AIid].text = AIData.physicalHealth.ToString() + "/" + AIData.physicalHealthMax.ToString();
        //         spiritualHealthTexts[AIData.AIid].text = AIData.spiritualHealth.ToString() + "/" + AIData.spiritualHealthMax.ToString();
        //         workAbilityTexts[AIData.AIid].text = AIData.workAbility.ToString();
        //         KPITexts[AIData.AIid].text = AIData.KPI.ToString();
        //         postLevelTexts[AIData.AIid].text = AIData.postLevel.ToString();
        //         rankingTexts[AIData.AIid].text = AIData.ranking.ToString();
        //         favorTexts[AIData.AIid].text = AIData.favor.ToString();
        //         PlayerData.Instance.JudgeRankingColor(AIData.ranking, rankingTexts[AIData.AIid], KPITexts[AIData.AIid]);
        //     }
        // }
    }

    public int CalculateAIDataCount()//计算现有的AIData数量
    {
        int n = 0;
        foreach (var AI_Cha in AI_Chas)
        {
            if (AI_Cha.AIData != null)
            {
                n++;
            }
        }
        return n;

    }

    public void FillAIDatas()//填充AIDatas这个list直到满
    {
        //总体而言就是看看没有哪个id的AI，就填充这个ID的AI
        while (CalculateAIDataCount() < 4)
        {
            List<int> idList = new List<int>();
            idList.AddRange(new List<int> { 0, 1, 2, 3 });
            foreach (var AI_Cha in AI_Chas)
            {
                AIData AIData = AI_Cha.AIData;
                if (AIData != null)
                {
                    if (AIData.AIid == 0 || AIData.AIid == 1 || AIData.AIid == 2 || AIData.AIid == 3)
                    {
                        idList.Remove(AIData.AIid);
                    }
                }
            }
            foreach (var id in idList)
            {
                AIData AIDataNew = new AIData(id);
                AI_Chas[id].AIData = AIDataNew;
                AI_Chas[id].workAbilityMatChangeColor();
                Mechanism.Instance.AICome(AIDataNew.name);
                if (Mechanism.Instance.week != AI_Appear_Week)
                {
                    Leader.NewAiNames.Add(AI_Chas[id].Name);
                }
            }
        }

    }

    public void AI_Exchange_OnClickButton()//在点击按钮时，自动将的pswkAbility给AI的buff
    {
        if (Mechanism.Instance.week == AI_Appear_Week || Mechanism.Instance.week > AI_Appear_Week && Mechanism.Instance.week % 4 == 1)//修改同事出现的时刻
        {
            FillAIDatas();    //增加新同事
        }

        UpdateAIData2UGUI();
        foreach (var AI_Cha in AI_Chas)
        {
            AIData AIData = AI_Cha.AIData;
            if (AIData != null)
            {
                AIData.buffer = new Buffer(AIData.PSWK_Ability);
            }
        }
    }
    public bool AI_Exchange_Assign_Queue16()//queue16发生//AI送主角屎的环节 //返回是否送屎
    {
        if (Mechanism.Instance.week <= AI_Appear_Week)//AI出现的第一周不会有送屎环节
        {
            return false;
        }


        bool isSend = false;
        List<AIData> AIDatas_Bad = new List<AIData>();//伤害主角的坏人


        foreach (var AI_Cha in AI_Chas)
        {
            AIData AIData = AI_Cha.AIData;
            if (AIData != null)
            {
                float r = Random.Range(0f, 1);
                //事实上r还会因AI的流派而变化
                if (r <= AISendCardpro_Global + AIData.ATK_Desire / 100f)//全局+个体的攻击欲望
                {
                    isSend = true;
                    Card card = CardStore.Instance.RandomCard_AI(AIData.postLevel);
                    card.creator = AIData.name;
                    card.AutoCardInfor();
                    PlayerData.Instance.playerCards.Add(card);
                    PlayerData.Instance.SortCards();
                    // LibraryManager.Instance.UpdateLibrary();
                    AIDatas_Bad.Add(AIData);
                    //播放特效
                    AI_Cha.SendShit(card);
                    // SimpleEffect se = names_W_Gameobject[AIData.AIid].GetComponent<SimpleEffect>();
                    // se.InstantiateCard(card, 0);
                    // StartCoroutine(se.Buff());

                    if (TeachManager.Instance.isFirstBeSentShit)
                    {
                        TeachManager.Instance.isFirstBeSentShit = false;
                        TeachManager.Instance.TeachEventTrigger_Delay("被陷害介绍", 4f);
                    }
                }
            }


        }
        if (isSend)
        {
            Mechanism.Instance.AISendCardtoPlayerSign(AIDatas_Bad);
        }
        return isSend;

    }

    public void AI_Exchange_WeekendMeeting()//每周AI末干的事情
    {
        int week = Mechanism.Instance.week;

        foreach (var AI_Cha in AI_Chas)
        {
            AIData AIData = AI_Cha.AIData;
            if (AIData != null)
            {
                if (AIData != null)
                {
                    AIData.AI_PSW_Max_Exchange_EveryWeek();//用buffer改变pswk
                    // AIData.AI_PostLevel();//AI自动升级  //自动升级职位
                }
            }
        }

        RankingAllByKPI();//排名
        CalculateKPITop();//计算最高的KPI
        //计算卷度，计算主角这一周和AI这一周的平均KPI数
        CalculateEnvirRollValue();


        //如果有你要开除的员工
        List<AIData> AIDatas_Remove = FieldManager.Instance.AIDatas_Remove;
        List<AIData> aIDatas = new List<AIData>();
        foreach (var AI_Cha in AI_Chas)
        {
            if (AI_Cha.AIData != null)
            {
                aIDatas.Add(AI_Cha.AIData);
            }
        }
        for (int i = 0; i < AIDatas_Remove.Count; i++)
        {
            if (aIDatas.Contains(AIDatas_Remove[i]))//因为有可能，某个人被除名了很多次，这是为了防止反复移除而报错
            {
                AIRemove(AIDatas_Remove[i], "你的决定");
            }
        }
        FieldManager.Instance.AIDatas_Remove.Clear();

        //如果有猝死的员工
        List<AIData> AIDatas_Temp = new List<AIData>(aIDatas);//如果直接用AIDatas会报错
        for (int i = 0; i < AIDatas_Temp.Count; i++)
        {
            if (AIDatas_Temp[i].physicalHealth <= 0 || AIDatas_Temp[i].spiritualHealth <= 0)
            {
                AIRemove(AIDatas_Temp[i], "猝死");
            }
        }

        //每月一次的结算动画
        //动画XXX因 绩效不合格/猝死/升迁/剧情 等原因离开了这个办公室
        if (week % 4 == 0)
        {
            Evalute_EveryMonth();
        }
        UpdateAIData2UGUI();

        //提升AI的能力（隐性）
        foreach (var AIData in aIDatas)
        {
            AIData.AI_PSWK_Ability_Exchange_EveryWeek();//每周增长的PSWK_Ability
        }


        ////播放KPIBar增长的动画，被剔除的AI不再播放动画
        foreach (var AI_Cha in AI_Chas)
        {

            AIData AIData = AI_Cha.AIData;
            if (AIData != null)
            {
                AI_Cha.Mini_Weekend.SetActive(true);
                AI_Cha.KPIBar_Ex();
            }
            else
            {
                AI_Cha.Mini_Weekend.SetActive(false);
            }
        }
        PlayerData.Instance.player_AI_Cha.KPIBar_Ex();//播放主角的KPI变化动画
        Leader.KPIBar_Ex();//事实上只是做了一个reset

    }


    void RankingAllByKPI()//计算所有对象的排名
    {
        List<int> ids = new List<int>();
        List<int> KPIs = new List<int>();
        foreach (var AI_Cha in AI_Chas)
        {
            AIData AIData = AI_Cha.AIData;
            if (AIData != null)
            {
                ids.Add(AIData.AIid);
                KPIs.Add(AIData.KPI);
            }
        }
        ids.Add(5);
        KPIs.Add(PlayerData.Instance.KPI);

        for (int i = 0; i < ids.Count - 1; i++)
        {
            for (var j = 0; j < ids.Count - 1 - i; j++)
            {
                if (KPIs[j] > KPIs[j + 1])
                {
                    int temp = KPIs[j];
                    KPIs[j] = KPIs[j + 1];
                    KPIs[j + 1] = temp;

                    int temp2 = ids[j];
                    ids[j] = ids[j + 1];
                    ids[j + 1] = temp2;
                }
            }
        }

        foreach (var id in ids)
        {
            int ranking = ids.Count - ids.IndexOf(id);
            if (id < 5)//AI
            {
                AI_Chas[id].AIData.ranking = ranking;
                // AIDatas[id].ranking = ranking;
            }
            else
            {
                PlayerData.Instance.ranking = ranking;
            }
        }

    }
    void Evalute_EveryMonth()//每月评估:计算环境卷度 人员剔除
    {
        //计算上个月的总KPI
        FieldManager.Instance.KPIAll = CalculateAllKPI();
        //末位淘汰


        List<AIData> aIDatas = new List<AIData>();
        foreach (var AI_Cha in AI_Chas)
        {
            if (AI_Cha.AIData != null)
            {
                aIDatas.Add(AI_Cha.AIData);
            }
        }
        List<AIData> AIDatas_Temp = new List<AIData>(aIDatas);//如果直接用AIDatas会报错
        for (int i = 0; i < AIDatas_Temp.Count; i++)
        {
            if (AIDatas_Temp[i].ranking == AIDatas_Temp.Count + 1)//算上主角，如果AI还排最后，那就滚
            {
                AIRemove(AIDatas_Temp[i], "排名过于靠后");
            }
            // else if (AIDatas_Temp[i].KPI < Mechanism.Instance.KPINeed_EveryMonth * (5 - AIDatas_Temp[i].joinWeek % 4))//*(5-AIDatas_Temp[i].joinWeek)是为了保证月中来的AI不至于被光速淘汰
            // {
            //     AIRemove(AIDatas_Temp[i], "未达成本月KPI");
            // }
        }

    }
    void AIRemove(AIData AIData, string reason)
    {
        List<Card> playerCards = PlayerData.Instance.playerCards;
        for (var i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i].creator == AIData.name)
            {
                playerCards.RemoveAt(i);
            }
        }
        PlayerData.Instance.SortCards();


        // AIDatas.Remove(AIData);
        AI_Chas[AIData.AIid].AIData = null;

        // names[AIData.AIid].text = "-";
        // names_W[AIData.AIid].text = "-";
        // physicalHealthTexts[AIData.AIid].text = "-";
        // spiritualHealthTexts[AIData.AIid].text = "-";
        // workAbilityTexts[AIData.AIid].text = "-";
        // KPITexts[AIData.AIid].text = "-";
        // postLevelTexts[AIData.AIid].text = "-";
        // rankingTexts[AIData.AIid].text = "-";
        // favorTexts[AIData.AIid].text = "-";
        Mechanism.Instance.AILeaveSign(AIData.name, reason);
        // Leader.LeaderWeekendSpeak(AIData.name, reason);
        if (!Leader.names_reasons_Dic.ContainsKey(AIData.name))
        {
            Leader.names_reasons_Dic.Add(AIData.name, reason);
        }

    }
    void CalculateEnvirRollValue()
    {
        if (Mechanism.Instance.week == 1)//第一周由于开头的buff不录入，所以无法计算环境卷度
        {
            return;
        }
        int KPIs = 0;
        int n = 0;
        foreach (var AI_Cha in AI_Chas)
        {
            AIData AIData = AI_Cha.AIData;
            if (AIData != null)
            {
                KPIs += AIData.buffer.K;
                n++;
            }
        }
        KPIs += Mechanism.Instance.functionEffectBuffer.KPI;
        // KPIs += PlayerData.Instance.KPI;
        n += 1;
        KPIs /= n;
        Mechanism.Instance.envirRollValue = KPIs;
        Mechanism.Instance.envirRollValueText.text = Mechanism.Instance.envirRollValue.ToString();
    }
    public int CalculateAllKPI()//计算所有的KPI
    {
        int n = 0;
        foreach (var AI_Cha in AI_Chas)
        {
            AIData AIData = AI_Cha.AIData;
            if (AIData != null)
            {
                n += AIData.KPI;
            }
        }
        n += PlayerData.Instance.KPI;
        return n;
    }

    public AIData CalculateBestAI(string property)//找到最xxx的AI
    {
        List<AIData> aIDatas = new List<AIData>();
        foreach (var AI_Cha in AI_Chas)
        {
            if (AI_Cha.AIData != null)
            {
                aIDatas.Add(AI_Cha.AIData);
            }
        }

        AIData AIData_Temp = aIDatas[0];

        foreach (var AIData in aIDatas)
        {
            if (property == "Ka1")//创造KPI的能力第一的AI
            {
                if (AIData_Temp.PSWK_Ability.K < AIData.PSWK_Ability.K)
                    AIData_Temp = AIData;
            }
        }
        return AIData_Temp;
    }


    public AIData RandomAIData()//返回随机一个在场角色的AIDATA
    {
        AIData AIData = null;
        List<AIData> aIDatas = new List<AIData>();
        foreach (var AI_Cha in AI_Chas)
        {
            if (AI_Cha.AIData != null)
            {
                aIDatas.Add(AI_Cha.AIData);
            }
        }
        if (aIDatas.Count > 0)
        {
            AIData = aIDatas[Random.Range(0, aIDatas.Count)];
        }
        return AIData;
    }

    [HideInInspector] public int KPITopThisWeek = 0;//本周最高的KPI数值
    public void CalculateKPITop()//计算最高的KPI数值
    {
        int KPITop = 0;
        foreach (var AI_Cha in AI_Chas)
        {
            if (AI_Cha.AIData != null)
            {
                if (AI_Cha.AIData.KPI >= KPITop)
                {
                    KPITop = AI_Cha.AIData.KPI;
                }
            }
        }
        if (PlayerData.Instance.KPI >= KPITop)
        {
            KPITop = PlayerData.Instance.KPI;
        }
        KPITopThisWeek = Mathf.Max(KPITop, 1);//到时候这个数是要被除的，所以不能为0
    }


    public void O_C_AllAIModel(bool b)
    {
        foreach (var AI_Cha in AI_Chas)
        {
            AI_Cha.Big.SetActive(b);
        }
    }
}
