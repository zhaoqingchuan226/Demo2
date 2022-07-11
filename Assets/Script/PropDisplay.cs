using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PropDisplay : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public Prop prop;

    void Start()
    {
        ShowProp();
    }

    void ShowProp()//将Card中的数据赋予给UI
    {
        //下面if里的代码是防止报错的，不用细看
        //三选一面板被激活的时候，有些剩余的卡还没删除，因此先随便填一个值，后续这些卡牌会被删掉，无所谓
        if (prop == null)
        {
            prop = PropStore.Instance.props[0];
        }



        titleText.text = prop.title;
        descriptionText.text = prop.description;
        priceText.text = prop.price.ToString();

    }
}
