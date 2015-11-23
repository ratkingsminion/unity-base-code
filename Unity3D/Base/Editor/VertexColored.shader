Shader "A Rat King/VertexColored" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Emission ("Emission Factor", float) = 0.2
	}
	SubShader {
		Tags { "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
		Cull Back
	
		CGPROGRAM
		#pragma surface surf Lambert
		
		fixed4 _Color;
		float _Emission;
		
		struct Input {
			float4 color : COLOR;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = IN.color * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = c * _Emission;
		}
		ENDCG
	}
	
	Fallback "Transparent/Cutout/VertexLit"
}
