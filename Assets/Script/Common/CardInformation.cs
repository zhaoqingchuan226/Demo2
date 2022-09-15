using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardInformation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 originScale;
    public GameObject InformationBoard;
    GameObject currentInfor;
    TextMeshProUGUI InforText;

    void Start()
    {
        originScale = transform.localScale;

    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (currentInfor == null)
        {
            GameObject CardInformationPool = GameObject.Find("CardInformationPool");
            GameObject InformationBoard_Instance = Instantiate(InformationBoard, CardInformationPool.transform);
            float offset = 0;
            if (this.transform.position.x <= Screen.width / 2)
            {
                offset = this.GetComponent<RectTransform>().sizeDelta.x * 1.16f * Screen.width / 1920;
            }
            else
            {
                offset = -this.GetComponent<RectTransform>().sizeDelta.x * 1.16f * Screen.width / 1920;
            }
            InformationBoard_Instance.transform.position = this.transform.position + new Vector3(offset, 0, 0);
            // RectTransform rt_this = this.GetComponent<RectTransform>();
            // RectTransform rt_infor = InformationBoard_Instance.GetComponent<RectTransform>();
            // rt_infor.anchoredPosition = new Vector2(rt_this.anchoredPosition.x + rt_this.sizeDelta.x, rt_this.anchoredPosition.y);
            currentInfor = InformationBoard_Instance;
            InforText = currentInfor.transform.Find("DescriptionText").gameObject.GetComponent<TextMeshProUGUI>();
            InforText.text = this.GetComponent<CardDisplay>().card.cardInfor;

        }

    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Destroy(currentInfor);
        currentInfor = null;
    }
    void OnDisable()
    {
        if (currentInfor != null)
        {
            Destroy(currentInfor);
            currentInfor = null;
        }
    }
}
