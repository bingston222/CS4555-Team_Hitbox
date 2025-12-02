// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Emissive Panner Noise"
{
	Properties
	{
		[HDR]_EmissionColor("Emission Color", Color) = (0.8392158,0,0.1607843,1)
		_NoiseContrast("Noise Contrast", Range( 0 , 10)) = 1
		_NoiseTiling("Noise Tiling", Range( 0 , 2)) = 0.64
		_NoiseSpeedX1("Noise Speed X", Float) = -1
		_NoiseSpeedY1("Noise Speed Y", Float) = -1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.5
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _EmissionColor;
		uniform float _NoiseSpeedX1;
		uniform float _NoiseSpeedY1;
		uniform float _NoiseTiling;
		uniform float _NoiseContrast;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _EmissionColor.rgb;
			float2 appendResult42 = (float2(_NoiseSpeedX1 , _NoiseSpeedY1));
			float2 panner43 = ( 1.0 * _Time.y * appendResult42 + i.uv_texcoord);
			float simplePerlin2D26 = snoise( panner43*_NoiseTiling );
			simplePerlin2D26 = simplePerlin2D26*0.5 + 0.5;
			o.Alpha = saturate( pow( simplePerlin2D26 , _NoiseContrast ) );
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.RangedFloatNode;41;-1423.057,238.0445;Inherit;False;Property;_NoiseSpeedY1;Noise Speed Y;4;0;Create;True;0;0;0;False;0;False;-1;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-1423.104,150.0145;Inherit;False;Property;_NoiseSpeedX1;Noise Speed X;3;0;Create;True;0;0;0;False;0;False;-1;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;-1179.812,150.3145;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;44;-1221.475,15.39697;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;34;-1058.251,422.4796;Inherit;False;Property;_NoiseTiling;Noise Tiling;2;0;Create;True;0;0;0;False;0;False;0.64;0.64;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;43;-969.1177,126.0145;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;26;-727.8531,125.047;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-730.0126,422.5816;Inherit;False;Property;_NoiseContrast;Noise Contrast;1;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;37;-422.2874,126.2148;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;45;-131.5504,123.7007;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;35;-189.8544,-112.4723;Inherit;False;Property;_EmissionColor;Emission Color;0;1;[HDR];Create;True;0;0;0;False;0;False;0.8392158,0,0.1607843,1;0.8392157,0,0.05685604,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;64.62476,-67.01291;Float;False;True;-1;5;;0;0;Unlit;TidalFlask/Emissive Panner Noise;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;42;0;40;0
WireConnection;42;1;41;0
WireConnection;43;0;44;0
WireConnection;43;2;42;0
WireConnection;26;0;43;0
WireConnection;26;1;34;0
WireConnection;37;0;26;0
WireConnection;37;1;38;0
WireConnection;45;0;37;0
WireConnection;0;2;35;0
WireConnection;0;9;45;0
ASEEND*/
//CHKSM=D962CE499C7F57665B637958C53201488428AFA7