using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager : MonoSingleton<CameraManager>
{
    // public CinemachineBrain cb;
    // Start is called before the first frame update
    public PostProcessVolume volume;
    public List<GameObject> vcams;
    public float focusDistance1 = 1.58f;
    public float focusDistance2 = 2.29f;
    public float focusDistanceChangeTime = 2f;
    float timer = 0;
    // public GameObject mainCameraObj;
    [HideInInspector] public CinemachineBrain cb;
    [HideInInspector] public CinemachineVirtualCamera currentVirtualCamera;
    [HideInInspector] public CinemachineVirtualCamera ChessCam;
    [HideInInspector] public CinemachineVirtualCamera BlackCam;
    public Transform Stranger_Abdomen;//陌生人的腹部
    string lastCamera;

    private void Awake()
    {
        cb = Camera.main.GetComponent<CinemachineBrain>();
        SetVirtualCam("BlackCam");
    }
    void Start()
    {


        // SetFieldDepth(0);

        foreach (var vcam in vcams)
        {
            if (vcam.name == "ChessCam")
            {
                ChessCam = vcam.GetComponent<CinemachineVirtualCamera>();
            }
            else if (vcam.name == "BlackCam")
            {
                BlackCam = vcam.GetComponent<CinemachineVirtualCamera>();
            }
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.L))
    //     {
    //         CameraShake(0.2f, 2);
    //     }
    // }

    public void SetVirtualCam(string camName, float SwicthTime)
    {
       
        bool isChange = false;
        foreach (var vcam in vcams)
        {
            if (vcam.name == camName)
            {
                isChange = true;
                vcam.SetActive(true);
                if (currentVirtualCamera != null && currentVirtualCamera.name != "LibrarySpeCam")
                {
                    lastCamera = currentVirtualCamera.name;
                }
                currentVirtualCamera = vcam.GetComponent<CinemachineVirtualCamera>();
                cb.m_DefaultBlend.m_Time = SwicthTime;
                float timer_Recover = 0f;
                if (SwicthTime != 1f)
                {
                    StartCoroutine(BlendTimeRecover(SwicthTime, timer_Recover, isChange, camName));
                }
            }

        }


    }

    public void SetVirtualCam(string camName)
    {

        bool isChange = false;
        foreach (var vcam in vcams)
        {
            if (vcam.name == camName)
            {
                isChange = true;
                vcam.SetActive(true);
                if (currentVirtualCamera != null && currentVirtualCamera.name != "LibrarySpeCam")
                {
                    lastCamera = currentVirtualCamera.name;
                }
                currentVirtualCamera = vcam.GetComponent<CinemachineVirtualCamera>();
            }

        }

        if (isChange)
        {
            foreach (var vcam in vcams)
            {
                if (vcam.name != camName)
                {
                    vcam.SetActive(false);
                }
            }
        }
    }

    public void BackLastCamera(float switchTime)
    {
        if (switchTime == 1f)
        {
            SetVirtualCam(lastCamera);
        }
        else
        {
            SetVirtualCam(lastCamera, switchTime);
        }

    }

    IEnumerator BlendTimeRecover(float switchTime, float timer_Recover, bool isChange, string camName)
    {
        while (true)
        {
            if (timer_Recover > switchTime)
            {
                cb.m_DefaultBlend.m_Time = 1f;
                if (isChange)
                {
                    foreach (var vcam in vcams)
                    {
                        if (vcam.name != camName)
                        {
                            vcam.SetActive(false);
                        }
                    }
                }
                yield break;
            }
            timer_Recover += Time.deltaTime;
            yield return null;
        }
    }
    public void SetVirtualCamFollow(Transform trans)
    {
        currentVirtualCamera.Follow = trans;
    }

    public void SetVirtualCamLookAt(Transform trans)
    {
        currentVirtualCamera.LookAt = trans;
    }

    //有关景深的函数暂时不使用
    public void SetFieldDepth(int state)//0 focusDistance1//1 focusDistance2
    {
        if (state == 0)
        {
            volume.profile.GetSetting<DepthOfField>().focusDistance.value = focusDistance1;
        }
        else
        {
            volume.profile.GetSetting<DepthOfField>().focusDistance.value = focusDistance2;
        }
    }

    IEnumerator DepthFieldEX()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > focusDistanceChangeTime)
            {
                timer = 0;
                volume.profile.GetSetting<DepthOfField>().focusDistance.value = focusDistance2;
                break;
            }
            volume.profile.GetSetting<DepthOfField>().focusDistance.value = Mathf.Lerp(focusDistance1, focusDistance2, timer / focusDistanceChangeTime);
            yield return null;
        }
    }
    public void Chess_Cam_BlackFade()
    {
        Cam_BlackFade(ChessCam, false, 3f);
    }
    public void Black_Cam_BlackFade()
    {
        Cam_BlackFade(BlackCam, true, 20f);
    }
    public void Cam_BlackFade(CinemachineVirtualCamera Vcam, bool isReverse, float exitTime)//暗场过渡
    {
        // Debug.Log(Vcam.name);
        GameObject quad = Vcam.transform.Find("Quad").gameObject;
        // Debug.Log(quad.transform.localPosition.z);
        quad.SetActive(true);
        Material mat = quad.GetComponent<MeshRenderer>().material;
        float timer_fade = 0;
        // Debug.Log("Chess_Cam_BlackFade");
        StartCoroutine(BlackFade(mat, timer_fade, quad, isReverse, exitTime));

    }
    IEnumerator BlackFade(Material mat, float timer_fade, GameObject quad, bool isReverse, float exitTime)
    {
        while (true)
        {
            if (timer_fade > exitTime)
            {
                quad.SetActive(false);
                yield break;
            }
            // Debug.Log(timer_fade);
            timer_fade += Time.deltaTime;
            float factor = 1 - Mathf.Clamp01((timer_fade - 1) / 2f);
            if (isReverse)
            {
                factor = 1 - factor;
            }
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, factor);
            yield return null;
        }
    }
    Coroutine last_co = null;
    public void CameraShake(float time, float scale)
    {
        if (last_co != null)
        {
            StopCoroutine(last_co);
        }

        if (currentVirtualCamera != null)
        {
            last_co = StartCoroutine(Shake(currentVirtualCamera.gameObject, time, scale));
        }
    }
    public AnimationCurve anic_Shake;
    public IEnumerator Shake(GameObject Cam, float time, float scale)
    {
        float t = 0;
        Vector3 originPos = Cam.transform.localPosition;
        while (true)
        {
            t += Time.deltaTime;
            if (timer > time)
            {
                t = 0;
                Cam.transform.localPosition = originPos;
                yield break;
            }
            float factor = t / time;
            Cam.transform.localPosition = originPos + new Vector3(0, 0, -scale * anic_Shake.Evaluate(factor) * 0.01f);
            yield return null;
        }


    }

}
