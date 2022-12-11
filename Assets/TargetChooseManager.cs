using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChooseManager : MonoSingleton<TargetChooseManager>
{
    public GameObject TargetGroup;
    public GameObject prefab;
    public List<Transform> roots = new List<Transform>();
    List<GameObject> Target_Objs = new List<GameObject>();

    List<MoothEndTemplate> moothEndTemplates_All = new List<MoothEndTemplate>
  {  MoothEndTemplate.绩效,
    MoothEndTemplate.身体,//剩余的体力
    MoothEndTemplate.精神,//剩余的精力
    MoothEndTemplate.态度,//消耗的精力
    MoothEndTemplate.卷度,//消耗的体力
    MoothEndTemplate.能力
   };
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Open();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            Close();
        }
    }
    public void Open()
    {
        GenerateThreeTarget();
        StartCoroutine(PanelMove());
    }

    public void Close()
    {
        StartCoroutine(PanelMove(true));
    }

    public void GenerateThreeTarget()
    {
        ResetAll();
        List<MoothEndTemplate> moothEndTemplates_All_Copy = new List<MoothEndTemplate>();
        moothEndTemplates_All_Copy.AddRange(moothEndTemplates_All);
        for (int i = 0; i < 3; i++)
        {
            GameObject need_obj = Instantiate(prefab, roots[i]);
            NeedDisplay nd = need_obj.GetComponent<NeedDisplay>();
            MoothEndTemplate me = moothEndTemplates_All_Copy[Random.Range(0, moothEndTemplates_All_Copy.Count)];
            nd.need = new Need(Mechanism.Instance.KPINeed_EveryMonth, NeedType.MoothEnd, me);
            moothEndTemplates_All_Copy.Remove(me);
            nd.Show();
            Target_Objs.Add(need_obj);
        }
    }
    void ResetAll()
    {
        foreach (var obj in Target_Objs)
        {
            Destroy(obj);
        }
        Target_Objs.Clear();
    }

    public IEnumerator PanelMove(bool isReverse = false)
    {
        float timer = 0;
        Vector3 originPos = TargetGroup.transform.position;
        Vector3 destPos;
        if (!isReverse)
        {
            destPos = originPos + new Vector3(-3, 0, 0);
        }
        else
        {
            destPos = originPos + new Vector3(3, 0, 0);
        }
        while (true)
        {
            if (timer > 0.5f)
            {
                TargetGroup.transform.position = destPos;
                yield break;
            }
            timer += Time.deltaTime;
            float lerpFactor = timer / 0.5f;
            TargetGroup.transform.position = Vector3.Lerp(originPos, destPos, lerpFactor);
            yield return null;
        }
    }
}
