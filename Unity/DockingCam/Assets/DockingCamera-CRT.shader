// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//#<!--
//#    CRT-simple shader
//#
//#    Copyright (C) 2011 DOLLS. Based on cgwg's CRT shader.
//#
//#    Modified by fontmas: 2015-03-06
//#
//#    This program is free software; you can redistribute it and/or modify it
//#    under the terms of the GNU General Public License as published by the Free
//#    Software Foundation; either version 2 of the License, or (at your option)
//#    any later version.
//#    -->
Shader "DockingCamera/CRT"
{
	Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
 
        _ScanLineSize ("Scan line size XY", Vector ) = (256,256,0,0)
        _YExtra ("Y-extra", Float) = 0.5
        _Gamma ("Gamma", Float) = 1.0
   
        _ColorScanLine1("Scan lines color 1", Color) =  (1,1,1,1)
        [MaterialToggle] UseIndividualColorChannels1 ("Use Individual Color Channels 1", Float) = 0
        _rgb1R ("Color1 r", Float) = 1.0
        _rgb1G("Color1 g", Float) = 1.0
        _rgb1B ("Color 1 b", Float)= 1.0
   
 
        _ColorScanLine2("Scan lines color 2", Color) =  (1,1,1,1)
         [MaterialToggle] UseIndividualColorChannels2("Use Individual Color Channels 2", Float) = 0
        _rgb2R ("Color2 r", Float)= 1.0
        _rgb2G ("Color2 g", Float) = 1.0
        _rgb2B ("Color2 b", Float)= 1.0
     
           _dotWeight ("dot weight", Float)= 2.0
     
           [MaterialToggle] UseDistortion("Use Distortion", Float) =0
           _Distortion ("Distortion", Float) = 0.1
    }
 
    SubShader
    {
        Tags
        {
			"RenderType" = "Opaque" 
			
            //"Queue"="Transparent"
            //"IgnoreProjector"="True"
            //"RenderType"="Transparent"
            //"PreviewType"="Plane"
            //"CanUseSpriteAtlas"="True"
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ USEINDIVIDUALCOLORCHANNELS1_ON
            #pragma multi_compile _ USEINDIVIDUALCOLORCHANNELS2_ON
            #pragma multi_compile _ USEDISTORTION_ON
            #include "UnityCG.cginc"
            //#define CURVATURE
              //#pragma target 3.0
            #define PI 3.141592653589
       
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
       
            fixed4 _Color;
         
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
           
                #endif
           
           
 
                return OUT;
            }
 
            uniform sampler2D _MainTex;
       
            fixed2 _InputSize;
            fixed2 _OutputSize;
            int2 _TextureSize;
            fixed2 _One;
            half2 _Texcoord;
             fixed  _Factor;
       
            uniform fixed _Distortion = .1f; // 0.1f
            uniform fixed _Gamma = 1.0f; // 1.0f
            uniform fixed _curvatureSet1 = 0.5f; // 0.5f
            uniform fixed _curvatureSet2 = 0.5f; // 0.5f
            uniform fixed _YExtra = 0.5f; // 0.5f;
            uniform fixed _rgb1R = .4f; // 1.0f
            uniform fixed _rgb1G = 1.0f; // 1.0f
            uniform fixed _rgb1B = .5f; // 1.0f
            uniform fixed _rgb2R = 1.0f; // 1.0f
            uniform fixed _rgb2G = .2f; // 1.0f
            uniform fixed _rgb2B = 1.0f; // 1.0f
            uniform fixed _dotWeight = 2.0f; // 2.0f
            uniform int4 _ScanLineSize;
            uniform fixed4 _ColorScanLine1;
            uniform fixed4 _ColorScanLine2;
   
 
             fixed4 ScanlineWeights(fixed distance, fixed4 color)
            {
                fixed4 width = 2.0f + 2.0f * pow(color, float4(4.0f, 4.0f, 4.0f, 4.0f));
                fixed4 weights = fixed4(distance / 0.5f, distance / 0.5f, distance / 0.5f, distance / 0.5f);
                return 1.4f * exp(-pow(weights * rsqrt(0.5f * width), width)) / (0.3f + 0.2f * width);
                }
           
               fixed2 RadialDistortion(fixed2 coord)
            {
                coord *= _TextureSize / _InputSize;
                fixed2 cc = coord - _curvatureSet1;
                fixed dist = dot(cc, cc) * _Distortion;
                return (coord + cc * (_curvatureSet2 + dist) * dist) * _InputSize / _TextureSize;
            }
 
 
       
            fixed4 frag(v2f IN) : SV_Target
            {
           
                _TextureSize = _ScanLineSize.xy;
                _Texcoord = IN.texcoord;
           
                _One = 1.0f / _TextureSize;
                _OutputSize = _TextureSize;
                _InputSize = _TextureSize;
                _Factor = _Texcoord.x * _TextureSize.x * _OutputSize.x / _InputSize.x;
               
             #ifdef USEDISTORTION_ON
                fixed2 xy = RadialDistortion(_Texcoord);
             #else
                fixed2 xy = _Texcoord;
             #endif
                fixed2 ratio = xy * _TextureSize - fixed2(0.5f, 0.5f);
                fixed2 uvratio = frac(ratio);
       
                xy.y = (floor(ratio.y) + _YExtra) / _TextureSize;
                fixed4 col = tex2D(_MainTex, xy) * IN.color;
                fixed4 col2 = tex2D(_MainTex, xy + fixed2(0.0f, _One.y)) * IN.color;
       
                fixed4 weights = ScanlineWeights(uvratio.y, col);
                fixed4 weights2 = ScanlineWeights(1.0f - uvratio.y, col2);
                fixed3 res = (col * weights + col2 * weights2).rgb;
             #ifdef USEINDIVIDUALCOLORCHANNELS1_ON
                fixed3 rgb1 = float3(_rgb1R, _rgb1G, _rgb1B);
             #else
                  fixed3 rgb1 = _ColorScanLine1;
              #endif
             
              #ifdef USEINDIVIDUALCOLORCHANNELS2_ON
                fixed3 rgb2 = float3(_rgb2R, _rgb2G, _rgb2B);
             #else
                  fixed3 rgb2 = _ColorScanLine2;
              #endif
       
                  fixed3 dotMaskWeights = lerp(rgb1, rgb2, floor(fmod(_Factor, _dotWeight)));
                  res *= dotMaskWeights;
               
                return fixed4(pow(res, fixed3(1.0f / _Gamma, 1.0f / _Gamma, 1.0f / _Gamma)), col.a) * col.a;
            //return float4(pow(res, float3(1.0f / ScreenGamma.x, 1.0f / ScreenGamma.y, 1.0f / ScreenGamma.z)), 1.0f);
            }
        ENDCG
        }
    }
}
