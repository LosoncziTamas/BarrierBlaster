Shader "Custom/Dissolve"
{
   Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale", Range(0, 1)) = 1
		_Rotation ("Rotation", Range(0, 180)) = 0
		_HighlightIntensity("Highlight Intensity", Range(0, 1)) = 0
	}
	SubShader
	{
	
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		
        #include "UnityCG.cginc"
        
        struct appdata
        {
            float4 vertex: POSITION;
            float3 normal: NORMAL;
            float2 texcoord: TEXCOORD0;
        };
		
		struct Input
		{
		    float4 vertex;
		    float2 uv_MainTex;
		    float3 viewDir;
		};
		
		sampler2D _MainTex;
		sampler2D _NoiseTex;
		float _Scale;
		float _Rotation;
		float _HighlightIntensity;
		
		float4 RotateAroundYInDegrees(float4 vertex, float degrees)
         {
             float alpha = degrees * UNITY_PI / 180.0;
             float sina, cosa;
             sincos(alpha, sina, cosa);
             float2x2 m = float2x2(cosa, -sina, sina, cosa);
             return float4(mul(m, vertex.xz), vertex.yw).xzyw;
         }
		
		void vert(inout appdata v, out Input o)
        {
            // Initializes o to zero.
            UNITY_INITIALIZE_OUTPUT(Input, o);
            v.vertex = RotateAroundYInDegrees(v.vertex, _Rotation);
            v.vertex.xyz *= _Scale;
        }
		
		void surf(Input IN, inout SurfaceOutput o)
		{
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
		    o.Albedo += _HighlightIntensity * float4(1.0, 1.0, 1.0, 1.0f);
		}
		
		ENDCG
	}
    FallBack "Diffuse"
}
