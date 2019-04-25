
Shader "Custom/NightVision" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Brightness ("Brightness Amount", Float ) = 2.0
        _Bright ("Bright Areas", Float ) = 1.0
    }
    SubShader {
        Pass {
           
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
 
            uniform sampler2D _MainTex;
            uniform float _Brightness;
            uniform float _Bright;
                   
            half4 frag (v2f_img i) : COLOR {
                half4 tex = tex2D( _MainTex, i.uv );
               
                // greyscale the pixel color values
                half4 grey = dot( half3( 0.222, 0.707, 0.071 ), tex );
               
                if(_Bright == 1.0 ) {
                    // white out bright spots
                    if( grey.x > 0.4 )
                        grey *= _Brightness;
                }
               
                // boost the green channel     
                grey.y *= 2.5;
               
                return grey;        
            }
        ENDCG
        }
    }
}
 

