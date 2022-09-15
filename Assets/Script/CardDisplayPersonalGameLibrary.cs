using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//此脚本挂载在card prefab上
//用于将Card中的数据赋予给UI
public enum CardPG_Type
{
    Libaray,
    HolidayStore,
    Choose1_3
}
public class CardDisplayPersonalGameLibrary : MonoBehaviour
{
    public CardPG_Type cardPG_Type;
    public Material[] mats;

    public float scaleSize = 1f;

    public Card card;

    public float generateTime = 0.2f;

    public GameObject sphereCenterPos;//球心位置
    [HideInInspector] public int posNum;
    [HideInInspector] public GameObject currentAdditive;
    GameObject quality;
    GameObject action;
    TextMeshPro titleText;
    TextMeshPro desText;
    GameObject info_obj;
    TextMeshPro infoText;
    public GameObject NEW;
    // public bool isHolidayStore = false;//是否是商店中的卡牌
    GameObject buy_fx;
    [HideInInspector] public float delatTime;//1_3专用
    public AnimationCurve anic_Down;//三选一界面坠落的动画
    public bool isDownAni = false;//初始落下的动画
    void Start()
    {
        currentAdditive = null;
        GameObject slabStone = null;
        if (card == null)
        {
            card = CardStore.Instance.SearchCard(1);
        }
        if (cardPG_Type == CardPG_Type.HolidayStore)
        {
            slabStone = HolidayStore.Instance.slabStones_obj[posNum];
            buy_fx = transform.Find("buy_fx").gameObject;
        }
        else if (cardPG_Type == CardPG_Type.Libaray)
        {
            slabStone = LibraryManager.Instance.slabStones_obj[posNum];
        }
        else if (cardPG_Type == CardPG_Type.Choose1_3)
        {
            slabStone = ThreeChooseOneManager.Instance.slabStones_obj[posNum];
        }


        action = slabStone.transform.Find("ShortSlabStone_2").gameObject;
        quality = slabStone.transform.Find("ShortSlabStone_3").gameObject;
        titleText = slabStone.transform.Find("Title").gameObject.GetComponent<TextMeshPro>();
        desText = slabStone.transform.Find("Des").gameObject.GetComponent<TextMeshPro>();
        info_obj = slabStone.transform.Find("LongSlabStone").gameObject;
        infoText = info_obj.transform.Find("FunDes").gameObject.GetComponent<TextMeshPro>();
        ShowCard();

        if (isDownAni)
        {
            StartCoroutine(Down());
        }
    }
    public void PlayBuyFx()
    {
        buy_fx.GetComponent<Animator>().SetTrigger("Play");
    }
    void ShowCard()//将Card中的数据赋予给UI
    {
        JudgeActionColor(card.actionType);
        JudgeQualityColor(card);
        JudgeAdditive(card);
        titleText.text = card.finalTitle;
        desText.text = card.description;
        infoText.text = card.cardInfor;
        if (cardPG_Type == CardPG_Type.Libaray)
        {
            NEW.SetActive(card.isNew);
        }

    }
    public void ShowCardInfo(bool b)
    {
        info_obj.SetActive(b);
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
        // currentAdditive.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
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

    public void ClearAll()
    {
        titleText.text = null;
        desText.text = null;
        quality.GetComponent<MeshRenderer>().material.color = Color.white;
        action.GetComponent<MeshRenderer>().material = mats[2];
    }

    IEnumerator Down(float timer = 0)
    {
        Vector3 destPos = Vector3.zero;
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

    void OnDestroy()
    {
        PronounceCore.Instance.StopAllAudio();
    }
}
