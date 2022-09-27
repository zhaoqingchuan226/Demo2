using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullControl : MonoSingleton<SkullControl>
{
    public GameObject eye_prefab;
    public List<Transform> eye_roots = new List<Transform>();//0是左边骷髅的眼睛，1、2、3是右边骷髅的眼睛，1是左眼，2是右眼，3是嘴巴
    public Dictionary<Transform, GameObject> eye_roots_GameObjs_Dic = new Dictionary<Transform, GameObject>();

    public GameObject pressButton;
    [HideInInspector] public bool isButtonPressed = false;
    public Light eyeLight;
    float eyeLightOriginIntensity;
    //！！！按下按钮！！！
    public void O_C_Button(bool b)
    {
        if (b && !isButtonPressed)
        {
            AudioManager.Instance.PlayClip("button1");
            pressButton.transform.position -= pressButton.transform.up * 0.01f;
            isButtonPressed = true;
        }
        else if (!b && isButtonPressed)
        {
            pressButton.transform.position += pressButton.transform.up * 0.01f;
            isButtonPressed = false;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        for (var i = 0; i < eye_roots.Count; i++)
        {
            eye_roots_GameObjs_Dic.Add(eye_roots[i], null);
        }
        eyeLightOriginIntensity = eyeLight.intensity;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    // private void Update()
    // {
    //     // if (Input.GetKeyDown(KeyCode.K))
    //     // {
    //     //     int n = Random.Range(0, 4);
    //     //     GenerateEyeBall(n);
    //     //     Debug.Log(n);
    //     // }
    //     if (Input.GetKeyDown(KeyCode.D))
    //     {
    //         DestroyEye(0);
    //     }

    //     // if (Input.GetKeyDown(KeyCode.N))
    //     // {
    //     //     GenerateEyeBall(0);
    //     //     // O_C_Button(true);
    //     // }
    //     // else if (Input.GetKeyDown(KeyCode.J))
    //     // {
    //     //     EmitEye(0, 3);
    //     // }
    //     // else if (Input.GetKeyDown(KeyCode.K))
    //     // {
    //     //     EmitEye(3, 1);
    //     // }
    //     // else if (Input.GetKeyDown(KeyCode.L))
    //     // {
    //     //     EmitEye(1, 0);
    //     // }
    // }

    //！！！生成眼球！！！
    public void GenerateEyeBall(int transNum)//第几个eye_roots生成眼球
    {
        if (eye_roots_GameObjs_Dic[eye_roots[transNum]] == null)//如果是空的就生，如果不是空的就不生
        {
            GameObject eye = Instantiate(eye_prefab, eye_roots[transNum]);
            eye_roots_GameObjs_Dic[eye_roots[transNum]] = eye;
            StartCoroutine(Scale(eye));
        }
    }

    IEnumerator Scale(GameObject g)
    {
        float timer = 0f;
        Vector3 originScale = g.transform.localScale;
        while (true)
        {
            if (timer > 0.5f)
            {
                g.transform.localScale = originScale;
                yield break;
            }
            timer += Time.deltaTime;
            float factor = timer / 0.5f;
            g.transform.localScale = factor * originScale;
            yield return null;
        }
    }

    //发射眼球
    public void EmitEye(int a, int b)//a,b是eye_roots中的发射点和接受点的数字顺序
    {
        AudioManager.Instance.PlayClip("Eye_shoot");
        //如果发射点有眼球且接受点无球
        if (eye_roots_GameObjs_Dic[eye_roots[a]] != null && eye_roots_GameObjs_Dic[eye_roots[b]] == null)
        {
            GameObject g = eye_roots_GameObjs_Dic[eye_roots[a]];
            StartCoroutine(FlyMove(g, a, b));
        }

    }

    float FlyTime = 0.25f;
    IEnumerator FlyMove(GameObject g, int a, int b)
    {
        float timer = 0f;
        Vector3 originPos = eye_roots[a].transform.position;
        bool isFirstArrived = true;//是否到达b
        Quaternion originRotation = Quaternion.identity;
        while (true)
        {
            if (timer > FlyTime)
            {
                if (isFirstArrived)
                {
                    isFirstArrived = false;
                    g.transform.position = eye_roots[b].transform.position;
                    g.transform.parent = eye_roots[b].transform;//设置新的父对象
                    eye_roots_GameObjs_Dic[eye_roots[a]] = null;
                    eye_roots_GameObjs_Dic[eye_roots[b]] = g;
                    originRotation = g.transform.localRotation;
                }

                if (timer > 2 * FlyTime)
                {
                    g.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    JudgeLightIntensityByKPILife();
                    yield break;
                }

                float factor2 = (timer - FlyTime) / FlyTime;
                g.transform.localRotation = Quaternion.Lerp(originRotation, Quaternion.Euler(0, 0, 0), factor2);
            }
            timer += Time.deltaTime;
            float factor = timer / FlyTime;
            g.transform.position = Vector3.Lerp(eye_roots[a].transform.position, eye_roots[b].transform.position, factor);
            yield return null;
        }
    }



    //！！！摧毁眼球！！！
    public void DestroyEye(int n)
    {
        GameObject g = eye_roots_GameObjs_Dic[eye_roots[n]];
        if (g != null)
        {
            AudioManager.Instance.PlayClip("Eye_fire");
            Material mat = g.GetComponent<MeshRenderer>().material;
            mat.SetFloat("_clii", 1);
            StartCoroutine(Dissolve(mat, g, n));
        }
    }
    IEnumerator Dissolve(Material mat, GameObject g, int n)
    {
        float timer = 0f;

        while (true)
        {
            if (timer > 1f)
            {
                eye_roots_GameObjs_Dic[eye_roots[n]] = null;
                Destroy(g);//毁灭
                JudgeLightIntensityByKPILife();
                yield break;
            }
            timer += Time.deltaTime;
            float factor = timer / 1f;
            mat.SetFloat("_Dissolve", factor);
            yield return null;
        }
    }


    public void EmitEye_Ani()//先塞1
    {
        if (eye_roots_GameObjs_Dic[eye_roots[0]] != null)
        {
            if (eye_roots_GameObjs_Dic[eye_roots[1]] == null)
            {
                EmitEye(0, 1);
            }
            else if (eye_roots_GameObjs_Dic[eye_roots[2]] == null)
            {
                EmitEye(0, 2);
            }
            else if (eye_roots_GameObjs_Dic[eye_roots[3]] == null)
            {
                EmitEye(0, 3);
            }
        }
    }


    public void DestoryEyeAni()//先丢3
    {
        if (eye_roots_GameObjs_Dic[eye_roots[3]] != null)
        {
            DestroyEye(3);

        }
        else if (eye_roots_GameObjs_Dic[eye_roots[2]] != null)
        {
            DestroyEye(2);

        }
        else if (eye_roots_GameObjs_Dic[eye_roots[1]] != null)
        {
            DestroyEye(1);

        }
    }

    public void JudgeLightIntensityByKPILife()//眼球消失，灯的亮度会改变
    {
        switch (PlayerData.Instance.KPILife)
        {
            case 0:
                eyeLight.intensity = eyeLightOriginIntensity * 0f;
                break;
            case 1:
                eyeLight.intensity = eyeLightOriginIntensity * 0.7f;
                break;
            case 2:
                eyeLight.intensity = eyeLightOriginIntensity * 1f;
                break;
            case 3:
                eyeLight.intensity = eyeLightOriginIntensity * 1.3f;
                break;
            default:
                break;
        }
    }

}

//月初检测眼睛槽位是否有物体，如果没有就生成眼睛
//  Transform g = eye_roots[0].Find("eye");

//每一周检测KPI，及格就发射眼球，不及格就不发射



//月末眼球消耗，眼球移动

