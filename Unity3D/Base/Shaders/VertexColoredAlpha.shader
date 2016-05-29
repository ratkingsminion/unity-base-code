Shader "A Rat King/VertexColoredAlpha" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Emission ("Emission Factor", float) = 0.2 // (0.5, 0.5, 0.5, 0.5)
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"} // Cutout"}
		LOD 200 
		Cull Off

		CGPROGRAM
		#pragma surface surf Lambert alpha
		
		fixed4 _Color;
		sampler2D _MainTex;
		float _Emission;
		
		struct Input {
			float4 color : COLOR;
			float2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = c * _Emission;
		}
		ENDCG
	}
	
	Fallback "Transparent/Cutout/VertexLit"
}
