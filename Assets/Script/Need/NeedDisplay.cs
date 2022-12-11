using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NeedDisplay : MonoBehaviour
{
    public TextMeshPro title;
    public TextMeshPro des;
    [HideInInspector] public Need need;

    public void Show()
    {
        title.text = "目标：" + need.TargetTiTle();
        des.text = need.des;
    }
}
