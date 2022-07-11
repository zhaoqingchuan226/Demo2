using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

//这个单例是用来统计各种奇奇怪怪的，会被卡牌的condi或者func用到的属性的
public class FieldManager : MonoSingleton<FieldManager>
{
    [HideInInspector]
    public bool isTripletReward = false;

    [HideInInspector]
    public int luckyTimes = 0; //幸运的次数,如果>0，那么本周末的三选一面板中必有三张下一等级的卡牌//由func F 开启

    [HideInInspector]
    public int TiredAll = 0; //场上总疲劳数

    [HideInInspector]
    public int NoColorAll = 0; //场上总无色卡数

    [HideInInspector]
    public int NoneAll = 20; //场上总空格数

    [HideInInspector]
    public int TypeCount = 0; //场上卡牌流派数

    [HideInInspector]
    public int universe = 0; //宇宙加成，数值为1时则表面是宇宙

    [HideInInspector]
    public int FishAll = 0; //场上带有“摸鱼”关键词的卡牌数

    public int Lv5C = 0; //场上质量等级5级的卡牌数

    [HideInInspector]
    public int FishPeopleAll = 0; //场上带有“摸鱼”关键词的卡牌数

    [HideInInspector]
    public int ET18 = 0; //id18的统筹把控卡牌总共吃掉过多少一级红卡牌
    public int ET42 = 0; //id42的怨恨执行卡牌总共吃掉过多少一级红卡牌
    public int DestroyCard = 0;//摧毁的卡牌数


    [HideInInspector]
    public int OverFillP = 0; //每回合P的溢出

    [HideInInspector]
    public int OverFillS = 0; //每回合S的溢出

    [HideInInspector] public int Ka1AIKa1 = 0;//创造KPI的能力 第一的AI的 创造KPI的能力
    [HideInInspector] public int KPIAll = 0;//上一月KPI的总量

    [HideInInspector] public List<AIData> AIDatas_Remove = new List<AIData>();//需要抹除的AIData

}
