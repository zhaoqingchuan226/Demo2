using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PropDisplay : MonoBehaviour
{
    [HideInInspector] public TextMeshPro titleText;
    [HideInInspector] public TextMeshPro desText;
    public TextMeshPro priceText;
    public Prop prop;
    [HideInInspector] public int posNum;
    GameObject buy_fx;
    [HideInInspector] public GameObject model;
    void Start()
    {
        GameObject slabStone = HolidayStore.Instance.slabStones_pp_obj[posNum];
        titleText = slabStone.transform.Find("Title").gameObject.GetComponent<TextMeshPro>();
        desText = slabStone.transform.Find("Des").gameObject.GetComponent<TextMeshPro>();
        ShowProp();
        buy_fx = transform.Find("buy_fx").gameObject;
    }

    void ShowProp()//将Card中的数据赋予给UI
    {
        //下面if里的代码是防止报错的，不用细看
        //三选一面板被激活的时候，有些剩余的卡还没删除，因此先随便填一个值，后续这些卡牌会被删掉，无所谓
        if (prop == null)
        {
            prop = PropStore.Instance.props[0];
        }
        model =Instantiate(PropStore.Instance.SearchProp(prop.id),this.gameObject.transform);


        titleText.text = prop.title;
        desText.text = prop.description;
        priceText.text = prop.price.ToString();

    }

    public void PlayBuyFx()
    {
        buy_fx.GetComponent<Animator>().SetTrigger("Play");
    }
}
