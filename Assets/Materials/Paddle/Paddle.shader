Shader "Custom/Paddle"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Tint ("Tint Color", Color) = (1,1,1,1)
        _MixScale ("Mix Scale", Range(0, 1)) = 0
    }
     SubShader
    {
        CGPROGRAM
            #pragma surface surf Lambert
            
            #include "UnityCG.cginc"

            struct Input
            {
                float3 color;
            };
            
            float4 _BaseColor;
            float4 _Tint;
            float _MixScale;
            
            void surf(Input IN, inout SurfaceOutput o)
            {
                o.Albedo = (1 - _MixScale) * _BaseColor.rgb + _MixScale * _Tint.rgb; 
            }
            
        ENDCG
    }
    FallBack "Diffuse"
}
