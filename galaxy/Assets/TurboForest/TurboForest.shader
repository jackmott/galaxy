Shader "Custom/TurboForest" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
	}
	SubShader 
	{
      Tags {"Queue"="AlphaTest" "IgnoreProjector"="True"}
      LOD 100
      Pass 
      {   
		  Tags{
		"LightMode" = "ForwardBase"
		}
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#define UNITY_PASS_FORWARDBASE
		#include "UnityCG.cginc"
		#pragma multi_compile_fwdbase_fullshadows
		#pragma multi_compile_fog
		#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
		#pragma target 3.0
		
		
		uniform sampler2D _MainTex;        
				
		
		struct vertexInput  
		{
			float4 vertex : POSITION;
			float4 tex : TEXCOORD0;
			float4 tex1 : TEXCOORD1;
			
			float4 pos : NORMAL;
		};
		struct vertexOutput 
		{
			float4 pos : SV_POSITION;
			float4 tex : TEXCOORD0;	
			
		};
 
		vertexOutput vert(vertexInput input) 
		{
			vertexOutput output;

			output.pos = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(input.pos.x, input.pos.y, input.pos.z, 1.0) ) - float4(input.vertex.x, -input.vertex.y, 0.0, 0.0));
			output.tex = input.tex;
			

			float3 dir=normalize(_WorldSpaceCameraPos.xyz - input.pos.xyz);
			float d=saturate(dot(dir,float3(0,1,0)));

			output.tex.y+=1.0-(1.0/8)*((int)(d*4))-1.0/8;
			output.tex.x+=(1.0/8)*((int)((d*4-(int)(d*4))*4));
			
			output.tex.x+=input.tex1.x;
			output.tex.y+=input.tex1.y;
			
	
			return output;
		}

		float4 frag(vertexOutput i) : COLOR
		{
			//fixed4 c=tex2D(_MainTex, input.tex.xy);
			

		float4 _MainTex_var = tex2D(_MainTex, TRANSFORM_TEX(i.tex, _MainTex));
		float3 finalColor = _MainTex_var.rgb;
		fixed4 finalRGBA = fixed4(finalColor, 1);
		

			return finalRGBA;
		}
		ENDCG

		}

		// Pass to render object as a shadow caster
		
	
	}
	
}
