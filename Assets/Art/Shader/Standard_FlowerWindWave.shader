// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_FlowerWindWave"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:forward novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float4 vertexColor : COLOR;
		};

		uniform float _CullMode;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float mulTime80 = _Time.y * 2.0;
			float temp_output_81_0 = (sin( mulTime80 )*0.5 + 0.5);
			v.vertex.xyz += ( float3( 0,0,0 ) + ( ( ( ( ase_vertex3Pos * 1.1 ) - ase_vertex3Pos ) * v.color.r ) * temp_output_81_0 ) );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_64_1 = i.vertexColor.r;
			float3 temp_cast_0 = (temp_output_64_1).xxx;
			o.Emission = temp_cast_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1274;128;1280;532;-352.9698;32.57909;2.08207;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;91;1879.392,588.9756;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;94;1943.407,739.7009;Inherit;False;Constant;_Float12;Float 12;15;0;Create;True;0;0;0;False;0;False;1.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;1199.199,1150.952;Inherit;False;Constant;_Float14;Float 14;15;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;2161.407,635.701;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;95;1995.581,810.9227;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;80;1351.76,1152.445;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;78;1650.718,1069.738;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;96;2337.208,715.1011;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexColorNode;64;1843.334,269.4741;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;2458.283,805.9601;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;81;1853.993,1032.094;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;2655.415,847.4035;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;63;-955.9695,-197.2466;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;18;-251.793,-363.0797;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;1,0.3544474,0.3544474,0;0.745283,0.745283,0.745283,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexToFragmentNode;62;466.6127,313.1176;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;285.1206,-213.9524;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;48;409.1035,546.3427;Inherit;True;Property;_Normal;Normal;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;30;-170.0925,-585.269;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;0;False;0;False;-1;None;621e2747323ee80409057aaaf5d7d29d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-349.054,337.9617;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;43;-685.0132,-95.21561;Inherit;False;COLOR;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TransformDirectionNode;61;268.5577,285.205;Inherit;False;Object;Tangent;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;34;-20.06026,-182.2785;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;24;-229.0494,327.6592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;85;1358.571,815.5823;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;76;1454.62,978.1841;Inherit;False;Constant;_Float13;Float 13;0;0;Create;True;0;0;0;False;0;False;0.001;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;33;178.0824,-62.66193;Inherit;False;Property;_IsEmissive;IsEmissive;9;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TransformDirectionNode;71;1122.257,600.402;Inherit;False;World;Object;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;32;-961.5894,467.1856;Inherit;False;Property;_CullMode;CullMode;0;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;58;720.6867,213.6882;Inherit;False;Property;_isNormal;isNormal;7;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-657.1017,427.3165;Inherit;False;Property;_Roughness;Roughness;5;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;10.10856,-58.13164;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;60;82.31779,353.8889;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwitchByFaceNode;52;705.9155,-163.8774;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-217.3945,62.85425;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;69;744.5363,704.5583;Inherit;False;Property;_WindDirection;WindDirection;13;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;68;767.6011,537.6198;Inherit;False;Property;_WindSpeed;WindSpeed;12;0;Create;True;0;0;0;False;0;False;1;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;737.2261,440.0443;Inherit;False;Property;_WindWaveScale;WindWaveScale;11;0;Create;True;0;0;0;False;0;False;1;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;70;895.704,643.9467;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;73;1336.516,617.6801;Inherit;False;Property;_WindIntensity;WindIntensity;14;0;Create;True;0;0;0;False;0;False;0.01;0.006;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;65;968.4285,431.8383;Inherit;False;SimpleGrassWind;-1;;1;eb6b5a71d4f47f64ab6869a5d5d0a9be;0;5;148;FLOAT;1;False;85;FLOAT;0;False;86;FLOAT;1;False;1;FLOAT;1;False;7;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;1519.222,180.8612;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;1594.41,568.0978;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;57;-1123.061,-393.2682;Inherit;False;Constant;_Vector0;Vector 0;10;0;Create;True;0;0;0;False;0;False;0,0,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;20;-58.16007,233.8573;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;21;-1456.483,-273.5691;Inherit;True;Property;_MRAE;MRAE;4;0;Create;True;0;0;0;False;0;False;-1;None;18e05a1eb0060a44fa3bcb9182ce4bea;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;56;-871.5573,-357.7544;Inherit;False;Property;_isMRAE;isMRAE;3;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;41;-1056.318,-120.6211;Inherit;False;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.45;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-780.1155,181.4457;Inherit;False;Property;_Metalic;Metalic;6;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;384.5384,-359.8366;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;99;2338.487,188.8558;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;36;-375.6716,-166.7028;Inherit;False;Property;_EColor;EColor;10;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;28;38.23729,60.4586;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;540.4073,-95.78464;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;83;1119.983,783.7444;Inherit;False;Property;_Dir;Dir;15;0;Create;True;0;0;0;False;0;False;0,2,0;0,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2582.848,-13.02787;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Standard_FlowerWindWave;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;DeferredOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;32;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;93;0;91;0
WireConnection;93;1;94;0
WireConnection;80;0;86;0
WireConnection;78;0;80;0
WireConnection;96;0;93;0
WireConnection;96;1;95;0
WireConnection;97;0;96;0
WireConnection;97;1;64;1
WireConnection;81;0;78;0
WireConnection;98;0;97;0
WireConnection;98;1;81;0
WireConnection;63;0;41;0
WireConnection;62;0;61;0
WireConnection;31;0;30;0
WireConnection;31;1;18;0
WireConnection;23;0;43;1
WireConnection;23;1;19;0
WireConnection;43;0;56;0
WireConnection;61;0;60;0
WireConnection;24;0;23;0
WireConnection;85;0;83;0
WireConnection;33;1;34;0
WireConnection;33;0;37;0
WireConnection;71;0;65;0
WireConnection;58;1;62;0
WireConnection;58;0;48;0
WireConnection;37;0;36;0
WireConnection;37;1;43;3
WireConnection;52;0;31;0
WireConnection;52;1;54;0
WireConnection;27;0;43;0
WireConnection;27;1;25;0
WireConnection;70;0;69;0
WireConnection;65;148;66;0
WireConnection;65;1;68;0
WireConnection;65;7;70;0
WireConnection;67;0;64;1
WireConnection;67;1;65;0
WireConnection;67;2;73;0
WireConnection;77;0;85;0
WireConnection;77;1;76;0
WireConnection;77;2;64;1
WireConnection;77;3;81;0
WireConnection;20;0;24;0
WireConnection;56;1;57;0
WireConnection;56;0;63;0
WireConnection;41;0;21;0
WireConnection;99;1;98;0
WireConnection;28;0;27;0
WireConnection;54;0;31;0
WireConnection;54;1;53;0
WireConnection;0;2;64;1
WireConnection;0;11;99;0
ASEEND*/
//CHKSM=332624514A43A547F9C3C7D056261B6B64800350