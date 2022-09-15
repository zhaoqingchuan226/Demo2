#ifndef UNITY_GBUFFER_MINE
    #define UNITY_GBUFFER_MINE
    sampler2D _CameraGBufferTexture0;
    sampler2D _CameraGBufferTexture1;
    sampler2D _CameraGBufferTexture2;
    sampler2D _CameraDepthTexture;

    float4 Gbuffer0(float2 uv)
    {
        half4 gbuffer0 = tex2D (_CameraGBufferTexture0, uv);
        return gbuffer0;
    }

    float4 Gbuffer2(float2 uv)
    {
        half4 gbuffer2 = tex2D (_CameraGBufferTexture2, uv);
        return gbuffer2;
    }

    float Depth(float2 UV)
    {
        return LinearEyeDepth(tex2D(_CameraDepthTexture,UV)).x;
    }
    

    float DirectionalLightShadowAtten()
    {
        
    }

#endif 
