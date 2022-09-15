// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/Internal-DeferredShading" {
    Properties {
        _LightTexture0 ("", any) = "" {}
        _LightTextureB0 ("", 2D) = "" {}
        _ShadowMapTexture ("", any) = "" {}
        _SrcBlend ("", Float) = 1
        _DstBlend ("", Float) = 1
        
    }
    SubShader {

        // Pass 1: Lighting pass
        //  LDR case - Lighting encoded into a subtractive ARGB8 buffer
        //  HDR case - Lighting additively blended into floating point buffer
        Pass {
            ZWrite Off
            Blend [_SrcBlend] [_DstBlend]

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_deferred
            #pragma fragment frag
            #pragma multi_compile_lightpass
            #pragma multi_compile ___ UNITY_HDR_ON

            #pragma exclude_renderers nomrt

            #include "UnityCG.cginc"
            #include "UnityDeferredLibrary.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardUtils.cginc"
            #include "UnityGBuffer.cginc"
            #include "UnityStandardBRDF.cginc"

            sampler2D _CameraGBufferTexture0;
            sampler2D _CameraGBufferTexture1;
            sampler2D _CameraGBufferTexture2;
            uniform half4 _CameraDepthTexture_TexelSize;

            

            void UnityDeferredCalculateLightParams_Mine (
            unity_v2f_deferred i,
            out float3 outWorldPos,
            out float2 outUV,
            out half3 outLightDir,
            out float outAtten,
            out float outFadeDist,
            out float lightDistFade
            )
            {
                i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
                float2 uv = i.uv.xy / i.uv.w;

                // read depth and reconstruct world position
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                depth = Linear01Depth (depth);
                float4 vpos = float4(i.ray * depth,1);
                float3 wpos = mul (unity_CameraToWorld, vpos).xyz;


                float fadeDist = UnityComputeShadowFadeDistance(wpos, vpos.z);
                lightDistFade=1;
                // spot light case
                #if defined (SPOT)
                    float3 tolight = _LightPos.xyz - wpos;
                    half3 lightDir = normalize (tolight);

                    float4 uvCookie = mul (unity_WorldToLight, float4(wpos,1));
                    // negative bias because http://aras-p.info/blog/2010/01/07/screenspace-vs-mip-mapping/
                    float atten = tex2Dbias (_LightTexture0, float4(uvCookie.xy / uvCookie.w, 0, -8)).w;
                    atten *= uvCookie.w < 0;
                    
                    float att = dot(tolight, tolight) * _LightPos.w;
                    lightDistFade=atten*att;
                    
                    
                    atten *= tex2D (_LightTextureB0, att.rr).r;

                    atten *= UnityDeferredComputeShadow (wpos, fadeDist, uv);

                    // directional light case
                #elif defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
                    half3 lightDir = -_LightDir.xyz;
                    float atten = 1.0;

                    atten *= UnityDeferredComputeShadow (wpos, fadeDist, uv);

                    #if defined (DIRECTIONAL_COOKIE)
                        atten *= tex2Dbias (_LightTexture0, float4(mul(unity_WorldToLight, half4(wpos,1)).xy, 0, -8)).w;
                    #endif //DIRECTIONAL_COOKIE

                    // point light case
                #elif defined (POINT) || defined (POINT_COOKIE)
                    float3 tolight = wpos - _LightPos.xyz;
                    half3 lightDir = -normalize (tolight);

                    float att = dot(tolight, tolight) * _LightPos.w;
                    float atten = tex2D (_LightTextureB0, att.rr).r;
                    lightDistFade=saturate(1-att);
                    atten *= UnityDeferredComputeShadow (tolight, fadeDist, uv);

                    #if defined (POINT_COOKIE)
                        atten *= texCUBEbias(_LightTexture0, float4(mul(unity_WorldToLight, half4(wpos,1)).xyz, -8)).w;
                    #endif //POINT_COOKIE
                #else
                    half3 lightDir = 0;
                    float atten = 0;
                #endif

                outWorldPos = wpos;
                outUV = uv;
                outLightDir = lightDir;
                outAtten = atten;
                outFadeDist = fadeDist;
            }

            half4 BRDF1_Unity_PBS_Mine (half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness,float3 normal, float3 viewDir,UnityLight light, UnityIndirect gi,half atten,float lightDistFade,float occlusion)
            {
                float perceptualRoughness = SmoothnessToPerceptualRoughness (smoothness);
                float3 halfDir = Unity_SafeNormalize (float3(light.dir) + viewDir);

                #define UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV 0

                #if UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV
                    // The amount we shift the normal toward the view vector is defined by the dot product.
                    half shiftAmount = dot(normal, viewDir);
                    normal = shiftAmount < 0.0f ? normal + viewDir * (-shiftAmount + 1e-5f) : normal;
                    // A re-normalization should be applied here but as the shift is small we don't do it to save ALU.
                    //normal = normalize(normal);

                    float nv = saturate(dot(normal, viewDir)); // TODO: this saturate should no be necessary here
                #else
                    half nv = abs(dot(normal, viewDir));    // This abs allow to limit artifact
                #endif

                float nl = saturate(dot(normal, light.dir));
                float nh = saturate(dot(normal, halfDir));

                half lv = saturate(dot(light.dir, viewDir));
                half lh = saturate(dot(light.dir, halfDir));

                // Diffuse term
                half diffuseTerm = DisneyDiffuse(nv, nl, lh, perceptualRoughness) * nl;
                // diffuseTerm= step(0.5,diffuseTerm)*0.5+0.5;


                // Specular term
                // HACK: theoretically we should divide diffuseTerm by Pi and not multiply specularTerm!
                // BUT 1) that will make shader look significantly darker than Legacy ones
                // and 2) on engine side "Non-important" lights have to be divided by Pi too in cases when they are injected into ambient SH
                float roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
                #if UNITY_BRDF_GGX
                    // GGX with roughtness to 0 would mean no specular at all, using max(roughness, 0.002) here to match HDrenderloop roughtness remapping.
                    roughness = max(roughness, 0.002);
                    float V = SmithJointGGXVisibilityTerm (nl, nv, roughness);
                    float D = GGXTerm (nh, roughness);
                #else
                    // Legacy
                    half V = SmithBeckmannVisibilityTerm (nl, nv, roughness);
                    half D = NDFBlinnPhongNormalizedTerm (nh, PerceptualRoughnessToSpecPower(perceptualRoughness));
                #endif

                float specularTerm = V*D * UNITY_PI; // Torrance-Sparrow model, Fresnel is applied later

                #   ifdef UNITY_COLORSPACE_GAMMA
                specularTerm = sqrt(max(1e-4h, specularTerm));
                #   endif

                // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
                specularTerm = max(0, specularTerm * nl);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularTerm = 0.0;
                #endif

                // surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(roughness^2+1)
                half surfaceReduction;
                #   ifdef UNITY_COLORSPACE_GAMMA
                surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;      // 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
                #   else
                surfaceReduction = 1.0 / (roughness*roughness + 1.0);           // fade \in [0.5;1]
                #   endif

                // To provide true Lambert lighting, we need to be able to kill specular completely.
                specularTerm *= any(specColor) ? 1.0 : 0.0;
                

                half grazingTerm = saturate(smoothness + (1-oneMinusReflectivity));


                // _ShadowColor
                // half s=1-smoothstep(-0.1,0.1, light.color * diffuseTerm).x;
                // half3 shadowColor=s*half3(15.1/255,0.1/255,73.1/255);
                // return half4(shadowColor.xyz,1);

                // half3 d_step=smoothstep(0.05,saturate(0.2+roughness*0.6), light.color * diffuseTerm);
                // half3 d=diffColor * (gi.diffuse + 0.4*d_step+0.3);


                half3 d_step=smoothstep(0.05,saturate(0.2+roughness*0.6),gi.diffuse+atten*  light.color *diffuseTerm);
                
                // half3 d=(pow(diffColor,0.8)+0.01) * (gi.diffuse +d_step*0.7+0.3)*lightDistFade;
                half3 d=diffColor * (gi.diffuse +d_step*0.7+0.3)*lightDistFade;
                
                
                // return  half4(light.color*atten *diffuseTerm,1);
                
                // return half4(gi.diffuse,1);
                // d+=shadowColor*0.2;
                // d=diffColor * (gi.diffuse + light.color * diffuseTerm);
                // return half4(1,1,1,1);
                half3 specAll=specularTerm * light.color *atten* FresnelTerm (specColor, lh)*occlusion;
                // specAll=smoothstep(0,1,specAll)*4;

                half3 color =   d*occlusion
                + specAll
                + surfaceReduction * gi.specular * FresnelLerp (specColor, grazingTerm, nv);
                


                return half4(color, 1);
            }

            fixed luminance(fixed4 color)
            {
                return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b; 
            }

            float edge(float2 uv)
            {

                float2 uvs[8];
                uvs[0] = uv + _CameraDepthTexture_TexelSize.xy * half2(-1, -1);
                uvs[1] = uv + _CameraDepthTexture_TexelSize.xy * half2(0, -1);
                uvs[2] = uv + _CameraDepthTexture_TexelSize.xy * half2(1, -1);
                uvs[3] = uv + _CameraDepthTexture_TexelSize.xy * half2(-1, 0);
                
                uvs[4] = uv + _CameraDepthTexture_TexelSize.xy * half2(1, 0);
                uvs[5] = uv +_CameraDepthTexture_TexelSize.xy * half2(-1, 1);
                uvs[6] = uv + _CameraDepthTexture_TexelSize.xy * half2(0, 1);
                uvs[7] = uv + _CameraDepthTexture_TexelSize.xy * half2(1, 1);

                const half Gx[8] = {-1,  2,  1,
                    -2,    2,
                -1,  -2,  1};
                const half Gy[8] = {-1, -2, -1,
                    2,   -2,
                1,  2,  1};		
                
                half texColor;
                half edgeX = 0;
                half edgeY = 0;
                for (int it = 0; it < 8; it++) {
                    float dd = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvs[it]);
                    texColor= Linear01Depth (dd).r;
                    //    texColor = tex2D(_CameraDepthTexture, uvs[it]).r;
                    edgeX += texColor * Gx[it];
                    edgeY += texColor * Gy[it];
                }

                half texColor_c;
                half edgeX_c = 0;
                half edgeY_c = 0;
                
                for (int it = 0; it < 8; it++) {
                    texColor_c =  luminance(tex2D(_CameraGBufferTexture0, uvs[it]));
                    //    texColor = tex2D(_CameraDepthTexture, uvs[it]).r;
                    edgeX_c += texColor_c * Gx[it];
                    edgeY_c += texColor_c * Gy[it];
                }

                half3 n=tex2D(_CameraGBufferTexture2, uv);
                n=normalize(n * 2 - 1);
                half3 nn;
                half edgeX_n = 0;
                half edgeY_n = 0;
                for (int it = 0; it < 8; it++) 
                {
                    nn =  tex2D(_CameraGBufferTexture2, uvs[it]);
                    nn=normalize(nn * 2 - 1);
                    //    texColor = tex2D(_CameraDepthTexture, uvs[it]).r;
                    edgeX_n +=-dot(nn,n) * Gx[it];
                    edgeY_n += -dot(nn,n) * Gy[it];
                }
                half edge_n=1-50*(abs(edgeX_n) +abs(edgeY_n));
                edge_n=saturate(edge_n);
                

                

                // half texColor_n;
                // half edgeX_n = 0;
                // half edgeY_n = 0;
                // for (int it = 0; it < 9; it++) {
                    //     texColor = tex2D(_, uvs[it]).r;
                    //     edgeX += texColor * Gx[it];
                    //     edgeY += texColor * Gy[it];
                // }
                
                half edge = 1 -180*( abs(edgeX) +abs(edgeY));
                edge=saturate(edge);

                half edge_c=1-20*(abs(edgeX_c) +abs(edgeY_c));
                edge_c=saturate(edge_c);
                // return edge_n;
                
                return min(max(edge_n,edge_c),edge);
                // return edge_c*edge;
                // return 1-step(edge,0.0001) ;
            }


            half4 CalculateLight (unity_v2f_deferred i)
            {
                float3 wpos;
                float2 uv;
                float atten, fadeDist;
                float lightDistFade;
                UnityLight light;
                UNITY_INITIALIZE_OUTPUT(UnityLight, light);
                UnityDeferredCalculateLightParams_Mine (i, wpos, uv, light.dir, atten, fadeDist,lightDistFade);

                light.color = _LightColor.rgb ;//* atten
                
                // unpack Gbuffer
                half4 gbuffer0 = tex2D (_CameraGBufferTexture0, uv);
                half4 gbuffer1 = tex2D (_CameraGBufferTexture1, uv);
                half4 gbuffer2 = tex2D (_CameraGBufferTexture2, uv);
                UnityStandardData data = UnityStandardDataFromGbuffer(gbuffer0, gbuffer1, gbuffer2);


                float e= edge(uv);
                
                // return e;
                // if(tex2D(_CameraDepthTexture,uv).a==2)
                // {
                    //     return 1;
                // }
                // else
                // {
                    //     return 0;
                // }
                float3 eyeVec = normalize(wpos-_WorldSpaceCameraPos);
                half oneMinusReflectivity = 1 - SpecularStrength(data.specularColor.rgb);

                UnityIndirect ind;
                UNITY_INITIALIZE_OUTPUT(UnityIndirect, ind);
                ind.diffuse = 0;
                ind.specular = 0;

                half4 res = BRDF1_Unity_PBS_Mine (data.diffuseColor, data.specularColor, oneMinusReflectivity, data.smoothness, data.normalWorld, -eyeVec, light, ind,atten,lightDistFade,data.occlusion);
                // return res+e*half4(0.2,0,0,1);
                res=lerp(half4(0,0,0,1),res,e);
                // return e;
                return res;
            }

            #ifdef UNITY_HDR_ON
                half4
            #else
                fixed4
            #endif
            frag (unity_v2f_deferred i) : SV_Target
            {
                half4 c = CalculateLight(i);
                
                #ifdef UNITY_HDR_ON
                    return c;
                #else
                    return exp2(-c);
                #endif
            }

            ENDCG
        }


        // Pass 2: Final decode pass.
        // Used only with HDR off, to decode the logarithmic buffer into the main RT
        Pass {
            ZTest Always Cull Off ZWrite Off
            Stencil {
                ref [_StencilNonBackground]
                readmask [_StencilNonBackground]
                // Normally just comp would be sufficient, but there's a bug and only front face stencil state is set (case 583207)
                compback equal
                compfront equal
            }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers nomrt

            #include "UnityCG.cginc"

            sampler2D _LightBuffer;
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert (float4 vertex : POSITION, float2 texcoord : TEXCOORD0)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                o.texcoord = texcoord.xy;
                #ifdef UNITY_SINGLE_PASS_STEREO
                    o.texcoord = TransformStereoScreenSpaceTex(o.texcoord, 1.0f);
                #endif
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return -log2(tex2D(_LightBuffer, i.texcoord));
            }
            ENDCG
        }

    }
    Fallback Off
}
