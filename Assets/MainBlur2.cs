using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MainBlur2 : MonoBehaviour
{
    public Material mat1;

    [Range(0, 8)]
    public int iteration = 1;
   
     [Range(0, 8)]
     public int textureScaleSize=2;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
  
  
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        RenderTexture[] RT = new RenderTexture[2 * iteration + 1];
        int width = src.width;
        int height = src.height;



        RT[0] = RenderTexture.GetTemporary(width, height, 0, src.format);
        Graphics.Blit(src,RT[0]);

      
        for (int i = 1; i < iteration + 1; i++)
        {
            width /= textureScaleSize; height /= textureScaleSize;
            RT[i] = RenderTexture.GetTemporary(width, height, 0, src.format);
            mat1.SetTexture("_MainTex", RT[i]);
            Graphics.Blit(RT[i - 1], RT[i], mat1, 1);
        }
        for (int i = iteration + 1; i < 2 * iteration + 1; i++)
        {
            width *= textureScaleSize; height *= textureScaleSize;
            RT[i] = RenderTexture.GetTemporary(width, height, 0, src.format);
            mat1.SetTexture("_MainTex", RT[i]);
            // mat1.SetTexture("_BloomTex", RT[2 * iteration - i]);
            Graphics.Blit(RT[i - 1], RT[i], mat1, 2);
        }
        // mat1.SetTexture("_BloomTex", RT[2 * iteration]);
        // float intensity = Mathf.Exp(_Intensity / 10.0f * 0.693f) - 1.0f;
        // mat1.SetFloat("_Intensity", intensity);
        Graphics.Blit(RT[2 * iteration], dest);
        for (int i = 0; i < 2 * iteration + 1; i++)
        {
            RenderTexture.ReleaseTemporary(RT[i]);
        }


    }
}
