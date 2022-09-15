using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum weekendPhase
{
    LeaderTime,//会议开头，领导发言
    Start,//会议刚开始，大家都会说自己的健康信息
    PostUpgrade,//升职的人会说自己升职了
    Else
}
public class WeekendMeetingAll : MonoSingleton<WeekendMeetingAll>
{
    public GameObject backGround;//整个周末会议场景
    public weekendPhase weekendPhase = weekendPhase.Else;
    public GameObject L;//场景太暗了，加了盏灯
    void Start()
    {
        weekendPhase = weekendPhase.Else;
        backGround.transform.localPosition = new Vector3(3, 0, 0);
        // backGround.SetActive(false);
        L.SetActive(false);
    }


    // void Update()
    // {
    //     Debug.Log(weekendPhase);
    // }

    public void Open()
    {
        L.SetActive(true);
        CameraManager.Instance.SetVirtualCam("WeekendCam", 0.5f);

        // Mechanism.Instance.ForceChessboardOut(true);
        StartCoroutine(Mechanism.Instance.ChessBoardMove());

        // backGround.SetActive(true);
        StartCoroutine(HolidayStoreMove());
    }

    public void Close()
    {
        L.SetActive(false);
        CameraManager.Instance.SetVirtualCam("BlackCam", 0.5f);

        // Mechanism.Instance.ForceChessboardOut(false);
        StartCoroutine(Mechanism.Instance.ChessBoardMove(true));

        StartCoroutine(HolidayStoreMove(true));
    }
    public IEnumerator HolidayStoreMove(bool isReverse = false)
    {
        float timer = 0;
        Vector3 originPos = backGround.transform.localPosition;

        Vector3 destPos;

        if (!isReverse)
        {
            destPos = originPos + new Vector3(-3, 0, 0);

        }
        else
        {
            destPos = originPos + new Vector3(3, 0, 0);

        }
        while (true)
        {
            if (timer > 0.5f)
            {
                backGround.transform.localPosition = destPos;

                // if (isReverse)
                // {
                //     backGround.SetActive(false);
                // }
                yield break;
            }
            timer += Time.deltaTime;
            float lerpFactor = timer / 0.5f;
            backGround.transform.localPosition = Vector3.Lerp(originPos, destPos, lerpFactor);

            yield return null;
        }
    }
}
