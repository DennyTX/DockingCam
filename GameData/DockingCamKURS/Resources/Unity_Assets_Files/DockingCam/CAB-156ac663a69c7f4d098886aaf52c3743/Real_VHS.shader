Shader "CameraFilterPack/Real_VHS" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 VHS ("Base (RGB)", 2D) = "white" { }
 VHS2 ("Base (RGB)", 2D) = "white" { }
}
SubShader { 
 Pass {
  ZTest Always
  GpuProgramID 38067
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