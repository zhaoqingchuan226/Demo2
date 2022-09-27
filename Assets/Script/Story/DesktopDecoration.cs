using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//桌面配置的数值在奇奇怪怪的的地方改
public class DesktopDecoration
{
    public int id;
    public string title;

    public int qualityLevel;
    public int maxCount;//最大持有数量
    public string description = "描述的内容";
    public string funDes;//有趣的介绍

    public DesktopDecoration()
    {

    }
    public DesktopDecoration(int id1, string title1, int qualityLevel1, int maxCount1, string description1, string funDes1)
    {
        id = id1;
        title = title1;
        qualityLevel = qualityLevel1;
        maxCount = maxCount1;
        description = description1;
        funDes = funDes1;
    }
    public DesktopDecoration(DesktopDecoration dd)
    {
        id = dd.id;
        title = dd.title;
        qualityLevel = dd.qualityLevel;
        maxCount = dd.maxCount;
        description = dd.description;
        funDes = dd.funDes;
    }


    public void GetDD()//获得桌面配置时触发
    {
        switch (id)
        {
            case 1://招魂仪，数值在Card FieldBuffValue()中改
                FieldManager.Instance.isUnDeath = true;
                break;
            case 2://提神发夹
                FieldManager.Instance.isOverload = true;
                break;
            case 3://血色蔷薇 白天的精力消耗降低，晚上kpi提升
                FieldManager.Instance.isFlower_Overload = true;
                break;
            case 4://禁忌书 月末给牌
                FieldManager.Instance.isBook_Cthugha = true;
                break;
            default:
                break;
        }
    }
}
