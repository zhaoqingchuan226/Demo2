using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolidayStore : MonoSingleton<HolidayStore>
{
    public int cardAmount = 3;
    public GameObject cardPrefab;
    List<GameObject> cardPrefabs = new List<GameObject>();
    public GameObject cardGroup;

    public int propAmount = 2;
    public GameObject propPrefab;
    List<GameObject> propPrefabs = new List<GameObject>();
    public GameObject propGroup;

    public GameObject delete;


    public List<int> value_EveryLevel = new List<int>();//每一等级卡牌的价格

    public int deletePrice = 3000;



    void OnEnable()
    {
        if (Mechanism.Instance.week > 8)
        {
            cardAmount = 6;
            propAmount = 3;
        }

        // Debug.Log(1);
        foreach (var cp in cardPrefabs)
        {
            Destroy(cp);
        }
        cardPrefabs.Clear();
        foreach (var pp in propPrefabs)
        {
            Destroy(pp);
        }
        propPrefabs.Clear();


        for (int i = 0; i < cardAmount; i++)
        {
            int postLevel = Mathf.Min(PlayerData.Instance.postLevel + 1, 5);
            Card card = CardStore.Instance.RandomCard(postLevel, false);
            int price = value_EveryLevel[card.qualityLevel - 1];
            GameObject cardPG2 = Instantiate(cardPrefab, cardGroup.transform);
            cardPG2.GetComponent<CardDisplay>().card = card;
            cardPG2.GetComponent<HolidayStoreCard>().price = price;
            cardPrefabs.Add(cardPG2);
        }



        for (int i = 0; i < propAmount; i++)
        {
            Prop prop = PropStore.Instance.RandomProp();
            GameObject propPG = Instantiate(propPrefab, propGroup.transform);
            propPG.GetComponent<PropDisplay>().prop = prop;
            propPrefabs.Add(propPG);
        }

        HolidayStoreDelete hd = delete.GetComponent<HolidayStoreDelete>();
        hd.isSell = false;
        hd.haveSold.SetActive(false);
        hd.price = deletePrice;
        hd.priceText.text = deletePrice.ToString();


    }



}

