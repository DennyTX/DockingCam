Shader "Custom/Real_VHS_Nightvision" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 VHS ("Base (RGB)", 2D) = "white" { }
 VHS2 ("Base (RGB)", 2D) = "white" { }
 _Brightness ("Brightness Amount", Float) = 2.000000
 _Bright ("Bright Areas", Float) = 1.000000
}
SubShader { 
 Pass {
  ZTest Always
  GpuProgramID 17552
Program "vp" {
SubProgram "d3d9 " {
GpuProgramIndex 0
}
SubProgram "d3d11 " {
GpuProgramIndex 1
}
}
Program "fp" {
SubProgram "d3d9 " {
GpuProgramIndex 2
}
SubProgram "d3d11 " {
GpuProgramIndex 3
}
}
 }
}
}