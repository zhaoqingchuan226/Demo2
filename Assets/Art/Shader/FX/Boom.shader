// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Boom"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Tiling("Tiling", Float) = 1
		_Cell("Cell", 2D) = "white" {}
		_NormalScale("NormalScale", Float) = 5.71
		_Clip("Clip", Range( 0 , 1)) = 0
		_OffsetIntensity("OffsetIntensity", Float) = 1
		_Ramp("Ramp", 2D) = "white" {}
		_EmisIntensity("EmisIntensity", Float) = 1
		_DissolveTex("DissolveTex", 2D) = "white" {}
		[Toggle(_SMOKE_ON)] _Smoke("Smoke", Float) = 1
		[Toggle(_ISONMINUSDISSOLVE_ON)] _IsOnminusDissolve("IsOnminusDissolve", Float) = 1
		[Toggle(_ISSINGLECOLOR_ON)] _IsSingleColor("IsSingleColor", Float) = 0
		[HDR]_SingleColor("SingleColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 4.6
		#pragma shader_feature_local _ISSINGLECOLOR_ON
		#pragma shader_feature_local _SMOKE_ON
		#pragma shader_feature_local _ISONMINUSDISSOLVE_ON
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 
		#pragma surface surf StandardCustomLighting keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _Cell;
		uniform float4 _Cell_ST;
		uniform float _Tiling;
		uniform float _NormalScale;
		uniform float _OffsetIntensity;
		uniform sampler2D _Ramp;
		uniform float _Clip;
		uniform float _EmisIntensity;
		uniform float4 _SingleColor;
		uniform sampler2D _DissolveTex;
		uniform float4 _DissolveTex_ST;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 uvs_Cell = v.texcoord;
			uvs_Cell.xy = v.texcoord.xy * _Cell_ST.xy + _Cell_ST.zw;
			float2 UV17 = ( uvs_Cell.xy * _Tiling );
			float2 temp_output_2_0_g1 = UV17;
			float2 break6_g1 = temp_output_2_0_g1;
			float temp_output_25_0_g1 = ( pow( 0.74 , 3.0 ) * 0.1 );
			float2 appendResult8_g1 = (float2(( break6_g1.x + temp_output_25_0_g1 ) , break6_g1.y));
			float4 tex2DNode14_g1 = tex2Dlod( _Cell, float4( temp_output_2_0_g1, 0, 0.0) );
			float temp_output_4_0_g1 = _NormalScale;
			float3 appendResult13_g1 = (float3(1.0 , 0.0 , ( ( tex2Dlod( _Cell, float4( appendResult8_g1, 0, 0.0) ).g - tex2DNode14_g1.g ) * temp_output_4_0_g1 )));
			float2 appendResult9_g1 = (float2(break6_g1.x , ( break6_g1.y + temp_output_25_0_g1 )));
			float3 appendResult16_g1 = (float3(0.0 , 1.0 , ( ( tex2Dlod( _Cell, float4( appendResult9_g1, 0, 0.0) ).g - tex2DNode14_g1.g ) * temp_output_4_0_g1 )));
			float3 normalizeResult22_g1 = normalize( cross( appendResult13_g1 , appendResult16_g1 ) );
			float3 normalizeResult110 = normalize( normalizeResult22_g1 );
			float cell20 = tex2Dlod( _Cell, float4( UV17, 0, 0.0) ).r;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 ase_worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
			half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
			float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * tangentSign;
			float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);
			float3 tangentTobjectDir5 = mul( unity_WorldToObject, float4( mul( ase_tangentToWorldFast, ( normalizeResult110 * cell20 ) ), 0 ) ).xyz;
			v.vertex.xyz += ( tangentTobjectDir5 * _OffsetIntensity * 0.1 );
			v.vertex.w = 1;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float4 uvs_DissolveTex = i.uv_texcoord;
			uvs_DissolveTex.xy = i.uv_texcoord.xy * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
			float4 tex2DNode91 = tex2D( _DissolveTex, uvs_DissolveTex.xy );
			#ifdef _ISONMINUSDISSOLVE_ON
				float4 staticSwitch112 = ( 1.0 - tex2DNode91 );
			#else
				float4 staticSwitch112 = tex2DNode91;
			#endif
			float4 temp_cast_1 = (i.uv_texcoord.w).xxxx;
			c.rgb = 0;
			c.a = 1;
			clip( step( staticSwitch112 , temp_cast_1 ).r - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float temp_output_89_0 = ( _Clip + i.uv_texcoord.xyz.z );
			float Clip58 = temp_output_89_0;
			float4 uvs_Cell = i.uv_texcoord;
			uvs_Cell.xy = i.uv_texcoord.xy * _Cell_ST.xy + _Cell_ST.zw;
			float2 UV17 = ( uvs_Cell.xy * _Tiling );
			float cell20 = tex2D( _Cell, UV17 ).r;
			float temp_output_40_0 = step( temp_output_89_0 , cell20 );
			float smoothstepResult49 = smoothstep( 0.0 , temp_output_89_0 , cell20);
			float2 appendResult55 = (float2(( (1.0 + (Clip58 - 0.0) * (-0.35 - 1.0) / (1.0 - 0.0)) + ( ( ( 1.0 - temp_output_40_0 ) * smoothstepResult49 ) * 0.6666 ) ) , 0.5));
			float4 temp_output_71_0 = ( tex2D( _Ramp, appendResult55 ) * _EmisIntensity );
			float4 SmokeColor31 = ( temp_output_40_0 * i.vertexColor );
			float SmokeMask63 = temp_output_40_0;
			#ifdef _SMOKE_ON
				float4 staticSwitch111 = ( ( SmokeColor31 * SmokeMask63 ) + ( ( 1.0 - SmokeMask63 ) * temp_output_71_0 ) );
			#else
				float4 staticSwitch111 = temp_output_71_0;
			#endif
			float4 ULColor75 = staticSwitch111;
			#ifdef _ISSINGLECOLOR_ON
				float4 staticSwitch113 = _SingleColor;
			#else
				float4 staticSwitch113 = ULColor75;
			#endif
			o.Emission = staticSwitch113.rgb;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1280;0;1280;659;195.1333;184.4763;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-2796.402,-84.21197;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-2655.18,114.7898;Inherit;False;Property;_Tiling;Tiling;1;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2516.465,-9.179681;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-2228.391,-23.94708;Inherit;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-1884.292,140.3196;Inherit;False;17;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1900.726,-81.7156;Inherit;True;Property;_Cell;Cell;4;0;Create;True;0;0;0;False;0;False;aec90e026d4440943857ec6ccdbd34a7;aec90e026d4440943857ec6ccdbd34a7;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;12;-1599.196,54.65249;Inherit;True;Property;_TextureSample3;Texture Sample 3;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;90;-3153.848,1049.524;Inherit;False;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;25;-3105.415,914.9274;Inherit;False;Property;_Clip;Clip;6;0;Create;True;0;0;0;False;0;False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-1256.933,102.0915;Inherit;False;cell;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;89;-2815.848,1005.524;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-3138.805,622.9213;Inherit;False;20;cell;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;40;-2411.181,500.6044;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-2722.173,814.3787;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;52;-2330.627,673.4662;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;49;-2498.466,824.9709;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-2619.835,353.6067;Inherit;False;Clip;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-2217.698,917.5453;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;-2005.064,998.7902;Inherit;False;58;Clip;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-2015.259,1115.811;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.6666;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;72;-1740.014,1062.887;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;-0.35;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-2014.016,1375.633;Inherit;False;Constant;_Float3;Float 3;9;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-1480.429,1055.679;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;97;-1914.551,785.2761;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;55;-1352.419,1286.982;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1658.352,736.9606;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-2163.706,322.5833;Inherit;False;SmokeMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;48;-1130.021,1336.978;Inherit;True;Property;_Ramp;Ramp;8;0;Create;True;0;0;0;False;0;False;-1;f36cf6420ef9b254a93dc06f18a9d4b7;f36cf6420ef9b254a93dc06f18a9d4b7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;65;-1071.307,1008.091;Inherit;False;63;SmokeMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-1379.386,630.3314;Inherit;False;SmokeColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-906.7903,1520.675;Inherit;False;Property;_EmisIntensity;EmisIntensity;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;69;-857.025,1139.18;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-703.0154,1338.442;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;64;-1073.126,834.2149;Inherit;False;31;SmokeColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-552.1503,1207.346;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-738.8544,910.925;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-911.4766,93.13728;Inherit;False;Constant;_Float1;Float 1;11;0;Create;True;0;0;0;False;0;False;0.74;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-904.3383,664.6788;Inherit;False;Property;_NormalScale;NormalScale;5;0;Create;True;0;0;0;False;0;False;5.71;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;101;-1193.02,287.5898;Inherit;False;17;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;102;-720.1786,227.9227;Inherit;False;NormalCreate;2;;1;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.5;False;4;FLOAT;2;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;95;-570.6032,0.08229995;Inherit;False;0;91;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-308.1152,1064.45;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;110;-520.3122,304.8818;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;106;-655.9301,644.7882;Inherit;False;20;cell;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;91;-249.214,56.47874;Inherit;True;Property;_DissolveTex;DissolveTex;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;111;-96.64378,1263.388;Inherit;False;Property;_Smoke;Smoke;11;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;96;85.53432,114.9191;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-14.57184,1089.134;Inherit;False;ULColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-396.2801,559.5492;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;335.5768,100.8644;Inherit;False;75;ULColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;112;109.5722,-37.75257;Inherit;False;Property;_IsOnminusDissolve;IsOnminusDissolve;12;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-58.58346,850.5834;Inherit;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-92.18349,632.9836;Inherit;False;Property;_OffsetIntensity;OffsetIntensity;7;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-89.66154,316.7799;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TransformDirectionNode;5;-191.8168,440.8928;Inherit;False;Tangent;Object;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;114;331.3667,165.2737;Inherit;False;Property;_SingleColor;SingleColor;14;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;92;298.7676,231.2342;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;113;539.3667,69.2737;Inherit;False;Property;_IsSingleColor;IsSingleColor;13;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-968.7938,375.2312;Inherit;False;Constant;_Float4;Float 4;11;0;Create;True;0;0;0;False;0;False;7.66;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;209.0433,458.2161;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;773.4927,-29.26611;Float;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;Boom;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Off;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;AlphaTest;ForwardOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1950.726,-131.7157;Inherit;False;939.933;387.4352;Cell;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;21;-2580.285,-116.7049;Inherit;False;540.4263;231.3692;UV;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;32;-2685.476,274.6636;Inherit;False;1225.09;662.439;SmokeColor;0;;1,1,1,1;0;0
WireConnection;39;0;16;0
WireConnection;39;1;38;0
WireConnection;17;0;39;0
WireConnection;12;0;1;0
WireConnection;12;1;18;0
WireConnection;20;0;12;1
WireConnection;89;0;25;0
WireConnection;89;1;90;3
WireConnection;40;0;89;0
WireConnection;40;1;23;0
WireConnection;52;0;40;0
WireConnection;49;0;23;0
WireConnection;49;1;50;0
WireConnection;49;2;89;0
WireConnection;58;0;89;0
WireConnection;51;0;52;0
WireConnection;51;1;49;0
WireConnection;54;0;51;0
WireConnection;72;0;59;0
WireConnection;57;0;72;0
WireConnection;57;1;54;0
WireConnection;55;0;57;0
WireConnection;55;1;56;0
WireConnection;28;0;40;0
WireConnection;28;1;97;0
WireConnection;63;0;40;0
WireConnection;48;1;55;0
WireConnection;31;0;28;0
WireConnection;69;0;65;0
WireConnection;71;0;48;0
WireConnection;71;1;70;0
WireConnection;68;0;69;0
WireConnection;68;1;71;0
WireConnection;66;0;64;0
WireConnection;66;1;65;0
WireConnection;102;1;1;0
WireConnection;102;2;101;0
WireConnection;102;3;103;0
WireConnection;102;4;34;0
WireConnection;67;0;66;0
WireConnection;67;1;68;0
WireConnection;110;0;102;0
WireConnection;91;1;95;0
WireConnection;111;1;71;0
WireConnection;111;0;67;0
WireConnection;96;0;91;0
WireConnection;75;0;111;0
WireConnection;107;0;110;0
WireConnection;107;1;106;0
WireConnection;112;1;91;0
WireConnection;112;0;96;0
WireConnection;5;0;107;0
WireConnection;92;0;112;0
WireConnection;92;1;94;4
WireConnection;113;1;76;0
WireConnection;113;0;114;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;6;2;8;0
WireConnection;0;2;113;0
WireConnection;0;10;92;0
WireConnection;0;11;6;0
ASEEND*/
//CHKSM=090EA7AD1ECAAAB6ABA759236DA8264447B7E103