Shader "A Rat King/BoxMappingDiffuse" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		fixed4 _Color;

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};


		/*
		float2x2 UVScaleRotation;
		float2 UVOffset;
		float2 CalculateUVs(float3 pos, float3 normal)
		{
			float diff = length(pos * normal) * 2;
			float2 uv = float2(diff + pos.x + (pos.z * normal.x), diff - pos.y + (pos.z * normal.y));
			return mul(uv, UVScaleRotation) + UVOffset;
		}
		*/


		void surf (Input IN, inout SurfaceOutput o) {			
			// taken from http://unifycommunity.com/wiki/index.php?title=3SideProjDiffuse
			float2 tex0 = IN.worldPos.xy;// * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 tex1 = IN.worldPos.zx;// * _MainTex_ST.yx + _MainTex_ST.wz;
			float2 tex2 = IN.worldPos.zy;// * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 color0_ = tex2D(_MainTex, tex0);
			float4 color1_ = tex2D(_MainTex, tex1);
			float4 color2_ = tex2D(_MainTex, tex2);

			float3 projnormal = saturate(pow(IN.worldNormal*1.5, 4));
			half3 c = lerp(color1_, color0_, projnormal.z);
			c = lerp(c, color2_, projnormal.x);
			/*
			UVScaleRotation = float2x2(1, 0, 0, 1);
			UVOffset = float2(0, 0);
			c = tex2D(_MainTex, CalculateUVs(IN.worldPos, IN.worldNormal));
			*/
			o.Albedo = c * _Color;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
