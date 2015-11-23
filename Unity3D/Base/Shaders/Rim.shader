Shader "A Rat King/Rim" {
Properties {
	_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
	_MainTex ("Texture", 2D) = "white" {}
	_BumpMap ("Bumpmap", 2D) = "bump" {}
	_Emission ("Emission Factor", float) = 0.2 // (0.5, 0.5, 0.5, 0.5)
	_RimColor ("Rim Color", Color) = (0.5,0.5,0.5,0.0)
	_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
}
SubShader {
	Tags { "RenderType" = "Opaque" }
		
	CGPROGRAM
	#pragma surface surf Lambert
	
	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float3 viewDir;
	};
	
	fixed4 _Color;
	sampler2D _MainTex;
	sampler2D _BumpMap;
	float _Emission;
	float4 _RimColor;
	float _RimPower;
	
	/*
	void surf (Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
		half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
		o.Emission = _RimColor.rgb * pow (rim, _RimPower) * _Emission;
		o.Alpha = _Color.a; // + _RimColor.a * o.Emission;
	}
	*/

	void surf (Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
		o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
		half rim = 1.0 - saturate(dot (Unity_SafeNormalize(IN.viewDir), o.Normal));
		o.Emission = _RimColor.rgb * pow (rim, _RimPower);
		o.Alpha = _Color.a; // + _RimColor.a * o.Emission;
	}

	ENDCG
}

Fallback "Diffuse"
}