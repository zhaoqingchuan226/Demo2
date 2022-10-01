using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public partial class Mechanism : MonoSingleton<Mechanism>
{
    // [HideInInspector] public List<Card> cardList_Black = new List<Card>();//20个card的集合
    // [HideInInspector] public List<Card> playerCards_Black = new List<Card>();//卡库复制
    // [HideInInspector] public List<Card> playerCards_Night_Black = new List<Card>();
    // [HideInInspector] public List<Card> playerCards_Noon_Black = new List<Card>();
    // [HideInInspector] public List<Card> playerCards_Others_Black = new List<Card>();

    [HideInInspector] public int physicalHealthAverage = 0;
    [HideInInspector] public int spiritualHealthAverage = 0;
    [HideInInspector] public int workAbilityAverage = 0;
    [HideInInspector] public int KPIAverage = 0;
    // const int times = 1;//计算次数
    // [HideInInspector] public FunctionEffect functionEffectBuffer_Black = default;
    // //CreatAnimation
    // public float animationTimer_Black;

    // //Assign
    // [HideInInspector] public List<Card> cardList_CurrentQueue_Black = new List<Card>();//在Mechanism中这个是gameobj的集合
    // // [HideInInspector] public List<Card> cardList_Queue8_Black = new List<Card>();//queue8时，被计算过的queue10卡牌
    void CalculatePlayerStrength()
    {
        physicalHealthAverage = Mechanism.Instance.functionEffectBuffer.physicalHealth;
        spiritualHealthAverage = Mechanism.Instance.functionEffectBuffer.physicalHealth;
        workAbilityAverage = Mechanism.Instance.functionEffectBuffer.workAbility;
        KPIAverage = Mechanism.Instance.functionEffectBuffer.KPI;
    }
    /*
        void CreatCardAnimation_Black()
        {
            //此次操作
            for (int i = 1; i < 21; i++)
            {
                if (i % 4 == 0)//晚
                {
                    if (playerCards_Night_Black.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        // print("Night");
                        CreatCard_Black(ref playerCards_Night_Black, i);//用playerCards_Night这个List来创建夜晚卡
                    }
                }
                else if ((i % 4 == 2))//中,如果中午没有事情，那么会用Others的行为来填充
                {
                    if (playerCards_Noon_Black.Count == 0)
                    {
                        if (playerCards_Others_Black.Count > 0)
                        {
                            // print("Others");
                            CreatCard_Black(ref playerCards_Others_Black, i);
                        }
                    }
                    else//playerCards_Noon中有卡牌，就用这个卡牌来填充
                    {
                        // print("Noon");
                        CreatCard_Black(ref playerCards_Noon_Black, i);
                    }
                }
                else
                {
                    if (playerCards_Others_Black.Count == 0)//其他时刻
                    {
                    }
                    else
                    {
                        // print("Others");
                        CreatCard_Black(ref playerCards_Others_Black, i);
                    }
                }

            }

        }
        void Assign_Black()
        {
            for (int i = 0; i < 22; i++)//i代表队列，也是animationCounter
            {
                if (i == 20)//亡语不计算
                {
                    continue;
                }
                if (i == 21)//第二十二次，也就是全部生成完毕，开始移动和修改PlayerData的数据
                {

                    foreach (var card in cardList_Black)
                    {
                        functionEffectBuffer_Black.physicalHealth += card.functionEffect.physicalHealth;
                        functionEffectBuffer_Black.spiritualHealth += card.functionEffect.spiritualHealth;
                        functionEffectBuffer_Black.workAbility += card.functionEffect.workAbility;
                        functionEffectBuffer_Black.KPI += card.functionEffect.KPI;

                    }

                    continue;
                }

                cardList_CurrentQueue_Black.Clear();
                foreach (var card in cardList_Black)
                {
                    if (card.executeQueue == i)
                    {
                        cardList_CurrentQueue_Black.Add(card);
                    }
                }

                //队列0-20
                //如果当前队列没有卡牌则直接下一队列
                if (cardList_CurrentQueue_Black.Count == 0)
                {
                    continue;
                }
                else
                {
                    if (i == 8)
                    {

                        bool isCirculation = true;
                        int CirculationTimes = 0;
                        while (isCirculation == true)
                        {
                            CirculationTimes++;
                            Card card = cardList_CurrentQueue_Black[0];

                            if (card.executeQueue == 10)  //如果是Queue10
                            {
                                // card.CardEffect();
                                // // card.functionEffectEx.Initialize();
                                // cardList_Queue8_Black.Add(card);//表明这些队列10的卡组已经在队列8时计算过了
                            }
                            else if (card.executeQueue == 8)
                            {
                                card.CardEffect();
                                functionEffectBuffer_Black.physicalHealth += card.functionEffect.physicalHealth;
                                functionEffectBuffer_Black.spiritualHealth += card.functionEffect.spiritualHealth;
                                functionEffectBuffer_Black.workAbility += card.functionEffect.workAbility;
                                functionEffectBuffer_Black.KPI += card.functionEffect.KPI;
                            }

                            cardList_CurrentQueue_Black.RemoveAt(0);
                            if (card.executeQueue == 10)//如果卡牌不再具有连轴能力，那就去找下一个槽位的连轴卡牌
                            {

                            }
                            else if ((card.executeQueue == 8))//连轴  b/gf
                            {
                                //彻底摧毁当前卡牌
                                cardList_Black.Remove(card);
                                List<Card> playerCards_Times_Less11 = new List<Card>();//卡牌必须要时间对，以及小于11
                                List<Card> playerCards_Times = new List<Card>();
                                if (card.times.Contains("晚") && card.times.ToCharArray().Length == 1)
                                {
                                    playerCards_Times = playerCards_Night_Black;
                                }
                                else if (card.times.Contains("中") && card.times.ToCharArray().Length == 1)//中,如果中午没有事情，那么会用Others的行为来填充
                                {
                                    playerCards_Times = playerCards_Noon_Black;
                                }
                                else
                                {
                                    playerCards_Times = playerCards_Others_Black;
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
                                    //卡牌不足
                                }
                                else
                                {
                                    int r = Random.Range(0, playerCards_Times_Less11.Count);
                                    Card card2 = playerCards_Times_Less11[r];
                                    playerCards_Times.Remove(card2);
                                    card2.posNum = card.posNum;
                                    cardList_Black.Add(card2);
                                    card2.functionEffectEx = card.functionEffectEx;
                                    cardList_CurrentQueue_Black.Insert(0, card2);
                                }
                            }
                            if (cardList_CurrentQueue_Black.Count == 0)//如果这个队列中的卡全部清零了（原卡组一张10都没有），或者全部都是10
                            {
                                isCirculation = false;//
                            }


                        }

                    }
                    else//除了连轴卡牌的卡牌
                    {
                        foreach (var card in cardList_CurrentQueue_Black)
                        {
                            // if (!cardList_Queue8_Black.Contains(card))//如果在queue8中计算过了，这边就不用再计算了
                            // {

                            card.CardEffect();//Calculate!!!!!!!!!!!!!!!!!

                            // }
                            // else
                            // {

                            // }
                        }
                    }
                }
            }





        }
        void CreatCard_Black(ref List<Card> playerCards_Times, int i)
        {
            int r = Random.Range(0, playerCards_Times.Count);
            Card card = playerCards_Times[r];
            playerCards_Times.RemoveAt(r);
            card.posNum = i;//posNum在此处赋值
            cardList_Black.Add(card);
        }
        void UpdateMechanism_Black()
        {
            playerCards_Black.Clear();
            playerCards_Night_Black.Clear();
            playerCards_Noon_Black.Clear();
            playerCards_Others_Black.Clear();

            foreach (var card in PlayerData.Instance.playerCards)
            {
                card.functionEffect = default;//所有卡牌的效果归零
                card.functionEffectEx.Initialize();//所有卡牌的效果Ex归零
                card.posNum = 0;//所有卡牌的位置归零

                playerCards_Black.Add(card);
            }
            foreach (var card in playerCards_Black)
            {
                if (card.times.Contains("晚") && card.times.ToCharArray().Length == 1)
                {
                    playerCards_Night_Black.Add(card);
                }
                else if (card.times.Contains("中") && card.times.ToCharArray().Length == 1)
                {
                    playerCards_Noon_Black.Add(card);
                }
                else
                {
                    playerCards_Others_Black.Add(card);
                }
            }

        }


    */
}
