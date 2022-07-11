using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//把卡送到playerdata的脚本是ThreeChooseOneCard
public class ThreeChooseOneManager : MonoSingleton<ThreeChooseOneManager>
{
    public int cardAmount = 3;
    public GameObject card3_1Prefab;
    public GameObject cardGroup;//用来排列卡牌的grid
    [HideInInspector] public bool isExitGame = false;

    List<Card> cards_LV1 = new List<Card>();//用来防止生成相同的卡牌
    List<Card> cards_LV2 = new List<Card>();//用来防止生成相同的卡牌
    List<Card> cards_LV3 = new List<Card>();//用来防止生成相同的卡牌
    List<Card> cards_LV4 = new List<Card>();//用来防止生成相同的卡牌
    List<Card> cards_LV5 = new List<Card>();//用来防止生成相同的卡牌
    List<GameObject> card3_1Prefabs = new List<GameObject>();
    private void OnEnable()
    {
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

    void CreateCardRandomLV1()
    {
        if (cards_LV1.Count > 0)
        {
            Card card = cards_LV1[Random.Range(0, cards_LV1.Count)];
            cards_LV1.Remove(card);
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            card3_1Prefabs.Add(cardPrefab);
        }
    }
    void CreateCardRandomLV2()
    {
        if (cards_LV2.Count > 0)
        {
            Card card = cards_LV2[Random.Range(0, cards_LV2.Count)];
            cards_LV2.Remove(card);
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            card3_1Prefabs.Add(cardPrefab);
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
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            card3_1Prefabs.Add(cardPrefab);
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
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            card3_1Prefabs.Add(cardPrefab);
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
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(card);
            card3_1Prefabs.Add(cardPrefab);
        }
        else
        {
            GameObject cardPrefab = Instantiate(card3_1Prefab, cardGroup.transform);
            cardPrefab.GetComponent<CardDisplay>().card = new Card(cards_LV1[Random.Range(0, cards_LV1.Count)]);
            card3_1Prefabs.Add(cardPrefab);

        }
    }



    private void OnDisable()
    {
        if (isExitGame == false)
        {
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
                TeachManager.Instance.TeachEventTrigger("岗位升级介绍");
                TeachManager.Instance.isFirstUpgrade = false;
            }
        }

    }

    public void ExitGame()
    {
        isExitGame = true;
    }



}
