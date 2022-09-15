using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//此脚本挂载在card prefab上
//用于将Card中的数据赋予给UI
public partial class CardDisplayPersonalGame : MonoBehaviour
{

    // public TextMeshPro titleText;
    public Material[] mats;
    // public TextMeshProUGUI TimeText;
    // public TextMeshProUGUI descriptionText;

    // public Image BackgourndImage;
    // public Image QualityImage;
    GameObject quality;
    GameObject action;

    public Card card;

    public AnimationCurve anic;//附加物变大的曲线
    public float generateTime = 0.2f;
    float timer = 0;
    public GameObject sphereCenterPos;//球心位置
    [HideInInspector] public GameObject currentAdditive;

    // public Transform flowerPos;
    // [HideInInspector] public GameObject currentFlower;

    Material originActionMat;
    // Material originQualityMat;


    void Start()
    {
        GameObject Chessbase = Mechanism.Instance.roots[card.posNum - 1].Find("Chessbase").gameObject;
        quality = Chessbase.transform.Find("QualityColor").gameObject;
        // originQualityMat = quality.GetComponent<MeshRenderer>().material;
        action = Chessbase.transform.Find("ActionColor").gameObject;
        originActionMat = action.GetComponent<MeshRenderer>().material;
        currentAdditive = null;
        ShowCard();
        if (currentAdditive != null)
        {
            currentAdditive.TryGetComponent<Animator>(out animator);
            Transform t = currentAdditive.transform.Find("PS");
            if (t != null)
            {
                ps =t.gameObject.GetComponent<ParticleSystem>();
            }
        }

    }

    void ShowCard()//将Card中的数据赋予给UI
    {
        // JudgeTitle(card);//根据标点符号换行
        // JudgeFontSize(card);
        JudgeActionColor(card.actionType);
        JudgeQualityColor(card);
        JudgeAdditive(card);
        // JudgeFlower(card);
    }

    public void MatRet()//每次点击开始的时候，重置下方的quality和action mat为默认
    {
        quality.GetComponent<MeshRenderer>().material.color = Color.black;
        action.GetComponent<MeshRenderer>().material = originActionMat;
    }
    // public void JudgeTitle(Card card1)
    // {
    //     string s = card1.title;
    //     if (s.Contains("："))
    //     {
    //         string[] ss = s.Split("：", System.StringSplitOptions.RemoveEmptyEntries);
    //         s = ss[0] + "：" + "\r\n" + ss[1];
    //     }
    //     else if (s.Contains("."))
    //     {
    //         string[] ss = s.Split(".", System.StringSplitOptions.RemoveEmptyEntries);
    //         s = ss[0] + "." + "\r\n" + ss[1];
    //     }
    //     titleText.text = s;
    //     titleText.color = Color.white;
    // }
    // public void JudgeFontSize(Card card1)
    // {
    //     char[] chars = card1.title.ToCharArray();
    //     if (chars.Length < 4)
    //     {
    //         titleText.fontSize = 4f;
    //     }
    //     else if (chars.Length == 4)
    //     {
    //         titleText.fontSize = 2.6f;
    //     }
    //     else if (chars.Length >= 5)
    //     {
    //         titleText.fontSize = 2.4f;
    //     }
    // }
    public void JudgeActionColor(Card.ActionType at)
    {
        if (at == Card.ActionType.Dynamic)
        {
            action.GetComponent<MeshRenderer>().material = mats[0];
        }
        else if (at == Card.ActionType.Static)
        {
            action.GetComponent<MeshRenderer>().material = mats[1];
        }
        else
        {
            action.GetComponent<MeshRenderer>().material = mats[2];
        }
    }

    public void JudgeQualityColor(Card card1)
    {
        Color c;
        switch (card1.qualityLevel)
        {
            case 1:
                c = new Color(210f / 255f, 210f / 255f, 210f / 255f, 1);
                break;
            case 2:
                c = new Color(175f / 255f, 239f / 255f, 96f / 255f, 1);
                break;
            case 3:
                c = new Color(35f / 255f, 130f / 255f, 236f / 255f, 1);
                break;
            case 4:
                c = new Color(179f / 255f, 33f / 255f, 180f / 255f, 1);
                break;
            case 5:
                c = new Color(255f / 255f, 195f / 255f, 50f / 255f, 1);
                break;
            default:
                c = Color.white;
                break;
        }
        quality.GetComponent<MeshRenderer>().material.color = c;
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

    // public void JudgeFlower(Card card1)
    // {
    //     if (currentFlower != null)
    //     {
    //         Destroy(currentFlower);
    //         currentFlower = null;
    //     }
    //     GameObject Flower = CardStore.Instance.SearchFlower_Model(card1.type);
    //     currentFlower = Instantiate(Flower, flowerPos);

    //     // StartCoroutine(Generate());
    // }
    // IEnumerator Generate()
    // {
    //     while (true)
    //     {
    //         timer += Time.deltaTime;
    //         if (timer > generateTime)
    //         {
    //             timer = 0;
    //             currentAdditive.transform.localScale = Vector3.one;
    //             yield break;
    //         }
    //         float factor = anic.Evaluate(timer / generateTime);
    //         currentAdditive.transform.localScale = new Vector3(factor, factor, factor);
    //         yield return null;
    //     }
    // }
}
