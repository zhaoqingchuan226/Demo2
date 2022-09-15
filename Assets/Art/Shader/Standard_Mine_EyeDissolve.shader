// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_Mine_EyeDissolve"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,0.3544474,0.3544474,0)
		[Toggle(_ISMRAE_ON)] _isMRAE("isMRAE", Float) = 1
		_MRAE("MRAE", 2D) = "white" {}
		_Roughness("Roughness", Range( -1 , 1)) = 1
		_Metalic("Metalic", Range( -1 , 1)) = 1
		[Toggle(_ISNORMAL_ON)] _isNormal("isNormal", Float) = 0
		_Normal("Normal", 2D) = "bump" {}
		[Toggle(_ISEMISSIVE_ON)] _IsEmissive("IsEmissive", Float) = 0
		[Toggle(_ISDUST_ON)] _isDust("isDust", Float) = 0
		[HDR]_EColor("EColor", Color) = (1,1,1,1)
		_DissolveTex("DissolveTex", 2D) = "white" {}
		[HDR]_DisColor("DisColor", Color) = (1,0,0,0)
		_clii("clii", Float) = 0
		_Dissolve("Dissolve", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.0
		#pragma shader_feature_local _ISNORMAL_ON
		#pragma shader_feature_local _ISDUST_ON
		#pragma shader_feature_local _ISEMISSIVE_ON
		#pragma shader_feature_local _ISMRAE_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 vertexToFrag62;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _CullMode;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform sampler2D _Dust1;
		uniform sampler2D _Dust2;
		uniform float4 _EColor;
		uniform sampler2D _MRAE;
		uniform float4 _MRAE_ST;
		uniform float4 _DisColor;
		uniform float _Dissolve;
		uniform sampler2D _DissolveTex;
		uniform float4 _DissolveTex_ST;
		uniform float _clii;
		uniform float _Metalic;
		uniform float _Roughness;
		uniform float _Cutoff = 0.5;


		inline float4 TriplanarSampling10_g2( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline float4 TriplanarSampling11_g2( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 ase_worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
			half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
			float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * tangentSign;
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 objectToTangentDir61 = mul( ase_worldToTangent, mul( unity_ObjectToWorld, float4( ase_vertexNormal, 0 ) ).xyz);
			o.vertexToFrag62 = objectToTangentDir61;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			#ifdef _ISNORMAL_ON
				float3 staticSwitch58 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			#else
				float3 staticSwitch58 = i.vertexToFrag62;
			#endif
			o.Normal = staticSwitch58;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 temp_output_31_0 = ( tex2D( _Albedo, uv_Albedo ) * _Color );
			float4 switchResult52 = (((i.ASEVFace>0)?(temp_output_31_0):(( temp_output_31_0 * 0.7 ))));
			float4 color71 = IsGammaSpace() ? float4(0.6792453,0.4908509,0.4908509,0) : float4(0.418999,0.2056704,0.2056704,0);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 break17_g2 = float4(0.5,0.5,0,0);
			float3 appendResult3_g2 = (float3(break17_g2.z , 0.0 , break17_g2.w));
			float3 temp_output_6_0_g2 = ( ( ase_worldPos * break17_g2.x ) + appendResult3_g2 );
			float4 triplanar10_g2 = TriplanarSampling10_g2( _Dust1, temp_output_6_0_g2, ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float4 triplanar11_g2 = TriplanarSampling11_g2( _Dust2, ( temp_output_6_0_g2 + ( ase_worldPos * break17_g2.y ) ), ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float temp_output_78_0 = ( 0.3 * saturate( max( triplanar10_g2.x , triplanar11_g2.x ) ) );
			float4 lerpResult66 = lerp( switchResult52 , color71 , temp_output_78_0);
			#ifdef _ISDUST_ON
				float4 staticSwitch69 = lerpResult66;
			#else
				float4 staticSwitch69 = switchResult52;
			#endif
			o.Albedo = staticSwitch69.rgb;
			float4 temp_cast_1 = (0.0).xxxx;
			float2 uv_MRAE = i.uv_texcoord * _MRAE_ST.xy + _MRAE_ST.zw;
			float4 saferPower41 = abs( tex2D( _MRAE, uv_MRAE ) );
			#ifdef _ISMRAE_ON
				float4 staticSwitch56 = saturate( pow( saferPower41 , 0.45 ) );
			#else
				float4 staticSwitch56 = float4(0,0,1,1);
			#endif
			float4 break43 = staticSwitch56;
			#ifdef _ISEMISSIVE_ON
				float4 staticSwitch33 = ( _EColor * pow( break43.w , 4.84 ) );
			#else
				float4 staticSwitch33 = temp_cast_1;
			#endif
			float4 lerpResult73 = lerp( staticSwitch33 , float4( 0,0,0,0 ) , temp_output_78_0);
			#ifdef _ISDUST_ON
				float4 staticSwitch72 = lerpResult73;
			#else
				float4 staticSwitch72 = staticSwitch33;
			#endif
			float4 temp_cast_3 = (0.0).xxxx;
			float2 uv_DissolveTex = i.uv_texcoord * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
			float4 tex2DNode84 = tex2D( _DissolveTex, uv_DissolveTex );
			float smoothstepResult87 = smoothstep( _Dissolve , ( _Dissolve + 0.1 ) , tex2DNode84.r);
			float SmoothDissolve90 = ( 1.0 - smoothstepResult87 );
			float clii102 = _clii;
			float4 lerpResult104 = lerp( temp_cast_3 , ( _DisColor * SmoothDissolve90 ) , clii102);
			o.Emission = ( staticSwitch72 + lerpResult104 ).rgb;
			float lerpResult74 = lerp( saturate( ( break43.x + _Metalic ) ) , 0.0 , temp_output_78_0);
			o.Metallic = lerpResult74;
			float lerpResult75 = lerp( ( 1.0 - saturate( ( break43.y + _Roughness ) ) ) , 0.0 , temp_output_78_0);
			o.Smoothness = lerpResult75;
			o.Occlusion = break43.z;
			o.Alpha = 1;
			float lerpResult100 = lerp( 1.0 , step( _Dissolve , tex2DNode84.r ) , _clii);
			clip( lerpResult100 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:forward novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 customPack1 : TEXCOORD1;
				float2 customPack2 : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xyz = customInputData.vertexToFrag62;
				o.customPack2.xy = customInputData.uv_texcoord;
				o.customPack2.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.vertexToFrag62 = IN.customPack1.xyz;
				surfIN.uv_texcoord = IN.customPack2.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-110.5;481;1536;1070.2;-1183.694;-460.3138;1.06392;True;False
Node;AmplifyShaderEditor.SamplerNode;21;-1456.483,-273.5691;Inherit;True;Property;_MRAE;MRAE;8;0;Create;True;0;0;0;False;0;False;-1;None;8a259ad49a2a1d84d929f95f85572763;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;41;-1105.589,-131.747;Inherit;False;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.45;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;57;-1123.061,-393.2682;Inherit;False;Constant;_Vector0;Vector 0;10;0;Create;True;0;0;0;False;0;False;0,0,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;63;-955.9695,-197.2466;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;89;1451.547,942.507;Inherit;False;Constant;_width;width;19;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;1431.776,591.5802;Inherit;False;Property;_Dissolve;Dissolve;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;56;-871.5573,-357.7544;Inherit;False;Property;_isMRAE;isMRAE;7;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;84;1285.089,712.2483;Inherit;True;Property;_DissolveTex;DissolveTex;17;0;Create;True;0;0;0;False;0;False;-1;None;aec90e026d4440943857ec6ccdbd34a7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;88;1624.759,848.29;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;43;-685.0132,-95.21561;Inherit;False;COLOR;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SmoothstepOpNode;87;1759.984,789.5051;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;106;1893.094,930.8109;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;18;-251.793,-363.0797;Inherit;False;Property;_Color;Color;6;0;Create;True;0;0;0;False;0;False;1,0.3544474,0.3544474,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;79;-574.229,77.8047;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;4.84;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-170.0925,-585.269;Inherit;True;Property;_Albedo;Albedo;5;0;Create;True;0;0;0;False;0;False;-1;None;5a7740b5f903c3c4d85c27ed7a2efa99;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;36;-375.6716,-166.7028;Inherit;False;Property;_EColor;EColor;16;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;47.93726,13.05098,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;99;1622.07,574.5364;Inherit;False;Property;_clii;clii;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;285.1206,-213.9524;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-657.1017,427.3165;Inherit;False;Property;_Roughness;Roughness;9;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;384.5384,-359.8366;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;10.10856,-58.13164;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-20.06026,-182.2785;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;2042.189,845.1888;Inherit;False;SmoothDissolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;540.4073,-95.78464;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;78;808.358,116.1167;Inherit;False;Dust;2;;2;e20cd2d7feb3aa14eafd712936aa60f3;0;1;14;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;92;2129.062,671.4545;Inherit;False;Property;_DisColor;DisColor;18;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0,0;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;25;-780.1155,181.4457;Inherit;False;Property;_Metalic;Metalic;10;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;1814.421,661.0975;Inherit;False;clii;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;60;82.31779,353.8889;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-349.054,337.9617;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;2083.741,564.2529;Inherit;False;90;SmoothDissolve;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;33;279.2767,-12.06483;Inherit;False;Property;_IsEmissive;IsEmissive;13;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;24;-229.0494,327.6592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;61;268.5577,285.205;Inherit;False;Object;Tangent;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;71;766.967,-17.10246;Inherit;False;Constant;_Color0;Color 0;13;0;Create;True;0;0;0;False;0;False;0.6792453,0.4908509,0.4908509,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwitchByFaceNode;52;880.8552,-157.1816;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;2309.194,872.944;Inherit;False;102;clii;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;73;1098.008,190.539;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;98;2072.063,455.646;Inherit;False;Constant;_Float3;Float 3;20;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-217.3945,62.85425;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;2363.782,602.7758;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;83;1544.372,476.6364;Inherit;False;Constant;_Float1;Float 1;16;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;66;1083.536,42.00903;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;104;2435.385,520.3506;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;48;409.1035,546.3427;Inherit;True;Property;_Normal;Normal;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;81;1701.282,408.3884;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;20;9.302736,226.6291;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;62;466.6127,313.1176;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;72;1255.042,171.5592;Inherit;False;Property;_isDust;isDust;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;69;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;28;38.23729,60.4586;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;80;1167.415,-68.53519;Inherit;False;Property;_DustToggle;_DustToggle;15;0;Create;True;0;0;0;False;0;False;1;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;69;1381.593,-123.6895;Inherit;False;Property;_isDust;isDust;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;75;1249.983,545.1641;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-961.5894,467.1856;Inherit;False;Property;_CullMode;CullMode;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;74;1243.053,396.1281;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;58;815.6634,226.7574;Inherit;False;Property;_isNormal;isNormal;11;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;100;1860.592,478.8156;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;2472.746,368.2731;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2694.686,23.90827;Float;False;True;-1;4;ASEMaterialInspector;0;0;Standard;Standard_Mine_EyeDissolve;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;DeferredOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;True;32;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;41;0;21;0
WireConnection;63;0;41;0
WireConnection;56;1;57;0
WireConnection;56;0;63;0
WireConnection;88;0;85;0
WireConnection;88;1;89;0
WireConnection;43;0;56;0
WireConnection;87;0;84;1
WireConnection;87;1;85;0
WireConnection;87;2;88;0
WireConnection;106;0;87;0
WireConnection;79;0;43;3
WireConnection;31;0;30;0
WireConnection;31;1;18;0
WireConnection;37;0;36;0
WireConnection;37;1;79;0
WireConnection;90;0;106;0
WireConnection;54;0;31;0
WireConnection;54;1;53;0
WireConnection;102;0;99;0
WireConnection;23;0;43;1
WireConnection;23;1;19;0
WireConnection;33;1;34;0
WireConnection;33;0;37;0
WireConnection;24;0;23;0
WireConnection;61;0;60;0
WireConnection;52;0;31;0
WireConnection;52;1;54;0
WireConnection;73;0;33;0
WireConnection;73;2;78;0
WireConnection;27;0;43;0
WireConnection;27;1;25;0
WireConnection;105;0;92;0
WireConnection;105;1;91;0
WireConnection;66;0;52;0
WireConnection;66;1;71;0
WireConnection;66;2;78;0
WireConnection;104;0;98;0
WireConnection;104;1;105;0
WireConnection;104;2;103;0
WireConnection;81;0;85;0
WireConnection;81;1;84;1
WireConnection;20;0;24;0
WireConnection;62;0;61;0
WireConnection;72;1;33;0
WireConnection;72;0;73;0
WireConnection;28;0;27;0
WireConnection;80;0;52;0
WireConnection;80;1;66;0
WireConnection;69;1;52;0
WireConnection;69;0;66;0
WireConnection;75;0;20;0
WireConnection;75;2;78;0
WireConnection;74;0;28;0
WireConnection;74;2;78;0
WireConnection;58;1;62;0
WireConnection;58;0;48;0
WireConnection;100;0;83;0
WireConnection;100;1;81;0
WireConnection;100;2;99;0
WireConnection;94;0;72;0
WireConnection;94;1;104;0
WireConnection;0;0;69;0
WireConnection;0;1;58;0
WireConnection;0;2;94;0
WireConnection;0;3;74;0
WireConnection;0;4;75;0
WireConnection;0;5;43;2
WireConnection;0;10;100;0
ASEEND*/
//CHKSM=178C303BA0DE06D1748D9BE6AF8D95AB0FB3FA56