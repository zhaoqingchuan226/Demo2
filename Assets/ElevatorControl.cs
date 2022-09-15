using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElevatorControl : MonoSingleton<ElevatorControl>
{

    public float UpTime = 20f;//上升时间
    public int levelNumAll = 10;//层数
    public TextMeshPro numText;
    public void ElevatorNumUp()//给signal用的
    {
        StartCoroutine(Up());
    }
    IEnumerator Up()
    {
        float timer = 0f;
        numText.text = "1";
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > UpTime / (levelNumAll - 1))
            {
                timer = 0;
                int n = int.Parse(numText.text);
                n += 1;
                numText.text = n.ToString();
                if (n == levelNumAll)
                {
                    yield break;
                }
            }
            yield return null;
        }
    }
}
