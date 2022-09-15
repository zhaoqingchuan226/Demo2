using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//工作日UI被buff发生的特效
public class SimpleEffectUI : MonoBehaviour
{

    float timer_Bebuffed;
    float timer_Buff;
  
    float scaleSize = 1.3f;
    float scaleTime = 0.5f;
    Vector3 originLocalScale;
    public GameObject Card_Fly;
    [HideInInspector] public bool isBebuffed = false;
    void Start()
    {
        originLocalScale = transform.localScale;
    }


    // public IEnumerator Buff()//buff别人会变色
    // {
    //     while (true)
    //     {
    //         // TextMeshPro tmp = GetComponentInChildren<TextMeshPro>();

    //         if (timer_Buff > scaleTime)
    //         {
    //             timer_Buff = 0;
    //             mat.color = originMatColor;
    //             isBuff = false;
    //             yield break;
    //         }
    //         isBuff = true;
    //         timer_Buff += Time.deltaTime;
    //         float lerpFactor = -Mathf.Abs(2 / scaleTime * timer_Buff - 1) + 1;
    //         // lerpFactor=Mathf.Clamp01(lerpFactor*2); 
    //         mat.color = Color.Lerp(originMatColor, Color.green, lerpFactor);
    //         yield return null;
    //     }
    // }


    public IEnumerator BeBuffed()//被buff会变大
    {
        while (true)
        {
            if (timer_Bebuffed > scaleTime)
            {
                timer_Bebuffed = 0;
                gameObject.transform.localScale = originLocalScale;
                isBebuffed = false;
                yield break;
            }
            isBebuffed = true;
            timer_Bebuffed += Time.deltaTime;
            float n = -Mathf.Abs((2 * scaleSize - 2) / scaleTime * timer_Bebuffed + 1 - scaleSize) + scaleSize;
            gameObject.transform.localScale = new Vector3(n * originLocalScale[0], n * originLocalScale[1], n * originLocalScale[2]);

            yield return null;
        }

    }

    // public void InstantiateCard(Card kidCard, float delayTime)
    // {
    //     GameObject card_Fly = Instantiate(Card_Fly, Mechanism.Instance.InstantiateCardsGroup.transform);
    //     card_Fly.GetComponent<CardDisplay>().card = kidCard;
    //     // card_Fly.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
    //     card_Fly.transform.position = this.transform.position;
    //     Vector3 pos = card_Fly.transform.position;
    //     pos.y += 160;
    //     card_Fly.transform.position = pos;
    //     Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    //     dir.Normalize();
    //     Mechanism.Instance.StartCoroutine(card_Fly.GetComponent<CardFly>().Fly(Mechanism.Instance.LibraryButton.transform.position, delayTime, 0.8f, dir));
    // }
}
