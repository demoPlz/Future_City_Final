Shader "Custom/WorldSpace"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normal", 2D) = "white" {}
        _SpecColor ("Specular", Color) = (1, 1, 1, 1)
        _Blend ("Blend", Float) = 1
        _Cull ("Cull", Float) = 1
        _QueueOffset ("Queue", Float) = 1
        _Surface ("Surface", Float) = 1
        _AlphaClip ("Alpha Clip", Float) = 1
        _BumpScale ("NormalScale", Float) = 5
        _Smoothness ("Smoothness", Float) = 0.5
        _SpecularHighlights ("Specular Highlights", Float) = 1
        _Scale ("Tiling", Float) = 0.1
    }
    SubShader
    {
        Tags {
            "Queue" = "Geometry+2"
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
        }
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            sampler2D _MainTex;
            sampler2D _BumpMap;
            float4 _SpecColor;
            float _Blend;
            float _QueueOffset;
            float _Surface;
            float _AlphaClip;
            float _Smoothness;
            float _SpecularHighlights;
            float _BumpScale;
            float _Scale;
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                return o;
            }
            float4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.worldNormal);
                float3 blend = abs(n);
                blend /= (blend.x + blend.y + blend.z);
                float3 wp = i.worldPos * _Scale;
                float3 xProj = tex2D(_MainTex, wp.zy).rgb;
                float3 yProj = tex2D(_MainTex, wp.xz).rgb;
                float3 zProj = tex2D(_MainTex, wp.xy).rgb;
                float3 nxProj = tex2D(_BumpMap, wp.zy).rgb;
                float3 nyProj = tex2D(_BumpMap, wp.xz).rgb;
                float3 nzProj = tex2D(_BumpMap, wp.xy).rgb;
                float3 col = xProj * blend.x + yProj * blend.y + zProj * blend.z;
                float3 bump = nxProj * blend.x * _BumpScale + nyProj * blend.y * _BumpScale + nzProj * blend.z * _BumpScale;
                Light light = GetMainLight();
                float s = saturate(dot(normalize(bump + n), normalize(light.direction)));
                return float4(col * s, 1.0);
            }
            ENDHLSL
        }
    }
}