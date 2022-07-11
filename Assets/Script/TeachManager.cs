using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeachManager : MonoSingleton<TeachManager>
{
    public bool isGuide = true;
    public GameObject BlackWord;
    public TextMeshProUGUI word_Text;
    [HideInInspector] public List<string> words = new List<string>();//黑衣人将要说的所有话
    [HideInInspector] public string currentEvent = null;//现在正在发生的事件，例如"开头介绍";



    public GameObject player_P;
    public GameObject player_S;
    public GameObject player_W;
    public GameObject playe_Level;
    public GameObject playe_K;
    public GameObject playe_ranking;
    public GameObject playe_PY;
    public GameObject KPINeed;
    public GameObject LibraryButton;
    public GameObject StartButton;
    public GameObject ExecuteButton;
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
    [HideInInspector] public bool isFirstSalary = true;//是不是第一次场上三连

    public void SetGuide(GameObject obj, bool b)//打开/关闭物体的黄色亮闪引导，前提是物体的一级子物体中必须有名为Guide的物体
    {
        Transform t = obj.transform.Find("Guide");
        // Debug.Log(t.gameObject.name);
        if (t != null)
        {
            t.gameObject.SetActive(b);
        }
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
            case "开头介绍":
                words.Add("故事的开头，要从一个刚毕业的新生开始说起");
                words.Add("你满怀期待地入职了一家心仪已久的公司");
                words.Add("尽管这家公司，充满了各种匪夷所思的传闻……");
                break;
            case "生命介绍":
                words.Add("话不多说。这是你的精力和体力，某一项归零，游戏就结束了");
                SetGuide(player_P, true);
                SetGuide(player_S, true);
                break;
            case "绩效需求介绍":
                words.Add("你每个月有绩效需求，达不到就会被开除");
                SetGuide(KPINeed, true);
                SetGuide(playe_K, true);
                break;
            case "牌库介绍":
                words.Add("什么，怎么创造绩效？当然是使用你的卡牌");
                words.Add("点击牌库查看你的初始卡牌");
                SetGuide(LibraryButton, true);
                break;
            case "开始介绍":
                words.Add("点击开始键，自动执行你这一周的计划");
                words.Add("开始体验被雇佣者的一周吧");
                SetGuide(StartButton, true);
                break;
            case "卡牌介绍":
                words.Add("这是你的初始卡牌");
                break;
            case "卡牌介绍时间":
                words.Add("此处为卡牌出现的时间，例如“夜班”只可能在“晚上”的槽位出现");
                SetGuide(timeGuide, true);
                break;
            case "卡牌介绍描述":
                words.Add("而这里是卡牌的效果描述");
                SetGuide(desGuide, true);
                break;
            case "卡牌介绍品质":
                words.Add("这一块则代表着卡牌的等级");
                words.Add("通过提升职位等级有机会获得更加高级的卡牌");
                SetGuide(qualityGuide, true);
                break;
            case "关闭牌库提示":
                words.Add("卡牌介绍完毕，再次点击“牌库”按钮关闭牌库");
                SetGuide(LibraryButton, true);
                break;
            case "周末选牌介绍":
                words.Add("每周末晚上，领导都会召开总结会议");
                words.Add("你可以选择新的卡牌，每周有两次选择新卡牌的机会");
                words.Add("领导让你们提前一个晚上来上班，就是让你们好好制定下一周的计划");
                words.Add("到底是谁定的这条破规则？");
                SetGuide(ExecuteButton, true);
                break;
            case "选牌三连蛊惑":
                words.Add("三张重复的卡牌同时在场上，可以引起卡牌三连，增加卡牌熟练度");
                words.Add("选择这张牌，将获得三连");
                words.Add("所以，不多说了，做出你的选择吧");
                break;
            case "岗位升级介绍":
                words.Add("你可以通过某些卡牌来提高能力");
                words.Add("能力够了就可以升级岗位等级，到时候你可以获得更好的卡牌");
                words.Add("可惜，现在能力还不够");
                words.Add("传闻中，有一位仅用一个月就升到2级的员工，是天才中的天才");
                SetGuide(player_W, true);
                SetGuide(playe_Level, true);
                SetGuide(ExecuteButton, true);
                break;
            case "公告栏介绍":
                words.Add("这是公告牌，可以显示最近发生的一些事件");
                SetGuide(noticeboard, true);
                break;
            case "场上三连介绍":
                words.Add("触发三连后，三张卡牌会合成一张熟练度更高的卡牌，例如从“工作”变为“工作+1”");
                words.Add("熟练度可以提升卡牌的数值");
                words.Add("此外，每次三连都可以额外选择一张强力卡牌");
                words.Add("啊，不停工作，然后变成一个熟练的工人");
                break;
            case "假日商店介绍":
                words.Add("在每周的工作结束后，周末面板有一定概率会出现");
                words.Add("你可以安排你的周末生活，当然，你得支付“一点点”费用");
                words.Add("赚钱，然后过上好生活，美好的循环，不是么");
                break;
            case "工资结算介绍":
                words.Add("在每个月底，KPI会被清零，并根据KPI的数量为你发放工资");
                words.Add("是摸是卷，你自己决定");
                words.Add("但江湖上有一个传闻，叫能者多劳");
                words.Add("领导总是会对工作能力高的员工提出更多要求");
                break;
            case "小组成员介绍":
                words.Add("一个月过去了，你已经不是实习生了，进入了一个特定的项目组");
                words.Add("你即将加入的工作小组中有其他几名成员，他们性格能力各异，和他们友好相处吧");
                words.Add("顺便一提，如果你的KPI排名两次垫底，你将被开除");
                break;
            case "受欢迎度介绍":
                words.Add("这是你的受欢迎度");
                words.Add("如果你做了对不起同事的事情，他们就会讨厌你，可能还会背地里对你动手");
                words.Add("当然，有些人，你不惹他们，他们也会缠上你");
                SetGuide(playe_PY, true);
                break;
            default:
                break;
        }

        //开启对话框并播放第一句话
        BlackWord.SetActive(true);
        // TimeManager.Instance.OnClickStopButton(0);
        if (words.Count > 0)
        {
            word_Text.text = words[0];
            words.RemoveAt(0);
        }
        else
        {
            word_Text.text = "word数量不足";
        }
    }

    public void Continue()
    {
        if (words.Count == 0)//如果没有下一句话了，就关闭对话框。如果有接着的对话，则开启
        {
            BlackWord.SetActive(false);
            // TimeManager.Instance.OnClickStopButton(1);
            switch (currentEvent)
            {
                case "开头介绍":
                    TeachEventTrigger("生命介绍");
                    break;
                case "生命介绍":
                    TeachEventTrigger("绩效需求介绍");
                    SetGuide(player_P, false);
                    SetGuide(player_S, false);
                    break;
                case "绩效需求介绍":
                    TeachEventTrigger("牌库介绍");
                    SetGuide(KPINeed, false);
                    SetGuide(playe_K, false);
                    break;
                case "卡牌介绍":
                    TeachEventTrigger("卡牌介绍时间");
                    break;
                case "卡牌介绍时间":
                    TeachEventTrigger("卡牌介绍描述");
                    SetGuide(timeGuide, false);
                    break;
                case "卡牌介绍描述":
                    TeachEventTrigger("卡牌介绍品质");
                    SetGuide(desGuide, false);
                    break;
                case "卡牌介绍品质":
                    TeachEventTrigger("关闭牌库提示");
                    SetGuide(qualityGuide, false);
                    break;
                case "岗位升级介绍":
                    SetGuide(player_W, false);
                    SetGuide(playe_Level, false);
                    SetGuide(ExecuteButton, false);
                    break;
                case "公告栏介绍":
                    SetGuide(noticeboard, false);
                    break;
                case "受欢迎度介绍":
                    SetGuide(playe_PY, false);
                    break;
                default:
                    break;
            }
        }
        else//如果有，那就继续
        {
            word_Text.text = words[0];
            words.RemoveAt(0);
        }
    }

}
