Shader "Custom/glass" {
 // 此效果可用于玻璃上面的附加文字，图案，比如透明显示器 //
 Properties{
  _Color ("Main Color", Color) = (1,1,1,1)
  _MainTex ("Base (RGB) Transparency(A)", 2D) = "white" {}
  _Reflections ("Base (RGB) Gloss(A)", Cube) = "skybox" {TexGen CubeReflect}
  
 }
 
 SubShader{
  Tags {"Queue" = "Transparent"}
  Pass{
   // 混合透明,让物体透明 //
   Blend SrcAlpha OneMinusSrcAlpha
   // 主要颜色 //
   Material{
    Diffuse [_Color]
   }
   Lighting On
   // 材质 //
   SetTexture [_MainTex] {
    combine texture * primary double, texture * primary
   }
  }
  
  Pass{
   // 镜面反射透明 //
   Blend One One
   // 主颜色可不加 //
   Material{
    Diffuse [_Color]
   }
   Lighting On
   // 反射天空色 //
   SetTexture [_Reflections]{
    combine texture
    Matrix [_Reflection]
   }
  }
 }
}