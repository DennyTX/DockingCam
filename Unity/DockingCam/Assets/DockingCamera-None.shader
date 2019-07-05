// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DockingCamera/None" { 
Properties { 
	_MainTex ("Base (RGB)", 2D) = "white" {}
    _Color("Color", Color) = (0,0,0,1) 
    _LinesSize("LinesSize", Range(1,10)) = 1 
} 
SubShader { 
    Tags {"IgnoreProjector" = "True" "Queue" = "Overlay"} 
    Fog { Mode Off } 
    Pass { 
        //ZTest Always 
        //ZWrite Off 
        //Blend SrcAlpha OneMinusSrcAlpha

CGPROGRAM
 
#pragma vertex vert_img 
#pragma fragment frag
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

uniform sampler2D _MainTex;

        fixed4 _Color; half _LinesSize;


        half4 frag(v2f_img i) : COLOR {

			half4 tex = tex2D( _MainTex, i.uv );
			return tex; 
         }
 
ENDCG
       }

} 

}
