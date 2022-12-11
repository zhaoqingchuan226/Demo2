using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Card
{
    [HideInInspector]
    public string cardInfor;

    public void AutoDescription()
    {
        this.description = null;
        this.P = 0;
        this.S = 0;
        //自动生成Condi描述文本
        AutoDescription_Condition();

        //自动生成Funcs描述文本
        AutoDescription_Funcs();

        AutoCardInfor();
        // if (this.description != null)
        // {
        //     if (this.description.EndsWith("。"))
        //     {
        //         this.description =
        //             this.description.Substring(0, this.description.Length - 1);
        //     }
        // }
    }

    public void AutoCardInfor()
    {
        string des = null;
        des += "<size=3>" + this.finalTitle.ToString() + "：\r\n" + "</size>";

        des += "\r\n";
        //时间
        des += "出现时间：" + times;
        des += "\r\n";
        des += "\r\n";

        if (creator != null)
        {
            des += "创建者:" + creator;
            des += "\r\n";
            des += "\r\n";
        }
        des += "<color=#00F0FF>" + this.funDes + "\r\n" + "</color>";
        des += "\r\n";
        if (this.description.Contains("连轴"))
        {
            des += "连轴：将一张牌从你的牌库拖入战场，并增加它的收益。如果牌库中没有足够多的卡牌，则无法连轴\r\n";
        }
        if (this.description.Contains("熟练度"))
        {
            des += "熟练度：卡牌每三连一次，即增加一级熟练度\r\n";
        }
        if (this.description.Contains("惊喜"))
        {
            des += "翻译翻译惊喜：惊喜就是下一次周末选卡时，会出现更高等级的卡牌\r\n";
        }
        if (this.description.Contains("自闭"))
        {
            des += "自闭：受欢迎度越低，自闭指数越高\r\n";
        }
        if (this.description.Contains("怠惰"))
        {
            des += "怠惰：该卡牌不会受其他卡牌的buff影响\r\n";
        }
        if (this.description.Contains("宇宙"))
        {
            des += "宇宙：如果场上没有重复的卡牌，则获得一个“宇宙”\r\n";
        }
        if (this.description.Contains("环境卷度"))
        {
            des += "所有人一周生产KPI的平均值";
        }
        if (this.description.Contains("同事"))
        {
            des += "同事是和你一组一起工作的人，将在第二个月出现";
        }
        if (this.description.Contains("开除"))
        {
            des += "当你成为领导后，你就有资格去劝退某些同事";
        }
        cardInfor = des;
    }

    void AutoDescription_Condition()
    {
        if (condition == "1" || condition == "0")
        {
            return;
        }
        else
        {
            string[] elements = condition.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length > 0)
            {
                {
                    float value = float.Parse(elements[3]);
                    value = Mathf.Round(value * 100) / 100f;
                    //效果
                    if (elements[0] == "Ex")
                    {
                        int id = (int)value;
                        if (id == 20013)
                        {
                            this.description += "条件：" + AutoRange(elements[2]) + "卡牌中存在" + AutoSpecialCard(20013) + "卡牌" + "\r\n" + "\r\n";
                        }
                    }
                    else if (elements[0] == "PP")
                    {
                        this.description += "条件：上一周的生命值" + elements[2] + "最大生命值的" + value.ToString() + "倍" + "\r\n" + "\r\n";
                    }
                }
            }
        }
    }

    void AutoDescription_Funcs()
    {
        string des = null;
        // string CardObj = null;
        // string AIObj = null;

        List<string> elements0 = new List<string>();
        List<string> elements1 = new List<string>();
        List<string> elements2 = new List<string>();
        List<float> elements3 = new List<float>();

        Dictionary<string, int> FuncName_Counter_Dic = new Dictionary<string, int>(); //用于检测次方法执行过几次

        //初始操作
        foreach (var func in functions)
        {
            string[] elements = func.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length > 0)
            {
                elements0.Add(elements[0]);
                elements1.Add(elements[1]);
                elements2.Add(elements[2]);
                elements3.Add(float.Parse(elements[3]));
            }
        }

        //D
        foreach (var e in elements0)
        {
            if (e == "D")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des += "摧毁" + AutoRange(elements2[n]);
                des += AutoSpecialCard((int)elements3[n]);
                des += "卡牌" + "\r\n";
            }
        }

        //TD
        foreach (var e in elements0)
        {
            if (e == "TD")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des += "耐久耗尽后摧毁此卡牌";
                des += "(耐久:" + this.life + ")" + "\r\n";
            }
        }

        //T
        foreach (var e in elements0)
        {
            if (e == "T")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des += "此卡牌计数" + AutoValue(elements3[n], "IntOri");
                des += "(计数:" + this.TimeCounter + ")" + "\r\n";
            }
        }

        //P+PA
        Func_P(ref des, elements0, elements1, elements2, elements3, FuncName_Counter_Dic);

        //Px//Pb Pdb
        List<string> funcs_Buff = new List<string>();
        string range_Buff = null;
        foreach (var e in elements0)
        {
            if (e == "Px" || e == "Sx" || e == "Wx" || e == "Kx" || e == "Pb" || e == "Sb" || e == "Wb" || e == "Kb" || e == "Pdb" || e == "Sdb" || e == "Wdb" || e == "Kdb"
            )
            {
                int n = elements0.IndexOf(e); //这边不用加FuncNameCounter(f, FuncName_Counter_Dic)
                funcs_Buff.Add(e);
                if (range_Buff == null)
                {
                    range_Buff = AutoRange(elements2[n]);
                }
            }
        }
        if (funcs_Buff.Count > 0)
        {
            des += "使" + range_Buff + "卡牌";
            foreach (var f in funcs_Buff)
            {
                int n = elements0.IndexOf(f, FuncNameCounter(f, FuncName_Counter_Dic));
                string s = null;
                if (f.Contains("P"))
                {
                    s = "P";
                }
                else if (f.Contains("S"))
                {
                    s = "S";
                }
                else if (f.Contains("W"))
                {
                    s = "W";
                }
                else if (f.Contains("K"))
                {
                    s = "K";
                }
                if (f.Contains("x"))
                {
                    des += AutoPSWK(s) + "数值";
                }
                else if (f.Contains("db"))
                {
                    des += AutoPSWK(s) + "消耗";
                }
                else
                {
                    des += AutoPSWK(s) + "收益";
                }

                des += AutoValue(elements3[n], "FloatMulti");

                if (n != funcs_Buff.Count - 1)
                {
                    des += "，";
                }
            }

            des += "\r\n";
        }

        //C连轴
        List<string> funcs_Connect = new List<string>();
        foreach (var e in elements0)
        {
            if (e == "CP" || e == "CS" || e == "CW" || e == "CK" || e == "CPx" || e == "CSx" || e == "CWx" || e == "CKx"
            )
            {
                funcs_Connect.Add(e);
            }
        }
        if (funcs_Connect.Count > 0)
        {
            des += "连轴：被拖出的卡牌";

            for (var i = 0; i < funcs_Connect.Count; i++)
            {
                int n = elements0.IndexOf(
                    funcs_Connect[i],
                    FuncNameCounter(funcs_Connect[i], FuncName_Counter_Dic)
                );
                string s = null;
                if (funcs_Connect[i].Contains("P"))
                {
                    s = "P";
                }
                else if (funcs_Connect[i].Contains("S"))
                {
                    s = "S";
                }
                else if (funcs_Connect[i].Contains("W"))
                {
                    s = "W";
                }
                else if (funcs_Connect[i].Contains("K"))
                {
                    s = "K";
                }
                des += AutoPSWK(s) + "收益";
                if (funcs_Connect[i].Contains("x"))
                {
                    des += AutoValue(elements3[n], "FloatMulti");
                }
                else
                {
                    des += AutoValue(elements3[n], "IntAdd");
                }

                if (i != funcs_Connect.Count - 1)
                {
                    des += "，";
                }
            }

            des += "\r\n";
        }

        //A
        foreach (var e in elements0)
        {
            if (e == "A")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));

                if (elements1[n] == "Add")
                {
                    des += "使" + Auto_PA_Property(elements2[n]) + AutoValue(elements3[n], "IntAdd");
                }
                else if (elements1[n] == "Mul")
                {
                    des +=
                        "使"
                        + Auto_PA_Property(elements2[n])
                        + AutoValue(elements3[n], "FloatMulti");
                }
                else if (elements1[n] == "Addby")
                {
                    string[] pros = elements2[n].Split(
                        '_',
                        System.StringSplitOptions.RemoveEmptyEntries
                    );
                    // Debug.Log(pros[0]);
                    des +=
                        "使"
                        + Auto_PA_Property(pros[0])
                        + "增加"
                        + AutoValue(elements3[n], "FloatAdd")
                        + "x"
                        + Auto_PA_Property(pros[1]);
                }
                des += "\r\n";
            }
        }

        //TR_UP1
        foreach (var e in elements0)
        {
            if (e == "TR_UP1")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des +=
                    "使"
                    + AutoRange(elements2[n])
                    + AutoSpecialCard((int)elements3[n])
                    + "卡牌随机升级为高一个等级的随机卡牌";
                des += "\r\n";
            }
        }

        //L
        foreach (var e in elements0)
        {
            if (e == "L")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des += "使" + AutoRange(elements2[n]) + "卡牌耐久" + AutoValue(elements3[n], "IntAdd");
                des += "\r\n";
            }
        }

        //U
        foreach (var e in elements0)
        {
            if (e == "U")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des += "使" + AutoRange(elements2[n]) + "卡牌熟练度" + AutoValue(elements3[n], "IntAdd");
                des += "\r\n";
            }
        }

        //I
        foreach (var e in elements0)
        {
            if (e == "I")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des += "将" + elements2[n] + "张" + AutoSpecialCard((int)elements3[n]) + "卡牌";
                des += "洗入你的牌库";
                des += "\r\n";
            }
        }

        //F
        foreach (var e in elements0)
        {
            if (e == "F")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des += "下" + (int)elements3[n] + "次非三连选牌时，会有惊喜发生";
                des += "\r\n";
            }
        }

        //AI_L
        foreach (var e in elements0)
        {
            if (e.Contains("AI_L"))
            {
                string[] ss = e.Split('_', System.StringSplitOptions.RemoveEmptyEntries);
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des += "抢占" + AutoObj(elements1[n]);
                float v = 1 - Mathf.Pow(elements3[n], (1 + addLevelPower * this.addLevel));
                v = Mathf.Round(100 * v) / 100f;
                des += v.ToString() + "倍的" + AutoPSWK(ss[2]);
                des += "\r\n";
            }
        }

        //AI_Px //AI_F
        List<string> funcs_AIBuff = new List<string>();
        string Obj_AIBuff = null;
        foreach (var e in elements0)
        {
            if (
                e == "AI_Px"
                || e == "AI_Sx"
                || e == "AI_Wx"
                || e == "AI_Kx"
                || e == "AI_Pb"
                || e == "AI_Sb"
                || e == "AI_Wb"
                || e == "AI_Kb"
                || e == "AI_Pdb"
                || e == "AI_Sdb"
                || e == "AI_Wdb"
                || e == "AI_Kdb"
                || e == "AI_PMx"
                || e == "AI_F"
            )
            {
                int n = elements0.IndexOf(e); //这边不用加FuncNameCounter(f, FuncName_Counter_Dic)
                funcs_AIBuff.Add(e);
                if (Obj_AIBuff == null)
                {
                    Obj_AIBuff = AutoObj(elements1[n]);
                }
            }
        }
        if (funcs_AIBuff.Count > 0)
        {
            des += "使" + Obj_AIBuff;

            for (var i = 0; i < funcs_AIBuff.Count; i++)
            {
                int n = elements0.IndexOf(
                    funcs_AIBuff[i],
                    FuncNameCounter(funcs_AIBuff[i], FuncName_Counter_Dic)
                );
                string s = null;
                if (funcs_AIBuff[i].Contains("PM"))
                {
                    s = "PM";
                }
                else if (funcs_AIBuff[i].Contains("P"))
                {
                    s = "P";
                }
                else if (funcs_AIBuff[i].Contains("S"))
                {
                    s = "S";
                }
                else if (funcs_AIBuff[i].Contains("W"))
                {
                    s = "W";
                }
                else if (funcs_AIBuff[i].Contains("K"))
                {
                    s = "K";
                }

                if (funcs_AIBuff[i].Contains("x"))
                {
                    des += AutoPSWK(s) + "数值";
                    des += AutoValue(elements3[n], "FloatMulti");
                }
                else if (funcs_AIBuff[i].Contains("db"))
                {
                    des += AutoPSWK(s) + "消耗";
                    des += AutoValue(elements3[n], "FloatMulti");
                }
                else if (funcs_AIBuff[i].Contains("b"))
                {
                    des += AutoPSWK(s) + "收益";
                    des += AutoValue(elements3[n], "FloatMulti");
                }
                else if (funcs_AIBuff[i].Contains("F"))
                {
                    des += AutoPSWK("F");
                    des += AutoValue(elements3[n], "IntAdd");
                }

                if (i != funcs_AIBuff.Count - 1)
                {
                    des += "，";
                }
            }
            des += "\r\n";
        }

        //AI_Rem
        foreach (var e in elements0)
        {
            if (e == "AI_Rem")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                des += "开除" + AutoObj(elements1[n]);
                des += "\r\n";
            }
        }

        //怠惰
        if (this.executeQueue == 18)
        {
            des += "怠惰" + "\r\n";
        }

        //DW
        List<string> funcs_DW = new List<string>();
        Dictionary<string, int> FuncName_Counter_DW_Dic = new Dictionary<string, int>(); //用于检测次方法执行过几次

        foreach (var func in functions)
        {
            if (func.Contains("DW_"))
            {
                string[] ss = func.Split('_', System.StringSplitOptions.RemoveEmptyEntries);
                funcs_DW.Add(ss[1]);
            }
        }
        if (funcs_DW.Count > 0)
        {
            des += "亡语：";
            List<string> DW_elements0 = new List<string>();
            List<string> DW_elements1 = new List<string>();
            List<string> DW_elements2 = new List<string>();
            List<float> DW_elements3 = new List<float>();
            foreach (var func in funcs_DW)
            {
                string[] DW_elements = func.Split(
                    '|',
                    System.StringSplitOptions.RemoveEmptyEntries
                );
                if (DW_elements.Length > 0)
                {
                    DW_elements0.Add(DW_elements[0]);
                    DW_elements1.Add(DW_elements[1]);
                    DW_elements2.Add(DW_elements[2]);
                    DW_elements3.Add(float.Parse(DW_elements[3]));
                }
            }
            Func_P(
                ref des,
                DW_elements0,
                DW_elements1,
                DW_elements2,
                DW_elements3,
                FuncName_Counter_DW_Dic
            );
        }

        //最后的加
        this.description += des;
    }

    void Func_P(
        ref string des,
        List<string> elements0,
        List<string> elements1,
        List<string> elements2,
        List<float> elements3,
        Dictionary<string, int> FuncName_Counter_Dic,
        bool isDW = false
    )
    {
        foreach (var e in elements0)
        {
            string s=null;
            if (e == "P" || e == "S" || e == "W" || e == "K")
            {
                int n = elements0.IndexOf(e, FuncNameCounter(e, FuncName_Counter_Dic));
                if (e == "W" || e == "K")
                {
                    s += AutoPSWK(e); // AutoValue(elements3[n], "IntAdd")

                    if (elements3[n] >= 0)
                    {
                        s += "+" + Mathf.Floor(elements3[n] * (1 + addLevelPower * this.addLevel)).ToString();
                    }
                    else
                    {
                        s += Mathf.Floor(elements3[n]).ToString();
                    }
                }
                else if (e == "P")
                {
                    this.P = (int)Mathf.Floor(elements3[n]);
                }
                else if (e == "S")
                {
                    this.S = (int)Mathf.Floor(elements3[n]);
                }

                foreach (var f in elements0)
                {
                    if (f == "WA" && e == "W" || f == "KA" && e == "K")
                    {
                        s += "，";
                    }
                    if (f == "PA" && e == "P" || f == "SA" && e == "S" || f == "WA" && e == "W" || f == "KA" && e == "K")
                    {
                        int o = elements0.IndexOf(f) + FuncNameCounter(f, FuncName_Counter_Dic);
                        s += "额外增加" + AutoValue(elements3[o], "FloatAdd") + "倍" + Auto_PA_Property(elements2[o]) + "的" + AutoPSWK(e) + Auto_PA_Property_Exist(elements2[o], elements3[o]);
                    }
                }
                if(s!=null)
                {
                    des+=s+"\r\n";
                }
                

            }
        }
    }

    int FuncNameCounter(string e, Dictionary<string, int> FuncName_Counter_Dic)
    {
        if (!FuncName_Counter_Dic.ContainsKey(e))
        {
            FuncName_Counter_Dic.Add(e, 0);
        }
        else
        {
            FuncName_Counter_Dic[e]++;
        }
        return FuncName_Counter_Dic[e];
    }

    string AutoValue(float value, string valueType)
    {
        string s = null;
        float v = 0;
        switch (valueType)
        {
            case "IntAdd":
                v = Mathf.Floor(value * (1 + addLevelPower * this.addLevel));
                break;
            case "FloatAdd":
                v = value * (1 + addLevelPower * this.addLevel);
                v = Mathf.Round(100 * v) / 100f;
                break;
            case "IntOri":
                v = (int)value;
                break;
            case "FloatMulti":
                v = Mathf.Pow(value, (1 + addLevelPower * this.addLevel));
                v = Mathf.Round(100 * v) / 100f;
                break;
            default:
                break;
        }
        if (valueType == "IntAdd" || valueType == "IntOri")
        {
            if (v >= 0)
            {
                s += "+" + v.ToString();
            }
            else
            {
                s += v.ToString();
            }
        }
        else if (valueType == "FloatMulti")
        {
            s += "x" + v.ToString();
        }
        else
        {
            s += v.ToString();
        }

        return s;
    }

    string Auto_PA_Property(string property) //PA的property //A的property
    {
        string s = null;
        s += "<color=#FFB100>";
        switch (property)
        {
            case "LV":
                s += "此卡牌的熟练度";
                break;
            case "LP":
                s += "失去体力的百分比";
                break;
            case "TA":
                s += "场上的疲劳卡牌数";
                break;
            case "NA":
                s += "场上无色卡牌数";
                break;
            case "ET18":
                s += "被“统筹把控”摧毁的卡牌数";
                break;
            case "ET42":
                s += "被“怨恨执行”摧毁的卡牌数";
                break;
            case "W":
                s += "能力";
                break;
            case "PM":
                s += "最大体力值";
                break;
            case "SM":
                s += "最大精力值";
                break;
            case "PO":
                s += "上一周溢出的体力值";
                break;
            case "SO":
                s += "上一周溢出的精力值";
                break;
            case "FA":
                s += "受欢迎度";
                break;
            case "ER":
                s += "环境卷度";
                break;
            case "Fish":
                s += "场上带有“摸鱼”的卡牌数";
                break;
            case "LV5C":
                s += "场上的五级卡牌数";
                break;
            case "FishP":
                s += "喜欢摸鱼的同事数";
                break;
            case "PC":
                s += "牌库总卡牌数";
                break;
            case "T":
                s += "此卡牌的计数";
                break;
            case "Ka1AIKa1":
                s += "KPI最高的同事的KPI";
                break;
            case "KAll":
                s += "上个月所有人KPI总量";
                break;
            case "L":
                s += "此卡牌的耐久";
                break;
            case "DC":
                s += "本局游戏中摧毁的卡牌总数";
                break;
            case "None":
                s += "场上的空格数";
                break;
            case "TC":
                s += "场上卡牌的流派种类数";
                break;
            case "Uni":
                s += "宇宙";
                break;
            case "KN":
                s += "本月所需KPI";
                break;
            default:
                break;
        }
        s += "</color>";
        return s;
    }

    string Auto_PA_Property_Exist(string property, float value) //(已增加15点) //value是倍数
    {
        string s = null;
        s += "<color=#FFB100>";
        switch (property)
        {
            case "LV":
                s += "（当前增加" + (int)(this.addLevel * value) + "）";
                break;
            case "ET18":
                s += "（当前增加" + (int)(FieldManager.Instance.ET18 * value) + "）";
                break;
            case "ET42":
                s += "（当前增加" + (int)(FieldManager.Instance.ET42 * value) + "）";
                break;
            case "W":
                s += "（当前增加" + (int)(PlayerData.Instance.workAbility * value) + "）";
                break;
            case "PM":
                s += "（当前增加" + (int)(PlayerData.Instance.physicalHealthMax * value) + "）";
                break;
            case "SM":
                s += "（当前增加" + (int)(PlayerData.Instance.spiritualHealth * value) + "）";
                break;
            case "PO":
                s += "（当前增加" + (int)(FieldManager.Instance.OverFillP * value) + "）";
                break;
            case "SO":
                s += "（当前增加" + (int)(FieldManager.Instance.OverFillS * value) + "）";
                break;
            case "FA":
                s += "（当前增加" + (int)(PlayerData.Instance.favorAll * value) + "）";
                break;
            case "ER":
                s += "（当前增加" + (int)(Mechanism.Instance.envirRollValue * value) + "）";
                break;
            case "FishP":
                s += "（当前增加" + (int)(FieldManager.Instance.FishPeopleAll * value) + "）";
                break;
            case "PC":
                s += "（当前增加" + (int)(PlayerData.Instance.playerCards.Count * value) + "）";
                break;
            case "T":
                s += "（当前增加" + (int)(this.TimeCounter * value) + "）";
                break;
            case "Ka1AIKa1":
                s += "（当前增加" + (int)(FieldManager.Instance.Ka1AIKa1 * value) + "）";
                break;
            case "KAll":
                s += "（当前增加" + (int)(FieldManager.Instance.KPIAll * value) + "）";
                break;
            case "L":
                s += "（当前增加" + (int)(this.life * value) + "）";
                break;
            case "DC":
                s += "（当前增加" + (int)(FieldManager.Instance.DestroyCard * value) + "）";
                break;
            default:
                break;
        }
        s += "</color>";
        return s;
    }

    string AutoRange(string range)
    {
        if (range == "1")
        {
            return "此";
        }
        else if (range == "2")
        {
            return "同时段的其他";
        }
        else if (range == "3")
        {
            return "下一张";
        }
        else if (range == "4")
        {
            return "同天";
        }
        else if (range == "5")
        {
            return "之后";
        }
        else if (range == "9")
        {
            return "四周";
        }
        else if (range == "25")
        {
            return "二十五宫格范围内";
        }
        else if (range == "100")
        {
            return "所有";
        }
        return "错误";
    }

    void AutoTrimEndString(string ori, string s) //ori是要被切的大段文字，s是要被切除的尾部字符串
    {
        if (ori != null)
        {
            if (ori.EndsWith(s))
            {
                ori = ori.Substring(0, ori.Length - s.ToCharArray().Length);
            }
        }
    }

    string AutoPSWK(string property)
    {
        string s = null;
        switch (property)
        {
            case "P":
                s = "体力";
                break;
            case "S":
                s = "精力";
                break;
            case "W":
                s = "能力";
                break;
            case "K":
                s = "绩效";
                break;
            case "PM":
                s = "最大体力";
                break;
            case "F":
                s = "好感度";
                break;
            default:
                break;
        }
        return s;
    }

    string AutoSpecialCard(int id)
    {
        string s = null;
        if (id == 0) { }
        else if (id < 10000)
        {
            foreach (var card in CardStore.Instance.cards)
            {
                if (card.id == id)
                {
                    s += card.title;
                }
            }
        }
        else if (id < 20000)
        {
            foreach (var card in CardStore.Instance.Colourless_Cards)
            {
                if (card.id == id)
                {
                    s += card.title;
                }
            }
        }
        else //>20000特殊卡牌
        {
            if (id == 20000)
            {
                s += "任一红色";
            }
            else if (id == 20001)
            {
                s += "任一一级红色";
            }
            else if (id == 20012)
            {
                s += "所有无色";
            }
            else if (id == 20013)
            {
                s += "任一无色";
            }
            else if (id == 20021)
            {
                s += "任一比你当前职位等级高一级的卡牌";
            }
            else if (id == 20040)
            {
                s += "任一一位同事的流派卡牌";
            }
        }
        return s;
    }

    string AutoObj(string objName)
    {
        string s = null;
        // if (objName == "M")
        // {
        //     s += "此";
        // }

        if (objName == "AllAI")
        {
            s += "所有同事";
        }
        else if (objName == "R1AI")
        {
            s += "随机1位同事";
        }
        else if (objName == "R2AI")
        {
            s += "随机2位同事";
        }
        else if (objName == "AllAI")
        {
            s += "所有同事";
        }
        else if (objName == "F4AI")
        {
            s += "对你好感度最低同事";
        }
        else if (objName == "W1AI")
        {
            s += "能力最强的同事";
        }
        else if (objName == "K1AI")
        {
            s += "本月KPI最多的同事";
        }
        else if (objName == "K4AI")
        {
            s += "本月KPI最少的同事";
        }
        else if (objName == "P4AI")
        {
            s += "生命值最低的同事";
        }
        return s;
    }

    public void AutoTitle()
    {
        if (this.addLevel > 0)
        {
            this.finalTitle = this.title + "+" + this.addLevel.ToString();
        }
        else
        {
            this.finalTitle = this.title;
        }
    }
}
