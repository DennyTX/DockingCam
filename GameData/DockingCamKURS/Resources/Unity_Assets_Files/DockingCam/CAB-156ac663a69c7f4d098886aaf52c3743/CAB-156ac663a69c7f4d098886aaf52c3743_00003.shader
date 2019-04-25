Shader "NightVisionEffectShader" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _VignetteTex ("Vignette Texture", 2D) = "white" { }
 _ScanLineTex ("Scan Line Texture", 2D) = "white" { }
 _NoiseTex ("Noise Texture", 2D) = "white" { }
 _NoiseXSpeed ("Noise X Speed", Float) = 100.000000
 _NoiseYSpeed ("Noise Y Speed", Float) = 100.000000
 _ScanLineTileAmount ("Scan Line Tile Amount", Float) = 4.000000
 _NightVisionColor ("Night Vision Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _Contrast ("Contrast", Range(0.000000,4.000000)) = 2.000000
 _Brightness ("Brightness", Range(0.000000,2.000000)) = 1.000000
 _RandomValue ("Random Value", Float) = 0.000000
 _distortion ("Distortion", Float) = 0.200000
 _scale ("Scale (Zoom)", Float) = 0.800000
}
SubShader { 
 Pass {
  CGPROGRAM
Program "vp" {
SubProgram "d3d9 " {
GpuProgramIndex 0
}
SubProgram "d3d11 " {
GpuProgramIndex 1
}
SubProgram "d3d11_9x " {
GpuProgramIndex 2
}
}
Program "fp" {
SubProgram "d3d9 " {
GpuProgramIndex 3
}
SubProgram "d3d11 " {
GpuProgramIndex 4
}
SubProgram "d3d11_9x " {
GpuProgramIndex 5
}
}
 }
}
Fallback Off
}
