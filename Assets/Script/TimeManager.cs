using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoSingleton<TimeManager>
{
    // Start is called before the first frame update
    [HideInInspector] public bool isStop = false;
    float originScale;

    public void OnClickStopButton()
    {
        if (!isStop)
        {
            isStop = true;
            originScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            isStop = false;
            Time.timeScale = originScale;
        }

    }

    public void OnClickStopButton(int i)//强制开关,O是关,1是开
    {
        if (i == 0)
        {
            isStop = true;
            originScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            isStop = false;
            Time.timeScale = originScale;
        }

    }
}
