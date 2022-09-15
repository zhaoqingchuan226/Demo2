using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LAC_Tree_Control : MonoSingleton<LAC_Tree_Control>
{
    //主干贴图插值
    public Material stone;//颜色瞬间变化
    public ParticleSystem pm_stone;//升级的特效
    // public Material main;//颜色、贴图插值
    // public Material branchs;//顶点动画、颜色插值
    // public Material leaves;//变大、颜色插值
    // public Material flowers;//变大、枯萎blendshape
    // public Material fruits;//变大、颜色插值（青变红）
    // Start is called before the first frame update
    public float[] factors = new float[4];
    public GameObject leaves_obj;

    List<Transform> leaves_trans = new List<Transform>();
    Dictionary<Transform, float> leaves_trans_OriginScale = new Dictionary<Transform, float>();

    public GameObject flowers_obj;

    List<Transform> flowers_trans = new List<Transform>();
    Dictionary<Transform, float> flowers_trans_OriginScale = new Dictionary<Transform, float>();

    public GameObject fruits_obj;

    List<Transform> fruits_trans = new List<Transform>();
    Dictionary<Transform, float> fruits_trans_OriginScale = new Dictionary<Transform, float>();
    public List<GameObject> objs = new List<GameObject>();
    private void Start()
    {
        leaves_trans.AddRange(leaves_obj.GetComponentsInChildren<Transform>());
        leaves_trans.RemoveAt(0);
        flowers_trans.AddRange(flowers_obj.GetComponentsInChildren<Transform>());
        flowers_trans.RemoveAt(0);
        fruits_trans.AddRange(fruits_obj.GetComponentsInChildren<Transform>());
        fruits_trans.RemoveAt(0);
        foreach (var t in leaves_trans)
        {
            leaves_trans_OriginScale.Add(t, t.localScale.x);
        }
        foreach (var t in flowers_trans)
        {
            flowers_trans_OriginScale.Add(t, t.localScale.x);
        }
        foreach (var t in fruits_trans)
        {
            fruits_trans_OriginScale.Add(t, t.localScale.x);
        }

        CalculateFactors(PlayerData.Instance.workAbility);
        Stone();
    }
    void Main()
    {
        //t1时期树干颜色由浅绿色变深绿色
        //t2时期树干进行贴图的插值
        //t3时期树干进行贴图的插值
    }
    void Branchs()
    {
        //t1时期变长
        //t2时期颜色插值，加深
    }
    void Leaves()
    {
        //t2时期变大
        //t3时期变绿
        //t4时期变黄
        // if (factors[1] == 0 || factors[1] == 1)
        // {
        //     return;
        // }
        foreach (var t in leaves_trans)
        {
            t.gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, factors[1]) * leaves_trans_OriginScale[t];
        }
    }

    void Flowers()
    {
        //t3时期变大
        //t4时期变小 枯萎
        // if (factors[2] == 0 || factors[2] == 1)
        // {
        //     return;
        // }
        foreach (var t in flowers_trans)
        {
            t.gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, factors[2] * Mathf.Lerp(1, 0.7f, factors[3])) * flowers_trans_OriginScale[t];
        }
    }

    void Fruits()
    {
        // if (factors[3] == 0 || factors[3] == 1)
        // {
        //     return;
        // }
        foreach (var t in fruits_trans)
        {
            t.gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, factors[3]) * fruits_trans_OriginScale[t];
        }
    }
    public void Stone(bool isUpgrade = false)//是否是升级
    {

        Color c;
        // Debug.Log(PlayerData.Instance.postLevel+"PlayerData.Instance.postLevel");
        switch (PlayerData.Instance.postLevel)
        {
            case 1:
                c = new Color(210f / 255f, 210f / 255f, 210f / 255f, 1);
                break;
            case 2:
                c = new Color(175f / 255f, 239f / 255f, 96f / 255f, 1);
                break;
            case 3:
                c = new Color(35f / 255f, 130f / 255f, 236f / 255f, 1);
                break;
            case 4:
                c = new Color(179f / 255f, 33f / 255f, 180f / 255f, 1);
                break;
            case 5:
                c = new Color(255f / 255f, 195f / 255f, 50f / 255f, 1);
                break;
            default:
                c = Color.white;
                break;
        }

        stone.color = c;
        if (isUpgrade)
        {
            pm_stone.Stop();
            pm_stone.Play();
            StartCoroutine(PostUpgrade());
        }

    }

    IEnumerator PostUpgrade()
    {
        LACControl.Instance.dialog.SetDiaglog("更好的颜色，更好的树！");
        CameraManager.Instance.SetVirtualCam("LacCam", 0.25f);
        yield return new WaitForSeconds(2f);
        CameraManager.Instance.BackLastCamera(0.25f);
        yield break;
    }

    [Range(0, 4)]
    public float factor = 0;
    public void CalculateFactors(int W)
    {
        // Debug.Log(W);
        int[] postUpgradeNeeds = Mechanism.Instance.postUpgradeNeeds;
        // int W = PlayerData.Instance.workAbility;
        factors[0] = Mathf.Clamp01(W / (float)postUpgradeNeeds[0]);
        factors[1] = Mathf.Clamp01((W - postUpgradeNeeds[0]) / (float)(postUpgradeNeeds[1] - postUpgradeNeeds[0]));
        factors[2] = Mathf.Clamp01((W - postUpgradeNeeds[1]) / (float)(postUpgradeNeeds[2] - postUpgradeNeeds[1]));
        factors[3] = Mathf.Clamp01((W - postUpgradeNeeds[2]) / (float)(postUpgradeNeeds[3] - postUpgradeNeeds[2]));
        factors[0] += Mathf.Clamp01(factor / 1);
        factors[1] += Mathf.Clamp01((factor - 1) / 1);
        factors[2] += Mathf.Clamp01((factor - 2) / 1);
        factors[3] += Mathf.Clamp01((factor - 3) / 1);
        // Debug.Log(factors[0] + "factors[0]");
        Shader.SetGlobalFloat("_factor0", factors[0]);
        Shader.SetGlobalFloat("_factor1", factors[1]);
        Shader.SetGlobalFloat("_factor2", factors[2]);
        Shader.SetGlobalFloat("_factor3", factors[3]);
        Leaves();
        Flowers();
        Fruits();
    }


    void Update()
    {
        // CalculateFactors();
        // leaves_Test.transform.localScale=Vector3.Lerp(Vector3.zero,Vector3.one,factors[1]);
    }


    public IEnumerator SetTree(int deltaW, float timer = 0)
    {
        int originW = PlayerData.Instance.workAbility;
        // Debug.Log(deltaW);
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > LACControl.Instance.setWeightTime)
            {
                timer = 0;
                CalculateFactors(originW + deltaW);
                yield break;
            }
            CalculateFactors(originW + (int)(deltaW * timer / LACControl.Instance.setWeightTime));
            yield return null;
        }
    }
    public void O_C_AllMeshes(bool b)
    {
        foreach (var obj in objs)
        {
            obj.SetActive(b);
        }
    }
}
