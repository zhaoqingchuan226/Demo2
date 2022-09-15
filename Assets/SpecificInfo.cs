using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class SpecificInfo : MonoBehaviour
{
    bool isInfoOpen = false;


    private void OnMouseDown()
    {
        CardDisplayPersonalGameLibrary cp = this.GetComponent<CardDisplayPersonalGameLibrary>();
        if (!LibraryManager.Instance.isDeleteMode && cp.cardPG_Type == CardPG_Type.Libaray)
        {
            isInfoOpen = !isInfoOpen;
            cp.ShowCardInfo(isInfoOpen);
            int posNum = cp.posNum;
            if (isInfoOpen)
            {
                CameraManager.Instance.SetVirtualCam("LibrarySpeCam", 0.3f);

                CameraManager.Instance.SetVirtualCamFollow(LibraryManager.Instance.roots[posNum]);
                LibraryManager.Instance.O_C_Roots(posNum, false);
            }
            else
            {
                CameraManager.Instance.BackLastCamera(0.3f);
                LibraryManager.Instance.O_C_Roots(posNum, true);
            }
        }
    }
    void OnMouseEnter()
    {
        Cursor.SetCursor(MouseControl.Instance.SearchTex, Vector2.zero, CursorMode.Auto);
    }
    void OnMouseExit()
    {
        Cursor.SetCursor(default, Vector2.zero, CursorMode.Auto);
    }
}
