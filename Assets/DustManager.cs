using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustManager : MonoBehaviour
{
    public Texture Dust1;
    public Texture Dust2;
    // Start is called before the first frame update
    void Awake()
    {
        Shader.SetGlobalTexture("_Dust1", Dust1);
        Shader.SetGlobalTexture("_Dust2", Dust2);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
