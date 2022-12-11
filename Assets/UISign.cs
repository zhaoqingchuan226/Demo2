using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public enum ExistModel//指到时存在，还是一直存在
{
    Temporal,
    Lasting
}

public enum UISignType
{
    MapPoint
}

public class UISign : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ExistModel existModel = ExistModel.Temporal;
    public UISignType uISignType = UISignType.MapPoint;
    public GameObject UISign_Obj;
    public TextMeshPro text;

    // void Start()
    // {
    //     if (existModel == ExistModel.Lasting)
    //     {
    //         UISign_Obj.SetActive(true);
    //     }
    // }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (existModel == ExistModel.Temporal)
        {
            switch (uISignType)
            {
                case UISignType.MapPoint:
                    EnterMapPoint();
                    break;
                default:
                    break;
            }
        }

    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (existModel == ExistModel.Temporal)
        {
            switch (uISignType)
            {
                case UISignType.MapPoint:
                    ExitMapPoint();
                    break;
                default:
                    break;
            }
        }
    }
    void EnterMapPoint()
    {
        UISign_Obj.SetActive(true);
        UpdateText();
    }
    void ExitMapPoint()
    {
        UISign_Obj.SetActive(false);
    }




    void UpdateText()
    {
        switch (uISignType)
        {
            case UISignType.MapPoint:
                MapPointSign();
                break;
            default:
                break;
        }
    }
    void MapPointSign()
    {
        MapPoint mp = this.gameObject.GetComponentInParent<MapPoint>();
        text.text = mp.mapPointType.ToString();
        // switch (mp.mapPointType)
        // {
        //     case MapPointType.工作:
        //         text.text = "工作";
        //         break;
        //     case MapPointType.学习:
        //         text.text = "学习";
        //         break;
        //     case MapPointType.娱乐:
        //         text.text = "娱乐";
        //         break;
        //     case MapPointType.未知:
        //         text.text = "未知";
        //         break;
        //     default:
        //         break;
        // }
    }

    public void OpenLastingSign()
    {
        UISign_Obj.SetActive(true);
        MapPoint mp = this.gameObject.GetComponentInParent<MapPoint>();
        text.text = mp.need.des;
    }
}
