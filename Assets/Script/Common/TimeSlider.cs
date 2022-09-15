using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{

    void Start()
    {

    }

    // Update is called once per frame
    /// <summary>
    /// Handler for message that is sent when an object or group of objects in the hierarchy changes.
    /// </summary>
    /// <summary>
    /// Handler for message that is sent whenever the state of the project changes.
    /// </summary>

    void OnGUI()
    {
        if (!TimeManager.Instance.isStop)
        {
            Time.timeScale = gameObject.GetComponent<Slider>().value;
        }

    }
    // private void OnProjectChange()
    // {

    // }


    // void Update()
    // {

    // }
}
