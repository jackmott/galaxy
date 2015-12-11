// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2865,x:33811,y:32402,varname:node_2865,prsc:2|diff-7781-OUT,spec-358-OUT,gloss-1813-OUT,normal-5964-RGB;n:type:ShaderForge.SFN_Multiply,id:6343,x:32569,y:32623,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31921,y:32805,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31921,y:32620,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32473,y:33117,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:358,x:32311,y:32899,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32250,y:33011,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_FragmentPosition,id:9867,x:31921,y:32436,varname:node_9867,prsc:2;n:type:ShaderForge.SFN_Transform,id:7698,x:32208,y:32419,varname:node_7698,prsc:2,tffrom:0,tfto:1|IN-9867-XYZ;n:type:ShaderForge.SFN_Code,id:8153,x:32569,y:32372,varname:node_8153,prsc:2,code:cgBlAHQAdQByAG4AIABmAGwAbwBhAHQANAAoADEALAAxACwAMQAsADEAKQA7AAoA,output:3,fname:Function_node_8153,width:247,height:181,input:2,input_1_label:_Pos|A-7698-XYZ;n:type:ShaderForge.SFN_ValueProperty,id:1448,x:32364,y:32091,ptovrint:False,ptlb:_Lacunarity,ptin:__Lacunarity,varname:node_1448,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:7781,x:33043,y:32552,varname:node_7781,prsc:2|A-8153-OUT,B-6343-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8465,x:32364,y:32174,ptovrint:False,ptlb:_Octaves,ptin:__Octaves,varname:node_8465,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:5789,x:32364,y:32249,ptovrint:False,ptlb:_Frequency,ptin:__Frequency,varname:node_5789,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:1872,x:32364,y:32005,ptovrint:False,ptlb:_Gain,ptin:__Gain,varname:node_1872,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:9592,x:32364,y:31915,ptovrint:False,ptlb:_Type,ptin:__Type,varname:node_9592,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:5964-6665-7736-358-1813-1448;pass:END;sub:END;*/

Shader "Shader Forge/Perlin" {
    Properties {
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _MainTex ("Base Color", 2D) = "white" {}
		_Metallic("Metallic", Range(0, 1)) = 0
		_Gloss("Gloss", Range(0, 1)) = 0.8
		_Lacunarity("_Lacunarity", Float) = 2
		_Frequency("_Frequency",Float) = 5
		_Gain("_Gain",Float) =.5
		_Type("_Type",Float) =1
		_Octaves("_Octaves",Float) =2
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
			uniform sampler2D _PermTable2D, _Gradient3D;
			uniform float _Octaves;
			uniform float _Frequency;
			uniform float _Lacunarity;
			uniform float _Gain;
			uniform float _Type;

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

			float4 getnoise(float3 v)
			{
				//uncomment this for fractal noise
				float n = fBm(v, _Octaves);

				//uncomment this for turbulent noise
				//float n = turbulence(i.uv.xyz,  _Octaves);

				//uncomment this for ridged multi fractal
				//float n = ridgedmf(i.uv.xyz,  _Octaves, 1.0);

				return float4(n, n, n, 1);
			}

            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (getnoise( mul( _World2Object, float4(i.posWorld.rgb,0) ).xyz.rgb )*(_MainTex_var.rgb*_Color.rgb)).rgb; // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, _Metallic, specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _Metallic;
            uniform float _Gloss;
			uniform sampler2D _PermTable2D, _Gradient3D;
			uniform float _Octaves;
			uniform float _Frequency;
			uniform float _Lacunarity;
			uniform float _Gain;
			uniform float _Type;

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

			float4 getnoise(float3 v)
			{
				//uncomment this for fractal noise
				float n = fBm(v, _Octaves);

			//uncomment this for turbulent noise
			//float n = turbulence(i.uv.xyz,  _Octaves);

			//uncomment this for ridged multi fractal
			//float n = ridgedmf(i.uv.xyz,  _Octaves, 1.0);

			return float4(n,n,n,1);
			}
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffuseColor = (getnoise( mul( _World2Object, float4(i.posWorld.rgb,0) ).xyz.rgb )*(_MainTex_var.rgb*_Color.rgb)).rgb; // Need this for specular when using metallic
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, _Metallic, specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Metallic;
            uniform float _Gloss;
            float4 Function_node_8153( float3 _Pos ){
            return float4(1,1,1,1);
            
            }
            
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 diffColor = (Function_node_8153( mul( _World2Object, float4(i.posWorld.rgb,0) ).xyz.rgb )*(_MainTex_var.rgb*_Color.rgb)).rgb;
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
