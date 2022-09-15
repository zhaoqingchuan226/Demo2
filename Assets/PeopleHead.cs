using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleHead : MonoBehaviour
{
    Transform originTrans;
    // Start is called before the first frame update
    void Start()
    {
        originTrans = transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = originTrans.position;
        this.transform.rotation = originTrans.rotation;
    }
}
