Shader "Custom/OldTV" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _TimeX ("Time", Range(-0.400000,2.000000)) = 1.000000
 _Distortion ("_Distortion", Range(0.000000,1.000000)) = 0.300000
 _ScreenResolution ("_ScreenResolution", Vector) = (0.000000,0.000000,0.000000,0.000000)
}
SubShader { 
 Pass {
  ZTest Always
  GpuProgramID 57617
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