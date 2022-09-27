using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlayState
{
    MainMenu,//主界面
    Black,//黑衣人动画
    Chess,//玩家游玩部分
    Story,//
}

//单次开始后的阶段
public enum Phase
{
    Start,
    CreatCardAnimation,
    Calculate,
    TripletReward,
    Story,
    HolidayStore,
    KPITest,//检测游戏胜利失败的环节
    WeekendMeeting,
}

public struct FunctionEffect
{
    public int physicalHealth;
    public int spiritualHealth;
    public int workAbility;
    public int KPI;
}

public class FunctionEffectEx//连轴专用
{
    public int physicalHealthAdd;
    public int spiritualHealthAdd;
    public int workAbilityAdd;
    public int KPIAdd;
    public int physicalHealthMulti;
    public int spiritualHealthMulti;
    public int workAbilityMulti;
    public int KPIMulti;

    public FunctionEffectEx()
    {
        this.physicalHealthAdd = 0;
        this.spiritualHealthAdd = 0;
        this.workAbilityAdd = 0;
        this.KPIAdd = 0;
        this.physicalHealthMulti = 1;
        this.spiritualHealthMulti = 1;
        this.workAbilityMulti = 1;
        this.KPIMulti = 1;
    }
    public void Initialize()
    {
        this.physicalHealthAdd = 0;
        this.spiritualHealthAdd = 0;
        this.workAbilityAdd = 0;
        this.KPIAdd = 0;
        this.physicalHealthMulti = 1;
        this.spiritualHealthMulti = 1;
        this.workAbilityMulti = 1;
        this.KPIMulti = 1;
    }
    public void Initialize(string s)
    {
        switch (s)
        {
            case "P":
                this.physicalHealthAdd = 0;
                this.physicalHealthMulti = 1;
                break;
            case "S":
                this.spiritualHealthAdd = 0;
                this.spiritualHealthMulti = 1;
                break;
            case "w":
                this.workAbilityAdd = 0;
                this.workAbilityMulti = 1;
                break;
            case "K":
                this.KPIAdd = 0;
                this.KPIMulti = 1;
                break;
            default:
                break;
        }
    }
}



public partial class Mechanism : MonoSingleton<Mechanism>
{
    public PlayState playState;

    //<<<<<Global>>>>>
    public Camera cam;//主相机
    [HideInInspector] public int week = 1;//周数计时
    public TextMeshProUGUI weekText;

    [HideInInspector] public int envirRollValue;//环境卷度
    public TextMeshProUGUI envirRollValueText;
    public int KPINeed_EveryMonth = 200;//每个月的KPI需求
    public TextMeshProUGUI KPINeed_EveryMonthText;
    public GameObject globalUI;
    public GameObject chessBoard;


    public GameObject FishPool;//鱼池子的坐标，用来送屎和塞卡

    // public Button[] buttons;
    public GameObject StartButton;
    public GameObject LibraryButton;
    public GameObject ExecuteButton;
    public GameObject PassButton;
    // public GameObject RotatePannel;

    public int[] postUpgradeNeeds = new int[4];//升级岗位所需的门槛

    bool buttonActive;
    public Phase phase;

    // public GameObject Scene;//20个格子
    // public GameObject WeekdayPannel;
    public float SignStayTime = 2f;//提示的文字停留的事件
    public GameObject SignPrefab;//提示的文字
    // public GameObject AI_Debuff_Group;//搞AI的组
    // public GameObject AI_Debuff_Sign_Pannel;//搞AI提示面板
    public TextMeshPro AI_Debuff_BUff_Text;//玩家欺负AI的文本提示
    public TextMeshPro AI_Debuff_BUff_Text2; //AI欺负玩家的文本提示

    //<<<<<3_1>>>>>
    public int chooseTimes = 0;//三选一面板出现的次数
    public GameObject Pannel3_1;
    public GameObject TripletGroup;//三连提示的文字存放的group
    public Transform point1_3;
    public ParticleSystem ps_Triplet;//三连的空中发光特效
    public AnimationCurve ac_Triplet;//三花聚顶位移控制曲线
    //Start
    [HideInInspector] public bool isAISendShit = false;

    //<<<<<CreatCardAnimation>>>>>
    public Transform fishMouth;
    float animationTimer;//计算时间//CreatCardAnimation()
    int animationCounter;//计算生成prefab的次数以及每张卡牌的posNum//CreatCardAnimation()
    public float CreateTime = 0.3f;//创建每一张牌花费的时间
    public AnimationCurve anic;//用于控制弹跳
    public Transform[] roots = new Transform[20];
    public GameObject cardPersonalGamePrefab;
    List<GameObject> cardPersonalGamePrefabs = new List<GameObject>();//20个card预制体的数组
    [HideInInspector] public List<Card> cardList = new List<Card>(); // 20个card的集合
    [HideInInspector] public Dictionary<Card, GameObject> cards_cardPersonalGamePrefabs_Dic = new Dictionary<Card, GameObject>();//card和对应的cardPersonalGamePrefab的dic

    List<Card> playerCards = new List<Card>();//PlayerData中playerCards数组的拷贝

    List<Card> playerCards_Night = new List<Card>();
    List<Card> playerCards_Noon = new List<Card>();
    List<Card> playerCards_Others = new List<Card>();

    //<<<<<Assign>>>>>
    float animationTimer2;//计算时间，生成的字体位移动画协程专用&&用户面板数字增长专用
    List<GameObject> cardPersonalGamePrefabs_CurrentQueue = new List<GameObject>();//现在执行的队列

    // public float ConnectingTime = 1f;//连轴卡牌生成的数值停留时间
    // public float buffTime = 0.1f;//一次buff的时间
    // public float AI_Debuff_Time = 1f;//queue12的AIdebuff卡牌的Assign动画时间
    public float NormalCard_Time = 1.2f;//queue10的正常卡牌的Assign动画时间
    public float GiveTime = 1f;//数值最后的位移时间
    public float PlayerDataChangeTime = 2;//UI上玩家数据改变的时间
    public GameObject textPrefab;
    List<GameObject> TextPrefabs = new List<GameObject>();//Assign中卡牌冒出来的数值
    bool isFirstGive = true;//赋予时，每个队列是否是第一次
    bool isFirstGive2 = true;//用于处理buff debuff以及连轴卡牌的延迟生成,8 9 11 12 13 14
    public List<Material> FontMats = new List<Material>();
    [HideInInspector] public FunctionEffect functionEffectBuffer = default;//总的数值缓冲区
    PlayerData_OnlyData playerData_Last;//上一帧的playerData数据

    [HideInInspector] public Dictionary<Card, List<GameObject>> Cards_TextPrefabList_Dic = new Dictionary<Card, List<GameObject>>();//用于后面队列的卡牌修改前面队列卡牌的文字效果,一张卡牌对应一个文字gameobj的集合
    bool isAudioPlayed = false;

    //<<<<<Triplet>>>>>
    [HideInInspector] public int[] TripletCards;//用于检测场景中物体三连情况的数组，卡牌id-卡牌数量
    [HideInInspector] public bool isChoose = false;//是否有三连
    bool isFirstTriplet = true;//检测是不是三连的第一帧

    //<<<<<HolidayStore>>>>>
    public GameObject holidayStorePanel;
    bool isFirstHolidayStore = true;


    //<<<<<WeekendMeeting>>>>>
    float recoverRate = 0.3f;//每周末恢复的体力比例
    bool isFirstWeekendMeeting = true;
    bool isFirstChooseCardButton = true;//选择卡牌的按钮是否还未被点击
                                        // public GameObject ChooseButton;//周末会议选择卡牌的button
                                        // public GameObject HolidayPannel;
                                        // public GameObject PostUpgradeGroup;//岗位升级提示的文字存放的group
    public TextMeshPro SignAll;//万能公告牌
    public List<string> Signs = new List<string>();//万能公告牌上的字


    public GameObject InstantiateCardsGroup;//放置生出来的卡牌的地方

    public List<Card> cardsDestroyedThisTurn = new List<Card>();
    public List<Card> cardTransformThisTurn = new List<Card>();


    public GameObject Warning_Panel;//用于显示死亡，警告信息的面板
    public TextMeshProUGUI warningText;
    public GameObject BackMainMenuButton;//游戏结束，返回主菜单

    //KPINeed数据测试
    public float KPI_Up_PerWeek_1_Min;
    public float KPI_Up_PerWeek_1_Max;
    public float KPI_Up_PerWeek_9_Min;
    public float KPI_Up_PerWeek_9_Max;



    private void Awake()
    {
        Time.timeScale = 1;
        playState = PlayState.MainMenu;
        Application.targetFrameRate = 60;
        // Screen.SetResolution(1920, 1080, true);
        int width = Mathf.Min(2560, Screen.width);
        int height = width * 9 / 16;
        Screen.SetResolution(width, height, true);
        // WeekdayPannel.SetActive(true);
        // HolidayPannel.SetActive(false);
        // Scene.SetActive(true);
        MeshRenderer mr = StartButton.GetComponent<MeshRenderer>();
        phase = Phase.Start;
        buttonActive = true;
        globalUI.SetActive(false);
        chessBoard.SetActive(true);

        SkullControl.Instance.GenerateEyeBall(0);
        SkullControl.Instance.GenerateEyeBall(1);
        SkullControl.Instance.GenerateEyeBall(2);
    }
    private void Start()
    {
        KPINeed_EveryMonthText.text = KPINeed_EveryMonth.ToString();
        // AI_Debuff_Sign_Pannel.SetActive(false);
        Warning_Panel.SetActive(false);
        BackMainMenuButton.SetActive(false);
        // StayTime = Mathf.Clamp(StayTime, 0, GiveTime);
        // RotatePannel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // if (!TeachManager.Instance.isGuide)
        // {
        //     TeachManager.Instance.SetGuide(StartButton, true);
        // }
        TeachManager.Instance.SetGuide(ExecuteButton, true);
        UpdateMechanism();//PlayerData中playerCards数组拷贝给本类中的playerCards，并根据时间分好组
        TripletCards = new int[150];

        // TeachManager.Instance.TeachEventTrigger("开头介绍");
    }



    //点击“开始”按钮后
    public void OnClickStart()
    {

        //PlayerDataLast的赋值
        PlayerDataLastTransfer();

        // else
        // {
        phase = Phase.CreatCardAnimation;
        //！
        // CameraManager.Instance.SetVirtualCam("ChessCam");ir
        // // CameraManager.Instance.SetVirtualCam("ChessFeatureCam");
        // CameraManager.Instance.SetVirtualCamFollow(roots[0]);
        // }


        //Field特效一回合体验卡到期归零
        FieldManager.Instance.TiredAll = 0;
        FieldManager.Instance.NoColorAll = 0;

        FieldManager.Instance.FishAll = 0;
        FieldManager.Instance.Lv5C = 0;
        FieldManager.Instance.FishPeopleAll = 0;
        FieldManager.Instance.NoneAll = 20;
        FieldManager.Instance.TypeCount = 0;
        FieldManager.Instance.universe = 0;
        //计算有多少人是摸鱼流的
        foreach (var AI_Cha in AIMechanism.Instance.AI_Chas)
        {
            AIData AIData = AI_Cha.AIData;
            if (AIData != null)
            {
                if (AIData.type == Type.Fishlike)
                {
                    FieldManager.Instance.FishPeopleAll++;
                }
            }

        }

        if (AIMechanism.Instance.CalculateAIDataCount() > 0)
        {
            FieldManager.Instance.Ka1AIKa1 = AIMechanism.Instance.CalculateBestAI("Ka1").PSWK_Ability.K;//返回工作能力最强AI的工作能力
        }

        buttonActive = false;

        // foreach (var button in buttons)
        // {
        //     button.gameObject.SetActive(buttonActive);
        // }

        // phase = Phase.CreatCardAnimation;

        //初始化FuncEffect

        //初始化,去掉上一次操作的残留
        foreach (var cardPersonalGame in cardPersonalGamePrefabs)
        {
            cardPersonalGame.GetComponent<CardDisplayPersonalGame>().MatRet();
            Destroy(cardPersonalGame);
        }
        cardPersonalGamePrefabs.Clear();

        UpdateMechanism();//清除playerCards并将playerData中的数据转移过来,并且根据时间段分好组


        cardList.Clear();
        cards_cardPersonalGamePrefabs_Dic.Clear();

        cardsDestroyedThisTurn.Clear();
        cardTransformThisTurn.Clear();


        //用于三连统计的卡组初始化
        for (int i = 0; i < TripletCards.Length; i++)
        {
            TripletCards[i] = 0;
        };

        // PlayerData卡库排序
        PlayerData.Instance.SortCards();
        // 依据新的PlayerData卡库，实例化生成新的卡组中卡牌
        // LibraryManager.Instance.UpdateLibrary();

        functionEffectBuffer = default;//Black

        isFirstTriplet = true;
        isTripleyFXPlayed = false;
        isFirstWeekendMeeting = true;
        isFirstChooseCardButton = true;
        isFirstHolidayStore = true;
        isAudioPlayed = false;
        isFirstCreatAnimation = true;
        isFirstOpenMouth = true;
        isFirstAssign = true;
        //周末用于选择卡牌的按钮
        // ChooseButton.SetActive(true);



    }
    public void OnClickContinue()
    {
        Mechanism.Instance.phase++;
    }

    // public void Rotate_RotatePannel()
    // {
    //     Animator a = RotatePannel.GetComponent<Animator>();
    //     a.SetTrigger("Rotate");
    // }
    IEnumerator WaitAISendShit()//等AI塞完屎
    {
        while (true)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer > 2.5f)
            {
                animationTimer = 0;
                // phase = Phase.CreatCardAnimation;
                // CameraManager.Instance.SetVirtualCam("ChessFeatureCam");
                // CameraManager.Instance.SetVirtualCamFollow(roots[0]);
                isAISendShit = false;
                yield break;
            }
            yield return null;
        }
    }
    void O_C_GlobalGI()
    {
        globalUI.SetActive(!globalUI.activeInHierarchy);
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            PlayerData.Instance.O_C_Texts();
            O_C_GlobalGI();
        }



        if (playState == PlayState.MainMenu)
        {

        }
        else if (playState == PlayState.Black)
        {

        }
        else if (playState == PlayState.Story)
        {

        }
        else if (playState == PlayState.Chess)
        {
            if (phase == Phase.CreatCardAnimation)
            {
                CreatCardAnimation();

            }
            else if (phase == Phase.Calculate)
            {
                Assign();
            }
            else if (phase == Phase.TripletReward)
            {
                //三连奖励
                TripletReward();
            }
            else if (phase == Phase.Story)
            {
                playState = PlayState.Story;
                StoryManager.Instance.StartPlot();
            }
            else if (phase == Phase.HolidayStore)
            {
                if (isFirstHolidayStore)
                {
                    isFirstHolidayStore = false;
                    float r = Random.Range(0, 1f);
                    if ((r > 0.25f && week > 3) || week == 3)//第三周必第一次 之后每周刷出的概率为25%
                    {
                        // holidayStorePanel.SetActive(true);
                        HolidayStore.Instance.OpenStore();
                        if (TeachManager.Instance.isFirstHolidayStore)
                        {
                            TeachManager.Instance.TeachEventTrigger_Delay("假日商店介绍", 1.1f);
                            TeachManager.Instance.isFirstHolidayStore = false;
                        }
                    }
                    else
                    {
                        phase++;
                    }
                }

            }
            else if (phase == Phase.KPITest)
            {
                KPITest();
            }
            else if (phase == Phase.WeekendMeeting)
            {
                WeekendMeeting();
            }
            else if (phase == Phase.Start && buttonActive == false && isAISendShit == false)//第二周及其以后，第一次进入Start阶段
            {
                buttonActive = true;
                // Rotate_RotatePannel();

                WeekendMeetingAll.Instance.Close();
                AIMechanism.Instance.AI_Exchange_OnClickButton();
                AI_Debuff_BUff_Text.text = null;
                AI_Debuff_BUff_Text2.text = null;
                bool isSend = AIMechanism.Instance.AI_Exchange_Assign_Queue16();//塞屎环节
                if (isSend)
                {
                    isAISendShit = true;
                    StartCoroutine(WaitAISendShit());
                }

                // if (week == 2)
                // {
                //     // TeachManager.Instance.TeachEventTrigger("公告栏介绍");
                // }
                if (week == AIMechanism.Instance.AI_Appear_Week)
                {
                    TeachManager.Instance.TeachEventTrigger_Delay("小组成员介绍", 0.21f);
                }



                StartButton.GetComponent<ClickButton>().Start_2_Continue();
                TeachManager.Instance.SetGuide(StartButton, true);
                TeachManager.Instance.SetGuide(ExecuteButton, true);

                PlayerData.Instance.UpdateDataToUI_Weekday();
                if (week % 4 == 1 && week != 1)//月初
                {
                    UpdateKPINeedEveryMonth();
                    SkullControl.Instance.GenerateEyeBall(0);//生成眼球
                }

                //黑衣人提示周数和月数
                string blackWords = weekText.text + "开始了";
                if (week % 4 == 0)
                {
                    blackWords += "，月末到了";
                }
                if (TeachManager.Instance.isGuide && AIMechanism.Instance.AI_Appear_Week == week)
                {

                }
                else
                {
                    StoryManager.Instance.BlackSpeak_Delay(blackWords, 0.21f);
                }

            }
        }
    }



    public void PhaseAddAdd()
    {
        phase++;
    }

    //生成卡牌并且播放动画
    float Timer_fish = 0f;
    float fishEmergyTime = 0.3f;
    bool isFirstCreatAnimation = true;//CreatCardAnimation的第一帧
    bool isFirstOpenMouth = true;//计时器结束后，张开嘴巴时的那一帧
    void CreatCardAnimation()
    {
        if (isFirstCreatAnimation)
        {
            FishControler.Instance.Open_Close_All();
            isFirstCreatAnimation = false;
        }
        Timer_fish += Time.deltaTime;
        //鱼出现的延时
        if (Timer_fish < fishEmergyTime)
        {
            return;
        }
        else
        {
            if (isFirstOpenMouth)
            {
                FishControler.Instance.Mouth();
                isFirstOpenMouth = false;
            }
        }


        //此次操作
        animationTimer += Time.deltaTime;

        if (animationTimer > CreateTime)//每隔一段时间生一个
        {
            animationTimer = 0;
            animationCounter++;

            if (animationCounter > 1 && cardPersonalGamePrefabs.Count == animationCounter - 1)//众神归位
            {

                if (cardPersonalGamePrefabs[animationCounter - 2] != null)
                {
                    cardPersonalGamePrefabs[animationCounter - 2].transform.localPosition = Vector3.zero;//上一个cardPG归位
                }
            }


            if (animationCounter == 21 || (playerCards_Night.Count == 0 && playerCards_Noon.Count == 0 && playerCards_Others.Count == 0))//第二十次循环结束或者卡抽光了
            {
                animationCounter = 0;
                // CameraManager.Instance.SetVirtualCam("ChessCam");
                phase = Phase.Calculate;
                FishControler.Instance.Open_Close_All();
                FishControler.Instance.Mouth();
                Timer_fish = 0;
                return;
            }


            if (animationCounter % 4 == 0)//晚
            {
                if (playerCards_Night.Count == 0)
                {
                    SkipTime_Create();
                    return;
                }
                else
                {
                    CreatCard(ref playerCards_Night);//用playerCards_Night这个List来创建夜晚卡
                }
            }
            else if ((animationCounter % 4 == 2))//中,如果中午没有事情，那么会用Others的行为来填充
            {
                if (playerCards_Noon.Count == 0)
                {
                    if (playerCards_Others.Count > 0)
                    {
                        CreatCard(ref playerCards_Others);
                    }
                    else
                    {
                        SkipTime_Create();
                        return;
                    }

                }
                else//playerCards_Noon中有卡牌，就用这个卡牌来填充
                {
                    CreatCard(ref playerCards_Noon);
                }
            }
            else
            {
                if (playerCards_Others.Count == 0)//其他时刻
                {
                    SkipTime_Create();
                    return;
                }
                else
                {
                    CreatCard(ref playerCards_Others);
                }
            }
        }
    }//赋值
    List<GameObject> TextPrefabs_P = new List<GameObject>();
    List<GameObject> TextPrefabs_S = new List<GameObject>();
    List<GameObject> TextPrefabs_W = new List<GameObject>();
    List<GameObject> TextPrefabs_K = new List<GameObject>();

    List<GameObject> TextPrefabs_Now = new List<GameObject>();//现在正在使用的TextPrefabs
    GameObject DeltaAll = null;//左边这周增加/减少的量（当前）
    public List<GameObject> DeltaAlls = new List<GameObject>();//四个deltaAll
    public List<GameObject> emits = new List<GameObject>();//落下时与屏幕摩擦的火花

    void BeginEmit(int n)
    {
        if (!emits[n].activeInHierarchy)
        {
            emits[n].SetActive(true);
            ParticleSystem ps = emits[n].GetComponent<ParticleSystem>();
            ps.Stop();
            ps.Play();
        }
    }

    bool isFirstPlayMagic = true;//magic音效在左侧delta字母逐渐明显的时候释放

    void LastAssign()//最后的赋值部分
    {
        CardStore.Instance.probability = new Probability(PlayerData.Instance.postLevel);
        CardStore.Instance.probability.CalculateLevelNumsFinal();//3选1界面出来前先算好概率

        TextMeshPro physicalHealthText = PlayerData.Instance.physicalHealthText;
        TextMeshPro spiritualHealthText = PlayerData.Instance.spiritualHealthText;
        TextMeshPro workAbilityText = PlayerData.Instance.workAbilityText;
        TextMeshPro KPIText = PlayerData.Instance.KPIText;
        if (isFirstGive)
        {

            CameraManager.Instance.SetVirtualCam("BlackCam", 0.5f);
            isFirstGive = false;
            foreach (var card in cardList)
            {
                functionEffectBuffer.physicalHealth += card.functionEffect.physicalHealth;
                functionEffectBuffer.spiritualHealth += card.functionEffect.spiritualHealth;
                functionEffectBuffer.workAbility += card.functionEffect.workAbility;
                functionEffectBuffer.KPI += card.functionEffect.KPI;
            }
            TextPrefabs_P.Clear();
            TextPrefabs_S.Clear();
            TextPrefabs_W.Clear();
            TextPrefabs_K.Clear();
            TextPrefabs_Now.Clear();
            TextPrefabs_Now = TextPrefabs_P;

            foreach (var TextPrefab in TextPrefabs)
            {
                TextMeshPro tmp = TextPrefab.GetComponent<TextMeshPro>();
                if (tmp.color.CompareRGB(Color.red))
                {
                    TextPrefabs_P.Add(TextPrefab);
                }
                else if (tmp.color.CompareRGB(Color.blue))
                {
                    TextPrefabs_S.Add(TextPrefab);
                }
                else if (tmp.color.CompareRGB(Color.green))
                {
                    TextPrefabs_W.Add(TextPrefab);
                }
                else if (tmp.color.CompareRGB(new Color(1, 150f / 255f, 0, 1)))//黄色
                {
                    TextPrefabs_K.Add(TextPrefab);
                }
            }


            DeltaAlls[0].transform.position = physicalHealthText.transform.position;
            DeltaAlls[1].transform.position = spiritualHealthText.transform.position;
            DeltaAlls[2].transform.position = workAbilityText.transform.position;
            DeltaAlls[3].transform.position = KPIText.transform.position;


            //重置所有的deltaAll
            for (int i = 0; i < DeltaAlls.Count; i++)
            {

                DeltaAlls[i].SetActive(true);
                DeltaAlls[i].GetComponent<TextMeshPro>().text = "0";
                DeltaAlls[i].transform.position += new Vector3(0, 0.04f, 0);
                DeltaAlls[i].transform.localScale = Vector3.zero;
                // StartCoroutine(DeltaAllMoveAndDie(s, destPos));
            }

            DeltaAll = DeltaAlls[0];
        }

        if (animationTimer < GiveTime)
        {
            //不加就会出错，很怪
            DeltaAlls[0].GetComponent<MeshRenderer>().material = FontMats[0];
            DeltaAlls[1].GetComponent<MeshRenderer>().material = FontMats[1];
            DeltaAlls[2].GetComponent<MeshRenderer>().material = FontMats[2];
            DeltaAlls[3].GetComponent<MeshRenderer>().material = FontMats[3];

            if (TextPrefabs_Now.Count == 0)//说明当前属性没有改变
            {
                animationTimer += GiveTime + PlayerDataChangeTime;
                return;
            }
            if (animationTimer > 0.5f * GiveTime && isFirstPlayMagic)
            {
                isFirstPlayMagic = false;
                AudioManager.Instance.PlayClip("magic0");
            }

            foreach (var TextPrefab in TextPrefabs_Now)
            {
                float lerpFactor = animationTimer / GiveTime;
                lerpFactor = Mathf.Clamp01(lerpFactor);
                TextMeshPro tmp = TextPrefab.GetComponent<TextMeshPro>();
                Vector3 UIWorldPos;
                if (TextPrefabs_P.Contains(TextPrefab))
                {
                    UIWorldPos = PlayerData.Instance.physicalHealthText.transform.position;
                }
                else if (TextPrefabs_S.Contains(TextPrefab))
                {
                    UIWorldPos = PlayerData.Instance.spiritualHealthText.transform.position;
                }
                else if (TextPrefabs_W.Contains(TextPrefab))
                {
                    UIWorldPos = PlayerData.Instance.workAbilityText.transform.position;
                }
                else if (TextPrefabs_K.Contains(TextPrefab))
                {
                    UIWorldPos = PlayerData.Instance.KPIText.transform.position;
                }
                else
                {
                    Debug.Log("UIPoserror");
                    UIWorldPos = new Vector3(Screen.width / 4, Screen.height / 4, 0);
                }
                UIWorldPos += new Vector3(0, 0.04f, 0);

                tmp.color = new Color(tmp.color[0], tmp.color[1], tmp.color[2], Mathf.Clamp01(1 - 1.2f * lerpFactor));//渐隐

                Vector3 pos = Vector3.Lerp(TextPrefab.transform.position, UIWorldPos, Mathf.Pow(lerpFactor, 5f));
                TextPrefab.transform.position = pos;
            }


            //给deltaAll赋值
            int deltaValue = 0;
            float lerpFactor_d = Mathf.Clamp01((animationTimer - GiveTime * 0.5f) / (GiveTime * 0.5f) + 0.1f);
            if (TextPrefabs_Now == TextPrefabs_P)
            {
                deltaValue = functionEffectBuffer.physicalHealth;
            }
            else if (TextPrefabs_Now == TextPrefabs_S)
            {
                deltaValue = functionEffectBuffer.spiritualHealth;
            }
            else if (TextPrefabs_Now == TextPrefabs_W)
            {
                deltaValue = functionEffectBuffer.workAbility;
            }
            else if (TextPrefabs_Now == TextPrefabs_K)
            {
                deltaValue = functionEffectBuffer.KPI;
            }
            int value = Mathf.RoundToInt(Mathf.Lerp(0, deltaValue, lerpFactor_d));

            string s = null;
            if (value >= 0)
            {
                s = "+" + value.ToString();

            }
            else
            {
                s = value.ToString();
            }
            DeltaAll.GetComponent<TextMeshPro>().text = s;
            // Debug.Log(TextPrefabs.Contains(DeltaAll));
            DeltaAll.transform.localScale = DeltaAllScale.Evaluate(lerpFactor_d) * new Vector3(0.1f, 0.1f, 0.1f);
        }


        if (animationTimer >= GiveTime && animationTimer <= GiveTime + PlayerDataChangeTime)
        {
            isFirstPlayMagic = true;

            if (!isAudioPlayed)
            {
                isAudioPlayed = true;
                AudioManager.Instance.PlayClip("recover0");
                if (DeltaAll == DeltaAlls[0])//P
                {
                    StartCoroutine(LACControl.Instance.SetWeight("P", 100 * (Mathf.Clamp(functionEffectBuffer.physicalHealth + PlayerData.Instance.physicalHealth, 0, PlayerData.Instance.physicalHealthMax) - Mathf.Max(PlayerData.Instance.physicalHealth, 0)) / PlayerData.Instance.physicalHealthMax));
                }
                else if (DeltaAll == DeltaAlls[1])//S
                {
                    StartCoroutine(LACControl.Instance.SetWeight("S", 100 * (Mathf.Clamp(functionEffectBuffer.spiritualHealth + PlayerData.Instance.spiritualHealth, 0, PlayerData.Instance.spiritualHealthMax) - Mathf.Max(PlayerData.Instance.spiritualHealth, 0)) / PlayerData.Instance.spiritualHealthMax));
                }
                else if (DeltaAll == DeltaAlls[3])//K
                {
                    StartCoroutine(LACControl.Instance.SetWeight("K", 100 * (Mathf.Clamp(functionEffectBuffer.KPI + PlayerData.Instance.KPI, 0, KPINeed_EveryMonth) - Mathf.Min(PlayerData.Instance.KPI, KPINeed_EveryMonth)) / KPINeed_EveryMonth));
                }
                else if (DeltaAll == DeltaAlls[2])//W
                {
                    StartCoroutine(LAC_Tree_Control.Instance.SetTree(functionEffectBuffer.workAbility));
                }
            }
            float lerpFactor = Mathf.Clamp01((animationTimer - GiveTime) / PlayerDataChangeTime + 0.1f);

            Vector3 destPos = Vector3.zero;
            if (DeltaAll == DeltaAlls[0])
            {
                destPos = physicalHealthText.transform.position;
            }
            else if (DeltaAll == DeltaAlls[1])
            {
                destPos = spiritualHealthText.transform.position;
            }
            else if (DeltaAll == DeltaAlls[2])
            {
                destPos = workAbilityText.transform.position;
            }
            else if (DeltaAll == DeltaAlls[3])
            {
                destPos = KPIText.transform.position;
            }

            DeltaAll.transform.position = destPos + new Vector3(0, 0.04f, 0) * (1 - lerpFactor);

            //这边不好缩，将就看吧
            if (TextPrefabs_Now == TextPrefabs_P)
            {
                BeginEmit(0);
                if (functionEffectBuffer.physicalHealth != 0)
                {
                    // if (functionEffectBuffer.physicalHealth > 0)
                    // {
                    //     physicalHealthText.color = Color.green;
                    // }
                    // else
                    // {
                    //     physicalHealthText.color = Color.yellow;
                    // }

                    physicalHealthText.text = "体力：" + ((int)Mathf.Lerp(playerData_Last.physicalHealth, Mathf.Min(playerData_Last.physicalHealth + functionEffectBuffer.physicalHealth, PlayerData.Instance.physicalHealthMax), lerpFactor)).ToString();
                    physicalHealthText.text += "/" + PlayerData.Instance.physicalHealthMax.ToString();
                }
            }

            if (TextPrefabs_Now == TextPrefabs_S)
            {
                BeginEmit(1);
                if (functionEffectBuffer.spiritualHealth != 0)
                {
                    // if (functionEffectBuffer.spiritualHealth > 0)
                    // {
                    //     spiritualHealthText.color = Color.green;
                    // }
                    // else
                    // {
                    //     spiritualHealthText.color = Color.yellow;
                    // }
                    spiritualHealthText.text = "精力：" + ((int)Mathf.Lerp(playerData_Last.spiritualHealth, Mathf.Min(playerData_Last.spiritualHealth + functionEffectBuffer.spiritualHealth, PlayerData.Instance.spiritualHealthMax), lerpFactor)).ToString();
                    spiritualHealthText.text += "/" + PlayerData.Instance.spiritualHealthMax.ToString();
                }
            }

            if (TextPrefabs_Now == TextPrefabs_W)
            {
                BeginEmit(2);
                if (functionEffectBuffer.workAbility != 0)
                {
                    // if (functionEffectBuffer.workAbility > 0)
                    // {
                    //     workAbilityText.color = Color.green;
                    // }
                    // else
                    // {
                    //     workAbilityText.color = Color.yellow;
                    // }
                    workAbilityText.text = "能力：" + ((int)Mathf.Lerp(playerData_Last.workAbility, playerData_Last.workAbility + functionEffectBuffer.workAbility, lerpFactor)).ToString();
                }
            }
            if (TextPrefabs_Now == TextPrefabs_K)
            {
                BeginEmit(3);
                if (functionEffectBuffer.KPI != 0)
                {
                    // if (functionEffectBuffer.KPI > 0)
                    // {
                    //     KPIText.color = Color.green;
                    // }
                    // else
                    // {
                    //     KPIText.color = Color.yellow;
                    // }
                    KPIText.text = "KPI：" + ((int)Mathf.Lerp(playerData_Last.KPI, playerData_Last.KPI + functionEffectBuffer.KPI, lerpFactor)).ToString() + "/" + KPINeed_EveryMonth.ToString();
                }
            }
        }
        if (animationTimer > GiveTime + PlayerDataChangeTime)
        {
            if (TextPrefabs_Now == TextPrefabs_K)
            {
                DeltaAlls[3].SetActive(false);
                foreach (var emit in emits)
                {
                    emit.SetActive(false);
                }

                foreach (var TextPrefab in TextPrefabs)
                {
                    DestroyImmediate(TextPrefab);
                }
                TextPrefabs.Clear();
                Cards_TextPrefabList_Dic.Clear();

                // physicalHealthText.color = Color.red;
                // spiritualHealthText.color = Color.blue;
                // workAbilityText.color = Color.green;
                // KPIText.color = new Color(1, 150 / 255f, 0);

                FieldManager.Instance.OverFillP = Mathf.Max(0, PlayerData.Instance.physicalHealth + functionEffectBuffer.physicalHealth - PlayerData.Instance.physicalHealthMax);
                FieldManager.Instance.OverFillS = Mathf.Max(0, PlayerData.Instance.spiritualHealth + functionEffectBuffer.spiritualHealth - PlayerData.Instance.spiritualHealthMax);
                PlayerData.Instance.physicalHealth = Mathf.Min(PlayerData.Instance.physicalHealth + functionEffectBuffer.physicalHealth, PlayerData.Instance.physicalHealthMax);
                PlayerData.Instance.spiritualHealth = Mathf.Min(PlayerData.Instance.spiritualHealth + functionEffectBuffer.spiritualHealth, PlayerData.Instance.spiritualHealthMax);
                PlayerData.Instance.workAbility += functionEffectBuffer.workAbility;
                PlayerData.Instance.KPI += functionEffectBuffer.KPI;
                // PlayerDataLastTransfer();//把PlayerData中的值赋予给playerData_Last,但是是错误的，其实应该从点击开始算
                PlayerData.Instance.UpdateDataToUI_Weekday();
                animationCounter = 0;
                animationTimer = 0;

                PlayerData.Instance.PSWarning();
                PlayerData.Instance.CalculateFavorAll();

                phase = Phase.TripletReward;
                isFirstGive = true;
                LACControl.Instance.SetFace(true);

                //在这里检测KPI是否达标，并发射眼球
                if (!isKPIThisMonthOK && PlayerData.Instance.KPI >= KPINeed_EveryMonth)
                {
                    isKPIThisMonthOK = true;
                    PlayerData.Instance.KPILifeChange(+1);
                    //播放眼球弹射动画
                    SkullControl.Instance.EmitEye_Ani();
                    SkullControl.Instance.O_C_Button(true);
                }

                if (TeachManager.Instance.isFirstAssignOver)
                {
                    TeachManager.Instance.isFirstAssignOver = false;
                    TeachManager.Instance.TeachEventTrigger("生命介绍");
                }
            }
            else
            {
                if (TextPrefabs_Now == TextPrefabs_P)
                {
                    DeltaAlls[0].SetActive(false);
                    DeltaAll = DeltaAlls[1];
                    TextPrefabs_Now = TextPrefabs_S;
                }
                else if (TextPrefabs_Now == TextPrefabs_S)
                {
                    DeltaAlls[1].SetActive(false);
                    DeltaAll = DeltaAlls[2];
                    TextPrefabs_Now = TextPrefabs_W;
                }
                else if (TextPrefabs_Now == TextPrefabs_W)
                {
                    DeltaAlls[2].SetActive(false);
                    DeltaAll = DeltaAlls[3];
                    TextPrefabs_Now = TextPrefabs_K;
                }
                animationTimer = 0;
                isAudioPlayed = false;
            }


        }

    }
    public AnimationCurve DeltaAllScale;
    // IEnumerator DeltaAllMoveAndDie(string s, Vector3 destPos, float timer = 0)//左边的delta值向下移动，最终消失
    // {
    //     // Vector3 originPos = DeltaAll.transform.position;
    //     // bool isLAC_Ani_Played = false;
    //     while (true)
    //     {
    //         if (timer > (GiveTime + PlayerDataChangeTime - 0.01f))
    //         {
    //             Destroy(DeltaAll);
    //             DeltaAll = null;
    //             yield break;
    //         }

    //         timer += Time.deltaTime;
    //         if (timer <= GiveTime)
    //         {
    //             DeltaAll.transform.localScale = DeltaAllScale.Evaluate(timer / GiveTime) * new Vector3(0.1f, 0.1f, 0.1f);
    //         }
    //         if (timer > GiveTime)
    //         {
    //             //     (!isLAC_Ani_Played)
    //             //   {
    //             //         isLAC_Ani_Played = true;
    //             //         LACControl.Instance.SetWeight(s,);
    //             //     }



    //             // DeltaAll.transform.position -= new Vector3(0, Time.deltaTime * 0.035f, 0);
    //             // DeltaAll.transform.position = Vector3.Lerp(originPos, destPos, factor);
    //         }
    //         yield return null;
    //     }
    // }
    bool isFirstAssign = true;
    void Assign()
    {
        if (isFirstAssign)
        {
            isFirstAssign = false;
            CameraManager.Instance.SetVirtualCam("ChessCam", 0.3f);
        }



        animationTimer += Time.deltaTime;
        if (animationCounter == 21)//第二十二次，也就是全部生成完毕，开始移动和修改PlayerData的数据
        {


            LastAssign();
            return;       //阻止后面的进程
        }

        if (isFirstGive)//第一次的时候启动
        {
            isFirstGive = false;
            //载入所有当前队列的卡牌
            cardPersonalGamePrefabs_CurrentQueue.Clear();
            foreach (var cardPersonalGamePrefab in cardPersonalGamePrefabs)
            {
                Card card = cardPersonalGamePrefab.GetComponent<CardDisplayPersonalGame>().card;
                if (animationCounter == 20)//亡语计算队列
                {
                    if (card.isAlive == false)
                    {
                        cardPersonalGamePrefabs_CurrentQueue.Add(cardPersonalGamePrefab);
                    }
                }
                else//非亡语队列
                {
                    if (card.executeQueue == animationCounter)
                    {
                        cardPersonalGamePrefabs_CurrentQueue.Add(cardPersonalGamePrefab);
                    }

                    if (animationCounter == 10)//此时连轴卡牌对场上卡牌的加减已经结束，可以计算场上卡牌的信息了
                    {
                        FieldManager.Instance.NoneAll = 20 - cardList.Count;//计算空格数

                        List<Type> types = new List<Type>();//计算流派数
                        Dictionary<int, int> id_amount_Dic = new Dictionary<int, int>();//计算有没有重复卡
                        foreach (var card2 in cardList)
                        {
                            if (!types.Contains(card2.type))
                            {
                                types.Add(card2.type);
                            }
                            if (!id_amount_Dic.ContainsKey(card2.id))
                            {
                                id_amount_Dic.Add(card2.id, 1);
                            }
                            else
                            {
                                id_amount_Dic[card2.id]++;
                            }

                        }

                        int u = 1;
                        List<int> valueList = new List<int>(id_amount_Dic.Values);
                        for (var i = 0; i < valueList.Count; i++)
                        {
                            if (valueList[i] > 1)
                            {
                                u = 0;
                            }

                        }

                        FieldManager.Instance.TypeCount = types.Count;
                        FieldManager.Instance.universe = u;
                    }
                }

            }

            //如果当前队列没有卡牌则直接下一队列
            if (cardPersonalGamePrefabs_CurrentQueue.Count == 0)
            {
                animationTimer = 0;
                animationCounter++;
                isFirstGive = true;
                return;
            }
            else//当前队列有卡牌
            {

            }
        }



        if (animationCounter != 8)//除了连轴外的所有卡牌
        {
            if (isFirstGive2)
            {
                isFirstGive2 = false;
                Vector3 localPosAdd = new Vector3(0, 0.3f * 0.0465f, 0);

                Card card = cardPersonalGamePrefabs_CurrentQueue[0].GetComponent<CardDisplayPersonalGame>().card;
                GameObject CardGameObject = cardPersonalGamePrefabs_CurrentQueue[0];
                cardPersonalGamePrefabs_CurrentQueue.RemoveAt(0);

                List<GameObject> textPrefabList_Last = new List<GameObject>();



                if (animationCounter == 20)
                {
                    card.DWCardEffect();
                }
                else
                {
                    card.CardEffect();//Calculate!!!!!!!!!!!!!!!!!
                }

                // CardGameObject.GetComponent<SimpleEffect>().CloseConnectBuff();
                List<GameObject> textPrefabList = new List<GameObject>();// TextPrefabs是所有的TextPrefab的集合，而textPrefabList是单个CardGameObject上的TextPrefab的集合

                if (Cards_TextPrefabList_Dic.ContainsKey(card) && animationCounter == 20)//20计算亡语
                {
                    textPrefabList_Last = Cards_TextPrefabList_Dic[card];
                    localPosAdd = new Vector3(0, 0.5f * 0.0465f, 0) * textPrefabList_Last.Count + new Vector3(0, 0.3f * 0.0465f, 0);
                    // Debug.Log(textPrefabList_Last.Count);
                    textPrefabList.AddRange(textPrefabList_Last);
                }

                if (card.functionEffect.physicalHealth != 0)
                {
                    CreatText10("P", ref localPosAdd, card, CardGameObject, ref card.functionEffect.physicalHealth, textPrefabList);
                }
                else if (card.functionEffect.physicalHealth == 0 && animationCounter == 20)//当亡语的P和之前的P抵消时，之前的字体需要被抹除
                {
                    RemoveTextPrefab("P", textPrefabList);
                }

                if (card.functionEffect.spiritualHealth != 0)
                {
                    CreatText10("S", ref localPosAdd, card, CardGameObject, ref card.functionEffect.spiritualHealth, textPrefabList);
                }
                else if (card.functionEffect.spiritualHealth == 0 && animationCounter == 20)
                {
                    RemoveTextPrefab("S", textPrefabList);
                }

                if (card.functionEffect.workAbility != 0)
                {
                    CreatText10("W", ref localPosAdd, card, CardGameObject, ref card.functionEffect.workAbility, textPrefabList);
                }
                else if (card.functionEffect.workAbility == 0 && animationCounter == 20)
                {
                    RemoveTextPrefab("W", textPrefabList);
                }


                if (card.functionEffect.KPI != 0)
                {
                    CreatText10("K", ref localPosAdd, card, CardGameObject, ref card.functionEffect.KPI, textPrefabList);
                }
                else if (card.functionEffect.KPI == 0 && animationCounter == 20)
                {
                    RemoveTextPrefab("K", textPrefabList);
                }

                if (Cards_TextPrefabList_Dic.ContainsKey(card) && animationCounter == 20)//后一个条件有点多余，但是保险起见
                {
                    textPrefabList_Last = textPrefabList;//覆盖
                }
                else
                {
                    Cards_TextPrefabList_Dic.Add(card, textPrefabList);
                }
                // }


            }

            float time = NormalCard_Time;//其他队列的卡牌都是NormalCard_Time，而12队列的捉弄AI卡牌时间会长一点
            if (animationCounter == 12)
            {
                time *= 2;
            }

            if (animationTimer > time)
            {
                animationTimer = 0;
                isFirstGive2 = true;//下一张进行
                if (cardPersonalGamePrefabs_CurrentQueue.Count == 0)//如果没有下一张
                {
                    animationCounter++;//下一队列
                    isFirstGive = true;
                }
            }
        }
        else  //queue8连轴
        {
            GameObject cardPG = cardPersonalGamePrefabs_CurrentQueue[0];
            Card card = cardPG.GetComponent<CardDisplayPersonalGame>().card;

            if (isFirstGive2)
            {
                isFirstGive2 = false;
                //Calculate!!!!!!!!!!!!!!!!!

                if (card.executeQueue == 10)
                {
                    cardPG.GetComponent<SimpleEffect>().OpenConnectBuff();
                }
                else//连轴
                {
                    Vector3 localPosAdd = new Vector3(0, 0.3f, 0);
                    if (animationCounter == 20)
                    {
                        card.DWCardEffect();
                    }
                    else
                    {
                        card.CardEffect();
                    }

                    //直接加到缓冲区中去
                    functionEffectBuffer.physicalHealth += card.functionEffect.physicalHealth;
                    functionEffectBuffer.spiritualHealth += card.functionEffect.spiritualHealth;
                    functionEffectBuffer.workAbility += card.functionEffect.workAbility;
                    functionEffectBuffer.KPI += card.functionEffect.KPI;

                    if (card.functionEffectEx.physicalHealthAdd != 0)
                    {
                        CreatText8("P", ref localPosAdd, card, cardPG, ref card.functionEffectEx.physicalHealthAdd, "Add");
                    }
                    if (card.functionEffectEx.spiritualHealthAdd != 0)
                    {
                        CreatText8("S", ref localPosAdd, card, cardPG, ref card.functionEffectEx.spiritualHealthAdd, "Add");
                    }
                    if (card.functionEffectEx.workAbilityAdd != 0)
                    {
                        CreatText8("W", ref localPosAdd, card, cardPG, ref card.functionEffectEx.workAbilityAdd, "Add");
                    }
                    if (card.functionEffectEx.KPIAdd != 0)
                    {
                        CreatText8("K", ref localPosAdd, card, cardPG, ref card.functionEffectEx.KPIAdd, "Add");
                    }
                    if (card.functionEffectEx.physicalHealthMulti != 1)
                    {
                        CreatText8("P", ref localPosAdd, card, cardPG, ref card.functionEffectEx.physicalHealthMulti, "Multi");
                    }
                    if (card.functionEffectEx.spiritualHealthMulti != 1)
                    {
                        CreatText8("S", ref localPosAdd, card, cardPG, ref card.functionEffectEx.spiritualHealthMulti, "Multi");
                    }
                    if (card.functionEffectEx.workAbilityMulti != 1)
                    {
                        CreatText8("W", ref localPosAdd, card, cardPG, ref card.functionEffectEx.workAbilityMulti, "Multi");
                    }
                    if (card.functionEffectEx.KPIMulti != 1)
                    {
                        CreatText8("K", ref localPosAdd, card, cardPG, ref card.functionEffectEx.KPIMulti, "Multi");
                    }
                }

            }
            if (animationTimer > NormalCard_Time)//如果大于连轴时间
            {
                animationTimer = 0;
                isFirstGive2 = true;//下一张进行

                cardPersonalGamePrefabs_CurrentQueue.RemoveAt(0);
                if (card.executeQueue == 10)//如果卡牌不再具有连轴能力，那就去找下一个槽位的连轴卡牌
                {

                }
                else if ((card.executeQueue == 8))//连轴  b/gf
                {
                    //彻底摧毁当前卡牌
                    DestroyCardConnect(cardPG);

                    List<Card> playerCards_Times_Less11 = new List<Card>();//卡牌必须要时间对，以及为8或者10
                    List<Card> playerCards_Times = new List<Card>();
                    if (card.times.Contains("晚") && card.times.ToCharArray().Length == 1)
                    {
                        playerCards_Times = playerCards_Night;
                    }
                    else if (card.times.Contains("中") && card.times.ToCharArray().Length == 1)//中,如果中午没有事情，那么会用Others的行为来填充
                    {
                        playerCards_Times = playerCards_Noon;
                    }
                    else
                    {
                        playerCards_Times = playerCards_Others;
                    }
                    foreach (var playerCard in playerCards_Times)
                    {
                        if (playerCard.executeQueue == 8 || playerCard.executeQueue == 10)
                        {
                            playerCards_Times_Less11.Add(playerCard);
                        }
                    }

                    //根据时间查找并生成新的卡牌（队列必定小于11）,不一定能找到
                    if (playerCards_Times_Less11.Count == 0)
                    {
                        GameObject TextPrefab = Instantiate(textPrefab, roots[card.posNum - 1]);
                        TextPrefab.transform.localScale *= 0.5f;
                        TextPrefab.transform.localPosition = new Vector3(0, 0.3f * 0.0465f, 0);
                        Destroy(TextPrefab, NormalCard_Time);//在ConnectingTime结束后销毁对象
                        TextMeshPro Text = TextPrefab.GetComponent<TextMeshPro>();
                        TextPrefab.GetComponent<MeshRenderer>().material = FontMats[0];
                        Text.color = Color.white;
                        Text.text = "卡牌不足";
                        // StopAllCoroutines();

                    }
                    else
                    {
                        GameObject cardPG2 = CreatCardConnect(ref playerCards_Times_Less11, ref playerCards_Times, card.posNum);
                        Card card2 = cardPG2.GetComponent<CardDisplayPersonalGame>().card;
                        card2.functionEffectEx = card.functionEffectEx;//传承functionEffectEx
                        cardPersonalGamePrefabs_CurrentQueue.Insert(0, cardPG2);
                    }
                }
                if (cardPersonalGamePrefabs_CurrentQueue.Count == 0)//如果这个队列中的卡全部清零了（原卡组一张10都没有），或者全部都是10
                {
                    animationCounter++;//下一队列
                    isFirstGive = true;
                }

            }
        }

    }
    public void SkipTime_Create()
    {
        animationTimer = CreateTime - 0.01f;
    }
    public void SkipTime_Assign()
    {
        animationTimer = NormalCard_Time - 0.01f;
        if (animationCounter == 12)
        {
            animationTimer = 2 * NormalCard_Time - 0.01f;
        }
    }


    void CreatCard(ref List<Card> playerCards_Times)//根据cardList（如playerCards_Noon）来创建卡牌
    {
        AudioManager.Instance.PlayClip("create0");
        int r = Random.Range(0, playerCards_Times.Count);
        GameObject cardPG = Instantiate(cardPersonalGamePrefab, roots[animationCounter - 1]);

        // CameraManager.Instance.SetVirtualCamFollow(roots[animationCounter - 1]);

        cardPG.GetComponent<CardDisplayPersonalGame>().card = playerCards_Times[r];
        cardPG.transform.localPosition = Vector3.zero;
        playerCards_Times.RemoveAt(r);
        cardPersonalGamePrefabs.Add(cardPG);
        Card card = cardPG.GetComponent<CardDisplayPersonalGame>().card;
        if (card.id < 10000)
        {
            if (card.finalTitle.Contains("摸鱼"))
            {
                FieldManager.Instance.FishAll++;
            }
            if (card.qualityLevel == 5)
            {
                FieldManager.Instance.Lv5C++;
            }
            TripletCards[card.id]++;//把卡牌添加进入计算三连的数组中
        }
        else if (card.id >= 10000)
        {
            FieldManager.Instance.NoColorAll++;
            if (card.id == 10000)//精神疲劳卡牌的总数计算
            {
                FieldManager.Instance.TiredAll++;
            }
        }
        card.posNum = animationCounter;//posNum在此处赋值，在OnClickStart的UpdateMechanism处归0
        cardList.Add(card);
        cards_cardPersonalGamePrefabs_Dic.Add(card, cardPG);
        // StopAllCoroutines();
        // StartCoroutine(Bounce(cardPG));
        StartCoroutine(cardPG.GetComponent<SimpleEffect>().Fly());

    }

    GameObject CreatCardConnect(ref List<Card> playerCards_Times_Less11, ref List<Card> playerCards_Times, int posNum)//根据时间和队列限制来创建卡牌
    {
        AudioManager.Instance.PlayClip("create0");
        int r = Random.Range(0, playerCards_Times_Less11.Count);
        GameObject cardPG = Instantiate(cardPersonalGamePrefab, roots[posNum - 1]);//在posNum-1的地方生成
        cardPG.GetComponent<CardDisplayPersonalGame>().card = playerCards_Times_Less11[r];
        cardPG.transform.localPosition = Vector3.zero;
        playerCards_Times.Remove(playerCards_Times_Less11[r]);
        cardPersonalGamePrefabs.Add(cardPG);
        Card card = cardPG.GetComponent<CardDisplayPersonalGame>().card;
        if (card.id < 10000)
        {
            if (card.finalTitle.Contains("摸鱼"))
            {
                FieldManager.Instance.FishAll++;
            }
            if (card.qualityLevel == 5)
            {
                FieldManager.Instance.Lv5C++;
            }
            TripletCards[card.id]++;
        }
        else if (card.id >= 10000)
        {
            FieldManager.Instance.NoColorAll++;
            if (card.id == 10000)//精神疲劳卡牌的总数计算
            {
                FieldManager.Instance.TiredAll++;
            }
        }

        card.posNum = posNum;
        cardList.Add(card);//在posNum-1的地方插入
        cards_cardPersonalGamePrefabs_Dic.Add(card, cardPG);//这个地方用add没问题
                                                            // StopAllCoroutines();
        StartCoroutine(cardPG.GetComponent<SimpleEffect>().Fly());
        // StartCoroutine(Bounce(cardPG));
        return cardPG;
    }
    void DestroyCardConnect(GameObject cardPG)//根据cardPG来摧毁卡牌，让其彻底人间蒸发
    {
        cardPersonalGamePrefabs.Remove(cardPG);//不知道会不会出问题,如果有问题，那就removeat
        Card card = cardPG.GetComponent<CardDisplayPersonalGame>().card;
        cardList.Remove(card);
        cards_cardPersonalGamePrefabs_Dic.Remove(card);//按照键抹除
        DestroyImmediate(cardPG);
    }

    //亡语的数值正负持平时（之前P-10，亡语P+10，最终=0），消除原来的字体gameobject
    void RemoveTextPrefab(string func, List<GameObject> textPrefabList)
    {
        for (var i = 0; i < textPrefabList.Count; i++)
        {
            GameObject tP = textPrefabList[i];
            TextMeshPro t2 = tP.GetComponent<TextMeshPro>();

            if (
               (func == "P" && t2.color == new Color(1, 0, 0, 1))
                || (func == "S" && t2.color == Color.blue)
                || (func == "W" && t2.color == Color.green)
                || (func == "K" && t2.color == new Color(1, 150f / 255f, 0, 1))
                )
            {
                if (true)//这个条件是多余的，但是为了健壮性//textPrefabList.Contains(textPrefabList[i]) && TextPrefabs.Contains(textPrefabList[i])
                {
                    textPrefabList.Remove(tP);
                    TextPrefabs.Remove(tP);
                    Destroy(tP);
                }
            }
        }
    }

    bool isPlayCreatText = false;
    IEnumerator PlayClipWithInterval(float timer = 0f)//0.5*createTime的CD
    {
        isPlayCreatText = true;
        AudioManager.Instance.PlayClip("create1");
        while (true)
        {
            if (timer > 0.5 * CreateTime)
            {
                isPlayCreatText = false;
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

    }

    //队列10的创建Text
    void CreatText10(string playerData, ref Vector3 localPosAdd, Card card, GameObject CardGameObject, ref int value, List<GameObject> textPrefabList)
    {
        if (!isPlayCreatText)
        {
            StartCoroutine(PlayClipWithInterval());
        }

        GameObject TextPrefab = null;
        bool isExist = false;
        if (animationCounter == 20)//如果是亡语，则尝试找之前的那个TextPrefab
        {
            foreach (var tP in textPrefabList)
            {
                TextMeshPro t = tP.GetComponent<TextMeshPro>();
                if (
                   (playerData == "P" && t.color == new Color(1, 0, 0, 1))
                    || (playerData == "S" && t.color == Color.blue)
                    || (playerData == "W" && t.color == Color.green)
                    || (playerData == "K" && t.color == new Color(1, 150f / 255f, 0, 1))
                    )
                {
                    TextPrefab = tP;
                    isExist = true;
                }
            }
        }


        if (TextPrefab == null)//如果亡语队列，且之前的TextPrefab不存在，那就新建//如果普通队列，就新建
        {
            TextPrefab = Instantiate(textPrefab, CardGameObject.transform);
            localPosAdd += new Vector3(0, 0.5f * 0.0465f, 0);
            TextPrefab.transform.localPosition += localPosAdd;
        }



        TextMeshPro Text = TextPrefab.GetComponent<TextMeshPro>();
        //正负修正
        if (value >= 0)
        {
            Text.text = "+" + value.ToString();
        }
        else
        {
            Text.text = value.ToString();
        }

        switch (playerData)
        {
            case "P":
                TextPrefab.GetComponent<MeshRenderer>().material = FontMats[0];
                Text.color = new Color(1, 0, 0, 1);
                TextPrefab.transform.Find("Font_FX_P").gameObject.SetActive(true);
                // TextPrefab.transform.Find("Font_FX_Create_P").gameObject.SetActive(true);
                break;
            case "S":
                TextPrefab.GetComponent<MeshRenderer>().material = FontMats[1];//选用不同的材质是因为需要调节hdr
                Text.color = Color.blue;
                TextPrefab.transform.Find("Font_FX_S").gameObject.SetActive(true);
                // TextPrefab.transform.Find("Font_FX_Create_S").gameObject.SetActive(true);
                break;
            case "W":
                TextPrefab.GetComponent<MeshRenderer>().material = FontMats[2];//选用不同的材质是因为需要调节hdr
                Text.color = Color.green;
                TextPrefab.transform.Find("Font_FX_W").gameObject.SetActive(true);
                // TextPrefab.transform.Find("Font_FX_Create_W").gameObject.SetActive(true);
                break;
            case "K":
                TextPrefab.GetComponent<MeshRenderer>().material = FontMats[3];//选用不同的材质是因为需要调节hdr
                Text.color = new Color(1, 150f / 255f, 0, 1);
                TextPrefab.transform.Find("Font_FX_K").gameObject.SetActive(true);
                // TextPrefab.transform.Find("Font_FX_Create_K").gameObject.SetActive(true);
                break;
            default: break;
        }

        if (isExist == false)//如果之前不存在，则存
        {
            TextPrefabs.Add(TextPrefab);//TextPrefabs是所有的TextPrefab的集合
            textPrefabList.Add(TextPrefab);//而textPrefabList是单个CardGameObject上的TextPrefab的集合
        }
    }
    void CreatText8(string playerData, ref Vector3 localPosAdd, Card card, GameObject CardGameObject, ref int value, string type)//type是加还是乘
    {
        if (!isPlayCreatText)
        {
            StartCoroutine(PlayClipWithInterval());
        }
        GameObject TextPrefab = Instantiate(textPrefab, CardGameObject.transform);
        Destroy(TextPrefab, NormalCard_Time);//在ConnectingTime结束后销毁对象
        localPosAdd += new Vector3(0, 0.5f * 0.0465f, 0);
        TextPrefab.transform.localPosition += localPosAdd;
        TextMeshPro Text = TextPrefab.GetComponent<TextMeshPro>();
        Text.text = "Next";
        if (type == "Multi")
        {
            Text.text += "x" + value.ToString();
        }
        else if (type == "Add")
        {
            //正负修正
            if (value >= 0)
            {
                Text.text += "+" + value.ToString();
            }
            else
            {
                Text.text += value.ToString();
            }
        }

        switch (playerData)
        {
            case "P":
                TextPrefab.GetComponent<MeshRenderer>().material = FontMats[0];
                Text.color = new Color(1, 0, 0, 1);
                break;
            case "S":
                TextPrefab.GetComponent<MeshRenderer>().material = FontMats[1];//选用不同的材质是因为需要调节hdr
                Text.color = Color.blue;
                break;
            case "W":
                TextPrefab.GetComponent<MeshRenderer>().material = FontMats[2];//选用不同的材质是因为需要调节hdr
                Text.color = Color.green;
                break;
            case "K":
                TextPrefab.GetComponent<MeshRenderer>().material = FontMats[3];//选用不同的材质是因为需要调节hdr
                Text.color = new Color(1, 150f / 255f, 0, 1);
                break;
            default: break;
        }
    }

    List<GameObject> FindAllTripletCardPGinScene(int id)//找到场上所有某id的卡牌
    {
        List<GameObject> cardPGs = new List<GameObject>();
        foreach (var card in cardList)
        {
            if (card.id == id)
            {
                cardPGs.Add(cards_cardPersonalGamePrefabs_Dic[card]);
            }
        }
        return cardPGs;

    }
    float TripletFXTime = 2f;
    IEnumerator TripletFX(int times, List<GameObject> TripletCardPGs)
    {
        float timer = 0f;
        bool isTop = false;//三个物体是否已经到达最高处
        List<GameObject> CardPGs_3 = new List<GameObject>();

        List<Vector3> CardPGs_3_Poss = new List<Vector3>();//每个棋子的原位置
        if (TripletCardPGs.Count >= 3)
        {

            CardPGs_3.Add(TripletCardPGs[0]);
            CardPGs_3_Poss.Add(TripletCardPGs[0].transform.position);
            CardPGs_3.Add(TripletCardPGs[1]);
            CardPGs_3_Poss.Add(TripletCardPGs[1].transform.position);
            CardPGs_3.Add(TripletCardPGs[2]);
            CardPGs_3_Poss.Add(TripletCardPGs[2].transform.position);

            TripletCardPGs.RemoveAt(0);
            TripletCardPGs.RemoveAt(0);
            TripletCardPGs.RemoveAt(0);

        }

        while (true)
        {
            if (timer > TripletFXTime)
            {
                if (CardPGs_3.Count == 3)
                {
                    CardPGs_3[0].transform.position = CardPGs_3_Poss[0];
                }
                timer = 0f;
                CardPGs_3.Clear();
                CardPGs_3_Poss.Clear();
                times--;
                isTop = false;
                if (times > 0)
                {
                    StartCoroutine(TripletFX(times, TripletCardPGs));
                }
                else
                {
                    isTripleyFXPlayed = true;
                    yield break;
                }

            }
            timer += Time.deltaTime;
            float factor = ac_Triplet.Evaluate(timer / TripletFXTime);
            if (CardPGs_3.Count == 3)
            {
                if (timer / TripletFXTime < 0.25f)
                {
                    for (int i = 0; i < CardPGs_3.Count; i++)
                    {
                        CardPGs_3[i].transform.position = Vector3.Lerp(CardPGs_3_Poss[i], point1_3.position, factor);
                    }
                }
                else if (timer / TripletFXTime < 0.75f)
                {
                    if (!isTop)//三花聚顶第一帧
                    {
                        isTop = true;
                        CardPGs_3[1].SetActive(false);
                        CardPGs_3[2].SetActive(false);
                        ps_Triplet.Stop();
                        ps_Triplet.Play();
                        CardPGs_3[0].GetComponent<SimpleEffect>().PlayTripletBuff();
                    }
                }
                else
                {
                    CardPGs_3[0].transform.position = Vector3.Lerp(CardPGs_3_Poss[0], point1_3.position, factor);
                }
            }

            yield return null;
        }
    }

    //三连特效启动函数
    void TripletFXAll(List<GameObject> TripletCardPGs)//TripletCardPGs必为3的倍数
    {
        int times = TripletCardPGs.Count / 3;
        StartCoroutine(TripletFX(times, TripletCardPGs));
    }
    public void TripletReduce(int id, ref int cardAmount)
    {
        int tripletNum = cardAmount / 3;

        if (tripletNum > 0)
        {
            List<GameObject> TripletCardPGs = FindAllTripletCardPGinScene(id);//这一id的所有会进行三连的卡牌gameobject
            TripletFXAll(TripletCardPGs);
            for (int i = 0; i < tripletNum; i++)
            {
                cardAmount -= 2;
                chooseTimes += 1;
                Card card = CardStore.Instance.SearchCard(id);

                //生成三连提示
                GameObject tripletSign = Instantiate(SignPrefab, TripletGroup.transform);
                string s = "“" + card.finalTitle + "”" + "获得了三连";
                tripletSign.GetComponent<TextMeshProUGUI>().text = s;
                SignAll_Update(s);
                if (TeachManager.Instance.isFirstTriplet)
                {
                    TeachManager.Instance.TeachEventTrigger_Delay("场上三连介绍", 3f);
                    TeachManager.Instance.isFirstTriplet = false;
                }


                Destroy(tripletSign, SignStayTime);
                ReduceCards(id, 2);//playerData中减少卡牌,注意三连是减少两张 //六张同牌可以两次三连,总共减少4张牌，升级2次
                PromoteCards(id, 1);//升级所有PlayData以及CardStore中的卡

            }
        }
    }
    void ReduceCards(int id, int num)
    {
        for (int i = 0; i < PlayerData.Instance.playerCards.Count; i++)
        {
            if (PlayerData.Instance.playerCards[i].id == id)
            {
                PlayerData.Instance.playerCards.Remove(PlayerData.Instance.playerCards[i]);
                num--;
            }
            if (num == 0)
            {
                return;
            }
        }

    }
    public void PromoteCards(int id, int value)//升级所有PlayData以及CardStore中的卡
    {
        foreach (var card in CardStore.Instance.cards)
        {
            if (card.id == id)
            {
                card.addLevel += value;
                card.AutoTitle();
                card.AutoDescription();
            }
        }
        foreach (var card in PlayerData.Instance.playerCards)
        {
            if (card.id == id)
            {
                card.addLevel += value;
                card.AutoTitle();
                card.AutoDescription();
                // card.isNew=true;
            }
        }
    }


    bool isTripleyFXPlayed = false;//三连特效是否已经播放
    //三连奖励
    void TripletReward()
    {
        if (isFirstTriplet == true)
        {
            //开始播放动画，第一帧算出结果，并且开启协程，协程结束后再开选择面板
            //检测三连
            for (int i = 0; i < TripletCards.Length; i++)
            {
                TripletReduce(i, ref TripletCards[i]);//检测三连并减少卡牌,增强卡牌
            }

            PlayerData.Instance.SortCards();

            isFirstTriplet = false;
            StartButton.GetComponent<ClickButton>().Start_2_Continue();
            TeachManager.Instance.SetGuide(StartButton, true);
            return;
        }
        if (isTripleyFXPlayed)
        {
            Open1_3_Triplet();
        }


    }

    void Open1_3_Triplet()//打开三连的三选一界面
    {
        if (isChoose == false && chooseTimes > 0)
        {
            FieldManager.Instance.isTripletReward = true;
            // Pannel3_1.SetActive(true);
            ThreeChooseOneManager.Instance.Open();
            isChoose = true;
        }

        //选择完卡牌后，isChoose会变成false，chooseTimes-1，具体代码在ThreeChooseOneCard类中

        else if (isChoose == false && chooseTimes == 0)//如果选完了并且没有剩余次数了，就进入下一环节
        {
            FieldManager.Instance.isTripletReward = false;
        }
    }

    void WeekendMeeting()
    {
        if (isFirstWeekendMeeting)//如果是第一次达到这个界面
        {

            WeekendMeetingAll.Instance.Open();
            WeekendMeetingAll.Instance.weekendPhase = weekendPhase.LeaderTime;

            if (TeachManager.Instance.isFirstWeekendMeeting)
            {
                TeachManager.Instance.TeachEventTrigger_Delay("周末选牌介绍", 1f);
                TeachManager.Instance.isFirstWeekendMeeting = false;
            }
            // Rotate_RotatePannel();
            // HolidayPannel.SetActive(true);
            isFirstWeekendMeeting = false;
            if (week % 4 == 0)
            {
                CalculatePlayerStrength();//计算玩家实力
            }
            AIMechanism.Instance.AI_Exchange_WeekendMeeting();//计算AI数据和所有对象的排名,并给AI改UI
            PlayerData.Instance.UpdateDataToUI_Weekday();//更新数据到UI
                                                         // AIMechanism.Instance.CalculateKPITop();//计算这周最高的KPI数值


        }

        if (!isFirstChooseCardButton)//如果按下了选择卡牌按钮
        {
            if (isChoose == false && chooseTimes > 0)
            {
                // Pannel3_1.SetActive(true);
                ThreeChooseOneManager.Instance.Open();
                isChoose = true;
            }
        }
    }
    public void OnClickNextWeekButton()//进入下一周，管理每月KPI清零，并且每周回血
    {
        //玩家和AI、KPI清零
        if (week % 4 == 0)
        {
            //禁忌书送牌
            if (FieldManager.Instance.isBook_Cthugha)
            {
                Card card = CardStore.Instance.RandomForbidenCard();
                card.isNew = true;
                // InstantiateEffect(this, kidCard, interval * i);
                PlayerData.Instance.playerCards.Add(card);
                PlayerData.Instance.SortCards();
            }

            //发工资了
            int salary = Mathf.Min(PlayerData.Instance.KPI * 10, PlayerData.Instance.postLevel * 10000);
            salary = (salary / 1000) * 1000;//确保单笔最少1000
            StartCoroutine(LACControl.Instance.Butt(salary));
            // Debug.Log(salary + "Salary");
            CameraManager.Instance.SetVirtualCam("LacCam", 0.25f);
            // PlayerData.Instance.ChangeMoney(salary);
            SignAll_Update("月末发薪水，你的财富增加了" + salary.ToString());
            StartCoroutine(LACControl.Instance.SetWeight("K", -100 * (Mathf.Clamp01(PlayerData.Instance.KPI / (float)KPINeed_EveryMonth))));
            PlayerData.Instance.KPI = 0;
            playerData_Last.KPI = 0;
            PlayerData.Instance.KPIText.text = "0";
            isKPIThisMonthOK = false;
            SkullControl.Instance.O_C_Button(false);
            warningText.text = null;

            foreach (var AI_Cha in AIMechanism.Instance.AI_Chas)
            {
                AIData AIData = AI_Cha.AIData;
                if (AIData != null)
                {
                    AIData.KPI = 0;
                    // AIMechanism.Instance.KPITexts[AIData.AIid].text = "0";
                }
            }
        }

        // GameObject sign = Instantiate(SignPrefab, PostUpgradeGroup.transform);
        // sign.GetComponent<TextMeshProUGUI>().text = "体力和精力得到了回复";
        // Destroy(sign, GiveTime * 2);
        string s = "体力和精力得到了回复";
        SignAll_Update(s);

        // foreach (var AIData in AIMechanism.Instance.AI_Chas)
        // {
        //     // AIMechanism.Instance.physicalHealthTexts[AIData.AIid].color = Color.green;
        //     // AIMechanism.Instance.spiritualHealthTexts[AIData.AIid].color = Color.green;
        // }
        // PlayerData.Instance.physicalHealthText.color = Color.green;
        // PlayerData.Instance.spiritualHealthText.color = Color.green;

        Dictionary<int, int> id_physicalHealths_Last = new Dictionary<int, int>();
        Dictionary<int, int> id_spiritualHealths_Last = new Dictionary<int, int>();

        CameraManager.Instance.SetVirtualCam("LacCam", 0.25f);
        AllRecover(id_physicalHealths_Last, id_spiritualHealths_Last);//恢复所有人的体力，并且储存恢复前的体力值到Dic中
        StartCoroutine(Recover(id_physicalHealths_Last, id_spiritualHealths_Last));//播放恢复动画的协程
    }
    void AllRecover(Dictionary<int, int> id_physicalHealths_Last, Dictionary<int, int> id_spiritualHealths_Last)//恢复数据计算
    {

        foreach (var AI_Cha in AIMechanism.Instance.AI_Chas)
        {
            AIData AIData = AI_Cha.AIData;
            if (AIData != null)
            {
                id_physicalHealths_Last.Add(AIData.AIid, AIData.physicalHealth);
                id_spiritualHealths_Last.Add(AIData.AIid, AIData.spiritualHealth);
                AIData.physicalHealth = Mathf.Clamp(AIData.physicalHealth + (int)(recoverRate * AIData.physicalHealthMax), -10000, AIData.physicalHealthMax);
                AIData.spiritualHealth = Mathf.Clamp(AIData.spiritualHealth + (int)(recoverRate * AIData.spiritualHealthMax), -10000, AIData.spiritualHealthMax);
            }
        }

        id_physicalHealths_Last.Add(5, PlayerData.Instance.physicalHealth);
        id_spiritualHealths_Last.Add(5, PlayerData.Instance.spiritualHealth);

        //
        LACControl.Instance.StartCoroutine(LACControl.Instance.SetWeight("P", 100f * (Mathf.Clamp((int)(recoverRate * PlayerData.Instance.physicalHealthMax) + PlayerData.Instance.physicalHealth, 0, PlayerData.Instance.physicalHealthMax) - Mathf.Max(PlayerData.Instance.physicalHealth, 0)) / (float)PlayerData.Instance.physicalHealthMax));
        LACControl.Instance.StartCoroutine(LACControl.Instance.SetWeight("S", 100f * (Mathf.Clamp((int)(recoverRate * PlayerData.Instance.spiritualHealthMax) + PlayerData.Instance.spiritualHealth, 0, PlayerData.Instance.spiritualHealthMax) - Mathf.Max(PlayerData.Instance.spiritualHealth, 0)) / (float)PlayerData.Instance.spiritualHealthMax));


        FieldManager.Instance.OverFillP += Mathf.Max(0, PlayerData.Instance.physicalHealth + (int)(recoverRate * PlayerData.Instance.physicalHealthMax) - PlayerData.Instance.physicalHealthMax);
        FieldManager.Instance.OverFillS += Mathf.Max(0, PlayerData.Instance.spiritualHealth + (int)(recoverRate * PlayerData.Instance.spiritualHealthMax) - PlayerData.Instance.spiritualHealthMax);
        PlayerData.Instance.physicalHealth = Mathf.Clamp(PlayerData.Instance.physicalHealth + (int)(recoverRate * PlayerData.Instance.physicalHealthMax), -10000, PlayerData.Instance.physicalHealthMax);
        PlayerData.Instance.spiritualHealth = Mathf.Clamp(PlayerData.Instance.spiritualHealth + (int)(recoverRate * PlayerData.Instance.spiritualHealthMax), -10000, PlayerData.Instance.spiritualHealthMax);





        PlayerData.Instance.PSWarning();
    }

    IEnumerator Recover(Dictionary<int, int> id_physicalHealths_Last, Dictionary<int, int> id_spiritualHealths_Last)//恢复动画
    {
        AudioManager.Instance.PlayClip("recover0");
        while (true)
        {
            animationTimer += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(animationTimer / PlayerDataChangeTime);
            // foreach (var AI_Cha in AIMechanism.Instance.AI_Chas)
            // {
            //     AIData AIData = AI_Cha.AIData;
            //     if (AIData != null)
            //     {
            //         AIMechanism.Instance.physicalHealthTexts[AIData.AIid].text = "体力：" + ((int)Mathf.Lerp(id_physicalHealths_Last[AIData.AIid], AIData.physicalHealth, lerpFactor)).ToString() + "/" + AIData.physicalHealthMax.ToString();
            //         AIMechanism.Instance.spiritualHealthTexts[AIData.AIid].text = "精力：" + ((int)Mathf.Lerp(id_spiritualHealths_Last[AIData.AIid], AIData.spiritualHealth, lerpFactor)).ToString() + "/" + AIData.spiritualHealth.ToString();
            //     }
            // }
            PlayerData.Instance.physicalHealthText.text = "体力：" + ((int)Mathf.Lerp(id_physicalHealths_Last[5], PlayerData.Instance.physicalHealth, lerpFactor)).ToString() + "/" + PlayerData.Instance.physicalHealthMax.ToString();
            PlayerData.Instance.spiritualHealthText.text = "精力：" + ((int)Mathf.Lerp(id_spiritualHealths_Last[5], PlayerData.Instance.spiritualHealth, lerpFactor)).ToString() + "/" + PlayerData.Instance.spiritualHealthMax.ToString();


            if (animationTimer > PlayerDataChangeTime + 1)//这个1是停留时间
            {
                // foreach (var AIData in AIMechanism.Instance.AIDatas)
                // {
                //     AIMechanism.Instance.physicalHealthTexts[AIData.AIid].color = Color.red;
                //     AIMechanism.Instance.spiritualHealthTexts[AIData.AIid].color = Color.blue;
                // }
                // PlayerData.Instance.physicalHealthText.color =Color.red;
                // PlayerData.Instance.spiritualHealthText.color =Color.blue;
                LACControl.Instance.SetFace();
                animationTimer = 0;
                week++;
                weekText.text = "第" + (((week - 1) / 4) + 1).ToString() + "月第" + ((week - 1) % 4 + 1).ToString() + "周";



                phase = Phase.Start;

                ClickButton cb_Execute = ExecuteButton.GetComponent<ClickButton>();
                cb_Execute.font.text = "选择卡牌";

                ClickButton cb_Pass = PassButton.GetComponent<ClickButton>();
                cb_Pass.font.text = "跳过";
                cb_Pass.ButtonRecover();

                yield break;
            }
            yield return null;
        }
    }

    void PlayerDataLastTransfer()
    {
        playerData_Last.physicalHealth = PlayerData.Instance.physicalHealth;
        playerData_Last.spiritualHealth = PlayerData.Instance.spiritualHealth;
        playerData_Last.workAbility = PlayerData.Instance.workAbility;
        playerData_Last.KPI = PlayerData.Instance.KPI;
        playerData_Last.ranking = PlayerData.Instance.ranking;
    }

    void UpdateMechanism()
    {
        playerCards.Clear();
        playerCards_Night.Clear();
        playerCards_Noon.Clear();
        playerCards_Others.Clear();
        foreach (var card in PlayerData.Instance.playerCards)
        {
            card.functionEffect = default;//所有卡牌的效果归零
            card.functionEffectEx.Initialize();//所有卡牌的效果Ex归零
            card.posNum = 0;//所有卡牌的位置归零
            card.isEffect = false;
            card.AIDatas_Debuff.Clear();
            playerCards.Add(card);
        }
        foreach (var card in playerCards)
        {
            if (card.times.Contains("晚") && card.times.ToCharArray().Length == 1)
            {
                playerCards_Night.Add(card);
            }
            else if (card.times.Contains("中") && card.times.ToCharArray().Length == 1)
            {
                playerCards_Noon.Add(card);
            }
            else
            {
                playerCards_Others.Add(card);
            }
        }
    }

    public void OnClickGiveUpButton()//三选一界面选择放弃的时候触发
    {
        isChoose = false;
        chooseTimes--;
        ThreeChooseOneManager.Instance.Close();
    }


    public void OnClickChooseCardButton()//在点击选择卡牌按钮的时候,获得三次选择次数,isFirstChooseCardButton为false;点击后button将不可用
    {
        chooseTimes += 2;
        isFirstChooseCardButton = false;
        // ChooseButton.SetActive(false);
    }
    public void OnClickPostUpgradeButton()//点击岗位升级按钮
    {
        // TextMeshProUGUI postLevelText = PlayerData.Instance.postLevelText;
        switch (PlayerData.Instance.postLevel)
        {
            case 1:
                if (PlayerData.Instance.workAbility >= postUpgradeNeeds[0])
                {
                    PostUpgrade(2);
                }
                else
                {
                    PostUpgradeFailed(2);
                }
                break;
            case 2:
                if (PlayerData.Instance.workAbility >= postUpgradeNeeds[1])
                {
                    PostUpgrade(3);
                }
                else
                {
                    PostUpgradeFailed(3);
                }
                break;
            case 3:
                if (PlayerData.Instance.workAbility >= postUpgradeNeeds[2])
                {
                    PostUpgrade(4);
                }
                else
                {
                    PostUpgradeFailed(4);
                }
                break;
            case 4:
                if (PlayerData.Instance.workAbility >= postUpgradeNeeds[3])
                {
                    PostUpgrade(5);
                }
                else
                {
                    PostUpgradeFailed(5);
                }
                break;
            case 5:
                PostUpgradeFailed(6);
                break;
            default:
                break;
        }

    }

    public void SignAll_Update(string s)
    {
        Signs.Add(s);
        if (Signs.Count > 4)
        {
            Signs.RemoveAt(0);
        }

        string signAll = null;
        for (int i = 0; i < Signs.Count; i++)
        {
            if (i < Signs.Count - 1)
            {
                signAll += "<color=#00E9D4>";
            }
            else
            {
                signAll += "<color=#FFB400>";
            }
            signAll += Signs[i] + "</color>" + "\r\n";
        }
        SignAll.text = signAll;
    }

    // public void SignAll_Update(List<string> ss)
    // {
    //     foreach (var s in ss)
    //     {
    //         Signs.Add(s);
    //     }

    //     while (Signs.Count > 4)
    //     {
    //         Signs.RemoveAt(0);
    //     }


    //     string signAll = null;
    //     for (int i = 0; i < Signs.Count; i++)
    //     {
    //         if (i < Signs.Count - ss.Count)
    //         {
    //             signAll += "<color=#00E9D4>";
    //         }
    //         else
    //         {
    //             signAll += "<color=#FFB400>";
    //         }
    //         signAll += Signs[i] + "</color>" + "\r\n";
    //     }
    //     SignAll.text = signAll;
    // }

    void PostUpgrade(int NextLevel)
    {
        PlayerData.Instance.postLevel += 1;
        PlayerData.Instance.postLevelText.text = PlayerData.Instance.postLevel.ToString();
        LAC_Tree_Control.Instance.Stone(true);//更新宝石颜色

        // GameObject sign = Instantiate(SignPrefab, PostUpgradeGroup.transform);
        // sign.GetComponent<TextMeshProUGUI>().text = "岗位升级到T" + NextLevel.ToString();
        // Destroy(sign, SignStayTime);
        string s = "岗位升级到T" + NextLevel.ToString();
        SignAll_Update(s);
    }
    public Dialog dialog_ExecuteButton;
    void PostUpgradeFailed(int NextLevel)
    {
        if (NextLevel > 5)
        {
            // GameObject sign = Instantiate(SignPrefab, PostUpgradeGroup.transform);
            // sign.GetComponent<TextMeshProUGUI>().text = "已升到最高等级";
            // Destroy(sign, SignStayTime);
            dialog_ExecuteButton.SetDiaglog("已升到最高等级");
            string s = "已升到最高等级";
            SignAll_Update(s);
        }
        else
        {
            // GameObject sign = Instantiate(SignPrefab, PostUpgradeGroup.transform);
            // sign.GetComponent<TextMeshProUGUI>().text = "能力值不足";
            // Destroy(sign, SignStayTime);
            dialog_ExecuteButton.SetDiaglog("能力值不足");
            string s = "能力值不足";
            SignAll_Update(s);
        }

    }
    public void AILeaveSign(string name, string reason)//AI滚了
    {
        // GameObject sign = Instantiate(SignPrefab, PostUpgradeGroup.transform);
        // sign.GetComponent<TextMeshProUGUI>().text = name + "因" + reason + "离开了办公室";
        // Destroy(sign, 2 * SignStayTime);
        string s = name + "因" + reason + "离开了办公室";
        SignAll_Update(s);
    }
    public void AICome(string name)//AI加入了职场搏杀
    {
        // GameObject sign = Instantiate(SignPrefab, PostUpgradeGroup.transform);
        // sign.GetComponent<TextMeshProUGUI>().text = name + "加入了你的工作组";
        // Destroy(sign, 2 * SignStayTime);
        string s = name + "加入了你的工作组";
        SignAll_Update(s);
    }

    bool isKPIThisMonthOK = false;//每个月的KPI达标了吗
    bool isFirstKPITest = true;//送眼球的动画
    bool isFirstKPITest2 = true;//消灭眼球的动画
    bool isFirstKPITest3 = true;
    float timer_KPITest = 0f;
    float EyeAniTime = 2f;//眼球动画的播放时间
    bool isGameOver = false;
    void NextPart()
    {

        isFirstKPITest = true;//送眼球的动画
        isFirstKPITest2 = true;//消灭眼球的动画
        isFirstKPITest3 = true;
        timer_KPITest = 0f;
        phase++;
    }

    //死因
    [HideInInspector] public string deathReason = null;

    //
    void KPITest()
    {

        timer_KPITest += Time.deltaTime;

        // if (isFirstKPITest)
        // {
        //     isFirstKPITest = false;
        // if (!isKPIThisMonthOK && PlayerData.Instance.KPI >= KPINeed_EveryMonth)
        // {
        //     isKPIThisMonthOK = true;
        //     PlayerData.Instance.KPILifeChange(+1);
        //     //播放眼球弹射动画
        //     SkullControl.Instance.EmitEye_Ani();
        //     SkullControl.Instance.O_C_Button(true);
        // }
        // else//如果不需要动画，那就跳过
        // {
        //     timer_KPITest = EyeAniTime;
        // }
        // }
        // if (timer_KPITest >= EyeAniTime)
        // {
        if (isFirstKPITest)
        {
            isFirstKPITest = false;

            //先检测有没有死亡，如果死亡，那就没有后面的时期了
            isGameOver = TestifDead();
            if (isGameOver)
            {
                return;
            }

            if (week % 4 == 0)
            {
                //烧毁眼球动画。这是默认要扣的
                PlayerData.Instance.KPILifeChange(-1);
                SkullControl.Instance.DestoryEyeAni();
            }
            else
            {
                timer_KPITest = EyeAniTime;
            }

        }

        // }
        if (timer_KPITest >= EyeAniTime)
        {
            //这是额外要扣的
            //这里面自动会扣KPI和烧眼球，且如果游戏结束，那就会自动弹出对话框
            if (isFirstKPITest2)
            {
                isFirstKPITest2 = false;
                if (week % 4 == 0)
                {
                    isGameOver = ISKPIOK();
                }
                else
                {
                    timer_KPITest = 2 * EyeAniTime;
                }
            }

        }
        if (timer_KPITest >= 2 * EyeAniTime)
        {

            if (!isGameOver)
            {
                NextPart();
            }

        }



    }


    public bool TestifDead()
    {
        bool isDead = false;
        if (
            (!FieldManager.Instance.isUnDeath && (PlayerData.Instance.physicalHealth <= 0 || PlayerData.Instance.spiritualHealth <= 0))
            || (FieldManager.Instance.isUnDeath && (PlayerData.Instance.physicalHealth < -50 || PlayerData.Instance.spiritualHealth < -50))
            )
        {
            isDead = true;
            // globalUI.SetActive(true);
            Warning_Panel.SetActive(true);
            string s = null;
            if (PlayerData.Instance.physicalHealth <= 0)
            {
                s += "“体力过低”";
            }
            if (PlayerData.Instance.spiritualHealth <= 0)
            {
                s += "“精力过低”";
            }
            warningText.text = "你因" + s + "猝死。";
            deathReason = "Health";
            BackMainMenuButton.SetActive(true);
            Debug.Log("Die");
        }
        //锁血
        else if ((FieldManager.Instance.isUnDeath &&
        (PlayerData.Instance.physicalHealth >= -50 && PlayerData.Instance.physicalHealth <= 0
        || PlayerData.Instance.spiritualHealth >= -50 && PlayerData.Instance.spiritualHealth <= 0)))
        {
            PlayerData.Instance.physicalHealth = Mathf.Min(10, PlayerData.Instance.physicalHealthMax);
            PlayerData.Instance.spiritualHealth = Mathf.Min(10, PlayerData.Instance.spiritualHealthMax);
            PlayerData.Instance.UpdateDataToUI_Weekday();
        }

        return isDead;
    }

    bool ISKPIOK()//返回是否被解雇，如果是则播放变黑吃人动画
    {
        bool isGameOver = false;//是否游戏失败
        bool isOpenPanel = false;
        if (PlayerData.Instance.ranking == 5 && AIMechanism.Instance.CalculateAIDataCount() != 0 && isKPIThisMonthOK)//注意这里的PlayerData.Instance.ranking == 5是对的
        {
            // isGameOver = KPILifeReduceSign();

            PlayerData.Instance.KPILifeChange(-1);
            SkullControl.Instance.DestoryEyeAni();
            PlayerData.Instance.UpdateKPILife();
            warningText.text = "你本月排名垫底，多燃烧了一颗眼球！";
            // StartCoroutine(DelaySetActive(Warning_Panel, EyeAniTime));
            // isGameOver = true;
            isOpenPanel = true;
        }
        // else if (PlayerData.Instance.KPI < KPINeed_EveryMonth)
        // {
        //     isGameOver = KPILifeReduceSign(false);
        // }
        // if (PlayerData.Instance.KPILife > 0)
        // {
        //     //+ PlayerData.Instance.KPILife.ToString() + "次"
        // }
        if (PlayerData.Instance.KPILife == 0)
        {
            warningText.text += "所有眼球都烧光了，随后你将成为它的饲料......";
            deathReason = "KPI";
            isOpenPanel = true;
            isGameOver = true;
        }

        if (isOpenPanel)
        {

            StartCoroutine(DelaySetActive(Warning_Panel, EyeAniTime));
        }
        if (isGameOver)
        {
            StartCoroutine(DelaySetActive(BackMainMenuButton, EyeAniTime));
        }
        return isGameOver;
    }
    // bool KPILifeReduceSign()//自己因KPI不足而丢失KPILife
    // {
    //     bool isGameOver = false;
    //     PlayerData.Instance.KPILifeChange(-1);
    //     SkullControl.Instance.DestoryEyeAni();
    //     PlayerData.Instance.UpdateKPILife();
    //     warningText.text = "你本月排名垫底，多燃烧了一颗眼球！";
    //     StartCoroutine(DelaySetActive(Warning_Panel, EyeAniTime));
    //     // Warning_Panel.SetActive(true);
    //     return isGameOver;
    // }

    IEnumerator DelaySetActive(GameObject g, float t)
    {
        yield return new WaitForSeconds(t);
        g.SetActive(true);
        yield break;
    }
    public void O_C_GameObject(GameObject g)
    {
        if (g.activeInHierarchy)
        {
            g.SetActive(false);
        }
        else
        {
            g.SetActive(true);
        }
    }

    public void UpdateTextPrefab(Card card)//后来的卡牌修改了前面卡牌的数值时。用于更新前面卡牌的数值
    {
        List<GameObject> textPrefabList = new List<GameObject>();


        textPrefabList = Cards_TextPrefabList_Dic[card];
        foreach (var textPrefab in textPrefabList)
        {
            TextMeshPro tmp = textPrefab.GetComponent<TextMeshPro>();
            if (tmp.color.CompareRGB(Color.red))
            {
                if (card.functionEffect.physicalHealth > 0)
                {
                    tmp.text = "+" + card.functionEffect.physicalHealth.ToString();
                }
                else
                {
                    tmp.text = card.functionEffect.physicalHealth.ToString();
                }
            }
            else if (tmp.color.CompareRGB(Color.blue))
            {
                if (card.functionEffect.spiritualHealth > 0)
                {
                    tmp.text = "+" + card.functionEffect.spiritualHealth.ToString();
                }
                else
                {
                    tmp.text = card.functionEffect.spiritualHealth.ToString();
                }
            }
            else if (tmp.color.CompareRGB(Color.green))
            {
                if (card.functionEffect.workAbility > 0)
                {
                    tmp.text = "+" + card.functionEffect.workAbility.ToString();
                }
                else
                {
                    tmp.text = card.functionEffect.workAbility.ToString();
                }
            }
            else if (tmp.color.CompareRGB(new Color(1, 150f / 255f, 0, 1)))
            {
                if (card.functionEffect.KPI > 0)
                {
                    tmp.text = "+" + card.functionEffect.KPI.ToString();
                }
                else
                {
                    tmp.text = card.functionEffect.KPI.ToString();
                }
            }
        }
    }
    public void CreatText_AIDebuff(Card card)//队列12伤害AI
    {
        string s = null;
        for (int i = 0; i < card.AIDatas_Debuff.Count; i++)
        {
            string t = AI_Debuff_BUff_Text.text;
            if (t == null)
            {
                s += card.AIDatas_Debuff[i].name;
                if (i != (card.AIDatas_Debuff.Count - 1))//如果不是最后一个
                {
                    s += "、";
                }
            }
            else
            {
                if (!t.Contains(card.AIDatas_Debuff[i].name))
                {
                    s += card.AIDatas_Debuff[i].name;
                    if (i != (card.AIDatas_Debuff.Count - 1))//如果不是最后一个
                    {
                        s += "、";
                    }
                }
            }

        }



        if (s != null)
        {
            // AI_Debuff_Sign_Pannel.SetActive(true);

            AI_Debuff_BUff_Text.text += "你影响了" + s + "\r\n";
        }
    }

    public void AISendCardtoPlayerSign(List<AIData> AIDatas_Bad)//AI送屎环节
    {
        string s = null;
        for (int i = 0; i < AIDatas_Bad.Count; i++)
        {
            s += AIDatas_Bad[i].name;
            if (i != (AIDatas_Bad.Count - 1))//如果不是最后一个
            {
                s += "、";
            }
        }
        if (s != null)
        {
            // AI_Debuff_Sign_Pannel.SetActive(true);
            AI_Debuff_BUff_Text2.text += s + "对你使坏了" + "\r\n";
        }
    }
    void UpdateKPINeedEveryMonth()
    {
        int KPI_Last = Mechanism.Instance.KPIAverage;//上个月最后一周的平均水平


        if (week <= 1 + 2 * 4)//前二个月，疯涨
        {//1.4 2
            int KPINeed_NextMonth_Min = (int)(KPINeed_EveryMonth * Mathf.Pow(KPI_Up_PerWeek_1_Min, 4));//和摸鱼的生存难度有关
            int KPINeed_NextMonth_Max = (int)(KPINeed_EveryMonth * Mathf.Pow(KPI_Up_PerWeek_1_Max, 4));//和猝死的生存难度有关   //(int)(KPINeed_EveryMonth * 2f * (1 + envirRollValue / 2000));
            KPINeed_EveryMonth = Mathf.Clamp((int)(KPI_Last * 4 * Mathf.Pow(KPI_Up_PerWeek_1_Min, 4)), KPINeed_NextMonth_Min, KPINeed_NextMonth_Max);
        }
        else
        {//1.12 1.2
            int KPINeed_NextMonth_Min = (int)(KPINeed_EveryMonth * Mathf.Pow(KPI_Up_PerWeek_9_Min, 4));
            int KPINeed_NextMonth_Max = (int)(KPINeed_EveryMonth * Mathf.Pow(KPI_Up_PerWeek_9_Max, 4));//(int)(KPINeed_EveryMonth * 1.2f * (1 + envirRollValue / 2000));
            KPINeed_EveryMonth = Mathf.Clamp((int)(KPI_Last * 4 * Mathf.Pow(KPI_Up_PerWeek_9_Min, 4)), KPINeed_NextMonth_Min, KPINeed_NextMonth_Max);
        }
        KPINeed_EveryMonthText.text = KPINeed_EveryMonth.ToString();
    }

    public void PlayChess(bool isBeginBlack = true)//需不需要开场的黑场过渡
    {
        StoryManager.Instance.pd.Stop();
        AudioManager.Instance.audioSource_bgm.volume = 0.05f;
        AudioManager.Instance.PlayClip("BGM0", "BGM");
        //显示ui
        // globalUI.SetActive(true);
        playState = PlayState.Chess;
        //开始教学

        if (!TeachManager.Instance.isGuide)
        {
            TeachManager.Instance.SetGuide(StartButton, true);
        }
        else
        {
            TeachManager.Instance.TeachEventTrigger_Delay("开头介绍", 1.1f);
        }

        CameraManager.Instance.SetVirtualCam("BlackCam");
        // if (isBeginBlack)//播完动画后开头有黑幕
        // {
        //     // CameraManager.Instance.Chess_Cam_BlackFade();
        //     // CameraManager.Instance.SetVirtualCam("ChessCam", 0f);//瞬间切换
        //     // StoryManager.Instance.Stranger_SitSofa();
        // }
        // else
        // {
        //     // StoryManager.Instance.Stranger_SitDesk();//坐过来
        //    //看人
        //     // CameraManager.Instance.SetVirtualCamLookAt(CameraManager.Instance.Stranger_Abdomen);//看腹部
        // }

    }

    public void CloseGlobalUI()//被黑衣人杀死时用
    {
        globalUI.SetActive(false);
    }

    // [HideInInspector] new Vector3 chessOriginPos;
    bool isChessBoardMoved = false;//如果还没移动
    // bool isForceOut = false;//有些时候（三选一卡牌的时候），需要强制让棋盘定在外面，以面关闭牌库时将棋盘抽回来
    // public void ForceChessboardOut(bool b)
    // {
    //     isForceOut = b;
    // }
    public IEnumerator ChessBoardMove(bool isReverse = false)
    {
        if (isChessBoardMoved && !isReverse || !isChessBoardMoved && isReverse)//防止三选一的时候，点击牌库时，棋盘被拉回来
        {
            yield break;
        }
        float timer = 0;
        Vector3 originPos = chessBoard.transform.position;
        Vector3 destPos;
        if (!isReverse)
        {
            isChessBoardMoved = true;
            destPos = originPos + new Vector3(-3, 0, 0);
        }
        else
        {
            isChessBoardMoved = false;
            destPos = originPos + new Vector3(3, 0, 0);
        }
        while (true)
        {
            if (timer > 0.5f)
            {
                chessBoard.transform.position = destPos;
                yield break;
            }
            timer += Time.deltaTime;
            float lerpFactor = timer / 0.5f;
            chessBoard.transform.position = Vector3.Lerp(originPos, destPos, lerpFactor);
            yield return null;
        }
    }



}
