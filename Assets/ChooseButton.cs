using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooseButton : MonoBehaviour
{
 public void Cli(TextMeshProUGUI tmp)
 {
     StoryManager.Instance.OnClickButton(tmp);
 }
}
