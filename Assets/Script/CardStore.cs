using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//所有原始卡的卡池

public class Probability//生成各个等级的卡牌的概率
{
    public float[] levelNums = { 2, 0, 0, 0, 0 };
    public float[] ExTemporal = { 1, 1, 1, 1, 1 };//每回合会被归一化
    public float[] ExEternal = { 1, 1, 1, 1, 1 };
    public float[] levelNumsFinal = { 2, 0, 0, 0, 0 };//每回合最终的概率
    public float levelNumsFinalSum = 0;
    public Probability(int postLevel)
    {
        switch (postLevel)
        {
            case 1:
                levelNums = new float[5] { 2, 0, 0, 0, 0, };
                break;
            case 2:
                levelNums = new float[5] { 4, 2, 0, 0, 0 };
                break;
            case 3:
                levelNums = new float[5] { 4, 4, 2, 0, 0 };
                break;
            case 4:
                levelNums = new float[5] { 2, 4, 4, 2, 0 };
                break;
            case 5:
                levelNums = new float[5] { 1, 2, 4, 4, 2 };
                break;
            default:
                levelNums = new float[5] { 2, 0, 0, 0, 0 };
                break;
        }
        CalculateLevelNumsFinal();
    }
    public void CalculateLevelNumsFinal()
    {
        levelNumsFinalSum = 0;
        for (int i = 0; i < 5; i++)
        {
            levelNumsFinal[i] = levelNums[i] * ExEternal[i] * ExTemporal[i];
            levelNumsFinalSum += levelNumsFinal[i];
        }
    }
}


public class CardStore : MonoSingleton<CardStore>
{
    public TextAsset cardData;
    public List<Card> cards = new List<Card>();//这是所有的原始卡牌
    public List<Card> Colourless_Cards = new List<Card>();//这是所有无色卡
    public List<Card> cards_LV1 = new List<Card>();
    public List<Card> cards_LV2 = new List<Card>();
    public List<Card> cards_LV3 = new List<Card>();
    public List<Card> cards_LV4 = new List<Card>();
    public List<Card> cards_LV5 = new List<Card>();
    [HideInInspector] public List<Card> cards_Forbiden = new List<Card>();//禁忌卡牌

    public List<GameObject> additives = new List<GameObject>();
    public List<Sprite> cardTextures = new List<Sprite>();
    public List<GameObject> flowers_model = new List<GameObject>();
    public List<Sprite> flowers_spirit = new List<Sprite>();

    [HideInInspector] public Probability probability = new Probability(1);
    void Awake()
    {
        probability.CalculateLevelNumsFinal();
        LoadCardDataFromCSV();
        // Test();////用于展示卡组里面全部的卡牌 
    }

    //从excel的csv文件中加载所有卡牌的数据到cards中
    void LoadCardDataFromCSV()
    {
        string[] dataRows = cardData.text.Split("\r\n", System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var dataRow in dataRows)
        {
            string[] elements = dataRow.Split(',');
            if (elements[0] == "id" || elements[0] == "condition" || elements[0] == "label" || elements[0] == "" || elements[0] == "func")//排除空行的影响
            {
                continue;
            }
            else//检测到是卡牌
            {
                int id = int.Parse(elements[0]);
                string title = elements[1];
                string description = elements[2];
                int qualityLevel = int.Parse(elements[3]);
                string times = elements[5];
                string condition = elements[6];
                int executeQueue = int.Parse(elements[13]);
                string funDes = elements[15];


                Card.ActionType actionType;
                if (elements[4].Contains("动"))
                {
                    actionType = Card.ActionType.Dynamic;
                }
                else if (elements[4].Contains("静"))
                {
                    actionType = Card.ActionType.Static;
                }

                else
                {
                    actionType = Card.ActionType.Unknown;
                }

                List<string> functions = new List<string>();
                for (int i = 7; i < 13; i++)
                {
                    if (elements[i] != null)
                    {
                        functions.Add(elements[i]);
                    }
                }

                string type = elements[14];//新来的
                Type t;
                switch (type)
                {

                    case "过载":
                        t = Type.OverLoad;
                        break;
                    case "精技":
                        t = Type.Skilled;
                        break;
                    case "摸鱼":
                        t = Type.Fishlike;
                        break;
                    case "暗算":
                        t = Type.Snaky;
                        break;
                    case "忍耐":
                        t = Type.Tolerant;
                        break;
                    case "善舞":
                        t = Type.Sociable;
                        break;
                    case "隐世":
                        t = Type.Flexible;
                        break;
                    case "叛乱":
                        t = Type.Revolting;
                        break;
                    default:
                        t = Type.Fishlike;
                        break;
                }



                Card card = new Card(id, title, description, qualityLevel, actionType, times, condition, functions, executeQueue, t, funDes);


                if (card.id < 10000)
                {
                    cards.Add(card);
                    switch (qualityLevel)
                    {
                        case 1:
                            cards_LV1.Add(card);

                            break;
                        case 2:
                            cards_LV2.Add(card);

                            break;
                        case 3:
                            cards_LV3.Add(card);

                            break;
                        case 4:
                            cards_LV4.Add(card);

                            break;
                        case 5:
                            cards_LV5.Add(card);

                            break;
                        default: break;
                    }
                }
                else//这边是无色卡专属，并不是AI专属
                {
                    Colourless_Cards.Add(card);
                }
                if (card.title.Contains("禁忌"))
                {
                    cards_Forbiden.Add(card);
                }


            }
        }

        List<Card> cards_All = new List<Card>();
        cards_All.AddRange(cards);
        cards_All.AddRange(Colourless_Cards);
        foreach (var card in cards_All)
        {
            card.AutoDescription();
        }
    }


    public Card RandomCard()//从原始卡牌中随机选择一张,返回其深拷贝
    {
        return new Card(cards[Random.Range(0, cards.Count)]);
    }



    public Card RandomCard(int postLevel2, bool isFixed = false)//isFixed代表是否只返回那个等级的卡牌，如果为否，则返回小于等于这个等级的卡牌
    {
        float r = Random.Range(0f, 1);
        Probability pro_temp = new Probability(postLevel2);

        float lv1Max = pro_temp.levelNumsFinal[0] / pro_temp.levelNumsFinalSum;
        float lv2Max = (pro_temp.levelNumsFinal[0] + pro_temp.levelNumsFinal[1]) / pro_temp.levelNumsFinalSum;
        float lv3Max = (pro_temp.levelNumsFinal[0] + pro_temp.levelNumsFinal[1] + pro_temp.levelNumsFinal[2]) / pro_temp.levelNumsFinalSum;
        float lv4Max = (pro_temp.levelNumsFinal[0] + pro_temp.levelNumsFinal[1] + pro_temp.levelNumsFinal[2] + pro_temp.levelNumsFinal[3]) / pro_temp.levelNumsFinalSum;

        if (isFixed)
        {
            switch (postLevel2)
            {
                case 1:
                    r = lv1Max - 0.00001f;
                    break;
                case 2:
                    r = lv2Max - 0.00001f;
                    break;
                case 3:
                    r = lv3Max - 0.00001f;
                    break;
                case 4:
                    r = lv4Max - 0.00001f;
                    break;
                case 5:
                    r = 1f;
                    break;

                default: break;
            }

        }

        if (r < lv1Max)
        {
            if (cards_LV1.Count > 0)
            {
                return new Card(cards_LV1[Random.Range(0, cards_LV1.Count)]);
            }
        }
        else if (r < lv2Max)
        {

            if (cards_LV2.Count > 0)
            {
                return new Card(cards_LV2[Random.Range(0, cards_LV2.Count)]);
            }
        }
        else if (r < lv3Max)
        {
            if (cards_LV3.Count > 0)
            {
                return new Card(cards_LV3[Random.Range(0, cards_LV3.Count)]);
            }
        }
        else if (r < lv4Max)
        {
            if (cards_LV4.Count > 0)
            {
                return new Card(cards_LV4[Random.Range(0, cards_LV4.Count)]);
            }
        }
        else
        {
            if (cards_LV5.Count > 0)
            {
                return new Card(cards_LV5[Random.Range(0, cards_LV5.Count)]);
            }
        }
        Card card = new Card(cards[Random.Range(0, cards.Count)]);
        return card;
    }

    public Card RandomCard_ByType(Type type)
    {
        List<Card> cards_type = new List<Card>();
        foreach (var card in cards)
        {
            if (card.type == type)
            {
                cards_type.Add(card);
            }
        }
        if (cards_type.Count > 0)
        {
            return new Card(cards_type[Random.Range(0, cards_type.Count)]);
        }
        else
        {
            return new Card(cards[0]);
        }

    }

    // public Card RandomCard_AI(int week)//AI发送给玩家的Debuff卡牌，周数越高，debuff卡牌越强
    // {
    //     if (week < 5)
    //     {
    //         Card card = new Card(Colourless_Cards[1]);
    //         return card;
    //     }
    //     else
    //     {
    //         Card card = new Card(Colourless_Cards[0]);
    //         return card;
    //     }

    // }

    public Card RandomCard_AI(int postLevel)//AI发送给玩家的Debuff卡牌，AI的postlevel越高，debuff卡牌越强
    {
        // if (week < 5)
        // {
        //     Card card = new Card(Colourless_Cards[1]);
        //     return card;
        // }
        // else
        // {
        //     Card card = new Card(Colourless_Cards[0]);
        //     return card;
        // }
        List<Card> cards_Debuff = new List<Card>();
        foreach (var card in Colourless_Cards)
        {
            if (card.qualityLevel <= postLevel)
            {
                cards_Debuff.Add(card);
            }
        }
        Card card_r = new Card(cards_Debuff[Random.Range(0, cards_Debuff.Count)]);
        return card_r;

    }

    public GameObject SearchModel(int id)
    {
        GameObject tmp = null;
        foreach (var additive in additives)
        {
            if ("Chess_" + id.ToString() == additive.name)
            {
                tmp = additive;
                break;
            }
        }
        if (tmp == null)
        {
            tmp = additives[0];
        }
        return tmp;
    }


    public Sprite SearchTexture(int id)
    {
        Sprite tmp = null;
        foreach (var sprite in cardTextures)
        {
            if ("Chess_" + id.ToString() == sprite.name)
            {
                tmp = sprite;
                break;
            }
        }
        return tmp;
    }

    public GameObject SearchFlower_Model(Type type)
    {
        GameObject tmp = null;
        foreach (var model in flowers_model)
        {
            if ("Flower_" + type.ToString() == model.name)
            {
                tmp = model;
                break;
            }
        }
        if (tmp == null)
        {
            tmp = flowers_model[0];
        }
        return tmp;
    }

    public Sprite SearchFlower_Spirite(Type type)
    {
        Sprite tmp = null;
        foreach (var sprite in flowers_spirit)
        {
            if ("Flower_" + type.ToString() == sprite.name)
            {
                tmp = sprite;
                break;
            }
        }
        return tmp;
    }

    public Card SearchCard(int id)
    {
        List<Card> cardsAll = new List<Card>();
        cardsAll.AddRange(cards);
        cardsAll.AddRange(Colourless_Cards);
        Card c = new Card();
        foreach (var card in cardsAll)
        {
            if (card.id == id)
            {
                c = card;
                break;
            }

        }
        if (c == null)
        {
            c = cards[0];
        }
        return new Card(c);
    }

    public Card RandomForbidenCard()//禁忌书送的随机卡牌
    {
        return new Card(cards_Forbiden[Random.Range(0, cards_Forbiden.Count)]);
    }

}

