// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_Mine"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,0.3544474,0.3544474,0)
		[Toggle(_ISMRAE_ON)] _isMRAE("isMRAE", Float) = 1
		_MRAE("MRAE", 2D) = "white" {}
		_Roughness("Roughness", Range( -1 , 1)) = 1
		_Metalic("Metalic", Range( -1 , 1)) = 1
		[Toggle(_ISNORMAL_ON)] _isNormal("isNormal", Float) = 0
		_Normal("Normal", 2D) = "bump" {}
		[Toggle(_ISEMISSIVE_ON)] _IsEmissive("IsEmissive", Float) = 0
		[HDR]_EColor("EColor", Color) = (1,1,1,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _ISNORMAL_ON
		#pragma shader_feature_local _ISEMISSIVE_ON
		#pragma shader_feature_local _ISMRAE_ON
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:forward novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float3 vertexToFrag62;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
		};

		uniform float _CullMode;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform float4 _EColor;
		uniform sampler2D _MRAE;
		uniform float4 _MRAE_ST;
		uniform float _Metalic;
		uniform float _Roughness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 ase_worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
			half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
			float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * tangentSign;
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 objectToTangentDir61 = mul( ase_worldToTangent, mul( unity_ObjectToWorld, float4( ase_vertexNormal, 0 ) ).xyz);
			o.vertexToFrag62 = objectToTangentDir61;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			#ifdef _ISNORMAL_ON
				float3 staticSwitch58 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			#else
				float3 staticSwitch58 = i.vertexToFrag62;
			#endif
			o.Normal = staticSwitch58;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 temp_output_31_0 = ( tex2D( _Albedo, uv_Albedo ) * _Color );
			float4 switchResult52 = (((i.ASEVFace>0)?(temp_output_31_0):(( temp_output_31_0 * 0.7 ))));
			o.Albedo = switchResult52.rgb;
			float4 temp_cast_1 = (0.0).xxxx;
			float2 uv_MRAE = i.uv_texcoord * _MRAE_ST.xy + _MRAE_ST.zw;
			float4 saferPower41 = abs( tex2D( _MRAE, uv_MRAE ) );
			#ifdef _ISMRAE_ON
				float4 staticSwitch56 = saturate( pow( saferPower41 , 0.45 ) );
			#else
				float4 staticSwitch56 = float4(0,0,1,1);
			#endif
			float4 break43 = staticSwitch56;
			#ifdef _ISEMISSIVE_ON
				float4 staticSwitch33 = ( _EColor * break43.w );
			#else
				float4 staticSwitch33 = temp_cast_1;
			#endif
			o.Emission = staticSwitch33.rgb;
			o.Metallic = saturate( ( break43.x + _Metalic ) );
			o.Smoothness = ( 1.0 - saturate( ( break43.y + _Roughness ) ) );
			o.Occlusion = break43.z;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1274;23;1280;637;255.4174;359.8967;1.543837;True;False
Node;AmplifyShaderEditor.SamplerNode;21;-1456.483,-273.5691;Inherit;True;Property;_MRAE;MRAE;4;0;Create;True;0;0;0;False;0;False;-1;None;e73f0aa2346ee56439deb8811d4e1fa6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;41;-1056.318,-120.6211;Inherit;False;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.45;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;63;-955.9695,-197.2466;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;57;-1123.061,-393.2682;Inherit;False;Constant;_Vector0;Vector 0;10;0;Create;True;0;0;0;False;0;False;0,0,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;56;-871.5573,-357.7544;Inherit;False;Property;_isMRAE;isMRAE;3;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;18;-251.793,-363.0797;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;1,0.3544474,0.3544474,0;0.6509434,0.6509434,0.6509434,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;30;-170.0925,-585.269;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;0;False;0;False;-1;None;84b8e6162a53b95459c62138bc1430b2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-657.1017,427.3165;Inherit;False;Property;_Roughness;Roughness;5;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;43;-685.0132,-95.21561;Inherit;False;COLOR;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.NormalVertexDataNode;60;82.31779,353.8889;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-349.054,337.9617;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-780.1155,181.4457;Inherit;False;Property;_Metalic;Metalic;6;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;61;268.5577,285.205;Inherit;False;Object;Tangent;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;36;-375.6716,-166.7028;Inherit;False;Property;_EColor;EColor;10;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;285.1206,-213.9524;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;53;384.5384,-359.8366;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-20.06026,-182.2785;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;10.10856,-58.13164;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexToFragmentNode;62;466.6127,313.1176;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;48;409.1035,546.3427;Inherit;True;Property;_Normal;Normal;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;540.4073,-95.78464;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;24;-229.0494,327.6592;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-217.3945,62.85425;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;33;279.2767,-12.06483;Inherit;False;Property;_IsEmissive;IsEmissive;9;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;20;9.302736,226.6291;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;52;911.8527,-81.90244;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;58;831.5099,224.9967;Inherit;False;Property;_isNormal;isNormal;7;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;28;38.23729,60.4586;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-961.5894,467.1856;Inherit;False;Property;_CullMode;CullMode;0;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1373.353,64.92613;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Standard_Mine;False;False;False;False;False;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;DeferredOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;32;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;41;0;21;0
WireConnection;63;0;41;0
WireConnection;56;1;57;0
WireConnection;56;0;63;0
WireConnection;43;0;56;0
WireConnection;23;0;43;1
WireConnection;23;1;19;0
WireConnection;61;0;60;0
WireConnection;31;0;30;0
WireConnection;31;1;18;0
WireConnection;37;0;36;0
WireConnection;37;1;43;3
WireConnection;62;0;61;0
WireConnection;54;0;31;0
WireConnection;54;1;53;0
WireConnection;24;0;23;0
WireConnection;27;0;43;0
WireConnection;27;1;25;0
WireConnection;33;1;34;0
WireConnection;33;0;37;0
WireConnection;20;0;24;0
WireConnection;52;0;31;0
WireConnection;52;1;54;0
WireConnection;58;1;62;0
WireConnection;58;0;48;0
WireConnection;28;0;27;0
WireConnection;0;0;52;0
WireConnection;0;1;58;0
WireConnection;0;2;33;0
WireConnection;0;3;28;0
WireConnection;0;4;20;0
WireConnection;0;5;43;2
ASEEND*/
//CHKSM=200C81493F08502E9957CF3209BE330458CC2460