using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
//点击购买prop的脚本
public class HolidayStoreProp : MonoBehaviour, IPointerClickHandler
{
    bool isSell = false;
    public GameObject haveSold;


    void Start()
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        PropDisplay pd = this.gameObject.GetComponent<PropDisplay>();
        int price = pd.prop.price;
        if (!isSell && PlayerData.Instance.playerMoney >= price)
        {
            LACControl.Instance.ReduceCoin(price);
            AudioManager.Instance.PlayClip("Ding");
            isSell = true;
            pd.priceText.text = "已出售";
            pd.priceText.color = Color.red;
            pd.PlayBuyFx();
            Effect();
            haveSold.SetActive(true);
        }
        else if (!isSell && PlayerData.Instance.playerMoney < price)
        {
            Mechanism.Instance.SignAll_Update("你没有财富啊没有财富");
        }

    }
    public void Effect()
    {
        Prop prop = this.gameObject.GetComponent<PropDisplay>().prop;
        switch (prop.id)
        {
            case 1:
                PlayerData.Instance.ChangeProperty("PM", 10);
                break;
            case 2:
                PlayerData.Instance.ChangeProperty("P", 50);
                break;
            case 3:
                PlayerData.Instance.ChangeProperty("SM", 10);
                break;
            case 4:
                PlayerData.Instance.ChangeProperty("SM", 10);
                break;
            case 5:
                foreach (var AI_Cha in AIMechanism.Instance.AI_Chas)
                {
                    AIData AIData = AI_Cha.AIData;
                    if (AIData != null)
                    {
                        AIData.favor += 10;
                        // AIMechanism.Instance.favorTexts[AIData.AIid].text = AIData.favor.ToString();
                    }
                }
                break;
            case 6:
                PlayerData.Instance.ChangeProperty("W", 50);
                break;
            case 7:
                Mechanism.Instance.week = Mathf.Max(1, Mechanism.Instance.week - 1);
                Mechanism.Instance.weekText.text = Mechanism.Instance.week.ToString();
                break;
            case 8:
                Mechanism.Instance.KPINeed_EveryMonth = Mathf.Max(0, Mechanism.Instance.KPINeed_EveryMonth - 100);
                Mechanism.Instance.KPINeed_EveryMonthText.text = Mechanism.Instance.KPINeed_EveryMonth.ToString();
                break;
            case 9:
                List<Card> cardsAll = new List<Card>();
                cardsAll.AddRange(PlayerData.Instance.playerCards);
                foreach (var card in cardsAll)
                {
                    if (card.finalTitle.Contains("摸鱼"))
                    {
                        Mechanism.Instance.PromoteCards(card.id, 1);
                    }
                }
                break;
            case 10:
                PlayerData.Instance.KPILife += 1;
                PlayerData.Instance.UpdateKPILife();
                break;

            default:
                break;
        }
        Mechanism.Instance.SignAll_Update(prop.description);
    }




}

