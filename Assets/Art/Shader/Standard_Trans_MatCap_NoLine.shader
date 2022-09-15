// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_Trans_MatCap_NoLine"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.BlendMode)]_Src("Src", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		_MatCap("MatCap", 2D) = "black" {}
		[HDR]_Color("Color", Color) = (1,1,1,1)
		[Toggle(_ISMATCAP2_ON)] _isMatCap2("isMatCap2", Float) = 0
		_MatCap2("MatCap2", 2D) = "white" {}
		[HDR]_Color2("Color2", Color) = (1,1,1,1)
		[Toggle(_ISMATCAP3_ON)] _isMatCap3("isMatCap3", Float) = 0
		_MatCap3("MatCap3", 2D) = "white" {}
		[HDR]_Color3("Color3", Color) = (1,1,1,1)
		[Toggle(_ISFRESNEL_ON)] _isFresnel("isFresnel", Float) = 0
		[HDR]_FresnelColor("FresnelColor", Color) = (1,1,1,1)
		_FresnelBSP("FresnelBSP", Vector) = (0,1,1,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		ZWrite Off
		Blend [_Src] [_Dst]
		
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ISMATCAP2_ON
		#pragma shader_feature_local _ISMATCAP3_ON
		#pragma shader_feature_local _ISFRESNEL_ON
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Unlit keepalpha exclude_path:deferred novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _Dst;
		uniform float _Src;
		uniform float _CullMode;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform sampler2D _MatCap;
		uniform float4 _Color;
		uniform sampler2D _MatCap2;
		uniform float4 _Color2;
		uniform sampler2D _MatCap3;
		uniform float4 _Color3;
		uniform float3 _FresnelBSP;
		uniform float4 _FresnelColor;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline float3 ASESafeNormalize(float3 inVec)
		{
			float dp3 = max( 0.001f , dot( inVec , inVec ) );
			return inVec* rsqrt( dp3);
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor151 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPos.xy/ase_grabScreenPos.w);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 objToView69 = mul( UNITY_MATRIX_MV, float4( ase_vertex3Pos, 1 ) ).xyz;
			float3 normalizeResult97 = normalize( objToView69 );
			float3 viewPos70 = normalizeResult97;
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float3 worldNormal63 = ase_normWorldNormal;
			float3 worldToViewDir73 = ASESafeNormalize( mul( UNITY_MATRIX_V, float4( worldNormal63, 0 ) ).xyz );
			float3 viewNormal74 = worldToViewDir73;
			float3 break82 = cross( viewPos70 , viewNormal74 );
			float3 appendResult81 = (float3(-break82.y , break82.x , break82.z));
			float3 break86 = appendResult81;
			float3 appendResult87 = (float3((break86.x*0.5 + 0.5) , (break86.y*0.5 + 0.5) , break86.z));
			float3 wtfDir177 = appendResult87;
			float4 matCap195 = tex2D( _MatCap, (wtfDir177).xy );
			float4 temp_cast_0 = (0.0).xxxx;
			float4 MatCap2114 = tex2D( _MatCap2, (wtfDir177).xy );
			#ifdef _ISMATCAP2_ON
				float4 staticSwitch123 = ( MatCap2114 * _Color2 * _Color2.a );
			#else
				float4 staticSwitch123 = temp_cast_0;
			#endif
			float4 temp_cast_1 = (0.0).xxxx;
			float4 MatCap3118 = tex2D( _MatCap3, (wtfDir177).xy );
			#ifdef _ISMATCAP3_ON
				float4 staticSwitch131 = ( MatCap3118 * _Color3 * _Color3.a );
			#else
				float4 staticSwitch131 = temp_cast_1;
			#endif
			float4 temp_cast_2 = (0.0).xxxx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV136 = dot( worldNormal63, ase_worldViewDir );
			float fresnelNode136 = ( _FresnelBSP.x + _FresnelBSP.y * pow( 1.0 - fresnelNdotV136, _FresnelBSP.z ) );
			float4 Fresnel143 = ( saturate( fresnelNode136 ) * _FresnelColor );
			#ifdef _ISFRESNEL_ON
				float4 staticSwitch144 = Fresnel143;
			#else
				float4 staticSwitch144 = temp_cast_2;
			#endif
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult101 = dot( worldNormal63 , ase_worldlightDir );
			float halfLambert104 = (saturate( dotResult101 )*0.5 + 0.5);
			float4 lerpResult156 = lerp( screenColor151 , ( ( ( matCap195 * _Color ) + staticSwitch123 + staticSwitch131 + staticSwitch144 ) * halfLambert104 ) , _Color.a);
			o.Emission = lerpResult156.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
200.8;1.2;1280;803;316.1815;215.9121;1.507461;True;False
Node;AmplifyShaderEditor.CommentaryNode;75;-2937.212,-548.845;Inherit;False;775.8701;923.8165;Base;10;70;64;65;63;69;62;68;73;72;74;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;62;-2887.212,-498.845;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;68;-2857.482,-45.1962;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-2564.115,-470.3144;Inherit;False;worldNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformPositionNode;69;-2647.469,-46.89406;Inherit;False;Object;View;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;72;-2869.169,141.9927;Inherit;False;63;worldNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformDirectionNode;73;-2623.873,190.9716;Inherit;False;World;View;True;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;97;-2408.806,58.35486;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-2259.108,-14.31018;Inherit;False;viewPos;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;90;-3538.148,694.5519;Inherit;False;1693.944;324.2996;MatCap1_UV;11;78;80;79;82;85;86;81;87;88;89;77;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-2387.742,201.6964;Inherit;False;viewNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;-3478.308,752.5514;Inherit;False;70;viewPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-3488.148,869.1774;Inherit;False;74;viewNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CrossProductOpNode;78;-3298.557,785.0217;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;82;-3133.823,793.952;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.NegateNode;85;-2998.621,797.8516;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;81;-2851.721,793.9514;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;86;-2708.718,793.9519;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ScaleAndOffsetNode;89;-2544.918,862.8515;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;88;-2561.817,744.5519;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;87;-2303.115,769.2519;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-2070.604,777.4797;Inherit;False;wtfDir1;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;112;-3597.594,1335.796;Inherit;False;77;wtfDir1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;116;-3601.832,1666.781;Inherit;False;77;wtfDir1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;102.8643,571.7689;Inherit;False;63;worldNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;139;142.8643,777.7689;Inherit;False;Property;_FresnelBSP;FresnelBSP;16;0;Create;True;0;0;0;False;0;False;0,1,1;0,0.56,4.42;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FresnelNode;136;412.3081,657.8724;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-3526.282,1084.575;Inherit;False;77;wtfDir1;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;117;-3384.427,1728.696;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;113;-3380.19,1397.711;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;262.1567,-831.0595;Inherit;False;63;worldNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;111;-3060.098,1377.956;Inherit;True;Property;_MatCap2;MatCap2;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;115;-3064.335,1708.941;Inherit;True;Property;_MatCap3;MatCap3;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;150;656.6797,670.6041;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;99;161.3146,-712.2209;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;94;-3308.878,1146.49;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;142;416.6977,865.5952;Inherit;False;Property;_FresnelColor;FresnelColor;15;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;22.05752,42.50107,102.7558,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;822.9554,726.7079;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;101;532.2748,-756.7208;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;118;-2700.487,1744.651;Inherit;False;MatCap3;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;76;-3073.498,1105.56;Inherit;True;Property;_MatCap;MatCap;6;0;Create;True;0;0;0;False;0;False;-1;None;9bdb8e3930d5d5e48ae4b9952fc56f70;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;-2696.249,1413.666;Inherit;False;MatCap2;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;-713.6939,-98.25904;Inherit;False;114;MatCap2;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;95;-2714.404,1168.739;Inherit;False;matCap1;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;132;-808.704,323.4058;Inherit;False;118;MatCap3;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;121;724.8071,-603.1741;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;133;-818.6878,441.2085;Inherit;False;Property;_Color3;Color3;13;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;130;-723.6777,19.54366;Inherit;False;Property;_Color2;Color2;10;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;8.511788,8.749548,9.082411,0.3254902;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;143;931.3901,747.7931;Inherit;False;Fresnel;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;-211.28,126.5511;Inherit;False;143;Fresnel;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;145;-111.4714,8.866405;Inherit;False;Constant;_Float3;Float 3;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;102;890.641,-572.6401;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;134;-793.4754,214.8819;Inherit;False;Constant;_Float2;Float 2;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-431.5807,-69.35954;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-515.7211,-523.8881;Inherit;False;95;matCap1;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;18;-547.4421,-425.0761;Inherit;False;Property;_Color;Color;7;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0.312849,1,0.6941177;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;124;-698.4653,-206.7829;Inherit;False;Constant;_Float1;Float 1;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-526.5909,352.3053;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;144;17.11205,44.15658;Inherit;False;Property;_isFresnel;isFresnel;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;123;-207.6421,-134.8066;Inherit;False;Property;_isMatCap2;isMatCap2;8;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;131;-302.6523,286.8582;Inherit;False;Property;_isMatCap3;isMatCap3;11;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;104;1159.263,-510.3306;Inherit;False;halfLambert;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;106.7097,-369.467;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;487.8676,51.9697;Inherit;False;104;halfLambert;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;122;294.287,-222.5889;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;60;-1294.468,391.778;Inherit;False;235.4278;386.7826;Comment;3;30;28;59;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScreenColorNode;151;925.8599,-10.30472;Inherit;False;Global;_GrabScreen0;Grab Screen 0;17;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;582.8066,-156.0678;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;554.8903,-883.4086;Inherit;False;-1;;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;116.0982,285.7759;Inherit;False;Property;_Roughness;Roughness;4;0;Create;True;0;0;0;False;0;False;1;0.07;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1244.468,441.7781;Inherit;False;Property;_CullMode;CullMode;2;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;65;-2540.401,-290.9947;Inherit;False;worldView;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;20;524.0909,186.0817;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceViewDirHlpNode;64;-2858.757,-286.6916;Inherit;False;1;0;FLOAT4;0,0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;25;228.1538,159.2094;Inherit;False;Property;_Metalic;Metalic;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;96;423.0712,302.7187;Inherit;False;95;matCap1;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1229.441,663.5605;Inherit;False;Property;_Dst;Dst;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;838.6385,-215.9761;Inherit;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1231.441,541.5606;Inherit;False;Property;_Src;Src;0;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;156;1114.572,152.5729;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1301.775,-114.4762;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Standard_Trans_MatCap_NoLine;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;ForwardOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;True;28;10;True;30;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.03;0,0,0,0;VertexScale;True;False;Cylindrical;False;True;Relative;0;;3;-1;-1;-1;0;False;0;0;True;59;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;63;0;62;0
WireConnection;69;0;68;0
WireConnection;73;0;72;0
WireConnection;97;0;69;0
WireConnection;70;0;97;0
WireConnection;74;0;73;0
WireConnection;78;0;79;0
WireConnection;78;1;80;0
WireConnection;82;0;78;0
WireConnection;85;0;82;1
WireConnection;81;0;85;0
WireConnection;81;1;82;0
WireConnection;81;2;82;2
WireConnection;86;0;81;0
WireConnection;89;0;86;1
WireConnection;88;0;86;0
WireConnection;87;0;88;0
WireConnection;87;1;89;0
WireConnection;87;2;86;2
WireConnection;77;0;87;0
WireConnection;136;0;138;0
WireConnection;136;1;139;1
WireConnection;136;2;139;2
WireConnection;136;3;139;3
WireConnection;117;0;116;0
WireConnection;113;0;112;0
WireConnection;111;1;113;0
WireConnection;115;1;117;0
WireConnection;150;0;136;0
WireConnection;94;0;92;0
WireConnection;140;0;150;0
WireConnection;140;1;142;0
WireConnection;101;0;98;0
WireConnection;101;1;99;0
WireConnection;118;0;115;0
WireConnection;76;1;94;0
WireConnection;114;0;111;0
WireConnection;95;0;76;0
WireConnection;121;0;101;0
WireConnection;143;0;140;0
WireConnection;102;0;121;0
WireConnection;126;0;125;0
WireConnection;126;1;130;0
WireConnection;126;2;130;4
WireConnection;135;0;132;0
WireConnection;135;1;133;0
WireConnection;135;2;133;4
WireConnection;144;1;145;0
WireConnection;144;0;148;0
WireConnection;123;1;124;0
WireConnection;123;0;126;0
WireConnection;131;1;134;0
WireConnection;131;0;135;0
WireConnection;104;0;102;0
WireConnection;34;0;110;0
WireConnection;34;1;18;0
WireConnection;122;0;34;0
WireConnection;122;1;123;0
WireConnection;122;2;131;0
WireConnection;122;3;144;0
WireConnection;106;0;122;0
WireConnection;106;1;105;0
WireConnection;65;0;64;0
WireConnection;20;0;19;0
WireConnection;156;0;151;0
WireConnection;156;1;106;0
WireConnection;156;2;18;4
WireConnection;0;2;156;0
ASEEND*/
//CHKSM=5211847F5B836660D7AD5572B207A1806BF9F54B