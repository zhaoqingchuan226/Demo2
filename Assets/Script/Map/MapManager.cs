using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MapType
{
    月末,
    非月末
}
public class MapManager : MonoSingleton<MapManager>
{
    [HideInInspector] public List<MapPoint> mapPoints = new List<MapPoint>();
    [HideInInspector] public List<GameObject> mapPoints_Objs = new List<GameObject>();
    [HideInInspector] public MapType mapType;
    // [Range(0, 1f)]
    // public float Connect_Pro = 0.1f;
    public GameObject prefab;//节点的prefab
    public GameObject prefab_Meeting;//会议的prefab
    public GameObject linePrefab;
    public List<GameObject> line_Objs;
    public GameObject Map;
    public List<GameObject> models = new List<GameObject>();
    List<MapPointType> MapPointType12 = new List<MapPointType>
{MapPointType.娱乐,MapPointType.未知,MapPointType.酒局,MapPointType.求佛,MapPointType.健身};
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    /// 
    [HideInInspector] public MapPoint currentMapPoint = null;
    [HideInInspector] public List<List<MapPoint>> paths = new List<List<MapPoint>>();
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            OpenMap();
        }
    }
    public void OpenMap()
    {
        ResetAll();
        GenerateMapPoints();
        ConnectMapPoints();
        FindAllPaths();
        FillStudyMapPoint();
        FillMistMapPoint();
    }

    void FindAllPaths()
    {
        //这个必是工作
        foreach (var mp_level1 in mapPoints[0].childrenMapPoint)
        {
            foreach (var mp_level2 in mp_level1.childrenMapPoint)
            {
                foreach (var mp_level3 in mp_level2.childrenMapPoint)
                {
                    paths.Add(new List<MapPoint> { mapPoints[0], mp_level1, mp_level2, mp_level3 });
                }
            }
        }

    }
    bool TestPathRepeat(List<MapPoint> path, out MapPoint RepeatMapPoint)//测试是否为重叠路径//RepeatMapPoint为分岔点
    {
        bool b = false;
        RepeatMapPoint = null;
        for (int i = 1; i < 3; i++)
        {
            if (path[i].childrenMapPoint.Count == 2 || path[i].ParentMapPoint.Count == 2)
            {
                b = true;
                RepeatMapPoint = path[i];
            }
        }
        return b;
    }
    void FillStudyMapPoint()//为每条路径添加一个学习节点 
    {
        foreach (var path in paths)
        {
            if (!TestPathRepeat(path, out MapPoint RepeatMapPoint))//如果此条路径不是重叠路径,那就选择1或者2中的一个点，作为学习
            {
                float r = Random.Range(0, 1f);
                if (r < 0.5f)
                {
                    ChangeOneMapPoint(path[1], MapPointType.学习);
                }
                else
                {
                    ChangeOneMapPoint(path[2], MapPointType.学习);
                }
            }
            else
            {
                if (RepeatMapPoint.mapPointType != MapPointType.学习 && RepeatMapPoint != null)
                {
                    ChangeOneMapPoint(RepeatMapPoint, MapPointType.学习);
                }
            }
        }
    }
    void FillMistMapPoint()//为整张图添加最多一个Mist节点
    {
        List<MapPoint> path = paths[Random.Range(0, paths.Count)];

        List<MapPoint> mps = new List<MapPoint>();
        if (path[1].mapPointType != MapPointType.学习)
        {
            mps.Add(path[1]);
        }
        if (path[2].mapPointType != MapPointType.学习)
        {
            mps.Add(path[2]);
        }
        if (mps.Count > 0)
        {
            MapPoint mp = mps[Random.Range(0, mps.Count)];
            ChangeOneMapPoint(mp, MapPointType.迷雾);
            mistMappoint = mp;
        }
    }


    void ChangeOneMapPoint(MapPoint mp, MapPointType mapPointType)
    {
        mp.mapPointType = mapPointType;
        mp.JudgeModel();
        // mp.JudgeText();
    }
    MapPoint mistMappoint = null;
    public void ChangeMistPoint()//改变迷雾节点
    {
        if (mistMappoint != null)  //如果存在迷雾节点
        {
            if (StoryManager.Instance.plotsThisWeek.Count > 0)  //检测到剧情
            {
                ChangeOneMapPoint(mistMappoint, MapPointType.剧情);
            }
            else//如果没有检测到剧情
            {
                ChangeOneMapPoint(mistMappoint, MapPointType12[Random.Range(0, MapPointType12.Count)]);
            }
        }
    }

    void ResetAll()
    {
        mistMappoint = null;
        foreach (var obj in mapPoints_Objs)
        {
            Destroy(obj);
        }
        mapPoints_Objs.Clear();
        mapPoints.Clear();

        foreach (var obj in line_Objs)
        {
            Destroy(obj);
        }
        line_Objs.Clear();
        currentMapPoint = null;
        paths.Clear();

    }
    void GenerateMapPoints()
    {
        //level0
        GMP(MapPointType.工作, 0);
        //level1
        int r1 = Random.Range(2, 4);
        for (int i = 0; i < r1; i++)
        {
            GMP(RandomType(MapPointType12), 1, r1, i);
        }
        //level2
        int r2 = Random.Range(2, 4);
        for (int i = 0; i < r2; i++)
        {
            GMP(RandomType(MapPointType12), 2, r2, i);
        }
        //level3检测点
        if (Mechanism.Instance.week % 4 == 0)//月末
        {
            MapPoint mp = GMP(MapPointType.会议, 3, 1, 0);
            mp.need = Mechanism.Instance.need_ThisMonth;
            mp.gameObject.GetComponentInChildren<UISign>().OpenLastingSign();
        }
        else
        {

            for (int i = 0; i < 2; i++)
            {
                MapPoint mp = GMP(MapPointType.会议, 3, 2, i);
                mp.need = new Need(Mechanism.Instance.KPINeed_EveryMonth, NeedType.Normal);
                mp.gameObject.GetComponentInChildren<UISign>().OpenLastingSign();
            }
        }

    }

    MapPointType RandomType(List<MapPointType> mapPointTypes)
    {
        return mapPointTypes[Random.Range(0, mapPointTypes.Count)];
    }
    MapPoint GMP(MapPointType mapPointType, int level, int levelMaxMPCount = 1, int levelIndex = 0)//生成单个节点
    {
        GameObject MP;
        if (mapPointType == MapPointType.会议)
        {
            MP = Instantiate(prefab_Meeting, Map.transform);
        }
        else
        {
            MP = Instantiate(prefab, Map.transform);
        }

        MapPoint mp = MP.GetComponent<MapPoint>();
        //计算类型，生成模型
        mp.mapPointType = mapPointType;
        mp.level = level;
        mp.JudgeModel();
        // mp.JudgeText();
        //计算位置
        switch (level)
        {
            case 0:
                MP.transform.position = new Vector3(2.95f, 0.0118f, 0.25f);
                break;
            case 1:
                MP.transform.position = new Vector3(2.95f, 0.0118f, 0.11f);
                break;
            case 2:
                MP.transform.position = new Vector3(2.95f, 0.0118f, -0.03f);
                break;
            case 3:
                MP.transform.position = new Vector3(2.95f, 0.0118f, -0.17f);
                break;
            default:
                break;
        }
        MP.transform.position += new Vector3((levelMaxMPCount * 0.5f - 0.5f - levelIndex) * 0.1f, 0, 0);
        //0-1 0
        //0-2 -1
        //1-2 1
        //0-3 -2
        //1-3 0
        //2-3 2

        mapPoints.Add(mp);
        mapPoints_Objs.Add(MP);
        return mp;
    }

    void ConnectMapPoints()
    {
        ConnectTwoLevels(0, 1);
        ConnectTwoLevels(1, 2);
        ConnectTwoLevels(2, 3);
    }
    void ConnectTwoLevels(int level0, int level1)//父在前，子在后
    {
        List<MapPoint> mps0_ori = FindAllMapPointsThisLevel(level0);
        List<MapPoint> mps1_ori = FindAllMapPointsThisLevel(level1);
        List<MapPoint> mps0 = FindAllMapPointsThisLevel(level0);
        List<MapPoint> mps1 = FindAllMapPointsThisLevel(level1);

        Dictionary<MapPoint, MapPoint> mp0_mp1_Dic = new Dictionary<MapPoint, MapPoint>();
        //先配对
        while (mps0.Count > 0 && mps1.Count > 0)
        {
            MapPoint mp0_random = mps0[Random.Range(0, mps0.Count)];
            MapPoint mp1_random = mps1[Random.Range(0, mps1.Count)];
            mp0_mp1_Dic.Add(mp0_random, mp1_random);
            mps0.Remove(mp0_random);
            mps1.Remove(mp1_random);
        }
        foreach (var mp0 in mp0_mp1_Dic.Keys)
        {
            ConnectTwoPoints(mp0, mp0_mp1_Dic[mp0]);
        }
        while (mps0.Count > 0)
        {
            ConnectTwoPoints(mps0[0], mps1_ori[Random.Range(0, mps1_ori.Count)]);
            mps0.RemoveAt(0);
        }
        while (mps1.Count > 0)
        {
            ConnectTwoPoints(mps0_ori[Random.Range(0, mps0_ori.Count)], mps1[0]);
            mps1.RemoveAt(0);
        }
    }

    void ConnectTwoPoints(MapPoint P, MapPoint C)//父在前，子在后
    {
        if (!P.childrenMapPoint.Contains(C))
        {
            P.childrenMapPoint.Add(C);
            C.ParentMapPoint.Add(P);
            DrawLine(P.gameObject.transform.position, C.gameObject.transform.position);
        }
    }
    void DrawLine(Vector3 pos0, Vector3 pos1)
    {
        GameObject line = Instantiate(linePrefab, Map.transform);
        line.transform.forward = Vector3.Normalize(pos0 - pos1);
        line.transform.localScale = new Vector3(1, 1, 100 * Vector3.Distance(pos0, pos1));
        line.transform.position = 0.5f * (pos0 + pos1);
        line_Objs.Add(line);
    }
    List<MapPoint> FindAllMapPointsThisLevel(int level)
    {
        List<MapPoint> mps = new List<MapPoint>();
        foreach (var mp in mapPoints)
        {
            if (mp.level == level)
            {
                mps.Add(mp);
            }
        }
        return mps;
    }

    public IEnumerator MapMove(bool isReversed = false)
    {

        float timer = 0;
        Vector3 originPos = Map.transform.position;
        Vector3 destPos;
        if (!isReversed)
        {
            destPos = originPos + new Vector3(3, 0, 0);
        }
        else
        {
            destPos = originPos + new Vector3(-3, 0, 0);
        }
        while (true)
        {

            if (timer > 0.5f)
            {
                timer = 0f;
                Map.transform.position = destPos;

                yield break;
            }
            timer += Time.deltaTime;
            float lerpFactor = timer / 0.5f;
            Map.transform.position = Vector3.Lerp(originPos, destPos, lerpFactor);
            yield return null;
        }
    }
}
