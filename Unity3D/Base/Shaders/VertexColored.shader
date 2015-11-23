Shader "A Rat King/VertexLit Colored" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
   // _SpecColor ("Spec Color", Color) = (1,1,1,0)
    _Emission ("Emissive Color", Color) = (0,0,0,0)
    //_Shininess ("Shininess", Range (0.01, 1)) = 0.7
    _MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
   // ZWrite Off
    //Alphatest Greater 0
    Tags {Queue=Geometry}
    //Blend SrcAlpha OneMinusSrcAlpha 
    ColorMask RGB
    Pass {
        Material {
			//Shininess [_Shininess]
            //Specular [_SpecColor]
            Emission [_Emission]    
        }
        ColorMaterial AmbientAndDiffuse
        Lighting On
       // SeperateSpecular On
        SetTexture [_MainTex] {
            Combine texture * primary, texture * primary
        }
        SetTexture [_MainTex] {
            constantColor [_Color]
            Combine previous * constant DOUBLE, previous * constant
        } 
    }
}

Fallback "VertexLit", 1
}