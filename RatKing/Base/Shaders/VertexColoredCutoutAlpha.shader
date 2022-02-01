Shader "A Rat King/VertexColoredCutoutAlpha" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Emission ("Emission Factor", float) = 0.2 // (0.5, 0.5, 0.5, 0.5)
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent"} // Cutout"}
		LOD 200

				CGPROGRAM
		#pragma surface surf Lambert alpha
		// alphatest:_Cutoff
		
		fixed4 _Color;
		sampler2D _MainTex;
		float _Emission;
		float _Cutoff;
		
		struct Input {
			float4 color : COLOR;
			float2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb * _Color.rgb;
			if (_Cutoff < 1.0 && c.a >= _Cutoff)
				o.Alpha = _Color.a;
			else
				o.Alpha = 0.0;
			o.Emission = c * _Emission;
		}
		ENDCG
	}
	
	Fallback "Transparent/Cutout/VertexLit"
}
