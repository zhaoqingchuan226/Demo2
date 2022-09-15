// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_Draw"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,0.3544474,0.3544474,0)
		[Toggle(_ISMRAE_ON)] _isMRAE("isMRAE", Float) = 1
		_MRAE("MRAE", 2D) = "white" {}
		_Roughness("Roughness", Range( -1 , 1)) = 1
		_DrawColor("DrawColor", Color) = (1,1,1,0)
		_Metalic("Metalic", Range( -1 , 1)) = 1
		[Toggle(_ISNORMAL_ON)] _isNormal("isNormal", Float) = 0
		_Normal("Normal", 2D) = "bump" {}
		[Toggle(_ISEMISSIVE_ON)] _IsEmissive("IsEmissive", Float) = 0
		[HDR]_EColor("EColor", Color) = (1,1,1,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ISNORMAL_ON
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
			float3 vertexToFrag8_g7;
			float3 worldPos;
			float3 vertexToFrag9_g7;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _CullMode;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _DrawColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform sampler2D _Hatch0;
		uniform sampler2D _Hatch1;
		uniform sampler2D _Hatch2;
		uniform sampler2D _Hatch3;
		uniform sampler2D _Hatch4;
		uniform sampler2D _Hatch5;
		uniform float4 _EColor;
		uniform sampler2D _MRAE;
		uniform float4 _MRAE_ST;
		uniform float _Metalic;
		uniform float _Roughness;


		float3 MyCustomExpression6_g7( float hatchFactor )
		{
			fixed3 hatchWeights0 = fixed3(0, 0, 0);
							
			fixed3 hatchWeights1 = fixed3(0, 0, 0);
						
			if (hatchFactor > 6.0)
			 {
			} 
			else if (hatchFactor > 5.0)
			 {
			hatchWeights0.x = hatchFactor - 5.0;
			} 
			else if (hatchFactor > 4.0) {
							hatchWeights0.x = hatchFactor - 4.0;
			hatchWeights0.y = 1.0 - hatchWeights0.x;
			} else if (hatchFactor > 3.0)
			 {
			hatchWeights0.y = hatchFactor - 3.0;
			hatchWeights0.z = 1.0 - hatchWeights0.y;
			} 
			else if (hatchFactor > 2.0)
			 {
						hatchWeights0.z = hatchFactor - 2.0;
							
			} 
			return hatchWeights0;
		}


		float3 MyCustomExpression7_g7( float hatchFactor )
		{
			fixed3 hatchWeights0 = fixed3(0, 0, 0);
							
			fixed3 hatchWeights1 = fixed3(0, 0, 0);
						
			 if (hatchFactor > 2.0)
			 {
						hatchWeights0.z = hatchFactor - 2.0;
							hatchWeights1.x = 1.0 - hatchWeights0.z;
			} 
			else if (hatchFactor > 1.0) {
						
			hatchWeights1.x = hatchFactor -1.0;
							hatchWeights1.y = 1.0 -hatchWeights1.x;
			} 
			else {
							hatchWeights1.y = hatchFactor;
			hatchWeights1.z = 1.0 - hatchWeights1.y;
			}
			return hatchWeights1;
		}


		inline float4 TriplanarSampling46_g7( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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


		inline float4 TriplanarSampling49_g7( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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


		inline float4 TriplanarSampling50_g7( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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


		inline float4 TriplanarSampling51_g7( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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


		inline float4 TriplanarSampling52_g7( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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


		inline float4 TriplanarSampling53_g7( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult3_g7 = dot( ase_worldlightDir , ase_worldNormal );
			float temp_output_5_0_g7 = ( saturate( (dotResult3_g7*0.5 + 0.5) ) * 7.0 );
			float hatchFactor6_g7 = temp_output_5_0_g7;
			float3 localMyCustomExpression6_g7 = MyCustomExpression6_g7( hatchFactor6_g7 );
			o.vertexToFrag8_g7 = localMyCustomExpression6_g7;
			float hatchFactor7_g7 = temp_output_5_0_g7;
			float3 localMyCustomExpression7_g7 = MyCustomExpression7_g7( hatchFactor7_g7 );
			o.vertexToFrag9_g7 = localMyCustomExpression7_g7;
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
			float3 hatchWeights011_g7 = i.vertexToFrag8_g7;
			float3 break17_g7 = hatchWeights011_g7;
			float3 hatchWeights110_g7 = i.vertexToFrag9_g7;
			float3 break19_g7 = hatchWeights110_g7;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 temp_output_48_0_g7 = ( 1.0 * ase_vertex3Pos );
			float4 triplanar46_g7 = TriplanarSampling46_g7( _Hatch0, temp_output_48_0_g7, ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float4 triplanar49_g7 = TriplanarSampling49_g7( _Hatch1, temp_output_48_0_g7, ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float4 triplanar50_g7 = TriplanarSampling50_g7( _Hatch2, temp_output_48_0_g7, ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float4 triplanar51_g7 = TriplanarSampling51_g7( _Hatch3, temp_output_48_0_g7, ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float4 triplanar52_g7 = TriplanarSampling52_g7( _Hatch4, temp_output_48_0_g7, ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float4 triplanar53_g7 = TriplanarSampling53_g7( _Hatch5, temp_output_48_0_g7, ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float4 lerpResult116 = lerp( _DrawColor , switchResult52 , saturate( ( ( 1.0 - ( break17_g7.x + break17_g7.y + break17_g7.z + break19_g7.x + break19_g7.y + break19_g7.z ) ) + ( break17_g7.x * triplanar46_g7.x ) + ( break17_g7.y * triplanar49_g7.x ) + ( break17_g7.z * triplanar50_g7.x ) + ( break19_g7.x * triplanar51_g7.x ) + ( break19_g7.y * triplanar52_g7.x ) + ( break19_g7.z * triplanar53_g7.x ) ) ));
			o.Albedo = lerpResult116.rgb;
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
				float4 staticSwitch33 = ( _EColor * break43.w );
			#else
				float4 staticSwitch33 = temp_cast_1;
			#endif
			o.Emission = staticSwitch33.rgb;
			o.Metallic = saturate( ( break43.x + _Metalic ) );
			o.Smoothness = ( 1.0 - saturate( ( break43.y + _Roughness ) ) );
			o.Occlusion = break43.z;
			o.Alpha = 1;
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
			#pragma target 3.0
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
				float3 customPack3 : TEXCOORD3;
				float3 customPack4 : TEXCOORD4;
				float4 tSpace0 : TEXCOORD5;
				float4 tSpace1 : TEXCOORD6;
				float4 tSpace2 : TEXCOORD7;
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
				o.customPack3.xyz = customInputData.vertexToFrag8_g7;
				o.customPack4.xyz = customInputData.vertexToFrag9_g7;
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
				surfIN.vertexToFrag8_g7 = IN.customPack3.xyz;
				surfIN.vertexToFrag9_g7 = IN.customPack4.xyz;
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
-1274;12;1280;648;3023.986;1259.175;4.113796;True;False
Node;AmplifyShaderEditor.SamplerNode;21;-1456.483,-273.5691;Inherit;True;Property;_MRAE;MRAE;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;41;-1056.318,-120.6211;Inherit;False;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.45;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;63;-955.9695,-197.2466;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;57;-1123.061,-393.2682;Inherit;False;Constant;_Vector0;Vector 0;10;0;Create;True;0;0;0;False;0;False;0,0,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;56;-871.5573,-357.7544;Inherit;False;Property;_isMRAE;isMRAE;11;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;18;-251.793,-363.0797;Inherit;False;Property;_Color;Color;10;0;Create;True;0;0;0;False;0;False;1,0.3544474,0.3544474,0;1,0.6943396,0.6943396,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;30;-170.0925,-585.269;Inherit;True;Property;_Albedo;Albedo;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;60;82.31779,353.8889;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;285.1206,-213.9524;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-657.1017,427.3165;Inherit;False;Property;_Roughness;Roughness;13;0;Create;True;0;0;0;False;0;False;1;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;384.5384,-359.8366;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;43;-685.0132,-95.21561;Inherit;False;COLOR;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;540.4073,-95.78464;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TransformDirectionNode;61;268.5577,285.205;Inherit;False;Object;Tangent;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;25;-780.1155,181.4457;Inherit;False;Property;_Metalic;Metalic;15;0;Create;True;0;0;0;False;0;False;1;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;-375.6716,-166.7028;Inherit;False;Property;_EColor;EColor;19;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-349.054,337.9617;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;62;466.6127,313.1176;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-20.06026,-182.2785;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-217.3945,62.85425;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;24;-229.0494,327.6592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;118;1029.363,251.6064;Inherit;False;Property;_DrawColor;DrawColor;14;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.3490565,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwitchByFaceNode;52;911.8527,-81.90244;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;48;409.1035,546.3427;Inherit;True;Property;_Normal;Normal;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;10.10856,-58.13164;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;125;1057.811,410.3477;Inherit;False;Draw;2;;7;35fcda0628d141442a63a797c9a22de9;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;33;279.2767,-12.06483;Inherit;False;Property;_IsEmissive;IsEmissive;18;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;68;1412.308,-117.8798;Inherit;False;Constant;_Float1;Float 1;12;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-961.5894,467.1856;Inherit;False;Property;_CullMode;CullMode;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;20;9.302736,226.6291;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;69;1113.736,-167.0661;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;58;831.5099,224.9967;Inherit;False;Property;_isNormal;isNormal;16;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;116;1299.515,328.9663;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;70;1305.098,-258.9269;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;66;2650.513,-180.5845;Inherit;True;Spherical;World;False;Top Texture 6;_TopTexture6;white;0;None;Mid Texture 6;_MidTexture6;white;-1;None;Bot Texture 6;_BotTexture6;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;28;38.23729,60.4586;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1495.223,36.12052;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Standard_Draw;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;DeferredOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;32;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;41;0;21;0
WireConnection;63;0;41;0
WireConnection;56;1;57;0
WireConnection;56;0;63;0
WireConnection;31;0;30;0
WireConnection;31;1;18;0
WireConnection;43;0;56;0
WireConnection;54;0;31;0
WireConnection;54;1;53;0
WireConnection;61;0;60;0
WireConnection;23;0;43;1
WireConnection;23;1;19;0
WireConnection;62;0;61;0
WireConnection;27;0;43;0
WireConnection;27;1;25;0
WireConnection;24;0;23;0
WireConnection;52;0;31;0
WireConnection;52;1;54;0
WireConnection;37;0;36;0
WireConnection;37;1;43;3
WireConnection;33;1;34;0
WireConnection;33;0;37;0
WireConnection;20;0;24;0
WireConnection;69;0;70;0
WireConnection;58;1;62;0
WireConnection;58;0;48;0
WireConnection;116;0;118;0
WireConnection;116;1;52;0
WireConnection;116;2;125;0
WireConnection;66;9;70;0
WireConnection;66;3;68;0
WireConnection;28;0;27;0
WireConnection;0;0;116;0
WireConnection;0;1;58;0
WireConnection;0;2;33;0
WireConnection;0;3;28;0
WireConnection;0;4;20;0
WireConnection;0;5;43;2
ASEEND*/
//CHKSM=996743F6F9B55DE738163743C61925BFEE6E7421