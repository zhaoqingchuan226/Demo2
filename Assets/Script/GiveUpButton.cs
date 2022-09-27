using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GiveUpButton : MonoBehaviour, IPointerClickHandler
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        animator.SetTrigger("21");
        Mechanism.Instance.OnClickGiveUpButton();
    }

}

