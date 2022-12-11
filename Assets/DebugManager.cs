using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugManager : MonoSingleton<DebugManager>
{
    public TextMeshProUGUI text;
    [HideInInspector] public string s = null;
    public void AddDebugThings(string ss)
    {
        s += ss;
        s += "\r\n";
        text.text = s;
    }
}
