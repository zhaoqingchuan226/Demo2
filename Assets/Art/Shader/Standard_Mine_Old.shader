// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Standard_Mine_Old"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		[Enum(UnityEngine.Rendering.BlendMode)]_Src("Src", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
		_DustColor("DustColor", Color) = (0.2169811,0.2169811,0.2169811,0)
		_Dusk1("Dusk1", 2D) = "white" {}
		_Dusk2("Dusk2", 2D) = "white" {}
		_DirtTiling("DirtTiling", Vector) = (1,1,1,1)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_CullMode]
		ZWrite Off
		Stencil
		{
			Ref 3
			Comp Less
			Pass Keep
		}
		Blend [_Src] [_Dst]
		
		CGPROGRAM
		#include "CGinclude/Gubuffer.cginc"
		#pragma target 3.0
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Unlit keepalpha noshadow exclude_path:deferred novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _Src;
		uniform float _Dst;
		uniform float _CullMode;
		uniform float4 _DustColor;
		uniform sampler2D _Dusk1;
		uniform float4 _DirtTiling;
		uniform sampler2D _Dusk2;


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


		inline float4 TriplanarSampling119( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline float4 TriplanarSampling122( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 temp_output_73_0 = (ase_grabScreenPosNorm).xy;
			float2 uv72 = temp_output_73_0;
			float4 localGbuffer072 = Gbuffer0( uv72 );
			float AO109 = (localGbuffer072).w;
			o.Emission = ( _DustColor * AO109 ).rgb;
			float temp_output_86_0 = ( 1.0 - pow( AO109 , 2.3 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 triplanar119 = TriplanarSampling119( _Dusk1, ( ase_worldPos * _DirtTiling.x ), ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float4 triplanar122 = TriplanarSampling122( _Dusk2, ( ase_worldPos * _DirtTiling.y ), ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float DirtAll136 = max( triplanar119.x , triplanar122.x );
			o.Alpha = ( _DustColor.a * saturate( ( saturate( ( temp_output_86_0 + ( temp_output_86_0 * DirtAll136 ) ) ) + ( 0.5 * DirtAll136 ) ) ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1274;50;1280;609.5;-1124.124;247.44;1.725525;True;False
Node;AmplifyShaderEditor.CommentaryNode;114;-711.6708,-271.1365;Inherit;False;1332.378;428.1031;Gbuffer;8;68;73;72;90;79;96;109;111;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GrabScreenPosition;68;-661.6707,-104.4045;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwizzleNode;73;-292.9876,-115.398;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;126;1734.926,1172.968;Inherit;False;Property;_DirtTiling;DirtTiling;9;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.5,0.5,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;72;-58.50823,-190.2929;Inherit;False; ;4;File;1;True;uv;FLOAT2;0,0;In;;Inherit;False;Gbuffer0;False;False;0;9681673c463cdfe499599c097037a0f3;False;1;0;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldPosInputsNode;127;1691.858,997.1628;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;1953.326,1037.768;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;120;1504.066,1217.874;Inherit;True;Property;_Dusk1;Dusk1;7;0;Create;True;0;0;0;False;0;False;None;0e362d8b8bd6cec43a6c70a1e78dcf88;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;1978.338,1428.765;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;121;1590.139,1364.395;Inherit;True;Property;_Dusk2;Dusk2;8;0;Create;True;0;0;0;False;0;False;None;dec730de9fbe8924fbb7137a029190e8;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SwizzleNode;79;155.278,-221.1365;Inherit;False;FLOAT;3;1;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;122;2095.739,1234.339;Inherit;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;0;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;109;362.8439,-211.8915;Inherit;False;AO;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;119;2021.431,917.8064;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;110;926.7442,571.5507;Inherit;False;109;AO;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;105;2065.875,699.95;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;113;1135.526,613.743;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;136;2206.434,717.9255;Inherit;False;DirtAll;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;86;1286.77,603.2538;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;1264.855,768.3905;Inherit;False;136;DirtAll;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;1433.666,660.5405;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;141;1577.855,589.3906;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;145;1569.589,710.8649;Inherit;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;1570.984,799.9796;Inherit;False;136;DirtAll;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;1792.107,645.6912;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;87;1612.568,521.7842;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;135;1879.109,536.7885;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;64;1273.92,170.7551;Inherit;False;Property;_DustColor;DustColor;3;0;Create;True;0;0;0;False;0;False;0.2169811,0.2169811,0.2169811,0;1,0.4481131,0.4481131,0.1568628;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;134;1992.666,539.0916;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;1846.709,241.0457;Inherit;False;109;AO;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;95;1223.711,1142.385;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;143;1436.459,1095.043;Inherit;False;Constant;_Float1;Float 1;10;0;Create;True;0;0;0;False;0;False;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;93;937.2466,1163.48;Inherit;False;Constant;_Vector1;Vector 1;17;0;Create;True;0;0;0;False;0;False;0.4,1,0.4;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;66;875.5651,69.75999;Inherit;False;Property;_DustMetalic;DustMetalic;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-799.7986,791.8373;Inherit;False;Property;_Src;Src;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;2077.708,297.6754;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;111;397.2788,10.28979;Inherit;False;worldNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;142;1532.724,904.2383;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;2100.497,124.1273;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;96;185.754,1.823589;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;90;-57.04885,-9.952532;Inherit;False; ;3;File;1;True;uv;FLOAT2;0,0;In;;Inherit;False;Gbuffer2;False;False;0;9681673c463cdfe499599c097037a0f3;False;1;0;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;2131.271,468.3326;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-797.7986,913.8373;Inherit;False;Property;_Dst;Dst;2;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;869.9638,187.2206;Inherit;False;Property;_DustRough;DustRough;6;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;112;1026.979,879.2509;Inherit;False;111;worldNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-961.5894,467.1856;Inherit;False;Property;_CullMode;CullMode;0;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;144;1372.869,962.2375;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;91;1256.612,915.2968;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2661.724,149.301;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Standard_Mine_Old;False;False;False;False;False;True;True;True;True;False;True;True;False;False;True;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;ForwardOnly;5;d3d9;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;True;3;False;-1;255;False;-1;255;False;-1;3;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;1;1;True;81;0;True;80;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;4;-1;-1;-1;0;False;0;0;True;32;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;73;0;68;0
WireConnection;72;0;73;0
WireConnection;128;0;127;0
WireConnection;128;1;126;1
WireConnection;129;0;127;0
WireConnection;129;1;126;2
WireConnection;79;0;72;0
WireConnection;122;0;121;0
WireConnection;122;9;129;0
WireConnection;109;0;79;0
WireConnection;119;0;120;0
WireConnection;119;9;128;0
WireConnection;105;0;119;1
WireConnection;105;1;122;1
WireConnection;113;0;110;0
WireConnection;136;0;105;0
WireConnection;86;0;113;0
WireConnection;139;0;86;0
WireConnection;139;1;140;0
WireConnection;141;0;86;0
WireConnection;141;1;139;0
WireConnection;131;0;145;0
WireConnection;131;1;138;0
WireConnection;87;0;141;0
WireConnection;135;0;87;0
WireConnection;135;1;131;0
WireConnection;134;0;135;0
WireConnection;95;0;93;0
WireConnection;111;0;96;0
WireConnection;142;0;144;0
WireConnection;142;1;143;0
WireConnection;118;0;64;0
WireConnection;118;1;115;0
WireConnection;96;0;90;0
WireConnection;90;0;73;0
WireConnection;99;0;64;4
WireConnection;99;1;134;0
WireConnection;144;0;91;0
WireConnection;91;0;112;0
WireConnection;91;1;95;0
WireConnection;0;2;118;0
WireConnection;0;9;99;0
ASEEND*/
//CHKSM=D3B866B552C59821142917254E40E2AADBBC4EF0