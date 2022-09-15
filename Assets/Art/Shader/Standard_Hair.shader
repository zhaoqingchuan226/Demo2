// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_Hair"
{
	Properties
	{
		_Roughness("Roughness", Range( 0 , 1)) = 1
		_AlbedoColor1("AlbedoColor1", Color) = (0.8396226,0.8396226,0.8396226,0)
		_SpecularNoise1("SpecularNoise1", Float) = 0
		_SpecularNoise2("SpecularNoise2", Float) = 0
		_SpecularColor1("SpecularColor1", Color) = (1,1,1,0)
		_SpecularColor2("SpecularColor2", Color) = (1,1,1,0)
		_SpecularOffset1("SpecularOffset1", Float) = 0
		_SpecularOffset2("SpecularOffset2", Float) = 0
		_SpecularShiniess1("SpecularShiniess1", Float) = 1
		_SpecularShiniess2("SpecularShiniess2", Float) = 1
		_Normal("Normal", 2D) = "bump" {}
		_AO("AO", 2D) = "white" {}
		_AnisoNoiseTex("AnisoNoiseTex", 2D) = "black" {}
		_DetailNormalStrength("DetailNormalStrength", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
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
			float2 uv2_texcoord2;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 vertexToFrag38;
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform sampler2D _AnisoNoiseTex;
		uniform float4 _AnisoNoiseTex_ST;
		uniform float _DetailNormalStrength;
		uniform float4 _AlbedoColor1;
		uniform float _SpecularOffset1;
		uniform float _SpecularNoise1;
		uniform float _SpecularShiniess1;
		uniform float4 _SpecularColor1;
		uniform float _SpecularOffset2;
		uniform float _SpecularNoise2;
		uniform float _SpecularShiniess2;
		uniform float4 _SpecularColor2;
		uniform float _Roughness;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;


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
			float2 uv_AnisoNoiseTex = i.uv_texcoord * _AnisoNoiseTex_ST.xy + _AnisoNoiseTex_ST.zw;
			float2 temp_output_2_0_g1 = uv_AnisoNoiseTex;
			float2 break6_g1 = temp_output_2_0_g1;
			float temp_output_25_0_g1 = ( pow( 0.5 , 3.0 ) * 0.1 );
			float2 appendResult8_g1 = (float2(( break6_g1.x + temp_output_25_0_g1 ) , break6_g1.y));
			float4 tex2DNode14_g1 = tex2D( _AnisoNoiseTex, temp_output_2_0_g1 );
			float temp_output_4_0_g1 = _DetailNormalStrength;
			float3 appendResult13_g1 = (float3(1.0 , 0.0 , ( ( tex2D( _AnisoNoiseTex, appendResult8_g1 ).g - tex2DNode14_g1.g ) * temp_output_4_0_g1 )));
			float2 appendResult9_g1 = (float2(break6_g1.x , ( break6_g1.y + temp_output_25_0_g1 )));
			float3 appendResult16_g1 = (float3(0.0 , 1.0 , ( ( tex2D( _AnisoNoiseTex, appendResult9_g1 ).g - tex2DNode14_g1.g ) * temp_output_4_0_g1 )));
			float3 normalizeResult22_g1 = normalize( cross( appendResult13_g1 , appendResult16_g1 ) );
			float3 DetailNormal_T192 = normalizeResult22_g1;
			float3 TangentNormal183 = BlendNormals( UnpackNormal( tex2D( _Normal, i.uv2_texcoord2 ) ) , DetailNormal_T192 );
			o.Normal = TangentNormal183;
			o.Albedo = _AlbedoColor1.rgb;
			float3 Normal28 = normalize( (WorldNormalVector( i , TangentNormal183 )) );
			float4 tex2DNode64 = tex2D( _AnisoNoiseTex, uv_AnisoNoiseTex );
			float4 temp_cast_1 = (0.5).xxxx;
			float aniso_noise65 = (( tex2DNode64 - temp_cast_1 )).r;
			float3 BiNormal39 = i.vertexToFrag38;
			float3 normalizeResult112 = normalize( ( ( Normal28 * ( _SpecularOffset1 + ( _SpecularNoise1 * aniso_noise65 ) ) ) + BiNormal39 ) );
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
			float3 normalizeResult155 = normalize( ( ( Normal28 * ( _SpecularOffset2 + ( _SpecularNoise2 * aniso_noise65 ) ) ) + BiNormal39 ) );
			float dotResult156 = dot( normalizeResult155 , H73 );
			float BdotH2158 = ( dotResult156 / _SpecularShiniess2 );
			float3 spec_term2172 = (( exp( -( ( BdotH2158 * BdotH2158 ) + ( TdotH79 * TdotH79 ) ) ) * aniso_atten133 * ase_lightColor * _SpecularColor2 )).rgb;
			o.Emission = ( spec_term1118 + spec_term2172 );
			o.Metallic = 0.0;
			o.Smoothness = ( 1.0 - _Roughness );
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Occlusion = tex2D( _AO, uv_AO ).r;
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
				float4 customPack1 : TEXCOORD1;
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
				o.customPack1.xy = customInputData.uv2_texcoord2;
				o.customPack1.xy = v.texcoord1;
				o.customPack1.zw = customInputData.uv_texcoord;
				o.customPack1.zw = v.texcoord;
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
				surfIN.uv2_texcoord2 = IN.customPack1.xy;
				surfIN.uv_texcoord = IN.customPack1.zw;
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
-1462.857;0;1462.857;761.8572;3219.566;-88.51302;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;99;-2260.343,111.645;Inherit;False;1155.124;1800.66;Pre;35;70;94;95;91;92;89;97;90;96;88;65;78;84;64;77;66;79;85;82;76;83;81;80;67;68;69;71;73;72;74;86;109;187;190;192;;0.8018868,0.2766617,0.2766617,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;191;-2726.125,308.8392;Inherit;True;Property;_AnisoNoiseTex;AnisoNoiseTex;14;0;Create;True;0;0;0;False;0;False;None;None;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;195;-2761.958,612.5627;Inherit;False;Property;_DetailNormalStrength;DetailNormalStrength;15;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;190;-2450.69,469.9487;Inherit;False;NormalCreate;1;;1;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.5;False;4;FLOAT;2;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-2205.908,501.364;Inherit;False;DetailNormal_T;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;181;-3270.438,-730.3367;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;144;-2542.301,150.485;Inherit;False;0;191;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;67;-1736.074,432.6771;Inherit;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;64;-2272.965,184.645;Inherit;True;Property;_AnisoNoise;AnisoNoise;2;0;Create;True;0;0;0;False;0;False;-1;None;a085526d2d4192f47b41d2fe45fb4623;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;193;-2926.594,-392.5439;Inherit;False;192;DetailNormal_T;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;176;-3040.526,-594.8657;Inherit;True;Property;_Normal;Normal;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;194;-2671.594,-445.5439;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;66;-1590.865,206.8936;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;98;-2238.018,-819.5358;Inherit;False;1163.34;754.1037;TBN;11;55;28;46;24;58;29;57;59;38;39;177;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SwizzleNode;109;-1375.833,322.8236;Inherit;False;FLOAT;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-2467.98,-512.8951;Inherit;False;TangentNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;46;-2188.018,-646.7333;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexTangentNode;55;-2163.01,-451.6193;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CustomExpressionNode;59;-1780.262,-276.2267;Float;False;return unity_WorldTransformParams.w@;1;Create;0;Test;True;False;0;;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;65;-1218.546,237.6232;Inherit;False;aniso_noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;-2388.434,-664.4724;Inherit;False;183;TangentNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;145;-952.0468,198.6328;Inherit;False;1572.776;655.4864;BdotH1;15;107;105;102;106;101;104;111;103;110;112;114;113;115;116;117;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TangentVertexDataNode;57;-2253.232,-263.0035;Inherit;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CrossProductOpNode;24;-1879.779,-557.7753;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;177;-2132.027,-779.5754;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;70;-2029.793,730.4638;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;107;-867.6208,713.8315;Inherit;False;65;aniso_noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-902.0468,568.7695;Inherit;False;Property;_SpecularNoise1;SpecularNoise1;4;0;Create;True;0;0;0;False;0;False;0;2.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-1676.054,-551.5951;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-888.7766,392.9009;Inherit;False;Property;_SpecularOffset1;SpecularOffset1;8;0;Create;True;0;0;0;False;0;False;0;0.54;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;146;-860.144,1446.374;Inherit;False;65;aniso_noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceViewDirHlpNode;68;-1996.717,506.6168;Inherit;False;1;0;FLOAT4;0,0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;-1803.024,797.2402;Inherit;False;L;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-649.4958,607.3221;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;38;-1587.514,-678.5631;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;147;-894.5701,1301.312;Inherit;False;Property;_SpecularNoise2;SpecularNoise2;5;0;Create;True;0;0;0;False;0;False;0;2.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-1897.732,-791.9948;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-881.2999,1125.443;Inherit;False;Property;_SpecularOffset2;SpecularOffset2;9;0;Create;True;0;0;0;False;0;False;0;0.54;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-1298.106,-543.1011;Inherit;False;BiNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;-499.8577,499.664;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;149;-642.019,1339.864;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-2148.817,1637.199;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;101;-840.1949,248.6328;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;-2210.343,1797.592;Inherit;False;94;L;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-1618.73,666.0162;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;72;-1484.933,764.1672;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;-832.7181,981.1752;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;151;-492.3809,1232.206;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-366.9438,370.4103;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;111;-459.0395,673.0212;Inherit;False;39;BiNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;92;-2022.517,1740.2;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-1328.648,665.7491;Inherit;False;H;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;110;-217.0024,485.9276;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-2038.365,1548.424;Inherit;False;Constant;_Float3;Float 3;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;97;-1905.401,1766.334;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;-451.5627,1405.563;Inherit;False;39;BiNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;-359.4669,1102.953;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;154;-209.5256,1218.47;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-109.1103,605.8115;Inherit;False;73;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;112;-118.0306,388.2516;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-1820.365,1547.424;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-1894.491,-347.7551;Inherit;False;Tangent;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-2031.151,1102.535;Inherit;False;29;Tangent;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;113;48.24259,508.4836;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-56.25962,740.4049;Inherit;False;Property;_SpecularShiniess1;SpecularShiniess1;10;0;Create;True;0;0;0;False;0;False;1;0.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-1682.341,1632.591;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;155;-110.5538,1120.794;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-2037.627,1205.531;Inherit;False;73;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;-101.6335,1338.354;Inherit;False;73;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-48.78283,1471.947;Inherit;False;Property;_SpecularShiniess2;SpecularShiniess2;11;0;Create;True;0;0;0;False;0;False;1;0.12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-1549.943,1577.004;Inherit;False;halfLambert;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;81;-1802.366,1151.799;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;116;221.8404,431.0829;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;156;55.71938,1241.026;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;129;-1998.75,1933.498;Inherit;False;88;halfLambert;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-1585.818,1165.364;Inherit;False;TdotH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;397.3009,507.9143;Inherit;False;BdotH1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;157;229.3172,1163.625;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;124;833.7395,325.2947;Inherit;False;117;BdotH1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;403.1661,1240.456;Inherit;False;BdotH2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;710.0092,466.9149;Inherit;False;79;TdotH;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;130;-1732.249,1925.699;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;855.7799,1135.065;Inherit;False;158;BdotH2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;939.0323,565.1651;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;732.0496,1276.685;Inherit;False;79;TdotH;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;1022.798,349.1985;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;131;-1602.249,2034.899;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;123;1179.331,425.3184;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;961.0728,1374.935;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;1044.838,1158.969;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;132;-1455.349,2067.399;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;126;1319.937,435.3656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;165;1201.371,1235.089;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;133;-1281.557,2096.14;Inherit;False;aniso_atten;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;166;1341.978,1245.136;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;1368.096,556.6634;Inherit;False;133;aniso_atten;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ExpOpNode;120;1487.913,423.7189;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;138;1621.578,743.6058;Inherit;False;Property;_SpecularColor1;SpecularColor1;6;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.7830188,0.7830188,0.7830188,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;196;1637.264,428.6281;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;170;1655.219,1550.976;Inherit;False;Property;_SpecularColor2;SpecularColor2;7;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.5283019,0.5283019,0.5283019,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;167;1433.619,1493.376;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ExpOpNode;168;1509.953,1233.489;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;169;1390.136,1366.433;Inherit;False;133;aniso_atten;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;134;1411.578,683.6058;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;1727.151,1241.59;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;1794.11,467.8197;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwizzleNode;171;1962.618,1400.376;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;137;2019.578,611.6058;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;118;2016.547,430.3628;Inherit;False;spec_term1;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;2038.587,1240.133;Inherit;False;spec_term2;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;753.327,-534.1215;Inherit;False;172;spec_term2;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;19;746.4038,-335.2705;Inherit;False;Property;_Roughness;Roughness;0;0;Create;True;0;0;0;False;0;False;1;0.494;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;142;748.9367,-619.6469;Inherit;False;118;spec_term1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;83;-1785.593,1352.733;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;444.092,-638.9517;Inherit;False;88;halfLambert;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;885.6578,-433.5175;Inherit;False;Constant;_Float1;Float 1;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;-1989.134,919.8155;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;186;1169.956,-143.4168;Inherit;True;Property;_AO;AO;13;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;141;794.9367,-752.6469;Inherit;False;Constant;_Float4;Float 4;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-2020.855,1406.465;Inherit;False;69;V;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;197;1299.313,-392.3138;Inherit;False;Constant;_Float6;Float 6;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-2014.379,1303.469;Inherit;False;28;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;86;-1569.046,1366.298;Inherit;False;NdotV;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;63;969.5625,-764.1363;Inherit;False;Property;_AlbedoColor1;AlbedoColor1;3;0;Create;True;0;0;0;False;0;False;0.8396226,0.8396226,0.8396226,0;0.2830189,0.2830189,0.2830189,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;174;976.3269,-590.1215;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;185;1163.271,-656.0742;Inherit;False;183;TangentNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-1849.257,262.9494;Inherit;False;roughness;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;49;310.0959,-450.9824;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-1598.844,942.6697;Inherit;False;NdotH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;78;-1781.817,967.7383;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-1328.728,493.7965;Inherit;False;V;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;20;1166.737,-311.0888;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-1979.507,1016.103;Inherit;False;73;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1671.969,-590.3405;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Standard_Hair;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;DeferredOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;190;1;191;0
WireConnection;190;4;195;0
WireConnection;192;0;190;0
WireConnection;64;0;191;0
WireConnection;64;1;144;0
WireConnection;176;1;181;0
WireConnection;194;0;176;0
WireConnection;194;1;193;0
WireConnection;66;0;64;0
WireConnection;66;1;67;0
WireConnection;109;0;66;0
WireConnection;183;0;194;0
WireConnection;65;0;109;0
WireConnection;24;0;46;0
WireConnection;24;1;55;0
WireConnection;177;0;184;0
WireConnection;58;0;24;0
WireConnection;58;1;57;4
WireConnection;58;2;59;0
WireConnection;94;0;70;0
WireConnection;106;0;105;0
WireConnection;106;1;107;0
WireConnection;38;0;58;0
WireConnection;28;0;177;0
WireConnection;39;0;38;0
WireConnection;104;0;102;0
WireConnection;104;1;106;0
WireConnection;149;0;147;0
WireConnection;149;1;146;0
WireConnection;71;0;68;0
WireConnection;71;1;70;0
WireConnection;72;0;71;0
WireConnection;151;0;148;0
WireConnection;151;1;149;0
WireConnection;103;0;101;0
WireConnection;103;1;104;0
WireConnection;92;0;91;0
WireConnection;92;1;95;0
WireConnection;73;0;72;0
WireConnection;110;0;103;0
WireConnection;110;1;111;0
WireConnection;97;0;92;0
WireConnection;153;0;150;0
WireConnection;153;1;151;0
WireConnection;154;0;153;0
WireConnection;154;1;152;0
WireConnection;112;0;110;0
WireConnection;90;0;89;0
WireConnection;90;1;97;0
WireConnection;29;0;55;0
WireConnection;113;0;112;0
WireConnection;113;1;114;0
WireConnection;96;0;90;0
WireConnection;96;1;89;0
WireConnection;155;0;154;0
WireConnection;88;0;96;0
WireConnection;81;0;80;0
WireConnection;81;1;82;0
WireConnection;116;0;113;0
WireConnection;116;1;115;0
WireConnection;156;0;155;0
WireConnection;156;1;159;0
WireConnection;79;0;81;0
WireConnection;117;0;116;0
WireConnection;157;0;156;0
WireConnection;157;1;160;0
WireConnection;158;0;157;0
WireConnection;130;0;129;0
WireConnection;122;0;121;0
WireConnection;122;1;121;0
WireConnection;125;0;124;0
WireConnection;125;1;124;0
WireConnection;131;0;130;0
WireConnection;123;0;125;0
WireConnection;123;1;122;0
WireConnection;164;0;162;0
WireConnection;164;1;162;0
WireConnection;163;0;161;0
WireConnection;163;1;161;0
WireConnection;132;0;131;0
WireConnection;126;0;123;0
WireConnection;165;0;163;0
WireConnection;165;1;164;0
WireConnection;133;0;132;0
WireConnection;166;0;165;0
WireConnection;120;0;126;0
WireConnection;196;0;120;0
WireConnection;196;1;128;0
WireConnection;168;0;166;0
WireConnection;173;0;168;0
WireConnection;173;1;169;0
WireConnection;173;2;167;0
WireConnection;173;3;170;0
WireConnection;127;0;196;0
WireConnection;127;1;134;0
WireConnection;127;2;138;0
WireConnection;171;0;173;0
WireConnection;137;0;127;0
WireConnection;118;0;137;0
WireConnection;172;0;171;0
WireConnection;83;0;84;0
WireConnection;83;1;85;0
WireConnection;86;0;83;0
WireConnection;174;0;142;0
WireConnection;174;1;175;0
WireConnection;187;0;64;0
WireConnection;74;0;78;0
WireConnection;78;0;76;0
WireConnection;78;1;77;0
WireConnection;69;0;68;0
WireConnection;20;0;19;0
WireConnection;0;0;63;0
WireConnection;0;1;185;0
WireConnection;0;2;174;0
WireConnection;0;3;62;0
WireConnection;0;4;20;0
WireConnection;0;5;186;1
ASEEND*/
//CHKSM=E876C64E23B4E4CC345B83BE6810C75C9CEB9BB1