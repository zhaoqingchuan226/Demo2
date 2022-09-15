using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class MyBillBoard1 : MonoBehaviour 
{

    public bool bTurnOver = false;

    void OnWillRenderObject()
    {
        if (Camera.current)
        {
            if (bTurnOver)
                transform.forward = Camera.current.transform.forward;
            else
                transform.forward = -Camera.current.transform.forward;
        }
    }
}
