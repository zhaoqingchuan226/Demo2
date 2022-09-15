// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/VegetationLeavesMine"
{
	Properties
	{
		[Toggle(_HIDESIDES_ON)] _HideSides("Hide Sides", Float) = 0
		_Float0("Float 0", Float) = 1
		_HidePower("Hide Power", Float) = 2.5
		_Expand("Expand", Float) = 10
		_VRange("VRange", Float) = 3
		[Header(Main Maps)][Space(10)]_MainColor("Main Color", Color) = (1,1,1,0)
		_Diffuse("Diffuse", 2D) = "white" {}
		_TreeHeight("TreeHeight", Float) = 2.5
		_GrowOffset("GrowOffset", Float) = 0
		_ClipMul("ClipMul", Float) = 1
		[Space(10)][Header(Gradient Parameters)][Space(10)]_GradientColor("Gradient Color", Color) = (1,1,1,0)
		_GradientFalloff("Gradient Falloff", Range( 0 , 2)) = 2
		_GradientPosition("Gradient Position", Range( 0 , 1)) = 0.5
		[Toggle(_INVERTGRADIENT_ON)] _InvertGradient("Invert Gradient", Float) = 0
		[Space(10)][Header(Color Variation)][Space(10)]_ColorVariation("Color Variation", Color) = (1,0,0,0)
		_ColorVariationPower("Color Variation Power", Range( 0 , 1)) = 1
		_ColorVariationNoise("Color Variation Noise", 2D) = "white" {}
		_GlobalWindSpeed("GlobalWindSpeed", Float) = 1
		_GlobalWindDir("GlobalWindDir", Vector) = (1,0,0,0)
		_NoiseScale("Noise Scale", Float) = 0.5
		[Space(10)][Header(Multipliers)][Space(10)]_WindMultiplier("BaseWind Multiplier", Float) = 0
		_MicroWindMultiplier("MicroWind Multiplier", Float) = 1
		[Space(10)][KeywordEnum(R,G,B,A)] _BaseWindChannel("Base Wind Channel", Float) = 2
		[KeywordEnum(R,G,B,A)] _MicroWindChannel("Micro Wind Channel", Float) = 0
		[Space(10)]_WindTrunkPosition("Wind Trunk Position", Float) = 0
		_WindTrunkContrast("Wind Trunk Contrast", Float) = 10
		[Toggle(_WINDDEBUGVIEW_ON)] _WindDebugView("WindDebugView", Float) = 0
		[Toggle(_SEEVERTEXCOLOR_ON)] _SeeVertexColor("See Vertex Color", Float) = 0
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		[HDR]_BornColor("BornColor", Color) = (1,0.906827,0.3339622,0)
		[HDR]_Color0("Color 0", Color) = (1,0.906827,0.3339622,0)
		_SickMainColor("SickMainColor", Color) = (0.490566,0.1173618,0,0)
		_SickGradientColor("SickGradientColor", Color) = (0.735849,0.5083417,0,0)
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 5.0
		#pragma shader_feature _SEEVERTEXCOLOR_ON
		#pragma shader_feature _WINDDEBUGVIEW_ON
		#pragma shader_feature_local _INVERTGRADIENT_ON
		#pragma shader_feature_local _BASEWINDCHANNEL_R _BASEWINDCHANNEL_G _BASEWINDCHANNEL_B _BASEWINDCHANNEL_A
		#pragma shader_feature_local _MICROWINDCHANNEL_R _MICROWINDCHANNEL_G _MICROWINDCHANNEL_B _MICROWINDCHANNEL_A
		#pragma shader_feature_local _HIDESIDES_ON
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float3 uv2_texcoord2;
			float4 screenPosition;
			float3 viewDir;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform float4 _SickMainColor;
		uniform float4 _BornColor;
		uniform float4 _MainColor;
		uniform float _Grow;
		uniform float _GrowOffset;
		uniform float _VRange;
		uniform float _TreeHeight;
		uniform float _Health;
		uniform float4 _SickGradientColor;
		uniform float4 _Color0;
		uniform float4 _GradientColor;
		uniform float _GradientPosition;
		uniform float _GradientFalloff;
		uniform float4 _ColorVariation;
		uniform float _ColorVariationPower;
		uniform sampler2D _ColorVariationNoise;
		uniform float _NoiseScale;
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform float WindSpeed;
		uniform float WindPower;
		uniform float WindBurstsSpeed;
		uniform float WindBurstsScale;
		uniform float WindBurstsPower;
		uniform float _WindTrunkContrast;
		uniform float _WindTrunkPosition;
		uniform float _WindMultiplier;
		uniform float MicroFrequency;
		uniform float MicroSpeed;
		uniform float MicroPower;
		uniform float _MicroWindMultiplier;
		uniform float _GlobalWindSpeed;
		uniform float3 _GlobalWindDir;
		uniform float _Float0;
		uniform float _Expand;
		uniform float _HidePower;
		uniform float _ClipMul;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline float Dither8x8Bayer( int x, int y )
		{
			const float dither[ 64 ] = {
				 1, 49, 13, 61,  4, 52, 16, 64,
				33, 17, 45, 29, 36, 20, 48, 32,
				 9, 57,  5, 53, 12, 60,  8, 56,
				41, 25, 37, 21, 44, 28, 40, 24,
				 3, 51, 15, 63,  2, 50, 14, 62,
				35, 19, 47, 31, 34, 18, 46, 30,
				11, 59,  7, 55, 10, 58,  6, 54,
				43, 27, 39, 23, 42, 26, 38, 22};
			int r = y * 8 + x;
			return dither[r] / 64; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !defined(DIRECTIONAL)
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float3 ase_worldPos = i.worldPos;
			float smoothstepResult411 = smoothstep( ( _Grow - _GrowOffset ) , ( _Grow + _VRange ) , ( ase_worldPos.y / _TreeHeight ));
			float V410 = ( 1.0 - smoothstepResult411 );
			float4 lerpResult483 = lerp( _BornColor , _MainColor , pow( V410 , 5.0 ));
			float4 lerpResult470 = lerp( _SickMainColor , lerpResult483 , _Health);
			float4 lerpResult485 = lerp( _Color0 , _GradientColor , pow( V410 , 5.0 ));
			float4 lerpResult474 = lerp( _SickGradientColor , lerpResult485 , _Health);
			float3 ase_worldNormal = i.worldNormal;
			#ifdef _INVERTGRADIENT_ON
				float staticSwitch306 = ( 1.0 - ase_worldNormal.y );
			#else
				float staticSwitch306 = ase_worldNormal.y;
			#endif
			float clampResult39 = clamp( ( ( staticSwitch306 + (-2.0 + (_GradientPosition - 0.0) * (1.0 - -2.0) / (1.0 - 0.0)) ) / _GradientFalloff ) , 0.0 , 1.0 );
			float4 lerpResult46 = lerp( lerpResult470 , lerpResult474 , clampResult39);
			float4 blendOpSrc53 = lerpResult46;
			float4 blendOpDest53 = _ColorVariation;
			float4 lerpBlendMode53 = lerp(blendOpDest53,( blendOpDest53/ max( 1.0 - blendOpSrc53, 0.00001 ) ),_ColorVariationPower);
			float2 appendResult71 = (float2(ase_worldPos.x , ase_worldPos.z));
			float4 temp_cast_0 = (3.0).xxxx;
			float4 lerpResult58 = lerp( lerpResult46 , ( saturate( lerpBlendMode53 )) , ( _ColorVariationPower * pow( tex2D( _ColorVariationNoise, ( appendResult71 * ( _NoiseScale / 100.0 ) ) ) , temp_cast_0 ) ));
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 tex2DNode56 = tex2D( _Diffuse, (uv_Diffuse*0.5 + 0.5) );
			float4 _Albedo339 = ( lerpResult58 * tex2DNode56 );
			float temp_output_102_0 = ( _Time.y * WindSpeed );
			float2 appendResult139 = (float2(WindBurstsSpeed , WindBurstsSpeed));
			float2 appendResult131 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner126 = ( 1.0 * _Time.y * appendResult139 + appendResult131);
			float simplePerlin2D379 = snoise( panner126*( WindBurstsScale / 10.0 ) );
			simplePerlin2D379 = simplePerlin2D379*0.5 + 0.5;
			float temp_output_129_0 = ( WindPower * ( simplePerlin2D379 * WindBurstsPower ) );
			#if defined(_BASEWINDCHANNEL_R)
				float staticSwitch285 = i.vertexColor.r;
			#elif defined(_BASEWINDCHANNEL_G)
				float staticSwitch285 = i.vertexColor.g;
			#elif defined(_BASEWINDCHANNEL_B)
				float staticSwitch285 = i.vertexColor.b;
			#elif defined(_BASEWINDCHANNEL_A)
				float staticSwitch285 = i.vertexColor.a;
			#else
				float staticSwitch285 = i.vertexColor.b;
			#endif
			float BaseWindColor288 = staticSwitch285;
			float4 temp_cast_1 = (pow( ( 1.0 - BaseWindColor288 ) , _WindTrunkPosition )).xxxx;
			float4 temp_output_299_0 = saturate( CalculateContrast(_WindTrunkContrast,temp_cast_1) );
			float3 appendResult113 = (float3(( ( sin( temp_output_102_0 ) * temp_output_129_0 ) * temp_output_299_0 ).r , 0.0 , ( ( cos( temp_output_102_0 ) * ( temp_output_129_0 * 0.5 ) ) * temp_output_299_0 ).r));
			float4 transform254 = mul(unity_WorldToObject,float4( appendResult113 , 0.0 ));
			float4 BaseWind151 = ( transform254 * _WindMultiplier );
			float2 temp_cast_5 = (MicroSpeed).xx;
			float3 appendResult174 = (float3(ase_worldPos.x , ase_worldPos.z , ase_worldPos.y));
			float2 panner175 = ( 1.0 * _Time.y * temp_cast_5 + appendResult174.xy);
			float simplePerlin2D176 = snoise( ( panner175 * 1.0 ) );
			simplePerlin2D176 = simplePerlin2D176*0.5 + 0.5;
			float3 clampResult49 = clamp( sin( ( MicroFrequency * ( ase_worldPos + simplePerlin2D176 ) ) ) , float3( -1,-1,-1 ) , float3( 1,1,1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			#if defined(_MICROWINDCHANNEL_R)
				float staticSwitch284 = i.vertexColor.r;
			#elif defined(_MICROWINDCHANNEL_G)
				float staticSwitch284 = i.vertexColor.g;
			#elif defined(_MICROWINDCHANNEL_B)
				float staticSwitch284 = i.vertexColor.b;
			#elif defined(_MICROWINDCHANNEL_A)
				float staticSwitch284 = i.vertexColor.a;
			#else
				float staticSwitch284 = i.vertexColor.r;
			#endif
			float MicroWindColor287 = staticSwitch284;
			float3 MicroWind152 = ( ( ( ( clampResult49 * ase_vertexNormal ) * MicroPower ) * MicroWindColor287 ) * _MicroWindMultiplier );
			float3 worldToObjDir390 = mul( unity_WorldToObject, float4( _GlobalWindDir, 0 ) ).xyz;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 objToWorld381 = mul( unity_ObjectToWorld, float4( ase_vertex3Pos, 1 ) ).xyz;
			float3 GlobalWind395 = ( ( sin( ( _Time.y * _GlobalWindSpeed ) ) * worldToObjDir390 ) * pow( saturate( ( objToWorld381.y / 20.0 ) ) , 2.0 ) );
			float3 appendResult402 = (float3(-i.uv2_texcoord2.x , i.uv2_texcoord2.y , i.uv2_texcoord2.z));
			float3 CenterPoint406 = ( appendResult402 * _Float0 * 0.1 );
			float3 temp_output_444_0 = ( CenterPoint406 * _Expand );
			float3 Offset431 = ( ( ( ( ase_vertex3Pos - temp_output_444_0 ) * V410 ) + temp_output_444_0 ) - ase_vertex3Pos );
			#ifdef _WINDDEBUGVIEW_ON
				float4 staticSwitch194 = ( BaseWind151 + float4( MicroWind152 , 0.0 ) + float4( GlobalWind395 , 0.0 ) + float4( Offset431 , 0.0 ) );
			#else
				float4 staticSwitch194 = _Albedo339;
			#endif
			#ifdef _SEEVERTEXCOLOR_ON
				float4 staticSwitch310 = i.vertexColor;
			#else
				float4 staticSwitch310 = staticSwitch194;
			#endif
			o.Albedo = staticSwitch310.rgb;
			float _Opacity231 = tex2DNode56.a;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen217 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither217 = Dither8x8Bayer( fmod(clipScreen217.x, 8), fmod(clipScreen217.y, 8) );
			float3 normalizeResult214 = normalize( cross( ddy( ase_worldPos ) , ddx( ase_worldPos ) ) );
			float dotResult200 = dot( i.viewDir , normalizeResult214 );
			float clampResult222 = clamp( ( ( _Opacity231 * ( 1.0 - ( ( 1.0 - abs( dotResult200 ) ) * 2.0 ) ) ) * _HidePower ) , 0.0 , 1.0 );
			dither217 = step( dither217, clampResult222 );
			float OpacityDither205 = dither217;
			#ifdef _HIDESIDES_ON
				float staticSwitch234 = OpacityDither205;
			#else
				float staticSwitch234 = _Opacity231;
			#endif
			float3 objToWorld449 = mul( unity_ObjectToWorld, float4( CenterPoint406, 1 ) ).xyz;
			float Grown420 = _Grow;
			float ClipUpper461 = step( objToWorld449.y , ( ( Grown420 * 0.3 ) - _ClipMul ) );
			float3 temp_cast_12 = (( staticSwitch234 * ClipUpper461 )).xxx;
			o.Emission = temp_cast_12;
			float3 temp_cast_13 = (1.0).xxx;
			o.Translucency = temp_cast_13;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustom keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
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
				float4 customPack3 : TEXCOORD3;
				float3 worldPos : TEXCOORD4;
				float3 worldNormal : TEXCOORD5;
				half4 color : COLOR0;
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
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyz = customInputData.uv2_texcoord2;
				o.customPack2.xyz = v.texcoord1;
				o.customPack3.xyzw = customInputData.screenPosition;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
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
				surfIN.uv2_texcoord2 = IN.customPack2.xyz;
				surfIN.screenPosition = IN.customPack3.xyzw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandardCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardCustom, o )
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
}
/*ASEBEGIN
Version=18935
-30.5;290;815.2;788.2;4654.768;1283.725;1.805515;True;False
Node;AmplifyShaderEditor.CommentaryNode;156;-6016,2816;Inherit;False;3067.315;862.5801;;22;152;304;62;303;54;289;52;51;49;44;40;36;32;34;176;190;175;174;26;172;305;372;MicroWind;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;226;-6016,1152;Inherit;False;2565;479;;18;205;217;222;224;223;215;283;209;216;207;204;200;214;199;213;211;212;210;Dithering;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;302;-6656,-1664;Inherit;False;889.3333;538;;7;55;285;288;284;287;292;293;VertexColor;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;421;-7487.554,4211.765;Inherit;False;1503.653;550.3027;V;12;409;410;411;412;413;414;415;416;417;418;419;420;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;172;-5984,3040;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;150;-6016,1792;Inherit;False;3196.337;864.2947;;37;291;151;164;165;254;113;111;112;114;299;104;109;298;106;103;108;296;107;102;297;129;105;295;100;148;101;294;290;149;135;126;139;127;131;125;128;379;Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;418;-7203.086,4261.765;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;417;-7173.017,4408.593;Inherit;False;Property;_TreeHeight;TreeHeight;8;0;Create;True;0;0;0;False;0;False;2.5;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;419;-7437.554,4496.263;Inherit;False;Global;_Grow;_Grow;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;415;-7099.436,4516.175;Inherit;False;Property;_GrowOffset;GrowOffset;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;416;-7055.195,4646.668;Inherit;False;Property;_VRange;VRange;5;0;Create;True;0;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;210;-5968,1456;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;26;-5760,3328;Float;False;Global;MicroSpeed;MicroSpeed;18;1;[HideInInspector];Create;False;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;407;-8879.958,783.0534;Inherit;False;1282.499;440.4616;CenterPoint;7;400;402;403;404;405;406;401;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;125;-5968,2208;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;174;-5728,3200;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;128;-5968,2400;Inherit;False;Global;WindBurstsSpeed;Wind Bursts Speed;22;1;[HideInInspector];Create;True;0;0;0;False;0;False;50;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;55;-6608,-1440;Inherit;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DdyOpNode;211;-5776,1392;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;414;-6790.495,4576.538;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;413;-6890.217,4467.093;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DdxOpNode;212;-5776,1488;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;412;-6782.913,4388.776;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-5504,3456;Inherit;False;Constant;_Float2;Float 2;16;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;285;-6304,-1440;Inherit;False;Property;_BaseWindChannel;Base Wind Channel;25;0;Create;True;0;0;0;False;1;Space(10);False;0;2;2;True;;KeywordEnum;4;R;G;B;A;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;175;-5536,3264;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;139;-5744,2384;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;400;-8829.958,852.6338;Inherit;False;1;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;127;-5760,2496;Inherit;False;Global;WindBurstsScale;Wind Bursts Scale;23;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;131;-5776,2240;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;411;-6608.702,4483.637;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CrossProductOpNode;213;-5648,1424;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;372;-5312,3360;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NegateNode;403;-8560.23,833.0534;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;135;-5488,2480;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;126;-5536,2304;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;359;-6656,-640;Inherit;False;1565;669;;16;43;38;39;37;35;33;306;29;155;28;27;469;470;473;474;489;Color Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;288;-6016,-1440;Inherit;False;BaseWindColor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;404;-8273.85,1076.703;Inherit;False;Constant;_Float3;Float 3;1;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;199;-5568,1200;Inherit;True;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;360;-6016,128;Inherit;False;2322;838;Color blend controlled by world-space noise;19;41;71;72;42;162;161;45;53;50;58;56;231;339;63;362;48;367;464;465;Color Variation;1,1,1,1;0;0
Node;AmplifyShaderEditor.NormalizeNode;214;-5488,1424;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;401;-8348.603,1008.032;Inherit;False;Property;_Float0;Float 0;1;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;27;-6624,-512;Inherit;True;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NoiseGeneratorNode;176;-5152,3360;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;379;-5248,2304;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;402;-8371.074,873.6509;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;149;-5184,2576;Inherit;False;Global;WindBurstsPower;Wind Bursts Power;24;1;[HideInInspector];Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;290;-4544,2400;Inherit;False;288;BaseWindColor;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;409;-6384.701,4563.637;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;200;-5312,1312;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-4656,1984;Inherit;False;Global;WindPower;Wind Power;21;1;[HideInInspector];Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;410;-6208.701,4579.636;Inherit;False;V;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;464;-4920.818,573.2478;Inherit;False;0;56;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-5072,3040;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-4832,2400;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-5056,2096;Inherit;False;Global;WindSpeed;Wind Speed;20;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;405;-8079.44,988.5796;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-6528,-256;Float;False;Property;_GradientPosition;Gradient Position;15;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;100;-5056,1968;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;-4336,2480;Inherit;False;Property;_WindTrunkPosition;Wind Trunk Position;27;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;29;-6400,-384;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;295;-4320,2368;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-5136,2880;Float;False;Global;MicroFrequency;MicroFrequency;19;1;[HideInInspector];Create;False;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;406;-7822.259,1108.115;Inherit;False;CenterPoint;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-4544,2224;Inherit;False;Constant;_Float8;Float 8;18;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;465;-4702.08,688.2478;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;204;-5184,1312;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;480;-5660.538,-1047.148;Inherit;False;410;V;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;482;-5592.354,-955.697;Inherit;False;Constant;_Float4;Float 4;36;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;306;-6240,-464;Inherit;False;Property;_InvertGradient;Invert Gradient;16;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;41;-5968,528;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;297;-4128,2432;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;487;-5591.371,76.91838;Inherit;False;Constant;_Float6;Float 6;36;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-5968,720;Inherit;False;Property;_NoiseScale;Noise Scale;22;0;Create;True;0;0;0;False;0;False;0.5;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-4448,1968;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-4912,2944;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-4800,1968;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;430;-4403.488,4989.875;Inherit;False;1261.807;603.981;Scale;9;422;423;424;425;427;428;444;445;447;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;296;-4192,2560;Inherit;False;Property;_WindTrunkContrast;Wind Trunk Contrast;28;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;488;-5659.555,-14.53249;Inherit;False;410;V;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;155;-6192,-256;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-2;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;396;-5310.445,3888.181;Inherit;False;1525.031;706.8687;Global Wind;16;380;381;382;383;384;385;386;387;388;389;390;391;392;393;394;395;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CosOpNode;106;-4544,2096;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;481;-5411.122,-977.0546;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;56;-4478.601,595.5751;Inherit;True;Property;_Diffuse;Diffuse;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-5968,-368;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;43;-5797.879,-739.0018;Float;False;Property;_MainColor;Main Color;6;0;Create;True;0;0;0;False;2;Header(Main Maps);Space(10);False;1,1,1,0;0.4101034,0.7264151,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;207;-5056,1312;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;103;-4544,1840;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;484;-5397.379,-1185.265;Inherit;False;Property;_BornColor;BornColor;40;1;[HDR];Create;True;0;0;0;False;0;False;1,0.906827,0.3339622,0;1,0.906827,0.3339622,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;486;-5410.139,55.56074;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;38;-5712,-384;Float;False;Property;_GradientColor;Gradient Color;13;0;Create;True;0;0;0;False;3;Space(10);Header(Gradient Parameters);Space(10);False;1,1,1,0;0.2224449,0.490566,0.01619794,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;72;-5776,720;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;-5776,560;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;423;-4392.879,5301.783;Inherit;False;406;CenterPoint;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;489;-5525.307,-139.6937;Inherit;False;Property;_Color0;Color 0;41;1;[HDR];Create;True;0;0;0;False;0;False;1,0.906827,0.3339622,0;1,0.906827,0.3339622,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;380;-4751.224,4288.973;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;35;-6016,-64;Float;False;Property;_GradientFalloff;Gradient Falloff;14;0;Create;True;0;0;0;False;0;False;2;0.9;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;425;-4381.055,5450.122;Inherit;False;Property;_Expand;Expand;3;0;Create;True;0;0;0;False;0;False;10;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;-4288,2160;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;298;-3968,2464;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SinOpNode;40;-4688,2944;Inherit;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-4288,1840;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;483;-5212.915,-1007.491;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;49;-4496,2944;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;-1,-1,-1;False;2;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;216;-4880,1280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;284;-6304,-1600;Inherit;False;Property;_MicroWindChannel;Micro Wind Channel;26;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;4;R;G;B;A;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;473;-5587.781,-512.0884;Inherit;False;Property;_SickGradientColor;SickGradientColor;43;0;Create;True;0;0;0;False;0;False;0.735849,0.5083417,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;44;-4816,3264;Inherit;True;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;231;-4176,656;Inherit;False;_Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;422;-4353.488,5039.875;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;362;-5632,640;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;382;-5178.701,3938.181;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;384;-4452.39,4417.217;Inherit;False;Constant;_Float13;Float 13;15;0;Create;True;0;0;0;False;0;False;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;381;-4528.634,4246.347;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;485;-5189.252,-131.3633;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;383;-5260.445,4042.117;Inherit;False;Property;_GlobalWindSpeed;GlobalWindSpeed;20;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;444;-4173.116,5297.449;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;37;-5712,-192;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;469;-5562.905,-575.9286;Inherit;False;Global;_Health;_Health;34;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;299;-3776,2464;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-4032,2096;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;472;-5535.968,-848.8564;Inherit;False;Property;_SickMainColor;SickMainColor;42;0;Create;True;0;0;0;False;0;False;0.490566,0.1173618,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;424;-4036.285,5162.071;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;386;-4983.319,3977.258;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;470;-5149.604,-600.6733;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-3776,2096;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;474;-5232.069,-443.353;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;385;-5127.368,4160.343;Inherit;False;Property;_GlobalWindDir;GlobalWindDir;21;0;Create;True;0;0;0;False;0;False;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;51;-4240,3200;Float;False;Global;MicroPower;MicroPower;20;0;Create;False;0;0;0;False;0;False;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-4192,3072;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;287;-6016,-1600;Inherit;False;MicroWindColor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;39;-5584,-192;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;447;-4014.2,5433.146;Inherit;False;410;V;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;-3776,1840;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-5328,848;Inherit;False;Constant;_Float1;Float 1;16;0;Create;True;0;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;387;-4288.583,4317.211;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;367;-5456,608;Inherit;True;Property;_ColorVariationNoise;Color Variation Noise;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;209;-4736,1296;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;283;-4752,1200;Inherit;False;231;_Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-3776,1968;Inherit;False;Constant;_Float9;Float 9;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;391;-4141.92,4479.649;Inherit;False;Constant;_Float14;Float 14;15;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;390;-4886.891,4131.487;Inherit;False;World;Object;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;113;-3600,1920;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-4032,3104;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;48;-5328,224;Inherit;False;Property;_ColorVariation;Color Variation;17;0;Create;True;0;0;0;False;3;Space(10);Header(Color Variation);Space(10);False;1,0,0,0;0.1103595,0.2924528,0.2567374,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;46;-4998.518,-346.5791;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;289;-4256,3392;Inherit;False;287;MicroWindColor;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;445;-3842.622,5175.906;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-5328,400;Inherit;False;Property;_ColorVariationPower;Color Variation Power;18;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;456;-2391.408,-253.2045;Inherit;False;793.0959;1039.635;Clip_Upper;9;448;449;450;452;453;459;466;467;468;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;388;-4103.554,4353.973;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;215;-4576,1264;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;223;-4608,1472;Inherit;False;Property;_HidePower;Hide Power;2;0;Create;True;0;0;0;False;0;False;2.5;4.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;420;-7280.093,4657.358;Inherit;False;Grown;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;389;-4825.45,3980.384;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;161;-5120,784;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;254;-3344,1920;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;165;-3376,2208;Inherit;False;Property;_WindMultiplier;BaseWind Multiplier;23;0;Create;False;0;0;0;False;3;Space(10);Header(Multipliers);Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;393;-4665.939,4037.828;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;392;-3971.88,4389.77;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;427;-3546.633,5138.76;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendOpsNode;53;-4944,192;Inherit;True;ColorDodge;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;468;-2289.446,122.5692;Inherit;False;Constant;_Float5;Float 5;34;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;452;-2264.52,66.87584;Inherit;False;420;Grown;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-4944,704;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;-4384,1328;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;303;-3760,3488;Inherit;False;Property;_MicroWindMultiplier;MicroWind Multiplier;24;0;Create;True;0;0;0;False;3;;;;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-3776,3232;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;453;-2318.99,188.5249;Inherit;True;Property;_ClipMul;ClipMul;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;-3136,2096;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;304;-3472,3296;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;448;-2341.408,671.0302;Inherit;False;406;CenterPoint;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;222;-4176,1328;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;428;-3307.281,5209.358;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;467;-2089.446,48.56917;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;394;-4213.13,4069.92;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;58;-4560,176;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-3312,3296;Inherit;False;MicroWind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformPositionNode;449;-2092.689,467.2833;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;466;-1892.851,39.30737;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DitheringNode;217;-3968,1344;Inherit;False;1;False;4;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;3;SAMPLERSTATE;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-4176,400;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;395;-4010.215,4084.542;Inherit;False;GlobalWind;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;151;-3088,1920;Inherit;False;BaseWind;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;431;-3098.285,5233.588;Inherit;False;Offset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;-4736,-528;Inherit;False;152;MicroWind;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;339;-3920,400;Inherit;False;_Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;397;-4704.997,-374.1401;Inherit;False;395;GlobalWind;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;-4736,-640;Inherit;False;151;BaseWind;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;205;-3680,1344;Inherit;False;OpacityDither;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;432;-4627.304,-265.9076;Inherit;False;431;Offset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;450;-1755.639,296.9894;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;232;-4288,-800;Inherit;False;231;_Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;461;-1544.885,271.2058;Inherit;False;ClipUpper;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;233;-4288,-704;Inherit;False;205;OpacityDither;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;346;-4480,-1280;Inherit;False;339;_Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-4489,-594;Inherit;True;4;4;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;462;-4049.404,-635.6879;Inherit;False;461;ClipUpper;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;363;-6656,-1024;Inherit;False;866;280;;3;57;65;342;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;309;-4224,-1168;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;234;-4096,-768;Inherit;False;Property;_HideSides;Hide Sides;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;194;-4288,-1280;Inherit;False;Property;_WindDebugView;WindDebugView;30;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;354;-4096,-896;Inherit;False;Constant;_Transluency;Transluency;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;434;-4152.562,-564.5757;Inherit;False;406;CenterPoint;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;463;-3873.184,-771.2117;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;305;-4512,3392;Inherit;True;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;291;-4192,1968;Inherit;False;288;BaseWindColor;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;292;-6304,-1280;Inherit;False;Property;_DepositLayerChannel;DepositLayer Channel;29;0;Create;True;0;0;0;False;0;False;0;2;2;True;;KeywordEnum;4;R;G;B;A;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;435;-3934.226,-493.8922;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;65;-6320,-976;Inherit;True;Property;_Normal;Normal;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;436;-3904.958,-374.3555;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;459;-2376.798,-142.138;Inherit;True;Property;_Alpha;Alpha;32;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;293;-6032,-1280;Inherit;False;DepositLayerColor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;342;-6016,-976;Inherit;False;_Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-6608,-976;Float;False;Property;_NormalPower;Normal Power;12;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;399;-4200.363,-437.6128;Inherit;False;1;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;310;-4032,-1232;Inherit;False;Property;_SeeVertexColor;See Vertex Color;31;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-3328.425,-952.6666;Float;False;True;-1;7;;0;0;Standard;Custom/VegetationLeavesMine;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;4;33;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;174;0;172;1
WireConnection;174;1;172;3
WireConnection;174;2;172;2
WireConnection;211;0;210;0
WireConnection;414;0;419;0
WireConnection;414;1;416;0
WireConnection;413;0;419;0
WireConnection;413;1;415;0
WireConnection;212;0;210;0
WireConnection;412;0;418;2
WireConnection;412;1;417;0
WireConnection;285;1;55;1
WireConnection;285;0;55;2
WireConnection;285;2;55;3
WireConnection;285;3;55;4
WireConnection;175;0;174;0
WireConnection;175;2;26;0
WireConnection;139;0;128;0
WireConnection;139;1;128;0
WireConnection;131;0;125;1
WireConnection;131;1;125;3
WireConnection;411;0;412;0
WireConnection;411;1;413;0
WireConnection;411;2;414;0
WireConnection;213;0;211;0
WireConnection;213;1;212;0
WireConnection;372;0;175;0
WireConnection;372;1;190;0
WireConnection;403;0;400;1
WireConnection;135;0;127;0
WireConnection;126;0;131;0
WireConnection;126;2;139;0
WireConnection;288;0;285;0
WireConnection;214;0;213;0
WireConnection;176;0;372;0
WireConnection;379;0;126;0
WireConnection;379;1;135;0
WireConnection;402;0;403;0
WireConnection;402;1;400;2
WireConnection;402;2;400;3
WireConnection;409;0;411;0
WireConnection;200;0;199;0
WireConnection;200;1;214;0
WireConnection;410;0;409;0
WireConnection;32;0;172;0
WireConnection;32;1;176;0
WireConnection;148;0;379;0
WireConnection;148;1;149;0
WireConnection;405;0;402;0
WireConnection;405;1;401;0
WireConnection;405;2;404;0
WireConnection;29;0;27;2
WireConnection;295;0;290;0
WireConnection;406;0;405;0
WireConnection;465;0;464;0
WireConnection;204;0;200;0
WireConnection;306;1;27;2
WireConnection;306;0;29;0
WireConnection;297;0;295;0
WireConnection;297;1;294;0
WireConnection;129;0;105;0
WireConnection;129;1;148;0
WireConnection;36;0;34;0
WireConnection;36;1;32;0
WireConnection;102;0;100;0
WireConnection;102;1;101;0
WireConnection;155;0;28;0
WireConnection;106;0;102;0
WireConnection;481;0;480;0
WireConnection;481;1;482;0
WireConnection;56;1;465;0
WireConnection;33;0;306;0
WireConnection;33;1;155;0
WireConnection;207;0;204;0
WireConnection;103;0;102;0
WireConnection;486;0;488;0
WireConnection;486;1;487;0
WireConnection;72;0;42;0
WireConnection;71;0;41;1
WireConnection;71;1;41;3
WireConnection;108;0;129;0
WireConnection;108;1;107;0
WireConnection;298;1;297;0
WireConnection;298;0;296;0
WireConnection;40;0;36;0
WireConnection;104;0;103;0
WireConnection;104;1;129;0
WireConnection;483;0;484;0
WireConnection;483;1;43;0
WireConnection;483;2;481;0
WireConnection;49;0;40;0
WireConnection;216;0;207;0
WireConnection;284;1;55;1
WireConnection;284;0;55;2
WireConnection;284;2;55;3
WireConnection;284;3;55;4
WireConnection;231;0;56;4
WireConnection;362;0;71;0
WireConnection;362;1;72;0
WireConnection;381;0;380;0
WireConnection;485;0;489;0
WireConnection;485;1;38;0
WireConnection;485;2;486;0
WireConnection;444;0;423;0
WireConnection;444;1;425;0
WireConnection;37;0;33;0
WireConnection;37;1;35;0
WireConnection;299;0;298;0
WireConnection;109;0;106;0
WireConnection;109;1;108;0
WireConnection;424;0;422;0
WireConnection;424;1;444;0
WireConnection;386;0;382;0
WireConnection;386;1;383;0
WireConnection;470;0;472;0
WireConnection;470;1;483;0
WireConnection;470;2;469;0
WireConnection;111;0;109;0
WireConnection;111;1;299;0
WireConnection;474;0;473;0
WireConnection;474;1;485;0
WireConnection;474;2;469;0
WireConnection;52;0;49;0
WireConnection;52;1;44;0
WireConnection;287;0;284;0
WireConnection;39;0;37;0
WireConnection;112;0;104;0
WireConnection;112;1;299;0
WireConnection;387;0;381;2
WireConnection;387;1;384;0
WireConnection;367;1;362;0
WireConnection;209;0;216;0
WireConnection;390;0;385;0
WireConnection;113;0;112;0
WireConnection;113;1;114;0
WireConnection;113;2;111;0
WireConnection;54;0;52;0
WireConnection;54;1;51;0
WireConnection;46;0;470;0
WireConnection;46;1;474;0
WireConnection;46;2;39;0
WireConnection;445;0;424;0
WireConnection;445;1;447;0
WireConnection;388;0;387;0
WireConnection;215;0;283;0
WireConnection;215;1;209;0
WireConnection;420;0;419;0
WireConnection;389;0;386;0
WireConnection;161;0;367;0
WireConnection;161;1;162;0
WireConnection;254;0;113;0
WireConnection;393;0;389;0
WireConnection;393;1;390;0
WireConnection;392;0;388;0
WireConnection;392;1;391;0
WireConnection;427;0;445;0
WireConnection;427;1;444;0
WireConnection;53;0;46;0
WireConnection;53;1;48;0
WireConnection;53;2;45;0
WireConnection;50;0;45;0
WireConnection;50;1;161;0
WireConnection;224;0;215;0
WireConnection;224;1;223;0
WireConnection;62;0;54;0
WireConnection;62;1;289;0
WireConnection;164;0;254;0
WireConnection;164;1;165;0
WireConnection;304;0;62;0
WireConnection;304;1;303;0
WireConnection;222;0;224;0
WireConnection;428;0;427;0
WireConnection;428;1;422;0
WireConnection;467;0;452;0
WireConnection;467;1;468;0
WireConnection;394;0;393;0
WireConnection;394;1;392;0
WireConnection;58;0;46;0
WireConnection;58;1;53;0
WireConnection;58;2;50;0
WireConnection;152;0;304;0
WireConnection;449;0;448;0
WireConnection;466;0;467;0
WireConnection;466;1;453;0
WireConnection;217;0;222;0
WireConnection;63;0;58;0
WireConnection;63;1;56;0
WireConnection;395;0;394;0
WireConnection;151;0;164;0
WireConnection;431;0;428;0
WireConnection;339;0;63;0
WireConnection;205;0;217;0
WireConnection;450;0;449;2
WireConnection;450;1;466;0
WireConnection;461;0;450;0
WireConnection;115;0;153;0
WireConnection;115;1;154;0
WireConnection;115;2;397;0
WireConnection;115;3;432;0
WireConnection;234;1;232;0
WireConnection;234;0;233;0
WireConnection;194;1;346;0
WireConnection;194;0;115;0
WireConnection;463;0;234;0
WireConnection;463;1;462;0
WireConnection;305;0;44;0
WireConnection;292;1;55;1
WireConnection;292;0;55;2
WireConnection;292;2;55;3
WireConnection;292;3;55;4
WireConnection;435;0;434;0
WireConnection;65;5;57;0
WireConnection;436;0;435;0
WireConnection;293;0;292;0
WireConnection;342;0;65;0
WireConnection;310;1;194;0
WireConnection;310;0;309;0
WireConnection;0;0;310;0
WireConnection;0;2;463;0
WireConnection;0;7;354;0
ASEEND*/
//CHKSM=BB5BFC18B4A34D9004E2A76E041CFB8895948A25