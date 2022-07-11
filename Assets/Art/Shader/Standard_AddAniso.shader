// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_AddAniso"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,0.3544474,0.3544474,0)
		_MRAE("MRAE", 2D) = "white" {}
		_Roughness("Roughness", Range( -1 , 1)) = 1
		_Metalic("Metalic", Range( -1 , 1)) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		[Toggle(_ISANISO_ON)] _IsAniso("IsAniso", Float) = 1
		[Toggle(_ISEMISSIVE_ON)] _IsEmissive("IsEmissive", Float) = 0
		[HDR]_EColor("EColor", Color) = (1,1,1,1)
		_Color1("Color 1", Color) = (1,1,1,0)
		_Color0("Color 0", Color) = (1,1,1,0)
		_sSpecularOffset1("sSpecularOffset1", Float) = 0
		_sSpecularOffset2("sSpecularOffset2", Float) = 0
		_sSpecularShiniess1("sSpecularShiniess1", Float) = 1
		_sSpecularShiniess2("sSpecularShiniess2", Float) = 1
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
		#pragma shader_feature_local _ISEMISSIVE_ON
		#pragma shader_feature_local _ISANISO_ON
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
			half ASEVFace : VFACE;
			float3 worldNormal;
			INTERNAL_DATA
			float3 vertexToFrag158;
			float3 worldPos;
		};

		uniform float _CullMode;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform float4 _EColor;
		uniform sampler2D _MRAE;
		uniform float4 _MRAE_ST;
		uniform float _sSpecularOffset1;
		uniform float _sSpecularShiniess1;
		uniform float4 _Color1;
		uniform float _sSpecularOffset2;
		uniform float _sSpecularShiniess2;
		uniform float4 _Color0;
		uniform float _Metalic;
		uniform float _Roughness;


		float Test146(  )
		{
			return unity_WorldTransformParams.w;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 ase_worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
			float4 ase_vertexTangent = v.tangent;
			float localTest146 = Test146();
			o.vertexToFrag158 = ( cross( ase_worldNormal , ase_worldTangent ) * ase_vertexTangent.w * localTest146 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 temp_output_31_0 = ( tex2D( _Albedo, uv_Albedo ) * _Color );
			float4 switchResult250 = (((i.ASEVFace>0)?(temp_output_31_0):(( temp_output_31_0 * 0.7 ))));
			o.Albedo = switchResult250.rgb;
			float4 temp_cast_1 = (0.0).xxxx;
			float2 uv_MRAE = i.uv_texcoord * _MRAE_ST.xy + _MRAE_ST.zw;
			float4 saferPower41 = abs( tex2D( _MRAE, uv_MRAE ) );
			float4 break43 = pow( saferPower41 , 0.45 );
			#ifdef _ISEMISSIVE_ON
				float4 staticSwitch33 = ( _EColor * break43.a );
			#else
				float4 staticSwitch33 = temp_cast_1;
			#endif
			float3 temp_cast_2 = (0.0).xxx;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float3 Normal153 = ase_normWorldNormal;
			float3 BiNormal166 = i.vertexToFrag158;
			float3 normalizeResult182 = normalize( ( ( Normal153 * _sSpecularOffset1 ) + BiNormal166 ) );
			float3 worldSpaceViewDir157 = WorldSpaceViewDir( float4( 0,0,0,1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult172 = normalize( ( worldSpaceViewDir157 + ase_worldlightDir ) );
			float3 H174 = normalizeResult172;
			float dotResult186 = dot( normalizeResult182 , H174 );
			float BdotH1196 = ( dotResult186 / _sSpecularShiniess1 );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 Tangent179 = ase_worldTangent;
			float dotResult194 = dot( Tangent179 , H174 );
			float TdotH198 = dotResult194;
			float3 L156 = ase_worldlightDir;
			float dotResult167 = dot( Normal153 , L156 );
			float halfLambert193 = ( ( 0.5 * saturate( dotResult167 ) ) + 0.5 );
			float aniso_atten215 = saturate( sqrt( max( halfLambert193 , 0.0 ) ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 spec_term1230 = (( ( exp( -( ( BdotH1196 * BdotH1196 ) + ( TdotH198 * TdotH198 ) ) ) * aniso_atten215 ) * ase_lightColor * _Color1 )).rgb;
			float3 normalizeResult189 = normalize( ( ( Normal153 * _sSpecularOffset2 ) + BiNormal166 ) );
			float dotResult191 = dot( normalizeResult189 , H174 );
			float BdotH2202 = ( dotResult191 / _sSpecularShiniess2 );
			float3 spec_term2231 = (( exp( -( ( BdotH2202 * BdotH2202 ) + ( TdotH198 * TdotH198 ) ) ) * aniso_atten215 * ase_lightColor * _Color0 )).rgb;
			#ifdef _ISANISO_ON
				float3 staticSwitch245 = ( spec_term1230 + spec_term2231 );
			#else
				float3 staticSwitch245 = temp_cast_2;
			#endif
			o.Emission = ( staticSwitch33 + float4( staticSwitch245 , 0.0 ) ).rgb;
			o.Metallic = saturate( ( break43.r + _Metalic ) );
			o.Smoothness = ( 1.0 - saturate( ( break43.g + _Roughness ) ) );
			o.Occlusion = break43.b;
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
				o.customPack2.xyz = customInputData.vertexToFrag158;
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
				surfIN.vertexToFrag158 = IN.customPack2.xyz;
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
-1456.571;35.42857;1462.857;727.5715;118.3384;365.0372;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;142;-4923.7,2904.624;Inherit;False;1163.34;754.1037;TBN;10;179;166;158;153;150;149;146;145;144;143;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;143;-4873.7,3077.427;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexTangentNode;144;-4848.691,3272.541;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TangentVertexDataNode;147;-4938.914,3461.156;Inherit;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;146;-4465.943,3447.933;Float;False;return unity_WorldTransformParams.w@;1;Create;0;Test;True;False;0;;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.CrossProductOpNode;145;-4565.461,3166.385;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;148;-8160.436,4547.309;Inherit;False;1155.124;1800.66;Pre;27;243;241;240;239;238;237;236;235;234;198;194;193;188;187;185;183;177;174;173;172;167;163;162;159;157;156;151;;0.8018868,0.2766617,0.2766617,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;151;-7936.435,5155.309;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-4361.735,3172.565;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;149;-4817.709,2944.585;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexToFragmentNode;158;-4273.196,3045.597;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;-7712.435,5235.309;Inherit;False;L;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;153;-4584.414,2932.165;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceViewDirHlpNode;157;-7904.435,4931.309;Inherit;False;1;0;FLOAT4;0,0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;152;-6848.435,4627.309;Inherit;False;1572.776;655.4864;BdotH1;11;196;192;190;186;182;180;178;170;168;165;154;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;165;-6736.436,4675.309;Inherit;False;153;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;166;-3983.788,3181.059;Inherit;False;BiNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;163;-8112.436,6227.308;Inherit;False;156;L;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;162;-7520.435,5091.309;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;-8048.436,6067.309;Inherit;False;153;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-6784.436,4819.309;Inherit;False;Property;_sSpecularOffset1;sSpecularOffset1;11;0;Create;True;0;0;0;False;0;False;0;-0.43;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;172;-7392.436,5203.309;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;167;-7920.435,6179.309;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-6784.436,5555.309;Inherit;False;Property;_sSpecularOffset2;sSpecularOffset2;12;0;Create;True;0;0;0;False;0;False;0;-0.43;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;169;-6736.436,5411.309;Inherit;False;153;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-6272.436,4803.309;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;-6368.436,5107.309;Inherit;False;166;BiNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;174;-7232.436,5091.309;Inherit;False;H;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;-6256.436,5539.309;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-7936.435,5987.309;Inherit;False;Constant;_Float12;Float 12;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;-6352.436,5843.309;Inherit;False;166;BiNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;178;-6112.436,4915.309;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;173;-7808.435,6195.308;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;180;-6016.436,5043.309;Inherit;False;174;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;-7728.435,5987.309;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;182;-6016.436,4819.309;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;179;-4580.173,3376.405;Inherit;False;Tangent;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;181;-6112.436,5651.309;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;186;-5856.436,4947.309;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;-6000.436,5763.309;Inherit;False;174;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;187;-7936.435,5635.309;Inherit;False;174;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-5952.436,5170.309;Inherit;False;Property;_sSpecularShiniess1;sSpecularShiniess1;13;0;Create;True;0;0;0;False;0;False;1;0.21;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;188;-7936.435,5539.309;Inherit;False;179;Tangent;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;189;-6016.436,5555.309;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-7584.435,6067.309;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;195;-5952.436,5907.309;Inherit;False;Property;_sSpecularShiniess2;sSpecularShiniess2;14;0;Create;True;0;0;0;False;0;False;1;0.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;193;-7456.436,6003.309;Inherit;False;halfLambert;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;191;-5840.437,5667.309;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;194;-7712.435,5587.309;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;192;-5680.437,4867.309;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;198;-7488.436,5603.309;Inherit;False;TdotH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;199;-5680.437,5603.309;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;196;-5504.436,4947.309;Inherit;False;BdotH1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;-7191.815,6505.366;Inherit;False;193;halfLambert;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;202;-5504.436,5667.309;Inherit;False;BdotH2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;201;-5200.437,4899.309;Inherit;False;198;TdotH;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;200;-6919.815,6489.366;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-5072.437,4755.309;Inherit;False;196;BdotH1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;204;-5040.437,5571.309;Inherit;False;202;BdotH2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;206;-4960.437,4995.309;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;207;-6791.815,6601.366;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;205;-5168.437,5715.309;Inherit;False;198;TdotH;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-4880.437,4787.309;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;211;-6647.815,6633.366;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-4864.437,5587.309;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;212;-4720.437,4851.309;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;-4944.437,5811.309;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;214;-4704.437,5667.309;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;213;-4576.437,4867.309;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;215;-7184.436,6531.308;Inherit;False;aniso_atten;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;218;-4560.437,5683.309;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;216;-4528.437,4995.309;Inherit;False;215;aniso_atten;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ExpOpNode;217;-4416.437,4851.309;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;220;-4288.437,5171.309;Inherit;False;Property;_Color1;Color 1;9;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.5283019,0.1325738,0.1325738,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ExpOpNode;223;-4400.437,5667.309;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;222;-4240.437,5987.309;Inherit;False;Property;_Color0;Color 0;10;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.7735849,0.3619793,0.3619793,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;225;-4272.437,4867.309;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;221;-4496.437,5123.309;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;224;-4464.437,5923.309;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;219;-4512.437,5795.309;Inherit;False;215;aniso_atten;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-4176.437,5667.309;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;227;-4112.437,4899.309;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwizzleNode;229;-3888.437,5043.309;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;21;-1324.996,-51.59125;Inherit;True;Property;_MRAE;MRAE;2;0;Create;True;0;0;0;False;0;False;-1;None;eb5e76089b0ff9142868e6df46bc6bed;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwizzleNode;228;-3936.437,5827.309;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;231;-3872.437,5667.309;Inherit;False;spec_term2;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;41;-1012.283,-77.01244;Inherit;False;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.45;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;230;-3888.437,4867.309;Inherit;False;spec_term1;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;18;-251.793,-363.0797;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,0.3544474,0.3544474,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-657.1017,427.3165;Inherit;False;Property;_Roughness;Roughness;3;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-170.0925,-585.269;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;b527320130b3e074fba73d9a5a42e280;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;248;125.5388,415.3861;Inherit;False;231;spec_term2;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;43;-801.2758,-151.6472;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;36;-375.6716,-166.7028;Inherit;False;Property;_EColor;EColor;8;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;6.498019,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;247;81.53876,295.3861;Inherit;False;230;spec_term1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;273.1206,-189.9524;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;246;342.2577,285.3776;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-780.1155,181.4457;Inherit;False;Property;_Metalic;Metalic;4;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-20.06026,-182.2785;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-349.054,337.9617;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;252;416.6616,-6.180084;Inherit;False;Constant;_Float1;Float 1;16;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;10.10856,-58.13164;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;245;431.3442,128.0984;Inherit;False;Property;_IsAniso;IsAniso;6;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-217.3945,62.85425;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;33;222.0824,-62.66193;Inherit;False;Property;_IsEmissive;IsEmissive;7;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;24;-229.0494,327.6592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;536.6616,-87.18008;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;236;-7504.436,5379.309;Inherit;False;NdotH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;249;642.5388,17.38614;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-961.5894,467.1856;Inherit;False;Property;_CullMode;CullMode;5;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;233;-5629.209,3157.294;Inherit;True;Property;_TextureSample1;Texture Sample 1;15;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;240;-7472.436,5795.309;Inherit;False;NdotV;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;244;-5166.983,3055.466;Inherit;False;242;TangentNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;28;38.23729,60.4586;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;20;-58.16007,233.8573;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;250;650.1171,-144.5792;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;235;-7920.435,5843.309;Inherit;False;234;V;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;242;-5309.934,3192.88;Inherit;False;TangentNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;243;-7696.435,5779.309;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;241;-7888.435,5347.309;Inherit;False;153;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;238;-7680.435,5395.309;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;239;-7888.435,5443.309;Inherit;False;174;H;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;237;-7920.435,5731.309;Inherit;False;153;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;232;-5915.15,2949.439;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;234;-7232.436,4931.309;Inherit;False;V;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;901.9144,-31.93621;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Standard_AddAniso;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;DeferredOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;32;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;145;0;143;0
WireConnection;145;1;144;0
WireConnection;150;0;145;0
WireConnection;150;1;147;4
WireConnection;150;2;146;0
WireConnection;158;0;150;0
WireConnection;156;0;151;0
WireConnection;153;0;149;0
WireConnection;166;0;158;0
WireConnection;162;0;157;0
WireConnection;162;1;151;0
WireConnection;172;0;162;0
WireConnection;167;0;159;0
WireConnection;167;1;163;0
WireConnection;168;0;165;0
WireConnection;168;1;154;0
WireConnection;174;0;172;0
WireConnection;176;0;169;0
WireConnection;176;1;161;0
WireConnection;178;0;168;0
WireConnection;178;1;170;0
WireConnection;173;0;167;0
WireConnection;183;0;177;0
WireConnection;183;1;173;0
WireConnection;182;0;178;0
WireConnection;179;0;144;0
WireConnection;181;0;176;0
WireConnection;181;1;175;0
WireConnection;186;0;182;0
WireConnection;186;1;180;0
WireConnection;189;0;181;0
WireConnection;185;0;183;0
WireConnection;185;1;177;0
WireConnection;193;0;185;0
WireConnection;191;0;189;0
WireConnection;191;1;184;0
WireConnection;194;0;188;0
WireConnection;194;1;187;0
WireConnection;192;0;186;0
WireConnection;192;1;190;0
WireConnection;198;0;194;0
WireConnection;199;0;191;0
WireConnection;199;1;195;0
WireConnection;196;0;192;0
WireConnection;202;0;199;0
WireConnection;200;0;197;0
WireConnection;206;0;201;0
WireConnection;206;1;201;0
WireConnection;207;0;200;0
WireConnection;208;0;203;0
WireConnection;208;1;203;0
WireConnection;211;0;207;0
WireConnection;210;0;204;0
WireConnection;210;1;204;0
WireConnection;212;0;208;0
WireConnection;212;1;206;0
WireConnection;209;0;205;0
WireConnection;209;1;205;0
WireConnection;214;0;210;0
WireConnection;214;1;209;0
WireConnection;213;0;212;0
WireConnection;215;0;211;0
WireConnection;218;0;214;0
WireConnection;217;0;213;0
WireConnection;223;0;218;0
WireConnection;225;0;217;0
WireConnection;225;1;216;0
WireConnection;226;0;223;0
WireConnection;226;1;219;0
WireConnection;226;2;224;0
WireConnection;226;3;222;0
WireConnection;227;0;225;0
WireConnection;227;1;221;0
WireConnection;227;2;220;0
WireConnection;229;0;227;0
WireConnection;228;0;226;0
WireConnection;231;0;228;0
WireConnection;41;0;21;0
WireConnection;230;0;229;0
WireConnection;43;0;41;0
WireConnection;31;0;30;0
WireConnection;31;1;18;0
WireConnection;246;0;247;0
WireConnection;246;1;248;0
WireConnection;23;0;43;1
WireConnection;23;1;19;0
WireConnection;37;0;36;0
WireConnection;37;1;43;3
WireConnection;245;1;34;0
WireConnection;245;0;246;0
WireConnection;27;0;43;0
WireConnection;27;1;25;0
WireConnection;33;1;34;0
WireConnection;33;0;37;0
WireConnection;24;0;23;0
WireConnection;251;0;31;0
WireConnection;251;1;252;0
WireConnection;236;0;238;0
WireConnection;249;0;33;0
WireConnection;249;1;245;0
WireConnection;233;1;232;0
WireConnection;240;0;243;0
WireConnection;28;0;27;0
WireConnection;20;0;24;0
WireConnection;250;0;31;0
WireConnection;250;1;251;0
WireConnection;242;0;233;0
WireConnection;243;0;237;0
WireConnection;243;1;235;0
WireConnection;238;0;241;0
WireConnection;238;1;239;0
WireConnection;234;0;157;0
WireConnection;0;0;250;0
WireConnection;0;2;249;0
WireConnection;0;3;28;0
WireConnection;0;4;20;0
WireConnection;0;5;43;2
ASEEND*/
//CHKSM=49FC1A7A29AC2DE539776F349C467A033DC536FF