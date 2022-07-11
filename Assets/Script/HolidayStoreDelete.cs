using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
//点击购买卡牌的脚本
public class HolidayStoreDelete : MonoBehaviour, IPointerClickHandler
{
    public bool isSell = false;
    public GameObject haveSold;
    [HideInInspector] public int price;
    public TextMeshProUGUI priceText;

    void Start()
    {
        // priceText.text = price.ToString();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSell && PlayerData.Instance.playerCards.Count > 0 && PlayerData.Instance.playerMoney >= price)
        {
            PlayerData.Instance.ChangeMoney(-price);
            isSell = true;
            OpenLibrary();
            haveSold.SetActive(true);
        }
        else if (PlayerData.Instance.playerMoney < price)
        {
            Mechanism.Instance.SignAll_Update("你没有财富啊没有财富");
        }
    }

    void OpenLibrary()
    {
        LibraryManager.Instance.gameObject.transform.Find("ScrollView").gameObject.SetActive(true);
        LibraryManager.Instance.UpdateLibrary();
        LibraryManager.Instance.isDeleteMode = true;
    }
    // public void SendCardToPlayerData()
    // {
    //     PlayerData.Instance.playerCards.Add(this.gameObject.GetComponent<CardDisplay>().card);
    //     PlayerData.Instance.SortCards();
    //     // PlayerData.Instance.SavePlayerData();//包含了sort服务  //将来需要永久改变playerData时，启用下面一行代码并注释掉上一行代码
    //     LibraryManager.Instance.UpdateLibrary();//用PlayerData更新Library信息
    // }
}

