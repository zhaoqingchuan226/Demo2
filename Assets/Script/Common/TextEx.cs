using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TextEx : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 originScale;
    public Color Entercolor;
    Color originColor;
    Image text;
    GameObject WhiteLine;
    Image whiteline;
    void Start()
    {
        originScale = transform.localScale;
        text = gameObject.transform.Find("Text").gameObject.GetComponent<Image>();
        WhiteLine = gameObject.transform.Find("whiteline").gameObject;
        whiteline = WhiteLine.GetComponent<Image>();
        WhiteLine.SetActive(false);
        originColor = text.color;
    }
    public float zoomSize = 1.2f;
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        transform.localScale = originScale * zoomSize;
        text.color = Entercolor;
        WhiteLine.SetActive(true);
        whiteline.color = Entercolor;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        transform.localScale = originScale;
        text.color = originColor;
        WhiteLine.SetActive(false);
        whiteline.color = originColor;
    }
}
