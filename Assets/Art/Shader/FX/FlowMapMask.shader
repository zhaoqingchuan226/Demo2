// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FlowMapMask"
{
	Properties
	{
		_Base("Base", 2D) = "white" {}
		_Size("Size", Range( 0 , 10)) = 1
		_MainSpeed("MainSpeed", Vector) = (-1,0,0,0)
		[HDR]_Color("Color ", Color) = (1,1,1,0)
		_Mask("Mask", 2D) = "white" {}
		_Flow("Flow", 2D) = "white" {}
		_FlowDir("FlowDir", Vector) = (0,0,0,0)
		_FlowIntensity("FlowIntensity", Float) = 1
		_FlowSpeed("FlowSpeed", Float) = 0.2
		[Enum(UnityEngine.Rendering.BlendMode)]_Src("Src", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
		[Toggle(_ISSOFT_ON)] _isSoft("isSoft", Float) = 1
		_SoftRange("SoftRange", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend [_Src] [_Dst]
		
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#pragma target 4.0
		#pragma shader_feature_local _ISSOFT_ON
		#pragma only_renderers d3d11_9x d3d11 glcore gles3 
		#pragma surface surf StandardCustomLighting keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 screenPos;
			float3 worldPos;
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

		uniform float _Dst;
		uniform float _Src;
		uniform float4 _Color;
		uniform sampler2D _Base;
		uniform float2 _MainSpeed;
		uniform sampler2D _Flow;
		uniform float4 _Flow_ST;
		uniform float _Size;
		uniform float2 _FlowDir;
		uniform float _FlowSpeed;
		uniform float _FlowIntensity;
		uniform float4 _Base_ST;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _SoftRange;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_Flow = i.uv_texcoord * _Flow_ST.xy + _Flow_ST.zw;
			float2 temp_output_4_0_g1 = (( uv_Flow / _Size )).xy;
			float2 temp_output_17_0_g1 = float2( 1,1 );
			float mulTime22_g1 = _Time.y * _FlowSpeed;
			float temp_output_27_0_g1 = frac( mulTime22_g1 );
			float2 temp_output_11_0_g1 = ( temp_output_4_0_g1 + ( -(_FlowDir*2.0 + -1.0) * temp_output_17_0_g1 * temp_output_27_0_g1 ) );
			float temp_output_55_0_g1 = 1.0;
			float2 temp_output_12_0_g1 = ( temp_output_4_0_g1 + ( -(_FlowDir*2.0 + -1.0) * temp_output_17_0_g1 * frac( ( mulTime22_g1 + 0.5 ) ) ) );
			float3 lerpResult9_g1 = lerp( UnpackScaleNormal( tex2D( _Flow, temp_output_11_0_g1 ), temp_output_55_0_g1 ) , UnpackScaleNormal( tex2D( _Flow, temp_output_12_0_g1 ), temp_output_55_0_g1 ) , ( abs( ( temp_output_27_0_g1 - 0.5 ) ) / 0.5 ));
			float2 Flow18 = (( lerpResult9_g1 * _FlowIntensity * 0.1 )).xy;
			float2 uv_Base = i.uv_texcoord * _Base_ST.xy + _Base_ST.zw;
			float2 panner4 = ( 1.0 * _Time.y * _MainSpeed + ( Flow18 + uv_Base ));
			float4 tex2DNode2 = tex2D( _Base, panner4 );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float temp_output_29_0 = ( i.vertexColor.a * ( tex2DNode2.r * tex2D( _Mask, uv_Mask ).r ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float eyeDepth5_g2 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float3 ase_worldPos = i.worldPos;
			float3 worldToView3_g2 = mul( UNITY_MATRIX_V, float4( ase_worldPos, 1 ) ).xyz;
			#ifdef _ISSOFT_ON
				float staticSwitch30 = ( temp_output_29_0 * saturate( ( saturate( ( ( eyeDepth5_g2 + worldToView3_g2.z ) - 0.0 ) ) / _SoftRange ) ) );
			#else
				float staticSwitch30 = temp_output_29_0;
			#endif
			c.rgb = 0;
			c.a = staticSwitch30;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float2 uv_Flow = i.uv_texcoord * _Flow_ST.xy + _Flow_ST.zw;
			float2 temp_output_4_0_g1 = (( uv_Flow / _Size )).xy;
			float2 temp_output_17_0_g1 = float2( 1,1 );
			float mulTime22_g1 = _Time.y * _FlowSpeed;
			float temp_output_27_0_g1 = frac( mulTime22_g1 );
			float2 temp_output_11_0_g1 = ( temp_output_4_0_g1 + ( -(_FlowDir*2.0 + -1.0) * temp_output_17_0_g1 * temp_output_27_0_g1 ) );
			float temp_output_55_0_g1 = 1.0;
			float2 temp_output_12_0_g1 = ( temp_output_4_0_g1 + ( -(_FlowDir*2.0 + -1.0) * temp_output_17_0_g1 * frac( ( mulTime22_g1 + 0.5 ) ) ) );
			float3 lerpResult9_g1 = lerp( UnpackScaleNormal( tex2D( _Flow, temp_output_11_0_g1 ), temp_output_55_0_g1 ) , UnpackScaleNormal( tex2D( _Flow, temp_output_12_0_g1 ), temp_output_55_0_g1 ) , ( abs( ( temp_output_27_0_g1 - 0.5 ) ) / 0.5 ));
			float2 Flow18 = (( lerpResult9_g1 * _FlowIntensity * 0.1 )).xy;
			float2 uv_Base = i.uv_texcoord * _Base_ST.xy + _Base_ST.zw;
			float2 panner4 = ( 1.0 * _Time.y * _MainSpeed + ( Flow18 + uv_Base ));
			float4 tex2DNode2 = tex2D( _Base, panner4 );
			o.Emission = ( ( _Color * tex2DNode2.r ) * i.vertexColor ).rgb;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1274;12;1280;648;205.8385;43.04581;1;True;False
Node;AmplifyShaderEditor.Vector2Node;12;-19.02088,734.3546;Inherit;False;Property;_FlowDir;FlowDir;6;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;9;-290.6009,485.8344;Inherit;True;Property;_Flow;Flow;5;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-183.0046,618.9078;Inherit;False;0;9;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-68.25622,866.33;Inherit;False;Property;_FlowSpeed;FlowSpeed;8;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;10;157.9703,511.3516;Inherit;False;Flow;0;;1;acad10cc8145e1f4eb8042bebe2d9a42;2,50,1,51,1;6;5;SAMPLER2D;;False;2;FLOAT2;0,0;False;55;FLOAT;1;False;18;FLOAT2;0,0;False;17;FLOAT2;1,1;False;24;FLOAT;0.2;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;17;377.4288,746.6508;Inherit;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;375.9449,654.4552;Inherit;False;Property;_FlowIntensity;FlowIntensity;7;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;582.2084,635.1646;Inherit;False;3;3;0;FLOAT3;1,0,0;False;1;FLOAT;0.1;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;25;704.5877,717.4465;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;864.4261,651.9323;Inherit;False;Flow;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-519.6248,-123.7539;Inherit;False;18;Flow;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-667.2148,-4.706738;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-308.5366,-36.96278;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;5;-608.1378,165.249;Inherit;False;Property;_MainSpeed;MainSpeed;2;0;Create;True;0;0;0;False;0;False;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;4;-191.8003,64.09285;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-11.48224,-11.43243;Inherit;True;Property;_Base;Base;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-346.5242,209.9514;Inherit;True;Property;_Mask;Mask;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;27;430.3556,43.91758;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;434.2768,222.0862;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;252.7369,406.6806;Inherit;False;Property;_SoftRange;SoftRange;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;4.70158,-233.6737;Inherit;False;Property;_Color;Color ;3;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;616.8339,220.5964;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;32;416.7369,352.6806;Inherit;False;DeferDepthFade;-1;;2;be949e34c5cbe0d45a6a88be82ad6ef3;0;4;1;FLOAT;0;False;4;FLOAT;0;False;8;FLOAT;0;False;10;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;322.4824,-106.1834;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;718.7369,343.6806;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-53.79865,1050.929;Inherit;False;Property;_Dst;Dst;10;1;[Enum];Create;True;0;1;Option1;0;1;UnityEngine.Rendering.BlendMode;True;0;False;10;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-188.9987,1027.529;Inherit;False;Property;_Src;Src;9;1;[Enum];Create;True;0;1;Option1;0;1;UnityEngine.Rendering.BlendMode;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;636.2739,16.19782;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;30;848.7369,249.6806;Inherit;False;Property;_isSoft;isSoft;11;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1036.367,-67.96175;Float;False;True;-1;4;ASEMaterialInspector;0;0;CustomLighting;FlowMapMask;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;ForwardOnly;4;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;8;5;True;22;1;True;24;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;5;9;0
WireConnection;10;2;11;0
WireConnection;10;18;12;0
WireConnection;10;24;13;0
WireConnection;16;0;10;0
WireConnection;16;1;15;0
WireConnection;16;2;17;0
WireConnection;25;0;16;0
WireConnection;18;0;25;0
WireConnection;21;0;19;0
WireConnection;21;1;3;0
WireConnection;4;0;21;0
WireConnection;4;2;5;0
WireConnection;2;1;4;0
WireConnection;6;0;2;1
WireConnection;6;1;1;1
WireConnection;29;0;27;4
WireConnection;29;1;6;0
WireConnection;32;10;33;0
WireConnection;8;0;7;0
WireConnection;8;1;2;1
WireConnection;31;0;29;0
WireConnection;31;1;32;0
WireConnection;28;0;8;0
WireConnection;28;1;27;0
WireConnection;30;1;29;0
WireConnection;30;0;31;0
WireConnection;0;2;28;0
WireConnection;0;9;30;0
ASEEND*/
//CHKSM=CE631E9D98C7C44DE81BD5BAD260A9FC1F9D6F1E