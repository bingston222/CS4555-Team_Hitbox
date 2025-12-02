// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Custom Normal UV Mask Switch"
{
	Properties
	{
		[NoScaleOffset]_BaseTexture("Base Texture", 2D) = "white" {}
		_BaseTexColorTint("Base Tex Color Tint", Color) = (1,1,1,0)
		[Toggle(_USECUSTOMMASKS_ON)] _UseCustomMasks("Use Custom Masks", Float) = 0
		[NoScaleOffset]_BaseTextureMasks("Base Texture Masks", 2D) = "white" {}
		_PrimaryColor("Primary Color", Color) = (0.2509804,0.2509804,0.2509804,1)
		_SecondaryColor("Secondary Color ", Color) = (0.8588236,0.8588236,0.8588236,1)
		_TertiaryColor("Tertiary Color ", Color) = (0.8745098,0.6220042,0,1)
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Roughness("Roughness", Range( 0 , 1)) = 0.1
		[NoScaleOffset]_NormalTexture("Normal Texture", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range( 0 , 5)) = 1
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 4.5
		#pragma shader_feature_local _USECUSTOMMASKS_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv3_texcoord3;
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalTexture;
		uniform float _NormalStrength;
		uniform float4 _BaseTexColorTint;
		uniform sampler2D _BaseTexture;
		uniform float4 _PrimaryColor;
		uniform sampler2D _BaseTextureMasks;
		uniform float4 _SecondaryColor;
		uniform float4 _TertiaryColor;
		uniform float _Metallic;
		uniform float _Roughness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv2_NormalTexture17 = i.uv3_texcoord3;
			o.Normal = UnpackScaleNormal( tex2D( _NormalTexture, uv2_NormalTexture17 ), _NormalStrength );
			float2 uv_BaseTexture1 = i.uv_texcoord;
			float4 color6 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float2 uv_BaseTextureMasks5 = i.uv_texcoord;
			float4 tex2DNode5 = tex2D( _BaseTextureMasks, uv_BaseTextureMasks5 );
			float4 lerpResult10 = lerp( color6 , _PrimaryColor , tex2DNode5.r);
			float4 lerpResult11 = lerp( lerpResult10 , _SecondaryColor , tex2DNode5.g);
			float4 lerpResult12 = lerp( lerpResult11 , _TertiaryColor , tex2DNode5.b);
			#ifdef _USECUSTOMMASKS_ON
				float4 staticSwitch3 = lerpResult12;
			#else
				float4 staticSwitch3 = ( _BaseTexColorTint * tex2D( _BaseTexture, uv_BaseTexture1 ) );
			#endif
			o.Albedo = staticSwitch3.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = ( 1.0 - _Roughness );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.ColorNode;6;-1123.181,-1044.942;Inherit;False;Constant;_Color0;Color 0;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;5;-1216.905,-871.1816;Inherit;True;Property;_BaseTextureMasks;Base Texture Masks;3;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;d5ef6b3b44c759040bb0af1076d536ab;d5ef6b3b44c759040bb0af1076d536ab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;7;-827.582,-1040.874;Inherit;False;Property;_PrimaryColor;Primary Color;4;0;Create;True;0;0;0;False;0;False;0.2509804,0.2509804,0.2509804,1;0.2509804,0.2509804,0.2509804,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;10;-543.5621,-873.7266;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;8;-826.5461,-631.7195;Inherit;False;Property;_SecondaryColor;Secondary Color ;5;0;Create;True;0;0;0;False;0;False;0.8588236,0.8588236,0.8588236,1;0.8588236,0.8588236,0.8588236,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;11;-319.249,-640.2368;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;4;-828.5,-190.5;Inherit;False;Property;_BaseTexColorTint;Base Tex Color Tint;1;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;1;-897.5,-3.5;Inherit;True;Property;_BaseTexture;Base Texture;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;cfc97df11b2ea344ba637d0760c121fa;cfc97df11b2ea344ba637d0760c121fa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;9;-826.2103,-432.5871;Inherit;False;Property;_TertiaryColor;Tertiary Color ;6;0;Create;True;0;0;0;False;0;False;0.8745098,0.6220042,0,1;0.8745099,0.3921569,0,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-550.5,14.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-502.7842,277.1432;Inherit;False;Property;_Roughness;Roughness;8;0;Create;True;0;0;0;False;0;False;0.1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;12;-161.2091,-453.6487;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-638.3687,625.6989;Inherit;False;Property;_NormalStrength;Normal Strength;10;0;Create;True;0;0;0;False;0;False;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;15;-219.7842,277.1432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-500.7842,192.1432;Inherit;False;Property;_Metallic;Metallic;7;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;3;-312.5,12.5;Inherit;False;Property;_UseCustomMasks;Use Custom Masks;2;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;17;-361.4623,452.8378;Inherit;True;Property;_NormalTexture;Normal Texture;9;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;40e98482595b46244a4895f883348db2;40e98482595b46244a4895f883348db2;True;2;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;5;;0;0;Standard;TidalFlask/Custom Normal UV Mask Switch;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;6;0
WireConnection;10;1;7;0
WireConnection;10;2;5;1
WireConnection;11;0;10;0
WireConnection;11;1;8;0
WireConnection;11;2;5;2
WireConnection;2;0;4;0
WireConnection;2;1;1;0
WireConnection;12;0;11;0
WireConnection;12;1;9;0
WireConnection;12;2;5;3
WireConnection;15;0;14;0
WireConnection;3;1;2;0
WireConnection;3;0;12;0
WireConnection;17;5;18;0
WireConnection;0;0;3;0
WireConnection;0;1;17;0
WireConnection;0;3;13;0
WireConnection;0;4;15;0
ASEEND*/
//CHKSM=6CC30619B6BE678B8D88538999806F87050460B7