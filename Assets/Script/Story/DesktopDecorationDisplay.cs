using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//此脚本挂载在card prefab上
//用于将Card中的数据赋予给UI
public class DesktopDecorationDisplay : MonoBehaviour, IPointerDownHandler
{

    public GameObject model = null;

    public DesktopDecoration dd;
    public GameObject infoPanel_Prefab;
    public GameObject infoPanel = null;
    Transform infoPool;
    // public TextMeshProUGUI infoText;
    // bool isInfoOPen = false;

    void Start()
    {
        ShowDD();
        infoPool = GameObject.Find("UI").transform.Find("----------Global-----------").Find("DDInformationPool");
    }
    void OnEnable()
    {
        // ShowCard();
    }

    void ShowDD()//将Card中的数据赋予给UI
    {
        if (dd != null)
        {
            // if (model != null)
            // {
            //     Destroy(model);
            //     model = null;
            // }
            model = Instantiate(DesktopDecorationStore.Instance.SearchDD_Model(dd.id), this.transform);
            dd.GetDD();
        }

    }
    public void OnPointerDown(PointerEventData eventData)
    {

        ShowInfo();
    }

    void ShowInfo()
    {
        if (infoPanel == null)
        {

            infoPanel = Instantiate(infoPanel_Prefab, infoPool);
            infoPanel.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
            infoPanel.GetComponentInChildren<TextMeshProUGUI>().text = AutoDDInfo();
        }
        else
        {
            infoPanel.SetActive(true);
        }
    }

    string AutoDDInfo()
    {
        string s = null;
        if (dd != null)
        {
            s += "<size=25>" + dd.title + "：\r\n" + "</size>";
            s += "\r\n";
            s += dd.description;
            s += "\r\n";
            s += "\r\n";
            s += "<color=#00F0FF>" + dd.funDes + "\r\n" + "</color>";
            s += "\r\n";
        }
        return s;
    }
}
