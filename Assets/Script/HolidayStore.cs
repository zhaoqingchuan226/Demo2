using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolidayStore : MonoSingleton<HolidayStore>
{
    public GameObject backGround;
    public int cardAmount = 3;
    public GameObject cardPrefab;
    List<GameObject> cardPrefabs = new List<GameObject>();


    public int propAmount = 2;
    public GameObject propPrefab;
    List<GameObject> propPrefabs = new List<GameObject>();
    public GameObject delete;

    public List<int> value_EveryLevel = new List<int>();//每一等级卡牌的价格

    public int deletePrice = 3000;

    public Transform[] roots_cards = new Transform[6];
    public Transform[] roots_props = new Transform[3];
    public List<GameObject> slabStones_obj = new List<GameObject>();
    public List<GameObject> slabStones_pp_obj = new List<GameObject>();
    [HideInInspector] public bool isOpen = false;

    public GameObject fish;//吃卡牌的鱼;
    public Dialog dialog_business;//商人的对话
    public Dialog dialog_fish;//鱼的对话
    float timer_business;//每隔一段时间触发对话

    float timer_All;//开店的总时长


    private void Update()
    {
      
        if (isOpen)
        {

            timer_business += Time.deltaTime;
            timer_All += Time.deltaTime;
            if (timer_business > 5f)
            {
                timer_business = 0;
                float r = Random.Range(0, 1f);

                if (timer_All > 20f)
                {
                    if (r < 0.15f)
                    {
                        dialog_business.SetDiaglog("点我，可以退出周末商店");
                        return;
                    }
                    else if (r < 0.3f)
                    {
                        dialog_business.SetDiaglog("如果没有别的需求，可以点击我退出商店");
                        return;
                    }
                    else if (r < 0.45f)
                    {
                        dialog_business.SetDiaglog("如果不打算购物了，点我就可以离开");
                        return;
                    }
                    else if (r < 0.6f)
                    {
                        dialog_fish.SetDiaglog("你买不买，我都快干了！");
                        return;
                    }
                }

                //鱼部分
                if (PlayerData.Instance.playerMoney < 3000)
                {
                    if (r < 0.15f)
                    {
                        dialog_fish.SetDiaglog("你还买不起我！");
                        return;
                    }
                    else if (r < 0.3f)
                    {
                        dialog_fish.SetDiaglog("贫穷的家伙，三千都拿不出来！");
                        return;
                    }
                }
                else
                {
                    if (r < 0.15f)
                    {
                        dialog_fish.SetDiaglog("只要三千，我就帮你吃掉一张牌！");
                        return;
                    }
                    else if (r < 0.3f)
                    {
                        dialog_fish.SetDiaglog("就三千，我来消灭你的卡牌！");
                        return;
                    }
                }



                //商人部分

                if (PlayerData.Instance.playerMoney < 1000)
                {
                    if (r < 0.2f)
                    {
                        dialog_business.SetDiaglog("穷鬼逛什么周末商店");
                    }
                    else if (r < 0.4f)
                    {
                        dialog_business.SetDiaglog("你没钱，周末的快乐时光不属于你");
                    }
                    else if (r < 0.6f)
                    {
                        dialog_business.SetDiaglog("真是穷得叮当响");
                    }
                    else if (r < 0.8f)
                    {
                        dialog_business.SetDiaglog("主也为你的贫困而惊呼");
                    }
                }
                else if (PlayerData.Instance.playerMoney < 10000)
                {
                    float rr = Random.Range(0, 1f);
                    if (r < 0.2f)
                    {
                        dialog_business.SetDiaglog("欢迎光临周末商店");
                    }
                    else if (r < 0.4f)
                    {
                        dialog_business.SetDiaglog("工作累了吧，来放松放松");
                    }
                    else if (r < 0.6f)
                    {
                        dialog_business.SetDiaglog("邪书禁咒，应有尽有");
                    }
                    else if (r < 0.8f)
                    {
                        dialog_business.SetDiaglog("为主的财富自由出一份力吧");
                    }
                }
                else if (PlayerData.Instance.playerMoney >= 10000)
                {
                    if (r < 0.2f)
                    {
                        dialog_business.SetDiaglog("贵客到来，有失远迎！");
                    }
                    else if (r < 0.4f)
                    {
                        dialog_business.SetDiaglog("慷慨的客人，愿主保佑您！");
                    }
                    else if (r < 0.6f)
                    {
                        dialog_business.SetDiaglog("敬请挑选，本店不打烊不关门");
                    }
                    else if (r < 0.8f)
                    {
                        dialog_business.SetDiaglog("祝您购物愉快");
                    }
                }
            }
        }
    }

    public void FishEatCard()
    {
        Cursor.SetCursor(default, Vector2.zero, CursorMode.Auto);//复原鼠标
        dialog_fish.SetDiaglog("啊！吃得好饱！");
        AudioManager.Instance.PlayClip("Eat_chess");
    }

    public void FishRefuse()
    {
        dialog_fish.SetDiaglog("你请不起我！");
    }

    public void FishHaveEaten()
    {
        dialog_fish.SetDiaglog("我吃饱了，下周再来找我！");
    }

    public void OpenStore()
    {
        if (!isOpen)
        {
            timer_business = 4f;
            CameraManager.Instance.SetVirtualCam("ChessCam");
            // Mechanism.Instance.ForceChessboardOut(true);
            StartCoroutine(Mechanism.Instance.ChessBoardMove());
            isOpen = true;
            backGround.SetActive(true);
            if (Mechanism.Instance.week > 8)
            {
                cardAmount = 6;
                propAmount = 3;
            }

            // Debug.Log(1);
            foreach (var cp in cardPrefabs)
            {
                Destroy(cp);
            }
            cardPrefabs.Clear();
            foreach (var pp in propPrefabs)
            {
                Destroy(pp);
            }
            propPrefabs.Clear();

            //卡牌
            for (int i = 0; i < cardAmount; i++)
            {
                int postLevel = Mathf.Min(PlayerData.Instance.postLevel + 1, 5);
                Card card = CardStore.Instance.RandomCard(postLevel, false);
                int price = value_EveryLevel[card.qualityLevel - 1];
                GameObject cardPG2 = Instantiate(cardPrefab, roots_cards[i]);
                cardPG2.GetComponent<CardDisplayPersonalGameLibrary>().card = card;
                cardPG2.GetComponent<CardDisplayPersonalGameLibrary>().posNum = i;
                cardPG2.GetComponent<HolidayStoreCard>().price = price;
                cardPrefabs.Add(cardPG2);
            }

            for (int i = 0; i < propAmount; i++)
            {
                Prop prop = PropStore.Instance.RandomProp();
                GameObject prop_obj = Instantiate(propPrefab, roots_props[i]);
                prop_obj.GetComponent<PropDisplay>().prop = prop;
                prop_obj.GetComponent<PropDisplay>().posNum = i;
                propPrefabs.Add(prop_obj);
            }
            HolidayStoreDelete.Instance.Reset();
            StartCoroutine(HolidayStoreMove());
            LACControl.Instance.SetMoneyBowlPos("holidayStore");
            StartCoroutine(LACControl.Instance.moneyBowlMove());

        }
    }
    public void CloseStore()
    {
        timer_business = 0;
        timer_All = 0;
        dialog_business.CloseDialog();
        dialog_fish.CloseDialog();
        if (isOpen)
        {
            isOpen = false;

            CameraManager.Instance.SetVirtualCam("BlackCam");
            // Mechanism.Instance.ForceChessboardOut(false);
            // StartCoroutine(Mechanism.Instance.ChessBoardMove(true));
            StartCoroutine(HolidayStoreMove(true));
            LACControl.Instance.SetMoneyBowlPos("origin");
        }

    }

    public IEnumerator HolidayStoreMove(bool isReverse = false)
    {
        float timer = 0;
        Vector3 originPos = backGround.transform.localPosition;
        Vector3 originPos_Fish = fish.transform.localPosition;
        Vector3 destPos;
        Vector3 destPos_Fish;
        if (!isReverse)
        {
            destPos = originPos + new Vector3(-3, 0, 0);
            destPos_Fish = originPos_Fish + new Vector3(-3, 0, 0);
        }
        else
        {
            destPos = originPos + new Vector3(3, 0, 0);
            destPos_Fish = originPos_Fish + new Vector3(3, 0, 0);
        }
        while (true)
        {
            if (timer > 0.5f)
            {
                backGround.transform.localPosition = destPos;
                fish.transform.localPosition = destPos_Fish;
                if (isReverse)
                {
                    backGround.SetActive(false);
                    Mechanism.Instance.PhaseAddAdd();
                }
                yield break;
            }
            timer += Time.deltaTime;
            float lerpFactor = timer / 0.5f;
            backGround.transform.localPosition = Vector3.Lerp(originPos, destPos, lerpFactor);
            fish.transform.localPosition = Vector3.Lerp(originPos_Fish, destPos_Fish, lerpFactor);
            yield return null;
        }
    }
}

