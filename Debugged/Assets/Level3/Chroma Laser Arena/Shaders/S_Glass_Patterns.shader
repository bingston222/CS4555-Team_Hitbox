// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Glass Patterns"
{
	Properties
	{
		[NoScaleOffset]_BaseTexture("Base Texture", 2D) = "white" {}
		_Color("Base Texture Color Tint", Color) = (0.09019608,0.572549,0.5490196,0.5019608)
		[HDR]_EmissionColorTint("Emission Color Tint", Color) = (0.09019608,0.572549,0.5490196,1)
		[Toggle(_CUSTOMENDING_ON)] _CustomEnding("Custom Ending", Float) = 0
		[Toggle(_LRSWITCH_ON)] _LRSwitch("L/R Switch", Float) = 0
		_EmissionPatternIntensity("Emission Pattern Intensity", Range( 0 , 1)) = 0.008
		_EmissionStrength("Emission Strength", Range( 0 , 0.1)) = 0.005
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Roughness("Roughness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.5
		#pragma shader_feature_local _CUSTOMENDING_ON
		#pragma shader_feature_local _LRSWITCH_ON
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _BaseTexture;
		uniform float _EmissionPatternIntensity;
		uniform float4 _EmissionColorTint;
		uniform float _EmissionStrength;
		uniform float _Metallic;
		uniform float _Roughness;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BaseTexture2 = i.uv_texcoord;
			float4 tex2DNode2 = tex2D( _BaseTexture, uv_BaseTexture2 );
			#ifdef _LRSWITCH_ON
				float staticSwitch3 = tex2DNode2.r;
			#else
				float staticSwitch3 = tex2DNode2.b;
			#endif
			#ifdef _CUSTOMENDING_ON
				float staticSwitch1 = staticSwitch3;
			#else
				float staticSwitch1 = tex2DNode2.g;
			#endif
			float4 temp_output_5_0 = ( _Color * staticSwitch1 );
			o.Albedo = temp_output_5_0.rgb;
			float4 color10 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 lerpResult7 = lerp( color10 , temp_output_5_0 , _EmissionPatternIntensity);
			float4 lerpResult9 = lerp( color10 , _EmissionColorTint , _EmissionStrength);
			o.Emission = ( lerpResult7 + lerpResult9 ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = ( 1.0 - _Roughness );
			o.Alpha = _Opacity;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.SamplerNode;2;-1293.501,5.500002;Inherit;True;Property;_BaseTexture;Base Texture;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;3fde9aef500050b4395a1dc792c92c8e;3fde9aef500050b4395a1dc792c92c8e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StaticSwitch;3;-963.5,93.5;Inherit;False;Property;_LRSwitch;L/R Switch;4;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1;-702.4985,7.500002;Inherit;False;Property;_CustomEnding;Custom Ending;3;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-702.0353,-174.1593;Inherit;False;Property;_Color;Base Texture Color Tint;1;0;Create;False;0;0;0;False;0;False;0.09019608,0.572549,0.5490196,0.5019608;0.09019593,0.572549,0.5490196,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;12;-713.3709,773.8618;Inherit;False;Property;_EmissionStrength;Emission Strength;6;0;Create;True;0;0;0;False;0;False;0.005;0.1;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-720.6923,368.8219;Inherit;False;Constant;_Color0;Color 0;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-323.4708,10.03529;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-720.6923,256.822;Inherit;False;Property;_EmissionPatternIntensity;Emission Pattern Intensity;5;0;Create;True;0;0;0;False;0;False;0.008;0.765;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-717.7574,549.207;Inherit;False;Property;_EmissionColorTint;Emission Color Tint;2;1;[HDR];Create;True;0;0;0;False;0;False;0.09019608,0.572549,0.5490196,1;0.09019593,0.572549,0.5490196,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;7;-320.6926,256.822;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;9;-320.6926,400.822;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-453.4919,998.7972;Inherit;False;Property;_Roughness;Roughness;9;0;Create;True;0;0;0;False;0;False;0;0.103;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-128.6926,256.822;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;16;-164.37,999.2796;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-290.3578,1095.956;Inherit;False;Property;_Opacity;Opacity;7;0;Create;True;0;0;0;False;0;False;0.5;0.484;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-293.3437,907.1727;Inherit;False;Property;_Metallic;Metallic;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;37,10;Float;False;True;-1;5;;0;0;Standard;TidalFlask/Glass Patterns;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.147;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;1;2;3
WireConnection;3;0;2;1
WireConnection;1;1;2;2
WireConnection;1;0;3;0
WireConnection;5;0;4;0
WireConnection;5;1;1;0
WireConnection;7;0;10;0
WireConnection;7;1;5;0
WireConnection;7;2;6;0
WireConnection;9;0;10;0
WireConnection;9;1;11;0
WireConnection;9;2;12;0
WireConnection;8;0;7;0
WireConnection;8;1;9;0
WireConnection;16;0;13;0
WireConnection;0;0;5;0
WireConnection;0;2;8;0
WireConnection;0;3;17;0
WireConnection;0;4;16;0
WireConnection;0;9;14;0
ASEEND*/
//CHKSM=B36136C4116C64B773D3BC590178A88D8BC9C7A4