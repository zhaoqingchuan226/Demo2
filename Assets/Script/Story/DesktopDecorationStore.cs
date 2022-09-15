using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//桌面配置的数值在奇奇怪怪的的地方改
public class DesktopDecorationStore : MonoSingleton<DesktopDecorationStore>
{
    public int test;
    public TextAsset ddData;
    public List<GameObject> dd_models = new List<GameObject>();

    // [HideInInspector] public List<DesktopDecoration> dds = new List<DesktopDecoration>();



    //棋盘上方的布局

    int currentPage = 1;//DD的当前页面，遗物多了就会有多个页面
    public GameObject ddPrefab;//暂时的dd
    public Transform originPos;
    public float widthAll = 0.6f;
    [Range(1, 20)]
    int amountAll = 12;
    List<GameObject> dds_GameObject = new List<GameObject>();
    [HideInInspector] List<DesktopDecoration> dds = new List<DesktopDecoration>();
    [HideInInspector] public List<Vector3> poss = new List<Vector3>();

    void Awake()
    {
        LoadDD_DataFromCSV();

    }
    void Update()
    {
        // GenerateDDs();
    }
    void LoadDD_DataFromCSV()
    {
        string[] dataRows = ddData.text.Split("\r\n", System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var dataRow in dataRows)
        {
            string[] elements = dataRow.Split(',');
            if (elements[0] == "id" || elements[0] == "")//排除空行的影响
            {
                continue;
            }
            else
            {
                int id = int.Parse(elements[0]);
                string title = elements[1];
                int qualityLevel = int.Parse(elements[2]);
                int maxCount = int.Parse(elements[3]);
                string description = elements[4];
                string funDes = elements[5];
                DesktopDecoration dd = new DesktopDecoration(id, title, qualityLevel, maxCount, description, funDes);
                dds.Add(dd);
            }
        }
    }

    public DesktopDecoration SearchDD(int id)
    {
        DesktopDecoration d = null;
        foreach (var dd in dds)
        {
            if (dd.id == id)
            {
                d = dd;
                break;
            }
        }
        if (d == null)
        {
            d = dds[0];
        }
        return new DesktopDecoration(d);
    }

    public GameObject SearchDD_Model(int id)
    {
        GameObject g = null;
        foreach (var dd_model in dd_models)
        {
            if (dd_model.name == "DD_" + id)
            {
                g = dd_model;
                break;
            }
        }
        if (g == null)
        {
            g = dd_models[0];
        }
        return g;
    }

   public void GenerateDDs()//根据playerData生成dds
    {
        List<DesktopDecoration> dds_player = PlayerData.Instance.dds;
        for (var i = 0; i < dds_GameObject.Count; i++)
        {
            Destroy(dds_GameObject[i]);
        }
        dds_GameObject.Clear();
        poss.Clear();

        for (var i = 0; i < amountAll; i++)
        {
            Vector3 t = originPos.position;
            t = new Vector3(originPos.position.x + widthAll / 2, originPos.position.y, originPos.position.z);
            t -= i * new Vector3(widthAll / (float)(amountAll - 1), 0, 0);
            poss.Add(t);
        }

        for (int i = 0; i < dds_player.Count; i++)
        {
            GameObject g = Instantiate(ddPrefab, originPos);
            g.GetComponent<DesktopDecorationDisplay>().dd = dds_player[i];
            g.transform.position = poss[i];
            dds_GameObject.Add(g);
        }

    }

}
