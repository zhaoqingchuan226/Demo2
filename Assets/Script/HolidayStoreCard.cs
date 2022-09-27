using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
//点击购买卡牌的脚本
public class HolidayStoreCard : MonoBehaviour, IPointerClickHandler
{
    bool isSell = false;
    public GameObject haveSold;
    [HideInInspector] public int price = 0;
    public TextMeshPro priceText;

    void Start()
    {
        priceText.text = price.ToString();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSell && PlayerData.Instance.playerMoney >= price)
        {
            LACControl.Instance.ReduceCoin(price);
            AudioManager.Instance.PlayClip("Ding");
            isSell = true;
            SendCardToPlayerData();
            haveSold.SetActive(true);
            this.GetComponent<CardDisplayPersonalGameLibrary>().PlayBuyFx();
            priceText.text = "已出售";
            priceText.color = Color.red;
        }
        else if (PlayerData.Instance.playerMoney < price && !isSell)
        {
            Mechanism.Instance.SignAll_Update("你没有财富啊没有财富");
        }

    }
    public void SendCardToPlayerData()
    {
        PlayerData.Instance.playerCards.Add(this.gameObject.GetComponent<CardDisplayPersonalGameLibrary>().card);
        PlayerData.Instance.SortCards();
    }
}

