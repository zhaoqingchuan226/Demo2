using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//将dataManager中的数据加载到library界面中并且生成相应的卡牌
using TMPro;
// public class SlabStone //卡牌下面垫的石头
// {
//     public  int posNum;
//    public  Material mat_Action;
//    public  Material mat_Quality;

// }

public class LibraryManager : MonoSingleton<LibraryManager>
{
    public GameObject libraryPanel;
    public GameObject cardPrefab;
    [HideInInspector] public List<GameObject> libraryCards = new List<GameObject>();
    [HideInInspector] public bool isDeleteMode = false;

    public Transform[] roots = new Transform[15];//一页最多十五张卡牌
    int page = 1;//当前页数
    int pageMax = 1;//最大页数
    [HideInInspector] public bool isOpen = false;


    //SlabStone
    public GameObject StoneAll;//所有石头的大组
    public List<GameObject> slabStones_obj = new List<GameObject>();
    // public List<SlabStone> slabStones;
    public GameObject FishAnchor;
    private void Awake()
    {
        libraryPanel.SetActive(false);
    }

    void CalculatePage()
    {
        page = 1;
        pageMax = (int)Mathf.Ceil(PlayerData.Instance.playerCards.Count / 15f);
    }

    public void TurnPage(bool isLeft)//翻页
    {
        if (isLeft)//往左翻
        {
            if (page > 1)
            {
                page -= 1;
                Creat15Cards((page - 1) * 15, Mathf.Min(page * 15 - 1, PlayerData.Instance.playerCards.Count - 1));
            }
        }
        else//往右翻
        {
            if (page < pageMax)
            {
                page += 1;
                Creat15Cards((page - 1) * 15, Mathf.Min(page * 15 - 1, PlayerData.Instance.playerCards.Count - 1));
            }
        }
    }
    void ClearCards_obj()
    {
        foreach (var libraryCard in libraryCards)
        {
            libraryCard.GetComponent<CardDisplayPersonalGameLibrary>().ClearAll();
            Destroy(libraryCard);
        }
        libraryCards.Clear();
    }
    void Creat15Cards(int first, int last)
    {
        ClearCards_obj();
        StopAllCoroutines();
        StartCoroutine(CreateCards(first, last));
    }

    IEnumerator CreateCards(int first, int last)
    {
        float timer = 0f;
        int i = first;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 0.01f)
            {
                timer = 0f;
                if (i < last + 1)
                {
                    GameObject card = Instantiate(cardPrefab, roots[i - first]);
                    card.GetComponent<CardDisplayPersonalGameLibrary>().card = PlayerData.Instance.playerCards[i];
                    card.GetComponent<CardDisplayPersonalGameLibrary>().posNum = i - first;
                    libraryCards.Add(card);
                    StartCoroutine(card.GetComponent<SimpleEffect>().Fly());
                    i += 1;
                }
                else
                {
                    yield break;
                }
            }
            yield return null;
        }
    }

    public void UpdateLibrary()
    {
        //重置当前页数为1并计算最大页数
        CalculatePage();


    }

    public void O_C_Roots(int n, bool b)//关闭除了n以外的所有pos
    {
        for (var i = 0; i < roots.Length; i++)
        {
            if (i != n)
            {
                roots[i].gameObject.SetActive(b);
            }
        }
    }

    public IEnumerator MoveLibraryPanel(bool isReverse = false)
    {
        Vector3 originPos;
        Vector3 destPos;
        if (!isReverse)
        {
            originPos = new Vector3(3, 0, 0);
            destPos = new Vector3(0.017f, 0, 0);
        }
        else
        {
            destPos = new Vector3(3, 0, 0);
            originPos = new Vector3(0.017f, 0, 0);
        }

        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                if (isReverse)
                {
                    libraryPanel.transform.localPosition = new Vector3(3, 0, 0);
                    LibraryManager.Instance.libraryPanel.SetActive(false);
                }
                else
                {
                    libraryPanel.transform.localPosition = new Vector3(0.017f, 0, 0);
                    Creat15Cards((page - 1) * 15, Mathf.Min(page * 15 - 1, PlayerData.Instance.playerCards.Count - 1));
                }
                yield break;
            }
            float factor = timer / 0.5f;
            libraryPanel.transform.localPosition = Vector3.Lerp(originPos, destPos, factor);
            yield return null;
        }
    }
    // IEnumerator InstantiateCards_Temporal(List<Card> cards)
    // {
    //     while (true)
    //     {
    //         for (var i = 0; i < 10; i++)//一帧生10个
    //         {
    //             if (cards.Count == 0)
    //             {
    //                 yield break;
    //             }
    //             GameObject card = Instantiate(cardPrefab, libraryPanel);
    //             card.GetComponent<CardDisplay>().card = cards[0];
    //             libraryCards.Add(card);
    //             cards.RemoveAt(0);

    //         }
    //         yield return null;

    //     }
    // }
    public void OpenLibrary()
    {
        CardStore.Instance.AutoReDesAllCards();
        StartCoroutine(Mechanism.Instance.ChessBoardMove());
        isOpen = true;
        libraryPanel.SetActive(true);
        UpdateLibrary();
        StartCoroutine(MoveLibraryPanel());
        FishControler.Instance.Open_Close_All();
        FishControler.Instance.Mouth();
        CameraManager.Instance.SetVirtualCam("LibraryCam");
        // StartCoroutine(FishAnchorMove());
    }

    public void CloseLibrary()
    {
        LibraryManager.Instance.isOpen = false;
        ClearCards_obj();
        StartCoroutine(MoveLibraryPanel(true));
        if (!HolidayStore.Instance.isOpen)
        {
            StartCoroutine(Mechanism.Instance.ChessBoardMove(true));
        }

        // LibraryManager.Instance.UpdateLibrary();
        foreach (var card in PlayerData.Instance.playerCards)
        {
            card.isNew = false;
        }

        FishControler.Instance.Open_Close_All();
        FishControler.Instance.Mouth();
        if (HolidayStore.Instance.isOpen)
        {
            CameraManager.Instance.SetVirtualCam("ChessCam");
        }
        else
        {
            CameraManager.Instance.SetVirtualCam("BlackCam");//可能会出问题
        }

        TeachManager.Instance.SetGuide(this.gameObject, false);
    }

    // public IEnumerator FishAnchorMove()
    // {
    //     float timer = 0;
    //     Vector3 originPos = FishAnchor.transform.localPosition + new Vector3(0, 3, 0);
    //     Vector3 destPos = FishAnchor.transform.localPosition;
    //     while (true)
    //     {
    //         if (timer > 0.7f)
    //         {
    //             FishAnchor.transform.localPosition = destPos;
    //             yield break;
    //         }
    //         timer += Time.deltaTime;
    //         float lerpFactor = timer / 0.7f;
    //         FishAnchor.transform.localPosition = Vector3.Lerp(originPos, destPos, lerpFactor);
    //         yield return null;
    //     }
    // }
}
