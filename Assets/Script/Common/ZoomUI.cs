using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 originScale;
    void Start()
    {
        originScale=transform.localScale;
    }
    public float zoomSize = 1.2f;
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        transform.localScale = originScale*zoomSize;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        transform.localScale = originScale;
    }
}
