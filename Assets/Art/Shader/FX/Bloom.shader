Shader "Hidden/Bloom"
{
    CGINCLUDE
    #include "UnityCG.cginc"
    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    sampler2D _BloomTex;
    float _Threshold;
    float _Intensity;
    fixed4 frag_PreFillter (v2f_img i) : SV_Target
    {
        half4 d=_MainTex_TexelSize.xyxy*half4(-1,-1,1,1);
        half4 s=0;
        s+=tex2D(_MainTex, i.uv+d.xy);
        s+=tex2D(_MainTex, i.uv+d.xw);
        s+=tex2D(_MainTex, i.uv+d.zy);
        s+=tex2D(_MainTex, i.uv+d.zw);
        
        s/=4;
        float br=max(max(s.r,s.g),s.b);
        br=max(0,(br-_Threshold))/max(br,0.000001f);
        s.rgb*=br;
        return s;
    }
    fixed4 frag_DownSample (v2f_img i) : SV_Target
    {
        half4 d=_MainTex_TexelSize.xyxy*half4(-1,-1,1,1);
        half4 s=0;
        s+=tex2D(_MainTex, i.uv+d.xy);
        s+=tex2D(_MainTex, i.uv+d.xw);
        s+=tex2D(_MainTex, i.uv+d.zy);
        s+=tex2D(_MainTex, i.uv+d.zw);

        s/=4;
        
        return s;
    }
    fixed4 frag_UpSample (v2f_img i) : SV_Target
    {
        
        half4 d=_MainTex_TexelSize.xyxy*half4(-1,-1,1,1);
        half4 s=0;
        s+=tex2D(_MainTex, i.uv+d.xy);
        s+=tex2D(_MainTex, i.uv+d.xw);
        s+=tex2D(_MainTex, i.uv+d.zy);
        s+=tex2D(_MainTex, i.uv+d.zw);
        // fixed4 bloom=tex2D(_BloomTex,i.uv);
        
        s/=4;
        // s.rgb+=bloom.rgb;
        return s;
    }

    fixed4 frag_Combine (v2f_img i) : SV_Target
    {
        
        half4 d=_MainTex_TexelSize.xyxy*half4(-1,-1,1,1);
        half4 s=0;
        s+=tex2D(_MainTex, i.uv+d.xy);
        s+=tex2D(_MainTex, i.uv+d.xw);
        s+=tex2D(_MainTex, i.uv+d.zy);
        s+=tex2D(_MainTex, i.uv+d.zw);
        half4 bloom=tex2D(_BloomTex,i.uv);
        
        s/=4;
        s.rgb+=bloom.rgb*_Intensity;

        
        return s;
    }

    ENDCG
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_PreFillter
            ENDCG
        }
         Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_DownSample
            ENDCG
        }
         Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_UpSample
            ENDCG
        }
         Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_Combine
            ENDCG
        }
      
        
    }
}