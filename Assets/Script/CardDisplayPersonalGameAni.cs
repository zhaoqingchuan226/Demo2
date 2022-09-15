using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//此脚本挂载在card prefab上
//用于将Card中的数据赋予给UI
public class CardDisplayPersonalGameAni : MonoBehaviour
{
    public int CardID;
    // public TextMeshPro titleText;
    public Material[] mats;
    // public TextMeshProUGUI TimeText;
    // public TextMeshProUGUI descriptionText;

    // public Image BackgourndImage;
    // public Image QualityImage;
    public float scaleSize = 1f;

    public Card card;



    public AnimationCurve anic_Down;//三选一界面坠落的动画
    public float generateTime = 0.2f;

    public GameObject sphereCenterPos;//球心位置
    [HideInInspector] public GameObject currentAdditive;

    public bool isAutoID = false;//如果是false，需要手动输入id，如果为true，则由程序规定id//前者用于开场动画//后者用于三选一界面
                                 // public Transform flowerPos;
                                 // [HideInInspector] public GameObject currentFlower;
    public bool isDownAni = false;//初始落下的动画
    [HideInInspector] public float delatTime;
    void Start()
    {
        if (isAutoID == false)
        {
            card = CardStore.Instance.SearchCard(CardID);
        }


        currentAdditive = null;
        ShowCard();
        if (isDownAni)
        {
            StartCoroutine(Down());
        }
    }

    void ShowCard()//将Card中的数据赋予给UI
    {
        // JudgeTitle(card);//根据标点符号换行
        // JudgeFontSize(card);

        JudgeAdditive(card);
        // JudgeFlower(card);
    }




    public IEnumerator Delay_JudgeAdditive(Card card1, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        JudgeAdditive(card1);
        yield break;
    }
    public void JudgeAdditive(Card card1)//delayTime是用来缓解一下次生成过多的prefab而造成的卡顿
    {
        if (currentAdditive != null)
        {
            Destroy(currentAdditive);
            currentAdditive = null;
        }
        GameObject additive = CardStore.Instance.SearchModel(card1.id);
        currentAdditive = Instantiate(additive, sphereCenterPos.transform);
        sphereCenterPos.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        if (card1.id > 10000)
        {
            MeshRenderer[] mrs = currentAdditive.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in mrs)
            {
                mr.material.color = Color.red;
            }
        }
        // StartCoroutine(Generate());
    }


    IEnumerator Down(float timer = 0)
    {
        Vector3 destPos = this.transform.position;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > generateTime + delatTime)
            {
                timer = 0;
                transform.localPosition = destPos;
                AudioManager.Instance.PlayClip("fall0");
                Speak();
                yield break;
            }
            float factor = anic_Down.Evaluate((timer - delatTime) / generateTime);
            transform.localPosition = destPos + new Vector3(0.15f * (1 - factor), 0.5f * (1 - factor), 0.15f * (1 - factor));
            yield return null;
        }
    }

    void Speak()
    {
        this.GetComponent<Dialog>().Speak(card.id, "1_3");
    }
}
