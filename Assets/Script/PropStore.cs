using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PropStore : MonoSingleton<PropStore>
{

    public TextAsset propDataCSV;
    public List<Prop> props = new List<Prop>();
    public List<GameObject> props_obj = new List<GameObject>();
    void Start()
    {
        LoadPropDataFromCSV();

    }
    void LoadPropDataFromCSV()
    {
        string[] dataRows = propDataCSV.text.Split("\r\n", System.StringSplitOptions.RemoveEmptyEntries);

        foreach (var dataRow in dataRows)
        {
            string[] elements = dataRow.Split(',');
            if (elements[0] == "id" || elements[0] == "condition" || elements[0] == "label" || elements[0] == "" || elements[0] == "func")//排除空行的影响
            {
                continue;
            }
            else
            {
                int id = int.Parse(elements[0]);
                string title = elements[1];
                string description = elements[2];
                int price = int.Parse(elements[3]);
                int pro = int.Parse(elements[4]);
                props.Add(new Prop(id, title, description, price, pro));
            }
        }
    }

    public Prop RandomProp()
    {
        return new Prop(props[Random.Range(0, props.Count)]);
    }

    public GameObject SearchProp(int id)
    {
        GameObject tmp = null;
        foreach (var props_obj in  props_obj )
        {
            if ("Prop_" + id.ToString() == props_obj.name)
            {
                tmp = props_obj;
                break;
            }
        }
        if (tmp == null)
        {
            tmp =props_obj[0];
        }
        return tmp;
    }
}


