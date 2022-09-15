using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class OpenDepth : MonoBehaviour
{
    // Start is called before the first frame update
   void Awake()
   {
       Camera cam=GetComponent<Camera>();
       cam.depthTextureMode=DepthTextureMode.Depth|DepthTextureMode.MotionVectors;
   }
}
