Shader "A Rat King/RimTransparent" {
Properties {
	_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Texture", 2D) = "white" {}
	_Emission ("Emission Factor", float) = 0.2 // (0.5, 0.5, 0.5, 0.5)
	_RimColor ("Rim Color", Color) = (0.5,0.5,0.5,0.0)
	_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
}
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	
	CGPROGRAM
	#pragma surface surf Lambert alpha
	
	struct Input {
	float2 uv_MainTex;
	float3 viewDir;
	};
	
	fixed4 _Color;
	sampler2D _MainTex;
	float _Emission;
	float4 _RimColor;
	float _RimPower;
	
	void surf (Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		//o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
		half rim = 1.0 - saturate(dot (Unity_SafeNormalize(IN.viewDir), o.Normal));
		o.Emission = _RimColor.rgb * pow (rim, _RimPower) * _Emission;
		
		o.Alpha = c.a;
	}
	ENDCG
	}

Fallback "Transparent/Diffuse"
}