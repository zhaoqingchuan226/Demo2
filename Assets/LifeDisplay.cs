using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeDisplay : MonoBehaviour
{
    public Card card;
    public TextMeshProUGUI numText;
    public GameObject LifePanel;
    private void OnEnable()
    {
        if (card != null)
        {
            LifePanel.SetActive(card.life != 0);
            numText.text = card.life.ToString();
        }
        else
        {
           
        }
    }
}
