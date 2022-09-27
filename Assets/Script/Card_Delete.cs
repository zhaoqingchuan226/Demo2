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
            Card card = this.gameObject.GetComponent<CardDisplayPersonalGameLibrary>().card;
            if (PlayerData.Instance.playerCards.Contains(card))
            {
    
                PlayerData.Instance.playerCards.Remove(card);

                // LibraryManager.Instance.UpdateLibrary();
                LibraryManager.Instance.isDeleteMode = false;
                LibraryManager.Instance.CloseLibrary();
                HolidayStore.Instance.backGround.SetActive(true);
                // this.GetComponent<CardDisplayPersonalGameLibrary>().ClearAll();
                // if (LibraryManager.Instance.libraryCards.Contains(this.gameObject))
                // {
                //     LibraryManager.Instance.libraryCards.Remove(this.gameObject);
                // }
                // Destroy(this.gameObject);
                HolidayStoreDelete.Instance.isFollowMouse = false;//不再跟随
                HolidayStoreDelete.Instance.SetOriginPos();//归位
                HolidayStore.Instance.FishEatCard();
            }
        }
    }
}
