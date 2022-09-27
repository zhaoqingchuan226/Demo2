using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
// using TMPro;
public class FishAnchorCollidor : MonoBehaviour, IPointerDownHandler
{
    public string direction;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (LibraryManager.Instance.isOpen && CameraManager.Instance.currentVirtualCamera.name != "LibrarySpeCam")//牌库打开并且现在的相机不是具体棋子的特写相机
        {
            if (direction == "left")
            {
                FishAnchorControl.Instance.TurnLeft();
            }
            else if (direction == "right")
            {
                FishAnchorControl.Instance.TurnRight();
            }
            AudioManager.Instance.PlayClip("button1");
        }

    }

}
