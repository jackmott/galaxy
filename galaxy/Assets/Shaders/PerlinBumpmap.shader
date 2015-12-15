//
//	Code repository for GPU noise development blog
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//
//	I'm not one for copyrights.  Use the code however you wish.
//	All I ask is that credit be given back to the blog or myself when appropriate.
//	And also to let me know if you come up with any changes, improvements, thoughts or interesting uses for this stuff. :)
//	Thanks!
//
//	Brian Sharpe
//	brisharpe CIRCLE_A yahoo DOT com
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//
//===============================================================================
//  Scape Software License
//===============================================================================
//
//Copyright (c) 2007-2012, Giliam de Carpentier
//All rights reserved.
//
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met: 
//
//1. Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer. 
//2. Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNERS OR CONTRIBUTORS BE LIABLE 
//FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
//DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
//CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.;

Shader "Noise/PerlinBumpmap" 
{
	Properties 
	{
		
		_Lacunarity("Lacunarity", Range(1.0, 10.0)) = 1.0
		_Gain("Persistence", Range(0, 10.0)) = 1.0
		_Frequency("Frequency",Range(0.0,20.0)) = 1.0				
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_ColorGradient("ColorGradient", 2D) = "white" {}

	}


		SubShader
	{




		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma glsl
		#pragma target 3.0


		uniform sampler2D _PermTable2D, _Gradient3D,_BumpMap;
		uniform float _Frequency, _Lacunarity, _Gain;

	

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


		struct Input 
		{
			float3 pos;
			float2 uv_BumpMap;

		};

		void vert (inout appdata_full v, out Input OUT)
		{
			UNITY_INITIALIZE_OUTPUT(Input, OUT);
			OUT.pos = v.vertex.xyz;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			
			float noise = fBm(IN.pos, 4);
			/*
			float2 gradientUV = float2(noise,0);
			float4 color = tex2D(_ColorGradient, gradientUV);
			*/
			 			
			o.Albedo = float3(noise, noise, noise);
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Alpha = 1.0;
		}
		ENDCG
	}
	
	FallBack "Diffuse"
}