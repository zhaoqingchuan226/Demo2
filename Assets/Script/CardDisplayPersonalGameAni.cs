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

    public AnimationCurve anic;//附加物变大的曲线
    public float generateTime = 0.2f;
    float timer = 0;
    public GameObject sphereCenterPos;//球心位置
    [HideInInspector] public GameObject currentAdditive;

    // public Transform flowerPos;
    // [HideInInspector] public GameObject currentFlower;
    void Start()
    {
        card = CardStore.Instance.SearchCard(CardID);
        currentAdditive = null;
        ShowCard();
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
        currentAdditive.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        if (card1.id > 10000)
        {
            MeshRenderer[] mrs = currentAdditive.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in mrs)
            {
                mr.material.color = Color.red;
            }
        }
        StartCoroutine(Generate());
    }


    IEnumerator Generate()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > generateTime)
            {
                timer = 0;
                currentAdditive.transform.localScale = Vector3.one*scaleSize;
                yield break;
            }
            float factor = anic.Evaluate(timer / generateTime);
            currentAdditive.transform.localScale = new Vector3(factor, factor, factor)*scaleSize;
            yield return null;
        }
    }
}
