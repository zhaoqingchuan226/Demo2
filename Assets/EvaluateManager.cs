using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EvaluateType
{
    猝死,
    开除,
    胜利
}
public class EvaluateManager : MonoSingleton<EvaluateManager>
{
    [HideInInspector] EvaluateType evaluateType = EvaluateType.猝死;
    public GameObject Evaluation_Panel;
    public TextMeshProUGUI tmp;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    [HideInInspector] public Dictionary<Type, int> CardType_Amout_Dic = new Dictionary<Type, int>();//玩家某类型卡牌的数量
    [HideInInspector] public int[] month_Salary = new int[12];
    List<Type> types_rank = new List<Type>();//卡牌流派按数量排序，多的放前面
    public Dictionary<NPC, int> NPC_Favor_Dic = new Dictionary<NPC, int>();//NPC的亲密度（遇到过几次）
    public Dictionary<int, int> CardID_EffectTimes_Dic = new Dictionary<int, int>();//卡牌id和使用次数
    public Image player_photo;//主角照片
    public GameObject duilian;//死亡对联
    public Sprite alive;
    public Sprite die;
    [Range(0, 1f)] public float Trigger_difficulty = 1;//触发特殊描述的难度
    private void Awake()
    {
        ResetCardType_Amout_Dic();
        types_rank = new List<Type> { Type.OverLoad, Type.Skilled, Type.Snaky, Type.Tolerant, Type.Fishlike, Type.Sociable, Type.Flexible, Type.Revolting };
        // NPC_Favor_Dic.Add(StoryManager.Instance.NPCs_HasFound[0], 0);
        // NPC_Favor_Dic.Add(StoryManager.Instance.NPCs_HasFound[1], 0);
        // NPC_Favor_Dic.Add(StoryManager.Instance.NPCs_HasFound[2], 0);
        // NPC_Favor_Dic.Add(StoryManager.Instance.NPCs_HasFound[3], 0);
        // NPC_Favor_Dic.Add(StoryManager.Instance.NPCs_HasFound[4], 0);
        // NPC_Favor_Dic.Add(StoryManager.Instance.NPCs_HasFound[5], 0);
        // NPC_Favor_Dic.Add(StoryManager.Instance.NPCs_HasFound[6], 0);
        // NPC_Favor_Dic.Add(StoryManager.Instance.NPCs_HasFound[7], 0);
    }




    // void Update()
    // {
    //     // if (Input.GetKeyDown(KeyCode.A))
    //     // {
    //     //     OpenEvaluation_Panel();
    //     // }
    //     // else if (Input.GetKeyDown(KeyCode.D))
    //     // {
    //     //     CloseEvaluation_Panel();
    //     // }


    // }
    public void OpenEvaluation_Panel(int t = 0)
    {
        switch (t)
        {
            case 0:
                evaluateType = EvaluateType.猝死;
                break;
            case 1:
                evaluateType = EvaluateType.开除;
                break;
            case 2:
                evaluateType = EvaluateType.胜利;
                break;
            default:
                break;
        }
        Evaluation_Panel.SetActive(true);
        if (evaluateType == EvaluateType.猝死)
        {
            player_photo.sprite = die;
            duilian.SetActive(true);
        }
        else
        {
            player_photo.sprite = alive;
            duilian.SetActive(false);
        }
        string s = null;
        TypeRange();//计数并排序现有卡库里的流派种类,返回一个排序队列，数量多的卡牌类型放前面
        Title(ref s);
        GloryName(ref s);
        FavorAll(ref s);
        NPCEvaluate(ref s);
        MoneyTest(ref s);
        TypeDes(ref s);
        SpecialAction(ref s);
        End(ref s);
        if (Mechanism.Instance.week <= 8)
        {
            s = "短短两月都无法生存的小角色，无人书写他的故事。";
        }
        tmp.text = s;
    }
    public void CloseEvaluation_Panel()
    {
        Evaluation_Panel.SetActive(false);
    }

    void Title(ref string s)
    {
        switch (evaluateType)
        {
            case EvaluateType.猝死:
                s += "尊敬的各位领导和同事：";
                s += "\r\n";
                s += "    青山不语，苍天含泪！";
                s += "\r\n";
                s += "    今日，我们怀着万分悲痛的心境，深切悼念";
                break;
            case EvaluateType.开除:
                s += "各位领导和同事，今日，让我们含泪送别我们的同事——";
                break;
            case EvaluateType.胜利:
                s += "各位领导和同事，今日，让我们含泪送别我们的同事——";
                break;
            default:
                break;
        }
    }


    void GloryName(ref string s)//荣誉名称
    {
        switch (PlayerData.Instance.postLevel)
        {
            case 1:
                s += "T1初级员工";
                break;
            case 2:
                s += "T2中级员工";
                break;
            case 3:
                s += "T3高级员工";
                break;
            case 4:
                if (types_rank.IndexOf(Type.Skilled) <= types_rank.IndexOf(Type.Sociable))
                {
                    s += "T4专家";
                }
                else
                {
                    s += "T4主管";
                }
                break;
            case 5:
                if (types_rank.IndexOf(Type.Skilled) <= types_rank.IndexOf(Type.Sociable))
                {
                    s += "T5高级专家";
                }
                else
                {
                    s += "T5总监";
                }
                break;
            default:
                break;
        }
        s += PlayerData.Instance.Name + "先生。";


    }
    void FavorAll(ref string s)//人品 受欢迎度
    {
        if (PlayerData.Instance.favorAll >= 300)
        {
            s += PlayerData.Instance.Name + "先生向来与人为善，有求必应，古道热肠，作风正派。乐于帮助同事解决问题，并积极组织公司团建活动，在同事眼中好似战友，胜似亲人。";
        }
        else if (PlayerData.Instance.favorAll >= 100)
        {
            s += PlayerData.Instance.Name + "先生向来人缘好，善于交际。在解决自己工作问题之余，时常帮助同事。对于夜晚加班的女同事，也积极地护送她们回家。";
        }
        else if (PlayerData.Instance.favorAll >= 20)
        {
            s += PlayerData.Instance.Name + "先生向来安分守己，寡言少语。从不拉帮结派，惹是生非。安于自己的一隅而少问喧嚣的世事。";
        }
        else
        {
            s += PlayerData.Instance.Name + "先生向来坏事做绝，丧尽天良，罪不容诛。几乎到了被所有同事都唾弃的地步，不唾弃他的同事比他还坏。";
        }
    }

    void NPCEvaluate(ref string s)//NPC的评价
    {
        List<NPC> NPCs = StoryManager.Instance.NPCs;
        if (NPC_Favor_Dic[NPCs[0]] >= 1)
        {
            s += @"有着""公司卷王""之称的非雨曾提到" + PlayerData.Instance.Name + "弟弟总是尽可能地做好自己的本职工作，偶尔甚至会自愿加班来完成任务。";
        }
        else if (NPC_Favor_Dic[NPCs[0]] >= 2)
        {
            s += @"有着""公司卷王""之称的非雨曾提到" + PlayerData.Instance.Name + "弟弟虽然莽撞，但一直是一个贴心可爱的人。";
        }

        if (NPC_Favor_Dic[NPCs[1]] >= 1)
        {
            s += "技术大牛月雾曾直言虽然" + PlayerData.Instance.Name + "技术水平一般，但是请教的态度一直很端正。";
        }
        else if (NPC_Favor_Dic[NPCs[1]] >= 2)
        {
            s += "技术大牛月雾曾直言虽然" + PlayerData.Instance.Name + "现在能力有欠缺，但是有着极高的技术天赋。";
        }
        s += "\r\n";
    }
    void MoneyTest(ref string s)
    {
        int n = 0;
        for (var i = 0; i < month_Salary.Length; i++)
        {
            n += month_Salary[i];
        }
        n /= Mathf.Max(1, Mechanism.Instance.week / 4);//常提拔美丽端庄的女士，成为公司公关的中流砥柱
        if (n >= 8000)
        {
            s += "    " + PlayerData.Instance.Name + "极其富裕且乐善好施，常帮助洗浴中心的失足妇女走出贫困，改邪归正。";
        }
        else if (n >= 3000)
        {
            s += "    " + PlayerData.Instance.Name + "经济条件不错，年纪轻轻一个月工资就超过三千。常出席于富人的party上，拾起掉落的戒指、项链、耳环。";
        }
        else
        {
            s += "    " + PlayerData.Instance.Name + "极度贫穷，曾因被商店贩子笑穷而大打出手，最终两人双双倒地。" + PlayerData.Instance.Name + "先生继续被商店里的鱼嘲讽。";
        }
    }

    void TypeDes(ref string s)//流派特征描述
    {

        string ss = null;
        for (var i = 0; i < 3; i++)
        {
            if (CardType_Amout_Dic[types_rank[i]] >= 4)
            {
                switch (i)
                {
                    case 0:
                        ss += "大多数时间";
                        break;
                    case 1:
                        ss += "时常";
                        break;
                    case 2:
                        ss += "偶尔";
                        break;
                    default:
                        break;
                }
                switch (types_rank[i])
                {
                    case Type.Fishlike:
                        ss += "自我放逐，寻求安逸，工作摆烂，热衷摸鱼。";
                        break;
                    case Type.Flexible:
                        ss += "看破世俗，厌倦争斗，在大事件中表现得游刃有余，最终全身而退。";
                        break;
                    case Type.OverLoad:
                        ss += "废寝忘食，自我透支，将行军床铺在办公桌旁，尽情熬夜加班。";
                        break;
                    case Type.Revolting:
                        ss += "伪装自我，谋划大事，策动人心，为举起叛乱的大旗做准备。";
                        break;
                    case Type.Skilled:
                        ss += "闭门造车，专精技术，发表了一篇又一篇会议论文，研究的算法通用于业界。";
                        break;
                    case Type.Snaky:
                        ss += "贪图小利，损人利己，化作一根搅屎棍，将办公室弄得乌烟瘴气。";
                        break;
                    case Type.Sociable:
                        ss += "知人善任，任人唯贤，招揽贤才，统御贤能，是同事们眼中的好领导，是领导们眼中的好同事。";
                        break;
                    case Type.Tolerant:
                        ss += "忍受苦痛，化血为脓。宁可在沉默中变态，也不在沉默中爆发。";
                        break;
                    default:

                        break;
                }
            }
        }
        if (ss != null)
        {
            s += PlayerData.Instance.Name + "先生" + ss;
        }
    }
    void SA(ref string ss, int cardid, int triggerTimes, string des)
    {
        if (CardID_EffectTimes_Dic.ContainsKey(cardid))
        {
            if (CardID_EffectTimes_Dic[cardid] >= (int)(triggerTimes * Trigger_difficulty))
            {
                ss += CardID_EffectTimes_Dic[cardid].ToString() + "次" + des;
                ss += "；";
            }
        }
    }

    void SA(ref string ss, List<int> cardids, int triggerTimes, string des)
    {

        bool b = false;
        foreach (var cardid in cardids)
        {
            if (CardID_EffectTimes_Dic.ContainsKey(cardid))
            {
                b = true;
                break;
            }
        }
        int n = 0;
        if (b)
        {
            foreach (var cardid in cardids)
            {
                if (CardID_EffectTimes_Dic.ContainsKey(cardid))
                {
                    n += CardID_EffectTimes_Dic[cardid];
                }
            }
        }
        if (n >= triggerTimes)
        {
            ss += n.ToString() + "次" + des;
            ss += "；";
        }



    }

    void SpecialAction(ref string s)//由卡牌带来的特殊成就
    {

        string ss = null;
        SA(ref ss, 4, 20, "顶着疲劳连轴转加班，得到了雨田君的赏识");
        SA(ref ss, 6, 20, "流水线式地产出，想必那时他的内心是麻木的");
        SA(ref ss, 7, 15, "在猝死的边缘疯狂试探，那时他眼中出现了幻影，却依然兴奋异常");
        SA(ref ss, new List<int> { 8, 9, 10 }, 10, "触碰了禁忌，在燃烧自己的同时有了狂热的信仰");
        SA(ref ss, 11, 20, "自我提升，在工作之余不忘提高能力，是同事们眼中的自学狂魔");
        SA(ref ss, 16, 30, "进入心流，大伙都评价，这是个在工作时心无旁骛的强人");
        SA(ref ss, 17, 10, "进行时间管理，因此有大量时间和女同事互动，被称为妇女之友");
        SA(ref ss, 20, 10, "自闭地一连几天不和人说话，直到别人扯掉你身上的蛛网");
        SA(ref ss, new List<int> { 21, 28, 29, 30 }, 50, "上班摸鱼，摸出了水平，摸出了风采，曾自言摸鱼时有宛若新生的快感");
        SA(ref ss, 22, 20, "午休，周围的键盘声丝毫不影响睡眠");
        SA(ref ss, 23, 20, "在下午三点喝茶，是大家眼中的提醒饮茶小助手");
        SA(ref ss, 26, 20, "和黏糊糊的神明做肮脏的交易，终究是有点坐不惯现在的椅子");
        SA(ref ss, 27, 15, "悠闲地看别人爆肝，他人的苦痛确实可以成为自己的快乐");
        SA(ref ss, 28, 10, "带薪拉屎，和领座双排，是领座眼中的好厕友");
        SA(ref ss, 29, 10, "在上班时间锻炼身体，公司健身房的教练表示非常感动，正因如此他才不至于失业");
        SA(ref ss, 30, 10, "在上班时间从事电竞活动，早已练成一招闪换界面来应对雨田君的查岗");
        SA(ref ss, 31, 10, "向领导打其他同事的小报告，让办公室人心惶惶");
        SA(ref ss, 32, 5, "抢占别人的功劳，项目组的其他人都苦不堪言，连美术的功劳都是你的");
        SA(ref ss, 33, 10, "挑拨离间他人，让项目组硝烟弥漫，被其他人誉为办公室搅屎棍");
        SA(ref ss, 35, 10, "往别人的杯子里倒入泻药，并锁上厕所门");
        SA(ref ss, 36, 5, "在领导面前夸下海口，但是无法满足领导的期望，最终被鄙视");
        SA(ref ss, 37, 5, @"联合关系好的同事排挤异己，将""顺我者昌，逆我者亡""作为座右铭");
        SA(ref ss, 38, 5, "以莫须有的罪名诬陷别人，颠倒黑白，让很多同事都对此颇有微词");
        SA(ref ss, 39, 5, "将自己的任务推给别人，忽悠领导让其他同事帮自己完成任务，让众多组员怨声载道");
        SA(ref ss, 40, 5, "做领导的舔狗，但除了减少工作量终是一无所有，升职什么的还是要靠自己");
        SA(ref ss, 41, 15, "强忍着同事的非难继续工作，将苦难化作动力，被领导看在眼里，记在心里");
        SA(ref ss, 43, 5, "麻痹自我，将小人的陷害当作是成长路上不可或缺的艰难关卡");
        SA(ref ss, 48, 15, "在小本本上记下陷害自己的同事，并发誓总有一天要复仇");
        SA(ref ss, 49, 15, "打击报复陷害自己的人，将所有的怨气在那一刻倾泻");
        SA(ref ss, 57, 10, @"给下属画饼，成为了下属和曾今的自己眼中""最讨厌的人""");
        SA(ref ss, new List<int> { 51, 52, 53, 54, 55 }, 20, "领导组员攻克难关，同舟共济，共享项目发行红利");
        SA(ref ss, new List<int> { 59, 60 }, 5, "开除组员，优化团队，却也心里过意不去");
        SA(ref ss, 62, 10, "依靠自己的丰富阅历解决问题，得到了同事们崇拜的目光");
        SA(ref ss, 63, 10, "在清闲中获得灵感，摆脱现实生活的沉重引力");

        if (ss != null)
        {
            s += PlayerData.Instance.Name + "曾";
            ss = ss.Substring(0, ss.ToCharArray().Length - 1);
            ss += "。";
            s += ss;
        }
        s += "\r\n";
    }



    void End(ref string s)
    {
        switch (evaluateType)
        {
            case EvaluateType.猝死:
                s += "    愿：" + PlayerData.Instance.Name + "先生一路走好！这个小盒才是你永远的家。";
                break;
            case EvaluateType.开除:
                s += "    祝" + PlayerData.Instance.Name + "先生前途无量，流落街头，爱埋哪埋哪。";
                break;
            case EvaluateType.胜利:
                s += "    祝" + PlayerData.Instance.Name + "先生事业有成，前程似锦。请相信公司永远是你的家。";
                break;
            default:
                break;
        }
    }

    void ResetCardType_Amout_Dic()
    {
        CardType_Amout_Dic.Clear();
        CardType_Amout_Dic.Add(Type.OverLoad, 0);
        CardType_Amout_Dic.Add(Type.Skilled, 0);
        CardType_Amout_Dic.Add(Type.Snaky, 0);
        CardType_Amout_Dic.Add(Type.Tolerant, 0);
        CardType_Amout_Dic.Add(Type.Fishlike, 0);
        CardType_Amout_Dic.Add(Type.Sociable, 0);
        CardType_Amout_Dic.Add(Type.Flexible, 0);
        CardType_Amout_Dic.Add(Type.Revolting, 0);
    }

    void TypeRange()//计数并排序现有卡库里的流派种类,返回一个排序队列，数量多的卡牌类型放前面
    {
        ResetCardType_Amout_Dic();

        //计数
        foreach (var card in PlayerData.Instance.playerCards)
        {
            if (card.id < 10000)
            {
                CardType_Amout_Dic[card.type]++;
            }
        }
        //冒泡排序，大的放后面

        Type temp;

        for (int i = 0; i < types_rank.Count - 1; i++)
        {
            for (var j = 0; j < types_rank.Count - 1 - i; j++)
            {
                if (CardType_Amout_Dic[types_rank[j]] > CardType_Amout_Dic[types_rank[j + 1]])
                {
                    temp = types_rank[j];
                    types_rank[j] = types_rank[j + 1];
                    types_rank[j + 1] = temp;
                }
            }
        }
        types_rank.Reverse();
    }

    public void AddEffectCard(Card card)
    {
        if (CardID_EffectTimes_Dic.ContainsKey(card.id))
        {
            CardID_EffectTimes_Dic[card.id]++;
        }
        else
        {
            CardID_EffectTimes_Dic.Add(card.id, 1);
        }
    }


}
