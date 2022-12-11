using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//把卡送到playerdata的脚本是ThreeChooseOneCard
public class ThreeChooseOneManager : MonoSingleton<ThreeChooseOneManager>
{
    public int cardAmount = 3;
    public GameObject PanelAll;
    public GameObject card3_1Prefab;
    public GameObject cardGroup;//用来排列卡牌的grid
    public List<Transform> roots = new List<Transform>();
    public List<GameObject> slabStones_obj = new List<GameObject>();//说明
    [HideInInspector] public bool isExitGame = false;


    List<Card> cards_LV1 = new List<Card>();//用来防止生成相同的卡牌
    List<Card> cards_LV2 = new List<Card>();//用来防止生成相同的卡牌
    List<Card> cards_LV3 = new List<Card>();//用来防止生成相同的卡牌
    List<Card> cards_LV4 = new List<Card>();//用来防止生成相同的卡牌
    List<Card> cards_LV5 = new List<Card>();//用来防止生成相同的卡牌
    List<GameObject> card3_1Prefabs = new List<GameObject>();
    public GameObject giveUpButton;


    bool isFirstCard16 = true;//高手的鼠标会偶尔将一张卡变成心流时刻，但是只能变一张，所以用这个bool来控制
    private void Start()
    {
        cardGroup.SetActive(false);
        PanelAll.SetActive(false);
        giveUpButton.SetActive(false);
    }

    [HideInInspector] public bool isFirstChoose = true;//三次三连中，第一次三连为true//三次平时选择中，第一次为true
    public void Open()//触发函数，相当于enabled
    {
        CardStore.Instance.AutoReDesAllCards();
        if (FieldManager.Instance.isMouse && !FieldManager.Instance.isTripletReward)
        {
            isFirstCard16 = true;
        }

        cardGroup.SetActive(true);
        FieldManager.Instance.isSpeakIn1_3 = false;
        if (isFirstChoose)
        {
            // WeekendMeetingAll.Instance.weekendPhase = weekendPhase.Else;

            giveUpButton.SetActive(true);
            isFirstChoose = false;
            // Mechanism.Instance.ForceChessboardOut(true);
            if (Mechanism.Instance.phase == Phase.TripletReward)//三连这么玩
            {
                Mechanism.Instance.StartCoroutine(Mechanism.Instance.ChessBoardMove());
            }
            // else//周末选牌会移动周末的场景
            // {
            //     StartCoroutine(WeekendMeetingAll.Instance.WeekendMeetingMove());
            // }

            CameraManager.Instance.SetVirtualCam("ChessCam");
        }


        // StoryManager.Instance.Stranger_SitDesk();//坐过来

        PanelAll.SetActive(true);
        cards_LV1.Clear();
        cards_LV2.Clear();
        cards_LV3.Clear();
        cards_LV4.Clear();
        cards_LV5.Clear();

        foreach (var card in CardStore.Instance.cards_LV1)
        {
            cards_LV1.Add(card);
        }
        foreach (var card in CardStore.Instance.cards_LV2)
        {
            cards_LV2.Add(card);
        }
        foreach (var card in CardStore.Instance.cards_LV3)
        {
            cards_LV3.Add(card);
        }
        foreach (var card in CardStore.Instance.cards_LV4)
        {
            cards_LV4.Add(card);
        }
        foreach (var card in CardStore.Instance.cards_LV5)
        {
            cards_LV5.Add(card);
        }



        for (int i = 0; i < cardAmount; i++)
        {
            if (cards_LV1.Count == 0 && cards_LV2.Count == 0 && cards_LV3.Count == 0 && cards_LV4.Count == 0 && cards_LV5.Count == 0)
            {
                return;
            }


            if (FieldManager.Instance.isTripletReward || (!FieldManager.Instance.isTripletReward && FieldManager.Instance.luckyTimes > 0))//三连时
            {
                switch (PlayerData.Instance.postLevel)
                {
                    case 1:
                        CreateCardRandomLV2();
                        break;
                    case 2:
                        CreateCardRandomLV3();
                        break;
                    case 3:
                        CreateCardRandomLV4();
                        break;
                    case 4:
                        CreateCardRandomLV5();
                        break;
                    case 5:
                        CreateCardRandomLV5();
                        break;
                    default: break;
                }
            }
            else//平时瞎摸
            {
                float r = Random.Range(0f, 1);
                float lv1Max = CardStore.Instance.probability.levelNumsFinal[0] / CardStore.Instance.probability.levelNumsFinalSum;
                float lv2Max = (CardStore.Instance.probability.levelNumsFinal[0] + CardStore.Instance.probability.levelNumsFinal[1]) / CardStore.Instance.probability.levelNumsFinalSum;
                float lv3Max = (CardStore.Instance.probability.levelNumsFinal[0] + CardStore.Instance.probability.levelNumsFinal[1] + CardStore.Instance.probability.levelNumsFinal[2]) / CardStore.Instance.probability.levelNumsFinalSum;
                float lv4Max = (CardStore.Instance.probability.levelNumsFinal[0] + CardStore.Instance.probability.levelNumsFinal[1] + CardStore.Instance.probability.levelNumsFinal[2] + CardStore.Instance.probability.levelNumsFinal[3]) / CardStore.Instance.probability.levelNumsFinalSum;

                if (r <= lv1Max)
                {
                    CreateCardRandomLV1();
                }
                else if (r > lv1Max && r < lv2Max)
                {
                    CreateCardRandomLV2();

                }
                else if (r >= lv2Max && r < lv3Max)
                {
                    CreateCardRandomLV3();
                }
                else if (r >= lv3Max && r < lv4Max)
                {
                    CreateCardRandomLV4();
                }
                else if (r >= lv4Max)
                {
                    CreateCardRandomLV5();
                }

            }

        }


    }


    [HideInInspector] public int currentCardAmount = 0;//现在界面里有多少卡牌
    // public float cardInterval = -0.2f;

    //在开启高手的鼠标时，有5%的概率会将卡牌变成心流时刻
    void CreatCard(Card card)
    {

        if (FieldManager.Instance.isMouse && !FieldManager.Instance.isTripletReward)
        {
            if (isFirstCard16)
            {
                float r = Random.Range(0, 1f);
                if (r < 0.5f)
                {
                    isFirstCard16 = false;
                    card = CardStore.Instance.SearchCard(16);
                }
            }
        }

        currentCardAmount += 1;
        // Vector3 deltaPos = new Vector3(cardInterval, 0, 0) * (currentCardAmount - 2);
        GameObject cardPrefab = Instantiate(card3_1Prefab, roots[currentCardAmount - 1]);
        cardPrefab.GetComponent<CardDisplayPersonalGameLibrary>().card = new Card(card);
        cardPrefab.GetComponent<CardDisplayPersonalGameLibrary>().delatTime = 0.05f * (currentCardAmount);
        cardPrefab.GetComponent<CardDisplayPersonalGameLibrary>().posNum = currentCardAmount - 1;
        //cardPrefab.transform.position += deltaPos;
        card3_1Prefabs.Add(cardPrefab);
    }

    void CreateCardRandomLV1()
    {
        if (cards_LV1.Count > 0)
        {
            Card card = cards_LV1[Random.Range(0, cards_LV1.Count)];
            cards_LV1.Remove(card);
            CreatCard(card);
            // GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            // cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            // card3_1Prefabs.Add(cardPrefab);
        }
    }
    void CreateCardRandomLV2()
    {
        if (cards_LV2.Count > 0)
        {
            Card card = cards_LV2[Random.Range(0, cards_LV2.Count)];
            cards_LV2.Remove(card);
            CreatCard(card);
            // GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            // cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            // card3_1Prefabs.Add(cardPrefab);
        }
        else
        {
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(cards_LV1[Random.Range(0, cards_LV1.Count)]);
            card3_1Prefabs.Add(cardPrefab);
        }
    }
    void CreateCardRandomLV3()
    {
        if (cards_LV3.Count > 0)
        {
            Card card = cards_LV3[Random.Range(0, cards_LV3.Count)];
            cards_LV3.Remove(card);
            CreatCard(card);
            // GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            // cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            // card3_1Prefabs.Add(cardPrefab);
        }
        else
        {
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(cards_LV1[Random.Range(0, cards_LV1.Count)]);
            card3_1Prefabs.Add(cardPrefab);
        }
    }
    void CreateCardRandomLV4()
    {
        if (cards_LV4.Count > 0)
        {
            Card card = cards_LV4[Random.Range(0, cards_LV4.Count)];
            cards_LV4.Remove(card);
            CreatCard(card);
            // GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            // cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            // card3_1Prefabs.Add(cardPrefab);
        }
        else
        {
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(cards_LV1[Random.Range(0, cards_LV1.Count)]);
            card3_1Prefabs.Add(cardPrefab);

        }
    }
    void CreateCardRandomLV5()
    {
        if (cards_LV5.Count > 0)
        {
            Card card = cards_LV5[Random.Range(0, cards_LV5.Count)];
            cards_LV5.Remove(card);
            CreatCard(card);
            // GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            // cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            // card3_1Prefabs.Add(cardPrefab);
        }
        else
        {
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(cards_LV1[Random.Range(0, cards_LV1.Count)]);
            card3_1Prefabs.Add(cardPrefab);
        }
    }



    public void Close()
    {
        if (isExitGame == false)
        {
            cardGroup.SetActive(false);
            currentCardAmount = 0;
            for (int i = 0; i < card3_1Prefabs.Count; i++)
            {
                Destroy(card3_1Prefabs[i]);
            }

            card3_1Prefabs.Clear();
            if (FieldManager.Instance.luckyTimes > 0 && !FieldManager.Instance.isTripletReward)
            {
                FieldManager.Instance.luckyTimes--;
            }

            if (TeachManager.Instance.isFirstUpgrade && Mechanism.Instance.chooseTimes == 0)
            {
                TeachManager.Instance.TeachEventTrigger_Delay("岗位升级介绍", 1.1f);
                TeachManager.Instance.isFirstUpgrade = false;
            }
            PanelAll.SetActive(false);

            //棋盘移动
            if (Mechanism.Instance.chooseTimes == 0)
            {

                if (Mechanism.Instance.phase == Phase.TripletReward)
                {
                    Mechanism.Instance.StartCoroutine(Mechanism.Instance.ChessBoardMove(true));
                    CameraManager.Instance.SetVirtualCam("BlackCam");
                }
                else
                {
                    Mechanism.Instance.EnterPhase(Phase.Map);
                }


                giveUpButton.SetActive(false);
                isFirstChoose = true;


            }
        }

    }

    public void ExitGame()
    {
        isExitGame = true;
    }



}
