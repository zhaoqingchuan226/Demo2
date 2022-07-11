using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//将dataManager中的数据加载到library界面中并且生成相应的卡牌
public class LibraryManager : MonoSingleton<LibraryManager>
{
    public Transform libraryPanel;
    public GameObject cardPrefab;
    public List<GameObject> libraryCards = new List<GameObject>();
    [HideInInspector] public bool isDeleteMode = false;


    void Start()
    {
        StartCoroutine(WaitForNextFrame());
    }
    IEnumerator WaitForNextFrame()
    {
        yield return null;
        // UpdateLibrary();
        yield break;
    }




    // public void UpdateLibrary()
    // {
    //     



    //     //依据playData的顺序生成实例


    // }


    // 依据playData的playerCards来更新Library
    //使用了Temporal
    public void UpdateLibrary()
    {
        //删除上一次
        foreach (var libraryCard in libraryCards)
        {
            Destroy(libraryCard);
        }
        libraryCards.Clear();

        List<Card> cards = new List<Card>();
        cards.AddRange(PlayerData.Instance.playerCards);
        StartCoroutine(InstantiateCards_Temporal(cards));
    }

    IEnumerator InstantiateCards_Temporal(List<Card> cards)
    {
        while (true)
        {
            for (var i = 0; i < 10; i++)//一帧生10个
            {
                if (cards.Count == 0)
                {
                    yield break;
                }
                GameObject card = Instantiate(cardPrefab, libraryPanel);
                card.GetComponent<CardDisplay>().card = cards[0];
                libraryCards.Add(card);
                cards.RemoveAt(0);

            }
            yield return null;

        }
    }
}
