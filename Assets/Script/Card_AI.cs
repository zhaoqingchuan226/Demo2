using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class Card
{

    public List<AIData> AIDatas_Debuff = new List<AIData>();//要施加debuff的AI们
    public float r_thisTur = 0;
    // [HideInInspector] public List<List<Action>> actions = new List<List<Action>>() { new List<Action>(), new List<Action>(), new List<Action>(), new List<Action>() };//四个AI的行为大全
    // [HideInInspector] public List<PSWK_Debuff> PSWK_Debuff_Buffer = new List<PSWK_Debuff> { new PSWK_Debuff(), new PSWK_Debuff(), new PSWK_Debuff(), new PSWK_Debuff() };//四个AI的PSWK修改


    public string creator = null;//AI发送给玩家debuff时，用来记录是谁发给玩家的
    void ExecuteFunctionAI(string[] elements)
    {
        // List<AIData> AIDatas = new List<AIData>();
        string funcName = elements[0];
        string range = elements[1];
        float value = float.Parse(elements[3]);//值
        // int range = int.Parse(elements[2]);//范围
        // range = Mathf.Clamp(range, 0, Mechanism.Instance.week * 3 + 2);//第一周不能改变某个对象的九个行为，因为第一周不可能有九个行为
        if (this.AIDatas_Debuff.Count == 0)//如果没有要祸害的AI，则填充
        {
            r_thisTur = Random.Range(0, 1);
            List<AIData> AIDatas_Copy = new List<AIData>();//浅拷贝
            foreach (var AI_Cha in AIMechanism.Instance.AI_Chas)
            {
                AIData AIData = AI_Cha.AIData;
                if (AIData != null)
                {
                    AIDatas_Copy.Add(AIData);
                }
            }
            if (range == "R1AI")
            {
                if (AIDatas_Copy.Count > 0)
                {
                    AIData AIData_Random = AIDatas_Copy[Random.Range(0, AIDatas_Copy.Count)];
                    this.AIDatas_Debuff.Add(AIData_Random);
                    AIDatas_Copy.Remove(AIData_Random);
                }

            }
            if (range == "R2AI")
            {
                for (int i = 0; i < 2; i++)
                {
                    if (AIDatas_Copy.Count > 0)
                    {
                        AIData AIData_Random = AIDatas_Copy[Random.Range(0, AIDatas_Copy.Count)];
                        this.AIDatas_Debuff.Add(AIData_Random);
                        AIDatas_Copy.Remove(AIData_Random);
                    }
                }
            }
            if (range == "AllAI")
            {
                this.AIDatas_Debuff.AddRange(AIDatas_Copy);
            }
            if (range == "F4AI")//KPI最低的AI
            {
                if (AIDatas_Copy.Count > 0)
                {

                    AIData AIData_Temp = AIDatas_Copy[0];

                    foreach (var AIData in AIDatas_Copy)
                    {
                        if (AIData_Temp.favor > AIData.favor)
                        {
                            AIData_Temp = AIData;
                        }
                    }
                    this.AIDatas_Debuff.Add(AIData_Temp);
                }
            }
            if (range == "W1AI")//工作能力最强的AI
            {
                if (AIDatas_Copy.Count > 0)
                {
                    AIData AIData_Temp = AIDatas_Copy[0];
                    foreach (var AIData in AIDatas_Copy)
                    {
                        if (AIData_Temp.workAbility < AIData.workAbility)
                        {
                            AIData_Temp = AIData;
                        }
                    }
                    this.AIDatas_Debuff.Add(AIData_Temp);
                }
            }
            if (range == "K1AI")//本月KPI第一的AI
            {
                if (AIDatas_Copy.Count > 0)
                {
                    AIData AIData_Temp = AIDatas_Copy[0];
                    foreach (var AIData in AIDatas_Copy)
                    {
                        if (AIData_Temp.KPI < AIData.KPI)
                        {
                            AIData_Temp = AIData;
                        }
                    }
                    this.AIDatas_Debuff.Add(AIData_Temp);
                }
            }
            if (range == "K4AI")//本月KPI最后的AI
            {
                if (AIDatas_Copy.Count > 0)
                {
                    AIData AIData_Temp = AIDatas_Copy[0];
                    foreach (var AIData in AIDatas_Copy)
                    {
                        if (AIData_Temp.KPI > AIData.KPI)
                        {
                            AIData_Temp = AIData;
                        }
                    }
                    this.AIDatas_Debuff.Add(AIData_Temp);
                }
            }
            if (range == "P4AI")//生命值最低的AI
            {
                if (AIDatas_Copy.Count > 0)
                {
                    AIData AIData_Temp = AIDatas_Copy[0];
                    foreach (var AIData in AIDatas_Copy)
                    {
                        if (AIData_Temp.physicalHealth > AIData.physicalHealth)
                        {
                            AIData_Temp = AIData;
                        }
                    }
                    this.AIDatas_Debuff.Add(AIData_Temp);
                }
            }
        }

        if (AIDatas_Debuff.Count > 0)
        {
            isEffect = true;
            if (funcName == "AI_Px" || funcName == "AI_Sx" || funcName == "AI_Wx" || funcName == "AI_Kx"
            || funcName == "AI_Pb" || funcName == "AI_Sb" || funcName == "AI_Wb" || funcName == "AI_Kb"
            || funcName == "AI_Pdb" || funcName == "AI_Sdb" || funcName == "AI_Wdb" || funcName == "AI_Kdb"
            || funcName == "AI_F"//影响AI好感度
            || funcName == "AI_L_K"//掠夺AIKPI
            || funcName == "AI_PMx"//修改AI体力最大值
            || funcName == "AI_Rem"
            )
            {
                BuffDebuffMulti_AI(funcName, AIDatas_Debuff, value);
            }
        }
    }


    void BuffDebuffMulti_AI(string funcName, List<AIData> AIDatas_Debuff, float value)
    {
        foreach (var AIData in AIDatas_Debuff)
        {
            bool b = false;


            if (r_thisTur < AIData.Def_Ability / 100f && this.type == Type.Snaky)
            {
                // SimpleEffect se = AIMechanism.Instance.names[AIData.AIid].GetComponentInParent<SimpleEffect>();
                // if (!se.isBuff)
                // {
                //     se.StartCoroutine(se.Buff());
                // }
                // if (!se.isAIDef)
                // {
                //     se.StartCoroutine(se.AIDef());
                // }


            }
            else
            {
                switch (funcName)
                {
                    case "AI_Px":
                        if (AIData.buffer.P != 0)
                        {
                            AIData.buffer.P = (int)(AIData.buffer.P * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Sx":
                        if (AIData.buffer.S != 0)
                        {
                            AIData.buffer.S = (int)(AIData.buffer.S * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Wx":
                        if (AIData.buffer.W != 0)
                        {
                            AIData.buffer.W = (int)(AIData.buffer.W * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Kx":
                        if (AIData.buffer.K != 0)
                        {
                            AIData.buffer.K = (int)(AIData.buffer.K * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Pb":
                        if (AIData.buffer.P > 0)
                        {
                            AIData.buffer.P = (int)(AIData.buffer.P * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Sb":
                        if (AIData.buffer.S > 0)
                        {
                            AIData.buffer.S = (int)(AIData.buffer.S * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Wb":
                        if (AIData.buffer.W > 0)
                        {
                            AIData.buffer.W = (int)(AIData.buffer.W * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Kb":
                        if (AIData.buffer.K > 0)
                        {
                            AIData.buffer.K = (int)(AIData.buffer.K * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Pdb":
                        if (AIData.buffer.P < 0)
                        {
                            AIData.buffer.P = (int)(AIData.buffer.P * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Sdb":
                        if (AIData.buffer.S < 0)
                        {
                            AIData.buffer.S = (int)(AIData.buffer.S * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Wdb":
                        if (AIData.buffer.W < 0)
                        {
                            AIData.buffer.W = (int)(AIData.buffer.W * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_Kdb":
                        if (AIData.buffer.K < 0)
                        {
                            AIData.buffer.K = (int)(AIData.buffer.K * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_F":
                        AIData.favor += (int)(value * (1 + 0.2f * this.addLevel));
                        b = true;
                        break;
                    case "AI_L_K":
                        if (AIData.buffer.K > 0)
                        {
                            int LootValue = (int)(AIData.buffer.K * (1 - Mathf.Pow(value, (1 + 0.2f * this.addLevel))));
                            this.functionEffectEx.KPIAdd += LootValue;
                            AIData.buffer.K = (int)(AIData.buffer.K * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                            b = true;
                        }
                        break;
                    case "AI_PMx":
                        AIData.physicalHealthMax = (int)(AIData.physicalHealthMax * Mathf.Pow(value, (1 + 0.2f * this.addLevel)));
                        AIData.physicalHealth = Mathf.Min(AIData.physicalHealth, AIData.physicalHealthMax);
                        b = true;
                        break;
                    case "AI_Rem":
                        FieldManager.Instance.AIDatas_Remove.Add(AIData);
                        b = true;
                        break;
                    default:
                        break;
                }
                // }
                if (b)
                {
                    // SimpleEffect se1 = Mechanism.Instance.cards_cardPersonalGamePrefabs_Dic[this].GetComponent<SimpleEffect>();
                    // if (!se1.isBuff)
                    // {
                    //     se1.StartCoroutine(se1.Buff());
                    // }

                    // GameObject NPC_Name_Gameobj = AIMechanism.Instance.names_W_Gameobject[AIData.AIid];
                    // SimpleEffect se2 = NPC_Name_Gameobj.GetComponent<SimpleEffect>();
                    // if (!se2.isBebuffed)
                    // {
                    //     se1.StartCoroutine(se2.BeBuffed());
                    // }

                }
            }

        }


    }
    /*
    void AddAction(AIData AIData, PSWK_Debuff PSWK_Debuff)
    {
        int index = AIData.PSWK_Debuffs.IndexOf(PSWK_Debuff);
        if (!actions[AIData.AIid].Contains(AIData.actions[index]))
        {
            actions[AIData.AIid].Add(AIData.actions[index]);
        }
    }
    */
}


