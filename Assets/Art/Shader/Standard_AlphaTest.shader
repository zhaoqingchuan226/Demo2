// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_AlphaTest"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.28
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,0.3544474,0.3544474,0)
		_MRAE("MRAE", 2D) = "white" {}
		_Roughness("Roughness", Range( -1 , 1)) = 1
		_Metalic("Metalic", Range( -1 , 1)) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		[Toggle(_ISEMISSIVE_ON)] _IsEmissive("IsEmissive", Float) = 0
		[HDR]_EColor("EColor", Color) = (1,1,1,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _ISEMISSIVE_ON
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:forward novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _CullMode;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform float4 _EColor;
		uniform sampler2D _MRAE;
		uniform float4 _MRAE_ST;
		uniform float _Metalic;
		uniform float _Roughness;
		uniform float _Cutoff = 0.28;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode30 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( tex2DNode30 * _Color ).rgb;
			float4 temp_cast_1 = (0.0).xxxx;
			float2 uv_MRAE = i.uv_texcoord * _MRAE_ST.xy + _MRAE_ST.zw;
			float4 saferPower41 = abs( tex2D( _MRAE, uv_MRAE ) );
			float4 break43 = pow( saferPower41 , 0.45 );
			#ifdef _ISEMISSIVE_ON
				float4 staticSwitch33 = ( _EColor * break43.a );
			#else
				float4 staticSwitch33 = temp_cast_1;
			#endif
			o.Emission = staticSwitch33.rgb;
			o.Metallic = saturate( ( break43.r + _Metalic ) );
			o.Smoothness = ( 1.0 - saturate( ( break43.g + _Roughness ) ) );
			o.Occlusion = break43.b;
			o.Alpha = 1;
			clip( tex2DNode30.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1456.571;12.57143;1462.857;750.4286;1181.71;721.6234;1.217613;True;False
Node;AmplifyShaderEditor.SamplerNode;21;-1324.996,-51.59125;Inherit;True;Property;_MRAE;MRAE;3;0;Create;True;0;0;0;False;0;False;-1;None;6b8ab8dfaaace4b4f8a59bc9b9125ec2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;41;-1012.283,-77.01244;Inherit;False;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.45;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;43;-801.2758,-151.6472;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;19;-657.1017,427.3165;Inherit;False;Property;_Roughness;Roughness;4;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-780.1155,181.4457;Inherit;False;Property;_Metalic;Metalic;5;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;-433.3409,-204.6431;Inherit;False;Property;_EColor;EColor;8;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;7.464264,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-349.054,337.9617;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-170.0925,-585.269;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;0;False;0;False;-1;None;d012ac0f0e44e224ab942e79b17a6a76;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;18;-251.793,-363.0797;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;1,0.3544474,0.3544474,0;1,0.5622642,0.5622642,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;10.10856,-58.13164;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;24;-229.0494,327.6592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-217.3945,62.85425;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-20.06026,-182.2785;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;273.1206,-189.9524;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;33;222.0824,-62.66193;Inherit;False;Property;_IsEmissive;IsEmissive;7;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;20;-58.16007,233.8573;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-961.5894,467.1856;Inherit;False;Property;_CullMode;CullMode;6;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;28;38.23729,60.4586;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;831.9144,-27.93621;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Standard_AlphaTest;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.28;True;True;0;True;Opaque;;AlphaTest;DeferredOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;True;32;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;41;0;21;0
WireConnection;43;0;41;0
WireConnection;23;0;43;1
WireConnection;23;1;19;0
WireConnection;37;0;36;0
WireConnection;37;1;43;3
WireConnection;24;0;23;0
WireConnection;27;0;43;0
WireConnection;27;1;25;0
WireConnection;31;0;30;0
WireConnection;31;1;18;0
WireConnection;33;1;34;0
WireConnection;33;0;37;0
WireConnection;20;0;24;0
WireConnection;28;0;27;0
WireConnection;0;0;31;0
WireConnection;0;2;33;0
WireConnection;0;3;28;0
WireConnection;0;4;20;0
WireConnection;0;5;43;2
WireConnection;0;10;30;4
ASEEND*/
//CHKSM=948FF56E7B09EC6DD0A27D7DD70CB2E93E0885B1