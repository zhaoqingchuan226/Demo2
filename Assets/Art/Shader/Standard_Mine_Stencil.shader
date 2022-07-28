// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_Mine_Stencil"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		_MainTex("MainTex", 2D) = "white" {}
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_FlowTex("FlowTex", 2D) = "white" {}
		_FlowDir("FlowDir", Vector) = (0,0,0,0)
		_FlowSpeed("FlowSpeed", Float) = 0.2
		_FlowIntensity("FlowIntensity", Float) = 1
		_Size("Size", Range( 0 , 10)) = 1
		_CollisionEdgeFactor("CollisionEdgeFactor", Vector) = (0,1,1,0)
		[HDR]_FresnelColor("FresnelColor", Color) = (0,0,0,0)
		_Diss("Diss", 2D) = "white" {}
		[Toggle]_isColor("_isColor", Float) = 1
		_Fresnel("Fresnel", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		ZWrite Off
		Stencil
		{
			Ref 3
			Comp Always
			Pass Replace
			Fail Replace
		}
		Blend SrcAlpha One
		
		ColorMask RGB
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.5
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Unlit keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 vertexToFrag184;
			INTERNAL_DATA
			float4 screenPos;
			half ASEVFace : VFACE;
		};

		uniform float _CullMode;
		uniform float _isColor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _FlowTex;
		uniform float4 _FlowTex_ST;
		uniform float _Size;
		uniform float2 _FlowDir;
		uniform float _FlowSpeed;
		uniform float _FlowIntensity;
		uniform float4 _Color;
		uniform float3 _Fresnel;
		uniform float4 _FresnelColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform sampler2D _Diss;
		uniform float4 _Diss_ST;
		uniform float3 _CollisionEdgeFactor;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 objToView191 = mul( UNITY_MATRIX_MV, float4( ase_vertex3Pos, 1 ) ).xyz;
			float clampResult192 = clamp( objToView191.z , -1000000.0 , -0.0201 );
			float3 appendResult196 = (float3(objToView191.x , objToView191.y , clampResult192));
			float3 viewToObj193 = mul( unity_WorldToObject, mul( UNITY_MATRIX_I_V , float4( appendResult196, 1 ) ) ).xyz;
			v.vertex.xyz += ( viewToObj193 - ase_vertex3Pos );
			v.vertex.w = 1;
			float3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 ase_worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
			half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
			float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * tangentSign;
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 objectToTangentDir182 = mul( ase_worldToTangent, mul( unity_ObjectToWorld, float4( ase_vertexNormal, 0 ) ).xyz);
			float3 break178 = objectToTangentDir182;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_tanViewDir = mul( ase_worldToTangent, ase_worldViewDir );
			float ase_faceVertex = (dot(ase_tanViewDir,float3(0,0,1)));
			float switchResult174 = (((ase_faceVertex>0)?(break178.z):(( break178.z * -1.0 ))));
			float3 appendResult175 = (float3(break178.x , break178.y , switchResult174));
			float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);
			float3 tangentToWorldDir183 = normalize( mul( ase_tangentToWorldFast, appendResult175 ) );
			o.vertexToFrag184 = tangentToWorldDir183;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 uv_FlowTex = i.uv_texcoord * _FlowTex_ST.xy + _FlowTex_ST.zw;
			float2 temp_output_4_0_g1 = (( uv_FlowTex / _Size )).xy;
			float2 temp_output_17_0_g1 = float2( 1,1 );
			float mulTime22_g1 = _Time.y * _FlowSpeed;
			float temp_output_27_0_g1 = frac( mulTime22_g1 );
			float2 temp_output_11_0_g1 = ( temp_output_4_0_g1 + ( -(_FlowDir*2.0 + -1.0) * temp_output_17_0_g1 * temp_output_27_0_g1 ) );
			float2 temp_output_12_0_g1 = ( temp_output_4_0_g1 + ( -(_FlowDir*2.0 + -1.0) * temp_output_17_0_g1 * frac( ( mulTime22_g1 + 0.5 ) ) ) );
			float4 lerpResult9_g1 = lerp( tex2D( _FlowTex, temp_output_11_0_g1 ) , tex2D( _FlowTex, temp_output_12_0_g1 ) , ( abs( ( temp_output_27_0_g1 - 0.5 ) ) / 0.5 ));
			float2 flow138 = ( (lerpResult9_g1).rg * _FlowIntensity );
			float4 tex2DNode127 = tex2D( _MainTex, ( uv_MainTex + flow138 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 worldView82 = ase_worldViewDir;
			float3 worldNormal84 = i.vertexToFrag184;
			float fresnelNdotV116 = dot( worldNormal84, worldView82 );
			float fresnelNode116 = ( _Fresnel.x + _Fresnel.y * pow( 1.0 - fresnelNdotV116, _Fresnel.z ) );
			float Fresnel122 = saturate( fresnelNode116 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float eyeDepth110 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float3 worldToView103 = mul( UNITY_MATRIX_V, float4( ase_worldPos, 1 ) ).xyz;
			float2 uv_Diss = i.uv_texcoord * _Diss_ST.xy + _Diss_ST.zw;
			float DeltaDepth95 = saturate( ( pow( ( saturate( ( ( 1.0 - ( eyeDepth110 + worldToView103.z ) ) - tex2D( _Diss, ( uv_Diss + ( flow138 * 0.3 ) ) ).r ) ) + _CollisionEdgeFactor.x ) , _CollisionEdgeFactor.z ) * _CollisionEdgeFactor.y ) );
			float4 temp_output_147_0 = ( ( tex2DNode127 * _Color * Fresnel122 ) + ( Fresnel122 * _FresnelColor ) + ( ( DeltaDepth95 * 0.35 ) * _Color ) );
			float4 switchResult186 = (((i.ASEVFace>0)?(temp_output_147_0):(( temp_output_147_0 * 0.7 ))));
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 objToView191 = mul( UNITY_MATRIX_MV, float4( ase_vertex3Pos, 1 ) ).xyz;
			float clampResult192 = clamp( objToView191.z , -1000000.0 , -0.0201 );
			o.Emission = (( _isColor )?( ( switchResult186 * saturate( max( ( ( -0.02 - clampResult192 ) - 0.0 ) , 0.0 ) ) ) ):( float4( float3(0,0,0) , 0.0 ) )).rgb;
			float luminance145 = Luminance(tex2DNode127.rgb);
			float mainlumiance146 = luminance145;
			o.Alpha = saturate( ( mainlumiance146 + Fresnel122 + DeltaDepth95 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1280;1;1280;659;225.7622;-1255.767;1.3;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;129;407.7646,1690.894;Inherit;True;Property;_FlowTex;FlowTex;4;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;133;441.9274,2124.727;Inherit;False;Property;_FlowSpeed;FlowSpeed;6;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;197;412.4758,1884.854;Inherit;False;0;129;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;132;409.9327,2004.849;Inherit;False;Property;_FlowDir;FlowDir;5;0;Create;True;0;0;0;False;0;False;0,0;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;128;746.3851,1846.871;Inherit;False;Flow;8;;1;acad10cc8145e1f4eb8042bebe2d9a42;2,50,0,51,0;6;5;SAMPLER2D;;False;2;FLOAT2;0,0;False;55;FLOAT;1;False;18;FLOAT2;0,0;False;17;FLOAT2;1,1;False;24;FLOAT;0.2;False;1;COLOR;0
Node;AmplifyShaderEditor.SwizzleNode;137;1092.029,1958.353;Inherit;False;FLOAT2;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;144;905.5891,2262.172;Inherit;False;Property;_FlowIntensity;FlowIntensity;7;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;1131.71,2145.72;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;1285.735,2141.061;Inherit;False;flow;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalVertexDataNode;181;-595.8729,32.48935;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;167;50.26471,771.5585;Inherit;False;138;flow;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TransformDirectionNode;182;-362.5455,156.8829;Inherit;False;Object;Tangent;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;102;-203.201,833.2661;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;173;172.876,1024.128;Inherit;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;172;329.876,916.1276;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TransformPositionNode;103;65.83231,845.8868;Inherit;False;World;View;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScreenDepthNode;110;-135.1409,594.3972;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;178;-172.1963,283.4751;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TextureCoordinatesNode;170;134.42,641.7466;Inherit;False;0;166;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;94;250.2406,449.6327;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;171;392.42,799.7466;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;180;-61.99622,433.0748;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;174;52.70377,322.2756;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;96;472.7733,510.8728;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;166;500.0778,660.713;Inherit;True;Property;_Diss;Diss;13;0;Create;True;0;0;0;False;0;False;-1;None;bcbcff00b39077841accefc54d8ac40f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;175;264.7038,231.375;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;160;714.8058,504.2967;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;165;844.0482,624.89;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;112;420.5642,955.783;Inherit;False;Property;_CollisionEdgeFactor;CollisionEdgeFactor;11;0;Create;True;0;0;0;False;0;False;0,1,1;-0.17,4,4.37;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformDirectionNode;183;377.9452,60.17098;Inherit;False;Tangent;World;True;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;113;898.8982,741.4159;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;81;140.4276,-243.4145;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexToFragmentNode;184;630.1774,210.2019;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;781.8312,51.90765;Inherit;False;worldNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;114;844.4535,906.5244;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;82;550.5012,-157.237;Inherit;False;worldView;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;988.5914,952.388;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;119;376.396,1456.917;Inherit;False;Property;_Fresnel;Fresnel;15;0;Create;True;0;0;0;False;0;False;0,0,0;0,1.39,1.07;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;117;340.2846,1274.528;Inherit;False;84;worldNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;341.2846,1367.528;Inherit;False;82;worldView;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;116;614.5455,1292.986;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;157;1174.635,912.0366;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;95;1308.158,740.8027;Inherit;False;DeltaDepth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;1417.166,2040.018;Inherit;False;138;flow;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;121;882.1172,1379.463;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;189;2821.74,1593.837;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;134;1340.853,1828.722;Inherit;False;0;127;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TransformPositionNode;191;3064.01,1614.156;Inherit;False;Object;View;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;201;1818.977,1313.49;Inherit;False;Constant;_Float3;Float 3;13;0;Create;True;0;0;0;False;0;False;0.35;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;97;1740.901,1161.738;Inherit;False;95;DeltaDepth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;139;1672.846,1990.833;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;122;1054.117,1365.463;Inherit;False;Fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;200;1988.977,1213.49;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;127;1831.797,1824.794;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;0;False;0;False;-1;None;f56c69edff2862d418a1d4dd9a8bbf75;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;216;3025.843,1896.369;Inherit;False;Constant;_Float4;Float 4;14;0;Create;True;0;0;0;False;0;False;-0.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;2028.504,2255.92;Inherit;False;122;Fresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;192;3202.664,1759.823;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-1000000;False;2;FLOAT;-0.0201;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;141;1850.57,2123.656;Inherit;False;Property;_Color;Color;3;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;5.151806,3.278422,11.18176,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;149;2163.826,2421.456;Inherit;False;Property;_FresnelColor;FresnelColor;12;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0.2007768,0.1727127,0.2924528,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;2210.429,2046.088;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;2437.163,2289.251;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;156;2137.774,1268.967;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;225;3112.116,2021.326;Inherit;False;Constant;_Float6;Float 6;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;226;3347.359,1854.609;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;224;3437.84,1949.476;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LuminanceNode;145;2276.1,1874.943;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;219;3135.683,2154.319;Inherit;False;Constant;_Float5;Float 5;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;188;2523.985,1502.677;Inherit;False;Constant;_Float2;Float 2;13;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;147;2551.59,1346.409;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;2830.52,1413.309;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;146;2597.585,1907.712;Inherit;False;mainlumiance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;218;3530.895,2037.026;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;220;3622.415,1956.734;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;151;1983.146,1531.132;Inherit;False;146;mainlumiance;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;196;3432.885,1657.494;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwitchByFaceNode;186;3101.444,1292.932;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;1987.271,1665.192;Inherit;False;122;Fresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;199;2051.142,1728.231;Inherit;False;95;DeltaDepth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;153;2266.271,1569.192;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;3513.735,1431.698;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TransformPositionNode;193;3564.988,1676.723;Inherit;False;View;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;214;3309.247,1089.78;Inherit;False;Constant;_Vector0;Vector 0;2;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;195;3826.889,1487.173;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;83;-186.9625,-133.596;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;162;2619.518,1768.068;Inherit;False;Constant;_Float1;Float 1;13;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;194;3748.485,1915.696;Inherit;False;False;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;154;2525.475,1620.642;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-961.5894,467.1856;Inherit;False;Property;_CullMode;CullMode;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;215;3780.98,1101.512;Inherit;True;Property;_isColor;_isColor;14;0;Create;True;0;0;0;False;0;False;1;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4133.482,1162.162;Float;False;True;-1;3;ASEMaterialInspector;0;0;Unlit;Standard_Mine_Stencil;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;ForwardOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;False;0;False;-1;True;3;False;-1;255;False;-1;255;False;-1;7;False;-1;3;False;-1;3;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;8;5;False;-1;1;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;True;32;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;128;5;129;0
WireConnection;128;2;197;0
WireConnection;128;18;132;0
WireConnection;128;24;133;0
WireConnection;137;0;128;0
WireConnection;143;0;137;0
WireConnection;143;1;144;0
WireConnection;138;0;143;0
WireConnection;182;0;181;0
WireConnection;172;0;167;0
WireConnection;172;1;173;0
WireConnection;103;0;102;0
WireConnection;178;0;182;0
WireConnection;94;0;110;0
WireConnection;94;1;103;3
WireConnection;171;0;170;0
WireConnection;171;1;172;0
WireConnection;180;0;178;2
WireConnection;174;0;178;2
WireConnection;174;1;180;0
WireConnection;96;0;94;0
WireConnection;166;1;171;0
WireConnection;175;0;178;0
WireConnection;175;1;178;1
WireConnection;175;2;174;0
WireConnection;160;0;96;0
WireConnection;160;1;166;1
WireConnection;165;0;160;0
WireConnection;183;0;175;0
WireConnection;113;0;165;0
WireConnection;113;1;112;1
WireConnection;184;0;183;0
WireConnection;84;0;184;0
WireConnection;114;0;113;0
WireConnection;114;1;112;3
WireConnection;82;0;81;0
WireConnection;115;0;114;0
WireConnection;115;1;112;2
WireConnection;116;0;117;0
WireConnection;116;4;118;0
WireConnection;116;1;119;1
WireConnection;116;2;119;2
WireConnection;116;3;119;3
WireConnection;157;0;115;0
WireConnection;95;0;157;0
WireConnection;121;0;116;0
WireConnection;191;0;189;0
WireConnection;139;0;134;0
WireConnection;139;1;140;0
WireConnection;122;0;121;0
WireConnection;200;0;97;0
WireConnection;200;1;201;0
WireConnection;127;1;139;0
WireConnection;192;0;191;3
WireConnection;142;0;127;0
WireConnection;142;1;141;0
WireConnection;142;2;148;0
WireConnection;150;0;148;0
WireConnection;150;1;149;0
WireConnection;156;0;200;0
WireConnection;156;1;141;0
WireConnection;226;0;216;0
WireConnection;226;1;192;0
WireConnection;224;0;226;0
WireConnection;224;1;225;0
WireConnection;145;0;127;0
WireConnection;147;0;142;0
WireConnection;147;1;150;0
WireConnection;147;2;156;0
WireConnection;187;0;147;0
WireConnection;187;1;188;0
WireConnection;146;0;145;0
WireConnection;218;0;224;0
WireConnection;218;1;219;0
WireConnection;220;0;218;0
WireConnection;196;0;191;1
WireConnection;196;1;191;2
WireConnection;196;2;192;0
WireConnection;186;0;147;0
WireConnection;186;1;187;0
WireConnection;153;0;151;0
WireConnection;153;1;152;0
WireConnection;153;2;199;0
WireConnection;221;0;186;0
WireConnection;221;1;220;0
WireConnection;193;0;196;0
WireConnection;195;0;193;0
WireConnection;195;1;189;0
WireConnection;154;0;153;0
WireConnection;215;0;214;0
WireConnection;215;1;221;0
WireConnection;0;2;215;0
WireConnection;0;9;154;0
WireConnection;0;11;195;0
ASEEND*/
//CHKSM=B345FA8A8E38C2DF67360C5551DED60F84D4E885