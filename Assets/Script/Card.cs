using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Card
{
    public int id;

    public string title = "卡名"; //原始卡名

    public string finalTitle = null; //最终卡名

    public string description = "描述的内容";
    public string funDes;//有趣的介绍

    public string times = "上中下";

    public List<string> timesList = new List<string>();

    //卡牌等级和等级颜色
    public int qualityLevel = 1;

    //卡牌的附加等级，比如“摸鱼+1"
    public int addLevel = 0;
    float addLevelPower = 0.5f;

    public Color
        qualityColor = new Color(210f / 255f, 210f / 255f, 210f / 255f, 1);

    //行动的动静状态和行动颜色
    public enum ActionType
    {
        Dynamic,
        Static,
        Unknown
    }

    public ActionType actionType = ActionType.Dynamic;

    public Color actionColor = new Color(239f / 255f, 101f / 255f, 96f / 255f, 1);

    public string condition;

    public List<string> functions = new List<string>();

    // public List<Card> AffectCard = new List<Card>();//影响的对象
    public FunctionEffect functionEffect = default;

    public FunctionEffectEx functionEffectEx = new FunctionEffectEx();

    public int posNum = 0; //从1-20,也就是Card在场景中的位置,0是默认值，代表此卡牌不在场景中

    //CardStore表中还没有加载这一数据，暂时不用
    public int executeQueue = 10;

    public Type type;

    //是新卡牌吗?
    public bool isNew = true;

    public int lifeMax; //出现n次后消失

    public int life;//TD自爆用
    public bool isAlive = true;//触发亡语用

    //T计次数器
    public int TimeCounter = 0;

    //卡牌是否生效，如果没有，则直接跳过此卡牌的assign结算时间
    public bool isEffect = false;//所有函数中只要生效过一次就不会跳过


    public Card()
    {
    }

    public Card(Card card)
    {
        this.id = card.id;
        this.title = card.title;
        this.description = card.description;
        this.qualityLevel = card.qualityLevel;
        this.actionType = card.actionType;
        this.times = card.times;
        this.condition = card.condition;
        this.functions = card.functions;
        this.addLevel = card.addLevel; //原始卡库的等级会影响新卡库
        this.executeQueue = card.executeQueue;
        this.type = card.type;
        this.isNew = card.isNew;
        this.life = card.life;
        this.funDes = card.funDes;
        this.cardInfor = card.cardInfor;

        switch (this.qualityLevel //灰绿蓝紫橙
        )
        {
            case 1:
                this.qualityColor =
                    new Color(210f / 255f, 210f / 255f, 210f / 255f, 1);
                break;
            case 2:
                this.qualityColor =
                    new Color(175f / 255f, 239f / 255f, 96f / 255f, 1);
                break;
            case 3:
                this.qualityColor =
                    new Color(35f / 255f, 130f / 255f, 236f / 255f, 1);
                break;
            case 4:
                this.qualityColor =
                    new Color(179f / 255f, 33f / 255f, 180f / 255f, 1);
                break;
            case 5:
                this.qualityColor =
                    new Color(255f / 255f, 195f / 255f, 50f / 255f, 1);
                break;
        }

        Color c;

        if (
            this.actionType == ActionType.Dynamic //红
        )
        {
            ColorUtility.TryParseHtmlString("#FF2A5B", out c);
            this.actionColor = c;
        }
        else if (
            this.actionType == ActionType.Static //蓝
        )
        {
            ColorUtility.TryParseHtmlString("#00FFE0", out c);
            this.actionColor = c;
        }
        else
        {
            ColorUtility.TryParseHtmlString("#787878", out c);
            this.actionColor = c;
        }
        AutoTitle();
    }

    public Card(
        int id1,
        string title1,
        string description1,
        int qualityLevel1,
        ActionType actiontype1,
        string times1,
        string condition1,
        List<string> functions1,
        int executeQueue1,
        Type type1,
        string funDes1
    ) //这个重载用于在CardStore中生成原始卡库
    {
        this.id = id1;
        this.title = title1;
        this.description = description1; //这是用表格中的值进行赋值
        this.qualityLevel = qualityLevel1;
        this.actionType = actiontype1;
        this.times = times1;
        this.condition = condition1;
        this.functions = functions1;
        this.executeQueue = executeQueue1;
        this.type = type1;
        this.funDes = funDes1;
        switch (this.qualityLevel //灰绿蓝紫橙
        )
        {
            case 1:
                this.qualityColor =
                    new Color(210f / 255f, 210f / 255f, 210f / 255f, 1);
                break;
            case 2:
                this.qualityColor =
                    new Color(175f / 255f, 239f / 255f, 96f / 255f, 1);
                break;
            case 3:
                this.qualityColor =
                    new Color(35f / 255f, 130f / 255f, 236f / 255f, 1);
                break;
            case 4:
                this.qualityColor =
                    new Color(179f / 255f, 33f / 255f, 180f / 255f, 1);
                break;
            case 5:
                this.qualityColor =
                    new Color(255f / 255f, 195f / 255f, 50f / 255f, 1);
                break;
        }

        if (
            this.actionType == ActionType.Dynamic //红
        )
        {
            this.actionColor =
                new Color(239f / 255f, 101f / 255f, 96f / 255f, 1);
        }
        else if (
            this.actionType == ActionType.Static //蓝
        )
        {
            this.actionColor =
                new Color(110f / 255f, 195f / 255f, 255f / 255f, 1);
        }
        else
        {
            this.actionColor = new Color(90f / 255f, 90f / 255f, 90f / 255f, 1);
        }
        AutoLife();

        // //自动生成文本的方法,会覆盖之前对description的赋值
        // AutoDescription();

        //自动生成标题finalTitle
        AutoTitle();

    }

    public void CardEffect() //传入影响的card列表List<Card> AffectCard
    {
        if (JudgeCondition(this.condition))
        {
            foreach (var func in functions)
            {
                if (!func.Contains("DW"))
                {
                    ExecuteFunction(func);
                }
            }

            if (this.executeQueue == 12)
            {
                Mechanism.Instance.CreatText_AIDebuff(this);
            }
        }
        if (!isEffect)
        {
            Mechanism.Instance.SkipTime_Assign();
        }
        else
        {
            Mechanism.Instance.cards_cardPersonalGamePrefabs_Dic[this].GetComponent<CardDisplayPersonalGame>().FX();
            if (this.executeQueue == 12)
            {
                foreach (var AIData in AIDatas_Debuff)
                {
                    AIMechanism.Instance.AI_Chas[AIData.AIid].Mini_BeDebuffed(this.id);
                    AIMechanism.Instance.Leader.Mini_BeDebuffed(this.id);
                }

            }
        }
    }

    public void DWCardEffect()//亡语卡牌的效果 //亡语的触发不受condi控制
    {
        foreach (var func in functions)
        {
            if (func.Contains("DW"))
            {
                ExecuteFunction(func);
            }

        }
    }

    void AutoLife()
    {
        bool isTDExist = false;
        foreach (var func in functions)
        {
            string[] elements =
                func.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length > 0)
            {
                if (elements[0] == "TD")
                {
                    isTDExist = true;
                    this.lifeMax = int.Parse(elements[3]);
                    this.life = this.lifeMax;
                }
            }
        }

        if (isTDExist)
        {
        }
        else
        {
            this.lifeMax = 0;
            this.life = this.lifeMax;
        }
    }



    //！！！！！！！！！condi区！！！！！！！！！
    public bool JudgeCondition(string condi)
    {
        if (condi == "0")
        {
            return false;
        }
        else if (condi == "1")
        {
            return true;
        }
        else
        {
            string[] elements =
                condi.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
            if (
                elements.Length > 0 //如果是空的，那elements的长度一定为0
            )
            {
                //！！！！！！！确定好对象 范围 值！！！！！！！
                List<Card> originCards = new List<Card>(); //对象们
                string range = elements[2]; //范围
                float value = float.Parse(elements[3]); //值
                if (
                    elements[1] == "M" //用于确定对象
                )
                {
                    originCards.Add(this);
                }

                //！！！！！！！根据函数的类型，填充对象 范围 值！！！！！！！
                if (elements[0] == "Ex")
                {
                    bool b = Exist(originCards, range, value);
                    return b;
                }
                else if (elements[0] == "ExAI")
                {
                    bool b = ExistAI((int)value);
                    return b;
                }
                else if (
                    elements[0] == "PP" ||
                    elements[0] == "SP" ||
                    elements[0] == "WP" ||
                    elements[0] == "KP"
                )
                {
                    bool b = PSWK_PercentageTest(elements[0], range, value); //这个range是< >这种
                    return b;
                }
            }
        }
        return false; //默认返回false
    }
    bool ExistAI(int num)
    {
        if (AIMechanism.Instance.CalculateAIDataCount() >= num)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool Exist(List<Card> originCards, string range, float value)
    {
        List<Card> allCards = new List<Card>();
        FindAllCards(originCards, range, allCards);
        int n = (int)value;
        if (n == 0)
        {
            if (allCards.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (n < 20000)
        {
            bool b = false;
            foreach (var card in allCards)
            {
                if (card.id == n)
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        else if (n == 20001)//存在至少一张 一级红
        {
            bool b = false;
            foreach (var card in allCards)
            {
                if (card.qualityLevel == 1 && card.actionType == ActionType.Dynamic)
                {
                    b = true;
                }
            }
            return b;
        }
        else if (n == 20013)
        {
            bool b = false;
            foreach (var card in allCards)
            {
                if (card.actionType == ActionType.Unknown)
                {
                    b = true;
                }
            }
            return b;
        }
        else
        {
            return false;
        }
    }

    bool PSWK_PercentageTest(string funcName, string range, float value)
    {
        if (funcName == "PP")
        {
            if (range == "<")
            {
                if (
                    PlayerData.Instance.physicalHealth <
                    PlayerData.Instance.physicalHealthMax * value
                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (range == ">")
            {
                if (
                    PlayerData.Instance.physicalHealth >
                    PlayerData.Instance.physicalHealthMax * value
                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

    //！！！！！！！！！查找区！！！！！！！！！
    void FindAllCards(List<Card> originCards, string range, List<Card> allCards)
    {
        foreach (var originCard in originCards)
        {
            switch (range)
            {
                case "1": //自己
                    SearchCard(originCard.posNum, allCards);
                    break;
                case "2": //同行（不包括自己）
                    for (int i = 1; i < 21; i++)
                    {
                        if (
                            i % 4 == originCard.posNum % 4 && i != this.posNum //如果同行
                        )
                        {
                            SearchCard(i, allCards);
                        }
                    }
                    break;
                case "3": //下一个对象
                    if (
                        (originCard.posNum) / 4 == (originCard.posNum - 1) / 4 //如果是同列
                    )
                    {
                        SearchCard(originCard.posNum + 1, allCards);
                    }
                    break;
                case "4": //今天所有（包括自己）
                    for (int i = 1; i < 21; i++)
                    {
                        if (
                            i == this.posNum //排除buff自己的可能性
                        )
                        {
                            continue;
                        }
                        if (
                            (i - 1) / 4 == (originCard.posNum - 1) / 4 //如果同列
                        )
                        {
                            SearchCard(i, allCards);
                        }
                    }
                    break;
                case "5": //今天之后所有
                    for (int i = 1; i < 21; i++)
                    {
                        if (i == this.posNum)
                        {
                            continue;
                        }
                        if (
                            (i - 1) / 4 == (originCard.posNum - 1) / 4 &&
                            i > originCard.posNum
                        )
                        {
                            SearchCard(i, allCards);
                        }
                    }
                    break;
                case "9": //九宫格范围内
                    SearchCard9(originCard.posNum, allCards, 0);
                    break;
                case "91": //九宫格范围内任一
                    SearchCard9(originCard.posNum, allCards, 1);
                    break;
                case "25": //二十五宫格范围内
                    SearchCard25(originCard.posNum, allCards);
                    break;
                case "100": //所有范围
                    SearchCardAll(allCards);
                    break;
                default:
                    break;
            }
        }
    }

    void SearchCard(int num, List<Card> allCards) //查找卡牌，输入的num必须在1-20之间
    {
        if (num > 0 && num < 21)
        {

            foreach (var card in Mechanism.Instance.cardList)
            {
                if (card.posNum == num)
                {
                    allCards.Add(card);
                }
            }

        }
    }

    void SearchCard9(int num, List<Card> allCards, int returnNum) //找到九宫格内的卡牌，已经做了防止越界的处理,第三个参数是返回几个，如果是0则全部返回，如果是1则返回一个
    {
        List<Card> allCardsTemp = new List<Card>(); //暂存卡组
        for (int i = 0; i < 3; i++)
        {
            if (
                i == 1 //不搜索此卡牌
            )
            {
                continue;
            }
            SearchCard(num - 4 + i * 4, allCardsTemp);
        }
        if (
            (int)Mathf.Floor((float)(num - 2) / (float)4) ==
            (int)Mathf.Floor((float)(num - 1) / (float)4) //如果上一卡牌与此卡牌同列
        )
        {
            for (int i = 0; i < 3; i++)
            {
                SearchCard(num - 5 + i * 4, allCardsTemp);
            }
        }
        if (
            (int)Mathf.Floor((float)num / (float)4) ==
            (int)Mathf.Floor((float)(num - 1) / (float)4) //如果下一卡牌与此卡牌同列
        )
        {
            for (int i = 0; i < 3; i++)
            {
                SearchCard(num - 3 + i * 4, allCardsTemp);
            }
        }
        switch (returnNum)
        {
            case 0:
                allCards.AddRange(allCardsTemp);
                break;
            case 1: //随机选取一个元素
                if (allCardsTemp.Count > 0)
                {
                    allCards
                        .Add(allCardsTemp[Random.Range(0, allCardsTemp.Count)]);
                }
                break;
            default:
                break;
        }
    }

    void SearchCard25(int num, List<Card> allCards) //找到九宫格内的卡牌，已经做了防止越界的处理
    {
        for (int i = 0; i < 5; i++)
        {
            if (
                i == 2 //不搜索此卡牌
            )
            {
                continue;
            }
            SearchCard(num - 8 + i * 4, allCards);
        }
        if (
            (int)Mathf.Floor((float)(num - 2) / (float)4) ==
            (int)Mathf.Floor((float)(num - 1) / (float)4)
        )
        {
            for (int i = 0; i < 5; i++)
            {
                SearchCard(num - 9 + i * 4, allCards);
            }
        }
        if (
            (int)Mathf.Floor((float)(num - 3) / (float)4) ==
            (int)Mathf.Floor((float)(num - 1) / (float)4)
        )
        {
            for (int i = 0; i < 5; i++)
            {
                SearchCard(num - 10 + i * 4, allCards);
            }
        }
        if (
            (int)Mathf.Floor((float)num / (float)4) ==
            (int)Mathf.Floor((float)(num - 1) / (float)4)
        )
        {
            for (int i = 0; i < 5; i++)
            {
                SearchCard(num - 7 + i * 4, allCards);
            }
        }
        if (
            (int)Mathf.Floor((float)(num + 1) / (float)4) ==
            (int)Mathf.Floor((float)(num - 1) / (float)4)
        )
        {
            for (int i = 0; i < 5; i++)
            {
                SearchCard(num - 6 + i * 4, allCards);
            }
        }
    }

    void SearchCardAll(List<Card> allCards)
    {

        allCards.AddRange(Mechanism.Instance.cardList);
        allCards.Remove(this);

    }

    void FindAllCards_PorpertyRange(
        string Range,
        string property,
        List<Card> allCards,
        List<Card> allCards_PropertyRange
    ) //通过属性（Property）的数字范围（Range）来查找卡牌
    {
        foreach (var card in allCards)
        {
            if (Range == ">0")
            {
                switch (property)
                {
                    case "P":
                        if (card.functionEffect.physicalHealth > 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "S":
                        if (card.functionEffect.spiritualHealth > 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "W":
                        if (card.functionEffect.workAbility > 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "K":
                        if (card.functionEffect.KPI > 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (Range == "<0")
            {
                switch (property)
                {
                    case "P":
                        if (card.functionEffect.physicalHealth < 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "S":
                        if (card.functionEffect.spiritualHealth < 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "W":
                        if (card.functionEffect.workAbility < 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "K":
                        if (card.functionEffect.KPI < 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (Range == "!=0")
            {
                switch (property)
                {
                    case "P":
                        if (card.functionEffect.physicalHealth != 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "S":
                        if (card.functionEffect.spiritualHealth != 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "W":
                        if (card.functionEffect.workAbility != 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "K":
                        if (card.functionEffect.KPI != 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (Range == "==0")
            {
                switch (property)
                {
                    case "P":
                        if (card.functionEffect.physicalHealth == 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "S":
                        if (card.functionEffect.spiritualHealth == 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "W":
                        if (card.functionEffect.workAbility == 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    case "K":
                        if (card.functionEffect.KPI == 0)
                        {
                            allCards_PropertyRange.Add(card);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //！！！！！！！！！特效区！！！！！！！！！
    void BuffBebuffedEffect(List<Card> allCards) //根据这张卡牌buff了别人或者是被别人buff来释放特效
    {
        // bool isThisExist = false; //这张卡牌存在吗
        // foreach (var card in allCards)
        // {
        //     if (card == this)
        //     {
        //         isThisExist = true;
        //     }
        // }

        // if (
        //     isThisExist == false && allCards.Count > 0 //如果作用的对象没有自己，且作用的对象存在，那么就判定为this buff了别的卡牌
        // )
        // {
        //     Buff(this);
        //     foreach (var card in allCards)
        //     {
        //         BeBuffed(card);
        //     }
        // }
        Buff(this);
        foreach (var card in allCards)
        {
            BeBuffed(card);
        }
    }

    void BeBuffed(Card card)
    {
        GameObject cardPersonalGamePrefab =
            Mechanism.Instance.cards_cardPersonalGamePrefabs_Dic[card];
        SimpleEffect se = cardPersonalGamePrefab.GetComponent<SimpleEffect>();
        if (se.isBebuffed == false)
        {
            se.StartCoroutine(se.BeBuffed());
        }
    }

    void Buff(Card card)
    {
        GameObject cardPersonalGamePrefab = Mechanism.Instance.cards_cardPersonalGamePrefabs_Dic[card];
        SimpleEffect se = cardPersonalGamePrefab.GetComponent<SimpleEffect>();
        if (se.isBuff == false)
        {
            se.StartCoroutine(se.Buff());
        }
    }
    void TDEffect()
    {
        GameObject cardPG_Des = Mechanism.Instance.cards_cardPersonalGamePrefabs_Dic[this];
        SimpleEffect se = cardPG_Des.GetComponent<SimpleEffect>();
        if (se.isTD == false)
        {
            se.StartCoroutine(se.TD());
        }
    }
    void DestroyEffect(List<Card> cards, Color color) //卡牌销毁特效
    {
        GameObject cardPG_Des = Mechanism.Instance.cards_cardPersonalGamePrefabs_Dic[this];
        SimpleEffect se = cardPG_Des.GetComponent<SimpleEffect>();

        List<Transform> destTranss = new List<Transform>();
        foreach (var card in cards)
        {
            GameObject cardPG_BeDesed = Mechanism.Instance.cards_cardPersonalGamePrefabs_Dic[card];
            Transform destPos = cardPG_BeDesed.GetComponent<SimpleEffect>().Des_EndPos;
            destTranss.Add(destPos);
        }

        if (se.isDes == false)
        {
            se.StartCoroutine(se.Destroy(color, destTranss));
        }
    }

    void InstantiateEffect(Card MotherCard, Card KidCard, float delayTime) //卡牌生成特效 //第一个参数是生卡牌的卡牌，第二个参数是被生的卡牌
    {
        GameObject cardPG = Mechanism.Instance.cards_cardPersonalGamePrefabs_Dic[MotherCard];
        SimpleEffect se = cardPG.GetComponent<SimpleEffect>();
        se.InstantiateCard(KidCard, delayTime);
    }

    //！！！！！！！！！计算区！！！！！！！！！
    public void ExecuteFunction(string func)
    {
        string[] elements =
            func.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
        if (
            elements.Length > 0 //如果是空的，那elements的长度一定为0
        )
        {
            if (
                !elements[0].Contains("AI") //如果func和AI无关
            )
            {
                //！！！！！！！确定好对象 范围 值！！！！！！！
                List<Card> originCards = new List<Card>(); //对象们
                string funcName = elements[0];
                string range = elements[2]; //范围
                float value = float.Parse(elements[3]); //值
                if (
                    elements[1] == "M" //用于确定对象
                )
                {
                    originCards.Add(this);
                }

                if (funcName.Contains("DW"))//如果是DW，那么就去头，比如 DW_P → P
                {
                    string[] eles = funcName.Split('_', System.StringSplitOptions.RemoveEmptyEntries);
                    funcName = eles[1];
                }

                //！！！！！！！根据函数的类型，填充对象 范围 值！！！！！！！
                if (
                    funcName == "P" ||
                    funcName == "S" ||
                    funcName == "W" ||
                    funcName == "K"
                )
                {
                    BaseAdd(funcName, originCards, range, value);
                }
                else if (
                    funcName == "PA" ||
                    funcName == "SA" ||
                    funcName == "WA" ||
                    funcName == "KA"
                )
                {
                    BaseAddAdd(funcName, originCards, range, value);
                }
                else if (funcName == "A")
                {

                    AdjustPorperty(elements[1], range, value);

                }
                else if (
                    funcName == "Px" ||
                    funcName == "Sx" ||
                    funcName == "Wx" ||
                    funcName == "Kx" ||
                    funcName == "Pb" ||
                    funcName == "Sb" ||
                    funcName == "Wb" ||
                    funcName == "Kb" ||
                    funcName == "Pdb" ||
                    funcName == "Sdb" ||
                    funcName == "Wdb" ||
                    funcName == "Kdb"
                )
                {
                    BuffDebuffMulti(funcName, originCards, range, value);
                }
                else if (funcName.Contains("C"))
                {
                    ConnectAddMulti(funcName, value);
                }
                else if (funcName == "D")
                {

                    DestroyCard(funcName, originCards, range, value);

                }
                else if (funcName == "TD")
                {

                    TimesDestroyCard(value);

                }
                else if (funcName == "TR_UP1")
                {
                    TransformCard_UP1(funcName, originCards, range, value);
                }
                else if (funcName == "I")
                {

                    InstantiateCard(funcName, originCards, range, value);

                }
                else if (funcName == "U")
                {

                    UpgradeCard(originCards, range, value);

                }
                else if (funcName == "F")
                {

                    Fortunate(value);

                }
                else if (funcName == "T")
                {
                    TimeCounter1(funcName, originCards, range, value);
                }
                else if (funcName == "L")
                {
                    LifeCounter1(funcName, originCards, range, value);
                }
            } //如果func和AI有关
            else
            {

                ExecuteFunctionAI(elements);

            }
        }
    }


    float FieldBuffValue(float value, string pro)//由场地buff造成的数值倍率
    {

        float n = 1;
        if (value >= 0)//表增益
        {
            switch (pro)
            {
                case "K":
                    n = 1f + (FieldManager.Instance.isOverload ? 1f : 0f) * 0.2f;
                    if (FieldManager.Instance.isFlower_Overload && this.times.Contains("晚"))
                    {
                        n += 0.5f;
                    }
                    break;
                default: break;
            }
        }
        else//表消耗
        {
            switch (pro)
            {
                case "P":
                    n = (1f + (FieldManager.Instance.isOverload ? 1f : 0f) * 0.1f);

                    break;
                case "S":
                    n =  (1f + (FieldManager.Instance.isOverload ? 1f : 0f) * 0.1f);
                    if (FieldManager.Instance.isFlower_Overload && !this.times.Contains("晚"))
                    {
                        n -= 0.2f;
                    }
                    break;
                default: break;
            }

        }
        n = Mathf.Max(0, n);
        return n;
    }

    //P S W K
    void BaseAdd(
        string funcName,
        List<Card> originCards,
        string range,
        float value
    )
    {
        List<Card> allCards = new List<Card>();
        FindAllCards(originCards, range, allCards); //根据原始卡牌和范围，找到所有作用的对象

        if (allCards.Count > 0)
        {
            isEffect = true;
        }

        foreach (var allCard in allCards)
        {
            if (value >= 0)
            {
                switch (funcName)
                {
                    case "P":
                        allCard.functionEffect.physicalHealth +=
                            (int)(((value * (1 + addLevelPower * this.addLevel)) * functionEffectEx.physicalHealthMulti + functionEffectEx.physicalHealthAdd) * FieldBuffValue(value, "P"));
                        functionEffectEx.Initialize("P");
                        break;

                    case "S":
                        allCard.functionEffect.spiritualHealth +=
                            (int)(((value * (1 + addLevelPower * this.addLevel)) * functionEffectEx.spiritualHealthMulti + functionEffectEx.spiritualHealthAdd) * FieldBuffValue(value, "S"));
                        functionEffectEx.Initialize("S");
                        break;
                    case "W":
                        allCard.functionEffect.workAbility +=
                            (int)(((value * (1 + addLevelPower * this.addLevel)) * functionEffectEx.workAbilityMulti + functionEffectEx.workAbilityAdd) * FieldBuffValue(value, "W"));
                        functionEffectEx.Initialize("W");
                        break;
                    case "K":
                        allCard.functionEffect.KPI +=
                            (int)(((value * (1 + addLevelPower * this.addLevel)) * functionEffectEx.KPIMulti + functionEffectEx.KPIAdd) * FieldBuffValue(value, "K"));
                        functionEffectEx.Initialize("K");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (funcName)
                {
                    case "P":
                        allCard.functionEffect.physicalHealth += (int)(value * FieldBuffValue(value, "P"));
                        break;
                    case "S":
                        allCard.functionEffect.spiritualHealth += (int)(value * FieldBuffValue(value, "S"));
                        break;
                    case "W":
                        allCard.functionEffect.workAbility += (int)(value * FieldBuffValue(value, "W"));
                        break;
                    case "K":
                        allCard.functionEffect.KPI += (int)(value * FieldBuffValue(value, "K"));
                        break;
                    default:
                        break;
                }
            }
        }

        // BuffBebuffedEffect(allCards);

    }

    //T
    void TimeCounter1(string funcName, List<Card> originCards, string range, float value)
    {
        List<Card> allCards = new List<Card>();
        FindAllCards(originCards, range, allCards); //根据原始卡牌和范围，找到所有作用的对象

        List<Card> allCards_Effect = new List<Card>();
        foreach (var allCard in allCards)
        {
            bool b = false;
            {
                foreach (var function in allCard.functions)
                {
                    if (function.Contains("T") && !function.Contains("TD"))//只有有计数器的同学，才会加
                    {
                        b = true;
                    }
                }
            }
            if (b)
            {
                isEffect = true;

                allCard.TimeCounter += (int)value;
                allCard.AutoDescription();
                allCards_Effect.Add(allCard);
            }

        }
        BuffBebuffedEffect(allCards_Effect);
    }


    //L
    void LifeCounter1(string funcName, List<Card> originCards, string range, float value)
    {
        List<Card> allCards = new List<Card>();
        FindAllCards(originCards, range, allCards); //根据原始卡牌和范围，找到所有作用的对象

        List<Card> allCards_Effect = new List<Card>();
        foreach (var allCard in allCards)
        {
            bool b = false;
            {
                foreach (var function in allCard.functions)
                {
                    if (function.Contains("TD"))//只有有生命的同学，才会加
                    {
                        b = true;
                    }
                }
            }
            if (b)
            {
                isEffect = true;

                allCard.life += (int)(value + addLevelPower * addLevel);
                allCards_Effect.Add(allCard);
            }
        }
        BuffBebuffedEffect(allCards_Effect);
    }
    //P/S/W/K+A
    void BaseAddAdd(
        string funcName,
        List<Card> originCards,
        string reference,
        float value
    )
    {
        isEffect = true;
        float referenceValue = 0;
        if (reference == "LV") //等级
        {
            referenceValue = this.addLevel;
        }
        else if (reference == "LP")//丢失的血量/总血量
        {
            float n = ((float)PlayerData.Instance.physicalHealthMax - (float)PlayerData.Instance.physicalHealth) / (float)PlayerData.Instance.physicalHealthMax;
            n = Mathf.Clamp01(n);
            referenceValue = n;
        }
        else if (reference == "TA")
        {
            referenceValue = FieldManager.Instance.TiredAll;
        }
        else if (reference == "NA")
        {
            referenceValue = FieldManager.Instance.NoColorAll;
        }
        else if (reference == "ET18")
        {
            referenceValue = FieldManager.Instance.ET18;
        }
        else if (reference == "ET42")
        {
            referenceValue = FieldManager.Instance.ET42;
        }
        else if (reference == "W")
        {
            referenceValue = PlayerData.Instance.workAbility;
        }
        else if (reference == "PM")
        {
            referenceValue = PlayerData.Instance.physicalHealthMax;
        }
        else if (reference == "SM")
        {
            referenceValue = PlayerData.Instance.spiritualHealthMax;
        }
        else if (reference == "PO")
        {
            referenceValue = FieldManager.Instance.OverFillP;
        }
        else if (reference == "SO")
        {
            referenceValue = FieldManager.Instance.OverFillS;
        }
        else if (reference == "FA")
        {
            referenceValue = PlayerData.Instance.favorAll;
        }
        else if (reference == "ER")
        {
            referenceValue = Mechanism.Instance.envirRollValue;
        }
        else if (reference == "Fish")
        {
            referenceValue = FieldManager.Instance.FishAll;
        }
        else if (reference == "LV5C")
        {
            referenceValue = FieldManager.Instance.Lv5C;
        }
        else if (reference == "FishP")
        {
            referenceValue = FieldManager.Instance.FishPeopleAll;
        }
        else if (reference == "PC")
        {
            referenceValue = PlayerData.Instance.playerCards.Count;
        }
        else if (reference == "T")
        {
            referenceValue = this.TimeCounter;
        }
        else if (reference == "Ka1AIKa1")
        {
            referenceValue = FieldManager.Instance.Ka1AIKa1;
        }
        else if (reference == "KAll")
        {
            referenceValue = FieldManager.Instance.KPIAll;
        }
        else if (reference == "L")
        {
            referenceValue = this.life;
        }
        else if (reference == "DC")
        {
            referenceValue = FieldManager.Instance.DestroyCard;
        }
        else if (reference == "None")
        {
            referenceValue = FieldManager.Instance.NoneAll;
        }
        else if (reference == "TC")
        {
            referenceValue = FieldManager.Instance.TypeCount;
        }
        else if (reference == "Uni")
        {
            referenceValue = FieldManager.Instance.universe;
        }




        if (funcName == "PA")
        {
            this.functionEffectEx.physicalHealthAdd += (int)(referenceValue * value * (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "SA")
        {
            this.functionEffectEx.spiritualHealthAdd += (int)(referenceValue * value * (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "WA")
        {
            this.functionEffectEx.workAbilityAdd += (int)(referenceValue * value * (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "KA")
        {
            this.functionEffectEx.KPIAdd +=
                (int)(referenceValue * value * (1 + addLevelPower * this.addLevel));
        }
    }

    //P/S/W/K + x/b/db
    void BuffDebuffMulti(
        string funcName,
        List<Card> originCards,
        string range,
        float value
    )
    {
        List<Card> allCards = new List<Card>();
        FindAllCards(originCards, range, allCards);
        List<Card> allCards_PropertyRange = new List<Card>();
        string r = null;
        if (
            funcName == "Px" ||
            funcName == "Sx" ||
            funcName == "Wx" ||
            funcName == "Kx"
        )
        {
            r = "!=0";
        }
        else if (
            funcName == "Pb" ||
            funcName == "Sb" ||
            funcName == "Wb" ||
            funcName == "Kb"
        )
        {
            r = ">0";
        }
        else if (
            funcName == "Pdb" ||
            funcName == "Sdb" ||
            funcName == "Wdb" ||
            funcName == "Kdb"
        )
        {
            r = "<0";
        }

        if (funcName.Contains("P"))
        {
            FindAllCards_PorpertyRange(r, "P", allCards, allCards_PropertyRange);
            foreach (var card in allCards_PropertyRange)//修改数值
            {
                card.functionEffect.physicalHealth =
                    (
                    int
                    )(card.functionEffect.physicalHealth *
                    Mathf.Pow(value, (1 + addLevelPower * this.addLevel)));

                Mechanism.Instance.UpdateTextPrefab(card);

            }
        }
        else if (funcName.Contains("S"))
        {
            FindAllCards_PorpertyRange(r,
            "S",
            allCards,
            allCards_PropertyRange);
            foreach (var
                card
                in
                allCards_PropertyRange //修改数值
            )
            {
                card.functionEffect.spiritualHealth =
                    (
                    int
                    )(card.functionEffect.spiritualHealth *
                    Mathf.Pow(value, (1 + addLevelPower * this.addLevel)));

                Mechanism.Instance.UpdateTextPrefab(card);

            }
        }
        else if (funcName.Contains("W"))
        {
            FindAllCards_PorpertyRange(r,
            "W",
            allCards,
            allCards_PropertyRange);
            foreach (var
                card
                in
                allCards_PropertyRange //修改数值
            )
            {
                card.functionEffect.workAbility =
                    (
                    int
                    )(card.functionEffect.workAbility *
                    Mathf.Pow(value, (1 + addLevelPower * this.addLevel)));

                Mechanism.Instance.UpdateTextPrefab(card);

            }
        }
        else if (funcName.Contains("K"))
        {
            FindAllCards_PorpertyRange(r,
            "K",
            allCards,
            allCards_PropertyRange);
            foreach (var
                card
                in
                allCards_PropertyRange //修改数值
            )
            {
                card.functionEffect.KPI =
                    (
                    int
                    )(card.functionEffect.KPI *
                    Mathf.Pow(value, (1 + addLevelPower * this.addLevel)));

                Mechanism.Instance.UpdateTextPrefab(card);

            }
        }

        if (allCards_PropertyRange.Count > 0)
        {
            isEffect = true;
            BuffBebuffedEffect(allCards_PropertyRange);
        }

    }

    //A
    void AdjustPorperty(string funcName, string property, float value)
    {
        value *= (1 + addLevelPower * addLevel);
        if (funcName == "Add")
        {
            if (property == "PM")
            {
                PlayerData.Instance.physicalHealthMax = (int)Mathf.Max(0, PlayerData.Instance.physicalHealthMax + value);
                PlayerData.Instance.physicalHealth = Mathf.Clamp(PlayerData.Instance.physicalHealth, 0, PlayerData.Instance.physicalHealthMax);
            }
            else if (property == "SM")
            {
                PlayerData.Instance.spiritualHealthMax = (int)Mathf.Max(0, PlayerData.Instance.spiritualHealthMax + value);
                PlayerData.Instance.spiritualHealth = Mathf.Clamp(PlayerData.Instance.spiritualHealth, 0, PlayerData.Instance.spiritualHealthMax);
            }

            else if (property == "KN")//KPI需求
            {
                Mechanism.Instance.KPINeed_EveryMonth = Mathf.Max(Mechanism.Instance.KPINeed_EveryMonth + (int)value, 0);
                Mechanism.Instance.KPINeed_EveryMonthText.text = Mechanism.Instance.KPINeed_EveryMonth.ToString();
            }
        }

        else if (funcName == "Mul")
        {
        }
        else if (funcName == "Addby")
        {
            string[] pros = property.Split('_', System.StringSplitOptions.RemoveEmptyEntries);
            if (pros.Length == 2)
            {
                string pro0 = pros[0];
                string pro1 = pros[1];
                if (pro1 == "PO")
                {
                    if (pro0 == "PM")
                    {
                        PlayerData.Instance.physicalHealthMax = (int)Mathf.Max(0, PlayerData.Instance.physicalHealthMax + value * FieldManager.Instance.OverFillP);
                        PlayerData.Instance.physicalHealth = Mathf.Clamp(PlayerData.Instance.physicalHealth, 0, PlayerData.Instance.physicalHealthMax);
                    }
                }
                else if (pro1 == "SO")
                {
                    if (pro0 == "SM")
                    {
                        PlayerData.Instance.spiritualHealthMax = (int)Mathf.Max(0, PlayerData.Instance.spiritualHealthMax + value * FieldManager.Instance.OverFillP);
                        PlayerData.Instance.spiritualHealth = Mathf.Clamp(PlayerData.Instance.spiritualHealth, 0, PlayerData.Instance.spiritualHealthMax);
                    }
                }
            }

        }
        isEffect = true;
        Buff(this);
    }

    //C
    void ConnectAddMulti(string funcName, float value)
    {
        if (funcName == "CP")
        {
            this.functionEffectEx.physicalHealthAdd +=
                (int)(value * (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "CS")
        {
            this.functionEffectEx.spiritualHealthAdd +=
                (int)(value * (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "CW")
        {
            this.functionEffectEx.workAbilityAdd +=
                (int)(value * (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "CK")
        {
            this.functionEffectEx.KPIAdd +=
                (int)(value * (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "CPx")
        {
            this.functionEffectEx.physicalHealthMulti =
                (
                int
                )(this.functionEffectEx.physicalHealthMulti *
                value *
                (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "CSx")
        {
            this.functionEffectEx.spiritualHealthMulti =
                (
                int
                )(this.functionEffectEx.spiritualHealthMulti *
                value *
                (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "CWx")
        {
            this.functionEffectEx.workAbilityMulti =
                (
                int
                )(this.functionEffectEx.workAbilityMulti *
                value *
                (1 + addLevelPower * this.addLevel));
        }
        else if (funcName == "CKx")
        {
            this.functionEffectEx.KPIMulti =
                (
                int
                )(this.functionEffectEx.KPIMulti *
                value *
                (1 + addLevelPower * this.addLevel));
        }
        isEffect = true;
    }

    //D
    void DestroyCard(string funcName, List<Card> originCards, string range, float value)
    {
        List<Card> allCards = new List<Card>();
        FindAllCards(originCards, range, allCards);

        //根据value再筛选一次
        int n = (int)value;
        List<Card> allCards_by_n = new List<Card>();
        FindAllSpecialCards(allCards, allCards_by_n, n);

        List<Card> cards_BeDesed = new List<Card>();//需要被消除的卡牌

        foreach (var allCard in allCards_by_n)
        {
            if (!Mechanism.Instance.cardsDestroyedThisTurn.Contains(allCard))//不能让一张卡牌被摧毁多次，不然三连数组会出问题 
            {
                if (this.id == 18)
                {
                    FieldManager.Instance.ET18++;
                }
                else if (this.id == 42)
                {
                    FieldManager.Instance.ET42++;
                }

                Mechanism.Instance.cardsDestroyedThisTurn.Add(allCard);
                cards_BeDesed.Add(allCard);
                FieldManager.Instance.DestroyCard++;

                PlayerData.Instance.playerCards.Remove(allCard);
                allCard.isAlive = false;
                if (allCard.id < 10000)
                {
                    Mechanism.Instance.TripletCards[allCard.id] -= 1; //从检测三连的卡组中移除
                }
            }
        }
        // Buff(this);
        if (cards_BeDesed.Count > 0)
        {
            isEffect = true;
            DestroyEffect(cards_BeDesed, Color.red);
        }

    }
    void FindAllSpecialCards(List<Card> allCards, List<Card> allCards_by_n, int n)//专门用来找D和TR的最后一个参数对应的卡牌
    {
        if (n == 0)//这个操作TD是没有的，TD的最后一个参数是控制时间的
        {
            allCards_by_n.AddRange(allCards);
        }
        else if (n < 20000)//1-19999都会查找具体id的卡牌，并摧毁,注意，是所有该id的卡牌
        {
            {
                foreach (var card in allCards)
                {
                    if (card.id == n)
                    {
                        allCards_by_n.Add(card);
                    }
                }
            }
        }
        else//>=20000的特殊卡牌
        {
            if (n == 20001)//如果规定要摧毁随机一张一级红
            {
                List<Card> allCard_Temp = new List<Card>();
                foreach (var card in allCards)
                {
                    if (card.actionType == ActionType.Dynamic && card.qualityLevel == 1)
                    {
                        allCard_Temp.Add(card);
                    }
                }
                if (allCard_Temp.Count > 0)
                {
                    allCards_by_n.Add(allCard_Temp[Random.Range(0, allCard_Temp.Count)]);
                }

            }
            if (n == 20012)//如果规定要摧毁所有无色卡
            {
                List<Card> allCard_Temp = new List<Card>();
                foreach (var card in allCards)
                {
                    if (card.actionType == ActionType.Unknown)
                    {
                        allCard_Temp.Add(card);
                    }
                }
                allCards_by_n.AddRange(allCard_Temp);
            }
        }
    }


    //TR_UP1 //变化指定，使其成为随机升一级的卡牌  //!!!记住这个老逼一定要在quque6执行
    void TransformCard_UP1(string funcName, List<Card> originCards, string range, float value)
    {
        List<Card> allCards = new List<Card>();
        FindAllCards(originCards, range, allCards);

        //根据value再筛选一次
        int n = (int)value;
        List<Card> allCards_by_n = new List<Card>();
        FindAllSpecialCards(allCards, allCards_by_n, n);


        for (int i = 0; i < allCards_by_n.Count; i++)
        {
            Card allCard = allCards_by_n[i];
            GameObject allCardGameobject = Mechanism.Instance.cards_cardPersonalGamePrefabs_Dic[allCard];
            if (!Mechanism.Instance.cardTransformThisTurn.Contains(allCard))//不能让一张卡牌被变换多次，不然三连数组会出问题 
            {
                Mechanism.Instance.cardTransformThisTurn.Add(allCard);

                PlayerData.Instance.playerCards.Remove(allCard);

                if (allCard.id < 10000)
                {
                    Mechanism.Instance.TripletCards[allCard.id] -= 1; //从检测三连的卡组中移除
                }

                int level = Mathf.Min(allCard.qualityLevel + 1, 5);
                Card newCard = new Card(CardStore.Instance.RandomCard(level, true));
                CardDisplayPersonalGame cpg = allCardGameobject.GetComponent<CardDisplayPersonalGame>();
                // cpg.JudgeTitle(newCard);
                // cpg.JudgeFontSize(newCard);//
                cpg.JudgeActionColor(newCard.actionType);//改颜色
                cpg.JudgeQualityColor(newCard);
                cpg.StartCoroutine(cpg.Delay_JudgeAdditive(newCard, Time.deltaTime * i * 3));
                // cpg.JudgeFlower(newCard);
                //万一被改变的卡牌需要用到buff函数来改变自身的颜色，注意，原来的se.originMatColor 是原卡牌的颜色，需要改为现卡牌的颜色
                SimpleEffect se = allCardGameobject.GetComponent<SimpleEffect>();
                // se.originMatColor = se.mat.color;

                PlayerData.Instance.playerCards.Add(newCard);
                PlayerData.Instance.SortCards();
                // LibraryManager.Instance.UpdateLibrary();


            }
        }

        if (allCards_by_n.Count > 0)
        {
            isEffect = true;
            BuffBebuffedEffect(allCards_by_n);
        }

    }
    //TD
    void TimesDestroyCard(float value)
    {

        life--;
        this.AutoDescription();
        if (life > 0 && this.isAlive)//命还在 且 没有被其他卡牌给摧毁
        {
            isEffect = true;
            List<Card> cards_BeDesed = new List<Card>();
            cards_BeDesed.Add(this);
            TDEffect();
        }
        else
        {
            if (
                !Mechanism.Instance.cardsDestroyedThisTurn.Contains(this) //不能让一张卡牌被摧毁多次，不然三联数组会出问题
            )
            {
                isEffect = true;
                Mechanism.Instance.cardsDestroyedThisTurn.Add(this);
                List<Card> cards_BeDesed = new List<Card>();
                cards_BeDesed.Add(this);
                DestroyEffect(cards_BeDesed, Color.red);
                FieldManager.Instance.DestroyCard++;

                PlayerData.Instance.playerCards.Remove(this);
                this.isAlive = false;
                if (this.id < 10000)
                {
                    Mechanism.Instance.TripletCards[this.id] -= 1; //从检测三连的卡组中移除
                }
            }
        }
        // Buff(this);
    }

    //I
    void InstantiateCard(
        string funcName,
        List<Card> originCards,
        string range,
        float value
    )
    {
        isEffect = true;

        List<Card> allCards = new List<Card>();

        int amount = int.Parse(range); //I方法的参数三——range是生成此卡牌的数量，value是生成的卡牌id


        int id = (int)value;
        if (id < 10000)
        {
            foreach (var card in CardStore.Instance.cards)
            {
                if (card.id == id)
                {
                    float interval = 0.05f;
                    for (int i = 0; i < amount; i++)
                    {
                        Card kidCard = new Card(card);
                        kidCard.isNew = true;
                        InstantiateEffect(this, kidCard, interval * i);
                        PlayerData.Instance.playerCards.Add(kidCard);
                        PlayerData.Instance.SortCards();
                        // LibraryManager.Instance.UpdateLibrary();
                    }
                }
            }
        }
        else if (id < 20000)
        {
            foreach (var card in CardStore.Instance.Colourless_Cards)
            {
                if (card.id == id)
                {
                    float interval = 0.05f;
                    for (int i = 0; i < amount; i++)
                    {
                        Card kidCard = new Card(card);
                        kidCard.isNew = true;
                        InstantiateEffect(this, kidCard, interval * i);
                        PlayerData.Instance.playerCards.Add(kidCard);
                        PlayerData.Instance.SortCards();
                        // LibraryManager.Instance.UpdateLibrary();
                    }
                }
            }
        }
        else//特殊的卡牌,>=20000
        {
            float interval = 0.05f;
            for (int i = 0; i < amount; i++)
            {
                Card kidCard;
                if (id == 20021)//返回一张+1等级的随机卡 若干次
                {
                    kidCard = CardStore.Instance.RandomCard(Mathf.Min(PlayerData.Instance.postLevel + 1, 5), true);//返回+1等级的随机卡
                }
                else if (id == 20040)//返回一张 某一个AI的流派卡牌
                {
                    AIData AIData_r = AIMechanism.Instance.RandomAIData();
                    if (AIData_r != null)
                    {
                        kidCard = CardStore.Instance.RandomCard_ByType(AIData_r.type);//返回+1等级的随机卡
                    }
                    else
                    {
                        kidCard = CardStore.Instance.cards[0];
                    }
                }
                else
                {
                    kidCard = CardStore.Instance.RandomCard();
                }
                kidCard.isNew = true;
                InstantiateEffect(this, kidCard, interval * i);
                PlayerData.Instance.playerCards.Add(kidCard);
                PlayerData.Instance.SortCards();
                // LibraryManager.Instance.UpdateLibrary();
            }
        }
    }

    //U
    void UpgradeCard(List<Card> originCards, string range, float value)
    {
        List<Card> allCards = new List<Card>();
        FindAllCards(originCards, range, allCards); //根据原始卡牌和范围，找到所有作用的对象
        List<int> ids_AlreadyUpgrade = new List<int>(); //已经buff过的卡牌的id，因为九宫格这种卡牌升级中，一个类型的卡牌只能被升级一次
        int level = (int)(value + addLevel * addLevelPower); //level是升的级数，也就是+5时，一次就可以让周围的卡牌升2级

        List<Card> allCards_Effect = new List<Card>();//需要加特效的卡牌

        foreach (var card in allCards)
        {
            if (
                !ids_AlreadyUpgrade.Contains(card.id) //如果没有升级过，就升级
            )
            {
                ids_AlreadyUpgrade.Add(card.id);
                Mechanism.Instance.PromoteCards(card.id, level); //升级所有PlayData以及CardStore中的卡
                allCards_Effect.Add(card);
            }
        }

        if (allCards_Effect.Count > 0)
        {
            isEffect = true;
            BuffBebuffedEffect(allCards_Effect);
        }

    }

    //F
    void Fortunate(float value)
    {
        isEffect = true;
        int n = (int)value;
        FieldManager.Instance.luckyTimes += n;
    }
}
