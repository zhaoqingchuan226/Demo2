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
    // Transform infoPool;
    [HideInInspector] public Dialog dialog;
    // public TextMeshProUGUI infoText;
    // bool isInfoOPen = false;
    List<string> words = new List<string>();
    int wordsCounter = 0;

    void Start()
    {

        dialog = this.GetComponent<Dialog>();
        ShowDD();
        AddWords();
        // infoPool = GameObject.Find("UI").transform.Find("----------Global-----------").Find("DDInformationPool");
    }

    void AddWords()
    {
        switch (dd.id)
        {
            case 1:
                words.Add("在你死亡时，我将会唤醒你，继续干活");
                words.Add("如果只是透支一点点，我还是能救活你的");
                words.Add("可别透支太多了，嘿嘿嘿");
                break;
            case 2:
                words.Add("加班时别上一个发夹，打起精神来！");
                words.Add("你将付出更多体力和精力，但是可以获得更多绩效！");
                words.Add("同时还能提高颅顶线，增加脱单率！");
                break;
            case 3:
                words.Add("白昼的精力需要节约");
                words.Add("当夜幕降临，释放你的潜能");
                words.Add("你的绩效将无人能敌！");
                break;
            case 4:
                words.Add("禁忌！更多的禁忌！");
                words.Add("每个月末，我会赋予你新的禁忌！");
                words.Add("记得查看你的牌库哦，哦嘿嘿嘿");
                break;
            case 11:
                words.Add("你听说过心流吗，一种极为专注的状态");
                words.Add("极为强大的它将被赋予给弱小的你");
                words.Add("选牌时，祈祷心流的眷顾吧！");
                break;
            case 12:
                words.Add("持续提升，阶段质变！");
                words.Add("让我看到你在提升自己的能力！");
                words.Add("每五次，你的能力收益将永久提高！");
                break;
            default:
                break;
        }
    }
    // void OnEnable()
    // {
    //     // ShowCard();
    // }

    void ShowDD()//将Card中的数据赋予给UI
    {
        if (dd != null)
        {
            model = Instantiate(DesktopDecorationStore.Instance.SearchDD_Model(dd.id), this.transform);
            dd.GetDD();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(dd.title);
        dialog.SetDiaglog(SpeakWords());
        // ShowInfo();
    }

    void OnMouseEnter()
    {
        Cursor.SetCursor(MouseControl.Instance.SearchTex, Vector2.zero, CursorMode.Auto);
    }
    void OnMouseExit()
    {
        Cursor.SetCursor(default, Vector2.zero, CursorMode.Auto);
    }

    string SpeakWords()
    {
        string s = null;
        if (words.Count > 0)
        {
            s = words[wordsCounter];
            wordsCounter++;
            if (wordsCounter == words.Count)
            {
                wordsCounter = 0;
            }
        }
        return s;
    }
    // void ShowInfo()
    // {
    //     if (infoPanel == null)
    //     {

    //         infoPanel = Instantiate(infoPanel_Prefab, infoPool);
    //         infoPanel.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
    //         infoPanel.GetComponentInChildren<TextMeshProUGUI>().text = AutoDDInfo();
    //     }
    //     else
    //     {
    //         infoPanel.SetActive(true);
    //     }
    // }

    // string AutoDDInfo()
    // {
    //     string s = null;
    //     if (dd != null)
    //     {
    //         s += "<size=25>" + dd.title + "：\r\n" + "</size>";
    //         s += "\r\n";
    //         s += dd.description;
    //         s += "\r\n";
    //         s += "\r\n";
    //         s += "<color=#00F0FF>" + dd.funDes + "\r\n" + "</color>";
    //         s += "\r\n";
    //     }
    //     return s;
    // }
}
