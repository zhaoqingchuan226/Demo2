using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThreeChooseOneCard : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlayClip("button0");
        SendCardToPlayerData();
        Mechanism.Instance.isChoose = false;
        Mechanism.Instance.chooseTimes--;
   
        ThreeChooseOneManager.Instance.Close();
  
    }
    public void SendCardToPlayerData()
    {
        PlayerData.Instance.playerCards.Add(this.gameObject.GetComponent<CardDisplayPersonalGameLibrary>().card);
        PlayerData.Instance.SortCards();
        // PlayerData.Instance.SavePlayerData();//包含了sort服务  //将来需要永久改变playerData时，启用下面一行代码并注释掉上一行代码
        // LibraryManager.Instance.UpdateLibrary();//用PlayerData更新Library信息
    }
}

