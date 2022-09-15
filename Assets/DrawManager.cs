using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// [ExecuteInEditMode]
public class DrawManager : MonoBehaviour
{
    public List<Texture> hatchs = new List<Texture>();
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < hatchs.Count; i++)
        {
            string s = "_Hatch" + i;
            Shader.SetGlobalTexture(s, hatchs[i]);
        }
    }
}
