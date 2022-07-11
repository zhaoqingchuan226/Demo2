using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card_Delete : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (LibraryManager.Instance.isDeleteMode)
        {
            Card card = this.gameObject.GetComponent<CardDisplay>().card;
            if (PlayerData.Instance.playerCards.Contains(card))
            {
                Debug.Log("Remove");
                PlayerData.Instance.playerCards.Remove(card);
                LibraryManager.Instance.UpdateLibrary();
                LibraryManager.Instance.isDeleteMode = false;
                LibraryManager.Instance.gameObject.transform.Find("ScrollView").gameObject.SetActive(false);
     
            }
        }
    }
}
