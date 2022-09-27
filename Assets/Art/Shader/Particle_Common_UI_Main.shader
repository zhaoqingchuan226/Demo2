// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Particle_Common_UI_Main"
{
	Properties
	{
		_Main1("Main1", 2D) = "white" {}
		_Size("Size", Range( 0 , 10)) = 1
		_Range("Range", Float) = 0.1
		_Base("Base", Float) = 0.3
		[HDR]_Color("Color", Color) = (1,0.6736357,0,1)
		_FlashSpeed("FlashSpeed", Float) = 1
		_FlowMap("FlowMap", 2D) = "white" {}
		_FlowIntensity("FlowIntensity", Float) = 0
		_FlowSpeed("FlowSpeed", Float) = 0
		_FlowDir("FlowDir", Vector) = (1,1,0,0)
		_Main("Main", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma only_renderers d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Unlit keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _Base;
		uniform float _Range;
		uniform sampler2D _Main;
		uniform float4 _Main_ST;
		uniform float4 _Color;
		uniform float _FlashSpeed;
		uniform sampler2D _Main1;
		uniform sampler2D _FlowMap;
		uniform float _Size;
		uniform float2 _FlowDir;
		uniform float _FlowSpeed;
		uniform float _FlowIntensity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Main = i.uv_texcoord * _Main_ST.xy + _Main_ST.zw;
			float4 tex2DNode41 = tex2D( _Main, uv_Main );
			float luminance19 = Luminance(tex2DNode41.rgb);
			float smoothstepResult20 = smoothstep( _Base , ( _Base + _Range ) , luminance19);
			float mulTime31 = _Time.y * _FlashSpeed;
			float temp_output_32_0 = (sin( mulTime31 )*0.5 + 0.5);
			float2 temp_output_4_0_g1 = (( i.uv_texcoord / _Size )).xy;
			float2 temp_output_17_0_g1 = float2( 1,1 );
			float mulTime22_g1 = _Time.y * _FlowSpeed;
			float temp_output_27_0_g1 = frac( mulTime22_g1 );
			float2 temp_output_11_0_g1 = ( temp_output_4_0_g1 + ( -(_FlowDir*2.0 + -1.0) * temp_output_17_0_g1 * temp_output_27_0_g1 ) );
			float2 temp_output_12_0_g1 = ( temp_output_4_0_g1 + ( -(_FlowDir*2.0 + -1.0) * temp_output_17_0_g1 * frac( ( mulTime22_g1 + 0.5 ) ) ) );
			float4 lerpResult9_g1 = lerp( tex2D( _FlowMap, temp_output_11_0_g1 ) , tex2D( _FlowMap, temp_output_12_0_g1 ) , ( abs( ( temp_output_27_0_g1 - 0.5 ) ) / 0.5 ));
			float TimeFade53 = temp_output_32_0;
			float4 lerpResult56 = lerp( tex2DNode41 , tex2D( _Main1, ( float4( i.uv_texcoord, 0.0 , 0.0 ) + ( lerpResult9_g1 * _FlowIntensity ) ).rg ) , (TimeFade53*0.5 + 0.0));
			o.Emission = ( ( ( smoothstepResult20 * _Color * temp_output_32_0 ) + lerpResult56 ) * i.vertexColor ).rgb;
			o.Alpha = i.vertexColor.a;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1274;17.5;1280;642.5;-1245.339;189.234;1.315515;True;False
Node;AmplifyShaderEditor.RangedFloatNode;28;70.67604,897.6326;Inherit;False;Property;_FlashSpeed;FlashSpeed;7;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;31;236.2041,857.2938;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;602.9134,1372.94;Inherit;False;Property;_FlowSpeed;FlowSpeed;10;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;33;398.9455,1027.953;Inherit;True;Property;_FlowMap;FlowMap;8;0;Create;True;0;0;0;False;0;False;None;38e7d5e6b7b166e4bb037342aa25de38;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;35;430.1207,1147.314;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;39;471.5472,1306.703;Inherit;False;Property;_FlowDir;FlowDir;11;0;Create;True;0;0;0;False;0;False;1,1;1,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SinOpNode;30;418.4239,880.9407;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;32;594.0122,893.2491;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;40;-365.2346,-160.5136;Inherit;True;Property;_Main;Main;12;0;Create;True;0;0;0;False;0;False;None;0a48e978f30f18a43bcd0e48523f0432;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;37;934.0623,1276.041;Inherit;False;Property;_FlowIntensity;FlowIntensity;9;0;Create;True;0;0;0;False;0;False;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;34;763.7815,1092.473;Inherit;False;Flow;1;;1;acad10cc8145e1f4eb8042bebe2d9a42;2,50,0,51,0;6;5;SAMPLER2D;;False;2;FLOAT2;0,0;False;55;FLOAT;1;False;18;FLOAT2;0,0;False;17;FLOAT2;1,1;False;24;FLOAT;0.2;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-97.71558,438.9515;Inherit;False;Property;_Range;Range;4;0;Create;True;0;0;0;False;0;False;0.1;0.67;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;1101.062,1132.041;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;41;-107.7673,-158.5222;Inherit;True;Property;_Main2;Main2;1;0;Create;True;0;0;0;False;0;False;40;None;0a48e978f30f18a43bcd0e48523f0432;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;840.9932,704.8176;Inherit;False;TimeFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-126.7156,327.9515;Inherit;False;Property;_Base;Base;5;0;Create;True;0;0;0;False;0;False;0.3;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;43;985.4415,929.1061;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;22;65.28442,425.9515;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;1334.571,1046.213;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;1139.585,672.9363;Inherit;False;53;TimeFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LuminanceNode;19;138.8768,229.4515;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;20;209.2844,375.9515;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;55;1353.594,685.7577;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;1469.937,932.7106;Inherit;True;Property;_Main1;Main1;1;0;Create;True;0;0;0;False;0;False;40;None;0a48e978f30f18a43bcd0e48523f0432;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;268.2903,545.3551;Inherit;False;Property;_Color;Color;6;1;[HDR];Create;True;0;0;0;False;0;False;1,0.6736357,0,1;2.731139,0,10.43295,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;56;1809.38,569.1073;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;756.3401,328.7251;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;57;2014.916,469.839;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;27;1913.413,386.0467;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;2135.943,306.7151;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;50;1787.78,1035.928;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2312.887,165.7283;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Particle_Common_UI_Main;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;ForwardOnly;4;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;8;10;False;9;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;31;0;28;0
WireConnection;30;0;31;0
WireConnection;32;0;30;0
WireConnection;34;5;33;0
WireConnection;34;2;35;0
WireConnection;34;18;39;0
WireConnection;34;24;38;0
WireConnection;36;0;34;0
WireConnection;36;1;37;0
WireConnection;41;0;40;0
WireConnection;53;0;32;0
WireConnection;22;0;23;0
WireConnection;22;1;21;0
WireConnection;44;0;43;0
WireConnection;44;1;36;0
WireConnection;19;0;41;0
WireConnection;20;0;19;0
WireConnection;20;1;23;0
WireConnection;20;2;22;0
WireConnection;55;0;54;0
WireConnection;18;1;44;0
WireConnection;56;0;41;0
WireConnection;56;1;18;0
WireConnection;56;2;55;0
WireConnection;26;0;20;0
WireConnection;26;1;24;0
WireConnection;26;2;32;0
WireConnection;27;0;26;0
WireConnection;27;1;56;0
WireConnection;58;0;27;0
WireConnection;58;1;57;0
WireConnection;0;2;58;0
WireConnection;0;9;57;4
ASEEND*/
//CHKSM=0B4F578A99E8B79567ACAADEFB20768A2E5AF64A