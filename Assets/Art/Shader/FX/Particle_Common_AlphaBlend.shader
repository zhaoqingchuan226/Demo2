// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Particle_Common_AlphaBlend"
{
	Properties
	{
		_Mask("Mask", 2D) = "white" {}
		[HDR]_Color("Color", Color) = (1,1,1,0)
		[Enum(UnityEngine.Rendering.BlendMode)]_Src("Src", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
		[Toggle(_ISSOFT_ON)] _isSoft("isSoft", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		Blend [_Src] [_Dst]
		
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ISSOFT_ON
		#pragma only_renderers d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Unlit keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 screenPos;
			float3 worldPos;
		};

		uniform float _Src;
		uniform float _Dst;
		uniform float4 _Color;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = ( i.vertexColor * _Color ).rgb;
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float temp_output_7_0 = ( i.vertexColor.a * tex2D( _Mask, uv_Mask ).r * _Color.a );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float eyeDepth5_g3 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float3 ase_worldPos = i.worldPos;
			float3 worldToView3_g3 = mul( UNITY_MATRIX_V, float4( ase_worldPos, 1 ) ).xyz;
			#ifdef _ISSOFT_ON
				float staticSwitch14 = ( temp_output_7_0 * saturate( ( saturate( ( ( eyeDepth5_g3 + worldToView3_g3.z ) - 0.0 ) ) / 0.01 ) ) );
			#else
				float staticSwitch14 = temp_output_7_0;
			#endif
			o.Alpha = staticSwitch14;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1274;155.5;1280;504.5;1157.327;-102.8529;1.738338;True;False
Node;AmplifyShaderEditor.SamplerNode;4;-450.2805,327.5635;Inherit;True;Property;_Mask;Mask;1;0;Create;True;0;0;0;False;0;False;-1;None;79aaf33959cc78f4cb13d2037e36fdc1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;5;-501.177,-38.24839;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-609.5,129;Inherit;False;Property;_Color;Color;2;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;38.93715,38.93715,38.93715,0.5372549;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-456.1531,658.6573;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-116.7709,218.0222;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;17;-327.2206,535.2726;Inherit;False;DeferDepthFade;-1;;3;be949e34c5cbe0d45a6a88be82ad6ef3;0;4;1;FLOAT;0;False;4;FLOAT;0;False;8;FLOAT;0;False;10;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-60.3313,467.2299;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-261.2642,17.64041;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-813.0193,349.429;Inherit;False;Property;_Src;Src;3;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-806.0193,476.429;Inherit;False;Property;_Dst;Dst;4;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;10;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;14;59.71977,357.9942;Inherit;False;Property;_isSoft;isSoft;5;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;206.2215,13.63141;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Particle_Common_AlphaBlend;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;ForwardOnly;4;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;True;8;10;True;9;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;5;4
WireConnection;7;1;4;1
WireConnection;7;2;1;4
WireConnection;17;10;16;0
WireConnection;15;0;7;0
WireConnection;15;1;17;0
WireConnection;6;0;5;0
WireConnection;6;1;1;0
WireConnection;14;1;7;0
WireConnection;14;0;15;0
WireConnection;0;2;6;0
WireConnection;0;9;14;0
ASEEND*/
//CHKSM=F43365687428A73458160BBD3B33C75F47490C4E