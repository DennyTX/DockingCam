Shader "Custom/NightVision" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Brightness ("Brightness Amount", Float) = 2.000000
 _Bright ("Bright Areas", Float) = 1.000000
}
SubShader { 
 Pass {
  Name "NIGHTVISION"
  GpuProgramID 49161
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