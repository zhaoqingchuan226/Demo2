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
    CinemachineVirtualCamera currentVirtualCamera;

    void Start()
    {
        SetVirtualCam("MainCam");
        // SetFieldDepth(0);
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
    public void SetVirtualCamFollow(Transform trans)
    {
        currentVirtualCamera.Follow =trans;
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
}
