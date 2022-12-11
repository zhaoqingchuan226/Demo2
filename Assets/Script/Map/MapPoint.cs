using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public enum MapPointType
{
    工作,
    会议,
    // 事件,
    剧情,
    娱乐,
    学习,
    未知,
    酒局,
    求佛,
    迷雾,
    健身
}
public class MapPoint : MonoBehaviour, IPointerClickHandler
{
    public MapPointType mapPointType;
    public GameObject model;
    public List<MapPoint> childrenMapPoint = new List<MapPoint>();
    public List<MapPoint> ParentMapPoint = new List<MapPoint>();
    public int level;

    public GameObject ArrivedSign;
    [HideInInspector] public Need need;//会议节点才有

    public void JudgeModel()
    {
        if (this.model != null)
        {
            Destroy(this.model);
            this.model = null;
        }
        switch (mapPointType)
        {
            case MapPointType.工作:
                this.model = Instantiate(MapManager.Instance.models[1], this.gameObject.transform);
                break;
            case MapPointType.学习:
                this.model = Instantiate(MapManager.Instance.models[2], this.gameObject.transform);
                break;
            case MapPointType.娱乐:
                this.model = Instantiate(MapManager.Instance.models[3], this.gameObject.transform);
                break;
            case MapPointType.未知:
                this.model = Instantiate(MapManager.Instance.models[4], this.gameObject.transform);
                break;
            case MapPointType.求佛:
                this.model = Instantiate(MapManager.Instance.models[5], this.gameObject.transform);
                break;
            case MapPointType.健身:
                this.model = Instantiate(MapManager.Instance.models[6], this.gameObject.transform);
                break;
            case MapPointType.酒局:
                this.model = Instantiate(MapManager.Instance.models[7], this.gameObject.transform);
                break;
            default:
                this.model = Instantiate(MapManager.Instance.models[0], this.gameObject.transform);
                break;

        }

    }
    // public void JudgeText()
    // {
    //     text.text = this.mapPointType.ToString();
    // }
    public void OnPointerClick(PointerEventData eventData)
    {
        ArriveTest();
    }

    void ArriveTest()
    {
        if (this.mapPointType == MapPointType.工作)
        {
            if (MapManager.Instance.currentMapPoint == null)
            {
                SetArrivSign();
                EnterPhase();
                MapManager.Instance.currentMapPoint = this;
            }
        }
        else
        {
            if (MapManager.Instance.currentMapPoint != null)
            {
                if (MapManager.Instance.currentMapPoint.childrenMapPoint.Contains(this))
                {
                    SetArrivSign();
                    EnterPhase();
                    MapManager.Instance.currentMapPoint = this;
                }
            }
        }

    }
    void EnterPhase()//链接mechanism
    {
        switch (this.mapPointType)
        {
            case MapPointType.工作:
                Mechanism.Instance.EnterPhase(Phase.Start);
                break;
            case MapPointType.娱乐:
                Mechanism.Instance.EnterPhase(Phase.HolidayStore);
                break;
            case MapPointType.学习:
                Mechanism.Instance.OnClickChooseCardButton();
                break;
            case MapPointType.会议:
                if (Mechanism.Instance.week % 4 != 0)
                {
                    Mechanism.Instance.need_ThisWeek = need;
                }
                Mechanism.Instance.EnterPhase(Phase.KPITest);
                break;
            case MapPointType.剧情:
                Mechanism.Instance.EnterPhase(Phase.Story);
                break;

            case MapPointType.酒局:
                PlayerData.Instance.ChangeProperty("P", 6);
                PlayerData.Instance.ChangeProperty("S", 6);
                Card kidCard = CardStore.Instance.SearchCard(10000);
                kidCard.isNew = true;
                PlayerData.Instance.playerCards.Add(kidCard);
                PlayerData.Instance.SortCards();
                StartCoroutine(StoryManager.Instance.OpenRewardPanel("恢复6点体力和精力，\r\n获得2张疲劳"));
                break;
            case MapPointType.健身:
                PlayerData.Instance.ChangeProperty("PM", 4);
                PlayerData.Instance.ChangeProperty("P", -10);
                StartCoroutine(StoryManager.Instance.OpenRewardPanel("增加4点体力上限，\r\n减少10点体力"));
                break;
            case MapPointType.求佛:
                PlayerData.Instance.ChangeProperty("SM", 4);
                PlayerData.Instance.ChangeProperty("S", -10);
                StartCoroutine(StoryManager.Instance.OpenRewardPanel("增加4点精力上限，\r\n减少10点精力"));
                break;
            case MapPointType.未知:
                List<MapPointType> mpts = new List<MapPointType> { MapPointType.酒局, MapPointType.健身, MapPointType.求佛 };
                this.mapPointType = mpts[Random.Range(0, mpts.Count)];
                EnterPhase();
                break;

            default:
                break;
        }
    }
    void LeaveMap()
    {

    }


    public void SetArrivSign(bool b = true)
    {
        ArrivedSign.SetActive(b);
    }


}
