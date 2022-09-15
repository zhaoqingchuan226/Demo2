using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class EleControl : MonoBehaviour
{
    public Transform shoulder;
    public GameObject ele;
    public Animator animator;
    // void Start()
    // {

    // }


    void Update()
    {
        ele.transform.position = shoulder.position;
        ele.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
