Shader "Custom/SpriteWithTransparency"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        sampler2D _MainTex;
        fixed4 _Color;
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c1 = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c1.rgb;
            o.Alpha = c1.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
