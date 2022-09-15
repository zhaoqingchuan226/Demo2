using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVTest : MonoBehaviour
{
    public TextAsset cardData;

    void Start()
    {
        string[] dataRows = cardData.text.Split("\r\n", System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var dataRow in dataRows)
        {
            string[] elements = dataRow.Split(',');

            foreach (var element in elements)
            {
                if (element == "5")
                {
                    Debug.Log("OK");
                }

            }


        }
    }


}
