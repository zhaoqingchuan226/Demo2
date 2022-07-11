// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Flow_Dissolve"
{
	Properties
	{
		_Size("Size", Range( 0 , 10)) = 1
		_Main("Main", 2D) = "white" {}
		_Dissolve("Dissolve", 2D) = "white" {}
		_FlowMap("FlowMap", 2D) = "white" {}
		_FlowSpeed("FlowSpeed", Vector) = (3,3,0,0)
		[HDR]_Color("Color", Color) = (1,1,1,0)
		_Soft("Soft", Range( 0 , 1)) = 0.1
		_FlowIntensity("FlowIntensity", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)]_Src("Src", Float) = 6
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 2
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 11
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_Cull]
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma only_renderers d3d11_9x d3d11 glcore gles3 
		#pragma surface surf StandardCustomLighting keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
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

		uniform float _Dst;
		uniform float _Src;
		uniform float _Cull;
		uniform float4 _Color;
		uniform sampler2D _Main;
		uniform sampler2D _FlowMap;
		uniform float _Size;
		uniform float2 _FlowSpeed;
		uniform float _FlowIntensity;
		uniform float _Soft;
		uniform sampler2D _Dissolve;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 temp_output_4_0_g1 = (( i.uv_texcoord.xy / _Size )).xy;
			float2 temp_output_17_0_g1 = _FlowSpeed;
			float mulTime22_g1 = _Time.y * 0.2;
			float temp_output_27_0_g1 = frac( mulTime22_g1 );
			float2 temp_output_11_0_g1 = ( temp_output_4_0_g1 + ( -(-(i.vertexColor).rg*2.0 + -1.0) * temp_output_17_0_g1 * temp_output_27_0_g1 ) );
			float temp_output_55_0_g1 = 1.0;
			float2 temp_output_12_0_g1 = ( temp_output_4_0_g1 + ( -(-(i.vertexColor).rg*2.0 + -1.0) * temp_output_17_0_g1 * frac( ( mulTime22_g1 + 0.5 ) ) ) );
			float3 lerpResult9_g1 = lerp( UnpackScaleNormal( tex2D( _FlowMap, temp_output_11_0_g1 ), temp_output_55_0_g1 ) , UnpackScaleNormal( tex2D( _FlowMap, temp_output_12_0_g1 ), temp_output_55_0_g1 ) , ( abs( ( temp_output_27_0_g1 - 0.5 ) ) / 0.5 ));
			float4 tex2DNode2 = tex2D( _Main, ( float3( i.uv_texcoord.xy ,  0.0 ) + ( lerpResult9_g1 * _FlowIntensity * 0.1 ) ).xy );
			float smoothstepResult16 = smoothstep( i.uv_texcoord.z , ( i.uv_texcoord.z + _Soft ) , ( tex2DNode2.r + tex2D( _Dissolve, i.uv_texcoord.xy ).r ));
			c.rgb = 0;
			c.a = ( tex2DNode2.r * smoothstepResult16 * i.vertexColor.a );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float2 temp_output_4_0_g1 = (( i.uv_texcoord.xy / _Size )).xy;
			float2 temp_output_17_0_g1 = _FlowSpeed;
			float mulTime22_g1 = _Time.y * 0.2;
			float temp_output_27_0_g1 = frac( mulTime22_g1 );
			float2 temp_output_11_0_g1 = ( temp_output_4_0_g1 + ( -(-(i.vertexColor).rg*2.0 + -1.0) * temp_output_17_0_g1 * temp_output_27_0_g1 ) );
			float temp_output_55_0_g1 = 1.0;
			float2 temp_output_12_0_g1 = ( temp_output_4_0_g1 + ( -(-(i.vertexColor).rg*2.0 + -1.0) * temp_output_17_0_g1 * frac( ( mulTime22_g1 + 0.5 ) ) ) );
			float3 lerpResult9_g1 = lerp( UnpackScaleNormal( tex2D( _FlowMap, temp_output_11_0_g1 ), temp_output_55_0_g1 ) , UnpackScaleNormal( tex2D( _FlowMap, temp_output_12_0_g1 ), temp_output_55_0_g1 ) , ( abs( ( temp_output_27_0_g1 - 0.5 ) ) / 0.5 ));
			float4 tex2DNode2 = tex2D( _Main, ( float3( i.uv_texcoord.xy ,  0.0 ) + ( lerpResult9_g1 * _FlowIntensity * 0.1 ) ).xy );
			o.Emission = ( _Color * tex2DNode2 * i.vertexColor ).rgb;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1274;6.5;1280;653.5;2360.235;939.1101;3.45214;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;3;-1195.806,79.20978;Inherit;True;Property;_FlowMap;FlowMap;6;0;Create;True;0;0;0;False;0;False;38e7d5e6b7b166e4bb037342aa25de38;38e7d5e6b7b166e4bb037342aa25de38;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1090,208.0352;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;7;-1014.914,359.8854;Inherit;False;Property;_FlowSpeed;FlowSpeed;7;0;Create;True;0;0;0;False;0;False;3,3;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;21;-710.0806,309.9794;Inherit;False;Property;_FlowIntensity;FlowIntensity;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;5;-816.4818,150.9324;Inherit;False;Flow;0;;1;acad10cc8145e1f4eb8042bebe2d9a42;2,50,1,51,1;6;5;SAMPLER2D;;False;2;FLOAT2;0,0;False;55;FLOAT;1;False;18;FLOAT2;0,0;False;17;FLOAT2;1,1;False;24;FLOAT;0.2;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-672.0806,401.9794;Inherit;False;Constant;_Float1;Float 1;10;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-534.6238,222.4912;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-665.5052,-50.88043;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-442.5918,111.9178;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-442.5455,345.4552;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-281.3275,-27.19934;Inherit;True;Property;_Main;Main;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;18;-97.52438,412.0738;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-239.4997,210.6262;Inherit;True;Property;_Dissolve;Dissolve;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-98.94714,596.4152;Inherit;False;Property;_Soft;Soft;9;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;66.59564,140.4552;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;201.0529,535.4152;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;16;223.8127,280.0367;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;26;57.47514,-31.58749;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-118.2924,-213.1709;Inherit;False;Property;_Color;Color;8;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;247.515,-44.6217;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-377.8125,753.3163;Inherit;False;Property;_Cull;Cull;12;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;30;436.1297,-145.3823;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-646.8573,612.2145;Inherit;False;Property;_Dst;Dst;13;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;11;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-611.0842,499.0255;Inherit;False;Property;_Src;Src;11;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;291.2845,121.1117;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;752.5566,-125.7544;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Flow_Dissolve;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;ForwardOnly;4;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;3;-1;-1;-1;0;False;0;0;True;27;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;5;3;0
WireConnection;5;2;6;0
WireConnection;5;17;7;0
WireConnection;23;0;5;0
WireConnection;23;1;21;0
WireConnection;23;2;22;0
WireConnection;9;0;8;0
WireConnection;9;1;23;0
WireConnection;2;1;9;0
WireConnection;4;1;25;0
WireConnection;15;0;2;1
WireConnection;15;1;4;1
WireConnection;20;0;18;3
WireConnection;20;1;19;0
WireConnection;16;0;15;0
WireConnection;16;1;18;3
WireConnection;16;2;20;0
WireConnection;14;0;13;0
WireConnection;14;1;2;0
WireConnection;14;2;26;0
WireConnection;24;0;2;1
WireConnection;24;1;16;0
WireConnection;24;2;26;4
WireConnection;0;2;14;0
WireConnection;0;9;24;0
ASEEND*/
//CHKSM=5754EE67C3E78486910CBB976B05E5F5D8D8E4BC