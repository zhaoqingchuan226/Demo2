// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CrossShinning_Particles"
{
	Properties
	{
		_Particle_Glow16("Particle_Glow16", 2D) = "white" {}
		[HDR]_Color("Color", Color) = (1,1,1,0)
		_OffsetTiling("OffsetTiling", Range( -2.9 , 1.02)) = -2
		_Circle_Radius("Circle_Radius", Float) = 0.16
		_Circle_Range("Circle_Range", Float) = 3.56
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma only_renderers d3d11_9x d3d11 glcore gles3 
		#pragma surface surf Unlit keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float4 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _Particle_Glow16;
		uniform float _OffsetTiling;
		uniform float _Circle_Radius;
		uniform float _Circle_Range;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = ( i.vertexColor * _Color ).rgb;
			float2 temp_cast_1 = (0.5).xx;
			float clampResult23 = clamp( ( _OffsetTiling + i.uv_texcoord.z ) , -2.9 , 1.02 );
			float2 temp_cast_2 = (0.5).xx;
			o.Alpha = ( _Color.a * ( i.vertexColor.a * tex2D( _Particle_Glow16, ( i.uv_texcoord.xy + ( ( i.uv_texcoord.xy - temp_cast_1 ) * clampResult23 * saturate( ( ( ( 1.0 - distance( i.uv_texcoord.xy , temp_cast_2 ) ) - _Circle_Radius ) * _Circle_Range ) ) ) ) ).r ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1274;6.5;1280;653.5;451.4933;325.0933;1.6;True;False
Node;AmplifyShaderEditor.CommentaryNode;18;-3040.723,808.267;Inherit;False;1911.516;806.3792;Mask;9;16;14;13;11;10;9;17;12;15;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-2982.194,1138.464;Inherit;False;Constant;_Float1;Float 1;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2990.723,858.267;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode;9;-2515.878,982.2379;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;12;-2132.744,1099.463;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1987.615,1288.45;Inherit;False;Property;_Circle_Radius;Circle_Radius;4;0;Create;True;0;0;0;False;0;False;0.16;0.53;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1254.262,539.3749;Inherit;False;Property;_OffsetTiling;OffsetTiling;3;0;Create;True;0;0;0;False;0;False;-2;-0.04;-2.9;1.02;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-1191.099,641.6533;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;13;-1799.201,1106.905;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1790.391,1356.246;Inherit;False;Property;_Circle_Range;Circle_Range;5;0;Create;True;0;0;0;False;0;False;3.56;8.49;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1637.562,160.2755;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-1416.523,390.6894;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1588.291,1256.646;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-921.1908,540.3621;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;4;-1241.626,324.4396;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;15;-1327.207,1165.137;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;23;-738.5564,521.7906;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-2.9;False;2;FLOAT;1.02;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-517.6861,423.413;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-468.6682,307.0688;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-137.4822,346.9082;Inherit;True;Property;_Particle_Glow16;Particle_Glow16;1;0;Create;True;0;0;0;False;0;False;-1;None;cd1dea05e1e221b408f7bb76a4595522;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;19;-252.3345,-218.9576;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;382.915,75.02374;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-173.3218,50.04907;Inherit;False;Property;_Color;Color;2;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;0,12.04524,22.89442,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;551.5031,-33.50302;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;79.66547,-105.9627;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1014.448,-181.3202;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;CrossShinning_Particles;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Back;2;False;-1;7;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;ForwardOnly;4;d3d11_9x;d3d11;glcore;gles3;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;11;0
WireConnection;9;1;10;0
WireConnection;12;0;9;0
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;16;0;13;0
WireConnection;16;1;17;0
WireConnection;22;0;7;0
WireConnection;22;1;21;3
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;15;0;16;0
WireConnection;23;0;22;0
WireConnection;6;0;4;0
WireConnection;6;1;23;0
WireConnection;6;2;15;0
WireConnection;8;0;3;0
WireConnection;8;1;6;0
WireConnection;1;1;8;0
WireConnection;24;0;19;4
WireConnection;24;1;1;1
WireConnection;25;0;2;4
WireConnection;25;1;24;0
WireConnection;20;0;19;0
WireConnection;20;1;2;0
WireConnection;0;2;20;0
WireConnection;0;9;25;0
ASEEND*/
//CHKSM=27FEFB99C8D79798EDC8E491BD382A803F3F9373