using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
//点击购买卡牌的脚本
public class HolidayStoreDelete : MonoSingleton<HolidayStoreDelete>, IPointerClickHandler
{
    public bool isSell = false;
    public GameObject haveSold;
    int price = 3000;
    public TextMeshProUGUI priceText;
    [HideInInspector] public bool isFollowMouse = false;

    public GameObject fish_model;//没有碰撞体的模型
    Vector3 fish_model_originPos;
    void Start()
    {
        fish_model_originPos = fish_model.transform.localPosition;

        // priceText.text = price.ToString();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSell && PlayerData.Instance.playerCards.Count > 0 && PlayerData.Instance.playerMoney >= price)
        {
            LACControl.Instance.ReduceCoin(price);
            isSell = true;
            OpenLibrary();
            if (haveSold != null)
            {
                haveSold.SetActive(true);
            }
            isFollowMouse = true;
        }
        else if (isSell && PlayerData.Instance.playerCards.Count > 0 && PlayerData.Instance.playerMoney >= price)
        {
            HolidayStore.Instance.FishHaveEaten();
        }
        else if (PlayerData.Instance.playerMoney < price)
        {
            HolidayStore.Instance.FishRefuse();
        }

    }
    void Update()
    {
        if (isFollowMouse)
        {
            FishFollow();
        }
    }
    public void SetOriginPos()
    {
        fish_model.transform.localPosition = fish_model_originPos;
    }
    void FishFollow()
    {
        Vector3 vePos = Camera.main.WorldToScreenPoint(fish_model.transform.position);
        Vector3 mosPos = Input.mousePosition;
        mosPos.z = vePos.z;
        Vector3 WorldPos = Camera.main.ScreenToWorldPoint(mosPos);
        fish_model.transform.position = WorldPos;
    }
    public void Reset()
    {
        if (haveSold != null)
        {
            haveSold.SetActive(false);
        }
        isSell = false;
        // Debug.Log(isSell);
    }

    void OpenLibrary()
    {
        // LibraryManager.Instance.gameObject.transform.Find("ScrollView").gameObject.SetActive(true);
        // LibraryManager.Instance.UpdateLibrary();
        // LibraryManager.Instance.isDeleteMode = true;
        LibraryManager.Instance.isDeleteMode = true;
        LibraryManager.Instance.OpenLibrary();
        HolidayStore.Instance.backGround.SetActive(false);
    }
    // public void SendCardToPlayerData()
    // {
    //     PlayerData.Instance.playerCards.Add(this.gameObject.GetComponent<CardDisplay>().card);
    //     PlayerData.Instance.SortCards();
    //     // PlayerData.Instance.SavePlayerData();//包含了sort服务  //将来需要永久改变playerData时，启用下面一行代码并注释掉上一行代码
    //     LibraryManager.Instance.UpdateLibrary();//用PlayerData更新Library信息
    // }
}

