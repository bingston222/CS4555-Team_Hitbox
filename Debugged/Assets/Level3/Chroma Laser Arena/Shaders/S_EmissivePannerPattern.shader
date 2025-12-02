// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Emissive Panner Pattern"
{
	Properties
	{
		[NoScaleOffset]_PatternMasks("Pattern Masks", 2D) = "white" {}
		_TriplanarBlendContrast("Triplanar Blend Contrast", Float) = 22
		_TilingPatternPrimary("Tiling Pattern Primary", Range( 0 , 1)) = 0.2
		_TilingPatternSecondary("Tiling Pattern Secondary", Range( 0 , 3)) = 2
		_TilingPatternTertiary("Tiling Pattern Tertiary", Range( 0 , 1)) = 0.3
		_PatternSpeedX("Pattern Speed X", Float) = 0.2
		_PatternSpeedZ("Pattern Speed Z", Float) = 0.6
		[HDR]_EmissionColorPrimary("Emission Color Primary", Color) = (0.6392157,0,0.5450981,1)
		[HDR]_EmissionColorSecondary("Emission Color Secondary", Color) = (0.4627451,0,0.3921569,1)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.5
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _EmissionColorPrimary;
		uniform float4 _EmissionColorSecondary;
		uniform sampler2D _PatternMasks;
		uniform float _TilingPatternSecondary;
		uniform float _TriplanarBlendContrast;
		uniform float _TilingPatternPrimary;
		uniform float _PatternSpeedX;
		uniform float _PatternSpeedZ;
		uniform float _TilingPatternTertiary;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 temp_output_66_0 = ( ase_worldPos * _TilingPatternSecondary );
			float3 ase_worldNormal = i.worldNormal;
			float3 break53 = ( sign( ase_worldNormal ) * float3( 1,1,-1 ) );
			float2 appendResult56 = (float2(break53.x , 1.0));
			float2 appendResult57 = (float2(break53.z , 1.0));
			float3 temp_cast_0 = (_TriplanarBlendContrast).xxx;
			float3 temp_output_4_0 = pow( abs( ase_worldNormal ) , temp_cast_0 );
			float dotResult7 = dot( temp_output_4_0 , float3( 1,1,1 ) );
			float lerpResult73 = lerp( tex2D( _PatternMasks, ( (temp_output_66_0).zy * appendResult56 ) ).g , tex2D( _PatternMasks, ( (temp_output_66_0).xy * appendResult57 ) ).g , ( temp_output_4_0 / dotResult7 ).z);
			float3 temp_output_15_0 = ( ase_worldPos * _TilingPatternPrimary );
			float lerpResult44 = lerp( tex2D( _PatternMasks, ( (temp_output_15_0).zy * appendResult56 ) ).r , tex2D( _PatternMasks, ( (temp_output_15_0).xy * appendResult57 ) ).r , ( temp_output_4_0 / dotResult7 ).z);
			float lerpResult77 = lerp( 0.0 , lerpResult73 , lerpResult44);
			float2 appendResult91 = (float2(_PatternSpeedX , _PatternSpeedZ));
			float3 temp_output_79_0 = ( ase_worldPos * _TilingPatternTertiary );
			float2 panner90 = ( 1.0 * _Time.y * appendResult91 + (temp_output_79_0).zy);
			float2 panner102 = ( 1.0 * _Time.y * appendResult91 + (temp_output_79_0).xy);
			float lerpResult85 = lerp( tex2D( _PatternMasks, ( panner90 * appendResult56 ) ).b , tex2D( _PatternMasks, ( panner102 * appendResult57 ) ).b , ( temp_output_4_0 / dotResult7 ).z);
			float lerpResult88 = lerp( 0.0 , lerpResult77 , lerpResult85);
			float4 lerpResult97 = lerp( _EmissionColorPrimary , _EmissionColorSecondary , lerpResult88);
			o.Emission = lerpResult97.rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.CommentaryNode;64;-1542.381,2635.624;Inherit;False;1172.906;371.244;triplanar blend;7;4;8;7;3;5;45;106;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;63;-1276.45,2305.153;Inherit;False;702;312.7682;mirror projection based on vertex normal;5;51;53;56;57;52;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;106;-1494.281,2696.151;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;96;-1615.59,315.2299;Inherit;False;1845.815;478.5733;primary pattern;10;10;15;48;18;44;13;58;59;117;118;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;95;-1610.918,874.2268;Inherit;False;1845.817;478.5747;secondary pattern;10;65;66;67;68;72;73;75;71;120;119;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;94;-1896.361,1416.816;Inherit;False;2143.55;585.0627;tertiary pattern;15;90;91;89;92;102;86;79;80;81;78;87;85;84;122;121;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SignOpNode;51;-1226.45,2357.153;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-1558.785,1108.828;Inherit;False;Property;_TilingPatternSecondary;Tiling Pattern Secondary;3;0;Create;True;0;0;0;False;0;False;2;2;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;78;-1848.111,1464.646;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;65;-1555.349,928.784;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;86;-1847.188,1799.41;Inherit;False;Property;_TilingPatternTertiary;Tiling Pattern Tertiary;4;0;Create;True;0;0;0;False;0;False;0.3;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;10;-1565.59,367.9307;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;13;-1563.457,549.8299;Inherit;False;Property;_TilingPatternPrimary;Tiling Pattern Primary;2;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1490.771,2893.87;Inherit;False;Property;_TriplanarBlendContrast;Triplanar Blend Contrast;1;0;Create;True;0;0;0;False;0;False;22;22;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1076.45,2357.153;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,-1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.AbsOpNode;3;-1268.028,2694.757;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-1145.917,926.9276;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1504.068,1468.099;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-1446.791,1711.743;Inherit;False;Property;_PatternSpeedX;Pattern Speed X;5;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-1446.744,1799.773;Inherit;False;Property;_PatternSpeedZ;Pattern Speed Z;6;0;Create;True;0;0;0;False;0;False;0.6;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1150.59,367.9307;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;53;-893.4525,2358.153;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.PowerNode;4;-1099.06,2691.899;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;117;-908.4124,366.2549;Inherit;False;FLOAT2;2;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;118;-909.4124,485.2549;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;120;-899.0823,1051.633;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;119;-898.0823,932.6326;Inherit;False;FLOAT2;2;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;121;-1205.579,1470.561;Inherit;False;FLOAT2;2;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;122;-1206.579,1589.561;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;91;-1208.276,1713.237;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;7;-909.7648,2761.906;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;56;-752.4527,2355.153;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;57;-753.6107,2480.915;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-589.3087,929.5172;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-591.6083,1044.218;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;90;-999.3159,1472.895;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;102;-997.4296,1690.382;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-607.7083,367.4706;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-606.3788,486.5714;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;49;-1203.498,2080.76;Inherit;True;Property;_PatternMasks;Pattern Masks;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;43a594af1fe9ad94e9a435aa054927c2;43a594af1fe9ad94e9a435aa054927c2;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleDivideOpNode;8;-760.0612,2692.899;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;72;-293.5817,1125.802;Inherit;True;Property;_Patterns1;Patterns;2;0;Create;True;0;0;0;False;0;False;-1;7cd2de8a15986b842bc45256738f05ac;43a594af1fe9ad94e9a435aa054927c2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;71;-293.1276,924.2268;Inherit;True;Property;_patterns3;patterns2;3;0;Create;True;0;0;0;False;0;False;-1;7cd2de8a15986b842bc45256738f05ac;43a594af1fe9ad94e9a435aa054927c2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-510.8592,1480.612;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;48;-297.8028,365.2299;Inherit;True;Property;_patterns2;patterns2;3;0;Create;True;0;0;0;False;0;False;-1;7cd2de8a15986b842bc45256738f05ac;43a594af1fe9ad94e9a435aa054927c2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;18;-298.2569,566.8032;Inherit;True;Property;_Patterns;Patterns;2;0;Create;True;0;0;0;False;0;False;-1;7cd2de8a15986b842bc45256738f05ac;43a594af1fe9ad94e9a435aa054927c2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-506.9062,1690.148;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;45;-516.4775,2690.417;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;84;-300.1844,1668.392;Inherit;True;Property;_Patterns2;Patterns;3;0;Create;True;0;0;0;False;0;False;-1;7cd2de8a15986b842bc45256738f05ac;43a594af1fe9ad94e9a435aa054927c2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;87;-299.7303,1466.816;Inherit;True;Property;_patterns4;patterns2;6;0;Create;True;0;0;0;False;0;False;-1;7cd2de8a15986b842bc45256738f05ac;43a594af1fe9ad94e9a435aa054927c2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;73;56.89909,1127.037;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;52.22408,568.0385;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;85;50.29638,1669.626;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;77;275.6493,1122.052;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;88;467.0081,1122.814;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;98;681.954,992.5562;Inherit;False;Property;_EmissionColorPrimary;Emission Color Primary;7;1;[HDR];Create;True;0;0;0;False;0;False;0.6392157,0,0.5450981,1;0.6392157,0,0.5450981,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;99;683.954,1218.556;Inherit;False;Property;_EmissionColorSecondary;Emission Color Secondary;8;1;[HDR];Create;True;0;0;0;False;0;False;0.4627451,0,0.3921569,1;0.4627451,0,0.3921569,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;97;1005.754,1107.556;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1478.151,1059.179;Float;False;True;-1;5;;0;0;Unlit;TidalFlask/Emissive Panner Pattern;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;51;0;106;0
WireConnection;52;0;51;0
WireConnection;3;0;106;0
WireConnection;66;0;65;0
WireConnection;66;1;75;0
WireConnection;79;0;78;0
WireConnection;79;1;86;0
WireConnection;15;0;10;0
WireConnection;15;1;13;0
WireConnection;53;0;52;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;117;0;15;0
WireConnection;118;0;15;0
WireConnection;120;0;66;0
WireConnection;119;0;66;0
WireConnection;121;0;79;0
WireConnection;122;0;79;0
WireConnection;91;0;92;0
WireConnection;91;1;89;0
WireConnection;7;0;4;0
WireConnection;56;0;53;0
WireConnection;57;0;53;2
WireConnection;67;0;119;0
WireConnection;67;1;56;0
WireConnection;68;0;120;0
WireConnection;68;1;57;0
WireConnection;90;0;121;0
WireConnection;90;2;91;0
WireConnection;102;0;122;0
WireConnection;102;2;91;0
WireConnection;59;0;117;0
WireConnection;59;1;56;0
WireConnection;58;0;118;0
WireConnection;58;1;57;0
WireConnection;8;0;4;0
WireConnection;8;1;7;0
WireConnection;72;0;49;0
WireConnection;72;1;68;0
WireConnection;71;0;49;0
WireConnection;71;1;67;0
WireConnection;80;0;90;0
WireConnection;80;1;56;0
WireConnection;48;0;49;0
WireConnection;48;1;59;0
WireConnection;18;0;49;0
WireConnection;18;1;58;0
WireConnection;81;0;102;0
WireConnection;81;1;57;0
WireConnection;45;0;8;0
WireConnection;84;0;49;0
WireConnection;84;1;81;0
WireConnection;87;0;49;0
WireConnection;87;1;80;0
WireConnection;73;0;71;2
WireConnection;73;1;72;2
WireConnection;73;2;45;2
WireConnection;44;0;48;1
WireConnection;44;1;18;1
WireConnection;44;2;45;2
WireConnection;85;0;87;3
WireConnection;85;1;84;3
WireConnection;85;2;45;2
WireConnection;77;1;73;0
WireConnection;77;2;44;0
WireConnection;88;1;77;0
WireConnection;88;2;85;0
WireConnection;97;0;98;0
WireConnection;97;1;99;0
WireConnection;97;2;88;0
WireConnection;0;2;97;0
ASEEND*/
//CHKSM=6B5A19CFB44C5A8E513795574770C82CA4147A8E