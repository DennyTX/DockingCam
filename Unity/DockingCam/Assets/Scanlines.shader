// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Scanlines" { 
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

        struct v2f { 
            half4 pos:POSITION; 
            fixed4 sPos:TEXCOORD; 
        };

        v2f vert(appdata_base v) {
            v2f o; o.pos = UnityObjectToClipPos(v.vertex); 
            o.sPos = ComputeScreenPos(o.pos); 
            return o; 
        }

        half4 frag(v2f_img i) : COLOR {

            fixed p = i.uv.y / i.uv.x;
half4 tex = tex2D( _MainTex, i.uv );
return tex; 

int i1 = (int)(i.uv.y/floor(_LinesSize));
if ((fixed)(i.uv.y / 4) % 2 == 0) return _Color;
//            if((fixed)(p*_ScreenParams.y/floor(_LinesSize))%2==0) discard;
               return tex;
         }
 
ENDCG
       }

} 

}
