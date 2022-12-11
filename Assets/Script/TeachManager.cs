using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeachManager : MonoSingleton<TeachManager>
{
    public bool isGuide = true;
    public GameObject BlackWord;// BlackWord下的continue事实上是挡住一切的遮罩
    // public TextMeshProUGUI word_Text;
    [HideInInspector] public List<string> words = new List<string>();//黑衣人将要说的所有话
    [HideInInspector] public Dictionary<string, string> words_owner_Dic = new Dictionary<string, string>();//黑衣人将要说的所有话
    [HideInInspector] public string currentEvent = null;//现在正在发生的事件，例如"开头介绍";



    public GameObject player_P;
    public GameObject player_S;
    public GameObject player_W;
    public GameObject KPIButton;
    public GameObject playe_Level;
    public GameObject playe_K;
    public GameObject playe_ranking;
    public GameObject playe_PY;
    public GameObject KPINeed;
    public GameObject LibraryButton;
    public GameObject StartButton;
    public GameObject ExecuteButton;
    public GameObject actionColor;
    public GameObject timeGuide;
    public GameObject desGuide;
    public GameObject qualityGuide;
    public GameObject noticeboard;




    [HideInInspector] public bool isFirstLibrary = true;
    [HideInInspector] public bool isFirstWeekendMeeting = true;
    [HideInInspector] public bool isFirstTriplet3_1 = true;//三选一界面中有机会三连
    [HideInInspector] public bool isFirstUpgrade = true;
    [HideInInspector] public bool isFirstTriplet = true;//是不是第一次场上三连
    [HideInInspector] public bool isFirstHolidayStore = true;//是不是第一次场上三连
    [HideInInspector] public bool isFirstSalary = true;//是不是第一次发薪水

    [HideInInspector] public bool isFirstAssignOver = true;//是不是第一次赋值完毕
    [HideInInspector] public bool isFirstBeSentShit = true;//是不是第一次被陷害

    public void SetGuide(GameObject obj, bool b)//打开/关闭物体的黄色亮闪引导，前提是物体的一级子物体中必须有名为Guide的物体
    {
        Transform t = obj.transform.Find("Guide");
        // Debug.Log(t.gameObject.name);
        if (t != null)
        {
            t.gameObject.SetActive(b);
        }
    }

    void AddWord(string s, string owner = "B")
    {
        words.Add(s);
        words_owner_Dic.Add(s, owner);
    }


    public void TeachEventTrigger(string EventName)
    {
        if (!isGuide)
        {
            return;
        }
        currentEvent = EventName;
        //根据发生的事件注入对话内容
        switch (EventName)
        {
            /*OK*/
            case "开头介绍":
                AddWord("记住，这间屋子里的野兽，不只有你们五个，和我肩膀上的一只");
                break;
            /*OK*/
            case "它的介绍":
                //黑夜怒吼音效播放
                AudioManager.Instance.PlayClip("Monster_growl_1");
                AddWord("还有在黑暗里垂涎欲滴的它");
                AddWord("还有我心里乱撞的小鹿", "M");
                AddWord("哦别当真，我这就去烤了我的小鹿", "M");
                break;
            /*OK*/
            case "眼球介绍":
                StoryManager.Instance.animator_black.SetTrigger("PointEye");
                AddWord("燃烧眼球，获得光亮");
                break;
            /*OK*/
            case "恐怖咀嚼":
                //咀嚼音效
                AddWord("眼球燃尽，夜幕落下，它就会吃掉你");
                break;
            /*OK*/
            case "绩效需求介绍":
                AddWord("每个月，你得获得足够多的绩效");
                AddWord("让拉奥孔先生的肚子变大，碰到按钮，才能获得新眼球");
                AddWord("简单来说，好好卖力，我给你让你活下去的眼球");
                AddWord("挺过十二个月，我就放你们走");
                // SetGuide(KPINeed, true);
                SetGuide(playe_K, true);
                SetGuide(KPIButton, true);
                break;
            /*OK*/
            case "生命介绍":
                AddWord("红手代表体力，蓝手代表精力");
                AddWord("手臂粗细显示健康状况");
                AddWord("消耗体力和精力，接着获得绩效");
                SetGuide(player_P, true);
                SetGuide(player_S, true);
                break;
            /*OK*/
            case "绩效介绍":
                AddWord("所以绩效就是中年男人的肚子吗？", "M");
                AddWord(@"用""体力""和""精力""这两只手臂干活，随后养大自己的肚子？", "M");
                AddWord("没错，宰相肚子里都是绩效");
                AddWord("那如果体力或精力没了，会怎么样？", "M");
                AddWord("手臂断掉，拉不住蛇");
                SetGuide(playe_K, true);
                break;
            /*OK*/
            case "断头台展示":
                //切镜头
                AddWord("啊哦，这个我知道，吸血鬼快乐台", "M");

                break;
            /*OK*/
            case "牌库介绍":
                AddWord("那绩效怎么获得？", "M");
                AddWord("查看你牌库里的卡牌，你将用它们拿绩效");
                SetGuide(LibraryButton, true);
                break;
            /*OK*/
            case "开始介绍":
                AddWord("接下来，开始你一周的工作吧");
                SetGuide(LibraryButton, false);
                SetGuide(StartButton, true);
                break;
            /*OK*/
            case "卡牌介绍":
                AddWord("红卡代表动，急功近利");
                AddWord("蓝卡代表静，注重健康");
                AddWord("现在只有红卡，因为你说过的");
                AddWord(@"""这个年纪，你睡得着觉？""");
                AddWord("你真是记仇，我都不记得这件事情了", "M");
                AddWord("顺便提一句，点击卡牌可以查看详细信息");
                SetGuide(actionColor, true);
                break;
            // case "卡牌介绍时间":
            //     AddWord("此处为卡牌出现的时间，例如 夜班 只可能在 晚上 的槽位出现");
            //     AddWord(" 上 中 下 则代表此卡牌会出现在上午、中午或者下午");
            //     SetGuide(timeGuide, true);
            //     break;
            // case "卡牌介绍描述":
            //     AddWord("而这里是卡牌的效果描述");
            //     SetGuide(desGuide, true);
            //     break;

            /*OK*/
            case "卡牌介绍品质":
                AddWord("这一块则代表着卡牌的等级");
                AddWord("白色卡是最基础的卡");
                AddWord("通过提升职位等级有机会获得更加高级的卡牌");
                SetGuide(qualityGuide, true);
                break;
            /*OK*/
            case "关闭牌库提示":
                AddWord("卡牌介绍完毕，再次点击 牌库 按钮关闭牌库");
                SetGuide(LibraryButton, true);
                break;
            /*OK*/
            case "能力介绍":
                AddWord("哦，这位性感先生头上是什么？", "M");
                AddWord(@"是一棵""卡牌树""，它周末将给予你果实");
                AddWord(@"你可以用卡牌产生的""能力""来培养它，它将结出更高等级的果实");
                // AddWord("变大？类似⚪起吗？", "M");
                // AddWord("随你怎么理解");
                SetGuide(player_W, true);
                break;
            /*OK*/
            case "周末选牌介绍":
                // AddWord("每周末晚上，树都会⚪起，随后喷射出不可描述的果实");
                // AddWord("话说清除啊喂，是卡牌！不是不可描述的果实", "M");
                AddWord("你每周有两次选择新卡牌的机会");
                AddWord("花开三朵，各表一枝，不同分支将走向不同结局");
                // AddWord("领导让你们提前一个晚上来上班，就是让你们好好制定下一周的计划");
                // AddWord("到底是谁定的这条破规则？");
                // AddWord("首先排除你的领导雨田君");
                SetGuide(ExecuteButton, true);
                break;
            /*OK*/
            case "选牌三连蛊惑":
                AddWord("三张重复的卡牌同时在场上，可以引起卡牌三连，增加卡牌熟练度");
                AddWord("选择这张牌，将获得三连");
                AddWord("选一选呗，它在发光欸");
                break;
            /*OK*/
            case "岗位升级介绍":
                AddWord("等你能力值到了一定程度，可以升级岗位");
                AddWord("也就是升级你的卡牌树");
                AddWord("可惜，现在能力还不够");
                AddWord("不知道你从T1初级员工到T2中级员工，需要要多久");
                SetGuide(player_W, true);
                SetGuide(playe_Level, true);
                SetGuide(ExecuteButton, true);
                break;
            // case "公告栏介绍":
            //     AddWord("这是公告牌，可以显示最近发生的一些事件");
            //     SetGuide(noticeboard, true);
            //     break;
            /*OK*/
            case "场上三连介绍":
                AddWord(@"三张相同卡牌会触发三连，他们会合体并强化，例如从""工作""变为""工作+1""");
                AddWord(@"这个""+1""代表熟练度,可以大量提升卡牌的数值");
                AddWord("此外，每次三连都可以额外选择一张强力卡牌");
                AddWord("啊，不停工作，然后变成一个熟练的工人");
                break;
            /*OK*/
            case "假日商店介绍":
                AddWord("在每周的工作结束后，周末面板有一定概率会出现");
                AddWord("你可以安排你的周末生活，当然，你得支付一点点费用");
                AddWord("赚钱，然后过上好生活，美好的循环，不是么");
                AddWord("不需要购买物品，那就点击商人，离开商店");
                break;
            /*OK*/
            case "工资结算介绍":
                AddWord("在每个月底，KPI会被清零，并根据KPI的数量为你发放工资");
                AddWord("是摸是卷，自己决定");
                AddWord("圈里的话，叫能者多劳");
                break;
            /*OK*/
            case "小组成员介绍":
                AddWord("看来你适应得不错，它们将加入你的战局");
                AddWord("每个月排名最后的那位，将会额外消耗一个眼球");
                AddWord("哦，我只是觉得，你们需要压力，它需要伙食");
                AddWord("这四位死亡后会复活，会用更强的角色重新加入你的游戏对局");
                StoryManager.Instance.animator_black.SetTrigger("Show");
                break;
            /*OK*/
            case "被陷害介绍":
                // AddWord("这是你的受欢迎度");
                // AddWord("如果你做了对不起同事的事情，他们就会讨厌你，可能还会背地里对你动手");
                AddWord("它们会骚扰你，往你的牌库里塞奇怪的东西");
                AddWord("那些卡牌会造成你的内耗");
                AddWord("你可以在牌库里找到它们");
                AddWord("看一看卡牌的创建者是何人，是谁，陷害了你");
                SetGuide(playe_PY, true);
                break;
            default:
                break;
        }

        //开启对话框并播放第一句话


        if (words.Count > 0)
        {
            BlackWord.SetActive(true);
            StoryManager.Instance.Speak_Teach(words[0], words_owner_Dic[words[0]]);
            words_owner_Dic.Remove(words[0]);
            words.RemoveAt(0);

        }
        else
        {
            BlackWord.SetActive(false);
            StoryManager.Instance.Speak_Teach(words[0], words_owner_Dic[words[0]]);
        }
    }

    public void Continue()
    {
        if (words.Count == 0)//如果没有下一句话了，就关闭对话框。如果有接着的对话，则开启
        {
            BlackWord.SetActive(false);
            // TimeManager.Instance.OnClickStopButton(1);
            StoryManager.Instance.CloseAllDialogs();


            switch (currentEvent)
            {
                case "开头介绍":
                    TeachEventTrigger("它的介绍");
                    break;
                case "它的介绍":
                    TeachEventTrigger("眼球介绍");
                    break;
                case "眼球介绍":
                    TeachEventTrigger("恐怖咀嚼");
                    break;
                case "恐怖咀嚼":
                    TeachEventTrigger("绩效需求介绍");
                    break;
                case "绩效需求介绍":
                    TeachEventTrigger("牌库介绍");
                    // SetGuide(KPINeed, false);
                    SetGuide(playe_K, false);
                    SetGuide(KPIButton, false);
                    break;
                case "生命介绍":
                    TeachEventTrigger("绩效介绍");
                    SetGuide(player_P, false);
                    SetGuide(player_S, false);
                    break;
                case "绩效介绍":
                    TeachEventTrigger_Delay("断头台展示", 0.5f);
                    SetGuide(playe_K, false);
                    CameraManager.Instance.SetVirtualCam("SnakeCam", 0.5f);
                    break;
                case "断头台展示":
                    CameraManager.Instance.SetVirtualCam("BlackCam", 0.5f);
                    TeachEventTrigger_Delay("能力介绍", 0.6f);
                    break;
                case "能力介绍":
                    SetGuide(player_W, false);
                    break;
                // case "牌库介绍":
                //     // TeachEventTrigger("卡牌介绍时间");
                //     // SetGuide(LibraryButton, false);
                //     break;
                case "卡牌介绍":
                    SetGuide(actionColor, false);
                    TeachEventTrigger("卡牌介绍品质");
                    // TeachEventTrigger("卡牌介绍时间");
                    break;
                // case "卡牌介绍时间":
                //     TeachEventTrigger("卡牌介绍描述");
                //     SetGuide(timeGuide, false);
                //     break;
                // case "卡牌介绍描述":
                //     TeachEventTrigger("卡牌介绍品质");
                //     SetGuide(desGuide, false);
                //     break;
                case "卡牌介绍品质":
                    TeachEventTrigger("关闭牌库提示");
                    SetGuide(qualityGuide, false);
                    break;


                case "岗位升级介绍":
                    SetGuide(player_W, false);
                    SetGuide(playe_Level, false);
                    SetGuide(ExecuteButton, false);
                    break;
                // case "公告栏介绍":
                //     SetGuide(noticeboard, false);
                //     break;

                default:
                    break;
            }
        }
        else//如果有，那就继续
        {
            //结束上一句对话
            StoryManager.Instance.CloseAllDialogs();

            StoryManager.Instance.Speak_Teach(words[0], words_owner_Dic[words[0]]);
            // word_Text.text = words[0];
            words_owner_Dic.Remove(words[0]);
            words.RemoveAt(0);
        }
    }

    public bool O_C_Guide()
    {
        isGuide = !isGuide;
        return isGuide;
    }

    public void TeachEventTrigger_Delay(string EventName, float time)
    {
        StartCoroutine(Delay(EventName, time));
    }

    IEnumerator Delay(string EventName, float time)
    {
        yield return new WaitForSeconds(time);
        TeachEventTrigger(EventName);
        yield break;
    }
}
