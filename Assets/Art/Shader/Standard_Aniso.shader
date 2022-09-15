// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_Aniso"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0)
		_MRA("MRA", 2D) = "white" {}
		_Roughness("Roughness", Range( -1 , 1)) = 1
		_Metalic("Metalic", Range( -1 , 1)) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		_SpecularColor1("SpecularColor1", Color) = (1,1,1,0)
		_SpecularColor2("SpecularColor2", Color) = (1,1,1,0)
		_SpecularOffset1("SpecularOffset1", Float) = 0
		_SpecularOffset2("SpecularOffset2", Float) = 0
		_SpecularShiniess1("SpecularShiniess1", Float) = 1
		_SpecularShiniess2("SpecularShiniess2", Float) = 1
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
		#pragma target 4.6
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
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 vertexToFrag38;
			float3 worldPos;
		};

		uniform float _CullMode;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform float _SpecularOffset1;
		uniform float _SpecularShiniess1;
		uniform float4 _SpecularColor1;
		uniform float _SpecularOffset2;
		uniform float _SpecularShiniess2;
		uniform float4 _SpecularColor2;
		uniform float _Metalic;
		uniform sampler2D _MRA;
		uniform float4 _MRA_ST;
		uniform float _Roughness;


		float Test59(  )
		{
			return unity_WorldTransformParams.w;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 ase_worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
			float4 ase_vertexTangent = v.tangent;
			float localTest59 = Test59();
			o.vertexToFrag38 = ( cross( ase_worldNormal , ase_worldTangent ) * ase_vertexTangent.w * localTest59 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = ( tex2D( _Albedo, uv_Albedo ) * _Color ).rgb;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float3 Normal28 = ase_normWorldNormal;
			float3 BiNormal39 = i.vertexToFrag38;
			float3 normalizeResult112 = normalize( ( ( Normal28 * _SpecularOffset1 ) + BiNormal39 ) );
			float3 worldSpaceViewDir68 = WorldSpaceViewDir( float4( 0,0,0,1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult72 = normalize( ( worldSpaceViewDir68 + ase_worldlightDir ) );
			float3 H73 = normalizeResult72;
			float dotResult113 = dot( normalizeResult112 , H73 );
			float BdotH1117 = ( dotResult113 / _SpecularShiniess1 );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 Tangent29 = ase_worldTangent;
			float dotResult81 = dot( Tangent29 , H73 );
			float TdotH79 = dotResult81;
			float3 L94 = ase_worldlightDir;
			float dotResult92 = dot( Normal28 , L94 );
			float halfLambert88 = ( ( 0.5 * saturate( dotResult92 ) ) + 0.5 );
			float aniso_atten133 = saturate( sqrt( max( halfLambert88 , 0.0 ) ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 spec_term1118 = (( ( exp( -( ( BdotH1117 * BdotH1117 ) + ( TdotH79 * TdotH79 ) ) ) * aniso_atten133 ) * ase_lightColor * _SpecularColor1 )).rgb;
			float3 normalizeResult155 = normalize( ( ( Normal28 * _SpecularOffset2 ) + BiNormal39 ) );
			float dotResult156 = dot( normalizeResult155 , H73 );
			float BdotH2158 = ( dotResult156 / _SpecularShiniess2 );
			float3 spec_term2172 = (( exp( -( ( BdotH2158 * BdotH2158 ) + ( TdotH79 * TdotH79 ) ) ) * aniso_atten133 * ase_lightColor * _SpecularColor2 )).rgb;
			o.Emission = ( spec_term1118 + spec_term2172 );
			float2 uv_MRA = i.uv_texcoord * _MRA_ST.xy + _MRA_ST.zw;
			float4 tex2DNode202 = tex2D( _MRA, uv_MRA );
			o.Metallic = saturate( ( _Metalic + tex2DNode202.r ) );
			o.Smoothness = ( 1.0 - saturate( ( tex2DNode202.g + _Roughness ) ) );
			o.Occlusion = tex2DNode202.b;
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
			#pragma target 4.6
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
				float2 customPack1 : TEXCOORD1;
				float3 customPack2 : TEXCOORD2;
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
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyz = customInputData.vertexToFrag38;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.vertexToFrag38 = IN.customPack2.xyz;
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
-1462.857;0;1462.857;761.8572;4982.485;-1299.916;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;98;-3147.021,-1158.514;Inherit;False;1163.34;754.1037;TBN;11;55;28;46;24;58;29;57;59;38;39;177;;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexTangentNode;55;-3072.013,-790.5971;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;46;-3097.021,-985.7112;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TangentVertexDataNode;57;-3162.235,-601.9814;Inherit;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CrossProductOpNode;24;-2788.782,-896.7532;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;59;-2689.265,-615.2047;Float;False;return unity_WorldTransformParams.w@;1;Create;0;Test;True;False;0;;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;99;-6383.757,484.1707;Inherit;False;1155.124;1800.66;Pre;27;70;94;95;91;92;89;97;90;96;88;78;84;77;79;85;82;76;83;81;80;68;69;71;73;72;74;86;;0.8018868,0.2766617,0.2766617,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;70;-6159.756,1092.171;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-2585.057,-890.573;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;177;-3041.03,-1118.553;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceViewDirHlpNode;68;-6127.756,868.1707;Inherit;False;1;0;FLOAT4;0,0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexToFragmentNode;38;-2496.517,-1017.541;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-2807.735,-1130.973;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;145;-5071.756,564.1707;Inherit;False;1572.776;655.4864;BdotH1;11;102;101;111;103;110;112;114;113;115;116;117;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;-5935.756,1172.171;Inherit;False;L;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-5007.757,756.1708;Inherit;False;Property;_SpecularOffset1;SpecularOffset1;8;0;Create;True;0;0;0;False;0;False;0;0.54;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;101;-4959.757,612.1707;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-2207.109,-882.079;Inherit;False;BiNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;-6335.757,2164.17;Inherit;False;94;L;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-5743.756,1028.171;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-6271.757,2004.171;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-4495.757,740.1708;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;72;-5615.757,1140.171;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-5002.968,1554.44;Inherit;False;Property;_SpecularOffset2;SpecularOffset2;9;0;Create;True;0;0;0;False;0;False;0;0.54;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;-4959.757,1348.171;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;111;-4591.757,1044.171;Inherit;False;39;BiNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;92;-6143.756,2116.171;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-5455.757,1028.171;Inherit;False;H;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;97;-6031.756,2132.17;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;110;-4335.757,852.1707;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;-4575.757,1780.171;Inherit;False;39;BiNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;-4479.757,1476.171;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-6159.756,1924.171;Inherit;False;Constant;_Float3;Float 3;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-4239.757,980.1708;Inherit;False;73;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-2803.494,-686.733;Inherit;False;Tangent;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;112;-4239.757,756.1708;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;154;-4335.757,1588.171;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-5951.756,1924.171;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;113;-4079.757,884.1707;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;-4223.757,1700.171;Inherit;False;73;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-5807.756,2004.171;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-6159.756,1476.171;Inherit;False;29;Tangent;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-6159.756,1572.171;Inherit;False;73;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-4175.757,1108.171;Inherit;False;Property;_SpecularShiniess1;SpecularShiniess1;10;0;Create;True;0;0;0;False;0;False;1;0.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;155;-4239.757,1492.171;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-5679.757,1940.171;Inherit;False;halfLambert;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;156;-4063.758,1604.171;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-4175.757,1844.171;Inherit;False;Property;_SpecularShiniess2;SpecularShiniess2;11;0;Create;True;0;0;0;False;0;False;1;0.12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;81;-5935.756,1524.171;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;116;-3903.758,804.1708;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;129;-5415.136,2442.228;Inherit;False;88;halfLambert;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-3727.757,884.1707;Inherit;False;BdotH1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;157;-3903.758,1540.171;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-5711.757,1540.171;Inherit;False;TdotH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;-3423.758,836.1708;Inherit;False;79;TdotH;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;130;-5143.136,2426.228;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;-3727.757,1604.171;Inherit;False;BdotH2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;124;-3295.758,692.1707;Inherit;False;117;BdotH1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;-3263.758,1508.171;Inherit;False;158;BdotH2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;131;-5015.136,2538.228;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-3183.758,932.1707;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;-3391.758,1652.171;Inherit;False;79;TdotH;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-3103.758,724.1707;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;132;-4871.136,2570.228;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;-3167.758,1748.171;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-3087.758,1524.171;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;123;-2943.758,788.1708;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;165;-2927.758,1604.171;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;133;-5407.757,2468.17;Inherit;False;aniso_atten;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;126;-2799.758,804.1708;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;166;-2783.758,1620.171;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ExpOpNode;120;-2639.758,788.1708;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;-2751.758,932.1707;Inherit;False;133;aniso_atten;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ExpOpNode;168;-2623.758,1604.171;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;138;-2511.758,1108.171;Inherit;False;Property;_SpecularColor1;SpecularColor1;6;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.7830188,0.7830188,0.7830188,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;170;-2463.758,1924.171;Inherit;False;Property;_SpecularColor2;SpecularColor2;7;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.5283019,0.5283019,0.5283019,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;134;-2719.758,1060.171;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;167;-2687.758,1860.171;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;169;-2735.758,1732.171;Inherit;False;133;aniso_atten;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;196;-2495.758,804.1708;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;-2399.758,1604.171;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-2335.758,836.1708;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;202;344.3166,-478.9789;Inherit;True;Property;_MRA;MRA;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;570.8876,-194.2514;Inherit;False;Property;_Roughness;Roughness;3;0;Create;True;0;0;0;False;0;False;1;0.494;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;137;-2111.758,980.1708;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;171;-2159.758,1764.171;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;-2095.758,1604.171;Inherit;False;spec_term2;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;118;-2111.758,804.1708;Inherit;False;spec_term1;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;201;625.1588,-560.5926;Inherit;False;Property;_Metalic;Metalic;4;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;205;806.9566,-307.7547;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;198;1184.348,-984.9769;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;206;986.9801,-297.2534;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;203;911.9705,-436.7715;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;1071.799,-484.9858;Inherit;False;172;spec_term2;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;142;1067.409,-577.7906;Inherit;False;118;spec_term1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;199;1242.635,-776.699;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,0.3544472,0.3544472,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;85;-6143.756,1780.171;Inherit;False;69;V;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;185;622.3777,-758.2657;Inherit;False;183;TangentNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;174;1318.457,-528.2468;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;78;-5903.756,1332.171;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;83;-5919.756,1716.171;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;176;-3852.53,-905.8436;Inherit;True;Property;_Normal;Normal;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-5727.757,1316.171;Inherit;False;NdotH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;204;1063.49,-376.7637;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;208;-6951.21,-727.0165;Inherit;False;Property;_CullMode;CullMode;5;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;86;-5695.757,1732.171;Inherit;False;NdotV;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;-6111.756,1284.171;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;181;-4138.471,-1113.699;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-5455.757,868.1707;Inherit;False;V;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-3533.255,-870.2579;Inherit;False;TangentNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;-3390.305,-1007.672;Inherit;False;183;TangentNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;207;1140,-279.251;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;200;1588.435,-732.4989;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-6111.756,1380.171;Inherit;False;73;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-6143.756,1668.171;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1731.769,-585.1406;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Standard_Aniso;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;DeferredOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;208;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;24;0;46;0
WireConnection;24;1;55;0
WireConnection;58;0;24;0
WireConnection;58;1;57;4
WireConnection;58;2;59;0
WireConnection;38;0;58;0
WireConnection;28;0;177;0
WireConnection;94;0;70;0
WireConnection;39;0;38;0
WireConnection;71;0;68;0
WireConnection;71;1;70;0
WireConnection;103;0;101;0
WireConnection;103;1;102;0
WireConnection;72;0;71;0
WireConnection;92;0;91;0
WireConnection;92;1;95;0
WireConnection;73;0;72;0
WireConnection;97;0;92;0
WireConnection;110;0;103;0
WireConnection;110;1;111;0
WireConnection;153;0;150;0
WireConnection;153;1;148;0
WireConnection;29;0;55;0
WireConnection;112;0;110;0
WireConnection;154;0;153;0
WireConnection;154;1;152;0
WireConnection;90;0;89;0
WireConnection;90;1;97;0
WireConnection;113;0;112;0
WireConnection;113;1;114;0
WireConnection;96;0;90;0
WireConnection;96;1;89;0
WireConnection;155;0;154;0
WireConnection;88;0;96;0
WireConnection;156;0;155;0
WireConnection;156;1;159;0
WireConnection;81;0;80;0
WireConnection;81;1;82;0
WireConnection;116;0;113;0
WireConnection;116;1;115;0
WireConnection;117;0;116;0
WireConnection;157;0;156;0
WireConnection;157;1;160;0
WireConnection;79;0;81;0
WireConnection;130;0;129;0
WireConnection;158;0;157;0
WireConnection;131;0;130;0
WireConnection;122;0;121;0
WireConnection;122;1;121;0
WireConnection;125;0;124;0
WireConnection;125;1;124;0
WireConnection;132;0;131;0
WireConnection;164;0;162;0
WireConnection;164;1;162;0
WireConnection;163;0;161;0
WireConnection;163;1;161;0
WireConnection;123;0;125;0
WireConnection;123;1;122;0
WireConnection;165;0;163;0
WireConnection;165;1;164;0
WireConnection;133;0;132;0
WireConnection;126;0;123;0
WireConnection;166;0;165;0
WireConnection;120;0;126;0
WireConnection;168;0;166;0
WireConnection;196;0;120;0
WireConnection;196;1;128;0
WireConnection;173;0;168;0
WireConnection;173;1;169;0
WireConnection;173;2;167;0
WireConnection;173;3;170;0
WireConnection;127;0;196;0
WireConnection;127;1;134;0
WireConnection;127;2;138;0
WireConnection;137;0;127;0
WireConnection;171;0;173;0
WireConnection;172;0;171;0
WireConnection;118;0;137;0
WireConnection;205;0;202;2
WireConnection;205;1;19;0
WireConnection;206;0;205;0
WireConnection;203;0;201;0
WireConnection;203;1;202;1
WireConnection;174;0;142;0
WireConnection;174;1;175;0
WireConnection;78;0;76;0
WireConnection;78;1;77;0
WireConnection;83;0;84;0
WireConnection;83;1;85;0
WireConnection;176;1;181;0
WireConnection;74;0;78;0
WireConnection;204;0;203;0
WireConnection;86;0;83;0
WireConnection;69;0;68;0
WireConnection;183;0;176;0
WireConnection;207;0;206;0
WireConnection;200;0;198;0
WireConnection;200;1;199;0
WireConnection;0;0;200;0
WireConnection;0;2;174;0
WireConnection;0;3;204;0
WireConnection;0;4;207;0
WireConnection;0;5;202;3
ASEEND*/
//CHKSM=FB28E7568BCA4C9DE4FC95959627B873ED10599D