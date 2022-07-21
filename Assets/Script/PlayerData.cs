using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public struct
PlayerData_OnlyData //仅数字数据
{
    public int physicalHealth;

    public int spiritualHealth;

    public int workAbility;

    public int KPI;

    public int ranking;
}

public class PlayerData : MonoSingleton<PlayerData>
{
    public int playerMoney;
    public TextMeshProUGUI playerMoney_Text;

    public List<Card> playerCards = new List<Card>();

    public TextAsset playerDataCSV;

    public List<string> datas = new List<string>();

    public string Date = "第一周";

    public string Name = "王大宝";

    public int physicalHealth = 100; //当前生命值

    public int physicalHealthMax = 100; //最大生命值

    public TextMeshPro physicalHealthText;

    public int spiritualHealth = 100; //当前精力值

    public int spiritualHealthMax = 100; //最大精力值

    public GameObject physicalHealthWarning1;//生命值不足提醒-黄
    public GameObject spiritualHealthWarning1;//精力值不足提醒-黄
    public GameObject physicalHealthWarning2;//生命值不足提醒-红
    public GameObject spiritualHealthWarning2;//精力值不足提醒-红


    public TextMeshPro spiritualHealthText;

    public int workAbility = 10;

    public TextMeshPro workAbilityText;

    public int KPI = 0;

    public TextMeshPro KPIText;

    public int ranking = 1;

    public TextMeshPro rangkingText;

    public int postLevel = 1;

    public TextMeshPro postLevelText;
    public int favorAll = 0;

    public TextMeshPro favorAllText;

    // //周末面板的主角信息UI
    // public TextMeshProUGUI physicalHealthText_H;

    // public TextMeshProUGUI spiritualHealthText_H;

    // public TextMeshProUGUI workAbilityText_H;

    // public TextMeshProUGUI KPIText_H;

    // public TextMeshProUGUI rangkingText_H;

    // public TextMeshProUGUI postLevelText_H;
    // public TextMeshProUGUI favorAllText_H;

    public int KPILife = 2;//kpi不及格2次就会被踢出去
    public GameObject KPILife_Prefab;//KPILife的icon
    [HideInInspector] public List<GameObject> KPILifes;//KPILife的数组
    public Transform KPILifeGroup;//放KPILife的icon的地方

    public List<Color> rankColors;




    void Start()
    {
        playerCards.Clear();
        datas.Clear();
        LoadPlayerData();
        UpdateKPILife();

    }

    public void UpdateKPILife()//更新KPILife的UI
    {
        foreach (var KPILife in KPILifes)
        {
            Destroy(KPILife);
        }
        KPILifes.Clear();
        for (var i = 0; i < KPILife; i++)
        {
            GameObject KPILife = Instantiate(KPILife_Prefab, KPILifeGroup);
            KPILifes.Add(KPILife);
        }
    }

    public void LoadPlayerData()
    {
        // AssetDatabase.Refresh();//刷新
        string[] dataRows = playerDataCSV.text.Split("\r\n", System.StringSplitOptions.RemoveEmptyEntries);

        foreach (var dataRow in dataRows)
        {
            string[] elements = dataRow.Split(',');
            if (elements[0] == "money")
            {
                playerMoney = int.Parse(elements[1]);
                playerMoney_Text.text = playerMoney.ToString();
            }
            else if (elements[0] == "card")
            {
                int id = int.Parse(elements[1]);
                List<Card> cards = new List<Card>();
                if (id < 10000)
                {
                    cards = CardStore.Instance.cards;
                }
                else
                {
                    cards = CardStore.Instance.Colourless_Cards;
                }
                foreach (var oriCard in cards)
                {
                    if (oriCard.id == id)
                    {
                        Card card = new Card(oriCard);
                        card.isNew = false;
                        playerCards.Add(card);
                    }
                }
            }
            else if (elements[0] == "isBeginAni")
            {
                if (elements[1] == "1")
                {
                 
                    //默认是开的
                }
                else
                {
               
                    StoryManager.Instance.isBeginAni = false;
                    SettingsManager.Instance.beginAni_tmp.text = "关闭";
                }

            }
            else if (elements[0] == "isGuide")
            {
                if (elements[1] == "1")
                {
                    //默认是开的
                }
                else
                {
                    TeachManager.Instance.isGuide = false;
                    SettingsManager.Instance.guide_tmp.text = "关闭";
                }
            }
        }
    }

    public void SavePlayerData() //储存玩家信息至csv
    {

        //路径
        string path = Application.dataPath + "/Data/PlayerData.csv";
        datas.Clear();
        datas.Add("money," + 1000);
        datas.Add("card,1");
        datas.Add("card,1");
        datas.Add("card,11");
        datas.Add("isBeginAni,0");
        datas.Add("isGuide,0");
        // datas.Add("#,id,title,description,qualitylevel,actionType,time,condition,func,,,,,,queue");

        // SortCards();

        /*
                foreach (var playerCard in playerCards)
                {
                    string actionType;
                    if (playerCard.actionType == Card.ActionType.Dynamic)
                    {
                        actionType = "动";
                    }
                    else if (playerCard.actionType == Card.ActionType.Static)
                    {
                        actionType = "静";
                    }
                    else
                    {
                        actionType = "未知";
                    }
                    string funcs = null;
                    for (int i = 0; i < playerCard.functions.Count; i++)
                    {
                        funcs += playerCard.functions[i] + ",";
                    }

                    string type = null;
                    switch (playerCard.type)
                    {
                        case Type.Fishlike:
                            type = "摸鱼";
                            break;
                        case Type.Flexible:
                            type = "隐世";
                            break;
                        case Type.OverLoad:
                            type = "过载";
                            break;
                        case Type.Revolting:
                            type = "叛乱";
                            break;
                        case Type.Skilled:
                            type = "精技";
                            break;
                        case Type.Snaky:
                            type = "暗算";
                            break;
                        case Type.Sociable:
                            type = "善舞";
                            break;
                        case Type.Tolerant:
                            type = "忍耐";
                            break;
                        default:
                            type = "摸鱼";
                            break;
                    }

                    string str =
                        "card," +
                        playerCard.id.ToString() +
                        "," +
                        playerCard.title +
                        "," +
                        playerCard.description +
                        "," +
                        playerCard.qualityLevel.ToString() +
                        "," +
                        actionType +
                        "," +
                        playerCard.times +
                        "," +
                        playerCard.condition.ToString() +
                        "," +
                        funcs +
                        playerCard.executeQueue.ToString() +
                        "," +
                        type;

                    datas.Add(str);
                }
        */
        File.WriteAllLines(path, datas);
    }


    // //Initiate按钮触发。玩家数据初始化
    // public void InitiatePlayerData()
    // {
    //     string path = Application.dataPath + "/Data/PlayerData.csv";
    //     List<string> datas = new List<string>();
    //     datas.Add("money,100");

    //     //储存datas到路径，生成csv
    //     File.WriteAllLines(path, datas);
    //     playerMoney = 100;
    //     playerCards.Clear();
    // }

    public void SortCards() //冒泡排序整理卡牌
    {
        Card temp = new Card();
        if (playerCards.Count > 1)
        {
            for (int i = 0; i < playerCards.Count - 1; i++)
            {
                for (var j = 0; j < playerCards.Count - 1 - i; j++)
                {
                    if (playerCards[j].id > playerCards[j + 1].id)
                    {
                        temp = playerCards[j];
                        playerCards[j] = playerCards[j + 1];
                        playerCards[j + 1] = temp;
                    }
                }
            }
        }
    }

    // public void UpdateDataToUI_Holiday() //把数据更新到HolidayUI
    // {
    //     physicalHealthText_H.text = physicalHealth.ToString() + "/" + physicalHealthMax.ToString();
    //     spiritualHealthText_H.text = spiritualHealth.ToString() + "/" + spiritualHealthMax.ToString();
    //     workAbilityText_H.text = workAbility.ToString();
    //     KPIText_H.text = KPI.ToString();
    //     rangkingText_H.text = ranking.ToString();
    //     JudgeRankingColor(ranking, rangkingText_H, KPIText_H);
    //     postLevelText_H.text = postLevel.ToString();
    //     CalculateFavorAll();//计算总受欢迎度,并更新平时和周末的UI
    // }

    public void UpdateDataToUI_Weekday() //把数据更新到WeekdatUI
    {
        physicalHealthText.text = physicalHealth.ToString() + "/" + physicalHealthMax.ToString();
        spiritualHealthText.text = spiritualHealth.ToString() + "/" + spiritualHealthMax.ToString();
        workAbilityText.text = workAbility.ToString();
        KPIText.text = KPI.ToString()+"/"+Mechanism.Instance.KPINeed_EveryMonth.ToString();
        rangkingText.text = ranking.ToString();
        JudgeRankingColor(ranking, rangkingText, KPIText);
        postLevelText.text = postLevel.ToString();

    }

    public void JudgeRankingColor(int ranking, TextMeshProUGUI rankTMP, TextMeshProUGUI KPITMP)
    {
        switch (ranking)
        {
            case 1:
                rankTMP.color = rankColors[0];
                KPITMP.color = rankColors[0];
                break;
            case 2:
                rankTMP.color = rankColors[1];
                KPITMP.color = rankColors[1];
                break;
            case 3:
                rankTMP.color = rankColors[2];
                KPITMP.color = rankColors[2];
                break;
            case 4:
                rankTMP.color = rankColors[3];
                KPITMP.color = rankColors[3];
                break;
            case 5:
                rankTMP.color = rankColors[4];
                KPITMP.color = rankColors[4];
                break;
            default:
                break;
        }
    }

    public void JudgeRankingColor(int ranking, TextMeshPro rankTMP, TextMeshPro KPITMP)
    {
        switch (ranking)
        {
            case 1:
                rankTMP.color = rankColors[0];
                KPITMP.color = rankColors[0];
                break;
            case 2:
                rankTMP.color = rankColors[1];
                KPITMP.color = rankColors[1];
                break;
            case 3:
                rankTMP.color = rankColors[2];
                KPITMP.color = rankColors[2];
                break;
            case 4:
                rankTMP.color = rankColors[3];
                KPITMP.color = rankColors[3];
                break;
            case 5:
                rankTMP.color = rankColors[4];
                KPITMP.color = rankColors[4];
                break;
            default:
                break;
        }
    }

    public void CalculateFavorAll() //计算总受欢迎度,并更新UI

    {
        favorAll = 0;
        foreach (var AIData in AIMechanism.Instance.AIDatas)
        {
            favorAll += AIData.favor;
        }
        favorAllText.text = favorAll.ToString();


    }

    public void ChangeProperty(string s, int value)
    {
        switch (s)
        {
            case "P":
                physicalHealth += value;
                physicalHealth = Mathf.Min(physicalHealth, physicalHealthMax);
                physicalHealthText.text = physicalHealth.ToString() + "/" + physicalHealthMax.ToString();
                break;
            case "S":
                spiritualHealth += value;
                spiritualHealth = Mathf.Min(spiritualHealth, spiritualHealthMax);
                spiritualHealthText.text = spiritualHealth.ToString() + "/" + spiritualHealthMax.ToString(); ;
                break;
            case "W":
                workAbility += value;
                workAbilityText.text = workAbility.ToString();
                break;
            case "K":
                KPI += value;
                KPIText.text = KPI.ToString();
                break;
            case "PM":
                physicalHealthMax += value;
                physicalHealth += value;
                physicalHealthText.text = physicalHealth.ToString() + "/" + physicalHealthMax.ToString();
                break;
            case "SM":
                spiritualHealthMax += value;
                spiritualHealth += value;
                spiritualHealthText.text = spiritualHealth.ToString() + "/" + spiritualHealthMax.ToString();
                break;
            default:
                break;
        }
    }

    public void ChangeMoney(int value)
    {
        playerMoney = Mathf.Max(0, playerMoney + value);
        playerMoney_Text.text = playerMoney.ToString();
    }

    public void PSWarning()
    {

        if ((float)physicalHealth / physicalHealthMax <= 0.25f)
        {
            physicalHealthWarning2.SetActive(true);
        }
        else if ((float)physicalHealth / physicalHealthMax <= 0.5f)
        {
            physicalHealthWarning1.SetActive(true);
        }
        else
        {
            physicalHealthWarning1.SetActive(false);
            physicalHealthWarning2.SetActive(false);
        }

        if ((float)spiritualHealth / spiritualHealthMax <= 0.25f)
        {
            spiritualHealthWarning2.SetActive(true);
        }
        else if ((float)spiritualHealth / spiritualHealthMax <= 0.5f)
        {
            spiritualHealthWarning1.SetActive(true);
        }
        else
        {
            spiritualHealthWarning1.SetActive(false);
            spiritualHealthWarning2.SetActive(false);
        }
    }

}
