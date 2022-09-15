using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//如果已经有了两张相同卡牌，则会在三选一界面提示玩家选择三连卡牌
public class TripletFlash : MonoBehaviour
{
    // Start is called before the first frame update
    bool isFlash = false;//如果你差一张三连，那么就会闪
    // Color origincolor;
    // public Color newColor;
    // float timer = 0;
    public float speed = 1f;

    Card card;
    public GameObject effect;//蛊惑三连的特效
    void Start()
    {
        // Transform T = this.gameObject.transform.Find("Background_Action");
        // Background_Action = T.gameObject.GetComponent<Image>();
        // mat = this.gameObject.GetComponent<CardDisplayPersonalGameAni>().currentAdditive.GetComponent<MeshRenderer>().material;
        // origincolor = mat.color;

        card = this.gameObject.GetComponent<CardDisplayPersonalGameLibrary>().card;

        if (card != null)
        {
            // timer = 0;
            int n = 0;
            foreach (var card2 in PlayerData.Instance.playerCards)
            {
                if (card2.id == card.id)
                {
                    n++;
                }
            }
            if (n == 2)
            {
                if (TeachManager.Instance.isFirstTriplet3_1 == true)
                {
                    TeachManager.Instance.TeachEventTrigger("选牌三连蛊惑");
                    TeachManager.Instance.isFirstTriplet3_1 = false;
                }
                isFlash = true;
            }
            else
            {
                isFlash = false;
            }
        }
        if (isFlash)
        {
            effect.SetActive(true);
        }
    }

    // void OnEnable()
    // {

    // }

    // Update is called once per frame
    // void Update()
    // {
    //     if (isFlash)
    //     {
    //         // timer += Time.deltaTime;
    //         // float factor = 0.5f * Mathf.Sin(timer * speed) + 0.5f;
    //         // mat.color = Color.Lerp(origincolor, newColor, factor);
    //     }
    // }
}
