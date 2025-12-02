// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Custom Normal UV RGB Patterns "
{
	Properties
	{
		[NoScaleOffset]_BaseTexture("Base Texture", 2D) = "white" {}
		[NoScaleOffset]_EmissionTexture("Emission Texture", 2D) = "white" {}
		_PrimaryColor("Primary Color", Color) = (0.8000001,0.8000001,0.8000001,1)
		_SecondaryColor("Secondary Color", Color) = (0.2745098,0.2745098,0.2745098,1)
		[HDR]_EmissionColor("Emission Color", Color) = (0.9843138,0,1,1)
		[Toggle(_CUSTOMENDING_ON)] _CustomEnding("Custom Ending", Float) = 0
		[Toggle(_LRSWITCH_ON)] _LRSwitch("L/R Switch", Float) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_RoughnessMin("Roughness Min", Range( 0 , 1)) = 0.1
		_RoughnessMax("Roughness Max", Range( 0 , 1)) = 0.33
		[NoScaleOffset]_NormalTexture("Normal Texture", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range( 0 , 5)) = 1
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 4.5
		#pragma shader_feature_local _CUSTOMENDING_ON
		#pragma shader_feature_local _LRSWITCH_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv3_texcoord3;
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalTexture;
		uniform float _NormalStrength;
		uniform float4 _PrimaryColor;
		uniform float4 _SecondaryColor;
		uniform sampler2D _BaseTexture;
		uniform float4 _EmissionColor;
		uniform sampler2D _EmissionTexture;
		uniform float _Metallic;
		uniform float _RoughnessMin;
		uniform float _RoughnessMax;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv2_NormalTexture19 = i.uv3_texcoord3;
			o.Normal = UnpackScaleNormal( tex2D( _NormalTexture, uv2_NormalTexture19 ), _NormalStrength );
			float2 uv_BaseTexture1 = i.uv_texcoord;
			float4 tex2DNode1 = tex2D( _BaseTexture, uv_BaseTexture1 );
			#ifdef _LRSWITCH_ON
				float staticSwitch7 = tex2DNode1.r;
			#else
				float staticSwitch7 = tex2DNode1.b;
			#endif
			#ifdef _CUSTOMENDING_ON
				float staticSwitch8 = staticSwitch7;
			#else
				float staticSwitch8 = tex2DNode1.g;
			#endif
			float4 lerpResult2 = lerp( _PrimaryColor , _SecondaryColor , staticSwitch8);
			o.Albedo = lerpResult2.rgb;
			float2 uv_EmissionTexture15 = i.uv_texcoord;
			o.Emission = ( _EmissionColor * tex2D( _EmissionTexture, uv_EmissionTexture15 ) ).rgb;
			o.Metallic = _Metallic;
			float clampResult11 = clamp( staticSwitch8 , _RoughnessMin , _RoughnessMax );
			o.Smoothness = ( 1.0 - clampResult11 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.SamplerNode;1;-1483.502,-13.9;Inherit;True;Property;_BaseTexture;Base Texture;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;3fde9aef500050b4395a1dc792c92c8e;3fde9aef500050b4395a1dc792c92c8e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StaticSwitch;7;-1153.501,74.1;Inherit;False;Property;_LRSwitch;L/R Switch;6;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-757.0369,193.1632;Inherit;False;Property;_RoughnessMin;Roughness Min;8;0;Create;True;0;0;0;False;0;False;0.1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-760.0931,273.1632;Inherit;False;Property;_RoughnessMax;Roughness Max;9;0;Create;True;0;0;0;False;0;False;0.33;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;8;-892.4995,-11.9;Inherit;False;Property;_CustomEnding;Custom Ending;5;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-861.5228,-391.9466;Inherit;False;Property;_PrimaryColor;Primary Color;2;0;Create;True;0;0;0;False;0;False;0.8000001,0.8000001,0.8000001,1;0.8000001,0.8000001,0.8000001,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;6;-859.5228,-210.9467;Inherit;False;Property;_SecondaryColor;Secondary Color;3;0;Create;True;0;0;0;False;0;False;0.2745098,0.2745098,0.2745098,1;0.2745098,0.2745098,0.2745098,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ClampOpNode;11;-389.0363,225.1632;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;15;-565.9478,642.3609;Inherit;True;Property;_EmissionTexture;Emission Texture;1;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;2b3be55f840d16545b5639f9f7b83aa8;2b3be55f840d16545b5639f9f7b83aa8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;17;-496.5479,454.5609;Inherit;False;Property;_EmissionColor;Emission Color;4;1;[HDR];Create;True;0;0;0;False;0;False;0.9843138,0,1,1;0.9843138,0,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;22;-871.1685,980.2307;Inherit;False;Property;_NormalStrength;Normal Strength;11;0;Create;True;0;0;0;False;0;False;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2;-556.4995,-8.9;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;14;-229.0363,225.1632;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-228.1479,452.1612;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-362.7999,136.5;Inherit;False;Property;_Metallic;Metallic;7;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-570.2068,930.6034;Inherit;True;Property;_NormalTexture;Normal Texture;10;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;d34cc2d4be183ce4f993656836a1625f;d34cc2d4be183ce4f993656836a1625f;True;2;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1,-12;Float;False;True;-1;5;;0;0;Standard;TidalFlask/Custom Normal UV RGB Patterns ;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;1;1;3
WireConnection;7;0;1;1
WireConnection;8;1;1;2
WireConnection;8;0;7;0
WireConnection;11;0;8;0
WireConnection;11;1;12;0
WireConnection;11;2;13;0
WireConnection;2;0;5;0
WireConnection;2;1;6;0
WireConnection;2;2;8;0
WireConnection;14;0;11;0
WireConnection;16;0;17;0
WireConnection;16;1;15;0
WireConnection;19;5;22;0
WireConnection;0;0;2;0
WireConnection;0;1;19;0
WireConnection;0;2;16;0
WireConnection;0;3;9;0
WireConnection;0;4;14;0
ASEEND*/
//CHKSM=5866D33CD3706031F4A200646C0CC8673360BA65