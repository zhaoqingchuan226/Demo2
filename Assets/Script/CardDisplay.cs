using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//此脚本挂载在card prefab上
//用于将Card中的数据赋予给UI
public class CardDisplay : MonoBehaviour
{

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI descriptionText;
    public Image BackgourndImage;
    public Image QualityImage;
    public Card card;
    public bool isLibrary = false;
    public GameObject newLight;//新卡牌特有的发光;
    public Image cardTexture;
    public Image FlowerTexture;
    void Start()
    {
        ShowCard();
    }
    void OnEnable()
    {
        ShowCard();
    }

    void ShowCard()//将Card中的数据赋予给UI
    {
        //下面if里的代码是防止报错的，不用细看
        //三选一面板被激活的时候，有些剩余的卡还没删除，因此先随便填一个值，后续这些卡牌会被删掉，无所谓
        if (card == null)
        {
            card = CardStore.Instance.cards[0];
        }


        gameObject.TryGetComponent<LifeDisplay>(out LifeDisplay lifeDisplay);
        if (lifeDisplay != null)
        {
            lifeDisplay.card = this.card;
        }

        titleText.text = card.finalTitle;
        TimeText.text = card.times;
        descriptionText.text = card.description;
        BackgourndImage.color = card.actionColor;
        QualityImage.color = card.qualityColor;
        if (isLibrary)
        {
            newLight.SetActive(card.isNew);
        }
        cardTexture.sprite = CardStore.Instance.SearchTexture(card.id);
        if (card.id > 10000)
        {
            cardTexture.color = Color.red;
        }

        if (FlowerTexture != null)
        {
            FlowerTexture.sprite = CardStore.Instance.SearchFlower_Spirite(card.type);
        }
    }

}
