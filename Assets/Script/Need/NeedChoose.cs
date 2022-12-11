using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NeedChoose : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Mechanism.Instance.UpdateND(this.gameObject.GetComponent<NeedDisplay>().need) ;
        
        TargetChooseManager.Instance.Close();
        MapManager.Instance.OpenMap();
        Mechanism.Instance.EnterPhase(Phase.Map);
        Mechanism.Instance.KPINeed_EveryMonthText.text = Mechanism.Instance.need_ThisMonth.K.ToString();
    }
}
