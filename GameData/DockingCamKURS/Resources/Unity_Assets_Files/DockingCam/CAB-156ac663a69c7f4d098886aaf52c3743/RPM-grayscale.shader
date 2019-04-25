Shader "RPM/Grayscale" {
Properties {
 _MainTex ("Render Input", 2D) = "white" { }
 _Gain ("Gain", Float) = 1.000000
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 39590
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
}