Shader "Noise/ StandardPerlin"
{
	Properties
	{
		_Octaves("Octaves",Range(1,8)) = 1
		_Lacunarity("Lacunarity", Range(1.0, 10.0)) = 1.0
		_Gain("Gain", Range(0, 10.0)) = 1.0
		_Frequency("Frequency",Range(0.0,20.0)) = 1.0
		_Type("1:FBM,2:Turbulent,3:Ridge",Range(1,3)) = 1
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		_ParallaxMap("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		_DetailMask("Detail Mask", 2D) = "white" {}

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

	

		[Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0


			// Blending state
			[HideInInspector] _Mode("__mode", Float) = 0.0
			[HideInInspector] _SrcBlend("__src", Float) = 1.0
			[HideInInspector] _DstBlend("__dst", Float) = 0.0
			[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}

		CGINCLUDE
#define UNITY_SETUP_BRDF_INPUT MetallicSetup
			ENDCG

			SubShader
		{
			Tags { "RenderType" = "Opaque" "PerformanceChecks" = "False" }
			LOD 300

			
			// ------------------------------------------------------------------
			//  Base forward pass (directional light, emission, lightmaps, ...)
			Pass
			{
				Name "FORWARD"
				Tags { "LightMode" = "ForwardBase" }

				Blend[_SrcBlend][_DstBlend]
				ZWrite[_ZWrite]

				CGPROGRAM
				#pragma target 3.0
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles

			// -------------------------------------

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP 
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog

			#pragma vertex vertBase
			#pragma fragment fragBase
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
			// ------------------------------------------------------------------
			//  Additive forward pass (one light per pass)
			Pass
			{
				Name "FORWARD_DELTA"
				Tags { "LightMode" = "ForwardAdd" }
				Blend[_SrcBlend] One
				Fog { Color(0,0,0,0) } // in additive pass fog should be black
				ZWrite Off
				ZTest LEqual

				CGPROGRAM
				#pragma target 3.0
			// GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles

			// -------------------------------------


			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog

			#pragma vertex vertAdd
			#pragma fragment fragAdd
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
			// ------------------------------------------------------------------
			//  Shadow rendering pass
			Pass {
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }

				ZWrite On ZTest LEqual

				CGPROGRAM
				#pragma target 3.0
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles

			// -------------------------------------


			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"

			ENDCG
		}

		
			// ------------------------------------------------------------------
			//  Deferred pass
			Pass
		{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			CGPROGRAM
			#pragma target 3.0
	// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
	#pragma exclude_renderers nomrt gles


	// -------------------------------------

	#pragma shader_feature _NORMALMAP
	#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
	#pragma shader_feature _EMISSION
	#pragma shader_feature _METALLICGLOSSMAP
	#pragma shader_feature ___ _DETAIL_MULX2
	#pragma shader_feature _PARALLAXMAP

	#pragma multi_compile ___ UNITY_HDR_ON
	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
	#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

	#pragma vertex vertDeferred
	#pragma fragment fragDeferred

	#include "UnityStandardCore.cginc"

	ENDCG
}



// ------------------------------------------------------------------
// Extracts information for lightmapping, GI (emission, albedo, ...)
// This pass it not used during regular rendering.
Pass
{
	Name "META"
	Tags { "LightMode" = "Meta" }

	Cull Off

	CGPROGRAM
	#pragma vertex vert_meta
	#pragma fragment frag_meta

	#pragma shader_feature _EMISSION
	#pragma shader_feature _METALLICGLOSSMAP
	#pragma shader_feature ___ _DETAIL_MULX2

	#include "UnityStandardMeta.cginc"
	ENDCG
}

		}

			SubShader
{
	Tags { "RenderType" = "Opaque" "PerformanceChecks" = "False" }
	LOD 150

	// ------------------------------------------------------------------
	//  Base forward pass (directional light, emission, lightmaps, ...)
	Pass
	{
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }

		Blend[_SrcBlend][_DstBlend]
		ZWrite[_ZWrite]

		CGPROGRAM
		#pragma target 2.0

		#pragma shader_feature _NORMALMAP
		#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
		#pragma shader_feature _EMISSION 
		#pragma shader_feature _METALLICGLOSSMAP 
		#pragma shader_feature ___ _DETAIL_MULX2
	// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP

	#pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE

	#pragma multi_compile_fwdbase
	#pragma multi_compile_fog

	#pragma vertex vertBase
	#pragma fragment fragBase
	#include "UnityStandardCoreForward.cginc"

	ENDCG
}

// ------------------------------------------------------------------
//  Additive forward pass (one light per pass)
Pass
{
	Name "FORWARD_DELTA"
	Tags { "LightMode" = "ForwardAdd" }
	Blend[_SrcBlend] One
	Fog { Color(0,0,0,0) } // in additive pass fog should be black
	ZWrite Off
	ZTest LEqual

	CGPROGRAM
	#pragma target 2.0

	#pragma shader_feature _NORMALMAP
	#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
	#pragma shader_feature _METALLICGLOSSMAP
	#pragma shader_feature ___ _DETAIL_MULX2
	// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP
	#pragma skip_variants SHADOWS_SOFT

	#pragma multi_compile_fwdadd_fullshadows
	#pragma multi_compile_fog

	#pragma vertex vertAdd
	#pragma fragment fragAdd
	#include "UnityStandardCoreForward.cginc"

	ENDCG
}
// ------------------------------------------------------------------
//  Shadow rendering pass
Pass {
	Name "ShadowCaster"
	Tags { "LightMode" = "ShadowCaster" }

	ZWrite On ZTest LEqual

	CGPROGRAM
	#pragma target 2.0

	#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
	#pragma skip_variants SHADOWS_SOFT
	#pragma multi_compile_shadowcaster

	#pragma vertex vertShadowCaster
	#pragma fragment fragShadowCaster

	#include "UnityStandardShadow.cginc"

	ENDCG
}

// ------------------------------------------------------------------
// Extracts information for lightmapping, GI (emission, albedo, ...)
// This pass it not used during regular rendering.
Pass
{
	Name "META"
	Tags { "LightMode" = "Meta" }

	Cull Off

	CGPROGRAM
	#pragma vertex vert_meta
	#pragma fragment frag_meta

	#pragma shader_feature _EMISSION
	#pragma shader_feature _METALLICGLOSSMAP
	#pragma shader_feature ___ _DETAIL_MULX2

	#include "UnityStandardMeta.cginc"
	ENDCG
}

Pass
{
	Name "Perlin"
	Blend One One
	CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#include "UnityCG.cginc"

	uniform sampler2D _PermTable2D, _Gradient3D;
fixed _Octaves;
float _Lacunarity;
float _Gain;
float _Frequency;
fixed _Type;


struct v2f
{
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD;
};

v2f vert(appdata_base v)
{
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.vertex;
	return o;
}

float3 fade(float3 t)
{
	return t * t * t * (t * (t * 6 - 15) + 10); // new curve
												//return t * t * (3 - 2 * t); // old curve
}

float4 perm2d(float2 uv)
{
	return tex2D(_PermTable2D, uv);
}

float gradperm(float x, float3 p)
{
	float3 g = tex2D(_Gradient3D, float2(x, 0)).rgb *2.0 - 1.0;
	return dot(g, p);
}

float inoise(float3 p)
{
	float3 P = fmod(floor(p), 256.0);	// FIND UNIT CUBE THAT CONTAINS POINT
	p -= floor(p);                      // FIND RELATIVE X,Y,Z OF POINT IN CUBE.
	float3 f = fade(p);                 // COMPUTE FADE CURVES FOR EACH OF X,Y,Z.

	P = P / 256.0;
	const float one = 1.0 / 256.0;

	// HASH COORDINATES OF THE 8 CUBE CORNERS
	float4 AA = perm2d(P.xy) + P.z;

	// AND ADD BLENDED RESULTS FROM 8 CORNERS OF CUBE
	return lerp(lerp(lerp(gradperm(AA.x, p),
		gradperm(AA.z, p + float3(-1, 0, 0)), f.x),
		lerp(gradperm(AA.y, p + float3(0, -1, 0)),
			gradperm(AA.w, p + float3(-1, -1, 0)), f.x), f.y),

		lerp(lerp(gradperm(AA.x + one, p + float3(0, 0, -1)),
			gradperm(AA.z + one, p + float3(-1, 0, -1)), f.x),
			lerp(gradperm(AA.y + one, p + float3(0, -1, -1)),
				gradperm(AA.w + one, p + float3(-1, -1, -1)), f.x), f.y), f.z);
}

// fractal sum, range -1.0 - 1.0
float fBm(float3 p, int octaves)
{
	float freq = _Frequency, amp = 0.5;
	float sum = 0;
	for (int i = 0; i < octaves; i++)
	{
		sum += inoise(p * freq) * amp;
		freq *= _Lacunarity;
		amp *= _Gain;
	}
	return sum;
}

// fractal abs sum, range 0.0 - 1.0
float turbulence(float3 p, int octaves)
{
	float sum = 0;
	float freq = _Frequency, amp = 1.0;
	for (int i = 0; i < octaves; i++)
	{
		sum += abs(inoise(p*freq))*amp;
		freq *= _Lacunarity;
		amp *= _Gain;
	}
	return sum;
}

// Ridged multifractal, range 0.0 - 1.0
// See "Texturing & Modeling, A Procedural Approach", Chapter 12
float ridge(float h, float offset)
{
	h = abs(h);
	h = offset - h;
	h = h * h;
	return h;
}

float ridgedmf(float3 p, int octaves, float offset)
{
	float sum = 0;
	float freq = _Frequency, amp = 0.5;
	float prev = 1.0;
	for (int i = 0; i < octaves; i++)
	{
		float n = ridge(inoise(p*freq), offset);
		sum += n*amp*prev;
		prev = n;
		freq *= _Lacunarity;
		amp *= _Gain;
	}
	return sum;
}

half4 frag(v2f i) : COLOR
{
	float n = 1;
//uncomment this for fractal noise
if (_Type == 1)
{
	n = fBm(i.uv.xyz, _Octaves);
}
else if (_Type == 2)
{
	n = turbulence(i.uv.xyz, _Octaves);
}
else if (_Type == 3)
{
	n = ridgedmf(i.uv.xyz, _Octaves, 1.0);
}

return half4(n, n, n, 1);
}



ENDCG

}

}


FallBack "VertexLit"

}
