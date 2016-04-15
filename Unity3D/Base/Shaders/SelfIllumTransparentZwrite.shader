Shader "A Rat King/SelfIllumTransparentZwrite" {
	Properties {
		_Color ("Main Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_Emission ("Emission Factor", float) = 0.2 // (0.5, 0.5, 0.5, 0.5)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		//Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		// Lighting On
		
		Pass {
			Zwrite on
			ColorMask 0
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _Color;
			struct f2v { float4 vertex : POSITION; half2 texcoord : TEXCOORD0; };
			struct v2f { float4 pos : POSITION; half2 uv : TEXCOORD0; float4 projPos : TEXCOORD1; };
			float4 _MainTex_ST;
			v2f vert (f2v v) { v2f o; o.pos = mul (UNITY_MATRIX_MVP, v.vertex); o.projPos = ComputeScreenPos(o.pos); COMPUTE_EYEDEPTH(o.projPos.z); o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); return o; }
			fixed4 frag( v2f i ) : COLOR { return tex2D(_MainTex, i.uv) * _Color; }
			ENDCG			
		}
        
		CGPROGRAM
		#pragma surface surf Lambert alpha
		
		sampler2D _MainTex;
		float4 _Color;
		float _Emission;
		
		struct Input {
			float2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Emission = c * _Emission;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Transparent/Diffuse"
}
