// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_Branchs"
{
	Properties
	{
		_VRange("VRange", Float) = 0
		_OffsetIntensity("OffsetIntensity", Float) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		_Albedo("Albedo", 2D) = "white" {}
		_Albedo1("Albedo1", 2D) = "white" {}
		_Albedo2("Albedo2", 2D) = "white" {}
		_Color("Color", Color) = (1,0.3544474,0.3544474,0)
		_Color1("Color1", Color) = (1,0.3544474,0.3544474,0)
		_Roughness("Roughness", Range( -1 , 1)) = 1
		_Metalic("Metalic", Range( -1 , 1)) = 0
		_Normal1("Normal1", 2D) = "bump" {}
		_Normal("Normal", 2D) = "bump" {}
		_Normal2("Normal2", 2D) = "bump" {}
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
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ISEMISSIVE_ON
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:forward novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _CullMode;
		uniform float _OffsetIntensity;
		uniform float _factor0;
		uniform float _VRange;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Normal1;
		uniform float4 _Normal1_ST;
		uniform float _factor1;
		uniform sampler2D _Normal2;
		uniform float4 _Normal2_ST;
		uniform float _factor3;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform float4 _Color1;
		uniform sampler2D _Albedo1;
		uniform float4 _Albedo1_ST;
		uniform sampler2D _Albedo2;
		uniform float4 _Albedo2_ST;
		uniform float4 _EColor;
		uniform float _Metalic;
		uniform float _Roughness;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float factor0125 = _factor0;
			float temp_output_102_0 = (factor0125*2.0 + -1.0);
			float smoothstepResult80 = smoothstep( temp_output_102_0 , ( temp_output_102_0 + _VRange ) , v.texcoord.xy.y);
			float V91 = max( smoothstepResult80 , pow( v.texcoord.xy.y , 5.0 ) );
			v.vertex.xyz += ( _OffsetIntensity * -ase_vertexNormal * 0.1 * V91 );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float2 uv_Normal1 = i.uv_texcoord * _Normal1_ST.xy + _Normal1_ST.zw;
			float factor1127 = _factor1;
			float3 lerpResult115 = lerp( UnpackScaleNormal( tex2D( _Normal, uv_Normal ), 0.1 ) , UnpackScaleNormal( tex2D( _Normal1, uv_Normal1 ), 0.4 ) , factor1127);
			float2 uv_Normal2 = i.uv_texcoord * _Normal2_ST.xy + _Normal2_ST.zw;
			float factor2129 = _factor3;
			float3 lerpResult119 = lerp( lerpResult115 , UnpackNormal( tex2D( _Normal2, uv_Normal2 ) ) , factor2129);
			o.Normal = lerpResult119;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float factor0125 = _factor0;
			float4 lerpResult106 = lerp( _Color , _Color1 , factor0125);
			float2 uv_Albedo1 = i.uv_texcoord * _Albedo1_ST.xy + _Albedo1_ST.zw;
			float4 lerpResult108 = lerp( ( tex2D( _Albedo, uv_Albedo ) * lerpResult106 ) , tex2D( _Albedo1, uv_Albedo1 ) , factor1127);
			float2 uv_Albedo2 = i.uv_texcoord * _Albedo2_ST.xy + _Albedo2_ST.zw;
			float4 lerpResult110 = lerp( lerpResult108 , tex2D( _Albedo2, uv_Albedo2 ) , factor2129);
			o.Albedo = lerpResult110.rgb;
			float4 temp_cast_1 = (0.0).xxxx;
			#ifdef _ISEMISSIVE_ON
				float4 staticSwitch33 = _EColor;
			#else
				float4 staticSwitch33 = temp_cast_1;
			#endif
			o.Emission = staticSwitch33.rgb;
			o.Metallic = _Metalic;
			o.Smoothness = ( 1.0 - saturate( _Roughness ) );
			o.Alpha = 1;
			float temp_output_102_0 = (factor0125*2.0 + -1.0);
			float smoothstepResult80 = smoothstep( temp_output_102_0 , ( temp_output_102_0 + _VRange ) , i.uv_texcoord.y);
			float V91 = max( smoothstepResult80 , pow( i.uv_texcoord.y , 5.0 ) );
			clip( step( V91 , 0.99 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
0;0;1536;803;1496.665;1269.376;2.650738;True;False
Node;AmplifyShaderEditor.RangedFloatNode;104;-880.2139,-268.4933;Inherit;False;Global;_factor0;_factor0;13;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;125;-510.3193,-280.9507;Inherit;False;factor0;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;103;840.0684,501.762;Inherit;False;1132.08;965.5353;Comment;17;85;102;84;99;98;91;88;87;96;90;97;92;94;89;95;80;79;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;135;832.0373,688.8901;Inherit;False;125;factor0;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;1033.445,836.8926;Inherit;False;Property;_VRange;VRange;0;0;Create;True;0;0;0;False;0;False;0;0.62;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;102;1032.185,672.6192;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;84;1259.445,743.8926;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;79;1147.228,551.762;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;18;-206.9734,-715.5659;Inherit;False;Property;_Color;Color;7;0;Create;True;0;0;0;False;0;False;1,0.3544474,0.3544474,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;130;-45.62799,-340.269;Inherit;False;125;factor0;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;109;-930.8115,-115.5324;Inherit;False;Global;_factor1;_factor1;13;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;99;1359.442,936.3214;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;105;-199.0122,-497.2696;Inherit;False;Property;_Color1;Color1;8;0;Create;True;0;0;0;False;0;False;1,0.3544474,0.3544474,0;0.5820783,0.7547169,0.4243502,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;80;1478.407,632.1648;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;106;261.9387,-534.942;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;128;-970.9173,3.010078;Inherit;False;Global;_factor3;_factor2;13;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;127;-545.2564,-127.2576;Inherit;False;factor1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-640.3079,715.061;Inherit;False;Constant;_Float4;Float 4;17;0;Create;True;0;0;0;False;0;False;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;98;1640.682,662.3333;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-599.9681,587.9984;Inherit;False;Constant;_Float2;Float 2;17;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-37.92286,-908.0015;Inherit;True;Property;_Albedo;Albedo;4;0;Create;True;0;0;0;False;0;False;-1;None;0edb67cafa1294e45a4c2733178b0600;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;91;1745.748,726.9386;Inherit;False;V;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;107;262.3854,-411.4786;Inherit;True;Property;_Albedo1;Albedo1;5;0;Create;True;0;0;0;False;0;False;-1;None;495e8c6df98989f4cacf61741b44947f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;131;148.6857,-259.3047;Inherit;False;127;factor1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;1253.414,-419.6117;Inherit;False;Property;_Roughness;Roughness;9;0;Create;True;0;0;0;False;0;False;1;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;482.5827,-704.2101;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;116;-447.6237,745.6426;Inherit;True;Property;_Normal1;Normal1;11;0;Create;True;0;0;0;False;0;False;-1;None;a14745ca802a3bb468d0a53d4c270a47;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;133;-60.18079,818.4302;Inherit;False;127;factor1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;48;-437.2762,553.9734;Inherit;True;Property;_Normal;Normal;12;0;Create;True;0;0;0;False;0;False;-1;None;6e125c4aa4a532b449e32a54fc4dcdf7;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;88;1273.272,1096.963;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;129;-592.4774,-3.062988;Inherit;False;factor2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;1599.179,961.1014;Inherit;False;Constant;_Float3;Float 3;16;0;Create;True;0;0;0;False;0;False;0.99;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;134;146.032,1012.971;Inherit;False;129;factor2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;1446.705,1034.729;Inherit;False;Property;_OffsetIntensity;OffsetIntensity;1;0;Create;True;0;0;0;False;0;False;0;0.21;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;108;687.5314,-500.6125;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;111;508.4034,-297.5922;Inherit;True;Property;_Albedo2;Albedo2;6;0;Create;True;0;0;0;False;0;False;-1;None;773bcb4ed7b880142b7da5b2d493fbed;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;94;1605.289,868.42;Inherit;False;91;V;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-119.1774,122.344;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;1505.38,1352.297;Inherit;False;91;V;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;97;1470.585,1212.544;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;132;594.2905,-70.32504;Inherit;False;129;factor2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;24;1594.795,-273.208;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;1544.47,1255.859;Inherit;False;Constant;_Float1;Float 1;14;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;115;-6.800167,633.61;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;117;-162.3676,936.5629;Inherit;True;Property;_Normal2;Normal2;13;0;Create;True;0;0;0;False;0;False;-1;None;a4819310e33b17c49a01c8f76f199f79;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;36;-390.8814,-52.62929;Inherit;False;Property;_EColor;EColor;15;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;32;-961.5894,467.1856;Inherit;False;Property;_CullMode;CullMode;3;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;62;507.8524,241.4906;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;33;347.5417,21.74913;Inherit;False;Property;_IsEmissive;IsEmissive;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;95;1807.692,877.1104;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;60;69.29469,323.5017;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;119;340.9667,746.9966;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;1801.105,1029.729;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;110;909.35,-406.9413;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;25;1719.086,-9.910128;Inherit;False;Property;_Metalic;Metalic;10;0;Create;True;0;0;0;False;0;False;0;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;61;251.1936,250.4768;Inherit;False;Object;Tangent;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;20;1876.557,-267.8828;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2195.546,129.4029;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Standard_Branchs;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;DeferredOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;2;-1;-1;-1;0;False;0;0;True;32;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;125;0;104;0
WireConnection;102;0;135;0
WireConnection;84;0;102;0
WireConnection;84;1;85;0
WireConnection;99;0;79;2
WireConnection;80;0;79;2
WireConnection;80;1;102;0
WireConnection;80;2;84;0
WireConnection;106;0;18;0
WireConnection;106;1;105;0
WireConnection;106;2;130;0
WireConnection;127;0;109;0
WireConnection;98;0;80;0
WireConnection;98;1;99;0
WireConnection;91;0;98;0
WireConnection;31;0;30;0
WireConnection;31;1;106;0
WireConnection;116;5;124;0
WireConnection;48;5;123;0
WireConnection;129;0;128;0
WireConnection;108;0;31;0
WireConnection;108;1;107;0
WireConnection;108;2;131;0
WireConnection;97;0;88;0
WireConnection;24;0;19;0
WireConnection;115;0;48;0
WireConnection;115;1;116;0
WireConnection;115;2;133;0
WireConnection;62;0;61;0
WireConnection;33;1;34;0
WireConnection;33;0;36;0
WireConnection;95;0;94;0
WireConnection;95;1;96;0
WireConnection;119;0;115;0
WireConnection;119;1;117;0
WireConnection;119;2;134;0
WireConnection;89;0;87;0
WireConnection;89;1;97;0
WireConnection;89;2;90;0
WireConnection;89;3;92;0
WireConnection;110;0;108;0
WireConnection;110;1;111;0
WireConnection;110;2;132;0
WireConnection;61;0;60;0
WireConnection;20;0;24;0
WireConnection;0;0;110;0
WireConnection;0;1;119;0
WireConnection;0;2;33;0
WireConnection;0;3;25;0
WireConnection;0;4;20;0
WireConnection;0;10;95;0
WireConnection;0;11;89;0
ASEEND*/
//CHKSM=D4F64F03FB5607AAE91A3ACF8805D2C3232C00A8