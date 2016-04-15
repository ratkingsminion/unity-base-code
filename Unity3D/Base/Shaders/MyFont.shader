Shader "A Rat King/3D Text Shader" { 
	Properties { 
	   _MainTex ("Font Texture", 2D) = "white" {} 
	   _Color ("Text Color", Color) = (1,1,1,1) 
	}
	
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off Cull Back ZWrite Off Fog { Mode Off } 
		Blend SrcAlpha OneMinusSrcAlpha 
		
		Pass {
			ColorMask RGB
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _Color;
			
			struct f2v {
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};
			
			struct v2f {
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				float4 color : COLOR;
				float4 projPos : TEXCOORD1;
			};
			
			//float4 _MainTex_ST;

			v2f vert (f2v v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.projPos = ComputeScreenPos(o.pos);
				COMPUTE_EYEDEPTH(o.projPos.z);
				//o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv = v.texcoord;
				o.color = v.color;
				return o;
			}

			fixed4 frag( v2f i ) : COLOR
			{			
				return tex2D(_MainTex, i.uv).a * i.color * _Color;
			}
			ENDCG
		}
	}
	
	//SubShader { 
	//	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" } 
	//	Lighting Off Cull Back ZWrite Off Fog { Mode Off } 
	//	Blend SrcAlpha OneMinusSrcAlpha 
	//	Pass { 
	//		ColorMask RGB
	//		Color [_Color] 
	//		SetTexture [_MainTex] { 
	//			combine primary, texture * primary 
	//		} 
	//	} 
	//}
}